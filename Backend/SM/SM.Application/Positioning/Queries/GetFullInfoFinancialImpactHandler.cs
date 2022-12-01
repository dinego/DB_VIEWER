using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Helpers;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using SM.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Repositories.Extensions;

namespace SM.Application.Positioning.Queries
{

    public class GetFullInfoFinancialImpactRequest :
        IRequest<GetFullInfoFinancialImpactResponse>
    {
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
        public object CategoryId { get; set; }
        public AnalyseFinancialImpactEnum SerieId { get; set; } = AnalyseFinancialImpactEnum.IFAMax;
        public bool? IsAsc { get; set; } = null;
        public int? SortColumnId { get; set; } = null;
    }

    public class GetFullInfoFinancialImpactResponse
    {
        public string Category { get; set; }
        public string Serie { get; set; }
        public string Scenario { get; set; }
        public GetTableFullInfoFinancialImpactResponse Table { get; set; }
    }

    public class GetTableFullInfoFinancialImpactResponse
    {

        public IEnumerable<GetDataFullInfoFinancialImpactResponse> Header { get; set; }
        public IEnumerable<IEnumerable<GetDataFullInfoFinancialImpactResponse>> Body { get; set; }

    }

    public class GetDataFullInfoFinancialImpactResponse
    {
        public int ColPos { get; set; }
        public string Value { get; set; }
        public string @Type { get; set; }
        public bool Sortable { get; set; } = false;
        public int ColumnId { get; set; }


    }

    public class BaseSalaryFullInfoFinancialImpactDTO
    {
        public string Company { get; set; }
        public long CompanyId { get; set; }
        public string CurrentyPosition { get; set; }
        public string PositionSM { get; set; }
        public string Employee { get; set; }
        public double? Salary { get; set; }
        public long GSM { get; set; }
        public long ProfileId { get; set; }
        public long LevelId { get; set; }
        public string Parameter { get; set; }
        public AnalyseFinancialImpactEnum Type { get; set; }
        public double HoursBaseSM { get; set; }
        public double HoursBase { get; set; }
        public long PositionId { get; set; }
        public string Tables { get; set; }
        public double FinancialImpact { get; set; }
        public IEnumerable<DataSalaryTableFullInfoFinancialImpact> SalaryTable { get; set; }
    }

    public class PositionSalaryFullInfoFinancialImpactDTO
    {
        public long PositionId { get; set; }
        public long GroupId { get; set; }
        public int GSM { get; set; }
        public string Area { get; set; }
        public string Parameter01 { get; set; }
        public string Parameter02 { get; set; }
        public string Parameter03 { get; set; }
        public string PositionSM { get; set; }
        public double HoursBaseSM { get; set; }
    }

    public class DataSalaryTableFullInfoFinancialImpact
    {
        public double Multiplicator { get; set; }
        public double Value { get; set; }
        public long TrackId { get; set; }
    }

    public class GroupXTableFullInfoFinancialImpactDTO
    {
        public long GroupId { get; set; }
        public long TableId { get; set; }
        public long CompanyId { get; set; }
        public long MinTrack { get; set; }
        public long MaxTrack { get; set; }
    }

    public class SalaryTableValuesFullInfoFinancialImpactDTO
    {
        public long TableId { get; set; }
        public double Midpoint { get; set; }
        public int GSM { get; set; }
        public IEnumerable<long> GroupIds { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
    }

    public class SalaryTableTracksFullInfoFinancialImpactDTO
    {
        public long TableId { get; set; }
        public double Multiplier { get; set; }
        public long TrackId { get; set; }
    }


    public class GetFullInfoFinancialImpactHandler
        : IRequestHandler<GetFullInfoFinancialImpactRequest, GetFullInfoFinancialImpactResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IGetLabelDisplayByInteractor _getLabelDisplayByInteractor;
        private readonly IEnumerable<FullInfoFinancialImpactEnum> _listEnum;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetFullInfoFinancialImpactHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            IGetLabelDisplayByInteractor getLabelDisplayByInteractor,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _getLabelDisplayByInteractor = getLabelDisplayByInteractor;
            _listEnum = Enum.GetValues(typeof(FullInfoFinancialImpactEnum)) as
                IEnumerable<FullInfoFinancialImpactEnum>;

