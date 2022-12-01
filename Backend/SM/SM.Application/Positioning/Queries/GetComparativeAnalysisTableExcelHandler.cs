using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors;
using SM.Application.Interactors.Interfaces;
using SM.Application.Interactors.Utils;
using SM.Domain.Common;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using SM.Domain.Helpers;
using SM.Domain.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Positioning.Queries
{

    public class GetComparativeAnalysisTableExcelRequest :
        IRequest<GetComparativeAnalysisTableExcelResponse>
    {
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
        public IEnumerable<object> CategoriesExp { get; set; } = new List<object>(); //using to share (EXP)

    }

    public class GetComparativeAnalysisTableExcelResponse
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }

    }

    public class BaseSalaryComparativeAnalysisTableExcelDTO
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

    public class PositionSalaryComparativeAnalysisTableExcelDTO
    {
        public long GroupId { get; set; }
        public string Parameter { get; set; }
        public long GSM { get; set; }
        public long PositionId { get; set; }
        public string CarrerAxis { get; set; }
        public int? LevelId { get; set; }
        public double HoursBaseSM { get; set; }
    }

    public class ResultListGroupByComparativeAnalysisTableExcelDTO
    {

        public string Name { get; set; }
        public string Id { get; set; }
        public IEnumerable<BaseSalaryComparativeAnalysisTableExcelDTO> Values { get; set; }
    }

    public class ComparativeExcelBody
    {
        public string Value { get; set; }
    }

    public class ComparativeExcelHeader
    {
        public ExcelFieldType @Type { get; set; } = ExcelFieldType.Default;
        public string Value { get; set; }
        public string GroupName { get; set; }
        public int? Width { get; set; } = null;
        public bool IsBold { get; set; } = false;

    }

    public class ColorExcel
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public string Color { get; set; }
    }
    public class ComparativeExcelHeaderIndexDTO
    {

        public ComparativeExcelHeader Value { get; set; }
        public int Index { get; set; }
    }
    public class GetComparativeAnalysisTableExcelHandler : IRequestHandler<GetComparativeAnalysisTableExcelRequest, GetComparativeAnalysisTableExcelResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IEnumerable<ComparativeAnalyseTableExcelEnum> _listEnums;
        private readonly InfoApp _infoApp;
        private readonly IGenerateExcelFileInteractor _generateExcelFileInteractor;

        public GetComparativeAnalysisTableExcelHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            IGenerateExcelFileInteractor generateExcelFileInteractor,
            InfoApp infoApp)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _listEnums = Enum.GetValues(typeof(ComparativeAnalyseTableExcelEnum))
                as IEnumerable<ComparativeAnalyseTableExcelEnum>;

            _listEnums = _listEnums.First().GetWithOrder().Select(s =>
            (ComparativeAnalyseTableExcelEnum)Enum.Parse(typeof(ComparativeAnalyseTableExcelEnum), s));

            _infoApp = infoApp;
            _generateExcelFileInteractor = generateExcelFileInteractor;
        }
        public async Task<GetComparativeAnalysisTableExcelResponse> Handle(GetComparativeAnalysisTableExcelRequest request, CancellationToken cancellationToken)
        {

            var result = await GetResultConsolidated(request);

            if (!result.Any()) return new GetComparativeAnalysisTableExcelResponse();

            var axisCarrer = result.Select(s => s.CareerAxis)
                .OrderBy(o => o).Distinct();

            switch (request.DisplayBy)
            {
                case DisplayByPositioningEnum.ProfileId:
                    return FitData(request,
                        await Profile(request, result),
                        axisCarrer, DisplayByPositioningEnum.ProfileId);

                case DisplayByPositioningEnum.LevelId:
                    return FitData(request, await Level(request, result),
                        axisCarrer, DisplayByPositioningEnum.LevelId);

                case DisplayByPositioningEnum.Area:
                    return FitData(request, Parameter(request, result),
                        axisCarrer, DisplayByPositioningEnum.Area);

                case DisplayByPositioningEnum.Parameter01:
                    return FitData(request, Parameter(request, result),
                        axisCarrer, DisplayByPositioningEnum.Area);

                case DisplayByPositioningEnum.Parameter02:
                    return FitData(request, Parameter(request, result),
                        axisCarrer, DisplayByPositioningEnum.Area);

                case DisplayByPositioningEnum.Parameter03:
                    return FitData(request, Parameter(request, result),
                        axisCarrer, DisplayByPositioningEnum.Area);

                default:
                    return FitData(request, Parameter(request, result),
                        axisCarrer, DisplayByPositioningEnum.Area);
            }


        }
        private GetComparativeAnalysisTableExcelResponse FitData(GetComparativeAnalysisTableExcelRequest request,
        IEnumerable<ResultListGroupByComparativeAnalysisTableExcelDTO> resultList,
        IEnumerable<string> axisCarrer, DisplayByPositioningEnum filter)
        {
            if (!resultList.Any())
                return new GetComparativeAnalysisTableExcelResponse();

            var isAddTotal = !request.CategoriesExp.Any(a => a.ToString().Equals(FilterAllEnum.All.ToString()) ||
                                                             a.ToString().Equals(((int)FilterAllEnum.All).ToString()));

            var headerExcel = GetHeaderExcel(axisCarrer, filter);
            var bodyExcel = GetBodyExcel(resultList, axisCarrer);

            var sheetName = $"{ModulesEnum.Positioning.GetDescription()}_{ModulesSuItemsEnum.ComparativeAnalysis.GetDescription()}";
            var fileName = $"{_infoApp.Name}_{sheetName}";

            var file = _generateExcelFileInteractor.GenerateCustomHandler(new GenerateExcelFileRequest
            {
                Body = bodyExcel,
                Header = headerExcel,
                FileName = fileName,
                SheetName = sheetName,
                TitleSheet = sheetName
            }, isAddTotal);

            return new GetComparativeAnalysisTableExcelResponse
            {
                File = file,
                FileName = fileName
            };
        }

        private List<List<ExcelBody>> GetBodyExcel(IEnumerable<ResultListGroupByComparativeAnalysisTableExcelDTO> resultList,
            IEnumerable<string> axisCarrer)
        {
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

            var result = new List<List<ExcelBody>>();
            var totalAmountEmployees = new List<int>();
            var totalAmountPositions = new List<int>();
            var dicMidPointAxisCareer = new Dictionary<string, List<double>>();
            var dicFrequencyAxisCareer = new Dictionary<string, List<int>>();
            var sumGeneralPeoplefrontMidPoint = new List<double>();
            var sumGeneralInternalFrequency = new List<int>();

            foreach (var row in resultList)
            {
                var cols = new List<ExcelBody>();
                var amountEmployees =
                            row.Values.Sum(s => s.InternalFrequency);

                var amountPositions = row.Values.GroupBy(g => g.CMCode).Count();

                foreach (var col in _listEnums)
                {
                    switch (col)
                    {
                        case ComparativeAnalyseTableExcelEnum.Filter:
                            cols.Add(new ExcelBody
                            {
                                Value = row.Name,
                            });
                            break;
                        case ComparativeAnalyseTableExcelEnum.PeopleAmount:
                            totalAmountEmployees.Add(amountEmployees.GetValueOrDefault(0));
                            cols.Add(new ExcelBody
                            {
                                Value = amountEmployees.GetValueOrDefault(0).ToString(CultureInfo.InvariantCulture),
                            });

                            break;
                        case ComparativeAnalyseTableExcelEnum.PeoplePercentage:
                            cols.Add(new ExcelBody
                            {
                                Value = amountEmployees.HasValue ?
                                (amountEmployees.Value / (double)totalEmployee).ToString(CultureInfo.InvariantCulture) : "0"
                            });
                            break;
                        case ComparativeAnalyseTableExcelEnum.PositionsAmount:
                            totalAmountPositions.Add(amountPositions);
                            cols.Add(new ExcelBody
                            {
                                Value = amountPositions.ToString(CultureInfo.InvariantCulture)
                            });
                            break;
                        case ComparativeAnalyseTableExcelEnum.PositionsPercentage:
                            cols.Add(new ExcelBody
                            {
                                Value = amountPositions == 0 ? "0" :
                                (amountPositions / (double)totalPositions)
                                .ToString(CultureInfo.InvariantCulture)
                            });
                            break;
                        case ComparativeAnalyseTableExcelEnum.General:
                            var peopleMidpoint = row.Values.Average(s => Convert.ToDouble(s.PeopleFrontMidPoint));
                            sumGeneralPeoplefrontMidPoint.Add(row.Values.Sum(x => x.PeopleFrontMidPoint));
                            sumGeneralInternalFrequency.Add(row.Values.Sum(x => x.InternalFrequency.GetValueOrDefault(0)));

                            cols.Add(new ExcelBody
                            {
                                Value = peopleMidpoint.ToString(CultureInfo.InvariantCulture)
                            });
                            break;
                        case ComparativeAnalyseTableExcelEnum.AxisCarrer:
                            foreach (var ac in axisCarrer)
                            {
                                var filterPeopleMidpoint = row.Values
                                     .Where(v => v.CareerAxis.Equals(ac));

                                var peopleMidpointByAxisCarrer = filterPeopleMidpoint.Safe().Any() ?
                                    filterPeopleMidpoint?.Average(a => a.PeopleFrontMidPoint) :
                                    null;
                                var sumPeopleMidPoint = filterPeopleMidpoint.Safe().Any() ?
                                    filterPeopleMidpoint?.Sum(a => a.PeopleFrontMidPoint) :
                                    null;

                                var sumFrequency = filterPeopleMidpoint.Safe().Any() ?
                                    filterPeopleMidpoint?.Sum(a => a.InternalFrequency) :
                                    null;

                                if (!dicMidPointAxisCareer.ContainsKey(ac))
                                {
                                    dicMidPointAxisCareer.Add(ac, new List<double> { sumPeopleMidPoint.GetValueOrDefault(0) });
                                    dicFrequencyAxisCareer.Add(ac, new List<int> { sumFrequency.GetValueOrDefault(0) });
                                }
                                else
                                {
                                    dicMidPointAxisCareer[ac].Add(sumPeopleMidPoint.GetValueOrDefault(0));
                                    dicFrequencyAxisCareer[ac].Add(sumFrequency.GetValueOrDefault(0));
                                }

                                cols.Add(new ExcelBody
                                {
                                    Value = peopleMidpointByAxisCarrer
                                                        .GetValueOrDefault(0).ToString(CultureInfo.InvariantCulture)
                                });
                            }
                            break;
                        default:
                            cols.Add(new ExcelBody
                            {
                                Value = string.Empty
                            });
                            break;
                    }
                }
                result.Add(cols);

            }

            var all = CreateAllRowColumn(dicMidPointAxisCareer,
                dicFrequencyAxisCareer,
                sumGeneralPeoplefrontMidPoint,
                sumGeneralInternalFrequency,
                totalAmountEmployees,
                totalAmountPositions,
                axisCarrer);

            if (all.Count > 0)
                result.Add(all);
            return result;
        }

        private List<ExcelHeader> GetHeaderExcel(IEnumerable<string> axisCarrer,
            DisplayByPositioningEnum filter)
        {
            var result = new List<ExcelHeader>();

            foreach (var item in _listEnums)
            {
                switch (item)
                {
                    case ComparativeAnalyseTableExcelEnum.Filter:
                        result.Add(new ExcelHeader
                        {
                            Value = filter.GetDescription(),
                            IsBold = true,
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.PeopleAmount:
                        result.Add(new ExcelHeader
                        {
                            Value = item.GetDescription(),
                            Type = ExcelFieldType.NumberSimples,
                            GroupName = ComparativeAnalyseTableEnum.People.GetDescription()
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.PeoplePercentage:
                        result.Add(new ExcelHeader
                        {
                            Value = item.GetDescription(),
                            Type = ExcelFieldType.Percentagem,
                            GroupName = ComparativeAnalyseTableEnum.People.GetDescription()
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.PositionsAmount:
                        result.Add(new ExcelHeader
                        {
                            Value = item.GetDescription(),
                            Type = ExcelFieldType.NumberSimples,
                            GroupName = ComparativeAnalyseTableEnum.Positions.GetDescription()
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.PositionsPercentage:
                        result.Add(new ExcelHeader
                        {
                            Value = item.GetDescription(),
                            Type = ExcelFieldType.Percentagem,
                            GroupName = ComparativeAnalyseTableEnum.Positions.GetDescription()
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.General:
                        result.Add(new ExcelHeader
                        {
                            Value = item.GetDescription(),
                            Type = ExcelFieldType.Percentagem,
                            IsBold = true
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.AxisCarrer:
                        foreach (var ac in axisCarrer)
                        {
                            result.Add(new ExcelHeader
                            {
                                Value = ac,
                                Type = ExcelFieldType.Percentagem,
                                IsBold = true
                            });
                        }
                        continue;
                    default:
                        result.Add(new ExcelHeader
                        {
                            Value = filter.GetDescription(),
                        });
                        break;
                }
            }

            return result;
        }

        private async Task<List<ResultListGroupByComparativeAnalysisTableExcelDTO>> Profile(GetComparativeAnalysisTableExcelRequest request,
        List<BaseSalaryComparativeAnalysisTableExcelDTO> result)
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
                .Select(s => new ResultListGroupByComparativeAnalysisTableExcelDTO
                {
                    Name = groupsProfiles.FirstOrDefault(f => f.Id == s.key)?.Name,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().ProfileId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private async Task<List<ResultListGroupByComparativeAnalysisTableExcelDTO>> Level(GetComparativeAnalysisTableExcelRequest request,
                        List<BaseSalaryComparativeAnalysisTableExcelDTO> result)
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
                .Select(s => new ResultListGroupByComparativeAnalysisTableExcelDTO
                {
                    Name = s.key,
                    Values = s.value,
                    Id = s.value.FirstOrDefault().LevelId.ToString()
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();
        }
        private List<ResultListGroupByComparativeAnalysisTableExcelDTO> Parameter(GetComparativeAnalysisTableExcelRequest request,
            List<BaseSalaryComparativeAnalysisTableExcelDTO> result)
        {
            var areas = result
                        .GroupBy(g => g.Parameter, (key, value) =>
                        new ResultListGroupByComparativeAnalysisTableExcelDTO
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

        private async Task<List<BaseSalaryComparativeAnalysisTableExcelDTO>>
        GetResultConsolidated(GetComparativeAnalysisTableExcelRequest request)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            IEnumerable<BaseSalaryComparativeAnalysisTableExcelDTO> result =
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
          request.CompaniesId.Contains(g.EmpresaId) &&
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
                return new List<BaseSalaryComparativeAnalysisTableExcelDTO>();

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
                return new List<BaseSalaryComparativeAnalysisTableExcelDTO>();

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
                                    .Where(f => f.CompanyId == companyId && f.GroupId == groupId);

                var tableIds = mapGroup?.Select(s => s.TableId);

                var maxTrackId = mapGroup?
                                .Where(s => tableIds.Safe().Any() && tableIds.Contains(s.TableId))?
                                .Max(m => m.MaxTrackId);

                var minTrackId = mapGroup?
                                .Where(s => tableIds.Safe().Any() && tableIds.Contains(s.TableId))?
                                .Min(m => m.MinTrackId);

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
        private async Task<IEnumerable<BaseSalaryComparativeAnalysisTableExcelDTO>> GetSalaryBaseXPositionSM(GetComparativeAnalysisTableExcelRequest request,
            PermissionJson permissionUser)
        {
            IEnumerable<BaseSalaryComparativeAnalysisTableExcelDTO> salaryBaseList =
                        new List<BaseSalaryComparativeAnalysisTableExcelDTO>();

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
                         .Select(s => new BaseSalaryComparativeAnalysisTableExcelDTO
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
                         .Select(s => new BaseSalaryComparativeAnalysisTableExcelDTO
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
                return new List<BaseSalaryComparativeAnalysisTableExcelDTO>();

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
                Select(s => new PositionSalaryComparativeAnalysisTableExcelDTO
                {
                    PositionId = s.CargoProjetoSmidLocal,
                    Parameter = s.CargosProjetoSMParametrosMapeamento.Any(f => f.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal) ? s.CargosProjetoSMParametrosMapeamento.FirstOrDefault(f => f.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal).ParametrosProjetosSMLista.ParametrosProjetosSMTipos.Parametro : string.Empty,
                    GroupId = s.GrupoSmidLocal,
                    GSM = s.CargosProjetosSmmapeamento.Any(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal) ? (long)s.CargosProjetosSmmapeamento.FirstOrDefault(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal).Gsm : 0,

                    //CarrerAxis = s.EixoCarreira,
                    LevelId = s.CmcodeNavigation != null ? s.CmcodeNavigation.NivelId : (int?)null,
                    HoursBaseSM = s.BaseHoraria
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

        private List<ExcelBody> CreateAllRowColumn(Dictionary<string, List<double>> dicMidPointAxisCareer,
        Dictionary<string, List<int>> dicFrequencyAxisCareer,
        List<double> sumGeneralPeoplefrontMidPoint,
        List<int> sumGeneralInternalFrequency,
        List<int> totalAmountEmployees,
        List<int> totalAmountPositions,
        IEnumerable<string> axisCarrer)
        {
            var cols = new List<ExcelBody>();
            foreach (var col in _listEnums)
            {
                switch (col)
                {
                    case ComparativeAnalyseTableExcelEnum.Filter:
                        cols.Add(new ExcelBody
                        {
                            Value = FilterAllEnum.All.GetDescription(),
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.General:

                        double allGeneralPeopleFrontMidPoint = sumGeneralPeoplefrontMidPoint.Sum(value => value);
                        int allGeneralFrequency = sumGeneralInternalFrequency.Sum(freq => freq);

                        double average = allGeneralFrequency > 0 ? (allGeneralPeopleFrontMidPoint / allGeneralFrequency) : 0;

                        cols.Add(new ExcelBody
                        {
                            Value = average.ToString(CultureInfo.InvariantCulture)
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.PeopleAmount:
                        cols.Add(new ExcelBody
                        {
                            Value = totalAmountEmployees.Sum(emp => emp).ToString()
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.PeoplePercentage:
                        cols.Add(new ExcelBody
                        {
                            Value = totalAmountEmployees.Sum(emp => emp).ToString()
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.PositionsPercentage:
                        cols.Add(new ExcelBody
                        {
                            Value = totalAmountPositions.Sum(pos => pos).ToString()
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.PositionsAmount:
                        cols.Add(new ExcelBody
                        {
                            Value = totalAmountPositions.Sum(pos => pos).ToString()
                        });
                        break;
                    case ComparativeAnalyseTableExcelEnum.AxisCarrer:
                        foreach (var ac in axisCarrer)
                        {
                            if (dicMidPointAxisCareer.ContainsKey(ac) && dicFrequencyAxisCareer.ContainsKey(ac))
                            {
                                int sumFrequency = dicFrequencyAxisCareer[ac].Sum(val => val);
                                double sumPeopleMidPoint = dicMidPointAxisCareer[ac].Sum(val => val);
                                double averageAxisCarrer = sumFrequency > 0 ?
                                (sumPeopleMidPoint / sumFrequency) : 0;
                                cols.Add(new ExcelBody
                                {
                                    Value = averageAxisCarrer.ToString(CultureInfo.InvariantCulture)
                                });
                            }
                        }
                        break;
                }
            }
            return cols;
        }
    }
}
