using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using SM.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.DashBoard.Queries
{

    public class GetDistributionAnalysisChartRequest
        : IRequest<GetDistributionAnalysisChartResponse>
    {
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;

        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public long ProjectId { get; set; }
    }
    public class GetDistributionAnalysisChartResponse
    {
        public ChartDistributionAnalysisDashResponse Chart { get; set; }
    }

    public class ChartDistributionAnalysisDashResponse
    {
        public IEnumerable<CategoriesDistributionAnalysisChartResponse> Main { get; set; }
        public IEnumerable<DistributionAnalysisDrillDownDashResponse> DrillDown { get; set; } = null;
    }

    public class CategoriesDistributionAnalysisChartResponse
    {
        public string Name { get; set; }
        public DistribuitionAnalyseEnum Type { get; set; }
        public IEnumerable<DataDistributionAnalysisChartResponse> Data { get; set; }

    }

    public class DataDistributionAnalysisChartResponse
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public class BaseSalaryDistributionAnalysisChartDTO
    {
        public double? FinalSalarySM { get; set; }
        public int? InternalFrequency { get; set; }
        public long CompanyId { get; set; }
        public long ProfileId { get; set; }
        public long LevelId { get; set; }
        public string Area { get; set; }
        public string Parameter01 { get; set; }
        public string Parameter02 { get; set; }
        public string Parameter03 { get; set; }
        public long GSM { get; set; }
        public double IFAMin { get; set; } = 0;
        public double IFAMax { get; set; } = 0;
        public double HoursBaseSM { get; set; }
        public double HoursBase { get; set; }
        public DistribuitionAnalyseEnum DistribuitionAnalyse { get; set; }
        public string TrackPositioning { get; set; }
        public long PositionId { get; set; }

    }

    public class DataSalaryTableFullInfoDistribuitionAnalysisDash
    {
        public double Multiplicator { get; set; }
        public double Value { get; set; }
        public long TrackId { get; set; }

    }

    public class PositionSalaryDistribuitionAnalysisChartDTO
    {
        public double HoursBaseSM { get; set; }
        public long GroupId { get; set; }
        public string Area { get; set; }
        public string Parameter01 { get; set; }
        public string Parameter02 { get; set; }
        public string Parameter03 { get; set; }
        public long GSM { get; set; }
        public long PositionId { get; set; }

    }
    public class DistributionAnalysisDrillDownDashResponse
    {
        public string ItemGrouped { get; set; }
        public IEnumerable<DataDistributionAnalysisDrillDownDashResponse> Data { get; set; }
    }
    public class DataDistributionAnalysisDrillDownDashResponse
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public double EmployeeAmount { get; set; }
        public DistribuitionAnalyseEnum Type { get; set; }
    }

    public class ResultDistributionAnalysisDTO
    {
        public double EmployeeAmount { get; set; } = 0;
        public double EmployeePercentage { get; set; } = 0;
        public DistribuitionAnalyseEnum DistribuitionAnalyse { get; set; }
        public string ItemGrouped { get; set; }

    }

    public class ResultListGroupByDistributionAnalysisChartDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<BaseSalaryDistributionAnalysisChartDTO> Values { get; set; }
    }
    public class GetDistributionAnalysisChartHandler
        : IRequestHandler<GetDistributionAnalysisChartRequest, GetDistributionAnalysisChartResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private IEnumerable<DistribuitionAnalyseEnum> _listEnum;

        public GetDistributionAnalysisChartHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _listEnum = Enum.GetValues(typeof(DistribuitionAnalyseEnum)) as
                IEnumerable<DistribuitionAnalyseEnum>;

            _listEnum = _listEnum.First().GetWithOrder().Select(s =>
            (DistribuitionAnalyseEnum)Enum.Parse(typeof(DistribuitionAnalyseEnum), s));
        }
        public async Task<GetDistributionAnalysisChartResponse> Handle(GetDistributionAnalysisChartRequest request, CancellationToken cancellationToken)
        {

            var result = await GetResultConsolidated(request);

            if (!result.Any())
                return new GetDistributionAnalysisChartResponse();

            switch (request.DisplayBy)
            {
                case DisplayByPositioningEnum.ProfileId:
                    return FitData(await Profile(request, result));

                case DisplayByPositioningEnum.LevelId:
                    return FitData(await Level(request, result));

                case DisplayByPositioningEnum.Area:
                    return FitData(Area(request, result));

                case DisplayByPositioningEnum.Parameter01:
                    return FitData(Parameter01(request, result));

                case DisplayByPositioningEnum.Parameter02:
                    return FitData(Parameter02(request, result));

                case DisplayByPositioningEnum.Parameter03:
                    return FitData(Parameter03(request, result));

                default:
                    return FitData(await Profile(request, result));
            }

            return FitData(await Profile(request, result));

        }
        private async Task<List<ResultListGroupByDistributionAnalysisChartDTO>> Profile(GetDistributionAnalysisChartRequest request,
            List<BaseSalaryDistributionAnalysisChartDTO> result)
        {
            var profiles = result
                        .GroupBy(g => g.ProfileId, (key, value) => new { key, value });

            var groupsIds = profiles.Select(s => s.key);

            var groupsProfiles = await _unitOfWork.GetRepository<GruposProjetosSalaryMark, long>()
                .GetListAsync(x => x.Where(g => groupsIds.Contains(g.GruposProjetosSmidLocal) &&
                 request.ProjectId == g.ProjetoId)?.
                Select(s => new
                {
                    Id = s.GruposProjetosSmidLocal,
                    Name = s.GrupoSm
                }));

            return profiles.OrderBy(o => o.key)
                .Select(s => new ResultListGroupByDistributionAnalysisChartDTO
                {
                    Name = groupsProfiles.FirstOrDefault(f => f.Id == s.key).Name,
                    Values = s.value,
                    Id = s.key
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }

        private async Task<List<ResultListGroupByDistributionAnalysisChartDTO>> Level(GetDistributionAnalysisChartRequest request,
                        List<BaseSalaryDistributionAnalysisChartDTO> result)
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

            return levels
                .OrderBy(o => o.value.FirstOrDefault().LevelId)
                .Select(s => new ResultListGroupByDistributionAnalysisChartDTO
                {
                    Name = s.key,
                    Values = s.value,
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private List<ResultListGroupByDistributionAnalysisChartDTO> Area(GetDistributionAnalysisChartRequest request,
            List<BaseSalaryDistributionAnalysisChartDTO> result)
        {
            var areas = result
                        .GroupBy(g => g.Area, (key, value) =>
                        new ResultListGroupByDistributionAnalysisChartDTO
                        {
                            Name = key,
                            Values = value
                        });

            return areas.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();

        }
        private List<ResultListGroupByDistributionAnalysisChartDTO> Parameter01(GetDistributionAnalysisChartRequest request,
            List<BaseSalaryDistributionAnalysisChartDTO> result)
        {
            var parameter01 = result
                .Where(s => !string.IsNullOrWhiteSpace(s.Parameter01))?
                        .GroupBy(g => g.Parameter01, (key, value) =>
                        new ResultListGroupByDistributionAnalysisChartDTO
                        {
                            Name = key,
                            Values = value
                        });

            if (!parameter01.Safe().Any())
                return new List<ResultListGroupByDistributionAnalysisChartDTO>();

            return parameter01.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();

        }
        private List<ResultListGroupByDistributionAnalysisChartDTO> Parameter02(GetDistributionAnalysisChartRequest request,
            List<BaseSalaryDistributionAnalysisChartDTO> result)
        {
            var parameter02 = result
                .Where(s => !string.IsNullOrWhiteSpace(s.Parameter02))?
                        .GroupBy(g => g.Parameter02, (key, value) =>
                        new ResultListGroupByDistributionAnalysisChartDTO
                        {
                            Name = key,
                            Values = value
                        });

            if (!parameter02.Safe().Any())
                return new List<ResultListGroupByDistributionAnalysisChartDTO>();

            return parameter02.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();
        }
        private List<ResultListGroupByDistributionAnalysisChartDTO> Parameter03(GetDistributionAnalysisChartRequest request,
            List<BaseSalaryDistributionAnalysisChartDTO> result)
        {
            var parameter03 = result
                .Where(s => !string.IsNullOrWhiteSpace(s.Parameter03))?
                        .GroupBy(g => g.Parameter03, (key, value) =>
                        new ResultListGroupByDistributionAnalysisChartDTO
                        {
                            Name = key,
                            Values = value
                        });

            if (!parameter03.Safe().Any())
                return new List<ResultListGroupByDistributionAnalysisChartDTO>();

            return parameter03.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();
        }

        private GetDistributionAnalysisChartResponse FitData(IEnumerable<ResultListGroupByDistributionAnalysisChartDTO> resultList)
        {

            var resultDistributionAnalysisDTOList = new List<ResultDistributionAnalysisDTO>();
            var resultDrillDownList = new List<DistributionAnalysisDrillDownDashResponse>();

            var employeeAmountTotalTotal = new List<double>();

            foreach (var itemResult in resultList)
            {
                var values = itemResult.Values;

                var employeeAmountTotal = values.Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0)));
                employeeAmountTotalTotal.Add(employeeAmountTotal);

                foreach (DistribuitionAnalyseEnum itemEnum in _listEnum)
                {

                    var employeeAmount = values
                        .Where(s => s.DistribuitionAnalyse == itemEnum)?
                        .Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0)));

                    resultDistributionAnalysisDTOList.Add(new ResultDistributionAnalysisDTO
                    {
                        DistribuitionAnalyse = itemEnum,
                        EmployeeAmount = employeeAmount.GetValueOrDefault(0),
                        EmployeePercentage = employeeAmountTotal == 0 ? 0 :
                        Math.Round(employeeAmount.GetValueOrDefault(0) / employeeAmountTotal * 100, 0),
                        ItemGrouped = itemResult.Name
                    });

                }
            }

            resultDistributionAnalysisDTOList = AddAll(
                resultDistributionAnalysisDTOList,
                employeeAmountTotalTotal);

            //fit response 
            return new GetDistributionAnalysisChartResponse
            {
                Chart = new ChartDistributionAnalysisDashResponse
                {
                    Main = resultDistributionAnalysisDTOList.GroupBy(g => g.DistribuitionAnalyse,
                                (key, value) => new CategoriesDistributionAnalysisChartResponse
                                {
                                    Name = key.GetDescription(),
                                    Type = value.FirstOrDefault().DistribuitionAnalyse,
                                    Data = value.Select(s => new DataDistributionAnalysisChartResponse
                                    {
                                        Name = s.ItemGrouped,
                                        Value = s.EmployeePercentage
                                    })
                                })
                }
            };
        }

        private List<ResultDistributionAnalysisDTO> AddAll(
                List<ResultDistributionAnalysisDTO> resultDistributionAnalysisDTOList,
                List<double> employeeAmountTotalTotal)
        {

            var sumemployeeAmountTotal = employeeAmountTotalTotal.Sum();

            var totalDistributionAnalysisDTOList = new List<ResultDistributionAnalysisDTO>();
            foreach (DistribuitionAnalyseEnum itemEnum in _listEnum)
            {
                var filterByAnalyse = resultDistributionAnalysisDTOList.Where(s => s.DistribuitionAnalyse == itemEnum);

                totalDistributionAnalysisDTOList.Add(new ResultDistributionAnalysisDTO
                {
                    DistribuitionAnalyse = itemEnum,
                    EmployeeAmount = filterByAnalyse.Sum(s => s.EmployeeAmount),
                    EmployeePercentage = sumemployeeAmountTotal == 0 ? 0 :
                    Math.Round(100 * (filterByAnalyse.Sum(s => s.EmployeeAmount) / sumemployeeAmountTotal), 0),
                    ItemGrouped = FilterAllEnum.All.GetDescription()
                });
            }

            //merge total + the other values 
            resultDistributionAnalysisDTOList =
                resultDistributionAnalysisDTOList.Union(totalDistributionAnalysisDTOList).ToList();

            return resultDistributionAnalysisDTOList;
        }

        private async Task<List<BaseSalaryDistributionAnalysisChartDTO>> GetResultConsolidated(GetDistributionAnalysisChartRequest request)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            IEnumerable<BaseSalaryDistributionAnalysisChartDTO> result =
                await GetSalaryBaseXPositionSM(request, permissionUser);

            var tablesExp =
                permissionUser.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                .SubItems;

            var groupIds = result
                   .Select(s => s.ProfileId).Distinct().ToList();

            var groupsData = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                                    .GetListAsync(x => x.Where(g => g.ProjetoId == request.ProjectId &&
                                     groupIds.Contains(g.GrupoProjetoSmidLocal) &&
                                     (!tablesExp.Safe().Any() || !tablesExp.Contains(g.TabelaSalarialIdLocal)) &&
                                     (request.UnitId.HasValue || request.CompaniesId.Contains(g.EmpresaId)) &&
                                    (!request.UnitId.HasValue || g.EmpresaId == request.UnitId.Value))?
                                    .Select(s => new
                                    {
                                        MinTrackId = s.FaixaIdInferior,
                                        MaxTrackId = s.FaixaIdSuperior,
                                        GroupId = s.GrupoProjetoSmidLocal,
                                        CompanyId = s.EmpresaId,
                                        TableId = s.TabelaSalarialIdLocal
                                    }));

            if (!groupsData.Any())
                return new List<BaseSalaryDistributionAnalysisChartDTO>();

            var listTableIds = groupsData?.Select(s => s.TableId).Distinct();

            var salaryTableResult = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                .Include("TabelasSalariaisFaixas")
                .Include("TabelasSalariaisValores")
                .GetListAsync(x => x.Where(ts => listTableIds.Contains(ts.TabelaSalarialIdLocal) &&
                 ts.ProjetoId == request.ProjectId)
                .Select(s => new
                {
                    TableId = s.TabelaSalarialIdLocal,
                    Tracks = s.TabelasSalariaisFaixas
                    .Select(fm => new { FactorMulti = fm.AmplitudeMidPoint, TrackId = fm.FaixaSalarialId }),
                    MidPoint = s.TabelasSalariaisValores.Select(mp => new
                    {
                        MidPoint = mp.FaixaMidPoint,
                        GSM = mp.Grade,
                    })
                }));

            if (!salaryTableResult.Any())
                return new List<BaseSalaryDistributionAnalysisChartDTO>();


            var tracks = salaryTableResult
                .OrderByDescending(o => o.Tracks.Count())
                .Take(1)
                .SelectMany(s => s.Tracks)
                .OrderBy(o => o.TrackId);

            var minSalaries = await _unitOfWork.GetRepository<TabelasSalariaisGrades, long>()
                                .GetListAsync(g => g.Where(tg => tg.ProjetoId == request.ProjectId &&
                                 listTableIds.Contains(tg.TabelaSalarialIdLocal))?
                                 .Select(se => new
                                 {
                                     TableId = se.TabelaSalarialIdLocal,
                                     SalaryMin = se.MenorSalario
                                 }));


            var groups = groupsData.Select(g => g.GroupId).Distinct();
            var tracksIds = tracks.Select(s => s.TrackId).Distinct();

            var policyParameters = await _unitOfWork.GetRepository<TabelasSalariaisParametrosPolitica, long>()
                    .GetListAsync(g => g.Where(tsp => tsp.ProjetoId == request.ProjectId &&
                    groups.Contains(tsp.GrupoProjetoMidLocal) &&
                    tracksIds.Contains(tsp.FaixaSalarialId))?
                    .Select(s => new
                    {
                        GroupId = s.GrupoProjetoMidLocal,
                        TrackId = s.FaixaSalarialId,
                        Label = s.RotuloPolitica
                    }));

            foreach (var salaryBasePositionSM in result)
            {

                var companyId = salaryBasePositionSM.CompanyId;
                var groupId = salaryBasePositionSM.ProfileId;
                var mapGroup = groupsData
                                    .Where(f => f.CompanyId == companyId && f.GroupId == groupId).ToList();

                var tableIds = mapGroup.Select(s => s.TableId);

                var maxTrackId = mapGroup.Count > 0 ? mapGroup
                                .Where(s => tableIds.Safe().Any() && tableIds.Contains(s.TableId))?
                                .Max(m => m.MaxTrackId) : 0;

                var minTrackId = mapGroup.Count > 0 ? mapGroup
                                .Where(s => tableIds.Safe().Any() && tableIds.Contains(s.TableId))?
                                .Min(m => m.MinTrackId) : 0;

                var minSalary = minSalaries.Where(w => tableIds.Contains(w.TableId)).Average(a => a.SalaryMin);

                var hoursBase = salaryBasePositionSM.HoursBase;
                var hoursBaseSM = salaryBasePositionSM.HoursBaseSM;
                var finalSalary = salaryBasePositionSM.FinalSalarySM;

                var salaryTable =
                        salaryTableResult.Where(st =>
                                tableIds.Safe().Any() && tableIds.Contains(st.TableId));

                if (salaryTable != null)
                {

                    double? midPoint = null;

                    var gsmData = salaryTable.SelectMany(s => s.MidPoint)?
                         .Where(s => s.GSM == salaryBasePositionSM.GSM);

                    if (gsmData.Any())
                        midPoint = gsmData
                         .Average(a => a.MidPoint) / hoursBaseSM * hoursBase;

                    var listSalaryTable = new List<DataSalaryTableFullInfoDistribuitionAnalysisDash>();

                    foreach (var track in tracks)
                    {
                        if (track.TrackId < minTrackId || track.TrackId > maxTrackId)
                        {
                            listSalaryTable.Add(new DataSalaryTableFullInfoDistribuitionAnalysisDash
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                            continue;
                        }

                        var fFM = salaryTable
                        .SelectMany(a => a.Tracks)
                        .Where(s => s.TrackId == track.TrackId
                        )?
                        .Average(a => a.FactorMulti);

                        var midPointResult = midPoint.HasValue ?
                                (midPoint.Value * fFM) : 0;

                        if (fFM.HasValue && (midPointResult >= minSalary))
                        {
                            listSalaryTable.Add(new DataSalaryTableFullInfoDistribuitionAnalysisDash
                            {
                                Value = Math.Round((double)midPointResult, 0),
                                Multiplicator = fFM.Value,
                                TrackId = track.TrackId
                            });
                        }
                        else
                        {
                            listSalaryTable.Add(new DataSalaryTableFullInfoDistribuitionAnalysisDash
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                        }
                    }

                    double ifamin = GetIFAMin(listSalaryTable.Select(s => s.Value),
                                finalSalary);

                    if (ifamin > 0)
                    {
                        salaryBasePositionSM.DistribuitionAnalyse =
                            DistribuitionAnalyseEnum.BelowSalaryPolicy;

                        salaryBasePositionSM.TrackPositioning =
                            DistribuitionAnalyseEnum.BelowSalaryPolicy.GetDescription();
                        continue;
                    }

                    double ifamax = GetIFAMax(listSalaryTable.Select(s => s.Value),
                                    finalSalary);

                    if (ifamax > 0)
                    {
                        salaryBasePositionSM.DistribuitionAnalyse =
                            DistribuitionAnalyseEnum.AboveSalaryPolicy;
                        salaryBasePositionSM.TrackPositioning =
                            DistribuitionAnalyseEnum.AboveSalaryPolicy.GetDescription();
                        continue;
                    }

                    salaryBasePositionSM.DistribuitionAnalyse =
                        DistribuitionAnalyseEnum.WithinTheSalaryPolicy;

                    var trackAboveFinalSalary = listSalaryTable
                        .Where(f => finalSalary.HasValue &&
                        f.Value < finalSalary.Value)?
                        .OrderByDescending(o => o.Value)?
                        .FirstOrDefault()?
                        .TrackId;

                    salaryBasePositionSM.TrackPositioning = trackAboveFinalSalary.HasValue ?
                        policyParameters?.FirstOrDefault(s => s.GroupId == groupId &&
                        s.TrackId == trackAboveFinalSalary)?.Label :
                        string.Empty;
                }
            }

            return result.ToList();
        }
        private async Task<IEnumerable<BaseSalaryDistributionAnalysisChartDTO>> GetSalaryBaseXPositionSM(GetDistributionAnalysisChartRequest request,
            PermissionJson permissionUser)
        {
            IEnumerable<BaseSalaryDistributionAnalysisChartDTO> salaryBaseList =
                        new List<BaseSalaryDistributionAnalysisChartDTO>();

            var groupsExp =
                        permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var areaExp = permissionUser.Areas;

            var levelsExp = permissionUser.Levels;

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
                         .Select(s => new BaseSalaryDistributionAnalysisChartDTO
                         {
                             FinalSalarySM = s.SalarioFinalSm,
                             CompanyId = s.EmpresaId.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0),
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionId = s.CargoIdSMMM.Value
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
                         .Select(s => new BaseSalaryDistributionAnalysisChartDTO
                         {
                             FinalSalarySM = s.SalarioFinalSm,
                             CompanyId = s.EmpresaId.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0),
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionId = s.CargoIdSMMI.Value
                         }));

            if (!salaryBaseList.Any())
                return new List<BaseSalaryDistributionAnalysisChartDTO>();

            var positionsIds = salaryBaseList.Select(s => s.PositionId).Distinct();

            var positionsSM = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                .GetListAsync(g => g
                .Where(cp =>
                cp.Ativo.HasValue &&
                cp.Ativo.Value &&
                positionsIds.Contains(cp.CargoProjetoSmidLocal) && cp.ProjetoId == request.ProjectId)?.
                Select(s => new PositionSalaryDistribuitionAnalysisChartDTO
                {
                    PositionId = s.CargoProjetoSmidLocal,
                    //Area = s.Area,
                    GroupId = s.GrupoSmidLocal,
                    //GSM = s.Gsm,
                    //Parameter01 = s.Parametro1,
                    //Parameter02 = s.Parametro2,
                    //Parameter03 = s.Parametro3,
                    //HoursBaseSM = s.BaseHoraria
                }));

            foreach (var salary in salaryBaseList)
            {
                var positionValues = positionsSM.FirstOrDefault(f => f.PositionId == salary.PositionId);

                //salary.Area = positionValues.Area;
                //salary.ProfileId = positionValues.GroupId;
                //salary.GSM = positionValues.GSM;
                //salary.Parameter01 = positionValues.Parameter01;
                //salary.Parameter02 = positionValues.Parameter02;
                //salary.Parameter03 = positionValues.Parameter03;
                //salary.HoursBaseSM = positionValues.HoursBaseSM;
            }

            return salaryBaseList.Where(s => (!groupsExp.Safe().Any() || !groupsExp.Contains(s.ProfileId)) &&
                (!areaExp.Safe().Any()));// || !areaExp.Contains(s.Area)));
        }
        private double GetIFAMin(IEnumerable<double> values, double? finalSalary)
        {
            if (!values.Any() || finalSalary.GetValueOrDefault(0) == 0) return 0;

            var list = values.Where(s => s > 0)?.ToList();

            if (!list.Safe().Any()) return 0;

            var diff = finalSalary.Value / list.Min();

            if (diff < 1) return list.Min() - finalSalary.Value;

            return 0;
        }
        private double GetIFAMax(IEnumerable<double> values, double? finalSalary)
        {
            if (!values.Any() || finalSalary.GetValueOrDefault(0) == 0) return 0;

            var list = values.Where(s => s > 0)?.ToList();

            if (!list.Safe().Any()) return 0;

            var diff = finalSalary.Value / list.Max();

            if (diff > 1) return finalSalary.Value - list.Max();

            return 0;
        }
    }
}