            _listEnum = _listEnum.First().GetWithOrder().Select(s =>
            (FullInfoFinancialImpactEnum)Enum.Parse(typeof(FullInfoFinancialImpactEnum), s));

            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
        }
        public async Task<GetFullInfoFinancialImpactResponse> Handle(GetFullInfoFinancialImpactRequest request, CancellationToken cancellationToken)
        {
            var result = await GetResultConsolidated(request);
            var categoryLabel = $"{request.DisplayBy.GetDescription()}-{await _getLabelDisplayByInteractor.Handler(request.CategoryId, request.ProjectId, request.DisplayBy)}";

            //configuration global labels 
            var configGlobalLabels =
                await _getGlobalLabelsInteractor.Handler(request.ProjectId);

            var customGSM = configGlobalLabels
                .FirstOrDefault(f => f.Id == (long)FullInfoFinancialImpactEnum.GSM);

            var dictonaryMMMI = new Dictionary<DisplayMMMIEnum, string>();

            if (request.UnitId.HasValue)
            {
                var permissionCompanySM = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>()
                    .GetAsync(g => g.Where(c => c.EmpresaId == request.UnitId.Value)?
                    .Select(s => new { s.MI, s.MM }));

                if (permissionCompanySM != null)
                {
                    dictonaryMMMI.Add(DisplayMMMIEnum.MM, permissionCompanySM.MM);
                    dictonaryMMMI.Add(DisplayMMMIEnum.MI, permissionCompanySM.MI);
                }

            }
            if (result.Any())
            {
                //header 
                var header = new List<GetDataFullInfoFinancialImpactResponse>();
                var countHeader = 0;
                foreach (FullInfoFinancialImpactEnum item in _listEnum)
                {
                    switch (item)
                    {
                        case FullInfoFinancialImpactEnum.GSM:
                            if (customGSM.IsChecked)
                            {
                                header.Add(new GetDataFullInfoFinancialImpactResponse
                                {
                                    Type = typeof(string).Name,
                                    Value = customGSM.Alias,
                                    Sortable = true,
                                    ColPos = countHeader,
                                    ColumnId = (int)item
                                });
                                countHeader++;
                            }
                            continue;
                        case FullInfoFinancialImpactEnum.Percentages:


                            var tracksPercentage =
                                    result.FirstOrDefault().SalaryTable;

                            foreach (var trackPercentage in tracksPercentage)
                            {

                                header.Add(new GetDataFullInfoFinancialImpactResponse
                                {
                                    Type = typeof(string).Name,
                                    Value = $"{Math.Round(trackPercentage.Multiplicator * 100, 0)}%",
                                    ColPos = countHeader
                                });

                                countHeader++;
                            }
                            continue;
                        case FullInfoFinancialImpactEnum.FinancialImpact:
                            header.Add(new GetDataFullInfoFinancialImpactResponse
                            {
                                Type = typeof(string).Name,
                                Value = item.GetDescription(),
                                Sortable = true,
                                ColPos = countHeader,
                                ColumnId = (int)item
                            });
                            countHeader++;
                            continue;
                        case FullInfoFinancialImpactEnum.HoursBase:
                            header.Add(new GetDataFullInfoFinancialImpactResponse
                            {
                                Type = typeof(string).Name,
                                Value = item.GetDescription(),
                                Sortable = false,
                                ColPos = countHeader,
                                ColumnId = (int)item
                            });
                            countHeader++;
                            continue;
                        default:
                            header.Add(new GetDataFullInfoFinancialImpactResponse
                            {
                                Type = typeof(string).Name,
                                Value = item.GetDescription(),
                                Sortable = true,
                                ColPos = countHeader,
                                ColumnId = (int)item
                            });

                            break;
                    }

                    countHeader++;
                }


                //body
                var body = new List<List<GetDataFullInfoFinancialImpactResponse>>();

                foreach (var salaryBaseData in result)
                {
                    var cols = new List<GetDataFullInfoFinancialImpactResponse>();
                    int countBody = 0;

                    foreach (FullInfoFinancialImpactEnum item in _listEnum)
                    {
                        var value = salaryBaseData
                                    .GetType()
                                    .GetProperty(item.ToString());
                        switch (item)
                        {
                            case FullInfoFinancialImpactEnum.GSM:
                                if (customGSM.IsChecked)
                                {
                                    cols.Add(new GetDataFullInfoFinancialImpactResponse
                                    {
                                        ColPos = countBody,
                                        @Type = value == null ? typeof(string).Name :
                                                (value.GetValue(salaryBaseData) == null ? typeof(string).Name : value.GetValue(salaryBaseData).GetType().Name),
                                        Value = value == null ? string.Empty : (value.GetValue(salaryBaseData) == null ? string.Empty : value.GetValue(salaryBaseData).ToString())
                                    });
                                    countBody++;
                                }
                                continue;
                            case FullInfoFinancialImpactEnum.Percentages:

                                foreach (var valueSalary in salaryBaseData.SalaryTable)
                                {
                                    cols.Add(new GetDataFullInfoFinancialImpactResponse
                                    {
                                        ColPos = countBody,
                                        @Type = valueSalary.Value == 0 ? typeof(string).Name : valueSalary.Value.GetType().Name,
                                        Value = valueSalary.Value == 0 ? "-" : valueSalary.Value.ToString()
                                    });
                                    countBody++;
                                }
                                continue;
                            case FullInfoFinancialImpactEnum.FinancialImpact:

                                if (salaryBaseData == null) continue;

                                var valueMidpoint = Math.Round(salaryBaseData.FinancialImpact, 0);

                                cols.Add(new GetDataFullInfoFinancialImpactResponse
                                {
                                    ColPos = countBody,
                                    @Type = valueMidpoint.GetType().Name,
                                    Value = valueMidpoint.ToString()
                                });

                                break;
                            case FullInfoFinancialImpactEnum.Salary:
                                cols.Add(new GetDataFullInfoFinancialImpactResponse
                                {
                                    ColPos = countBody,
                                    @Type = value == null ? typeof(string).Name :
                                            (value.GetValue(salaryBaseData) == null ? typeof(string).Name : value.GetValue(salaryBaseData).GetType().Name),
                                    Value = value == null ? string.Empty : (value.GetValue(salaryBaseData) == null ? string.Empty : Math.Round((double)value.GetValue(salaryBaseData), 0).ToString(CultureInfo.InvariantCulture))
                                });
                                break;
                            default:

                                cols.Add(new GetDataFullInfoFinancialImpactResponse
                                {
                                    ColPos = countBody,
                                    @Type = value == null ? typeof(string).Name :
                                            (value.GetValue(salaryBaseData) == null ? typeof(string).Name : value.GetValue(salaryBaseData).GetType().Name),
                                    Value = value == null ? string.Empty : (value.GetValue(salaryBaseData) == null ? string.Empty : value.GetValue(salaryBaseData).ToString())
                                });
                                break;
                        }
                        countBody++;
                    }

                    body.Add(cols);
                }

                return new GetFullInfoFinancialImpactResponse
                {
                    Category = categoryLabel,
                    Scenario = !dictonaryMMMI.Any() ? request.Scenario.GetDescription() : dictonaryMMMI[request.Scenario],
                    Serie = request.SerieId.GetDescription(),
                    Table = new GetTableFullInfoFinancialImpactResponse
                    {
                        Body = body,
                        Header = header
                    }
                };
            }
            return new GetFullInfoFinancialImpactResponse
            {
                Category = categoryLabel,
                Scenario = !dictonaryMMMI.Any() ? request.Scenario.GetDescription() : dictonaryMMMI[request.Scenario],
                Serie = request.SerieId.GetDescription(),
                Table = new GetTableFullInfoFinancialImpactResponse()

            };

        }

