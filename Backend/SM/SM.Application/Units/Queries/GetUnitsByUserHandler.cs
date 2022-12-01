using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Units.Queries
{
    public class GetUnitsByUserRequest : IRequest<IEnumerable<GetUnitsByUserResponse>>
    {
        public IEnumerable<long> CompanyIds { get; set; }
    }
    
    public class GetUnitsByUserResponse
    {
        public long UnitId { get; set; }
        public string Unit { get; set; }
    }

    public class GetUnitsByUserHandler :
        IRequestHandler<GetUnitsByUserRequest, IEnumerable<GetUnitsByUserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUnitsByUserHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<GetUnitsByUserResponse>> Handle(GetUnitsByUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.GetRepository<ProjetosSalaryMarkEmpresas, long>()
                .Include("Empresa")
                .GetListAsync(x => x.Where(pme => request.CompanyIds.Contains(pme.EmpresaId))
                ?.Select(s => new GetUnitsByUserResponse
                {
                    UnitId = s.EmpresaId,
                    Unit = string.IsNullOrWhiteSpace(s.Empresa.NomeFantasia) ?
                    (string.IsNullOrWhiteSpace(s.Empresa.RazaoSocial) ? string.Empty : s.Empresa.RazaoSocial) :
                    s.Empresa.NomeFantasia
                }).
                OrderBy(o=> o.Unit));

            if (!result.Any())
                throw new Exception("Nenhuma unidade foi encontrada");
            return result;
        }
    }
}
