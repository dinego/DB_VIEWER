using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Helpers;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Application.TableSalary.Queries.Response;
using SM.Application.TableSalary.Validators;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Repositories.Extensions;
using SM.Application.Interactors;
using AgileObjects.AgileMapper.Extensions;

namespace SM.Application.GetSalaryPositionTable
{
    public class GetSalaryTablePositionRequest : IRequest<GetSalaryTablePositionResponse>
    {
        public long ProjectId { get; set; }
        public long TableId { get; set; }
        public long? GroupId { get; set; }
        public long? UnitId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public ContractTypeEnum ContractType { get; set; } = ContractTypeEnum.CLT;
        public DataBaseSalaryEnum HoursType { get; set; } = DataBaseSalaryEnum.MonthSalary;
        public List<int> ColumnsExcluded { get; set; } = new List<int>(); // exp columns
        public int Page { get; set; }
        public int PageSize { get; set; } = 10;
        public long UserId { get; set; }
        public bool? IsAsc { get; set; } = null;
        public string FilterSearch { get; set; }
        public bool CanEditGlobalLabels { get; set; }
        public int? SortColumnId { get; set; } = null;
        public bool IgnorePagination { get; set; }
    }

    public class GetSalaryTablePositionHandler : IRequestHandler<GetSalaryTablePositionRequest, GetSalaryTablePositionResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMultiplierFactorHourlyInteractor _multiplierFactorHourlyInteractor;
        private readonly IMultiplierFactorTypeContratcInteractor _multiplierFactorTypeContratcInteractor;
        private readonly IValidator<GetTableSalaryPermissionValidatorsException> _validator;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly ValidatorResponse _validatorResponse;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;
        private readonly IEnumerable<TableSalaryColumnPositionEnum> _tableSalaryColumns;
        private readonly IGetLocalLabelsInteractor _getLocalLabelsInteractor;

        public GetSalaryTablePositionHandler(IUnitOfWork unitOfWork,
            IMultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor,
            IMultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor,
            IValidator<GetTableSalaryPermissionValidatorsException> validator,
            IPermissionUserInteractor permissionUserInteractor,
            ValidatorResponse validatorResponse,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor,
            IGetLocalLabelsInteractor getLocalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _multiplierFactorHourlyInteractor = multiplierFactorHourlyInteractor;
            _multiplierFactorTypeContratcInteractor = multiplierFactorTypeContratcInteractor;
            _validator = validator;
            _permissionUserInteractor = permissionUserInteractor;
            _validatorResponse = validatorResponse;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
            _tableSalaryColumns = Enum.GetValues(typeof(TableSalaryColumnPositionEnum)) as
               IEnumerable<TableSalaryColumnPositionEnum>;
            _getLocalLabelsInteractor = getLocalLabelsInteractor;
        }
        public async Task<GetSalaryTablePositionResponse> Handle(GetSalaryTablePositionRequest request, CancellationToken cancellationToken)
        {
            var getSalaryTableResponse = new GetSalaryTablePositionResponse
            {
                Table = new TablePositionInfo
                {
                    Header = new List<HeaderPositionInfo>(),
                    Body = new List<List<DataPositionBody>>()
                }
            };

            var resName = _validator.Validate(new GetTableSalaryPermissionValidatorsException
            {
                ContractType = request.ContractType,
                GroupId = request.GroupId.GetValueOrDefault(0),
                HourType = request.HoursType,
                TableId = request.TableId,
                UserId = request.UserId
            });

            if (!resName.IsValid)
                _validatorResponse.AddNotifications(resName.Errors.ToList());
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
            #region CALCULATION FACTOR
            var factorHour = await _multiplierFactorHourlyInteractor.Handler(request.ProjectId, request.HoursType);
            var multiplierFactor = await _multiplierFactorTypeContratcInteractor.Handler(request.ProjectId, request.ContractType);
            #endregion
            #region PERMISSIONS
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);
            var groupsExp = permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var tablesExp = permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                        .SubItems;

            var areasExcept = permissionUser.Areas.Safe().Any() ? permissionUser.Areas.ToList() : new List<long>();
            var levelsExcept = permissionUser.Levels.Safe().Any() ? permissionUser.Levels.ToList() : new List<long>();
            #endregion

