using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.UserParameters.Queries
{
    public class GetUsersRequest : IRequest<IEnumerable<GetUsersResponse>>
    {
        public List<long> UserCompanies { get; set; }
        public int Page { get; set; } = 1;
        public long UserId { get; set; }
        public int PageSize { get; set; } = 10;
    }

    public class GetUsersResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Area { get; set; }
        public string Photo { get; set; }
        public bool Active { get; set; }
        public DateTime? LastAccess { get; set; }
    }
    public class GetUsersHandler : IRequestHandler<GetUsersRequest, IEnumerable<GetUsersResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetUsersHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GetUsersResponse>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.GetRepository<Usuarios, long>()
                .Include("Empresa")
                .Include("Departamento")
                .Include("UsuarioPermissaoSm")
                .Include("Empresa.ProjetosSalaryMarkEmpresas")
                .Include("ProdutoUsuarios")
                .GetListAsync(x => x.Where(e => e.Status.HasValue && e.Status.Value &&
                                                e.Id != request.UserId &&    
                                                e.ProdutoUsuarios.Any(a => a.ProdutoId == (int)ProductTypesEnum.SM) &&
                                                e.UsuarioPermissaoSm.Any() &&
                                                e.Empresa != null && e.Empresa.ProjetosSalaryMarkEmpresas.Any(emp=> request.UserCompanies.Contains(emp.EmpresaId)))
                .Select(s => new GetUsersResponse
                {
                    Id = s.Id,
                    Name = s.Nome,
                    Mail = s.Email,
                    Photo = s.FotoPerfil,
                    LastAccess = s.UltimoAcesso,
                    Active = s.Status.HasValue && s.Status.Value,
                    Area = s.Departamento != null ? s.Departamento.Departamento : string.Empty
                })
                .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
                .OrderBy(o => o.Active).ThenBy(t => t.Name));

            return users;
        }
    }
}
