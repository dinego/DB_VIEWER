using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Units.Queries
{
    public class GetUnitsByFilterRequest : IRequest<IEnumerable<GetUnitsByFilterResponse>>
    {
        public long? TableId { get; set; }
        public long? GroupId { get; set; }
        public IEnumerable<long> CompanyIds { get; set; }
        public long ProjectId { get; set; }
    }

    public class GetUnitsByFilterResponse
    {
        public long UnitId { get; set; }
        public string Unit { get; set; }
    }

    public class GetUnitsByFilterHandler :
        IRequestHandler<GetUnitsByFilterRequest, IEnumerable<GetUnitsByFilterResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUnitsByFilterHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<GetUnitsByFilterResponse>> Handle(GetUnitsByFilterRequest request, CancellationToken cancellationToken)
        {
            List<GetUnitsByFilterResponse> result = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
            .Include("Empresa")
            .GetListAsync(x => x.Where(pme => pme.ProjetoId == request.ProjectId &&
                                       (!request.GroupId.HasValue || pme.GrupoProjetoSmidLocal == request.GroupId.Value) &&
                                       (!request.TableId.HasValue || pme.TabelaSalarialIdLocal == request.TableId.Value))
            ?.Select(s => new GetUnitsByFilterResponse
            {
                UnitId = s.EmpresaId,
                Unit = string.IsNullOrWhiteSpace(s.Empresa.NomeFantasia) ?
                (string.IsNullOrWhiteSpace(s.Empresa.RazaoSocial) ? string.Empty : s.Empresa.RazaoSocial) :
                s.Empresa.NomeFantasia
            }).Distinct().OrderBy(o => o.Unit));

            if (result.Safe().Count() > 1)
                result.Insert(0, new GetUnitsByFilterResponse
                {
                    Unit = "Todas",
                    UnitId = 0
                });
            return result;
        }
    }
}