            #region ORDER BY
            var isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;
            var sortColumnProperty = request.SortColumnId.HasValue ?
                _tableSalaryColumns.FirstOrDefault(f => (int)f == request.SortColumnId.Value).ToString() :
                TableSalaryColumnPositionEnum.GSM.ToString();
            #endregion

            var groupProjectMapping = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                                      .Include("GruposProjetosSalaryMark")
                                      .Include("Empresa")
                                      .GetListAsync(x => x.Where(gpm => gpm.ProjetoId == request.ProjectId &&
                                                                 gpm.TabelaSalarialIdLocal == request.TableId &&
                                                                 (!request.UnitId.HasValue || gpm.EmpresaId == request.UnitId.Value) &&
                                                                 !groupsExp.Safe().Contains(gpm.GrupoProjetoSmidLocal) &&
                                                                 (!request.GroupId.HasValue || gpm.GrupoProjetoSmidLocal == request.GroupId.Value))
                                                                .Select(res => new SalaryMarkMappingProjectsPositionGroups
                                                                {
                                                                    GroupId = res.GrupoProjetoSmidLocal,
                                                                    Group = res.GruposProjetosSalaryMark != null ? res.GruposProjetosSalaryMark.GrupoSm : string.Empty,
                                                                    ProjectPositionSMId = res.GruposProjetosSalaryMark != null ? res.GruposProjetosSalaryMark.GruposProjetosSmidLocal : (long?)null,
                                                                    CompanyId = res.EmpresaId,
                                                                    Company = res.Empresa != null ? res.Empresa.NomeFantasia : string.Empty,
                                                                    RangeDown = res.FaixaIdInferior,
                                                                    RangeUp = res.FaixaIdSuperior,
                                                                    TableId = res.TabelaSalarialIdLocal
                                                                }));

            var groupsMapping = groupProjectMapping.Select(res => res.GroupId);
            var mapPositionSM = await _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>()
                        .Include("CargosProjetosSm")
                        .GetListAsync(x => x.Where(cpm => cpm.ProjetoId == request.ProjectId &&
                        cpm.TabelaSalarialIdLocal == request.TableId &&
                        (!groupsExp.Safe().Contains(cpm.CargosProjetosSm.GrupoSmidLocal)) &&
                        (!request.GroupId.HasValue || cpm.CargosProjetosSm.GrupoSmidLocal == request.GroupId) &&
                        (string.IsNullOrEmpty(request.FilterSearch) ||
                         (cpm.CargosProjetosSm != null && cpm.CargosProjetosSm.CargoSm.ToLower().Contains(request.FilterSearch.ToLower())))
                        )
                        .Select(res => new
                        {
                            GSM = res.Gsm.GetValueOrDefault(),
                            GroupId = res.CargosProjetosSm != null ? res.CargosProjetosSm.GrupoSmidLocal : (long?)null,
                            CompanyId = res.EmpresaId,
                            TableId = res.TabelaSalarialIdLocal.GetValueOrDefault()
                        }));

            var salaryTableRanges = await _unitOfWork.GetRepository<TabelasSalariaisFaixas, long>()
            .GetListAsync(x => x.Where(str => str.TabelaSalarialIdLocal == request.TableId &&
                                        str.ProjetoId == request.ProjectId)
                                        .Select(res => new SalaryTablesPositionRanges
                                        {
                                            Id = res.Id,
                                            SalaryRangeId = res.FaixaSalarialId,
                                            AmplitudeMidPoint = res.AmplitudeMidPoint,
                                            LocalIdSalaryTable = res.TabelaSalarialIdLocal
                                        }).OrderBy(o => o.SalaryRangeId));

