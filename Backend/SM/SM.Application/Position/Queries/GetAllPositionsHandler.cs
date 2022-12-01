using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Helpers;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Domain.Enum;
using SM.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Repositories.Extensions;
using SM.Application.Interactors;
using SM.Application.Position.Queries.Response;

namespace SM.Application.Position.Querie
{
    public class GetAllPositionsRequest : IRequest<GetAllPositionsResponse>
    {
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public long UserId { get; set; }
        public long ProjectId { get; set; }
        public long TableId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Term { get; set; }
        public bool ShowJustWithOccupants { get; set; } = false;
        public ContractTypeEnum ContractType { get; set; } = ContractTypeEnum.CLT;
        public DataBaseSalaryEnum HoursType { get; set; } = DataBaseSalaryEnum.MonthSalary;
        public List<int> ColumnsExcluded { get; set; } = new List<int>(); //using to share (EXP)
        public bool? IsAsc { get; set; } = null;
        public int? SortColumnId { get; set; } = null;
        public bool Share { get; set; }
    }

    public class GetAllPositionsHandler : IRequestHandler<GetAllPositionsRequest, GetAllPositionsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IMultiplierFactorHourlyInteractor _multiplierFactorHourlyInteractor;
        private readonly IMultiplierFactorTypeContratcInteractor _multiplierFactorTypeContratcInteractor;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;
        private readonly IEnumerable<PositionProjectColumnsEnum> _listEnums;
        private readonly IGetLocalLabelsInteractor _getLocalLabelsInteractor;

        public GetAllPositionsHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            IMultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor,
            IMultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor,
            IGetLocalLabelsInteractor getLocalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _multiplierFactorHourlyInteractor = multiplierFactorHourlyInteractor;
            _multiplierFactorTypeContratcInteractor = multiplierFactorTypeContratcInteractor;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
            _getLocalLabelsInteractor = getLocalLabelsInteractor;
            _listEnums = Enum.GetValues(typeof(PositionProjectColumnsEnum)) as
                IEnumerable<PositionProjectColumnsEnum>;

