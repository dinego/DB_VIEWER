using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Helpers;
using SM.Application.Interactors.Interfaces;
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

    public class GetFullInfoProposedMovementsRequest :
        IRequest<GetFullInfoProposedMovementsResponse>
    {
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
        public object CategoryId { get; set; }
        public ProposedMovementsEnum SerieId { get; set; } = ProposedMovementsEnum.AdequacyOfNomenclature;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsAsc { get; set; } = null;
        public int? SortColumnId { get; set; } = null;
    }
    public class GetFullInfoProposedMovementsResponse
    {
        public int NextPage { get; set; }
        public string Category { get; set; }
        public string Scenario { get; set; }
        public ProposedMovementsProposedMovements Table { get; set; }
    }

    public class ProposedMovementsProposedMovements
    {

        public IEnumerable<GetDataProposedMovementsResponse> Header { get; set; }
        public IEnumerable<IEnumerable<GetDataProposedMovementsResponse>> Body { get; set; }

    }

    public class GetDataProposedMovementsResponse
    {
        public int ColPos { get; set; }
        public string Value { get; set; }
        public string @Type { get; set; }
        public bool Sortable { get; set; } = false;
        public int ColumnId { get; set; }
    }

    public class BaseSalaryFullInfoProposedMovementsDTO
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
        public double HoursBaseSM { get; set; }
        public double HoursBase { get; set; }
        public long PositionId { get; set; }
        public long PositionIdDefault { get; set; }
        public string Tables { get; set; }
        public IEnumerable<DataSalaryTableFullInfoProposedMovements> SalaryTable { get; set; }
        public ProposedMovementsEnum ProposedMovements { get; set; }
        public string ProposedMovementLabel { get; set; }
    }

    public class DataSalaryTableFullInfoProposedMovementsImpact
    {
        public double Multiplicator { get; set; }
        public double Value { get; set; }
        public long TrackId { get; set; }

    }

    public class DataSalaryTableFullInfoProposedMovements
    {
        public double Multiplicator { get; set; }
        public double Value { get; set; }
        public long TrackId { get; set; }

    }

    public class GetFullInfoProposedMovementsHandler
         : IRequestHandler<GetFullInfoProposedMovementsRequest, GetFullInfoProposedMovementsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IGetLabelDisplayByInteractor _getLabelDisplayByInteractor;
        private readonly IEnumerable<FullInfoProposedMovementEnum> _listEnum;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetFullInfoProposedMovementsHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            IGetLabelDisplayByInteractor getLabelDisplayByInteractor,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _getLabelDisplayByInteractor = getLabelDisplayByInteractor;
            _listEnum = Enum.GetValues(typeof(FullInfoProposedMovementEnum)) as
                IEnumerable<FullInfoProposedMovementEnum>;

            _listEnum = _listEnum.First().GetWithOrder().Select(s =>
            (FullInfoProposedMovementEnum)Enum.Parse(typeof(FullInfoProposedMovementEnum), s));

            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
        }
        public async Task<GetFullInfoProposedMovementsResponse> Handle(GetFullInfoProposedMovementsRequest request, CancellationToken cancellationToken)
        {
            var result = await GetResultConsolidated(request);
            var categoryLabel = $"{request.DisplayBy.GetDescription()}-{await _getLabelDisplayByInteractor.Handler(request.CategoryId, request.ProjectId, request.DisplayBy)}";
            //configuration global labels 
            var configGlobalLabels =
                await _getGlobalLabelsInteractor.Handler(request.ProjectId);

            var customGSM = configGlobalLabels
                .FirstOrDefault(f => f.Id == (long)FullInfoProposedMovementEnum.GSM);

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
                var header = new List<GetDataProposedMovementsResponse>();
                var countHeader = 0;
                foreach (FullInfoProposedMovementEnum item in _listEnum)
                {
                    switch (item)
                    {
                        case FullInfoProposedMovementEnum.GSM:
                            if (customGSM.IsChecked)
                            {
                                header.Add(new GetDataProposedMovementsResponse
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

                        case FullInfoProposedMovementEnum.Percentages:

                            var tracksPercentage =
                                    result.FirstOrDefault().SalaryTable;

                            foreach (var trackPercentage in tracksPercentage)
                            {

                                header.Add(new GetDataProposedMovementsResponse
                                {
                                    Type = typeof(string).Name,
                                    Value = $"{Math.Round(trackPercentage.Multiplicator * 100, 0)}%",
                                    ColPos = countHeader
                                });

                                countHeader++;
                            }
                            continue;
                        case FullInfoProposedMovementEnum.ProposedMovementLabel:
                            header.Add(new GetDataProposedMovementsResponse
                            {
                                Type = typeof(string).Name,
                                Value = item.GetDescription(),
                                ColPos = countHeader,
                                ColumnId = (int)item
                            });
                            countHeader++;
                            continue;
                        case FullInfoProposedMovementEnum.HoursBase:
                            header.Add(new GetDataProposedMovementsResponse
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
                            header.Add(new GetDataProposedMovementsResponse
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
                var body = new List<List<GetDataProposedMovementsResponse>>();

                foreach (var salaryBaseData in result)
                {
                    var cols = new List<GetDataProposedMovementsResponse>();
                    int countBody = 0;

                    foreach (FullInfoProposedMovementEnum item in _listEnum)
                    {
                        var value = salaryBaseData
                                    .GetType()
                                    .GetProperty(item.ToString());

                        switch (item)
                        {
                            case FullInfoProposedMovementEnum.GSM:
                                if (customGSM.IsChecked)
                                {
                                    cols.Add(new GetDataProposedMovementsResponse
                                    {
                                        ColPos = countBody,
                                        @Type = value == null ? typeof(string).Name :
                                                (value.GetValue(salaryBaseData) == null ? typeof(string).Name : value.GetValue(salaryBaseData).GetType().Name),
                                        Value = value == null ? string.Empty : (value.GetValue(salaryBaseData) == null ? string.Empty : value.GetValue(salaryBaseData).ToString())
                                    });
                                    countBody++;
                                }
                                continue;

                            case FullInfoProposedMovementEnum.Percentages:

                                foreach (var valueSalary in salaryBaseData.SalaryTable)
                                {
                                    cols.Add(new GetDataProposedMovementsResponse
                                    {
                                        ColPos = countBody,
                                        @Type = valueSalary.Value == 0 ? typeof(string).Name : valueSalary.Value.GetType().Name,
                                        Value = valueSalary.Value == 0 ? "-" : valueSalary.Value.ToString()
                                    });
                                    countBody++;
                                }
                                continue;
                            case FullInfoProposedMovementEnum.ProposedMovementLabel:

                                if (salaryBaseData == null) continue;

                                cols.Add(new GetDataProposedMovementsResponse
                                {
                                    ColPos = countBody,
                                    @Type = salaryBaseData.ProposedMovementLabel.GetType().Name,
                                    Value = salaryBaseData.ProposedMovementLabel
                                });

                                continue;
                            case FullInfoProposedMovementEnum.Salary:
                                cols.Add(new GetDataProposedMovementsResponse
                                {
                                    ColPos = countBody,
                                    @Type = value == null ? typeof(string).Name :
                                            (value.GetValue(salaryBaseData) == null ? typeof(string).Name : value.GetValue(salaryBaseData).GetType().Name),
                                    Value = value == null ? string.Empty : (value.GetValue(salaryBaseData) == null ? string.Empty : Math.Round((double)value.GetValue(salaryBaseData), 0).ToString(CultureInfo.InvariantCulture))
                                });
                                break;
                            default:

                                cols.Add(new GetDataProposedMovementsResponse
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



                return new GetFullInfoProposedMovementsResponse
                {
                    Category = categoryLabel,
                    NextPage = body.Count > 0 ? request.Page + 1 : 0,
                    Scenario = !dictonaryMMMI.Any() ? request.Scenario.GetDescription() : dictonaryMMMI[request.Scenario],
                    Table = new ProposedMovementsProposedMovements
                    {
                        Body = body,
                        Header = header
                    }
                };
            }
            return new GetFullInfoProposedMovementsResponse
            {
                Category = categoryLabel,
                Scenario = !dictonaryMMMI.Any() ? request.Scenario.GetDescription() : dictonaryMMMI[request.Scenario],
                Table = new ProposedMovementsProposedMovements()
            };

        }

        private async Task<List<BaseSalaryFullInfoProposedMovementsDTO>>
    GetResultConsolidated(GetFullInfoProposedMovementsRequest request)
        {

            IEnumerable<BaseSalaryFullInfoProposedMovementsDTO> result =
                await GetSalaryBaseXPositionSM(request);

            var groupIds = result
                .Select(s => s.ProfileId).Distinct().ToList();

            var groupsData = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                                .GetListAsync(x => x.Where(g => g.ProjetoId == request.ProjectId &&
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
                return new List<BaseSalaryFullInfoProposedMovementsDTO>();

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
                return new List<BaseSalaryFullInfoProposedMovementsDTO>();

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

                var minSalary = minSalaries.Where(w => tableIds.Contains(w.TableId)).Average(a => a.SalaryMin);

                var hoursBase = salaryBasePositionSM.HoursBase;
                var hoursBaseSM = salaryBasePositionSM.HoursBaseSM;
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
                         .Average(a => a.MidPoint);

                    var listSalaryTable = new List<DataSalaryTableFullInfoProposedMovements>();

                    foreach (var track in tracks)
                    {

                        if (track.TrackId < minTrackId || track.TrackId > maxTrackId)
                        {
                            listSalaryTable.Add(new DataSalaryTableFullInfoProposedMovements
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                            continue;
                        }

                        var fFM = salaryTable
                        .SelectMany(a => a.Tracks)
                        .Where(s => s.TrackId == track.TrackId)?
                        .Average(a => a.FactorMulti);

                        var midPointResult = midPoint.HasValue ? (midPoint.Value * fFM / hoursBaseSM * hoursBase) : 0;

                        if (fFM.HasValue && (midPointResult >= minSalary))
                        {
                            listSalaryTable.Add(new DataSalaryTableFullInfoProposedMovements
                            {
                                Value = Math.Round((double)midPointResult, 0),
                                Multiplicator = fFM.Value,
                                TrackId = track.TrackId
                            });
                        }
                        else
                        {
                            listSalaryTable.Add(new DataSalaryTableFullInfoProposedMovements
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                        }
                    }

                    salaryBasePositionSM.SalaryTable = listSalaryTable;
                }
            }

            return result.ToList();
        }


        private async Task<IEnumerable<BaseSalaryFullInfoProposedMovementsDTO>>
            GetSalaryBaseXPositionSM(GetFullInfoProposedMovementsRequest request)
        {
            IEnumerable<BaseSalaryFullInfoProposedMovementsDTO> salaryBaseList =
                        new List<BaseSalaryFullInfoProposedMovementsDTO>();

            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var levelsExp = permissionUser.Levels;

            var areasExp = permissionUser.Areas;

            var groupsExp =
                        permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var tablesExp =
                        permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                        .SubItems;

            var isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;
            var sortColumnProperty = request.SortColumnId.HasValue ?
                _listEnum.FirstOrDefault(f => (int)f == request.SortColumnId.Value).ToString() :
                FullInfoProposedMovementEnum.Company.ToString();

            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);
            var result = new List<BaseSalaryFullInfoProposedMovementsDTO>();

            if (request.Scenario == DisplayMMMIEnum.MM)
            {
                var conditionMM = ConverterObject.GetCondition<VwBaseSalarialSalaryMarkMm>(request.DisplayBy.ToString(),
                            request.CategoryId);

                result = await _unitOfWork.GetRepository<VwBaseSalarialSalaryMarkMm, Guid>()
                    .GetListAsync(x => x.Where(bs =>
                          bs.CargoIdSmmm.HasValue &&
                          bs.ProjetoId == request.ProjectId &&
                          request.CompaniesId.Contains(bs.EmpresaId.Value) &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active) &&
                          (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.LevelId)) &&
                          (!areasExp.Safe().Any()) && // || !areasExp.Contains(bs.Area)) &&
                          (!groupsExp.Safe().Any() || !groupsExp.Contains(bs.ProfileId)))?
                          .Where(conditionMM)?
                         .Select(s => new BaseSalaryFullInfoProposedMovementsDTO
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
                             PositionIdDefault = s.CargoIdSM.GetValueOrDefault(0),
                             Parameter = s.Area,
                             GSM = s.Gsm,
                             HoursBaseSM = s.BaseHorariaSm,
                             //Parameter01 = s.Parameter01,
                             //Parameter02 = s.Parameter02,
                             //Parameter03 = s.Parameter03,
                             PositionSM = s.CargoSm,
                             ProfileId = s.ProfileId,
                             Tables = s.TabelaSalariais,
                             ProposedMovementLabel = "-"
                         }).OrderBy(sortColumnProperty, isDesc));

            }
            else
            {
                var conditionMI = ConverterObject
                                .GetCondition<VwBaseSalarialSalaryMarkMi>(request.DisplayBy.ToString(),
                                            request.CategoryId);

                result = await _unitOfWork.GetRepository<VwBaseSalarialSalaryMarkMi, Guid>()
                    .GetListAsync(x => x.Where(bs =>
                          bs.CargoIdSmmi.HasValue &&
                          bs.ProjetoId == request.ProjectId &&
                          request.CompaniesId.Contains(bs.EmpresaId.Value) &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active) &&
                          (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.LevelId)) &&
                          (!areasExp.Safe().Any()) && // || !areasExp.Contains(bs.Area)) &&
                          (!groupsExp.Safe().Any() || !groupsExp.Contains(bs.ProfileId)))?
                          .Where(conditionMI)?
                         .Select(s => new BaseSalaryFullInfoProposedMovementsDTO
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
                             PositionIdDefault = s.CargoIdSM.GetValueOrDefault(0),
                             Parameter = s.Area,
                             GSM = s.Gsm,
                             HoursBaseSM = s.BaseHorariaSm,
                             //Parameter01 = s.Parameter01,
                             //Parameter02 = s.Parameter02,
                             //Parameter03 = s.Parameter03,
                             PositionSM = s.CargoSm,
                             ProfileId = s.ProfileId,
                             Tables = s.TabelaSalariais,
                             ProposedMovementLabel = "-"
                         })
                         .OrderBy(sortColumnProperty, isDesc));
            }

            //complete info 

            foreach (var item in result)
            {

                item.ProposedMovements = GetProposedMovementsAnalyse(item.PositionId,
                    item.PositionIdDefault, item.PositionSM, item.CurrentyPosition);

                item.ProposedMovementLabel = item.ProposedMovements.GetDescription();
            }

            result = result.Where(bs => bs.ProposedMovements == request.SerieId &&
                                        (!tablesExp.Safe().Any() || Array.ConvertAll(bs.Tables.Split(',', StringSplitOptions.RemoveEmptyEntries), long.Parse)
                                                                    .Except(tablesExp).Any())).ToList();

            return result;
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
