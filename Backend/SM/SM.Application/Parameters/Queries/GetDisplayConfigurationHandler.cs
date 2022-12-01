using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Application.Parameters.Queries.Response;
using SM.Application.Parameters.Validators;
using SM.Domain.Common;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Parameters.Queries
{
    public class GetDisplayConfigurationRequest : IRequest<GetDisplayConfigurationResponse>
    {
        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class GetDisplayConfigurationHandler : IRequestHandler<GetDisplayConfigurationRequest, GetDisplayConfigurationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionCompanyInteractor _permissionCompanyInteractor;
        private readonly DisplayConfigurationRule _validateDisplay;
        private readonly DisplayConfigurationModulesRule _validatorModules;
        private readonly ValidatorResponse _validator;
        public GetDisplayConfigurationHandler(IUnitOfWork unitOfWork,
        IPermissionCompanyInteractor permissionCompanyInteractor,
        ValidatorResponse validator)
        {
            _unitOfWork = unitOfWork;
            _permissionCompanyInteractor = permissionCompanyInteractor;
            _validator = validator;
            _validateDisplay = new DisplayConfigurationRule(_validator);
            _validatorModules = new DisplayConfigurationModulesRule(_validator);
        }

        public async Task<GetDisplayConfigurationResponse> Handle(GetDisplayConfigurationRequest request, CancellationToken cancellationToken)
        {
            var result = new GetDisplayConfigurationResponse
            {
                DisplayConfiguration = new DisplayConfigurationResponse { DisplayTypes = new List<DisplayTypeResponse>() },
                Preference = new PreferenceResponse { GlobalLabels = new List<GlobalLabelResponse>() }
            };
            if (!_validateDisplay.IsSatisfiedBy(request.Map().ToANew<DisplayConfigurationValidator>()))
                return result;
            var userPermission = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
                .GetAsync(x => x.Where(up => up.UsuarioId == request.UserId)
                .Select(res => new
                {
                    Permission = res.ConfiguraExibicaoAdmin,
                    GlobalLabels = res.RotulosGlobaisAdmin
                }));

            var userPermissionSaved = new PermissionJson();
            userPermission.Permission.TryParseJson(out userPermissionSaved);

            var validatorModule = new DisplayConfigurationModuleValidator { Modules = userPermission != null ? userPermissionSaved.Modules.Map().ToANew<List<FieldCheckedUserJson>>() : null };
            if (userPermission == null || !_validatorModules.IsSatisfiedBy(validatorModule))
                return result;
            var companyPermission = await _permissionCompanyInteractor.Handler(request.ProjectId);

            var lstDisplayTypes = await PrepareDisplayConfiguration(userPermissionSaved, companyPermission.Permission, validatorModule);
            var lstGlobalLabels = PreparePreferences(request.UserId, userPermission.GlobalLabels, companyPermission.GlobalLabels, validatorModule);

            result.DisplayConfiguration.DisplayTypes.AddRange(lstDisplayTypes);
            result.Preference.GlobalLabels.AddRange(lstGlobalLabels);

            return result;
        }

        private async Task<List<DisplayTypeResponse>> PrepareDisplayConfiguration(
            PermissionJson userPermission,
            PermissionJson companyPermission,
            DisplayConfigurationModuleValidator validator)
        {
            var displayTypes = new List<DisplayTypeResponse>();
            validator.SubModules = ModulesSuItemsEnum.DisplayConfiguration;
            if (!_validatorModules.IsSatisfiedBy(validator))
                return displayTypes;

            #region TYPE OF CONTRACT
            var typeOfContract = await _unitOfWork.GetRepository<TipoDeContrato, long>()
                .GetListAsync(x => x.Where(ms => ms.Ativo.HasValue &&
                                           ms.Ativo.Value &&
                                           !companyPermission.TypeOfContract.Safe().Contains(ms.Id))
                .Select(s => new DisplayItemResponse
                {
                    Id = s.Id,
                    Name = s.Nome,
                    IsChecked = !userPermission.TypeOfContract.Any(tc => tc == s.Id)
                }
               ));
            if (typeOfContract.Safe().Any())
            {
                var typeOfContractResult = new DisplayTypeResponse
                {
                    Id = (int)ParameterDisplay.TypeOfContract,
                    Name = ParameterDisplay.TypeOfContract.GetDescription(),
                    SubItems = typeOfContract
                };
                displayTypes.Add(typeOfContractResult);
            }

            #endregion

            #region HOURLY BASIS
            var hourlyBasis = await _unitOfWork.GetRepository<BaseDeDados, long>()
                .GetListAsync(x => x.Where(ms => ms.Status.HasValue &&
                                           ms.Status.Value &&
                                           !companyPermission.DataBase.Safe().Contains(ms.Id))
                .Select(s => new DisplayItemResponse
                {
                    Id = s.Id,
                    Name = s.Nome,
                    IsChecked = !userPermission.DataBase.Any(hr => hr == s.Id)
                }
               ));
            if (hourlyBasis.Safe().Any())
            {
                var hourlyBasisResult = new DisplayTypeResponse
                {
                    Id = (int)ParameterDisplay.HourlyBasis,
                    Name = ParameterDisplay.HourlyBasis.GetDescription(),
                    SubItems = hourlyBasis
                };
                displayTypes.Add(hourlyBasisResult);
            }

            #endregion

            #region SCENARIOS
            if (companyPermission.Display != null &&
                companyPermission.Display.Scenario.Safe().Any(sc => sc.IsChecked))
            {
                var companyScenarios = companyPermission.Display.Scenario.Where(sc => sc.IsChecked).Select(res => res.Id);
                var scenarios = new DisplayTypeResponse
                {
                    Id = (int)ParameterDisplay.Scenario,
                    Name = ParameterDisplay.Scenario.GetDescription(),
                    SubItems = userPermission.Display != null && userPermission.Display.Scenario.Safe().Any() ?
                               userPermission.Display.Scenario.Where(sc => companyScenarios.Contains(sc.Id))
                                                        .Select(res => new DisplayItemResponse
                                                        {
                                                            Id = res.Id,
                                                            Name = res.Name,
                                                            IsChecked = res.IsChecked
                                                        }) : new List<DisplayItemResponse>()
                };
                displayTypes.Add(scenarios);
            }
            #endregion

            #region PERSONS
            var personDisplays = new List<DisplayItemResponse>();
            if (companyPermission.Permission.Any(p => p.Id == (long)PermissionItensEnum.InactivePerson &&
                                                 p.IsChecked))
                personDisplays.Add(new DisplayItemResponse
                {
                    Id = (long)PermissionItensEnum.InactivePerson,
                    Name = PermissionItensEnum.InactivePerson.GetDescription(),
                    IsChecked = !userPermission.Permission.Safe().Any(p => p.Id == (long)PermissionItensEnum.InactivePerson &&
                                                 !p.IsChecked)
                });
            if (!companyPermission.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions))
                personDisplays.Add(new DisplayItemResponse
                {
                    Id = (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions,
                    Name = ContentsSubItemsEnum.ShowPeopleWithoutPositions.GetDescription(),
                    IsChecked = !userPermission.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions)
                });
            if (personDisplays.Safe().Any())
            {
                var persons = new DisplayTypeResponse
                {
                    Id = (int)ParameterDisplay.Person,
                    Name = ParameterDisplay.Person.GetDescription(),
                    SubItems = personDisplays
                };
                displayTypes.Add(persons);
            }
            #endregion

            #region ANALYSIS
            if (!companyPermission.Permission.Any(c => c.Id == (long)PermissionItensEnum.MoveNextStep))
            {
                var analysisDisplay = new DisplayTypeResponse
                {
                    Id = (int)ParameterDisplay.Analysis,
                    Name = ParameterDisplay.Analysis.GetDescription(),
                    SubItems = new List<DisplayItemResponse>
                    {
                        new DisplayItemResponse
                        {
                            Id = (long)PermissionItensEnum.MoveNextStep,
                            Name = PermissionItensEnum.MoveNextStep.GetDescription(),
                            IsChecked = !userPermission.Permission.Safe().Any(c => c.Id == (long)PermissionItensEnum.MoveNextStep)
                        }
                    }
                };
                displayTypes.Add(analysisDisplay);
            }
            #endregion

            #region TECNICAL ADJUST
            if (!companyPermission.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.ShowTecnicalAdjust))
            {
                var tecnicalAdjustDisplay = new DisplayTypeResponse
                {
                    Id = (int)ParameterDisplay.TecnicalAdjust,
                    Name = ParameterDisplay.TecnicalAdjust.GetDescription(),
                    SubItems = new List<DisplayItemResponse>{
                        new DisplayItemResponse{
                            Id = (long)ContentsSubItemsEnum.ShowTecnicalAdjust,
                            Name = ContentsSubItemsEnum.ShowTecnicalAdjust.GetDescription(),
                            IsChecked = !userPermission.Contents.Safe().Any(c => c.Id == (long)ContentsSubItemsEnum.ShowTecnicalAdjust)
                        }
                    }
                };
                displayTypes.Add(tecnicalAdjustDisplay);
            }
            #endregion
            return displayTypes;
        }

        private List<GlobalLabelResponse> PreparePreferences(long userId,
        string globalLabelsJson,
        List<GlobalLabelsJson> companyGlobalLabels,
        DisplayConfigurationModuleValidator validator)
        {
            validator.SubModules = ModulesSuItemsEnum.GlobalLabels;
            if (!_validatorModules.IsSatisfiedBy(validator))
                return new List<GlobalLabelResponse>();

            var globalLabelToIgnoreDefault = new List<long> {
                    (long)GlobalLabelEnum.PositionSalaryMark,
                    (long)GlobalLabelEnum.GSM,
                    (long)GlobalLabelEnum.Level
                };

            if (string.IsNullOrEmpty(globalLabelsJson))
                return new List<GlobalLabelResponse>();

            var userGlobalLabel = new List<GlobalLabelsJson>();
            globalLabelsJson.TryParseJson(out userGlobalLabel);
            var defaultLabels = userGlobalLabel.Where(gl => gl.IsChecked &&
                                !globalLabelToIgnoreDefault.Contains(gl.Id))
                                .Select(res => res.Id);
            var globalLabelsDisabled = new List<long> { (long)GlobalLabelEnum.Profile, (long)GlobalLabelEnum.CareerAxis };

            var result = companyGlobalLabels.Where(gl => defaultLabels.Contains(gl.Id)).Map().ToANew<List<GlobalLabelResponse>>();
            result.ForEach(gl =>
            {
                gl.Disabled = globalLabelsDisabled.Contains(gl.Id);
            });
            return result;
        }
    }
}

