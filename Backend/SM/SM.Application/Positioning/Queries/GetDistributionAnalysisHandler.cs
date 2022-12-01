using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
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

    public class GetDistributionAnalysisRequest : IRequest<GetDistributionAnalysisResponse>
    {
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
        public IEnumerable<object> CategoriesExp { get; set; } = new List<object>(); //using to share (EXP)
    }

    public class GetDistributionAnalysisResponse
    {
        public ChartDistributionAnalysisResponse Chart { get; set; }
        public ShareDistributionAnalysisResponse Share { get; set; }
        public IEnumerable<FilterDistributionAnalysisResponse> Categories { get; set; }
    }

    public class FilterDistributionAnalysisResponse
    {
        public string Name { get; set; }
        public string Id { get; set; }

    }

    public class ShareDistributionAnalysisResponse
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

    public class ChartDistributionAnalysisResponse
    {
        public IEnumerable<CategoriesDistributionAnalysisResponse> Main { get; set; }
        public IEnumerable<DistributionAnalysisDrillDownResponse> DrillDown { get; set; }
    }

    public class CategoriesDistributionAnalysisResponse
    {
        public string Name { get; set; }
        public DistribuitionAnalyseEnum Type { get; set; }
        public IEnumerable<DataDistributionAnalysisResponse> Data { get; set; }

    }

    public class DataDistributionAnalysisResponse
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public class DistributionAnalysisDrillDownResponse
    {
        public string ItemGrouped { get; set; }
        public IEnumerable<DataDistributionAnalysisDrillDownResponse> Data { get; set; }
    }
    public class DataDistributionAnalysisDrillDownResponse
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public double EmployeeAmount { get; set; }
        public long? TrackId { get; set; }
        public DistribuitionAnalyseEnum Type { get; set; }
    }

    public class BaseSalaryDistributionAnalysisDTO
    {
        public double? FinalSalarySM { get; set; }
        public int? InternalFrequency { get; set; }
        public long CompanyId { get; set; }
        public long ProfileId { get; set; }
        public long LevelId { get; set; }
        public string Parameter { get; set; }
        public long GSM { get; set; }
        public double HoursBaseSM { get; set; }
        public double HoursBase { get; set; }
        public DistribuitionAnalyseEnum DistribuitionAnalyse { get; set; }
        public string TrackPositioning { get; set; }
        public long? TrackId { get; set; }
        public long PositionId { get; set; }
    }

    public class DataSalaryTableFullInfoDistribuitionAnalysis
    {
        public double Multiplicator { get; set; }
        public double Value { get; set; }
        public long TrackId { get; set; }

    }
    public class GroupXTableDistributionAnalysisDTO
    {
        public long GroupId { get; set; }
        public long TableId { get; set; }
        public long CompanyId { get; set; }
    }
    public class SalaryTableValuesDistributionAnalysisDTO
    {
        public long TableId { get; set; }
        public double Midpoint { get; set; }
        public int GSM { get; set; }
        public IEnumerable<long> GroupIds { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
    }
    public class SalaryTableTracksDistributionAnalysisDTO
    {
        public long TableId { get; set; }
        public double Multiplier { get; set; }
    }

    public class SalaryTableResultDistributionAnalysisDTO
    {
        public double Multiplier { get; set; }
        public double Value { get; set; }
    }

    public class ResultDistributionAnalysisDTO
    {
        public double EmployeeAmount { get; set; } = 0;
        public double EmployeePercentage { get; set; } = 0;
        public DistribuitionAnalyseEnum DistribuitionAnalyse { get; set; }
        public string ItemGrouped { get; set; }

    }

    public class ResultListGroupByDistributionAnalysisDTO
    {

        public string Name { get; set; }
        public string Id { get; set; }
        public IEnumerable<BaseSalaryDistributionAnalysisDTO> Values { get; set; }
    }


    public class PositionSalaryDistribuitionAnalysisDTO
    {
        public double HoursBaseSM { get; set; }
        public long GroupId { get; set; }
        public string Parameter { get; set; }
        public long GSM { get; set; }
        public long PositionId { get; set; }
        public int? LevelId { get; set; }
    }

    public class GetDistributionAnalysisHandler
        : IRequestHandler<GetDistributionAnalysisRequest, GetDistributionAnalysisResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IEnumerable<DistribuitionAnalyseEnum> _listEnum;
        public GetDistributionAnalysisHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _listEnum = Enum.GetValues(typeof(DistribuitionAnalyseEnum)) as
                IEnumerable<DistribuitionAnalyseEnum>;

            _listEnum = _listEnum.First().GetWithOrder().Select(s =>
            (DistribuitionAnalyseEnum)Enum.Parse(typeof(DistribuitionAnalyseEnum), s));
        }
        public async Task<GetDistributionAnalysisResponse> Handle(GetDistributionAnalysisRequest request, CancellationToken cancellationToken)
        {
            var result = await GetResultConsolidated(request);

            if (result.Any())
            {

                if (request.DisplayBy == DisplayByPositioningEnum.ProfileId)
                    return FitData(request, await Profile(request, result));
                if (request.DisplayBy == DisplayByPositioningEnum.LevelId)
                    return FitData(request, await Level(request, result));
                if (request.DisplayBy == DisplayByPositioningEnum.Area ||
                    request.DisplayBy == DisplayByPositioningEnum.Parameter01 ||
                    request.DisplayBy == DisplayByPositioningEnum.Parameter02 ||
                    request.DisplayBy == DisplayByPositioningEnum.Parameter03)
                    return FitData(request, Parameter(request, result));
            }

            return new GetDistributionAnalysisResponse();
        }

        private async Task<List<ResultListGroupByDistributionAnalysisDTO>> Profile(GetDistributionAnalysisRequest request,
            List<BaseSalaryDistributionAnalysisDTO> result)
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
                .Select(s => new ResultListGroupByDistributionAnalysisDTO
                {
                    Name = groupsProfiles.FirstOrDefault(f => f.Id == s.key)?.Name,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().ProfileId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private async Task<List<ResultListGroupByDistributionAnalysisDTO>> Level(GetDistributionAnalysisRequest request,
                        List<BaseSalaryDistributionAnalysisDTO> result)
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
                .Select(s => new ResultListGroupByDistributionAnalysisDTO
                {
                    Name = s.key,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().LevelId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private List<ResultListGroupByDistributionAnalysisDTO> Parameter(GetDistributionAnalysisRequest request,
            List<BaseSalaryDistributionAnalysisDTO> result)
        {
            var areas = result
                        .GroupBy(g => g.Parameter, (key, value) =>
                        new ResultListGroupByDistributionAnalysisDTO
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
        private GetDistributionAnalysisResponse FitData(GetDistributionAnalysisRequest request,
            IEnumerable<ResultListGroupByDistributionAnalysisDTO> resultList)
        {

            if (!resultList.Any())
                return new GetDistributionAnalysisResponse();

            var resultDistributionAnalysisDTOList = new List<ResultDistributionAnalysisDTO>();
            var resultDrillDownList = new List<DistributionAnalysisDrillDownResponse>();

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

                //drilldown 

                var dataDrillDown = new List<DataDistributionAnalysisDrillDownResponse>();
                var groupByTrackPositioning = values.GroupBy(g => g.TrackPositioning);

                foreach (var trackPositioning in groupByTrackPositioning)
                {
                    dataDrillDown.Add(new DataDistributionAnalysisDrillDownResponse
                    {
                        EmployeeAmount = trackPositioning.Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0))),
                        Value = employeeAmountTotal == 0 ? 0 :
                            Math.Round(trackPositioning.Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0))) / employeeAmountTotal * 100),
                        Name = trackPositioning.Key,
                        TrackId = trackPositioning.FirstOrDefault().TrackId,
                        Type = trackPositioning.FirstOrDefault().DistribuitionAnalyse
                    });
                }

                resultDrillDownList.Add(new DistributionAnalysisDrillDownResponse
                {
                    ItemGrouped = itemResult.Name,
                    Data = dataDrillDown.OrderBy(o => (int)o.Type).ThenBy(t => t.TrackId)
                });

            }

            var categories = resultList.Map().ToANew<IEnumerable<FilterDistributionAnalysisResponse>>().ToList();

            resultDistributionAnalysisDTOList = AddAll(request,
                resultDistributionAnalysisDTOList,
                employeeAmountTotalTotal,
                categories,
                resultDrillDownList);

            //fit response 
            return new GetDistributionAnalysisResponse
            {
                Categories = categories,
                Chart = new ChartDistributionAnalysisResponse
                {
                    Main = resultDistributionAnalysisDTOList.GroupBy(g => g.DistribuitionAnalyse,
                                (key, value) => new CategoriesDistributionAnalysisResponse
                                {
                                    Name = key.GetDescription(),
                                    Type = value.FirstOrDefault().DistribuitionAnalyse,
                                    Data = value.Select(s => new DataDistributionAnalysisResponse
                                    {
                                        Name = s.ItemGrouped,
                                        Value = s.EmployeePercentage
                                    })
                                }),
                    DrillDown = resultDrillDownList
                }
            };
        }

        private List<ResultDistributionAnalysisDTO> AddAll(GetDistributionAnalysisRequest request,
            List<ResultDistributionAnalysisDTO> resultDistributionAnalysisDTOList,
            List<double> employeeAmountTotalTotal,
            List<FilterDistributionAnalysisResponse> categories,
            List<DistributionAnalysisDrillDownResponse> drillDownList)
        {
            var isNumeric = Regex.IsMatch(categories.FirstOrDefault().Id, @"\d");
            var allLabel = isNumeric ? ((int)FilterAllEnum.All).ToString() : FilterAllEnum.All.ToString();

            if (!request.CategoriesExp.Any(a => a.ToString().Equals(allLabel)))
            {
                //add Total 
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
                        Math.Round((filterByAnalyse.Sum(s => s.EmployeeAmount) / sumemployeeAmountTotal * 100), 0),
                        ItemGrouped = FilterAllEnum.All.GetDescription()
                    });
                }

                //merge total + the other values 
                resultDistributionAnalysisDTOList =
                    resultDistributionAnalysisDTOList.Union(totalDistributionAnalysisDTOList).ToList();
                //drillDown 
                var allDrillDown = drillDownList.SelectMany(s => s.Data);
                var groupByDrillDown = allDrillDown.GroupBy(g => g.Name);
                var dataDrillDown = new List<DataDistributionAnalysisDrillDownResponse>();

                foreach (var item in groupByDrillDown)
                {
                    double valueResult = sumemployeeAmountTotal == 0 ? 0 : (item.Sum(s => s.EmployeeAmount) / sumemployeeAmountTotal * 100);
                    dataDrillDown.Add(new DataDistributionAnalysisDrillDownResponse
                    {
                        TrackId = item.FirstOrDefault().TrackId,
                        Name = item.Key,
                        Type = item.FirstOrDefault().Type,
                        Value = Math.Round(valueResult, 0)
                    });
                }
                drillDownList.Add(new DistributionAnalysisDrillDownResponse
                {
                    ItemGrouped = FilterAllEnum.All.GetDescription(),
                    Data = dataDrillDown.OrderBy(x => x.Type).ThenBy(x => x.TrackId)
                });

                categories.Add(new FilterDistributionAnalysisResponse
                {
                    Id = allLabel,
                    Name = FilterAllEnum.All.GetDescription()
                });
            }

            return resultDistributionAnalysisDTOList;
        }

        private async Task<List<BaseSalaryDistributionAnalysisDTO>> GetResultConsolidated(GetDistributionAnalysisRequest request)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            IEnumerable<BaseSalaryDistributionAnalysisDTO> result =
                await GetSalaryBaseXPositionSM(request, permissionUser);

            var groupIds = result
                   .Select(s => s.ProfileId).Distinct().ToList();

            var tablesExp =
                permissionUser.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                .SubItems;

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
                return new List<BaseSalaryDistributionAnalysisDTO>();

            var listTableIds = groupsData?.Select(s => s.TableId).Distinct();

            var salaryTableResult = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                .Include("TabelasSalariaisFaixas")
                .Include("TabelasSalariaisValores")
                .Include("GruposProjetosSalaryMarkMapeamento")
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
                return new List<BaseSalaryDistributionAnalysisDTO>();

            var groups = groupsData.Select(g => g.GroupId).Distinct();

            var policyParameters = await _unitOfWork.GetRepository<TabelasSalariaisParametrosPolitica, long>()
                    .GetListAsync(g => g.Where(tsp => tsp.ProjetoId == request.ProjectId &&
                    groups.Contains(tsp.GrupoProjetoMidLocal))?
                    .Select(s => new
                    {
                        GroupId = s.GrupoProjetoMidLocal,
                        TrackId = s.FaixaSalarialId,
                        Label = s.RotuloPolitica
                    }));
            var policyTrackId = policyParameters.Select(x => x.TrackId);
            var tracks = salaryTableResult
                .OrderByDescending(o => o.Tracks.Count())
                .Take(1)
                .SelectMany(s => s.Tracks);

            tracks = tracks.Where(x => policyTrackId.Contains(x.TrackId)).OrderBy(o => o.TrackId);

            var minSalaries = await _unitOfWork.GetRepository<TabelasSalariaisGrades, long>()
                    .GetListAsync(g => g.Where(tg => tg.ProjetoId == request.ProjectId &&
                     listTableIds.Contains(tg.TabelaSalarialIdLocal))?
                     .Select(se => new
                     {
                         TableId = se.TabelaSalarialIdLocal,
                         SalaryMin = se.MenorSalario
                     }));

            foreach (var salaryBasePositionSM in result)
            {
                var companyId = salaryBasePositionSM.CompanyId;
                var groupId = salaryBasePositionSM.ProfileId;
                var mapGroup = groupsData
                                    .Where(f => f.CompanyId == companyId && f.GroupId == groupId).ToList();

                var tableIds = mapGroup?.Select(s => s.TableId);

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
                    var gsmData = salaryTable.SelectMany(s => s.MidPoint)?.Where(s => s.GSM == salaryBasePositionSM.GSM);

                    if (gsmData.Any())
                        midPoint = gsmData.Average(a => a.MidPoint) / hoursBaseSM * hoursBase;

                    var listSalaryTable = new List<DataSalaryTableFullInfoDistribuitionAnalysis>();

                    foreach (var track in tracks)
                    {
                        if (track.TrackId < minTrackId || track.TrackId > maxTrackId)
                        {
                            listSalaryTable.Add(new DataSalaryTableFullInfoDistribuitionAnalysis
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                            continue;
                        }

                        var fFM = salaryTable.SelectMany(a => a.Tracks)
                                             .Where(s => s.TrackId == track.TrackId)?
                                             .Average(a => a.FactorMulti);


                        var midPointResult = midPoint.HasValue ? (midPoint.Value * fFM) : 0;

                        if (fFM.HasValue && (midPointResult >= minSalary))
                            listSalaryTable.Add(new DataSalaryTableFullInfoDistribuitionAnalysis
                            {
                                Value = (double)midPointResult,
                                Multiplicator = fFM.Value,
                                TrackId = track.TrackId
                            });
                        else
                            listSalaryTable.Add(new DataSalaryTableFullInfoDistribuitionAnalysis
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                    }

                    double ifamin = GetIFAMin(listSalaryTable.Select(s => s.Value), finalSalary);
                    if (ifamin > 0)
                    {
                        salaryBasePositionSM.DistribuitionAnalyse = DistribuitionAnalyseEnum.BelowSalaryPolicy;

                        salaryBasePositionSM.TrackPositioning = DistribuitionAnalyseEnum.BelowSalaryPolicy.GetDescription();
                        continue;
                    }

                    double ifamax = GetIFAMax(listSalaryTable.Select(s => s.Value), finalSalary);
                    if (ifamax > 0)
                    {
                        salaryBasePositionSM.DistribuitionAnalyse = DistribuitionAnalyseEnum.AboveSalaryPolicy;
                        salaryBasePositionSM.TrackPositioning = DistribuitionAnalyseEnum.AboveSalaryPolicy.GetDescription();
                        continue;
                    }

                    salaryBasePositionSM.DistribuitionAnalyse = DistribuitionAnalyseEnum.WithinTheSalaryPolicy;

                    long? trackNearlySalary = listSalaryTable.OrderBy(x => Math.Abs((long)x.Value - finalSalary.Value)).FirstOrDefault()?.TrackId;

                    if (trackNearlySalary.HasValue)
                    {
                        salaryBasePositionSM.TrackId = trackNearlySalary;
                        var polity = policyParameters?.FirstOrDefault(s => s.GroupId == groupId && s.TrackId == trackNearlySalary);
                        polity = polity == null ? policyParameters?.FirstOrDefault(s => s.TrackId == trackNearlySalary) : polity;
                        salaryBasePositionSM.TrackPositioning = polity != null ? polity.Label : string.Empty;
                    }
                }
            }

            return result.ToList();
        }
        private async Task<IEnumerable<BaseSalaryDistributionAnalysisDTO>> GetSalaryBaseXPositionSM(GetDistributionAnalysisRequest request,
            PermissionJson permissionUser)
        {
            IEnumerable<BaseSalaryDistributionAnalysisDTO> salaryBaseList =
                        new List<BaseSalaryDistributionAnalysisDTO>();


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
                          request.CompaniesId.Contains(bs.EmpresaId.Value) &&
                          (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId)) &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))?
                         .Select(s => new BaseSalaryDistributionAnalysisDTO
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
                          request.CompaniesId.Contains(bs.EmpresaId.Value) &&
                          (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId)) &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))?
                         .Select(s => new BaseSalaryDistributionAnalysisDTO
                         {
                             FinalSalarySM = s.SalarioFinalSm,
                             CompanyId = s.EmpresaId.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0),
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionId = s.CargoIdSMMI.Value
                         }));

            if (!salaryBaseList.Any())
                return new List<BaseSalaryDistributionAnalysisDTO>();

            var positionsIds = salaryBaseList.Select(s => s.PositionId).Distinct();

            var positionsSM = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                .Include("CmcodeNavigation")
                .Include("CargosProjetosSmmapeamento")
                .Include("CargosProjetoSMParametrosMapeamento.ParametrosProjetosSMLista.ParametrosProjetosSMTipos")
                .GetListAsync(g => g
                .Where(cp => cp.Ativo.HasValue &&
                cp.Ativo.Value &&
                positionsIds.Contains(cp.CargoProjetoSmidLocal) && cp.ProjetoId == request.ProjectId)?.
                Select(s => new PositionSalaryDistribuitionAnalysisDTO
                {
                    PositionId = s.CargoProjetoSmidLocal,
                    Parameter = s.CargosProjetoSMParametrosMapeamento.Any(f => f.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal) ? s.CargosProjetoSMParametrosMapeamento.FirstOrDefault(f => f.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal).ParametrosProjetosSMLista.ParametrosProjetosSMTipos.Parametro : string.Empty,
                    GroupId = s.GrupoSmidLocal,
                    GSM = s.CargosProjetosSmmapeamento.Any(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal) ? (long)s.CargosProjetosSmmapeamento.FirstOrDefault(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal).Gsm : 0,
                    HoursBaseSM = s.BaseHoraria,
                    LevelId = s.CmcodeNavigation != null ? s.CmcodeNavigation.NivelId : (int?)null
                }));

            foreach (var salary in salaryBaseList)
            {
                var positionValues = positionsSM.FirstOrDefault(f => f.PositionId == salary.PositionId);

                salary.Parameter = positionValues.Parameter;
                salary.ProfileId = positionValues.GroupId;
                salary.GSM = positionValues.GSM;
                salary.HoursBaseSM = positionValues.HoursBaseSM;
                salary.LevelId = positionValues.LevelId ?? salary.LevelId;
            }

            return salaryBaseList.Where(s => (!groupsExp.Safe().Any() || !groupsExp.Contains(s.ProfileId)) &&
                (!areaExp.Safe().Any()));// || !areaExp.Contains(s.Parameter)));
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
