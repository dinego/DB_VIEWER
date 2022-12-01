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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.DashBoard.Queries
{

    public class GetComparativeAnalysisDashRequest :
    IRequest<GetComparativeAnalysisDashResponse>
    {
        public long UserId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
    }

    public class GetComparativeAnalysisDashResponse
    {
        public AreaChartComparativeAnalysisChartDashResponse Chart { get; set; }
    }

    public class AreaChartComparativeAnalysisChartDashResponse
    {
        public double Average { get; set; } = 0;
        public IEnumerable<GetComparativeAnalysisDashResult> Chart { get; set; }
    }

    public class GetComparativeAnalysisDashResult
    {
        public string Name { get; set; }
        public ComparativeAnalyseChartEnum Type { get; set; }
        public IEnumerable<DataComparativeAnalysisDashResponse> Data { get; set; }

    }

    public class DataComparativeAnalysisDashResponse
    {
        public double Percentage { get; set; }
        public string Name { get; set; }
    }

    public class BaseSalaryComparativeAnalysisDashDTO
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
        public double PeopleFrontMidPoint { get; set; } = 0;
        public double MidPointToMaximum { get; set; } = 0;
        public double MidpointToMinimum { get; set; } = 0;
        public long PositionId { get; set; }
        public double HoursBase { get; set; }
        public double HoursBaseSM { get; set; }
    }

    public class ResultComparativeAnalysisDashDTO
    {
        public double Percentagem { get; set; }
        public ComparativeAnalyseChartEnum ComparativeAnalyseEnum { get; set; }
        public string ItemGrouped { get; set; }

    }

    public class ResultListGroupByComparativeAnalysisDashDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<BaseSalaryComparativeAnalysisDashDTO> Values { get; set; }
    }

    public class PositionSalaryComparativeAnalysisDashDTO
    {
        public long GroupId { get; set; }
        public string Area { get; set; }
        public string Parameter01 { get; set; }
        public string Parameter02 { get; set; }
        public string Parameter03 { get; set; }
        public long GSM { get; set; }
        public long PositionId { get; set; }
        public double HoursBaseSM { get; set; }
    }

    public class TotalValueDTO
    {
        public double MidPointToMaximum { get; set; }
        public double MidpointToMinimum { get; set; }
        public double PeopleFrontMidPoint { get; set; }
    }
    public class GetComparativeAnalysisDashHandler
         : IRequestHandler<GetComparativeAnalysisDashRequest, GetComparativeAnalysisDashResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IEnumerable<ComparativeAnalyseChartEnum> _listEnum;
        public GetComparativeAnalysisDashHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _listEnum = Enum.GetValues(typeof(ComparativeAnalyseChartEnum)) as
                IEnumerable<ComparativeAnalyseChartEnum>;

            _listEnum = _listEnum.First().GetWithOrder().Select(s =>
            (ComparativeAnalyseChartEnum)Enum.Parse(typeof(ComparativeAnalyseChartEnum), s));
        }
        public async Task<GetComparativeAnalysisDashResponse> Handle(GetComparativeAnalysisDashRequest request, CancellationToken cancellationToken)
        {
            var result = await GetResultConsolidated(request);

            if (!result.Any())
                return new GetComparativeAnalysisDashResponse();

            var totalValues = new TotalValueDTO
            {
                PeopleFrontMidPoint = result.Average(a => a.PeopleFrontMidPoint),
                MidpointToMinimum = result.Min(a => a.MidpointToMinimum),
                MidPointToMaximum = result.Max(a => a.MidPointToMaximum)
            };

            switch (request.DisplayBy)
            {
                case DisplayByPositioningEnum.ProfileId:
                    return FitData(request, await Profile(request, result), totalValues);

                case DisplayByPositioningEnum.LevelId:
                    return FitData(request, await Level(request, result), totalValues);

                case DisplayByPositioningEnum.Area:
                    return FitData(request, Area(request, result), totalValues);

                case DisplayByPositioningEnum.Parameter01:
                    return FitData(request, Parameter01(request, result), totalValues);

                case DisplayByPositioningEnum.Parameter02:
                    return FitData(request, Parameter02(request, result), totalValues);

                case DisplayByPositioningEnum.Parameter03:
                    return FitData(request, Parameter03(request, result), totalValues);

                default:
                    return FitData(request, await Profile(request, result), totalValues);
            }
        }

        private async Task<List<ResultListGroupByComparativeAnalysisDashDTO>> Level(GetComparativeAnalysisDashRequest request,
                        List<BaseSalaryComparativeAnalysisDashDTO> result)
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
                .Select(s => new ResultListGroupByComparativeAnalysisDashDTO
                {
                    Name = s.key,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().LevelId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private List<ResultListGroupByComparativeAnalysisDashDTO> Area(GetComparativeAnalysisDashRequest request,
            List<BaseSalaryComparativeAnalysisDashDTO> result)
        {
            var areas = result
                        .GroupBy(g => g.Area, (key, value) =>
                        new ResultListGroupByComparativeAnalysisDashDTO
                        {
                            Name = key,
                            Id = key,
                            Values = value
                        });

            return areas.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();

        }
        private List<ResultListGroupByComparativeAnalysisDashDTO> Parameter01(GetComparativeAnalysisDashRequest request,
            List<BaseSalaryComparativeAnalysisDashDTO> result)
        {
            var parameter01 = result
                .Where(s => !string.IsNullOrWhiteSpace(s.Parameter01))?
                        .GroupBy(g => g.Parameter01, (key, value) =>
                        new ResultListGroupByComparativeAnalysisDashDTO
                        {
                            Name = key,
                            Id = key,
                            Values = value
                        });

            if (!parameter01.Safe().Any())
                return new List<ResultListGroupByComparativeAnalysisDashDTO>();

            return parameter01.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();

        }
        private List<ResultListGroupByComparativeAnalysisDashDTO> Parameter02(GetComparativeAnalysisDashRequest request,
            List<BaseSalaryComparativeAnalysisDashDTO> result)
        {
            var parameter02 = result
                .Where(s => !string.IsNullOrWhiteSpace(s.Parameter02))?
                        .GroupBy(g => g.Parameter02, (key, value) =>
                        new ResultListGroupByComparativeAnalysisDashDTO
                        {
                            Name = key,
                            Id = key,
                            Values = value
                        });

            if (!parameter02.Safe().Any())
                return new List<ResultListGroupByComparativeAnalysisDashDTO>();


            return parameter02.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();

        }
        private List<ResultListGroupByComparativeAnalysisDashDTO> Parameter03(GetComparativeAnalysisDashRequest request,
            List<BaseSalaryComparativeAnalysisDashDTO> result)
        {
            var parameter03 = result
                .Where(s => !string.IsNullOrWhiteSpace(s.Parameter03))?
                        .GroupBy(g => g.Parameter03, (key, value) =>
                        new ResultListGroupByComparativeAnalysisDashDTO
                        {
                            Name = key,
                            Id = key,
                            Values = value
                        });

            if (!parameter03.Safe().Any())
                return new List<ResultListGroupByComparativeAnalysisDashDTO>();

            return parameter03.Where(re => !string.IsNullOrWhiteSpace(re.Name)).OrderBy(o => o.Name).ToList();

        }

        private async Task<List<ResultListGroupByComparativeAnalysisDashDTO>> Profile(GetComparativeAnalysisDashRequest request,
                List<BaseSalaryComparativeAnalysisDashDTO> result)
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
                .Select(s => new ResultListGroupByComparativeAnalysisDashDTO
                {
                    Name = groupsProfiles.FirstOrDefault(f => f.Id == s.key).Name,
                    Values = s.value
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }

        private GetComparativeAnalysisDashResponse FitData(GetComparativeAnalysisDashRequest request,
            IEnumerable<ResultListGroupByComparativeAnalysisDashDTO> resultList,
            TotalValueDTO totalValue)
        {
            var resultComparativeAnalysisDTOList = new List<ResultComparativeAnalysisDashDTO>();
            var totalValueDto = new TotalValueDTO
            {
                MidPointToMaximum = totalValue.MidPointToMaximum,
                MidpointToMinimum = totalValue.MidpointToMinimum,
                PeopleFrontMidPoint = totalValue.PeopleFrontMidPoint
            };

            foreach (var itemResult in resultList)
            {
                var values = itemResult.Values;

                foreach (ComparativeAnalyseChartEnum itemEnum in _listEnum)
                {

                    double percentage = 0;
                    if (itemEnum.Equals(ComparativeAnalyseChartEnum.PeopleFrontMidPoint))
                    {
                        percentage = values
                                    .Average(s => Convert.ToDouble(s.PeopleFrontMidPoint));
                    }
                    else
                    {

                        var x = Expression.Parameter(typeof(BaseSalaryComparativeAnalysisDashDTO), "x");
                        var body = Expression.PropertyOrField(x, itemEnum.ToString());
                        var lambda = Expression.Lambda<Func<BaseSalaryComparativeAnalysisDashDTO, double>>(body, x);

                        percentage = values
                                               .Select(lambda.Compile())
                                               .Average();

                    }


                    resultComparativeAnalysisDTOList.Add(new ResultComparativeAnalysisDashDTO
                    {
                        Percentagem = percentage,
                        ComparativeAnalyseEnum = itemEnum,
                        ItemGrouped = itemResult.Name,
                    });

                }

            }
            totalValue.PeopleFrontMidPoint = resultComparativeAnalysisDTOList
                                                  .Where(x => x.ComparativeAnalyseEnum == ComparativeAnalyseChartEnum.PeopleFrontMidPoint)
                                                  .Average(x => x.Percentagem);
            //add Total 
            var totalResultComparativeAnalysisDTOList = new List<ResultComparativeAnalysisDashDTO>();
            foreach (ComparativeAnalyseChartEnum itemEnum in _listEnum)
            {
                switch (itemEnum)
                {
                    case ComparativeAnalyseChartEnum.MidPointToMaximum:
                        totalResultComparativeAnalysisDTOList.Add(new ResultComparativeAnalysisDashDTO
                        {
                            ComparativeAnalyseEnum = itemEnum,
                            Percentagem = totalValueDto.MidPointToMaximum,
                            ItemGrouped = FilterAllEnum.All.GetDescription()
                        });
                        break;
                    case ComparativeAnalyseChartEnum.MidpointToMinimum:
                        totalResultComparativeAnalysisDTOList.Add(new ResultComparativeAnalysisDashDTO
                        {
                            ComparativeAnalyseEnum = itemEnum,
                            Percentagem = totalValueDto.MidpointToMinimum,
                            ItemGrouped = FilterAllEnum.All.GetDescription()
                        });
                        break;
                    case ComparativeAnalyseChartEnum.PeopleFrontMidPoint:
                        totalResultComparativeAnalysisDTOList.Add(new ResultComparativeAnalysisDashDTO
                        {
                            ComparativeAnalyseEnum = itemEnum,
                            Percentagem = totalValueDto.PeopleFrontMidPoint,
                            ItemGrouped = FilterAllEnum.All.GetDescription()
                        });
                        break;
                    default:
                        totalResultComparativeAnalysisDTOList.Add(new ResultComparativeAnalysisDashDTO
                        {
                            ComparativeAnalyseEnum = itemEnum,
                            Percentagem = totalValueDto.PeopleFrontMidPoint,
                            ItemGrouped = FilterAllEnum.All.GetDescription()
                        });
                        break;
                }
            }

            var average = totalValue.PeopleFrontMidPoint;

            //merge total + the other values 
            var result = resultComparativeAnalysisDTOList.Union(totalResultComparativeAnalysisDTOList);

            //fit response 
            return new GetComparativeAnalysisDashResponse
            {
                Chart = new AreaChartComparativeAnalysisChartDashResponse
                {
                    Average = Math.Round(totalValueDto.PeopleFrontMidPoint * 100, 0),
                    Chart = result.GroupBy(g => g.ComparativeAnalyseEnum,
                (key, value) => new GetComparativeAnalysisDashResult
                {
                    Name = key.GetDescription(),
                    Type = key,
                    Data = value.Select(s => new DataComparativeAnalysisDashResponse
                    {
                        Name = s.ItemGrouped,
                        Percentage = Math.Round(s.Percentagem * 100, 0),
                    })
                })
                }
            };
        }
        private async Task<List<BaseSalaryComparativeAnalysisDashDTO>> GetResultConsolidated(GetComparativeAnalysisDashRequest request)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            IEnumerable<BaseSalaryComparativeAnalysisDashDTO> result =
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
                return new List<BaseSalaryComparativeAnalysisDashDTO>();

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
                return new List<BaseSalaryComparativeAnalysisDashDTO>();

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

                var tableIds = mapGroup.Select(s => s.TableId);

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

        private async Task<IEnumerable<BaseSalaryComparativeAnalysisDashDTO>> GetSalaryBaseXPositionSM(GetComparativeAnalysisDashRequest request,
            PermissionJson permissionUser)
        {
            IEnumerable<BaseSalaryComparativeAnalysisDashDTO> salaryBaseList =
            new List<BaseSalaryComparativeAnalysisDashDTO>();

            var groupsExp =
                permissionUser.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                .SubItems;

            var areaExp =
                        permissionUser.Areas;

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
                         .Select(s => new BaseSalaryComparativeAnalysisDashDTO
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
                         .Select(s => new BaseSalaryComparativeAnalysisDashDTO
                         {
                             FinalSalarySM = s.SalarioFinalSm,
                             CompanyId = s.EmpresaId.Value,
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionId = s.CargoIdSMMI.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0)
                         }));

            if (!salaryBaseList.Any())
                return new List<BaseSalaryComparativeAnalysisDashDTO>();

            var positionsIds = salaryBaseList.Select(s => s.PositionId).Distinct();

            var positionsSM = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                .GetListAsync(g => g
                .Where(cp => cp.Ativo.HasValue &&
                cp.Ativo.Value &&
                positionsIds.Contains(cp.CargoProjetoSmidLocal) && cp.ProjetoId == request.ProjectId)?.
                Select(s => new PositionSalaryComparativeAnalysisDashDTO
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

            return
                salaryBaseList.Where(s => (!groupsExp.Safe().Any() || !groupsExp.Contains(s.ProfileId)) &&
                (!areaExp.Safe().Any()));// || !areaExp.Contains(s.Area)));
        }
        private double GetPercentageMidPoint(double? midPointValue, double? finalSalary)
        {
            if (midPointValue.GetValueOrDefault(0) == 0 || finalSalary.GetValueOrDefault(0) == 0) return 0;

            return finalSalary.Value / midPointValue.Value;
        }

    }
}
