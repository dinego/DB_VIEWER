using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Helpers;
using SM.Application.Interactors.Interfaces;
using SM.Application.Share.Queries;
using SM.Domain.Common;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using SM.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Positioning.Queries
{
    public class GetComparativeAnalysisChartRequest :
        IRequest<GetResultComparativeAnalysisChartResponse>
    {
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
        public IEnumerable<object> CategoriesExp { get; set; } = new List<object>(); //using to share (EXP)
    }

    public class GetResultComparativeAnalysisChartResponse
    {
        public AreaChartComparativeAnalysisChartResponse Chart { get; set; }
        public ShareResultComparativeAnalysisChartResponse Share { get; set; }
        public IEnumerable<FilterComparativeAnalysisChartResponse> Categories { get; set; }
    }
    public class AreaChartComparativeAnalysisChartResponse
    {
        public double Average { get; set; } = 0;
        public IEnumerable<GetComparativeAnalysisChartResponse> Chart { get; set; }
    }

    public class FilterComparativeAnalysisChartResponse
    {
        public string Name { get; set; }
        public string Id { get; set; }

    }

    public class ShareResultComparativeAnalysisChartResponse
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

    public class GetComparativeAnalysisChartResponse
    {
        public string Name { get; set; }
        public ComparativeAnalyseChartEnum Type { get; set; }
        public IEnumerable<DataComparativeAnalysisChartResponse> Data { get; set; }

    }

    public class DataComparativeAnalysisChartResponse
    {
        public double Percentage { get; set; }
        public string Name { get; set; }
        public ClickComparativeAnalysisChartResponse Click { get; set; }
    }

    public class ClickComparativeAnalysisChartResponse
    {
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public object CategoryId { get; set; }
    }

    public class BaseSalaryComparativeAnalysisChartDTO
    {
        public double? FinalSalarySM { get; set; }
        public int? InternalFrequency { get; set; }
        public long CompanyId { get; set; }
        public long ProfileId { get; set; }
        public long LevelId { get; set; }
        //public string Area { get; set; }
        public string Parameter { get; set; }
        //public string Parameter02 { get; set; }
        //public string Parameter03 { get; set; }
        public long GSM { get; set; }
        public double PeopleFrontMidPoint { get; set; } = 0;
        public double MidPointToMaximum { get; set; } = 0;
        public double MidpointToMinimum { get; set; } = 0;
        public long PositionId { get; set; }
        public double HoursBase { get; set; }
        public double HoursBaseSM { get; set; }
    }

    public class PositionSalaryComparativeAnalysisChartDTO
    {
        public long GroupId { get; set; }
        public string Parameter { get; set; }
        public long GSM { get; set; }
        public long PositionId { get; set; }
        public int? LevelId { get; set; }
        public double HoursBaseSM { get; set; }
    }

    public class GroupXTableComparativeAnalysisChartDTO
    {
        public long GroupId { get; set; }
        public long TableId { get; set; }
        public long CompanyId { get; set; }
    }
    public class SalaryTableValuesComparativeAnalysisChartDTO
    {
        public long TableId { get; set; }
        public double Midpoint { get; set; }
        public int GSM { get; set; }
        public IEnumerable<long> GroupIds { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
    }

    public class ResultComparativeAnalysisChartDTO
    {
        public double Percentagem { get; set; }
        public ComparativeAnalyseChartEnum ComparativeAnalyseEnum { get; set; }
        public string ItemGrouped { get; set; }
        public object CategoryId { get; set; }

    }

    public class ResultListGroupByComparativeAnalysisChartDTO
    {

        public string Name { get; set; }
        public string Id { get; set; }
        public IEnumerable<BaseSalaryComparativeAnalysisChartDTO> Values { get; set; }
    }
    public class TotalValueDTO
    {
        public double MidPointToMaximum { get; set; }
        public double MidpointToMinimum { get; set; }
        public double PeopleFrontMidPoint { get; set; }
    }

    public class GetComparativeAnalysisChartHandler
        : IRequestHandler<GetComparativeAnalysisChartRequest, GetResultComparativeAnalysisChartResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IEnumerable<ComparativeAnalyseChartEnum> _listEnum;

        public GetComparativeAnalysisChartHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _listEnum = Enum.GetValues(typeof(ComparativeAnalyseChartEnum)) as
                IEnumerable<ComparativeAnalyseChartEnum>;

            _listEnum = _listEnum.First().GetWithOrder().Select(s =>
            (ComparativeAnalyseChartEnum)Enum.Parse(typeof(ComparativeAnalyseChartEnum), s));
        }
        public async Task<GetResultComparativeAnalysisChartResponse> Handle(GetComparativeAnalysisChartRequest request, CancellationToken cancellationToken)
        {

            var result = await GetResultConsolidated(request);

            if (!result.Any())
                return new GetResultComparativeAnalysisChartResponse();

            var totalValues = new TotalValueDTO
            {
                PeopleFrontMidPoint = result.Average(a => a.PeopleFrontMidPoint),
                MidpointToMinimum = result.Min(a => a.MidpointToMinimum),
                MidPointToMaximum = result.Max(a => a.MidPointToMaximum)
            };

            if (request.DisplayBy == DisplayByPositioningEnum.ProfileId)
                return FitData(request, await Profile(request, result), totalValues);
            if (request.DisplayBy == DisplayByPositioningEnum.LevelId)
                return FitData(request, await Level(request, result), totalValues);
            if (request.DisplayBy == DisplayByPositioningEnum.Area ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter01 ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter02 ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter03)
                return FitData(request, Parameter(request, result), totalValues);


            return FitData(request, await Profile(request, result), totalValues);
        }
        private async Task<List<ResultListGroupByComparativeAnalysisChartDTO>> Profile(GetComparativeAnalysisChartRequest request,
                        List<BaseSalaryComparativeAnalysisChartDTO> result)
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
                .Select(s => new ResultListGroupByComparativeAnalysisChartDTO
                {
                    Name = groupsProfiles.FirstOrDefault(f => f.Id == s.key)?.Name,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().ProfileId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private async Task<List<ResultListGroupByComparativeAnalysisChartDTO>> Level(GetComparativeAnalysisChartRequest request,
                        List<BaseSalaryComparativeAnalysisChartDTO> result)
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
                .Select(s => new ResultListGroupByComparativeAnalysisChartDTO
                {
                    Name = s.key,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().LevelId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private List<ResultListGroupByComparativeAnalysisChartDTO> Parameter(GetComparativeAnalysisChartRequest request,
            List<BaseSalaryComparativeAnalysisChartDTO> result)
        {
            var areas = result
                        .GroupBy(g => g.Parameter, (key, value) =>
                        new ResultListGroupByComparativeAnalysisChartDTO
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

        private GetResultComparativeAnalysisChartResponse FitData(GetComparativeAnalysisChartRequest request,
            IEnumerable<ResultListGroupByComparativeAnalysisChartDTO> resultList,
            TotalValueDTO totalValue)
        {
            var totalValueDto = new TotalValueDTO
            {
                MidPointToMaximum = totalValue.MidPointToMaximum,
                MidpointToMinimum = totalValue.MidpointToMinimum,
                PeopleFrontMidPoint = totalValue.PeopleFrontMidPoint
            };

            if (!resultList.Any())
                return new GetResultComparativeAnalysisChartResponse();

            var resultComparativeAnalysisDTOList = new List<ResultComparativeAnalysisChartDTO>();

            foreach (var itemResult in resultList)
            {
                var values = itemResult.Values;

                foreach (ComparativeAnalyseChartEnum itemEnum in _listEnum)
                {

                    double percentage = 0;
                    if (itemEnum.Equals(ComparativeAnalyseChartEnum.PeopleFrontMidPoint))
                        percentage = values.Average(s => s.PeopleFrontMidPoint);
                    else
                    {

                        var x = Expression.Parameter(typeof(BaseSalaryComparativeAnalysisChartDTO), "x");
                        var body = Expression.PropertyOrField(x, itemEnum.ToString());
                        var lambda = Expression.Lambda<Func<BaseSalaryComparativeAnalysisChartDTO, double>>(body, x);

                        percentage = values.Select(lambda.Compile()).Average();

                    }


                    resultComparativeAnalysisDTOList.Add(new ResultComparativeAnalysisChartDTO
                    {
                        Percentagem = percentage,
                        ComparativeAnalyseEnum = itemEnum,
                        ItemGrouped = itemResult.Name,
                        CategoryId = ConverterObject.TryConvertToInt(itemResult.Id)
                    });

                }

            }

            var categories =
                resultList.Map().ToANew<IEnumerable<FilterComparativeAnalysisChartResponse>>().ToList();

            totalValue.PeopleFrontMidPoint = resultComparativeAnalysisDTOList
                                                  .Where(x => x.ComparativeAnalyseEnum == ComparativeAnalyseChartEnum.PeopleFrontMidPoint)
                                                  .Average(x => x.Percentagem);
            resultComparativeAnalysisDTOList = AddAll(request,
                resultComparativeAnalysisDTOList,
                categories,
                totalValueDto);

            var average = totalValue.PeopleFrontMidPoint;

            //fit response 
            return new GetResultComparativeAnalysisChartResponse
            {
                Categories = categories,
                Chart = new AreaChartComparativeAnalysisChartResponse
                {
                    Average = Math.Round(totalValueDto.PeopleFrontMidPoint * 100, 0),
                    Chart = resultComparativeAnalysisDTOList.GroupBy(g => g.ComparativeAnalyseEnum,
                            (key, value) => new GetComparativeAnalysisChartResponse
                            {
                                Name = key.GetDescription(),
                                Type = key,
                                Data = value.Select(s => new DataComparativeAnalysisChartResponse
                                {
                                    Name = s.ItemGrouped,
                                    Percentage = Math.Round(s.Percentagem * 100, 0),
                                    Click = new ClickComparativeAnalysisChartResponse
                                    {
                                        CategoryId = s.CategoryId,
                                        DisplayBy = request.DisplayBy,
                                        Scenario = request.Scenario,
                                        UnitId = request.UnitId
                                    }
                                })
                            })
                }
            };
        }

        private List<ResultComparativeAnalysisChartDTO> AddAll(GetComparativeAnalysisChartRequest request,
            List<ResultComparativeAnalysisChartDTO> resultComparativeAnalysisDTOList,
            List<FilterComparativeAnalysisChartResponse> categories,
            TotalValueDTO totalValue)
        {
            var isNumeric = Regex.IsMatch(categories.FirstOrDefault().Id, @"\d");
            var allLabel = isNumeric ? ((int)FilterAllEnum.All).ToString() : FilterAllEnum.All.ToString();

            if (!request.CategoriesExp.Any(a => a.ToString().Equals(allLabel)))
            {
                //add Total 

                var totalResultComparativeAnalysisDTOList = new List<ResultComparativeAnalysisChartDTO>();

                foreach (ComparativeAnalyseChartEnum itemEnum in _listEnum)
                {
                    switch (itemEnum)
                    {
                        case ComparativeAnalyseChartEnum.MidPointToMaximum:
                            totalResultComparativeAnalysisDTOList.Add(new ResultComparativeAnalysisChartDTO
                            {
                                ComparativeAnalyseEnum = itemEnum,
                                Percentagem = totalValue.MidPointToMaximum,
                                ItemGrouped = FilterAllEnum.All.GetDescription(),
                                CategoryId = ConverterObject.TryConvertToInt(allLabel)
                            });
                            break;
                        case ComparativeAnalyseChartEnum.MidpointToMinimum:
                            totalResultComparativeAnalysisDTOList.Add(new ResultComparativeAnalysisChartDTO
                            {
                                ComparativeAnalyseEnum = itemEnum,
                                Percentagem = totalValue.MidpointToMinimum,
                                ItemGrouped = FilterAllEnum.All.GetDescription(),
                                CategoryId = ConverterObject.TryConvertToInt(allLabel)
                            });
                            break;
                        case ComparativeAnalyseChartEnum.PeopleFrontMidPoint:
                            totalResultComparativeAnalysisDTOList.Add(new ResultComparativeAnalysisChartDTO
                            {
                                ComparativeAnalyseEnum = itemEnum,
                                Percentagem = totalValue.PeopleFrontMidPoint,
                                ItemGrouped = FilterAllEnum.All.GetDescription(),
                                CategoryId = ConverterObject.TryConvertToInt(allLabel)
                            });
                            break;
                        default:
                            totalResultComparativeAnalysisDTOList.Add(new ResultComparativeAnalysisChartDTO
                            {
                                ComparativeAnalyseEnum = itemEnum,
                                Percentagem = totalValue.PeopleFrontMidPoint,
                                ItemGrouped = FilterAllEnum.All.GetDescription(),
                                CategoryId = ConverterObject.TryConvertToInt(allLabel)
                            });
                            break;
                    }
                }

                //merge total + the other values 
                resultComparativeAnalysisDTOList =
                    resultComparativeAnalysisDTOList.Union(totalResultComparativeAnalysisDTOList)
                    .ToList();

                categories.Add(new FilterComparativeAnalysisChartResponse
                {

                    Id = allLabel,
                    Name = FilterAllEnum.All.GetDescription()

                });
            }

            return resultComparativeAnalysisDTOList;
        }

        private async Task<List<BaseSalaryComparativeAnalysisChartDTO>> GetResultConsolidated(GetComparativeAnalysisChartRequest request)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            IEnumerable<BaseSalaryComparativeAnalysisChartDTO> result =
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
                return new List<BaseSalaryComparativeAnalysisChartDTO>();

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
                return new List<BaseSalaryComparativeAnalysisChartDTO>();

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
                                tableIds.Safe().Any() && tableIds.Contains(st.TableId)).ToList();

                if (salaryTable.Count > 0)
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

                    //MinTrack
                    salaryBasePositionSM.MidpointToMinimum = salaryTable
                        .SelectMany(s => s.Tracks)
                        .Where(s => s.TrackId >= minTrackId &&
                                s.TrackId <= maxTrackId)
                        .Min(m => m.FactorMulti);

                    //MaxTrack
                    salaryBasePositionSM.MidPointToMaximum = salaryTable
                        .SelectMany(s => s.Tracks)
                        .Where(s => s.TrackId >= minTrackId &&
                                s.TrackId <= maxTrackId)
                        .Max(m => m.FactorMulti);
                }
            }

            return result.ToList();
        }
        private async Task<IEnumerable<BaseSalaryComparativeAnalysisChartDTO>> GetSalaryBaseXPositionSM(GetComparativeAnalysisChartRequest request,
            PermissionJson permissionUser)
        {
            IEnumerable<BaseSalaryComparativeAnalysisChartDTO> salaryBaseList =
                        new List<BaseSalaryComparativeAnalysisChartDTO>();


            var levelsExp = permissionUser.Levels;
            var groupsExp =
                        permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;
            var areaExp = permissionUser.Areas.ToList();
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
                         .Select(s => new BaseSalaryComparativeAnalysisChartDTO
                         {
                             FinalSalarySM = s.SalarioFinalSm,
                             CompanyId = s.EmpresaId.Value,
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionId = s.CargoIdSMMM.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0)
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
                         .Select(s => new BaseSalaryComparativeAnalysisChartDTO
                         {
                             FinalSalarySM = s.SalarioFinalSm,
                             CompanyId = s.EmpresaId.Value,
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionId = s.CargoIdSMMI.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0)
                         }));

            if (!salaryBaseList.Any())
                return new List<BaseSalaryComparativeAnalysisChartDTO>();

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
                Select(s => new PositionSalaryComparativeAnalysisChartDTO
                {
                    PositionId = s.CargoProjetoSmidLocal,
                    Parameter = s.CargosProjetoSMParametrosMapeamento.Any(f => f.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal) ? s.CargosProjetoSMParametrosMapeamento.FirstOrDefault(f => f.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal).ParametrosProjetosSMLista.ParametrosProjetosSMTipos.Parametro : string.Empty,
                    GroupId = s.GrupoSmidLocal,
                    GSM = s.CargosProjetosSmmapeamento.Any(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal) ? (long)s.CargosProjetosSmmapeamento.FirstOrDefault(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal).Gsm : 0,
                    LevelId = s.CmcodeNavigation != null ? s.CmcodeNavigation.NivelId : (int?)null
                }));

            foreach (var salary in salaryBaseList)
            {
                var positionValues = positionsSM.FirstOrDefault(f => f.PositionId == salary.PositionId);

                salary.Parameter = positionValues.Parameter;
                salary.ProfileId = positionValues.GroupId;
                salary.GSM = positionValues.GSM;
                salary.LevelId = positionValues.LevelId ?? salary.LevelId;
                salary.HoursBaseSM = positionValues.HoursBaseSM;
            }

            return salaryBaseList.Where(s => (!groupsExp.Safe().Any() || !groupsExp.Contains(s.ProfileId)) &&
                (!areaExp.Safe().Any()));//|| !areaExp.Contains(s.Parameter)));
        }
        private double GetPercentageMidPoint(double? midPointValue, double? finalSalary)
        {
            if (midPointValue.GetValueOrDefault(0) == 0 || finalSalary.GetValueOrDefault(0) == 0) return 0;

            return finalSalary.Value / midPointValue.Value;
        }
    }
}
