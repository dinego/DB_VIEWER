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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Positioning.Queries
{

    public class GetComparativeAnalysisTableRequest :
        IRequest<GetComparativeAnalysisTableResponse>
    {
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
        public IEnumerable<object> CategoriesExp { get; set; } = new List<object>(); //using to share (EXP)
    }

    public class GetComparativeAnalysisTableResponse
    {

        public IEnumerable<GetComparativeAnalysisTableResultResponse> Tables { get; set; }
        public ShareResultComparativeAnalysisTableResponse Share { get; set; }
        public IEnumerable<FilterComparativeAnalysisTableResponse> Categories { get; set; }

    }

    public class ShareResultComparativeAnalysisTableResponse
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

    public class FilterComparativeAnalysisTableResponse
    {
        public string Name { get; set; }
        public string Id { get; set; }

    }

    public class GetComparativeAnalysisTableResultResponse
    {
        public TypeTableEnum @Type { get; set; }
        public IEnumerable<GetHeaderComparativeAnalysisTableResultResponse> Header { get; set; }
        public IEnumerable<GetTotalComparativeAnalysisTableResultResponse> Total { get; set; }
        public IEnumerable<IEnumerable<GetBodyComparativeAnalysisTableResultResponse>> Body { get; set; }
    }

    public class GetHeaderComparativeAnalysisTableResultResponse
    {
        public string Name { get; set; }
        public int ColumnId { get; set; } = 0;
        public int ColPos { get; set; }
    }

    public class GetBodyComparativeAnalysisTableResultResponse
    {
        public int ColPos { get; set; }
        public long? Amount { get; set; }
        public string Percentage { get; set; } = "-";
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public double PercentageValue { get; set; }
        public double? SumPeopleFrontMidPoint { get; set; }
        public int? SumInternalFrequency { get; set; }
    }

    public class GetTotalComparativeAnalysisTableResultResponse
    {
        public int ColPos { get; set; }
        public double Total { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
    }

    public class BaseSalaryComparativeAnalysisTableDTO
    {
        public double? FinalSalarySM { get; set; }
        public int? InternalFrequency { get; set; }
        public long CompanyId { get; set; }
        public long ProfileId { get; set; }
        public long LevelId { get; set; }
        public string Parameter { get; set; }
        public long GSM { get; set; }
        public double PeopleFrontMidPoint { get; set; } = 0;
        public string CareerAxis { get; set; }
        public long PositionId { get; set; }
        public long CMCode { get; set; }
        public double HoursBase { get; set; }
        public double HoursBaseSM { get; set; }
    }

    public class ResultListGroupByComparativeAnalysisTableDTO
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public IEnumerable<BaseSalaryComparativeAnalysisTableDTO> Values { get; set; }
    }

    public class PositionSalaryComparativeAnalysisTableDTO
    {
        public long GroupId { get; set; }
        public string Parameter { get; set; }
        public long GSM { get; set; }
        public long PositionId { get; set; }
        public string CarrerAxis { get; set; }
        public int? LevelId { get; set; }
        public double HoursBaseSM { get; set; }
    }
    public class GetComparativeAnalysisTableHandler
        : IRequestHandler<GetComparativeAnalysisTableRequest, GetComparativeAnalysisTableResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IEnumerable<ComparativeAnalyseTableEnum> _listEnum;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetComparativeAnalysisTableHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _listEnum = Enum.GetValues(typeof(ComparativeAnalyseTableEnum)) as
                IEnumerable<ComparativeAnalyseTableEnum>;

            _listEnum = _listEnum.First().GetWithOrder().Select(s =>
            (ComparativeAnalyseTableEnum)Enum.Parse(typeof(ComparativeAnalyseTableEnum), s));
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
        }
        public async Task<GetComparativeAnalysisTableResponse> Handle(GetComparativeAnalysisTableRequest request, CancellationToken cancellationToken)
        {
            var result = await GetResultConsolidated(request);

            if (!result.Any()) return new GetComparativeAnalysisTableResponse();

            var axisCarrer = result.Select(s => s.CareerAxis)
                .OrderBy(o => o).Distinct();

            if (request.DisplayBy == DisplayByPositioningEnum.ProfileId)
                return await FitData(request, await Profile(request, result), axisCarrer, DisplayByPositioningEnum.ProfileId);
            if (request.DisplayBy == DisplayByPositioningEnum.LevelId)
                return await FitData(request, await Level(request, result), axisCarrer, DisplayByPositioningEnum.LevelId);
            if (request.DisplayBy == DisplayByPositioningEnum.Area ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter01 ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter02 ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter03)
                return await FitData(request, Area(request, result), axisCarrer, DisplayByPositioningEnum.Area);

            return await FitData(request, await Profile(request, result), axisCarrer, DisplayByPositioningEnum.ProfileId);
        }

        private async Task<List<ResultListGroupByComparativeAnalysisTableDTO>> Profile(GetComparativeAnalysisTableRequest request,
                List<BaseSalaryComparativeAnalysisTableDTO> result)
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
                .Select(s => new ResultListGroupByComparativeAnalysisTableDTO
                {
                    Name = groupsProfiles.FirstOrDefault(f => f.Id == s.key)?.Name,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().ProfileId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private async Task<List<ResultListGroupByComparativeAnalysisTableDTO>> Level(GetComparativeAnalysisTableRequest request,
                        List<BaseSalaryComparativeAnalysisTableDTO> result)
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
                .Select(s => new ResultListGroupByComparativeAnalysisTableDTO
                {
                    Name = s.key,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().LevelId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private List<ResultListGroupByComparativeAnalysisTableDTO> Area(GetComparativeAnalysisTableRequest request,
            List<BaseSalaryComparativeAnalysisTableDTO> result)
        {
            var areas = result
                        .GroupBy(g => g.Parameter, (key, value) =>
                        new ResultListGroupByComparativeAnalysisTableDTO
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
        private async Task<GetComparativeAnalysisTableResponse> FitData(GetComparativeAnalysisTableRequest request,
            IEnumerable<ResultListGroupByComparativeAnalysisTableDTO> resultList,
            IEnumerable<string> axisCarrer, DisplayByPositioningEnum filter)
        {
            if (!resultList.Any())
                return new GetComparativeAnalysisTableResponse();

            var categories = resultList.Map().ToANew<IEnumerable<FilterComparativeAnalysisTableResponse>>().ToList();

            var isNumeric = Regex.IsMatch(categories.FirstOrDefault().Id, @"\d");
            var allLabel = isNumeric ? ((int)FilterAllEnum.All).ToString() : FilterAllEnum.All.ToString();

            var isAddTotal = !request.CategoriesExp.Any(a => a.ToString().Equals(allLabel));

            if (isAddTotal)
            {
                categories.Add(new FilterComparativeAnalysisTableResponse
                {

                    Id = allLabel,
                    Name = FilterAllEnum.All.GetDescription()

                });
            }

            var listTables = new List<GetComparativeAnalysisTableResultResponse>();

            var headers = await FixedTable(request, resultList, filter, isAddTotal);
            listTables.Add(headers);
            listTables.Add(DinamicTable(resultList, axisCarrer, isAddTotal));

            return new GetComparativeAnalysisTableResponse
            {
                Categories = categories,
                Tables = listTables

            };
        }

        private async Task<GetComparativeAnalysisTableResultResponse> FixedTable(GetComparativeAnalysisTableRequest request,
            IEnumerable<ResultListGroupByComparativeAnalysisTableDTO> resultList,
            DisplayByPositioningEnum filter,
            bool isAddTotal = true)
        {
            //dinamic table
            var header = new List<GetHeaderComparativeAnalysisTableResultResponse>();
            //configuration global labels 
            var configGlobalLabels = await _getGlobalLabelsInteractor.Handler(request.ProjectId);
            int colHeader = 0;
            foreach (var item in _listEnum)
            {
                switch (item)
                {
                    case ComparativeAnalyseTableEnum.Filter:
                        var globalLabel = configGlobalLabels.FirstOrDefault(x => x.Id == (long)filter);
                        header.Add(new GetHeaderComparativeAnalysisTableResultResponse
                        {
                            ColPos = colHeader,
                            ColumnId = (int)item,
                            Name = globalLabel != null ? globalLabel.Alias : filter.GetDescription()
                        });
                        break;
                    case ComparativeAnalyseTableEnum.AxisCarrer:
                        continue;
                    default:
                        header.Add(new GetHeaderComparativeAnalysisTableResultResponse
                        {
                            ColPos = colHeader,
                            ColumnId = (int)item,
                            Name = item.GetDescription()
                        });
                        break;
                }
                colHeader++;
            }

            //total
            var totalEmployee = resultList
                .SelectMany(s => s.Values)
                .Sum(s => s.InternalFrequency);

            var totalPositions = resultList
                .GroupBy(g => g.Name, (key, value) =>
                    value.SelectMany(se => se.Values)
                    .GroupBy(gb => gb.CMCode)
                    .Count())
                .Sum();

            //body
            var body = new List<List<GetBodyComparativeAnalysisTableResultResponse>>();
            foreach (var row in resultList)
            {

                var cols = new List<GetBodyComparativeAnalysisTableResultResponse>();
                int countBody = 0;
                foreach (var col in _listEnum)
                {
                    switch (col)
                    {
                        case ComparativeAnalyseTableEnum.Filter:
                            cols.Add(new GetBodyComparativeAnalysisTableResultResponse
                            {
                                Name = row.Name,
                                ColPos = countBody,
                                CategoryId = row.Id
                            });
                            break;
                        case ComparativeAnalyseTableEnum.People:
                            var amountEmployees =
                                row.Values.Sum(s => s.InternalFrequency);
                            var percentagePeople = amountEmployees.HasValue ?
                                Math.Round(amountEmployees.Value / (double)totalEmployee * 100, 0) : 0;

                            cols.Add(new GetBodyComparativeAnalysisTableResultResponse
                            {
                                ColPos = countBody,
                                Amount = amountEmployees.GetValueOrDefault(0),
                                Percentage = percentagePeople == 0 ? "-" :
                                percentagePeople.ToString("N0", CultureInfo.InvariantCulture),
                                CategoryId = row.Id
                            });

                            break;
                        case ComparativeAnalyseTableEnum.Positions:
                            var amountPositions =
                                        row.Values
                                        .GroupBy(g => g.CMCode)
                                        .Count();
                            var percentagePositions = amountPositions == 0 ? 0 :
                                Math.Round(amountPositions / (double)totalPositions * 100, 0);

                            cols.Add(new GetBodyComparativeAnalysisTableResultResponse
                            {
                                ColPos = countBody,
                                Amount = amountPositions,
                                Percentage = percentagePositions == 0 ? "-" :
                                    percentagePositions.ToString("N0", CultureInfo.InvariantCulture),
                                CategoryId = row.Id
                            });
                            break;
                        case ComparativeAnalyseTableEnum.General:

                            var peopleMidpoint = Math.Round(row.Values
                                            .Average(s => s.PeopleFrontMidPoint) * 100, 0);

                            var sumPeoplefronMidPoint = row.Values.Sum(x => x.PeopleFrontMidPoint);
                            var sumInternalFrequency = row.Values.Sum(x => x.InternalFrequency);

                            cols.Add(new GetBodyComparativeAnalysisTableResultResponse
                            {
                                ColPos = countBody,
                                Percentage = peopleMidpoint == 0 ?
                                            "-" : peopleMidpoint.ToString("N0", CultureInfo.InvariantCulture),
                                CategoryId = row.Id,
                                PercentageValue = peopleMidpoint,
                                SumInternalFrequency = sumInternalFrequency,
                                SumPeopleFrontMidPoint = sumPeoplefronMidPoint
                            });
                            break;
                        case ComparativeAnalyseTableEnum.AxisCarrer:
                            continue;
                        default:
                            break;
                    }
                    countBody++;
                }

                body.Add(cols);

            }


            List<GetTotalComparativeAnalysisTableResultResponse> total = isAddTotal ?
                AddAllFixed(body) : new List<GetTotalComparativeAnalysisTableResultResponse>();


            return new GetComparativeAnalysisTableResultResponse
            {
                Type = TypeTableEnum.Fixed,
                Body = body,
                Header = header,
                Total = total
            };

        }

        private GetComparativeAnalysisTableResultResponse DinamicTable(
    IEnumerable<ResultListGroupByComparativeAnalysisTableDTO> resultList,
    IEnumerable<string> axisCarrer,
    bool isAddTotal = true)
        {
            //dinamic table
            var header = new List<GetHeaderComparativeAnalysisTableResultResponse>();
            int colHeader = 0;

            foreach (var ac in axisCarrer)
            {
                header.Add(new GetHeaderComparativeAnalysisTableResultResponse
                {
                    ColPos = colHeader,
                    Name = ac
                });
                colHeader++;
            }

            //body
            var body = new List<List<GetBodyComparativeAnalysisTableResultResponse>>();
            foreach (var row in resultList)
            {

                var cols = new List<GetBodyComparativeAnalysisTableResultResponse>();
                int countBody = 0;

                foreach (var ac in axisCarrer)
                {

                    var filterPeopleMidpoint = row.Values
                         .Where(v => v.CareerAxis.Equals(ac));

                    var peopleMidpointByAxisCarrer = filterPeopleMidpoint.Safe().Any() ?
                        filterPeopleMidpoint?.Average(a => a.PeopleFrontMidPoint) :
                        null;

                    var peopleMidpointByAxisCarrerPercentage = Math.Round(100 * peopleMidpointByAxisCarrer
                                                    .GetValueOrDefault(0), 0);

                    var sumPeopleFrontMidPoint = filterPeopleMidpoint.Safe().Any() ?
                        filterPeopleMidpoint?.Sum(a => a.PeopleFrontMidPoint) :
                        null;

                    var sumInternalFrequency = filterPeopleMidpoint.Safe().Any() ?
                        filterPeopleMidpoint?.Sum(a => a.InternalFrequency) :
                        null;

                    cols.Add(new GetBodyComparativeAnalysisTableResultResponse
                    {
                        ColPos = countBody,
                        Percentage = peopleMidpointByAxisCarrerPercentage == 0 ? "-" :
                                        peopleMidpointByAxisCarrerPercentage.ToString("N0", CultureInfo.InvariantCulture),
                        Name = countBody == 0 ? row.Name : null,
                        CategoryId = row.Id,
                        SumPeopleFrontMidPoint = sumPeopleFrontMidPoint,
                        SumInternalFrequency = sumInternalFrequency
                    });

                    countBody++;
                }

                body.Add(cols);
            }


            List<GetTotalComparativeAnalysisTableResultResponse> total = isAddTotal ?
                AddAllDinamic(axisCarrer, body) : new List<GetTotalComparativeAnalysisTableResultResponse>();


            return new GetComparativeAnalysisTableResultResponse
            {
                Type = TypeTableEnum.Dynamic,
                Body = body,
                Header = header,
                Total = total
            };

        }

        private List<GetTotalComparativeAnalysisTableResultResponse> AddAllFixed(
            List<List<GetBodyComparativeAnalysisTableResultResponse>> body)
        {
            var total = new List<GetTotalComparativeAnalysisTableResultResponse>();

            //add Total 

            int colTotal = 0;
            foreach (var item in _listEnum)
            {
                switch (item)
                {
                    case ComparativeAnalyseTableEnum.Filter:
                        total.Add(new GetTotalComparativeAnalysisTableResultResponse
                        {
                            ColPos = colTotal,
                            Name = FilterAllEnum.All.GetDescription(),
                            CategoryId = FilterAllEnum.All.ToString()
                        });
                        break;
                    case ComparativeAnalyseTableEnum.AxisCarrer:
                        continue;
                    case ComparativeAnalyseTableEnum.General:
                        //var sumGeneralTotal = body.Average(s => s.FirstOrDefault(f => f.ColPos == colTotal).PercentageValue);

                        double sumPeopleFrontMidPoint = body.Where(x => x.Any(f => f.ColPos == colTotal))
                                    .Sum(s => Convert.ToDouble(s.FirstOrDefault(f => f.ColPos == colTotal).SumPeopleFrontMidPoint));

                        int? internalFrequency = body.Where(x => x.Any(f => f.ColPos == colTotal))
                                            .Sum(s => s.FirstOrDefault(f => f.ColPos == colTotal).SumInternalFrequency);

                        double sumGeneralTotal = internalFrequency.HasValue && internalFrequency.Value > 0 ?
                                    (sumPeopleFrontMidPoint / internalFrequency.Value) * 100 : 0;

                        total.Add(new GetTotalComparativeAnalysisTableResultResponse
                        {
                            ColPos = colTotal,
                            Total = Math.Round(sumGeneralTotal, 0),
                            CategoryId = FilterAllEnum.All.ToString()
                        });
                        break;
                    default:

                        var sumTotal = body.Sum(s =>
                                s.FirstOrDefault(f => f.ColPos == colTotal).Amount);

                        total.Add(new GetTotalComparativeAnalysisTableResultResponse
                        {
                            ColPos = colTotal,
                            Total = sumTotal.GetValueOrDefault(0),
                            CategoryId = FilterAllEnum.All.ToString()
                        });
                        break;
                }
                colTotal++;
            }

            return total;
        }

        private List<GetTotalComparativeAnalysisTableResultResponse> AddAllDinamic(
                    IEnumerable<string> axisCarrer,
                    List<List<GetBodyComparativeAnalysisTableResultResponse>> body)
        {
            var total = new List<GetTotalComparativeAnalysisTableResultResponse>();

            //add Total 
            int colTotal = 0;

            foreach (var ac in axisCarrer)
            {
                double sumPeopleFrontMidPoint = body.Where(x => x.Any(f => f.ColPos == colTotal && !f.Percentage.Equals("-")))
                                    .Sum(s => Convert.ToDouble(s.FirstOrDefault(f => f.ColPos == colTotal).SumPeopleFrontMidPoint));

                int? internalFrequency = body.Where(x => x.Any(f => f.ColPos == colTotal && !f.Percentage.Equals("-")))
                                    .Sum(s => s.FirstOrDefault(f => f.ColPos == colTotal).SumInternalFrequency);

                double averageAll = internalFrequency.HasValue && internalFrequency.Value > 0 ?
                            (sumPeopleFrontMidPoint / internalFrequency.Value) * 100 : 0;
                if (colTotal == 0)
                {
                    total.Add(new GetTotalComparativeAnalysisTableResultResponse
                    {
                        ColPos = colTotal,
                        Total = Math.Round(averageAll, 0),
                        Name = FilterAllEnum.All.GetDescription(),
                        CategoryId = FilterAllEnum.All.ToString()
                    });
                    colTotal++;

                    continue;
                }
                total.Add(new GetTotalComparativeAnalysisTableResultResponse
                {
                    ColPos = colTotal,
                    Total = Math.Round(averageAll, 0),
                    CategoryId = FilterAllEnum.All.ToString()
                });
                colTotal++;
            }

            return total;
        }

        private async Task<List<BaseSalaryComparativeAnalysisTableDTO>>
            GetResultConsolidated(GetComparativeAnalysisTableRequest request)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            IEnumerable<BaseSalaryComparativeAnalysisTableDTO> result =
                await GetSalaryBaseXPositionSM(request, permissionUser);

            var groupIds = result
                        .Select(s => s.ProfileId).Distinct().ToList();

            var tablesExp =
                permissionUser.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                .SubItems;


            var groupsData = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                                              .GetListAsync(x => x.Where(g => g.ProjetoId == request.ProjectId &&
                                                                (!tablesExp.Safe().Any() || !tablesExp.Contains(g.TabelaSalarialIdLocal)) &&
                                                                 groupIds.Contains(g.GrupoProjetoSmidLocal) &&
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
                return new List<BaseSalaryComparativeAnalysisTableDTO>();

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
                return new List<BaseSalaryComparativeAnalysisTableDTO>();

            var tracks = salaryTableResult
                            .OrderByDescending(o => o.Tracks.Count())
                            .Take(1)
                            .SelectMany(s => s.Tracks)
                            .OrderBy(o => o.TrackId);


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
                         .Average(a => a.MidPoint) / salaryBasePositionSM.HoursBaseSM * salaryBasePositionSM.HoursBase;

                    //MipPointComparation
                    salaryBasePositionSM.PeopleFrontMidPoint = GetPercentageMidPoint(
                                                        midPoint,
                                                        finalSalary);
                }
            }

            return result.ToList();
        }
        private async Task<IEnumerable<BaseSalaryComparativeAnalysisTableDTO>> GetSalaryBaseXPositionSM(GetComparativeAnalysisTableRequest request,
            PermissionJson permissionUser)
        {
            IEnumerable<BaseSalaryComparativeAnalysisTableDTO> salaryBaseList =
                        new List<BaseSalaryComparativeAnalysisTableDTO>();

            var levelsExp = permissionUser.Levels;

            var areaExp = permissionUser.Areas;

            var groupsExp =
                    permissionUser.Contents?
                    .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                    .SubItems;

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
                         .Select(s => new BaseSalaryComparativeAnalysisTableDTO
                         {
                             FinalSalarySM = s.SalarioFinalSm,
                             CompanyId = s.EmpresaId.Value,
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionId = s.CargoIdSMMM.Value,
                             CMCode = s.Cmcode.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0)
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
                         .Select(s => new BaseSalaryComparativeAnalysisTableDTO
                         {
                             FinalSalarySM = s.SalarioFinalSm,
                             CompanyId = s.EmpresaId.Value,
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionId = s.CargoIdSMMI.Value,
                             CMCode = s.Cmcode.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0)
                         }));

            if (!salaryBaseList.Any())
                return new List<BaseSalaryComparativeAnalysisTableDTO>();

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
                Select(s => new PositionSalaryComparativeAnalysisTableDTO
                {
                    PositionId = s.CargoProjetoSmidLocal,
                    GSM = s.CargosProjetosSmmapeamento.Any(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal) ? s.CargosProjetosSmmapeamento.FirstOrDefault(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal).Gsm.HasValue ? s.CargosProjetosSmmapeamento.FirstOrDefault(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal).Gsm.Value : 0 : 0,
                    HoursBaseSM = s.BaseHoraria,
                    Parameter = s.CargosProjetoSMParametrosMapeamento.Any(a => a.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal) ? s.CargosProjetoSMParametrosMapeamento.FirstOrDefault(a => a.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal).ParametrosProjetosSMLista.ParametrosProjetosSMTipos.Parametro : string.Empty,
                    //CarrerAxis = s.EixoCarreira,
                    LevelId = s.CmcodeNavigation != null ? s.CmcodeNavigation.NivelId : (int?)null
                }));

            foreach (var salary in salaryBaseList)
            {
                var positionValues = positionsSM.FirstOrDefault(f => f.PositionId == salary.PositionId);

                salary.Parameter = positionValues.Parameter;
                salary.ProfileId = positionValues.GroupId;
                salary.GSM = positionValues.GSM;
                salary.CareerAxis = positionValues.CarrerAxis;
                salary.LevelId = positionValues.LevelId ?? salary.LevelId;
                salary.HoursBaseSM = positionValues.HoursBaseSM;
            }

            return salaryBaseList.Where(s => (!groupsExp.Safe().Any() || !groupsExp.Contains(s.ProfileId)) &&
                (!areaExp.Safe().Any()));// || !areaExp.Contains(s.Parameter)));
        }
        private double GetPercentageMidPoint(double? midPointValue, double? finalSalary)
        {
            if (midPointValue.GetValueOrDefault(0) == 0 || finalSalary.GetValueOrDefault(0) == 0) return 0;

            return finalSalary.Value / midPointValue.Value;
        }
    }
}
