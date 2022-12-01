using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.UserParameters.Queries
{
    public class CanAccessUsersRequest : IRequest<bool>
    {
        public long UserId { get; set; }
        public List<long> UserCompanies { get; set; }
    }
    public class CanAccessUsersHandler : IRequestHandler<CanAccessUsersRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CanAccessUsersHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CanAccessUsersRequest request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.GetRepository<Usuarios, long>()
                .Include("Empresa")
                .Include("UsuarioPermissaoSm")
                .Include("Empresa.ProjetosSalaryMarkEmpresas")
                .Include("ProdutoUsuarios")
                .GetListAsync(x => x.Where(e => e.Status.HasValue && e.Status.Value &&
                                                e.Id != request.UserId &&
                                                e.ProdutoUsuarios.Any(a => a.ProdutoId == (int)ProductTypesEnum.SM) &&
                                                e.UsuarioPermissaoSm.Any() &&
                                                e.Empresa != null && e.Empresa.ProjetosSalaryMarkEmpresas.Any(emp => request.UserCompanies.Contains(emp.EmpresaId)))
                .Select(s => s.Id));

            return users.Safe().Any();
        }
    }
}
