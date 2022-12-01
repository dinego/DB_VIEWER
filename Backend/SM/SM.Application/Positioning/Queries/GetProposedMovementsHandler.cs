using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Helpers;
using SM.Application.Interactors.Interfaces;
using SM.Application.Share.Queries;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using SM.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Positioning.Queries
{

    public class GetProposedMovementsRequest : IRequest<GetProposedMovementsResponse>
    {
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
        public IEnumerable<object> CategoriesExp { get; set; } = new List<object>(); //using to share (EXP)
    }

    public class GetProposedMovementsResponse
    {
        public IEnumerable<CategoriesProposedMovementsResponse> Chart { get; set; }
        public ShareProposedMovementsResponse Share { get; set; }

        public IEnumerable<FilterResultProposedMovementsResponse> Categories { get; set; }
    }

    public class FilterResultProposedMovementsResponse
    {
        public string Name { get; set; }
        public string Id { get; set; }

    }

    public class ShareProposedMovementsResponse
    {
        public string User { get; set; } = null;
        public DateTime? Date { get; set; } = null;
        public string Unit { get; set; }
        public long? UnitId { get; set; }
        public string DisplayBy { get; set; }
        public int DisplayById { get; set; }
        public string Scenario { get; set; }
        public int ScenarioId { get; set; }
        public PermissionShared Permissions { get; set; }
    }

    public class CategoriesProposedMovementsResponse
    {
        public string Name { get; set; }
        public ProposedMovementsEnum Type { get; set; }
        public IEnumerable<DataProposedMovementsResponse> Data { get; set; }

    }

    public class DataProposedMovementsResponse
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public ClickProposedMovementsResponse Click { get; set; }
    }

    public class ClickProposedMovementsResponse
    {
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public object CategoryId { get; set; }
        public ProposedMovementsEnum SerieId { get; set; }
    }

    public class ResultListGroupByProposedMovementsDTO
    {

        public string Name { get; set; }
        public string Id { get; set; }
        public IEnumerable<BaseSalaryProposedMovementsDTO> Values { get; set; }
    }
    public class BaseSalaryProposedMovementsDTO
    {
        public int? InternalFrequency { get; set; }
        public long CompanyId { get; set; }
        public long ProfileId { get; set; }
        public long LevelId { get; set; }
        public string Parameter { get; set; }
        public long GSM { get; set; }
        public ProposedMovementsEnum ProposedMovements { get; set; }
        public string PositionSalaryBase { get; set; }
        public long PositionId { get; set; }
        public long? PositionIdDefault { get; set; }

    }

    public class PositionSalaryProposedMovementsDTO
    {
        public long GroupId { get; set; }
        public string Parameter { get; set; }
        public long GSM { get; set; }
        public long PositionId { get; set; }
        public string PositionSM { get; set; }
        public int? LevelId { get; set; }
    }

    public class ResultProposedMovementsDTO
    {
        public double EmployeeAmount { get; set; } = 0;
        public double EmployeePercentage { get; set; } = 0;
        public ProposedMovementsEnum ProposedMovement { get; set; }
        public string ItemGrouped { get; set; }
        public object CategoryId { get; set; }
    }


    public class GetProposedMovementsHandler
        : IRequestHandler<GetProposedMovementsRequest, GetProposedMovementsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IEnumerable<ProposedMovementsEnum> _listEnum;

        public GetProposedMovementsHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _listEnum = Enum.GetValues(typeof(ProposedMovementsEnum)) as
                IEnumerable<ProposedMovementsEnum>;

            _listEnum = _listEnum.First().GetWithOrder().Select(s =>
            (ProposedMovementsEnum)Enum.Parse(typeof(ProposedMovementsEnum), s));
        }
        public async Task<GetProposedMovementsResponse> Handle(GetProposedMovementsRequest request, CancellationToken cancellationToken)
        {
            var result = await GetResultConsolidated(request);

            if (request.DisplayBy == DisplayByPositioningEnum.ProfileId)
                return FitData(request, await Profile(request, result));
            if (request.DisplayBy == DisplayByPositioningEnum.LevelId)
                return FitData(request, await Level(request, result));
            if (request.DisplayBy == DisplayByPositioningEnum.Area ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter01 ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter02 ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter03)
                return FitData(request, Parameter(request, result));

            return FitData(request, await Profile(request, result));

        }

        private async Task<List<ResultListGroupByProposedMovementsDTO>> Profile(GetProposedMovementsRequest request,
                    List<BaseSalaryProposedMovementsDTO> result)
        {
            var profiles = result
                        .GroupBy(g => g.ProfileId, (key, value) => new { key, value });

            //filter
            if (request.CategoriesExp.Any())
            {
                var filter = request.CategoriesExp.Select(s => Convert.ToInt64(s));

                profiles = profiles.Where(f => !filter.Contains(f.value.FirstOrDefault().ProfileId));
            }

            var groupsIds = profiles.Select(s => s.key);

            var groupsProfiles = await _unitOfWork.GetRepository<GruposProjetosSalaryMark, long>()
                .GetListAsync(x => x.Where(g => groupsIds.Contains(g.GruposProjetosSmidLocal) &&
                 request.ProjectId == g.ProjetoId)?.
                Select(s => new
                {
                    Id = s.GruposProjetosSmidLocal,
                    Name = s.GrupoSm
                }));


            return profiles
                .OrderBy(o => o.key)
                .Select(s => new ResultListGroupByProposedMovementsDTO
                {
                    Name = groupsProfiles.FirstOrDefault(f => f.Id == s.key)?.Name,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().ProfileId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name))
            .ToList();
        }
        private async Task<List<ResultListGroupByProposedMovementsDTO>> Level(GetProposedMovementsRequest request,
                        List<BaseSalaryProposedMovementsDTO> result)
        {

            var groupsIds = result.Select(s => s.LevelId).Distinct();

            var groupsLevels = await _unitOfWork.GetRepository<NivelEmpresas, long>()
                        .GetListAsync(x => x.Where(g => groupsIds.Contains(g.NivelId) &&
                                    request.CompaniesId.Contains(g.EmpresaId) &&
                                    (!request.UnitId.HasValue || g.EmpresaId == request.UnitId.Value))?.
                        Select(s => new
                        {
                            Id = s.NivelId,
                            Name = s.NivelEmpresa
                        }));

            var levels = result
                        .GroupBy(g => groupsLevels.FirstOrDefault(f => f.Id == g.LevelId)?.Name, (key, value) => new { key, value });

            //filter
            if (request.CategoriesExp.Any())
            {
                var filter = request.CategoriesExp.Select(s => (long)s);

                levels = levels.Where(f => !filter.Contains(f.value.FirstOrDefault().LevelId));
            }

            return levels
                .OrderBy(o => o.value.FirstOrDefault().LevelId)
                .Select(s => new ResultListGroupByProposedMovementsDTO
                {
                    Name = s.key,
                    Id = s.value.FirstOrDefault().LevelId.ToString(),
                    Values = s.value
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name))
            .ToList();
        }
        private List<ResultListGroupByProposedMovementsDTO> Parameter(GetProposedMovementsRequest request,
            List<BaseSalaryProposedMovementsDTO> result)
        {
            var areas = result
                        .GroupBy(g => g.Parameter, (key, value) =>
                        new ResultListGroupByProposedMovementsDTO
                        {
                            Name = key,
                            Id = key,
                            Values = value
                        });
            //filter
            if (request.CategoriesExp.Any())
            {
                var filter = request.CategoriesExp.Select(s => (string)s);

                areas = areas.Where(f => !filter.Contains(f.Name));
            }

            return areas.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();

        }

        private GetProposedMovementsResponse FitData(GetProposedMovementsRequest request,
            IEnumerable<ResultListGroupByProposedMovementsDTO> resultList)
        {
            if (!resultList.Any())
                return new GetProposedMovementsResponse();

            var resultProposedMovementsDTOList = new List<ResultProposedMovementsDTO>();
            var employeeAmountTotalTotal = new List<double>();
            var costTotalTotal = new List<double>();

            foreach (var itemResult in resultList)
            {
                var values = itemResult.Values;

                var employeeAmountTotal = values
                    .Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0)));
                employeeAmountTotalTotal.Add(employeeAmountTotal);

                foreach (ProposedMovementsEnum itemEnum in _listEnum)
                {

                    var employeeAmount = values
                        .Where(s => s.ProposedMovements == itemEnum)?
                        .Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0)));

                    resultProposedMovementsDTOList.Add(new ResultProposedMovementsDTO
                    {
                        ProposedMovement = itemEnum,
                        EmployeeAmount = Math.Round(employeeAmount.GetValueOrDefault(0), 0),
                        EmployeePercentage = employeeAmountTotal == 0 ? 0 :
                        Math.Round(employeeAmount.GetValueOrDefault(0) / employeeAmountTotal * 100, 0),
                        ItemGrouped = itemResult.Name,
                        CategoryId = ConverterObject.TryConvertToInt(itemResult.Id)
                    });

                }

            }

            var categories = resultList.Map().ToANew<IEnumerable<FilterResultProposedMovementsResponse>>().ToList();

            resultProposedMovementsDTOList =
                AddAll(request, resultProposedMovementsDTOList, employeeAmountTotalTotal, categories);


            //fit response 
            return new GetProposedMovementsResponse
            {
                Categories = categories,
                Chart = resultProposedMovementsDTOList.GroupBy(g => g.ProposedMovement,
                (key, value) => new CategoriesProposedMovementsResponse
                {
                    Name = key.GetDescription(),
                    Type = value.FirstOrDefault().ProposedMovement,
                    Data = value.Select(s => new DataProposedMovementsResponse
                    {
                        Name = s.ItemGrouped,
                        Value = s.EmployeePercentage,
                        Click = new ClickProposedMovementsResponse
                        {
                            CategoryId = s.CategoryId,
                            DisplayBy = request.DisplayBy,
                            Scenario = request.Scenario,
                            UnitId = request.UnitId,
                            SerieId = key
                        }
                    })
                })
            };
        }
        private List<ResultProposedMovementsDTO> AddAll(GetProposedMovementsRequest request, List<ResultProposedMovementsDTO> resultProposedMovementsDTOList, List<double> employeeAmountTotalTotal, List<FilterResultProposedMovementsResponse> categories)
        {
            var isNumeric = Regex.IsMatch(categories.FirstOrDefault().Id, @"\d");
            var allLabel = isNumeric ? ((int)FilterAllEnum.All).ToString() : FilterAllEnum.All.ToString();

            if (!request.CategoriesExp.Any(a => a.ToString().Equals(allLabel)))
            {
                //add Total 
                var sumemployeeAmountTotal = employeeAmountTotalTotal.Sum();

                var totalResultProposedMovementsDTOList = new List<ResultProposedMovementsDTO>();
                foreach (ProposedMovementsEnum itemEnum in _listEnum)
                {
                    var filterByProposedMovement =
                        resultProposedMovementsDTOList.Where(s => s.ProposedMovement == itemEnum);

                    totalResultProposedMovementsDTOList.Add(new ResultProposedMovementsDTO
                    {

                        ProposedMovement = itemEnum,
                        EmployeeAmount = filterByProposedMovement.Sum(s => s.EmployeeAmount),
                        EmployeePercentage = sumemployeeAmountTotal == 0 ? 0 :
                        Math.Round(100 * (filterByProposedMovement.Sum(s => s.EmployeeAmount) / sumemployeeAmountTotal), 0),
                        ItemGrouped = "Todos",
                        CategoryId = ConverterObject.TryConvertToInt(allLabel)
                    });
                }

                //merge total + the other values 
                resultProposedMovementsDTOList =
                   resultProposedMovementsDTOList.Union(totalResultProposedMovementsDTOList).ToList();

                categories.Add(new FilterResultProposedMovementsResponse
                {

                    Id = allLabel,
                    Name = FilterAllEnum.All.GetDescription()

                });
            }

            return resultProposedMovementsDTOList;
        }

        private async Task<List<BaseSalaryProposedMovementsDTO>> GetResultConsolidated(GetProposedMovementsRequest request)
        {

            IEnumerable<BaseSalaryProposedMovementsDTO> result =
                await GetSalaryBaseXPositionSM(request);

            return result.ToList();
        }
        private async Task<IEnumerable<BaseSalaryProposedMovementsDTO>> GetSalaryBaseXPositionSM(GetProposedMovementsRequest request)
        {
            IEnumerable<BaseSalaryProposedMovementsDTO> salaryBaseList =
                        new List<BaseSalaryProposedMovementsDTO>();
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var levelsExp = permissionUser.Levels;

            var groupsExp =
                        permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var areaExp = permissionUser.Areas;
            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            if (request.Scenario == DisplayMMMIEnum.MM)
                salaryBaseList = await _unitOfWork.GetRepository<BaseSalariais, long>()
                    .Include("CmcodeNavigation")
                    .GetListAsync(x => x.Where(bs =>
                           bs.CargoIdSMMM.HasValue &&
                          (request.UnitId.HasValue || request.CompaniesId.Contains(bs.EmpresaId.Value)) &&
                          (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId)) &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))?
                         .Select(s => new BaseSalaryProposedMovementsDTO
                         {
                             CompanyId = s.EmpresaId.Value,
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionSalaryBase = s.CargoEmpresa,
                             PositionId = s.CargoIdSMMM.Value,
                             PositionIdDefault = s.CargoIdSM
                         }));

            if (request.Scenario == DisplayMMMIEnum.MI)
                salaryBaseList = await _unitOfWork.GetRepository<BaseSalariais, long>()
                    .Include("CmcodeNavigation")
                    .GetListAsync(x => x.Where(bs =>
                            bs.CargoIdSMMI.HasValue &&
                          (request.UnitId.HasValue || request.CompaniesId.Contains(bs.EmpresaId.Value)) &&
                          (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId)) &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))?
                         .Select(s => new BaseSalaryProposedMovementsDTO
                         {
                             CompanyId = s.EmpresaId.Value,
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionSalaryBase = s.CargoEmpresa,
                             PositionId = s.CargoIdSMMI.Value,
                             PositionIdDefault = s.CargoIdSM
                         }));

            if (!salaryBaseList.Any())
                return new List<BaseSalaryProposedMovementsDTO>();

            var positionsIds = salaryBaseList.Select(s => s.PositionId).Distinct();

            var positionsSM = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                .Include("CmcodeNavigation")
                .Include("CargosProjetosSmmapeamento")
                .Include("CargosProjetoSMParametrosMapeamento.ParametrosProjetosSMLista.ParametrosProjetosSMTipos")
                .GetListAsync(g => g
                .Where(cp =>
                cp.Ativo.HasValue &&
                cp.Ativo.Value &&
                positionsIds.Contains(cp.CargoProjetoSmidLocal) && cp.ProjetoId == request.ProjectId)?.
                Select(s => new PositionSalaryProposedMovementsDTO
                {
                    PositionId = s.CargoProjetoSmidLocal,
                    Parameter = s.CargosProjetoSMParametrosMapeamento.Any(f => f.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal) ? s.CargosProjetoSMParametrosMapeamento.FirstOrDefault(f => f.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal).ParametrosProjetosSMLista.ParametrosProjetosSMTipos.Parametro : string.Empty,
                    GroupId = s.GrupoSmidLocal,
                    GSM = s.CargosProjetosSmmapeamento.Any(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal) ? (long)s.CargosProjetosSmmapeamento.FirstOrDefault(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal).Gsm : 0,
                    PositionSM = s.CargoSm,
                    LevelId = s.CmcodeNavigation != null ? s.CmcodeNavigation.NivelId : (int?)null
                }));

            foreach (var salary in salaryBaseList)
            {
                var positionValues = positionsSM.FirstOrDefault(f => f.PositionId == salary.PositionId);

                salary.Parameter = positionValues.Parameter;
                salary.ProfileId = positionValues.GroupId;
                salary.GSM = positionValues.GSM;
                salary.LevelId = positionValues.LevelId ?? salary.LevelId;
                salary.ProposedMovements = GetProposedMovementsAnalyse(salary.PositionId,
                    salary.PositionIdDefault, positionValues.PositionSM, salary.PositionSalaryBase);
            }

            return salaryBaseList.Where(s => (!groupsExp.Safe().Any() || !groupsExp.Contains(s.ProfileId)) &&
                (!areaExp.Safe().Any()));// || !areaExp.Contains(s.Parameter)));
        }

        private ProposedMovementsEnum GetProposedMovementsAnalyse(long? positionId,
            long? positionDefaultId,
            string positionSM,
            string positionSalaryBase)
        {
            if (!positionDefaultId.HasValue)
                return ProposedMovementsEnum.ChangeOfPosition;

            if (positionId.Value != positionDefaultId.Value)
                return ProposedMovementsEnum.ChangeOfPosition;

            if (!positionSM.ToLower().Trim().Equals(positionSalaryBase.ToLower().Trim()))
                return ProposedMovementsEnum.AdequacyOfNomenclature;

            return ProposedMovementsEnum.WithoutProposedAdjustment;
        }
    }
}