            var salaryTableValues = _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                                    .Include("TabelasSalariais.GruposProjetosSalaryMarkMapeamento.GruposProjetosSalaryMark")
                                    .Include("TabelasSalariais.CargosProjetosSmMapeamento.CargosProjetosSm")
                                    .GetQueryList(x => x.Where(s => s.ProjetoId == request.ProjectId &&
                                                                s.TabelasSalariais.GruposProjetosSalaryMarkMapeamento
                                                                    .Any(a => a.TabelaSalarialIdLocal == request.TableId &&
                                                                              a.ProjetoId == request.ProjectId &&
                                                                              groupsMapping.Safe().Contains(a.GrupoProjetoSmidLocal)) &&

                                                                (string.IsNullOrEmpty(request.FilterSearch) ||
                                                                 (s.TabelasSalariais.CargosProjetosSmMapeamento.Any(cp => cp.CargosProjetosSm != null &&
                                                                                                        cp.CargosProjetosSm.CargoSm.ToLower().Contains(request.FilterSearch.ToLower())) ||
                                                                  s.TabelasSalariais.GruposProjetosSalaryMarkMapeamento.Any(gps => gps.GruposProjetosSalaryMark != null &&
                                                                                                        gps.GruposProjetosSalaryMark.GrupoSm.ToLower().Contains(request.FilterSearch.ToLower())) ||
                                                                  s.Grade.ToString().Contains(request.FilterSearch)) &&

                                                              (!request.UnitId.HasValue || (s.Projeto != null &&
                                                                s.Projeto.ProjetosSalaryMarkEmpresas
                                                                        .Any(psm => psm.EmpresaId == request.UnitId.Value &&
                                                                                    psm.ProjetoId == request.ProjectId))) &&

                                                              (!tablesExp.Safe().Any() || !tablesExp.Contains(s.TabelaSalarialIdLocal)) &&
                                                                s.TabelaSalarialIdLocal == request.TableId &&
                                                                s.TabelasSalariais != null &&
                                                               s.TabelasSalariais.Ativo.HasValue && s.TabelasSalariais.Ativo.Value))
                                           .Select(s => new SalaryTablesPositionValues
                                           {
                                               Id = s.Id,
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
                                               LocalIdSalaryTable = s.TabelaSalarialIdLocal,
                                               TableSalaryName = s.TabelasSalariais != null &&
                                                                  s.TabelasSalariais.TabelaSalarial != null ?
                                                                  s.TabelasSalariais.TabelaSalarial : String.Empty
                                           }));

            if (!salaryTableValues.Safe().Any())
                return getSalaryTableResponse;

            var salaryTableAll = new List<SalaryTablesPositionValues>();
            groupProjectMapping.ForEach(gp =>
            {
                var gsmByGroup = mapPositionSM.Where(mp => mp.GroupId == gp.GroupId &&
                                                     mp.CompanyId == gp.CompanyId &&
                                                     mp.TableId == gp.TableId)
                                              .Select(res => res.GSM);
                var salaryValues = salaryTableValues.Where(x => x.LocalIdSalaryTable == gp.TableId &&
                                                                gsmByGroup.Contains(x.GSM));
                var lstValues = new List<SalaryTablesPositionValues>(salaryValues);
                lstValues.AsParallel().ForAll(sl =>
                {
                    var values = sl.Map().ToANew<SalaryTablesPositionValues>();
                    lock (salaryTableAll)
                    {
                        values.Company = gp.Company;
                        values.CompanyId = gp.CompanyId;
                        values.GroupId = gp.GroupId;
                        if (!salaryTableAll.Any(st => st.GSM == sl.GSM &&
                                                st.GroupId == gp.GroupId))
                            salaryTableAll.Add(values);
                    }
                });
            });

            salaryTableAll = request.IgnorePagination ?
                            salaryTableAll.AsQueryable().OrderBy(sortColumnProperty, isDesc).ToList() :
                            salaryTableAll.AsQueryable().OrderBy(sortColumnProperty, isDesc)
                                        .Skip((request.Page - 1) * request.PageSize)
                                        .Take(request.PageSize).ToList();

            var listGsms = salaryTableAll.Select(x => x.GSM).Distinct().ToList();

            List<ProjectPositionSM> positionsByGsm = await GetPositionProject(request.ProjectId,
            listGsms, permissionUser,
            request.GroupId,
            request.TableId,
            request.FilterSearch,
            levelsExcept,
            groupsExp
            );

