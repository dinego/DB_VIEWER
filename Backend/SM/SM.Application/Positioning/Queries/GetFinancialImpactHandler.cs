using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Helpers;
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
    public class GetFinancialImpactRequest : IRequest<GetResultFinancialImpactResponse>
    {
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
        public IEnumerable<object> CategoriesExp { get; set; } = new List<object>(); //using to share (EXP)
    }

    public class GetResultFinancialImpactResponse
    {
        public IEnumerable<GetFinancialImpactResponse> Chart { get; set; }
        public ShareResultFinancialImpactResponse Share { get; set; }
        public IEnumerable<FilterResultFinancialImpactResponse> Categories { get; set; }

    }

    public class FilterResultFinancialImpactResponse
    {
        public string Name { get; set; }
        public string Id { get; set; }

    }
    public class GetFinancialImpactResponse
    {
        public string Name { get; set; }
        public IEnumerable<DataFinancialImpactResponse> Data { get; set; }
    }


    public class ShareResultFinancialImpactResponse
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

    public class DataFinancialImpactResponse
    {
        public double Y { get; set; }
        public double Percentage { get; set; }
        public double Func { get; set; }
        public double FuncPercentage { get; set; }
        public string Name { get; set; }
        public ClickFinancialImpactResponse Click { get; set; }
    }

    public class ClickFinancialImpactResponse
    {
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public object CategoryId { get; set; }
        public AnalyseFinancialImpactEnum SerieId { get; set; } = AnalyseFinancialImpactEnum.IFAMax;
    }

    public class BaseSalaryFinancialImpactDTO
    {
        public double? FinalSalarySM { get; set; }
        public int? InternalFrequency { get; set; }
        public long CompanyId { get; set; }
        public long ProfileId { get; set; }
        public long LevelId { get; set; }
        public string Parameter { get; set; }
        //public string Parameter01 { get; set; }
        //public string Parameter02 { get; set; }
        //public string Parameter03 { get; set; }
        public long GSM { get; set; }
        public double IFAMin { get; set; } = 0;
        public double IFPS { get; set; } = 0;
        public double IFMP { get; set; } = 0;
        public double IFAMax { get; set; } = 0;
        public double HoursBaseSM { get; set; }
        public double HoursBase { get; set; }
        public long PositionId { get; set; }

    }

    public class PositionSalaryFinancialImpactDTO
    {
        public long PositionId { get; set; }
        public long GroupId { get; set; }
        public int GSM { get; set; }
        public string Parameter { get; set; }
        public double HoursBaseSM { get; set; }
        public int? LevelId { get; set; }
    }

    public class GroupXTableFinancialImpactDTO
    {
        public long GroupId { get; set; }
        public long TableId { get; set; }
        public long CompanyId { get; set; }
    }
    public class SalaryTableValuesFinancialImpactDTO
    {
        public long TableId { get; set; }
        public double Midpoint { get; set; }
        public int GSM { get; set; }
        public IEnumerable<long> GroupIds { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
    }

    public class ResultFinancialImpactDTO
    {
        public double EmployeeAmount { get; set; } = 0;
        public double EmployeePercentage { get; set; } = 0;
        public double Cost { get; set; } = 0;
        public double CostPercentage { get; set; } = 0;
        public AnalyseFinancialImpactEnum AnalyseFinancialImpact { get; set; }
        public string ItemGrouped { get; set; }
        public object CategoryId { get; set; }
    }

    public class ResultListGroupByFinancialImpactDTO
    {

        public string Name { get; set; }
        public string Id { get; set; }
        public IEnumerable<BaseSalaryFinancialImpactDTO> Values { get; set; }
    }

    public class DataSalaryTableFinancialImpact
    {
        public double Multiplicator { get; set; }
        public double Value { get; set; }
        public long TrackId { get; set; }

    }


    public class GetFinancialImpactHandler
        : IRequestHandler<GetFinancialImpactRequest, GetResultFinancialImpactResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IEnumerable<AnalyseFinancialImpactEnum> _listEnum;

        public GetFinancialImpactHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _listEnum = Enum.GetValues(typeof(AnalyseFinancialImpactEnum)) as
                IEnumerable<AnalyseFinancialImpactEnum>;

            _listEnum = _listEnum.First().GetWithOrder().Select(s =>
            (AnalyseFinancialImpactEnum)Enum.Parse(typeof(AnalyseFinancialImpactEnum), s));
        }
        public async Task<GetResultFinancialImpactResponse> Handle(GetFinancialImpactRequest request, CancellationToken cancellationToken)
        {
            var result = await GetResultConsolidated(request);

            if (!result.Any())
                return new GetResultFinancialImpactResponse();

            switch (request.DisplayBy)
            {
                case DisplayByPositioningEnum.ProfileId:
                    return FitData(request, await Profile(request, result));

                case DisplayByPositioningEnum.LevelId:
                    return FitData(request, await Level(request, result));

                case DisplayByPositioningEnum.Area:
                    return FitData(request, Area(request, result));

                case DisplayByPositioningEnum.Parameter01:
                    return FitData(request, Parameter01(request, result));

                case DisplayByPositioningEnum.Parameter02:
                    return FitData(request, Parameter02(request, result));

                case DisplayByPositioningEnum.Parameter03:
                    return FitData(request, Parameter03(request, result));

                default:
                    return FitData(request, await Profile(request, result));
            }
        }
        private async Task<List<ResultListGroupByFinancialImpactDTO>> Profile(GetFinancialImpactRequest request,
            List<BaseSalaryFinancialImpactDTO> result)
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

            return profiles.OrderBy(o => o.key)
                .Select(s => new ResultListGroupByFinancialImpactDTO
                {
                    Name = groupsProfiles.FirstOrDefault(f => f.Id == s.key)?.Name,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().ProfileId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private async Task<List<ResultListGroupByFinancialImpactDTO>> Level(GetFinancialImpactRequest request,
                        List<BaseSalaryFinancialImpactDTO> result)
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
                .Select(s => new ResultListGroupByFinancialImpactDTO
                {
                    Name = s.key,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().LevelId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private List<ResultListGroupByFinancialImpactDTO> Area(GetFinancialImpactRequest request, List<BaseSalaryFinancialImpactDTO> result)
        {

            var areas = result
                        .GroupBy(g => g.Parameter, (key, value) =>
                        new ResultListGroupByFinancialImpactDTO
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
        private List<ResultListGroupByFinancialImpactDTO> Parameter01(GetFinancialImpactRequest request, List<BaseSalaryFinancialImpactDTO> result)
        {
            var parameter01 = result
                .Where(s => !string.IsNullOrWhiteSpace(s.Parameter))?
                        .GroupBy(g => g.Parameter, (key, value) =>
                        new ResultListGroupByFinancialImpactDTO
                        {
                            Name = key,
                            Id = key,
                            Values = value
                        });

            if (!parameter01.Safe().Any())
                return new List<ResultListGroupByFinancialImpactDTO>();

            //filter
            if (request.CategoriesExp.Any())
            {
                var filter = request.CategoriesExp.Select(s => (string)s);

                parameter01 = parameter01.Where(f => !filter.Contains(f.Name));
            }

            return parameter01.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();

        }
        private List<ResultListGroupByFinancialImpactDTO> Parameter02(GetFinancialImpactRequest request, List<BaseSalaryFinancialImpactDTO> result)
        {

            var parameter02 = result
                .Where(s => !string.IsNullOrWhiteSpace(s.Parameter))?
                        .GroupBy(g => g.Parameter, (key, value) =>
                        new ResultListGroupByFinancialImpactDTO
                        {
                            Name = key,
                            Id = key,
                            Values = value
                        });

            if (!parameter02.Safe().Any())
                return new List<ResultListGroupByFinancialImpactDTO>();

            //filter
            if (request.CategoriesExp.Any())
            {
                var filter = request.CategoriesExp.Select(s => (string)s);

                parameter02 = parameter02.Where(f => !filter.Contains(f.Name));
            }

            return parameter02.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();

        }
        private List<ResultListGroupByFinancialImpactDTO> Parameter03(GetFinancialImpactRequest request, List<BaseSalaryFinancialImpactDTO> result)
        {
            var parameter03 = result
                .Where(s => !string.IsNullOrWhiteSpace(s.Parameter))?
                        .GroupBy(g => g.Parameter, (key, value) =>
                        new ResultListGroupByFinancialImpactDTO
                        {
                            Name = key,
                            Id = key,
                            Values = value
                        });

            if (!parameter03.Safe().Any())
                return new List<ResultListGroupByFinancialImpactDTO>();

            //filter
            if (request.CategoriesExp.Any())
            {
                var filter = request.CategoriesExp.Select(s => (string)s);

                parameter03 = parameter03.Where(f => !filter.Contains(f.Name));
            }

            return parameter03.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();

        }
        private GetResultFinancialImpactResponse FitData(GetFinancialImpactRequest request, IEnumerable<ResultListGroupByFinancialImpactDTO> resultList)
        {
            if (!resultList.Any())
                return new GetResultFinancialImpactResponse();

            var resultFinancialImpactDTOList = new List<ResultFinancialImpactDTO>();
            var employeeAmountTotalTotal = new List<double>();
            var costTotalTotal = new List<double>();

            foreach (var itemResult in resultList)
            {
                var values = itemResult.Values;

                var employeeAmountTotal = values.Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0)));
                employeeAmountTotalTotal.Add(employeeAmountTotal);
                var costTotal = values.Sum(s => Convert.ToDouble(s.FinalSalarySM));
                costTotalTotal.Add(costTotal);

                foreach (AnalyseFinancialImpactEnum itemEnum in _listEnum)
                {

                    var employeeAmount = values
                        .Where(s => Convert.ToDouble(s.GetType().GetProperty(itemEnum.ToString()).GetValue(s)) > 0)?
                        .Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0)));

                    var cost = values
                                        .Where(s => Convert.ToDouble(s.GetType().GetProperty(itemEnum.ToString()).GetValue(s)) > 0)?
                                        .Sum(s => Convert.ToDouble(s.GetType().GetProperty(itemEnum.ToString()).GetValue(s)));

                    resultFinancialImpactDTOList.Add(new ResultFinancialImpactDTO
                    {
                        AnalyseFinancialImpact = itemEnum,
                        EmployeeAmount = Math.Round(employeeAmount.GetValueOrDefault(0), 0),
                        Cost = Math.Round(cost.GetValueOrDefault(0), 0),
                        CostPercentage = costTotal == 0 ? 0 :
                        Math.Round(100 * (cost.GetValueOrDefault(0) / costTotal), 1),
                        EmployeePercentage = employeeAmountTotal == 0 ? 0 :
                        Math.Round(100 * (employeeAmount.GetValueOrDefault(0) / employeeAmountTotal), 0),
                        ItemGrouped = itemResult.Name,
                        CategoryId = ConverterObject.TryConvertToInt(itemResult.Id)
                    });

                }

            }

            var categories = resultList.Map().ToANew<IEnumerable<FilterResultFinancialImpactResponse>>().ToList();

            resultFinancialImpactDTOList =
                AddAll(request, resultFinancialImpactDTOList, employeeAmountTotalTotal, costTotalTotal, categories);

            //fit response 
            return new GetResultFinancialImpactResponse
            {
                Categories = categories,
                Chart = resultFinancialImpactDTOList.GroupBy(g => g.AnalyseFinancialImpact,
                (key, value) => new GetFinancialImpactResponse
                {
                    Name = key.GetDescription(),
                    Data = value.Select(s => new DataFinancialImpactResponse
                    {
                        Name = s.ItemGrouped,
                        Func = s.EmployeeAmount,
                        FuncPercentage = s.EmployeePercentage,
                        Percentage = s.CostPercentage,
                        Y = s.Cost,
                        Click = new ClickFinancialImpactResponse
                        {
                            CategoryId = s.CategoryId,
                            SerieId = s.AnalyseFinancialImpact,
                            DisplayBy = request.DisplayBy,
                            Scenario = request.Scenario,
                            UnitId = request.UnitId
                        }
                    })
                })
            };
        }

        private List<ResultFinancialImpactDTO> AddAll(GetFinancialImpactRequest request, List<ResultFinancialImpactDTO> resultFinancialImpactDTOList, List<double> employeeAmountTotalTotal, List<double> costTotalTotal, List<FilterResultFinancialImpactResponse> categories)
        {
            var isNumeric = Regex.IsMatch(categories.FirstOrDefault().Id, @"\d");
            var allLabel = isNumeric ? ((int)FilterAllEnum.All).ToString() : FilterAllEnum.All.ToString();

            if (!request.CategoriesExp.Any(a => a.ToString().Equals(allLabel)))
            {
                //add Total 
                var sumcostTotal = costTotalTotal.Sum();
                var sumemployeeAmountTotal = employeeAmountTotalTotal.Sum();

                var totalFinancialImpactDTOList = new List<ResultFinancialImpactDTO>();
                foreach (AnalyseFinancialImpactEnum itemEnum in _listEnum)
                {
                    var filterByAnalyse = resultFinancialImpactDTOList.Where(s => s.AnalyseFinancialImpact == itemEnum);

                    totalFinancialImpactDTOList.Add(new ResultFinancialImpactDTO
                    {
                        AnalyseFinancialImpact = itemEnum,
                        Cost = filterByAnalyse.Sum(s => s.Cost),
                        CostPercentage = sumcostTotal == 0 ? 0 :
                        Math.Round(100 * (filterByAnalyse.Sum(s => s.Cost) / sumcostTotal), 1),
                        EmployeeAmount = filterByAnalyse.Sum(s => s.EmployeeAmount),
                        EmployeePercentage = sumemployeeAmountTotal == 0 ? 0 :
                        Math.Round(100 * (filterByAnalyse.Sum(s => s.EmployeeAmount) / sumemployeeAmountTotal), 0),
                        ItemGrouped = FilterAllEnum.All.GetDescription(),
                        CategoryId = ConverterObject.TryConvertToInt(allLabel)
                    });
                }

                resultFinancialImpactDTOList = resultFinancialImpactDTOList.Union(totalFinancialImpactDTOList).ToList();

                categories.Add(new FilterResultFinancialImpactResponse
                {

                    Id = allLabel,
                    Name = FilterAllEnum.All.GetDescription()

                });

            }

            return resultFinancialImpactDTOList;
        }

        private async Task<List<BaseSalaryFinancialImpactDTO>> GetResultConsolidated(GetFinancialImpactRequest request)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            IEnumerable<BaseSalaryFinancialImpactDTO> result = await GetSalaryBaseXPositionSM(request, permissionUser);

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
                return new List<BaseSalaryFinancialImpactDTO>();

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
                return new List<BaseSalaryFinancialImpactDTO>();

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

                var hoursBase = salaryBasePositionSM.HoursBase;
                var hoursBaseSM = salaryBasePositionSM.HoursBaseSM;
                var minSalary = minSalaries.Where(w => tableIds.Contains(w.TableId)).Average(a => a.SalaryMin) / hoursBaseSM * hoursBase;

                var finalSalary = salaryBasePositionSM.FinalSalarySM;

                var salaryTable =
                        salaryTableResult.Where(st =>
                                tableIds.Safe().Any() && tableIds.Contains(st.TableId));

                if (salaryTable.Count() > 0)
                {
                    double? midPoint = null;

                    var gsmData = salaryTable.SelectMany(s => s.MidPoint)?
                         .Where(s => s.GSM == salaryBasePositionSM.GSM);

                    if (gsmData.Any())
                        midPoint = gsmData.Average(a => a.MidPoint) / hoursBaseSM * hoursBase;

                    var listSalaryTable = new List<DataSalaryTableFinancialImpact>();

                    foreach (var track in tracks)
                    {
                        if (track.TrackId < minTrackId || track.TrackId > maxTrackId)
                        {
                            listSalaryTable.Add(new DataSalaryTableFinancialImpact
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
                            listSalaryTable.Add(new DataSalaryTableFinancialImpact
                            {
                                Value = Math.Round((double)midPointResult, 0),
                                Multiplicator = fFM.Value,
                                TrackId = track.TrackId
                            });
                        }
                        else
                        {
                            listSalaryTable.Add(new DataSalaryTableFinancialImpact
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                        }
                    }

                    salaryBasePositionSM.IFAMin = GetIFAMin(listSalaryTable.Select(s => s.Value),
                                finalSalary);

                    salaryBasePositionSM.IFAMax = GetIFAMax(listSalaryTable.Select(s => s.Value),
                                    finalSalary);


                    salaryBasePositionSM.IFPS = GetIFPS(
                                    listSalaryTable.Where(m => m.TrackId <= (int)TrackIdDefault.MidPoint && m.Value > 0)
                                    .Select(s => s.Value),
                                    finalSalary,
                                    salaryBasePositionSM.IFAMin);

                    salaryBasePositionSM.IFMP = GetIFMP(midPoint,
                        finalSalary);

                }
            }

            return result.ToList();
        }
        private async Task<IEnumerable<BaseSalaryFinancialImpactDTO>> GetSalaryBaseXPositionSM(GetFinancialImpactRequest request,
            PermissionJson permissionUser)
        {
            IEnumerable<BaseSalaryFinancialImpactDTO> salaryBaseList =
                        new List<BaseSalaryFinancialImpactDTO>();


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
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId))
                          &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))?
                         .Select(s => new BaseSalaryFinancialImpactDTO
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
                         .Select(s => new BaseSalaryFinancialImpactDTO
                         {
                             FinalSalarySM = s.SalarioFinalSm,
                             CompanyId = s.EmpresaId.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0),
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionId = s.CargoIdSMMI.Value
                         }));

            if (!salaryBaseList.Any())
                return new List<BaseSalaryFinancialImpactDTO>();

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
                Select(s => new PositionSalaryFinancialImpactDTO
                {
                    PositionId = s.CargoProjetoSmidLocal,
                    //Area = s.Area,
                    GroupId = s.GrupoSmidLocal,
                    GSM = s.CargosProjetosSmmapeamento.Any(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal) ? s.CargosProjetosSmmapeamento.FirstOrDefault(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal).Gsm.HasValue ? s.CargosProjetosSmmapeamento.FirstOrDefault(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal).Gsm.Value : 0 : 0,
                    HoursBaseSM = s.BaseHoraria,
                    Parameter = s.CargosProjetoSMParametrosMapeamento.Any(a => a.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal) ? s.CargosProjetoSMParametrosMapeamento.FirstOrDefault(a => a.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal).ParametrosProjetosSMLista.ParametrosProjetosSMTipos.Parametro : string.Empty,
                    //Parameter01 = s.Parametro1,
                    //Parameter02 = s.Parametro2,
                    //Parameter03 = s.Parametro3,
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
                (!areaExp.Safe().Any()));// || !areaExp.Contains("Area")));

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
        private double GetIFPS(IEnumerable<double> values,
            double? finalSalary,
            double iFAMin)
        {
            if (iFAMin > 1) return iFAMin;

            if (!values.Any()) return 0;

            double nextStep = values
                .FirstOrDefault(f => f > finalSalary.GetValueOrDefault(0));

            return nextStep == 0 ? 0 : nextStep
                - finalSalary.GetValueOrDefault(0);
        }

        private double GetIFMP(double? midpoint,
                        double? finalSalary)
        {
            return midpoint.GetValueOrDefault(0) - finalSalary.GetValueOrDefault(0);
        }

    }
}
