using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Common.Queries
{
    public class CommonParametersRequest : IRequest<IEnumerable<GetCommonParametersResponse>>
    {
        public long ProjectId { get; set; }
    }

    public class GetCommonParametersResponse
    {
        public long ParameterId { get; set; }
        public IEnumerable<ParametersResponse> Parameters { get; set; }
    }

    public class ParametersResponse
    {
        public string Title { get; set; }

        public long Id { get; set; }
    }

    public class GetCommonParametersHandler : IRequestHandler<CommonParametersRequest, IEnumerable<GetCommonParametersResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validatorResponse;
        public GetCommonParametersHandler(IUnitOfWork unitOfWork,
            ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _validatorResponse = validatorResponse;
        }
        public async Task<IEnumerable<GetCommonParametersResponse>> Handle(CommonParametersRequest request, CancellationToken cancellationToken)
        {
            var parameters = new List<long> { (long)ParametersProjectsTypes.Area, (long)ParametersProjectsTypes.ParameterOne, (long)ParametersProjectsTypes.ParameterTwo, (long)ParametersProjectsTypes.ParameterThree };

            var commonParameters = await _unitOfWork.GetRepository<ParametrosProjetosSMLista, long>()
                                    .Include("CargosProjetoSMParametrosMapeamento")
                                    .GetListAsync(x => x.Where(pp => pp.ProjetoId == request.ProjectId &&
                                                            parameters.Contains(pp.ParametroSMTipoId))
                                               .Select(res => new
                                               {
                                                   Id = res.Id,
                                                   ParameterId = res.ParametroSMTipoId,
                                                   Title = res.ParametroProjetoSMLista
                                               }));
            if (commonParameters == null)
                return new List<GetCommonParametersResponse>();

            var result = commonParameters.GroupBy(g => g.ParameterId, (key, value) => new GetCommonParametersResponse
            {
                ParameterId = key,
                Parameters = value.Select(res => new ParametersResponse
                {
                    Id = res.Id,
                    Title = res.Title
                }).OrderBy(o => o.Title)
            });
            return result;
        }
    }
}