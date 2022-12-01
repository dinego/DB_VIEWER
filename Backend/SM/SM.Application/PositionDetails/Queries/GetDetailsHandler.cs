using CMC.Common.Repositories;
using MediatR;
using SM.Application.PositionDetails.Queries.Response;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Domain.Entities;
using System.Linq;
using System.Collections.Generic;
using SM.Domain.Enum;
using SM.Application.Interactors.Interfaces;
using CMC.Common.Extensions;
using SM.Domain.Common;
using SM.Application.Interactors;

namespace SM.Application.TableSalary.Queries
{
    public class GetDetailsRequest : IRequest<DetailsResponse>
    {
        public long ProjectId { get; set; }
        public long PositionId { get; set; }
        public long UserId { get; set; }

    }

    public class GetDetailsHandler : IRequestHandler<GetDetailsRequest, DetailsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGetGlobalLabelsInteractor _globalLabelsInteractor;
        private readonly IGetUserLocalLabelsInteractor _getUserLocalLabelsInteractor;

        public GetDetailsHandler(IUnitOfWork unitOfWork,
            IGetGlobalLabelsInteractor globalLabelsInteractor,
            IGetUserLocalLabelsInteractor getUserLocalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _globalLabelsInteractor = globalLabelsInteractor;
            _getUserLocalLabelsInteractor = getUserLocalLabelsInteractor;
        }
        public async Task<DetailsResponse> Handle(GetDetailsRequest request, CancellationToken cancellationToken)
        {
            var globalLabels = await _globalLabelsInteractor.Handler(request.ProjectId);
            var userLabels = await _getUserLocalLabelsInteractor.Handler(request.UserId);
            var position = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                                      .Include("CargosProjetoSMParametrosMapeamento")
                                      .Include("CargosProjetoSMEixoCarreiraMapeamentos")
                                      .GetAsync(x => x.Where(c => c.CargoProjetoSmidLocal == request.PositionId &&
                                                                 c.ProjetoId == request.ProjectId)
                                                    .Select(res => new PositionSMDetails
                                                    {
                                                        Header = new PositionHeader
                                                        {
                                                            PositionId = res.CargoProjetoSmidLocal,
                                                            Position = res.CargoSm,
                                                            CMCode = res.Cmcode,
                                                            SMCode = res.Smcode,
                                                            GroupId = res.GrupoSmidLocal,
                                                            LevelId = res.NivelId,
                                                        },
                                                        ProjectParameterIds = res.CargosProjetoSMParametrosMapeamento.Select(cp => cp.ParametroProjetoSMId),
                                                        CareerAxisIds = res.CargosProjetoSMEixoCarreiraMapeamentos.Where(ca => ca.ProjetoId == request.ProjectId &&
                                                                                                                          ca.CargoProjetoSmidLocal == request.PositionId)
                                                                                                                   .Select(cres => cres.EixoCarreiraId)
                                                    }));

            position.Header.PositionSalaryMarkLabel = globalLabels.Any() ?
                globalLabels.FirstOrDefault(gl => gl.Id == (long)GlobalLabelEnum.PositionSalaryMark).Alias :
                GlobalLabelEnum.PositionSalaryMark.GetDescription();

            var projectParametersId = position.ProjectParameterIds.ToList();

            var parameterList = await _unitOfWork.GetRepository<ParametrosProjetosSMLista, long>()
                                              .GetListAsync(x => x.Where(ppl => projectParametersId.Contains(ppl.Id))
                                              .Select(res => new ParameterListDetails
                                              {
                                                  ProjectParameterId = res.Id,
                                                  ProjectParameter = res.ParametroProjetoSMLista,
                                                  ParameterId = res.ParametroSMTipoId
                                              }).OrderBy(o => o.ParameterId));

            var projectCareersId = position.CareerAxisIds.ToList();

            var carrierAxisList = await _unitOfWork.GetRepository<EixoCarreiraSM, long>()
                                              .GetListAsync(x => x.Where(ppl => projectCareersId.Contains(ppl.Id))
                                              .Select(res => new CareerAxisDetails
                                              {
                                                  CareerAxisId = res.Id,
                                                  CareerAxis = res.EixoCarreira,
                                                  ParentParameterId = res.ParametroProjetoSMListaId
                                              }).OrderBy(o => o.CareerAxisId));

            var parameters = new List<ParameterPosition>();

            var parametersId = parameterList.Select(res => res.ParameterId).Distinct().ToList();

            parametersId.ForEach(parameter =>
            {
                switch (parameter)
                {
                    case (long)ParametersProjectsTypes.Area:
                        var areaData = PrepareData(ParametersProjectsTypes.Area, GlobalLabelEnum.Area, globalLabels, userLabels, parameterList, InternalLabelsEnum.Area);
                        if (areaData != null)
                            parameters.Add(areaData);
                        if (carrierAxisList.Safe().Any())
                        {
                            var parameterCareerAxis = PrepareCareerAxisData(carrierAxisList);
                            if (parameterCareerAxis != null)
                                parameters.Add(parameterCareerAxis);
                        }
                        break;
                    case (long)ParametersProjectsTypes.ParameterOne:
                        var parameterOneData = PrepareData(ParametersProjectsTypes.ParameterOne, GlobalLabelEnum.Parameter1, globalLabels, userLabels, parameterList, InternalLabelsEnum.ParameterOne);
                        if (parameterOneData != null)
                            parameters.Add(parameterOneData);
                        break;
                    case (long)ParametersProjectsTypes.ParameterTwo:
                        var parameterTwoData = PrepareData(ParametersProjectsTypes.ParameterTwo, GlobalLabelEnum.Parameter2, globalLabels, userLabels, parameterList, InternalLabelsEnum.ParameterTwo);
                        if (parameterTwoData != null)
                            parameters.Add(parameterTwoData);
                        break;
                    case (long)ParametersProjectsTypes.ParameterThree:
                        var parameterThreeData = PrepareData(ParametersProjectsTypes.ParameterThree, GlobalLabelEnum.Parameter3, globalLabels, userLabels, parameterList, InternalLabelsEnum.ParameterThree);
                        if (parameterThreeData != null)
                            parameters.Add(parameterThreeData);
                        break;
                }
            });

            var result = new DetailsResponse
            {
                Header = position.Header,
                Parameters = parameters
            };
            return result;
        }