            _listEnums = _listEnums.First().GetWithOrder().Select(s =>
            (PositionProjectColumnsEnum)Enum.Parse(typeof(PositionProjectColumnsEnum), s));

        }
        public async Task<GetAllPositionsResponse> Handle(GetAllPositionsRequest request, CancellationToken cancellationToken)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var term = request.Term == null ? string.Empty : request.Term.ToLower().Trim();
            var areasExp = permissionUser.Areas;
            var levelsExp = permissionUser.Levels;

            var groupsExp = permissionUser.Contents?
                            .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                            .SubItems;

            var tablesExp = permissionUser.Contents?
                            .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                            .SubItems;

            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            //permissions user
            PermissionUser(permissionUser, out bool canEditColumns, out bool displayAllOccupants);

            request.ShowJustWithOccupants = request.ShowJustWithOccupants ? request.ShowJustWithOccupants : !displayAllOccupants ? !displayAllOccupants : request.ShowJustWithOccupants;

            var projectPositionsList = await QueryPositionSM(request, term, areasExp, levelsExp, groupsExp, ignoreSituationPerson);

            if (!projectPositionsList.Any())
                return new GetAllPositionsResponse();

            var salaryTableValues = await GetSalaryTableValues(request, groupsExp, tablesExp);
            request.ColumnsExcluded = request.Share ? request.ColumnsExcluded : new List<int>();

            #region Load Global Labels and Local Labels
            var globalLabels = await _getGlobalLabelsInteractor.Handler(request.ProjectId);
            var rangeGlobalIds = globalLabels.Safe().Any() ?
                                 globalLabels.Where(co => !co.IsChecked)
                                                   .Select(s => (int)s.Id).ToList() :
                                 new List<int>();
            if (rangeGlobalIds.Safe().Any() &&
                !request.ColumnsExcluded.Any(ce => !rangeGlobalIds.Contains(ce)))
                request.ColumnsExcluded.AddRange(rangeGlobalIds);

            var userDisplaySM = await _getLocalLabelsInteractor.Handler(new LocalLabelsRequest
            {
                Module = ModulesEnum.TableSalary,
                UserId = request.UserId,
            });
            #endregion

            await GetLevelsCompanies(request, projectPositionsList);

            //salary Base 
            var projectPositionsListIds = projectPositionsList.Select(pp => pp.PositionSMLocalId).Distinct();

            List<PositionBaseDTO> positionBaseDTO = await QuerySalaryBase(request, projectPositionsListIds, ignoreSituationPerson);


            var groupsIdxTableId = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                            .GetListAsync(x => x.Where(gpm => gpm.ProjetoId == request.ProjectId &&
                                gpm.TabelaSalarialIdLocal == request.TableId &&
                               gpm.Ativo.HasValue && gpm.Ativo.Value &&
                               (request.UnitId.HasValue || request.CompaniesId.Contains(gpm.EmpresaId)) &&
                               (!request.UnitId.HasValue || gpm.EmpresaId == request.UnitId.Value))?
                               .Where(gpm1 => !request.UnitId.HasValue || request.UnitId.Value == gpm1.EmpresaId)?
                              .Select(s => new
                              {
                                  GroupId = s.GrupoProjetoSmidLocal,
                                  MaxTrack = s.FaixaIdSuperior,
                                  MinTrack = s.FaixaIdInferior
                              }));

            if (!groupsIdxTableId.Any())
                return new GetAllPositionsResponse();

            var groupsIdxTableIdMaxTrack = groupsIdxTableId.Max(s => s.MaxTrack);
            var groupsIdxTableIdMinTrack = groupsIdxTableId.Min(s => s.MinTrack);

            var gsms = projectPositionsList.Select(s => s.GSM).Distinct();

            var salaryValuesTables = await _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                                        .GetListAsync(x => x.Where(tv => tv.ProjetoId == request.ProjectId &&
                                         request.TableId == tv.TabelaSalarialIdLocal)?
                                        .Where(tv1 => gsms.Contains(tv1.Grade))?
                                        .Select(s => new
                                        {
                                            GSM = s.Grade,
                                            MidPoint = s.FaixaMidPoint
                                        }));

            if (!salaryValuesTables.Any())
                return new GetAllPositionsResponse();

            var rangeTableSalary = await _unitOfWork.GetRepository<TabelasSalariaisFaixas, long>()
                 .GetListAsync(x => x.Where(tsv => tsv.ProjetoId == request.ProjectId &&
                 request.TableId == tsv.TabelaSalarialIdLocal &&
                 tsv.FaixaSalarialId >= groupsIdxTableIdMinTrack && tsv.FaixaSalarialId <= groupsIdxTableIdMaxTrack)
                 ?.Select(s => new
                 {
                     TrackId = s.FaixaSalarialId,
                     TableId = s.TabelaSalarialIdLocal,
                     FactorMulti = s.AmplitudeMidPoint,
                 }).OrderBy(o => o.FactorMulti));

            if (!rangeTableSalary.Any())
                return new GetAllPositionsResponse();

            var minMidPoint = await _unitOfWork.GetRepository<TabelasSalariaisGrades, long>()
               .GetAsync(g => g.Where(t => t.ProjetoId == request.ProjectId && t.TabelaSalarialIdLocal == request.TableId)
               .Select(s => s.MenorSalario));

            var multiplierFactorHourlyInteractor = await _multiplierFactorHourlyInteractor.Handler(request.ProjectId, request.HoursType);
            var multiplierFactorTypeContratcInteractor = await _multiplierFactorTypeContratcInteractor.Handler(request.ProjectId, request.ContractType);

            foreach (var positonSM in projectPositionsList)
            {
                var salaryTableValueByPosition = new List<SalaryTableValueByPosition>();

                var midPoint = salaryValuesTables
                    .FirstOrDefault(f => f.GSM == positonSM.GSM)?
                    .MidPoint;

                var maxTrack = groupsIdxTableId
                    .FirstOrDefault(f => f.GroupId == positonSM.GroupId)?
                    .MaxTrack;

                var minTrack = groupsIdxTableId
                    .FirstOrDefault(f => f.GroupId == positonSM.GroupId)?
                    .MinTrack;

                if (request.HoursType == DataBaseSalaryEnum.HourSalary)
                {
                    multiplierFactorHourlyInteractor = 1 / positonSM.HourBase;
                }

                var salaryTableValuesItem = salaryTableValues.FirstOrDefault(stv => stv.GSM == positonSM.GSM);

                foreach (var item in rangeTableSalary)
                {
                    double? rangeValue = SetValueRangeItem(salaryTableValuesItem, item.TrackId);
                    double valueSalary = rangeValue.HasValue ? rangeValue.Value * multiplierFactorHourlyInteractor * multiplierFactorTypeContratcInteractor.GetValueOrDefault(0) : 0;

                    salaryTableValueByPosition.Add(new SalaryTableValueByPosition
                    {
                        Multiplicator = item.FactorMulti,
                        Value = item.TrackId >= minTrack && item.TrackId <= maxTrack &&
                                valueSalary >= minMidPoint ? valueSalary : 0
                    });

                }
                positonSM.SalaryTableValues = salaryTableValueByPosition;
            }

            GetAllPositionsResponse getAllPositionsResponse = new GetAllPositionsResponse
            {
                Table = new TableInfoPosition
                {
                    Header = new List<HeaderInfoPosition>(),
                    Body = new List<List<DataBodyPosition>>()
                }
            };

            //header
            var headerDinamic = projectPositionsList.FirstOrDefault().SalaryTableValues;

            getAllPositionsResponse.Table.Header = await
                GetHeader(request, userDisplaySM, canEditColumns, headerDinamic, _listEnums);

            var typeOfContractExpIds = permissionUser.TypeOfContract;
            //body 
            GetBody(request,
                projectPositionsList,
                displayAllOccupants,
                positionBaseDTO,
                getAllPositionsResponse,
                _listEnums,
                typeOfContractExpIds);

            getAllPositionsResponse.NextPage = getAllPositionsResponse.Table.Body.Count > 0 ? request.Page + 1 : 0;
            return getAllPositionsResponse;
        }

        private async Task GetLevelsCompanies(GetAllPositionsRequest request, IEnumerable<ProjectPositions> projectPositionsList)
        {
            //levels company
            var levelsIdFromPositionSM = projectPositionsList.Select(s => s.LevelId);
            var levelsCompanies = await _unitOfWork.GetRepository<NivelEmpresas, long>()
                .GetListAsync(x => x.Where(ne => request.CompaniesId.Contains(ne.EmpresaId) &&
                 levelsIdFromPositionSM.Contains(ne.NivelId))
                .Where(bs1 => !request.UnitId.HasValue || bs1.EmpresaId == request.UnitId.Value)?
                .Select(s => new LevelCompanyAllPositionsDTO
                {
                    LevelId = s.NivelId,
                    Level = s.NivelEmpresa
                }));

            if (levelsCompanies.Any())
            {
                levelsCompanies = levelsCompanies.GroupBy(g => g.LevelId, (key, value) =>
                new LevelCompanyAllPositionsDTO
                {
                    LevelId = key,
                    Level = value.FirstOrDefault().Level
                }).ToList();
            }

            foreach (var projectPosition in projectPositionsList)
            {
                var level = levelsCompanies
                    .FirstOrDefault(f => f.LevelId == projectPosition.LevelId);

                projectPosition.Level = level == null ? string.Empty : level.Level;
            }
        }

        private void GetBody(GetAllPositionsRequest request,
            IEnumerable<ProjectPositions> projectPositionsList,
            bool isDisplayOccupants,
            List<PositionBaseDTO> positionBaseDTO,
            GetAllPositionsResponse getAllPositionsResponse,
            IEnumerable<PositionProjectColumnsEnum> listEnums,
            IEnumerable<long> expTypeContract)
        {
            var columnsCustom = new List<int>{(int)PositionProjectColumnsEnum.Area, (int)PositionProjectColumnsEnum.CareerAxis,
            (int)PositionProjectColumnsEnum.Parameter01, (int)PositionProjectColumnsEnum.Parameter02, (int)PositionProjectColumnsEnum.Parameter03};
            foreach (var projectPosition in projectPositionsList)
            {
                List<DataBodyPosition> listBodyLine = new List<DataBodyPosition>();

                var baseDTO = positionBaseDTO.Where(f => f.CargoSmId == projectPosition.PositionSMLocalId &&
                                                         f.CompanyId == projectPosition.CompanyId);

                var ocupantType = isDisplayOccupants ?
                    (baseDTO.Any() ? baseDTO.FirstOrDefault().OccupantType : string.Empty) :
                    string.Empty;

                var ocupantTypeId = isDisplayOccupants ?
                    (baseDTO.Any() ? baseDTO.FirstOrDefault().OccupantTypeId : null) :
                    null;

                var isOccupantPJ = baseDTO.Any(a => PJOrCLT.IsPJ(a.OccupantType).GetValueOrDefault(false));
                var isOccupantCLT = baseDTO.Any(a => PJOrCLT.IsCLT(a.OccupantType).GetValueOrDefault(false));

                var tooltip = baseDTO.Any() ?
                    baseDTO.GroupBy(g => g.PositionBase, (key, value) => new Tooltip { Amount = value.Count(), Position = key }).ToList() :
                    new List<Tooltip>();

                if (ocupantTypeId.HasValue &&
                    expTypeContract.Safe().Any() &&
                    expTypeContract.Contains(ocupantTypeId.Value))
                {
                    ocupantType = null;
                    ocupantTypeId = null;
                    tooltip = new List<Tooltip>();
                }

                //fixed columns
                var countBody = 0;
                foreach (PositionProjectColumnsEnum item in listEnums)
                {
                    if (!request.ColumnsExcluded.Any(a => a == (int)item))
                    {
                        object value = null;
                        switch (item)
                        {
                            case PositionProjectColumnsEnum.Area:
                                value = projectPosition != null ?
                                   string.Join(',', projectPosition.Area) : string.Empty;
                                break;
                            case PositionProjectColumnsEnum.CareerAxis:
                                value = projectPosition != null ?
                                    string.Join(',', projectPosition.CareerAxis) : string.Empty;
                                break;
                            case PositionProjectColumnsEnum.Parameter01:
                                value = projectPosition != null ?
                                    string.Join(',', projectPosition.Parameter01) : string.Empty;
                                break;
                            case PositionProjectColumnsEnum.Parameter02:
                                value = projectPosition != null ?
                                    string.Join(',', projectPosition.Parameter02) : string.Empty;
                                break;
                            case PositionProjectColumnsEnum.Parameter03:
                                value = projectPosition != null ?
                                    string.Join(',', projectPosition.Parameter03) : string.Empty;
                                break;
                            default:
                                value = projectPosition != null ?
                                projectPosition
                                .GetType()
                                .GetProperty(item.ToString())
                                .GetValue(projectPosition) : null;
                                break;
                        }


                        listBodyLine.Add(new DataBodyPosition
                        {
                            ColPos = countBody,
                            PositionSmId = projectPosition.PositionSMLocalId,
                            OccupantCLT = isOccupantCLT,
                            OccupantPJ = isOccupantPJ,
                            Tooltips = tooltip,
                            @Type = columnsCustom.Contains((int)item) ? "custom" : value == null ? typeof(string).Name : value.GetType().Name,
                            Value = value == null ? string.Empty : value.ToString(),
                            CmCode = projectPosition.CmCode
                        });

                        countBody++;
                    }
                }

                //dinamic columns

                if (projectPosition.SalaryTableValues.Any())
                {
                    foreach (var item in projectPosition.SalaryTableValues)
                    {

                        listBodyLine.Add(new DataBodyPosition
                        {
                            ColPos = countBody,
                            PositionSmId = projectPosition.PositionSMLocalId,
                            @Type = item.Value.GetType().Name,
                            Value = item.Value == 0 ? "-" :
                            (request.HoursType == DataBaseSalaryEnum.HourSalary ?
                            Math.Round(item.Value, 2).ToString("N2", CultureInfo.InvariantCulture) :
                            Math.Round(item.Value, 0).ToString(CultureInfo.InvariantCulture)),
                            CmCode = projectPosition.CmCode
                        });
                        countBody++;
                    }
                }

                getAllPositionsResponse.Table.Body.Add(listBodyLine);
            }
        }

        private async Task<List<HeaderInfoPosition>> GetHeader(GetAllPositionsRequest request,
            IEnumerable<LocalLabelsResponse> userDisplaySM,
            bool canEditColumns,
            IEnumerable<SalaryTableValueByPosition> headerDinamic,
            IEnumerable<PositionProjectColumnsEnum> listEnums)
        {
            var companyDefaultHeader = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>().GetAsync(x => x.Where(ep => request.CompaniesId.Contains(ep.EmpresaId)));
            var listHeader = new List<HeaderInfoPosition>();
            //fixed columns
            var countHeader = 0;
            foreach (PositionProjectColumnsEnum item in listEnums)
            {
                if (!request.ColumnsExcluded.Any(a => a == (int)item))
                {
                    var columnData = userDisplaySM.FirstOrDefault(f => f.InternalCode == (long)item);

                    switch (item)
                    {
                        case PositionProjectColumnsEnum.PositionSalaryMark:
                            string name = columnData == null && companyDefaultHeader != null ?
                               companyDefaultHeader.CargoSalaryMark : columnData == null ? item.GetDescription() :
                               columnData.Label;
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = true,
                                ColName = name,
                                NickName = name,
                                ColumnId = (int)item,
                                Sortable = true,
                                Editable = true,
                                IsChecked = true
                            });
                            break;
                        case PositionProjectColumnsEnum.Company:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                           columnData.Label,
                                ColumnId = (int)item,
                                Sortable = true,
                                Editable = false,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.Table:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                           columnData.Label,
                                ColumnId = (int)item,
                                Sortable = true,
                                Editable = false,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.Profile:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                           columnData.Label,
                                ColumnId = (int)item,
                                Sortable = true,
                                Editable = false,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.Level:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                           columnData.Label,
                                ColumnId = (int)item,
                                Sortable = true,
                                Editable = false,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.Area:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                            columnData.Label,
                                ColumnId = (int)item,
                                Sortable = true,
                                Editable = true,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.CareerAxis:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                            columnData.Label,
                                ColumnId = (int)item,
                                Sortable = true,
                                Editable = false,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.Parameter01:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                            columnData.Label,
                                ColumnId = (int)item,
                                Sortable = true,
                                Editable = true,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.Parameter02:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                           columnData.Label,
                                ColumnId = (int)item,
                                Sortable = true,
                                Editable = true,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.Parameter03:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                            columnData.Label,
                                ColumnId = (int)item,
                                Sortable = true,
                                Editable = true,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.GSM:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = true,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                    columnData.Label,
                                ColumnId = (int)item,
                                Sortable = true,
                                Editable = true,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.HourBase:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                            columnData.Label,
                                ColumnId = (int)item,
                                Sortable = false,
                                Editable = false,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.Smcode:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                            columnData.Label,
                                ColumnId = (int)item,
                                Sortable = false,
                                Editable = false,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                        case PositionProjectColumnsEnum.TechnicalAdjustment:
                            listHeader.Add(new HeaderInfoPosition
                            {
                                ColPos = countHeader,
                                Disabled = false,
                                ColName = item.GetDescription(),
                                NickName = columnData == null ? item.GetDescription() :
                                            columnData.Label,
                                ColumnId = (int)item,
                                Sortable = false,
                                Editable = false,
                                IsChecked = columnData == null ? true : columnData.IsChecked
                            });
                            break;
                    }
                    countHeader++;
                }

            }

            //dinamic columns
            foreach (var hd in headerDinamic)
            {
                listHeader.Add(new HeaderInfoPosition
                {
                    ColName = $"{Math.Round(hd.Multiplicator * 100, 0)}%",
                    NickName = $"{Math.Round(hd.Multiplicator * 100, 0)}%",
                    Disabled = true,
                    Editable = false,
                    IsChecked = false,
                    Visible = false,
                    ColPos = countHeader,
                });
                countHeader++;
            }

            return listHeader;
        }

        private async Task<List<PositionBaseDTO>> QuerySalaryBase(GetAllPositionsRequest request, IEnumerable<long> projectPositionsListIds,
          bool ignoreSituationPerson)
        {

            return await _unitOfWork.GetRepository<BaseSalariais, long>()
                .Include("RegimeContratacao")
                .Include("Empresa.GruposProjetosSalaryMarkMapeamento")
            .GetListAsync(g => g.Where(bs => bs.CargoIdSMMM.HasValue &&
                projectPositionsListIds.Contains(bs.CargoIdSMMM.Value) &&
                bs.Empresa != null &&
                bs.Empresa.GruposProjetosSalaryMarkMapeamento != null &&
                bs.Empresa.GruposProjetosSalaryMarkMapeamento.Any(x => x.TabelaSalarialIdLocal == request.TableId) &&
                (request.UnitId.HasValue || request.CompaniesId.Contains(bs.EmpresaId.Value)) &&
                (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))?
            .Where(bs1 => !request.UnitId.HasValue || bs1.EmpresaId == request.UnitId.Value)
            .Select(se => new PositionBaseDTO
            {
                CargoSmId = se.CargoIdSMMM.Value,
                OccupantType = se.RegimeContratacao != null ? se.RegimeContratacao.Regime : string.Empty,
                OccupantTypeId = se.RegimeContratacaoId,
                PositionBase = se.CargoEmpresa,
                CompanyId = se.EmpresaId.GetValueOrDefault()
            }));
        }

        private void PermissionUser(PermissionJson permissionUser, out bool canEditColumns, out bool displayAllOccupants)
        {
            canEditColumns = CanEditColumns(permissionUser);
            displayAllOccupants = permissionUser.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.Positions);
        }

        private async Task<IEnumerable<ProjectPositions>> QueryPositionSM(GetAllPositionsRequest request, string term, IEnumerable<long> areasExp,
            IEnumerable<long> levelsExp, IEnumerable<long> groupsExp, bool ignoreSituationPerson)
        {
            var smIds = new List<long>();


            var isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;
            var sortColumnProperty = request.SortColumnId.HasValue ?
                _listEnums.FirstOrDefault(f => (int)f == request.SortColumnId.Value).ToString() :
                PositionProjectColumnsEnum.PositionSalaryMark.ToString();

            if (request.ShowJustWithOccupants)
            {
                smIds = await _unitOfWork.GetRepository<BaseSalariais, long>()
                           .Include("Empresa.GruposProjetosSalaryMarkMapeamento")
                           .GetListAsync(g => g.Where(bs => bs.CargoIdSMMM.HasValue &&
                                         bs.RegimeContratacaoId.HasValue &&
                                         bs.Empresa != null && bs.Empresa.GruposProjetosSalaryMarkMapeamento != null &&
                                         bs.Empresa.GruposProjetosSalaryMarkMapeamento.Any(x => x.TabelaSalarialIdLocal == request.TableId) &&
                                         (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active) &&
                                         request.CompaniesId.Contains(bs.EmpresaId.Value))?
                           .Select(s => s.CargoIdSMMM.Value)
                           .Distinct());
            }

            var positionMapping = await _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>()
                                    .Include("Empresa")
                                    .Include("TabelasSalariais")
                                    .Include("CargosProjetosSm.GruposProjetosSalaryMark")
                                    .Include("CargosProjetosSm.CmcodeNavigation.Nivel")
                                    .Include("AjusteTecnicoMotivo")
                                    .Include("CargosProjetosSm.CargosProjetoSMParametrosMapeamento.ParametrosProjetosSMLista")
                                    .GetListAsync(x => x.Where(cpm =>
                                                               cpm.ProjetoId == request.ProjectId &&
                                                               cpm.TabelaSalarialIdLocal == request.TableId &&
                                                               cpm.CargosProjetosSm != null &&
                                                               cpm.CargosProjetosSm.Ativo.HasValue &&
                                                               cpm.CargosProjetosSm.Ativo.Value &&
                                                               cpm.CargosProjetosSm.ProjetoId == request.ProjectId &&
                                                               (!request.UnitId.HasValue ||
                                                                 cpm.EmpresaId == request.UnitId.Value) &&
                                                               (!request.ShowJustWithOccupants ||
                                                                    (smIds.Any() && smIds.Contains(cpm.CargoProjetoSmidLocal)))
                                                               //    (
                                                               //        string.IsNullOrWhiteSpace(term) ||
                                                               //        (!request.ColumnsExcluded.Any(a => a == (int)PositionProjectColumnsEnum.PositionSalaryMark) &&
                                                               //          !string.IsNullOrWhiteSpace(cpm.CargosProjetosSm.CargoSm) &&
                                                               //          cpm.CargosProjetosSm.CargoSm.ToLower().Trim().Contains(term)) ||

                                                               //        (!request.ColumnsExcluded.Any(a => a == (int)PositionProjectColumnsEnum.Profile) &&
                                                               //         cpm.CargosProjetosSm.GruposProjetosSalaryMark != null &&
                                                               //         string.IsNullOrWhiteSpace(cpm.CargosProjetosSm.GruposProjetosSalaryMark.GrupoSm) &&
                                                               //         cpm.CargosProjetosSm.GruposProjetosSalaryMark.GrupoSm.ToLower().Trim().Contains(term)) ||

                                                               //         (!request.ColumnsExcluded.Any(a => a == (int)PositionProjectColumnsEnum.Level) &&
                                                               //             cpm.CargosProjetosSm.CmcodeNavigation != null && cpm.CargosProjetosSm.CmcodeNavigation.Nivel != null &&
                                                               //             !string.IsNullOrWhiteSpace(cpm.CargosProjetosSm.CmcodeNavigation.Nivel.Rotulo) &&
                                                               //             cpm.CargosProjetosSm.CmcodeNavigation.Nivel.Rotulo.ToLower().Trim().Contains(term)) ||

                                                               //         (!request.ColumnsExcluded.Any(a => a == (int)PositionProjectColumnsEnum.HourBase) &&
                                                               //           cpm.CargosProjetosSm.BaseHoraria.ToString().Contains(term)) ||

                                                               //         (!request.ColumnsExcluded.Any(a => a == (int)PositionProjectColumnsEnum.TechnicalAdjustment) &&
                                                               //           cpm.AjusteTecnicoMotivo != null &&
                                                               //           !string.IsNullOrWhiteSpace(cpm.AjusteTecnicoMotivo.AjusteTecnicoMotivo1) &&
                                                               //          cpm.AjusteTecnicoMotivo.AjusteTecnicoMotivo1.ToLower().Trim().Contains(term))) ||

                                                               //         (!request.ColumnsExcluded.Any(a => a == (int)PositionProjectColumnsEnum.Smcode) &&
                                                               //          !string.IsNullOrWhiteSpace(cpm.CargosProjetosSm.Smcode) && cpm.CargosProjetosSm.Smcode.ToLower().Trim().Contains(term)) ||

                                                               //         (!request.ColumnsExcluded.Any(a => a == (int)PositionProjectColumnsEnum.GSM) &&
                                                               //          cpm.Gsm.HasValue && cpm.Gsm.Value.ToString().Contains(term)) ||

                                                               //         (!request.ColumnsExcluded.Any(a => a == (int)PositionProjectColumnsEnum.Parameter01) &&
                                                               //           cpm.CargosProjetosSm.CargosProjetoSMParametrosMapeamento.Any(cpmp =>
                                                               //               cpmp.ParametrosProjetosSMLista != null &&
                                                               //               cpmp.ParametrosProjetosSMLista.ParametroSMTipoId == (long)ParametersProjectsTypes.ParameterOne &&
                                                               //              !string.IsNullOrWhiteSpace(cpmp.ParametrosProjetosSMLista.ParametroProjetoSMLista) &&
                                                               //              cpmp.ParametrosProjetosSMLista.ParametroProjetoSMLista.ToLower().Trim().Contains(term))) ||

                                                               //         (!request.ColumnsExcluded.Any(a => a == (int)PositionProjectColumnsEnum.Parameter02) &&
                                                               //           cpm.CargosProjetosSm.CargosProjetoSMParametrosMapeamento.Any(cpmp =>
                                                               //               cpmp.ParametrosProjetosSMLista != null &&
                                                               //               cpmp.ParametrosProjetosSMLista.ParametroSMTipoId == (long)ParametersProjectsTypes.ParameterTwo &&
                                                               //              !string.IsNullOrWhiteSpace(cpmp.ParametrosProjetosSMLista.ParametroProjetoSMLista) &&
                                                               //              cpmp.ParametrosProjetosSMLista.ParametroProjetoSMLista.ToLower().Trim().Contains(term))) ||

                                                               //         (!request.ColumnsExcluded.Any(a => a == (int)PositionProjectColumnsEnum.Parameter03) &&
                                                               //           cpm.CargosProjetosSm.CargosProjetoSMParametrosMapeamento.Any(cpmp =>
                                                               //               cpmp.ParametrosProjetosSMLista != null &&
                                                               //               cpmp.ParametrosProjetosSMLista.ParametroSMTipoId == (long)ParametersProjectsTypes.ParameterThree &&
                                                               //              !string.IsNullOrWhiteSpace(cpmp.ParametrosProjetosSMLista.ParametroProjetoSMLista) &&
                                                               //              cpmp.ParametrosProjetosSMLista.ParametroProjetoSMLista.ToLower().Trim().Contains(term))) ||

                                                               //         (!request.ColumnsExcluded.Any(a => a == (int)PositionProjectColumnsEnum.CarrierAxis) &&
                                                               //           cpm.CargosProjetosSm.CargosProjetoSMParametrosMapeamento.Any(cpmp =>
                                                               //               cpmp.ParametrosProjetosSMLista != null &&
                                                               //               cpmp.ParametrosProjetosSMLista.ParametroSMTipoId == (long)ParametersProjectsTypes.CarreiraEixo &&
                                                               //              !string.IsNullOrWhiteSpace(cpmp.ParametrosProjetosSMLista.ParametroProjetoSMLista) &&
                                                               //              cpmp.ParametrosProjetosSMLista.ParametroProjetoSMLista.ToLower().Trim().Contains(term)))
                                                               )
                                                        .Select(res => new ProjectPositions
                                                        {
                                                            PositionSMLocalId = res.CargoProjetoSmidLocal,
                                                            PositionSalaryMark = res.CargosProjetosSm != null ? res.CargosProjetosSm.CargoSm : string.Empty,
                                                            Profile = res.CargosProjetosSm != null && res.CargosProjetosSm.GruposProjetosSalaryMark != null ?
                                                                      res.CargosProjetosSm.GruposProjetosSalaryMark.GrupoSm : string.Empty,
                                                            LevelId = res.CargosProjetosSm != null && res.CargosProjetosSm.CmcodeNavigation != null ?
                                                                     res.CargosProjetosSm.CmcodeNavigation.NivelId : 0,
                                                            Level = res.CargosProjetosSm != null && res.CargosProjetosSm.CmcodeNavigation != null ?
                                                                    res.CargosProjetosSm.CmcodeNavigation.Nivel.Rotulo : string.Empty,
                                                            CmCode = res.CargosProjetosSm != null ? res.CargosProjetosSm.Cmcode : 0,
                                                            HourBase = res.CargosProjetosSm != null ? res.CargosProjetosSm.BaseHoraria : 0,
                                                            GSM = res.Gsm.HasValue ? res.Gsm.Value : 0,
                                                            Smcode = res.CargosProjetosSm != null ? res.CargosProjetosSm.Smcode : string.Empty,
                                                            GroupId = res.CargosProjetosSm != null ? res.CargosProjetosSm.GrupoSmidLocal : 0,
                                                            TechnicalAdjustment = res.AjusteTecnicoMotivo != null ?
                                                                                 res.AjusteTecnicoMotivo.AjusteTecnicoMotivo1 : string.Empty,
                                                            Company = res.Empresa != null ? res.Empresa.NomeFantasia : string.Empty,
                                                            CompanyId = res.EmpresaId,
                                                            Table = res.TabelasSalariais != null ? res.TabelasSalariais.TabelaSalarial : string.Empty,
                                                            Area = res.CargosProjetosSm.CargosProjetoSMParametrosMapeamento.Where(cpmp =>
                                                                          cpmp.ProjetoId == request.ProjectId &&
                                                                          cpmp.ParametrosProjetosSMLista != null &&
                                                                          cpmp.ParametrosProjetosSMLista.ParametroSMTipoId == (long)ParametersProjectsTypes.Area)
                                                                         .Select(cpres => cpres.ParametrosProjetosSMLista.ParametroProjetoSMLista),
                                                            CareerAxis = res.CargosProjetosSm.CargosProjetoSMParametrosMapeamento.Where(cpmp =>
                                                                          cpmp.ProjetoId == request.ProjectId &&
                                                                          cpmp.ParametrosProjetosSMLista != null &&
                                                                          cpmp.ParametrosProjetosSMLista.ParametroSMTipoId == (long)ParametersProjectsTypes.CarreiraEixo)
                                                                         .Select(cpres => cpres.ParametrosProjetosSMLista.ParametroProjetoSMLista),
                                                            Parameter01 = res.CargosProjetosSm.CargosProjetoSMParametrosMapeamento.Where(cpmp =>
                                                                          cpmp.ProjetoId == request.ProjectId &&
                                                                          cpmp.ParametrosProjetosSMLista != null &&
                                                                          cpmp.ParametrosProjetosSMLista.ParametroSMTipoId == (long)ParametersProjectsTypes.ParameterOne)
                                                                         .Select(cpres => cpres.ParametrosProjetosSMLista.ParametroProjetoSMLista),
                                                            Parameter02 = res.CargosProjetosSm.CargosProjetoSMParametrosMapeamento.Where(cpmp =>
                                                                          cpmp.ProjetoId == request.ProjectId &&
                                                                          cpmp.ParametrosProjetosSMLista != null &&
                                                                          cpmp.ParametrosProjetosSMLista.ParametroSMTipoId == (long)ParametersProjectsTypes.ParameterTwo)
                                                                         .Select(cpres => cpres.ParametrosProjetosSMLista.ParametroProjetoSMLista),
                                                            Parameter03 = res.CargosProjetosSm.CargosProjetoSMParametrosMapeamento.Where(cpmp =>
                                                                          cpmp.ProjetoId == request.ProjectId &&
                                                                          cpmp.ParametrosProjetosSMLista != null &&
                                                                          cpmp.ParametrosProjetosSMLista.ParametroSMTipoId == (long)ParametersProjectsTypes.ParameterThree)
                                                                         .Select(cpres => cpres.ParametrosProjetosSMLista.ParametroProjetoSMLista),
                                                        }).OrderBy(sortColumnProperty, isDesc)
                                                       .Skip((request.Page - 1) * request.PageSize)
                                                       .Take(request.PageSize));
            return positionMapping;
        }

        private bool CanEditColumns(PermissionJson userJson)
        {
            var resultPermission = userJson.Permission?.FirstOrDefault(p => p.Id == (long)PermissionItensEnum.RenameColumn);
            return resultPermission == null;
        }

        private async Task<IEnumerable<SalaryTableValuesPosition>> GetSalaryTableValues(GetAllPositionsRequest request,
        IEnumerable<long> groupsExp,
        IEnumerable<long> tablesExp)
        {
            var salaryTableValues = await _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                                   .Include("TabelasSalariais.GruposProjetosSalaryMarkMapeamento")
                                   .GetListAsync(x => x.Where(s => s.ProjetoId == request.ProjectId &&
                                                                s.TabelasSalariais.GruposProjetosSalaryMarkMapeamento
                                                                    .Any(a => a.TabelaSalarialIdLocal == request.TableId &&
                                                                              a.ProjetoId == request.ProjectId &&
                                                                              !groupsExp.Safe().Contains(a.GrupoProjetoSmidLocal)) &&

                                                              (!request.UnitId.HasValue || (s.Projeto != null &&
                                                                s.Projeto.ProjetosSalaryMarkEmpresas
                                                                        .Any(psm => psm.EmpresaId == request.UnitId.Value &&
                                                                                    psm.ProjetoId == request.ProjectId))) &&

                                                              (!tablesExp.Safe().Any() || !tablesExp.Contains(s.TabelaSalarialIdLocal)) &&
                                                                s.TabelaSalarialIdLocal == request.TableId &&
                                                                s.TabelasSalariais != null &&
                                                               (s.TabelasSalariais.Ativo.HasValue && s.TabelasSalariais.Ativo.Value))
                                                       .Select(s => new SalaryTableValuesPosition
                                                       {
                                                           GSM = s.Grade,
                                                           RangeMinus6 = s.FaixaMenos6,
                                                           RangeMinus5 = s.FaixaMenos5,
                                                           RangeMinus4 = s.FaixaMenos4,
                                                           RangeMinus3 = s.FaixaMenos3,
                                                           RangeMinus2 = s.FaixaMenos2,
                                                           RangeMinus1 = s.FaixaMenos1,
                                                           RangeMidPoint = s.FaixaMidPoint,
                                                           RangePlus1 = s.FaixaMais1,
                                                           RangePlus2 = s.FaixaMais2,
                                                           RangePlus3 = s.FaixaMais3,
                                                           RangePlus4 = s.FaixaMais4,
                                                           RangePlus5 = s.FaixaMais5,
                                                           RangePlus6 = s.FaixaMais6,
                                                           LocalIdSalaryTable = s.TabelaSalarialIdLocal
                                                       })
                                                 );
            return salaryTableValues;
        }

        private double? SetValueRangeItem(SalaryTableValuesPosition salaryTableValuesItem, long salaryRangeId)
        {
            switch ((SalarialRanges)salaryRangeId)
            {
                case SalarialRanges.RangeMinus6:
                    return salaryTableValuesItem.RangeMinus6 ?? null;
                case SalarialRanges.RangeMinus5:
                    return salaryTableValuesItem.RangeMinus5 ?? null;
                case SalarialRanges.RangeMinus4:
                    return salaryTableValuesItem.RangeMinus4 ?? null;
                case SalarialRanges.RangeMinus3:
                    return salaryTableValuesItem.RangeMinus3 ?? null;
                case SalarialRanges.RangeMinus2:
                    return salaryTableValuesItem.RangeMinus2 ?? null;
                case SalarialRanges.RangeMinus1:
                    return salaryTableValuesItem.RangeMinus1 ?? null;
                case SalarialRanges.MidPoint:
                    return salaryTableValuesItem.RangeMidPoint ?? null;
                case SalarialRanges.RangePlus1:
                    return salaryTableValuesItem.RangePlus1 ?? null;
                case SalarialRanges.RangePlus2:
                    return salaryTableValuesItem.RangePlus2 ?? null;
                case SalarialRanges.RangePlus3:
                    return salaryTableValuesItem.RangePlus3 ?? null;
                case SalarialRanges.RangePlus4:
                    return salaryTableValuesItem.RangePlus4 ?? null;
                case SalarialRanges.RangePlus5:
                    return salaryTableValuesItem.RangePlus5 ?? null;
                case SalarialRanges.RangePlus6:
                    return salaryTableValuesItem.RangePlus6 ?? null;
                default:
                    return 0;
            }
        }
    }
}