        private async Task<List<BaseSalaryFullInfoFinancialImpactDTO>> GetResultConsolidated(GetFullInfoFinancialImpactRequest request)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);
            bool isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;
            IEnumerable<BaseSalaryFullInfoFinancialImpactDTO> result = await GetSalaryBaseXPositionSM(request, permissionUser);

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
                return new List<BaseSalaryFullInfoFinancialImpactDTO>();

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
                return new List<BaseSalaryFullInfoFinancialImpactDTO>();

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

                var finalSalary = salaryBasePositionSM.Salary;

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
                         .Average(a => a.MidPoint) / hoursBaseSM * hoursBase;

                    var listSalaryTable = new List<DataSalaryTableFullInfoFinancialImpact>();

                    foreach (var track in tracks)
                    {
                        if (track.TrackId < minTrackId || track.TrackId > maxTrackId)
                        {
                            listSalaryTable.Add(new DataSalaryTableFullInfoFinancialImpact
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

                        var midPointResult = midPoint.HasValue ? (midPoint.Value * fFM) : 0;

                        if (fFM.HasValue && (midPointResult >= minSalary))
                        {
                            listSalaryTable.Add(new DataSalaryTableFullInfoFinancialImpact
                            {
                                Value = Math.Round((double)midPointResult, 0),
                                Multiplicator = fFM.Value,
                                TrackId = track.TrackId
                            });
                        }
                        else
                        {
                            listSalaryTable.Add(new DataSalaryTableFullInfoFinancialImpact
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                        }
                    }

                    salaryBasePositionSM.SalaryTable = listSalaryTable;
                    double ifamin = GetIFAMin(listSalaryTable.Select(s => s.Value), finalSalary);
                    double ifps = GetIFPS(listSalaryTable.Where(m => m.TrackId <= (int)TrackIdDefault.MidPoint && m.Value > 0).Select(s => s.Value),
                                    finalSalary,
                                    ifamin);
                    double ifmp = GetIFMP(midPoint, finalSalary);
                    double ifamax = GetIFAMax(listSalaryTable.Select(s => s.Value), finalSalary);

                    switch (request.SerieId)
                    {
                        case AnalyseFinancialImpactEnum.IFAMin:
                            if (ifamin > 0)
                            {
                                salaryBasePositionSM.Type = AnalyseFinancialImpactEnum.IFAMin;
                                salaryBasePositionSM.FinancialImpact = ifamin;
                            }
                            break;
                        case AnalyseFinancialImpactEnum.IFPS:
                            if (ifps > 0)
                            {
                                salaryBasePositionSM.Type = AnalyseFinancialImpactEnum.IFPS;
                                salaryBasePositionSM.FinancialImpact = ifps;
                            }
                            break;
                        case AnalyseFinancialImpactEnum.IFMP:
                            if (ifmp > 0)
                            {
                                salaryBasePositionSM.Type = AnalyseFinancialImpactEnum.IFMP;
                                salaryBasePositionSM.FinancialImpact = ifmp;
                            }
                            break;
                        case AnalyseFinancialImpactEnum.IFAMax:
                            if (ifamax > 0)
                            {
                                salaryBasePositionSM.Type = AnalyseFinancialImpactEnum.IFAMax;
                                salaryBasePositionSM.FinancialImpact = ifamax;
                            }
                            break;
                    }
                }
            }

            result = result.Where(x => x.Type == request.SerieId);
            if (request.SortColumnId.HasValue && (int)FullInfoFinancialImpactEnum.FinancialImpact == request.SortColumnId.Value)
                result = isDesc ? result.OrderByDescending(x => x.FinancialImpact) : result.OrderBy(x => x.FinancialImpact);

            return result.Safe().Any() ? result.ToList() : new List<BaseSalaryFullInfoFinancialImpactDTO>();
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

            if (!values.Safe().Any()) return 0;

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

        private async Task<IEnumerable<BaseSalaryFullInfoFinancialImpactDTO>> GetSalaryBaseXPositionSM(GetFullInfoFinancialImpactRequest request, PermissionJson permissionUser)
        {
            IEnumerable<BaseSalaryFullInfoFinancialImpactDTO> salaryBaseList =
                        new List<BaseSalaryFullInfoFinancialImpactDTO>();

            var levelsExp = permissionUser.Levels;

            var areasExp = permissionUser.Areas;

            var groupsExp =
                        permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var tablesExp = permissionUser.Contents?
                            .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                            .SubItems.ToList();


            bool isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;
            var sortColumnProperty = request.SortColumnId.HasValue ?
                _listEnum.FirstOrDefault(f => (int)f == request.SortColumnId.Value).ToString() :
                FullInfoFinancialImpactEnum.Company.ToString();
            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            if (request.Scenario == DisplayMMMIEnum.MM)
            {
                var conditionMM = ConverterObject.GetCondition<VwBaseSalarialSalaryMarkMm>(request.DisplayBy.ToString(),
                            request.CategoryId);

                var resulMM = await _unitOfWork.GetRepository<VwBaseSalarialSalaryMarkMm, Guid>()
                    .GetListAsync(x => x.Where(bs =>
                          bs.CargoIdSmmm.HasValue &&
                          bs.ProjetoId == request.ProjectId &&
                          (request.UnitId.HasValue || request.CompaniesId.Contains(bs.EmpresaId.Value)) &&
                          (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.LevelId)) &&
                          (!areasExp.Safe().Any()) && // || !areasExp.Contains(bs.Area)) &&
                          (!groupsExp.Safe().Any() || !groupsExp.Contains(bs.ProfileId)) &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))?
                          .Where(conditionMM)?
                         .Select(s => new BaseSalaryFullInfoFinancialImpactDTO
                         {
                             Salary = s.SalarioFinalSm,
                             LevelId = s.LevelId,
                             CurrentyPosition = s.CargoEmpresa,
                             Employee = s.Funcionario,
                             Company = string.IsNullOrWhiteSpace(s.NomeFantasia) ?
                             (string.IsNullOrWhiteSpace(s.RazaoSocial) ? string.Empty : s.RazaoSocial) : s.NomeFantasia,
                             CompanyId = s.EmpresaId.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0),
                             PositionId = s.CargoIdSmmm.Value,
                             Parameter = s.Area,
                             GSM = s.Gsm,
                             HoursBaseSM = s.BaseHorariaSm,
                             //Parameter01 = s.Parameter01,
                             //Parameter02 = s.Parameter02,
                             //Parameter03 = s.Parameter03,
                             PositionSM = s.CargoSm,
                             ProfileId = s.ProfileId,
                             Tables = s.TabelaSalariais,
                             FinancialImpact = 0
                         })
                         .OrderBy(sortColumnProperty, isDesc));

                resulMM = resulMM.Where(bs => !tablesExp.Safe().Any() ||
                                          Array.ConvertAll(bs.Tables.Split(',', StringSplitOptions.RemoveEmptyEntries), long.Parse).Except(tablesExp).Any()).ToList();

                return resulMM;
            }

            var conditionMI = ConverterObject
                .GetCondition<VwBaseSalarialSalaryMarkMi>(request.DisplayBy.ToString(),
                            request.CategoryId);

            var resultMI = await _unitOfWork.GetRepository<VwBaseSalarialSalaryMarkMi, Guid>()
                .GetListAsync(x => x.Where(bs =>
                      bs.CargoIdSmmi.HasValue &&
                      bs.ProjetoId == request.ProjectId &&
                      (request.UnitId.HasValue || request.CompaniesId.Contains(bs.EmpresaId.Value)) &&
                      (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                      (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.LevelId)) &&
                      (!areasExp.Safe().Any()) &&// || !areasExp.Contains(bs.Area)) &&
                      (!groupsExp.Safe().Any() || !groupsExp.Contains(bs.ProfileId)) &&
                      (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))?
                      .Where(conditionMI)?
                     .Select(s => new BaseSalaryFullInfoFinancialImpactDTO
                     {
                         Salary = s.SalarioFinalSm,
                         LevelId = s.LevelId,
                         CurrentyPosition = s.CargoEmpresa,
                         Employee = s.Funcionario,
                         Company = string.IsNullOrWhiteSpace(s.NomeFantasia) ?
                         (string.IsNullOrWhiteSpace(s.RazaoSocial) ? string.Empty : s.RazaoSocial) : s.NomeFantasia,
                         CompanyId = s.EmpresaId.Value,
                         HoursBase = s.BaseHoraria.GetValueOrDefault(0),
                         PositionId = s.CargoIdSmmi.Value,
                         Parameter = s.Area,
                         GSM = s.Gsm,
                         HoursBaseSM = s.BaseHorariaSm,
                         //Parameter01 = s.Parameter01,
                         //Parameter02 = s.Parameter02,
                         //Parameter03 = s.Parameter03,
                         PositionSM = s.CargoSm,
                         ProfileId = s.ProfileId,
                         Tables = s.TabelaSalariais,
                         FinancialImpact = 0
                     })
                     .OrderBy(sortColumnProperty, isDesc));

            resultMI = resultMI.Where(bs => !tablesExp.Safe().Any() ||
                                          Array.ConvertAll(bs.Tables.Split(',', StringSplitOptions.RemoveEmptyEntries), long.Parse).Except(tablesExp).Any()).ToList();

            return resultMI;
        }
    }
}