        private ParameterPosition PrepareData(
            ParametersProjectsTypes parameterId,
            GlobalLabelEnum globalLabelEnum,
            IEnumerable<GlobalLabelsJson> globalLabels,
            IEnumerable<GetUserLocalLabelsResponse> userLabels,
            List<ParameterListDetails> parameterList,
            InternalLabelsEnum? internalLabelsEnum = null)
        {
            var parameters = parameterList.Where(pl => pl.ParameterId == (long)parameterId).ToList();
            var globaLabel = globalLabels.FirstOrDefault(gl => gl.Id == (long)globalLabelEnum);
            var userLabel = internalLabelsEnum.HasValue ?
                            userLabels.FirstOrDefault(ul => ul.InternalCode == (long)internalLabelsEnum) : null;
            string label = userLabel != null ? userLabel.Label : globaLabel != null ? globaLabel.Alias : globalLabelEnum.GetDescription();
            var parameterPosition = new ParameterPosition
            {
                ParameterId = (long)parameterId,
                Title = label,
                ProjetParameters = new List<ProjectParameterPosition>()
            };
            parameters.ForEach(p =>
            {
                var projectParameter = new ProjectParameterPosition
                {
                    Id = p.ProjectParameterId,
                    Title = p.ProjectParameter
                };
                parameterPosition.ProjetParameters.Add(projectParameter);
            });
            return parameterPosition;
        }

        private ParameterPosition PrepareCareerAxisData(List<CareerAxisDetails> careerAxisList)
        {
            var parameterPosition = new ParameterPosition
            {
                ParameterId = (long)ParametersProjectsTypes.CarreiraEixo,
                Title = GlobalLabelEnum.CareerAxis.GetDescription(),
                ProjetParameters = new List<ProjectParameterPosition>()
            };
            careerAxisList.ForEach(p =>
            {
                var projectParameter = new ProjectParameterPosition
                {
                    Id = p.CareerAxisId,
                    Title = p.CareerAxis,
                    ParentParameterId = p.ParentParameterId
                };
                parameterPosition.ProjetParameters.Add(projectParameter);
            });
            return parameterPosition;
        }
    }
}