            bool displayAllOccupants = permissionUser.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.Positions);

            var typeOfContractIdsExp = permissionUser.TypeOfContract;

            var positionsByGsmSmIdList = positionsByGsm.Select(x => x.ProjectPositionSMId).Distinct().ToList();
            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            var companyByTableId = groupProjectMapping.Select(res => res.CompanyId).Distinct();

            var subItemsContractType = await _unitOfWork.GetRepository<BaseSalariais, long>()
                .Include("RegimeContratacao")
                .Include("CmcodeNavigation.RotuloCargos")
                .Include("CmcodeNavigation")
                .GetListAsync(x => x
                .Where(bs1 => companyByTableId.Contains(bs1.EmpresaId.Value) &&
                       bs1.CargoIdSMMM.HasValue &&
                       (!levelsExcept.Safe().Any() || !levelsExcept.Contains(bs1.CmcodeNavigation.NivelId)) &&
                       (ignoreSituationPerson || bs1.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active) &&
                       positionsByGsmSmIdList.Contains(bs1.CargoIdSMMM.Value))
                .Select(res => new
                {
                    PositionSMMM = res.CargoIdSMMM,
                    TypeOfContractId = res.RegimeContratacaoId,
                    TypeOfContract = res.RegimeContratacao != null ? res.RegimeContratacao.Regime : string.Empty,
                    PositionName = res.CargoEmpresa,
                    CompanyId = res.EmpresaId
                }).Distinct());

            var lowestSalary = await _unitOfWork.GetRepository<TabelasSalariaisGrades, long>()
                .GetAsync(g => g.Where(w => w.TabelaSalarialIdLocal == request.TableId && w.ProjetoId == request.ProjectId)?
                .Select(s => s.MenorSalario));

            var resultLowestSalary = lowestSalary.GetValueOrDefault(0) * factorHour * multiplierFactor.GetValueOrDefault(0);

            long? rangeDown = null;
            long? rangeUp = null;

            if (groupProjectMapping.Count() != 0 && request.GroupId.HasValue)
            {
                rangeDown = groupProjectMapping
                    .Where(s => s.GroupId == request.GroupId.Value &&
                            (!request.UnitId.HasValue || s.CompanyId == request.UnitId.Value))
                    .Min(m => m.RangeDown);

                rangeUp = groupProjectMapping
                    .Where(s => s.GroupId == request.GroupId.Value &&
                            (!request.UnitId.HasValue || s.CompanyId == request.UnitId.Value))
                    .Max(m => m.RangeUp);
            }

            var lstSalaryTableRanges = salaryTableRanges
                .Where(x => x.LocalIdSalaryTable == request.TableId &&
                            (!request.GroupId.HasValue || x.SalaryRangeId >= rangeDown.Value && x.SalaryRangeId <= rangeUp.Value))
                .OrderBy(x => x.AmplitudeMidPoint).ToList();

            getSalaryTableResponse.Table.Header = AddHeadersPositions(request, userDisplaySM, globalLabels, lstSalaryTableRanges);

            foreach (var salaryTableValuesItem in salaryTableAll)
            {
                var listBody = new List<DataPositionBody>();
                var grade = salaryTableValuesItem.GSM;
                var salaryTable = salaryTableValuesItem.TableSalaryName;
                var smPositionByGSM = positionsByGsm.Where(x => x.GSM == salaryTableValuesItem.GSM &&
                                                                x.GroupId == salaryTableValuesItem.GroupId &&
                                                                (!request.UnitId.HasValue || x.CompanyId == request.UnitId.Value));

                //subitems da tabela (row)
                if (!smPositionByGSM.Safe().Any())
                {
                    getSalaryTableResponse.Table.Body.Add(addLineValuesEmptyPosition(grade,
                    salaryTableValuesItem,
                    resultLowestSalary,
                    request,
                    lstSalaryTableRanges,
                    groupProjectMapping,
                    factorHour,
                    multiplierFactor));
                }

                int rowPosition = 0;
                foreach (var position in smPositionByGSM)
                {
                    var bodyPositionSubitemsbyGsm = new List<DataPositionBody>();
                    var isOccupantCLT = false;
                    var isOccupantPJ = false;
                    var auxColPos = 0;

                    var positionItem = subItemsContractType
                        .Where(x => x.CompanyId == position.CompanyId &&
                                    x.PositionSMMM.HasValue &&
                                    position.ProjectPositionSMId == x.PositionSMMM.Value &&
                                    x.TypeOfContractId.HasValue &&
                                    !typeOfContractIdsExp.Contains(x.TypeOfContractId.Value));

                    var rowSpan = rowPosition == 0 ? (int?)smPositionByGSM.Count() : null;

                    isOccupantCLT = positionItem.Safe().Any() ?
                            positionItem.ToList().Any(item => PJOrCLT.IsCLT(item?.TypeOfContract).GetValueOrDefault(false)) : false;

                    isOccupantPJ = positionItem.Safe().Any() ?
                            positionItem.ToList().Any(item => PJOrCLT.IsPJ(item?.TypeOfContract).GetValueOrDefault(false)) : false;

                    var positionCompanyNames = positionItem.Select(res => res.PositionName);
                    bodyPositionSubitemsbyGsm.Add(new DataPositionBody
                    {
                        GSM = grade,
                        RowSpan = rowSpan,
                        ColPos = auxColPos,
                        Row = rowPosition,
                        Value = grade.ToString(),
                        @Type = grade.GetType().Name,
                        OccupantCLT = isOccupantCLT,
                        OccupantPJ = isOccupantPJ,
                        PositionId = position.ProjectPositionSMId,
                        OccupantPositions = positionCompanyNames
                    }); ;
                    auxColPos++;

                    bodyPositionSubitemsbyGsm.Add(new DataPositionBody
                    {
                        GSM = grade,
                        ColPos = auxColPos,
                        Row = rowPosition,
                        Value = position.Position,
                        @Type = position.Position.GetType().Name,
                        OccupantCLT = isOccupantCLT,
                        OccupantPJ = isOccupantPJ,
                        PositionId = position.ProjectPositionSMId,
                        OccupantPositions = positionCompanyNames
                    });
                    auxColPos++;

                    bodyPositionSubitemsbyGsm.Add(new DataPositionBody
                    {
                        GSM = grade,
                        ColPos = auxColPos,
                        Row = rowPosition,
                        Value = groupProjectMapping.Any(w => w.GroupId == position.GroupId) ?
                                groupProjectMapping.FirstOrDefault(w => w.GroupId == position.GroupId)
                                                    .Group :
                                request.GroupId.HasValue &&
                                groupProjectMapping.Any(w => w.GroupId == request.GroupId) ?
                                groupProjectMapping.FirstOrDefault(w => w.GroupId == request.GroupId)
                                                    .Group :
                                                    "-",
                        @Type = "-".GetType().Name,
                        OccupantCLT = isOccupantCLT,
                        OccupantPJ = isOccupantPJ,
                        PositionId = position.ProjectPositionSMId,
                        OccupantPositions = positionCompanyNames
                    });
                    auxColPos++;

                    bodyPositionSubitemsbyGsm.Add(new DataPositionBody
                    {
                        GSM = grade,
                        ColPos = auxColPos,
                        Row = rowPosition,
                        Value = position.Company,
                        @Type = "-".GetType().Name,
                        OccupantCLT = isOccupantCLT,
                        OccupantPJ = isOccupantPJ,
                        PositionId = position.ProjectPositionSMId,
                        OccupantPositions = positionCompanyNames
                    });
                    auxColPos++;

                    foreach (var item in lstSalaryTableRanges)
                    {
                        var bodyPosition = new DataPositionBody();
                        bool setGroupRange = request.GroupId.HasValue ? groupProjectMapping.Any(gp => gp.GroupId == request.GroupId.Value &&
                                                                             gp.CompanyId == position.CompanyId &&
                                                                             gp.RangeDown <= item.SalaryRangeId &&
                                                                             gp.RangeUp >= item.SalaryRangeId) : true;

                        double? rangeValue = setGroupRange ? SetValueRangeItem(salaryTableValuesItem, item.SalaryRangeId) : null;

                        var valueSalary = rangeValue.HasValue ? rangeValue.Value * factorHour * multiplierFactor.GetValueOrDefault(0) : 0;

                        bodyPosition = new DataPositionBody
                        {
                            GSM = grade,
                            isMidPoint = item.SalaryRangeId == (int)SalarialRanges.MidPoint,
                            ColPos = auxColPos,
                            Row = rowPosition,
                            Value = valueSalary == 0 || valueSalary < resultLowestSalary ? "-" : (request.HoursType == DataBaseSalaryEnum.HourSalary ?
                                    Math.Round(valueSalary, 2).ToString("N2", CultureInfo.InvariantCulture) :
                                    Math.Round(valueSalary, 0).ToString(CultureInfo.InvariantCulture)),
                            @Type = valueSalary == 0 || valueSalary < resultLowestSalary ? "-".GetType().Name : factorHour.GetType().Name,
                            TableRangeId = item.SalaryRangeId,
                            PositionId = position.ProjectPositionSMId
                        };
                        auxColPos++;
                        bodyPositionSubitemsbyGsm.Add(bodyPosition);
                    }

                    if (rowPosition < salaryTableAll.Count())
                        rowPosition++;

                    getSalaryTableResponse.Table.Body.Add(bodyPositionSubitemsbyGsm);
                }
            }
            getSalaryTableResponse.NextPage = getSalaryTableResponse.Table.Body.Count > 0 ? request.Page + 1 : 0;
            return getSalaryTableResponse;
        }


        private List<DataPositionBody> addLineValuesEmptyPosition(double grade,
            SalaryTablesPositionValues salaryTableValuesItem,
            double resultLowestSalary,
            GetSalaryTablePositionRequest request,
            List<SalaryTablesPositionRanges> salaryTableRanges,
            List<SalaryMarkMappingProjectsPositionGroups> groupProjectMapping,
            double factorHour,
            double? multiplierFactor)
        {
            var bodyPositionSubitemsbyGsm = new List<DataPositionBody>();
            var auxColPos = 0;
            var rowSpan = 1;

            bodyPositionSubitemsbyGsm.Add(new DataPositionBody
            {
                GSM = grade,
                RowSpan = rowSpan,
                ColPos = auxColPos,
                Row = 0,
                Value = grade.ToString(),
                @Type = grade.GetType().Name,
            });
            auxColPos++;

            bodyPositionSubitemsbyGsm.Add(new DataPositionBody
            {
                GSM = grade,
                ColPos = auxColPos,
                Row = 0,
                Value = "-",
                @Type = "-".GetType().Name,
            });
            auxColPos++;

            bodyPositionSubitemsbyGsm.Add(new DataPositionBody
            {
                GSM = grade,
                ColPos = auxColPos,
                Row = 0,
                Value = groupProjectMapping.Any(w => w.GroupId == salaryTableValuesItem.GroupId) ?
                                groupProjectMapping.FirstOrDefault(w => w.GroupId == salaryTableValuesItem.GroupId)
                                                    .Group :
                                                    "-",
                @Type = "-".GetType().Name,
            });
            auxColPos++;

            bodyPositionSubitemsbyGsm.Add(new DataPositionBody
            {
                GSM = grade,
                ColPos = auxColPos,
                Row = 0,
                Value = groupProjectMapping.Any(w => w.GroupId == salaryTableValuesItem.GroupId) ?
                                groupProjectMapping.FirstOrDefault(w => w.GroupId == salaryTableValuesItem.GroupId)
                                                    .Company :
                                request.GroupId.HasValue &&
                                groupProjectMapping.Any(w => w.GroupId == request.GroupId) ?
                                groupProjectMapping.FirstOrDefault(w => w.GroupId == request.GroupId)
                                                    .Company :
                                                    "-",
                @Type = "-".GetType().Name
            });

            foreach (var item in salaryTableRanges)
            {
                var bodyPosition = new DataPositionBody();

                item.Value = SetValueRangeItem(salaryTableValuesItem, item.SalaryRangeId);

                var valueSalary = item.Value.HasValue ? item.AmplitudeMidPoint * item.Value.Value * factorHour * multiplierFactor.GetValueOrDefault(0) : 0;

                if (valueSalary >= resultLowestSalary)
                {
                    var valueForType = Math.Round(valueSalary, 0).ToString(CultureInfo.InvariantCulture);
                    bodyPosition = new DataPositionBody
                    {
                        GSM = grade,
                        isMidPoint = item.SalaryRangeId == (int)SalarialRanges.MidPoint,
                        ColPos = auxColPos,
                        Row = 0,
                        Value = valueSalary == 0 ? "-" : (request.HoursType == DataBaseSalaryEnum.HourSalary ?
                            Math.Round(valueSalary, 2).ToString("N2", CultureInfo.InvariantCulture) :
                            valueForType),
                        @Type = valueSalary == 0 ? "-".GetType().Name : 0.GetType().Name,
                        TableRangeId = item.SalaryRangeId,
                    };
                }
                else
                {
                    bodyPosition = new DataPositionBody
                    {
                        GSM = grade,
                        ColPos = auxColPos,
                        Row = 0,
                        Value = "-",
                        @Type = typeof(string).Name,
                        TableRangeId = item.SalaryRangeId,
                    };
                }
                auxColPos++;
                bodyPositionSubitemsbyGsm.Add(bodyPosition);
            }

            return bodyPositionSubitemsbyGsm;
        }

        private IEnumerable<HeaderPositionInfo> AddHeadersPositions(GetSalaryTablePositionRequest request,
            IEnumerable<LocalLabelsResponse> userDisplaySM,
            IEnumerable<GlobalLabelsJson> globalLabels,
            List<SalaryTablesPositionRanges> tabelasSalariaisFaixasList)
        {
            var listHeaderPosition = new List<HeaderPositionInfo>();
            if (!request.ColumnsExcluded.Any(a => a == (int)TableSalaryColumnPositionEnum.GSM))
            {
                var gsmLocal = userDisplaySM.FirstOrDefault(x => x.InternalCode == (int)TableSalaryColumnEnum.GSM);
                var gsmDefault = globalLabels.FirstOrDefault(f => f.Id == (long)TableSalaryColumnEnum.GSM);
                string gsm = gsmDefault == null && gsmLocal != null && !string.IsNullOrEmpty(gsmLocal.Label) ? gsmLocal.Label :
                    gsmDefault != null && string.IsNullOrEmpty(gsmDefault.Alias) ? gsmDefault.Name :
                    gsmDefault != null && !string.IsNullOrEmpty(gsmDefault.Alias) ? gsmDefault.Alias :
                    TableSalaryColumnEnum.GSM.GetDescription();
                listHeaderPosition.Add(new HeaderPositionInfo
                {
                    ColPos = 0,
                    ColName = TableSalaryColumnPositionEnum.GSM.GetDescription(),
                    NickName = gsm,
                    Editable = request.CanEditGlobalLabels,
                    Sortable = true,
                    IsChecked = true,
                    Disabled = true,
                    Visible = true,
                    ColumnId = (int)TableSalaryColumnPositionEnum.GSM
                });
            }

            if (!request.ColumnsExcluded.Any(a => a == (int)TableSalaryColumnPositionEnum.PositionSM))
            {
                var positionLocal = userDisplaySM.FirstOrDefault(x => x.InternalCode == (int)TableSalaryColumnPositionEnum.PositionSM);
                var positionDefault = globalLabels.FirstOrDefault(f => f.Id == (long)TableSalaryColumnPositionEnum.PositionSM);
                string positionSM = positionDefault == null && positionLocal != null && !string.IsNullOrEmpty(positionLocal.Label) ? positionLocal.Label :
                    positionDefault != null && string.IsNullOrEmpty(positionDefault.Alias) ? positionDefault.Name :
                    positionDefault != null && !string.IsNullOrEmpty(positionDefault.Alias) ? positionDefault.Alias :
                    TableSalaryColumnPositionEnum.PositionSM.GetDescription();
                listHeaderPosition.Add(new HeaderPositionInfo
                {
                    ColPos = 1,
                    ColName = TableSalaryColumnPositionEnum.PositionSM.GetDescription(),
                    NickName = positionSM,
                    Disabled = false,
                    Sortable = false,
                    Editable = false,
                    IsChecked = positionLocal == null ? true : positionLocal.IsChecked,
                    Visible = false,
                    ColumnId = (int)TableSalaryColumnPositionEnum.PositionSM
                });
            }

            if (!request.ColumnsExcluded.Any(a => a == (int)TableSalaryColumnPositionEnum.Profile))
            {
                var profileLocal = userDisplaySM.FirstOrDefault(x => x.InternalCode == (int)TableSalaryColumnPositionEnum.Profile);
                var profileDefault = globalLabels.FirstOrDefault(f => f.Id == (long)TableSalaryColumnPositionEnum.Profile);
                string profile = profileDefault == null && profileLocal != null && !string.IsNullOrEmpty(profileLocal.Label) ? profileLocal.Label :
                    profileDefault != null && string.IsNullOrEmpty(profileDefault.Alias) ? profileDefault.Name :
                    profileDefault != null && !string.IsNullOrEmpty(profileDefault.Alias) ? profileDefault.Alias :
                    TableSalaryColumnPositionEnum.Profile.GetDescription();
                listHeaderPosition.Add(new HeaderPositionInfo
                {
                    ColPos = 2,
                    ColName = TableSalaryColumnPositionEnum.Profile.GetDescription(),
                    NickName = profile,
                    Disabled = false,
                    Sortable = false,
                    Editable = false,
                    IsChecked = profileLocal == null ? true : profileLocal.IsChecked,
                    Visible = true,
                    ColumnId = (int)TableSalaryColumnPositionEnum.Profile
                });
            }

            if (!request.ColumnsExcluded.Any(a => a == (int)TableSalaryColumnPositionEnum.Company))
            {
                var companyLocal = userDisplaySM.FirstOrDefault(x => x.InternalCode == (int)TableSalaryColumnPositionEnum.Company);

                string company = TableSalaryColumnPositionEnum.Company.GetDescription();
                listHeaderPosition.Add(new HeaderPositionInfo
                {
                    ColPos = 3,
                    ColName = TableSalaryColumnPositionEnum.Company.GetDescription(),
                    NickName = company,
                    Disabled = false,
                    Sortable = false,
                    Editable = false,
                    IsChecked = companyLocal == null ? true : companyLocal.IsChecked,
                    Visible = true,
                    ColumnId = (int)TableSalaryColumnPositionEnum.Company
                });
            }

            var countHeaderPosition = listHeaderPosition.Count();

            foreach (var item in tabelasSalariaisFaixasList)
            {
                listHeaderPosition.Add(new HeaderPositionInfo
                {
                    isMidPoint = item.SalaryRangeId == (int)SalarialRanges.MidPoint,
                    ColPos = countHeaderPosition,
                    ColName = $"{(Math.Round(item.AmplitudeMidPoint * 100)).ToString()}%",
                    NickName = $"{(Math.Round(item.AmplitudeMidPoint * 100)).ToString()}%",
                    Disabled = false,
                    Editable = false,
                    IsChecked = true,
                    Visible = false
                });
                countHeaderPosition++;
            }

            return listHeaderPosition;
        }

        private double? SetValueRangeItem(SalaryTablesPositionValues salaryTableValuesItem, long i)
        {
            switch ((SalarialRanges)i)
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
        /// <summary>
        /// Get all positions by project
        /// </summary>
        /// <param name="projectId">project id</param>
        /// <param name="listGsms">lst of gsm used on project</param>
        /// <param name="permissionUser">permissions user</param>
        /// <param name="groupId">profile id</param>
        /// <param name="tableId">table id</param>
        /// <param name="filterSearch">term</param>
        /// <returns></returns>
        private async Task<List<ProjectPositionSM>> GetPositionProject
            (long projectId,
            List<int> listGsms,
            PermissionJson permissionUser,
            long? groupId,
            long tableId,
            string filterSearch,
            List<long> levelsExcept,
            IEnumerable<long> groupsExp)
        {

            var positionMapping = await _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>()
                        .Include("CargosProjetosSm")
                        .Include("Empresa")
                .GetListAsync(x => x
                .Where(s => s.ProjetoId == projectId &&
                            s.TabelaSalarialIdLocal == tableId && listGsms.Contains(s.Gsm.GetValueOrDefault()) &&
                            s.CargosProjetosSm != null &&
                            s.CargosProjetosSm.Ativo.HasValue &&
                            s.CargosProjetosSm.Ativo.Value &&
                            (!groupId.HasValue || s.CargosProjetosSm.GrupoSmidLocal == groupId.Value) &&
                            s.CargosProjetosSm.CmcodeNavigation != null &&
                            (!levelsExcept.Safe().Any() || !levelsExcept.Contains(s.CargosProjetosSm.CmcodeNavigation.NivelId)) &&
                            (!groupsExp.Safe().Any() || !groupsExp.Contains(s.CargosProjetosSm.GrupoSmidLocal)) &&
                            (string.IsNullOrWhiteSpace(filterSearch) ||
                            (!string.IsNullOrWhiteSpace(s.CargosProjetosSm.CargoSm) &&
                                s.CargosProjetosSm.CargoSm.ToLower().Contains(filterSearch.ToLower()))))
                .Select(res => new ProjectPositionSM
                {
                    ProjectPositionSMId = res.CargoProjetoSmidLocal,
                    GroupId = res.CargosProjetosSm.GrupoSmidLocal,
                    Position = res.CargosProjetosSm.CargoSm,
                    GSM = res.Gsm.GetValueOrDefault(),
                    CompanyId = res.EmpresaId,
                    Company = res.Empresa != null ? res.Empresa.NomeFantasia : "-"
                }).Distinct());

            return positionMapping;
        }
    }
}

