using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Interactors;
using SM.Application.Interactors.Interfaces;
using SM.Application.TableSalary.Validators;
using SM.Domain.Enum;
using SM.Domain.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Repositories.Extensions;
using SM.Application.TableSalary.Queries.Response;
using SM.Domain.Helpers;
using SM.Domain.Common;
using AgileObjects.AgileMapper.Extensions;

namespace SM.Application.GetSalaryTable
{
    public class GetSalaryTableExcelRequest : IRequest<GetSalaryTableExcelResponse>
    {
        public long ProjectId { get; set; }
        public long TableId { get; set; }
        public long? GroupId { get; set; }
        public long? UnitId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public ContractTypeEnum ContractType { get; set; } = ContractTypeEnum.CLT;
        public DataBaseSalaryEnum HoursType { get; set; } = DataBaseSalaryEnum.MonthSalary;
        public List<int> ColumnsExcluded { get; set; } = new List<int>();
        public long UserId { get; set; }
        public bool? IsAsc { get; set; } = null;
        public int? SortColumnId { get; set; } = null;
        public bool ShowAllGsm { get; set; }
    }

    public class GetSalaryTableExcelResponse
    {
        public IEnumerable<SalaryTableExcelHeader> Headers { get; set; }
        public IEnumerable<List<SalaryTableExcelBody>> Body { get; set; }
        public string FileName { get; set; }
        public string Table { get; set; }
    }

    public class SalaryTableExcelBody
    {
        public string Value { get; set; } = "-";
        public bool isMidPoint { get; set; }
    }

    public class SalaryTableExcelHeader
    {
        public string Value { get; set; }
    }

    public class GetSalaryTableExcelHandler
        : IRequestHandler<GetSalaryTableExcelRequest, GetSalaryTableExcelResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMultiplierFactorHourlyInteractor _multiplierFactorHourlyInteractor;
        private readonly IMultiplierFactorTypeContratcInteractor _multiplierFactorTypeContratcInteractor;
        private readonly IValidator<GetTableSalaryPermissionValidatorsException> _validator;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly ValidatorResponse _validatorResponse;
        private readonly InfoApp _infoApp;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;
        private readonly IEnumerable<TableSalaryColumnEnum> _tableSalaryColumns;
        private readonly IGetLocalLabelsInteractor _getLocalLabelsInteractor;

        public GetSalaryTableExcelHandler(IUnitOfWork unitOfWork,
                                    IMultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor,
                                    IMultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor,
                                    IValidator<GetTableSalaryPermissionValidatorsException> validator,
                                    IPermissionUserInteractor permissionUserInteractor,
                                    ValidatorResponse validatorResponse,
                                    InfoApp infoApp,
                                    IGetGlobalLabelsInteractor getGlobalLabelsInteractor,
                                    IGetLocalLabelsInteractor getLocalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _multiplierFactorHourlyInteractor = multiplierFactorHourlyInteractor;
            _multiplierFactorTypeContratcInteractor = multiplierFactorTypeContratcInteractor;
            _validator = validator;
            _permissionUserInteractor = permissionUserInteractor;
            _validatorResponse = validatorResponse;
            _infoApp = infoApp;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
            _tableSalaryColumns = Enum.GetValues(typeof(TableSalaryColumnEnum)) as
                IEnumerable<TableSalaryColumnEnum>;
            _tableSalaryColumns = _tableSalaryColumns.FirstOrDefault().GetWithOrder().Select(s =>
            (TableSalaryColumnEnum)Enum.Parse(typeof(TableSalaryColumnEnum), s));
            _getLocalLabelsInteractor = getLocalLabelsInteractor;
        }
        public async Task<GetSalaryTableExcelResponse> Handle(GetSalaryTableExcelRequest request,
            CancellationToken cancellationToken)
        {
            var table = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                                 .GetAsync(g => g.Where(x => x.TabelaSalarialIdLocal == request.TableId && x.ProjetoId == request.ProjectId)
                                 .Select(res => res.TabelaSalarial));

            var tableName = table == null ? $"{ModulesEnum.TableSalary.GetDescription()}" :
                $"{ModulesEnum.TableSalary.GetDescription()} | {table}";


            var fileName = $"{_infoApp.Name}_{ModulesEnum.TableSalary.GetDescription()}";

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

            var factorHour = await _multiplierFactorHourlyInteractor.Handler(request.ProjectId, request.HoursType);
            var multiplierFactor = await _multiplierFactorTypeContratcInteractor.Handler(request.ProjectId, request.ContractType);
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);
            var groupsExp = permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var tablesExp = permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                        .SubItems;

            var isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;
            var sortColumnProperty = request.SortColumnId.HasValue ?
                _tableSalaryColumns.FirstOrDefault(f => (int)f == request.SortColumnId.Value).ToString() :
                TableSalaryColumnEnum.GSM.ToString();

            var salaryTableRanges = await _unitOfWork.GetRepository<TabelasSalariaisFaixas, long>()
            .GetListAsync(x => x.Where(str => str.TabelaSalarialIdLocal == request.TableId &&
                                        str.ProjetoId == request.ProjectId)
                                        .Select(res => new SalaryTablesRanges
                                        {
                                            Id = res.Id,
                                            SalaryRangeId = res.FaixaSalarialId,
                                            AmplitudeMidPoint = res.AmplitudeMidPoint,
                                            LocalIdSalaryTable = res.TabelaSalarialIdLocal
                                        }).OrderBy(o => o.SalaryRangeId));

            var groupProjectMapping = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                                      .Include("Empresa")
                                      .GetListAsync(x => x.Where(gpm => gpm.ProjetoId == request.ProjectId &&
                                                                 gpm.TabelaSalarialIdLocal == request.TableId &&
                                                                 (!request.UnitId.HasValue || gpm.EmpresaId == request.UnitId.Value) &&
                                                                 !groupsExp.Safe().Contains(gpm.GrupoProjetoSmidLocal) &&
                                                                 (!request.GroupId.HasValue || gpm.GrupoProjetoSmidLocal == request.GroupId.Value))
                                                                .Select(res => new SalaryMarkMappingProjectsGroups
                                                                {
                                                                    GroupId = res.GrupoProjetoSmidLocal,
                                                                    CompanyId = res.EmpresaId,
                                                                    RangeDown = res.FaixaIdInferior,
                                                                    RangeUp = res.FaixaIdSuperior,
                                                                    TableId = res.TabelaSalarialIdLocal,
                                                                    Company = res.Empresa != null ? res.Empresa.NomeFantasia : string.Empty
                                                                }));

            var groupsMapping = groupProjectMapping.Select(res => res.GroupId);
            var mapPositionSM = await _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>()
                        .Include("CargosProjetosSm")
                        .Include("Empresa")
                        .GetListAsync(x => x.Where(cpm => cpm.ProjetoId == request.ProjectId &&
                        cpm.TabelaSalarialIdLocal == request.TableId &&
                        (!groupsExp.Safe().Contains(cpm.CargosProjetosSm.GrupoSmidLocal)) &&
                        (!request.GroupId.HasValue || cpm.CargosProjetosSm.GrupoSmidLocal == request.GroupId))
                        .Select(res => new
                        {
                            GSM = res.Gsm.GetValueOrDefault(),
                            GroupId = res.CargosProjetosSm != null ? res.CargosProjetosSm.GrupoSmidLocal : (long?)null,
                            CompanyId = res.EmpresaId,
                            TableId = res.TabelaSalarialIdLocal.GetValueOrDefault()
                        }));

            var salaryTableValues = _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                                   .Include("TabelasSalariais.GruposProjetosSalaryMarkMapeamento")
                                   .GetQueryList(x => x.Where(s => s.ProjetoId == request.ProjectId &&
                                                                s.TabelasSalariais.GruposProjetosSalaryMarkMapeamento
                                                                    .Any(a => a.TabelaSalarialIdLocal == request.TableId &&
                                                                              a.ProjetoId == request.ProjectId &&
                                                                              groupsMapping.Safe().Contains(a.GrupoProjetoSmidLocal)) &&

                                                              (!request.UnitId.HasValue || (s.Projeto != null &&
                                                                s.Projeto.ProjetosSalaryMarkEmpresas
                                                                        .Any(psm => psm.EmpresaId == request.UnitId.Value &&
                                                                                    psm.ProjetoId == request.ProjectId))) &&

                                                              (!tablesExp.Safe().Any() || !tablesExp.Contains(s.TabelaSalarialIdLocal)) &&
                                                                s.TabelaSalarialIdLocal == request.TableId &&
                                                                s.TabelasSalariais != null &&
                                                               (s.TabelasSalariais.Ativo.HasValue && s.TabelasSalariais.Ativo.Value))
                                                       .Select(s => new SalaryTablesValues
                                                       {
                                                           GSM = s.Grade,
                                                           Company = string.Empty,
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
                                                           TableSalaryName = s.TabelasSalariais != null && s.TabelasSalariais.TabelaSalarial != null ? s.TabelasSalariais.TabelaSalarial : String.Empty,
                                                       })
                                                 );

            if (!salaryTableValues.Any())
                return new GetSalaryTableExcelResponse();

            var salaryTableAll = new List<SalaryTablesValues>();
            groupProjectMapping.ForEach(gp =>
            {
                var gsmByPosition = mapPositionSM.Where(mp => mp.GroupId == gp.GroupId &&
                                                     mp.CompanyId == gp.CompanyId &&
                                                     mp.TableId == gp.TableId)
                                              .Select(res => res.GSM);
                var salaryValues = salaryTableValues.Where(x => x.LocalIdSalaryTable == gp.TableId &&
                        (request.ShowAllGsm || gsmByPosition.Contains(x.GSM)));
                var lstValues = new List<SalaryTablesValues>(salaryValues);
                lstValues.AsParallel().ForAll(sl =>
                {
                    var values = sl.Map().ToANew<SalaryTablesValues>();
                    lock (salaryTableAll)
                    {
                        values.Company = gp.Company;
                        values.CompanyId = gp.CompanyId;
                        if (!salaryTableAll.Any(st => st.GSM == sl.GSM &&
                        !string.IsNullOrEmpty(st.Company) && st.Company.Equals(gp.Company)))
                            salaryTableAll.Add(values);
                    }
                });
            });

            salaryTableAll = salaryTableAll.AsQueryable().OrderBy(sortColumnProperty, isDesc).ToList();

            AddUserConfigurationUncheckedOnColumnExceptionList(request, userDisplaySM, globalLabels);

            var lowestSalary = await _unitOfWork.GetRepository<TabelasSalariaisGrades, long>()
                .GetAsync(g => g.Where(w => w.TabelaSalarialIdLocal == request.TableId && w.ProjetoId == request.ProjectId)?
                .Select(s => s.MenorSalario));

            var resultLowestSalary = lowestSalary.GetValueOrDefault(0) * factorHour * multiplierFactor.GetValueOrDefault(0);
            long rangeDown = 0;
            long rangeUp = 0;

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
                .Where(x => x.LocalIdSalaryTable == salaryTableValues.FirstOrDefault().LocalIdSalaryTable &&
                            (!request.GroupId.HasValue || x.SalaryRangeId >= rangeDown && x.SalaryRangeId <= rangeUp))
                .OrderBy(x => x.AmplitudeMidPoint).ToList();

            var headersExcel = PrepareHeaders(request, userDisplaySM, globalLabels, lstSalaryTableRanges);
            var bodyExcel = new List<List<SalaryTableExcelBody>>();

            foreach (var salaryTableValuesItem in salaryTableAll)
            {
                var listBody = new List<SalaryTableExcelBody>();
                var grade = salaryTableValuesItem.GSM;

                if (!request.ColumnsExcluded.Any(a => a == (int)TableSalaryColumnEnum.GSM))
                    listBody.Add(new SalaryTableExcelBody { Value = grade.ToString() });
                if (!request.ColumnsExcluded.Any(a => a == (int)TableSalaryColumnEnum.TableSalaryName))
                    listBody.Add(new SalaryTableExcelBody { Value = salaryTableValuesItem.TableSalaryName });
                if (!request.ColumnsExcluded.Any(a => a == (int)TableSalaryColumnEnum.Company))
                    listBody.Add(new SalaryTableExcelBody { Value = salaryTableValuesItem.Company });

                foreach (var item in lstSalaryTableRanges)
                {
                    bool setGroupRange = request.GroupId.HasValue ? groupProjectMapping.Any(gp => gp.GroupId == request.GroupId.Value &&
                                                                             gp.CompanyId == salaryTableValuesItem.CompanyId &&
                                                                             gp.RangeDown <= item.SalaryRangeId &&
                                                                             gp.RangeUp >= item.SalaryRangeId) : true;

                    double? rangeValue = setGroupRange ? SetValueRangeItem(salaryTableValuesItem, item.SalaryRangeId) : null;
                    var valueSalary = rangeValue.HasValue ? rangeValue.Value * factorHour * multiplierFactor.GetValueOrDefault(0) : 0;

                    if (valueSalary >= resultLowestSalary)
                    {
                        listBody.Add(new SalaryTableExcelBody
                        {
                            isMidPoint = item.SalaryRangeId == (int)SalarialRanges.MidPoint,
                            Value = Math.Round(valueSalary, 0).ToString("n2", CultureInfo.InvariantCulture),
                        });
                        continue;
                    }
                    listBody.Add(new SalaryTableExcelBody());
                }

                if (listBody.Count > 0)
                    bodyExcel.Add(listBody);
            }

            return new GetSalaryTableExcelResponse
            {
                FileName = fileName,
                Table = tableName,
                Headers = headersExcel,
                Body = bodyExcel
            };
        }

        private double? SetValueRangeItem(SalaryTablesValues salaryTableValuesItem, long salaryRangeId)
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

        private void AddUserConfigurationUncheckedOnColumnExceptionList(GetSalaryTableExcelRequest request,
            IEnumerable<LocalLabelsResponse> userDisplaySM,
            IEnumerable<GlobalLabelsJson> globalLabels)
        {
            userDisplaySM.Where(x => !x.IsChecked).ToList()
            .ForEach(ud =>
            {
                if (request.ColumnsExcluded.Safe().Any(col => col == ud.InternalCode))
                    return;
                switch (ud.InternalCode)
                {
                    case (int)TableSalaryColumnEnum.TableSalaryName:
                        request.ColumnsExcluded.Add((int)TableSalaryColumnEnum.TableSalaryName);
                        break;
                    case (int)TableSalaryColumnEnum.Company:
                        request.ColumnsExcluded.Add((int)TableSalaryColumnEnum.Company);
                        break;
                }
            });
            var rangeGlobalIds = globalLabels.Any() ?
                globalLabels
                .Where(co => !co.IsChecked)?
                .Select(s => (int)s.Id)?
                .ToList() : new List<int>();

            if (rangeGlobalIds.Safe().Any())
                request.ColumnsExcluded.AddRange(rangeGlobalIds);
        }

        private List<SalaryTableExcelHeader> PrepareHeaders(GetSalaryTableExcelRequest request,
            IEnumerable<LocalLabelsResponse> userDisplaySM,
            IEnumerable<GlobalLabelsJson> globalLabels,
            List<SalaryTablesRanges> salaryTableRanges)
        {

            var headers = new List<SalaryTableExcelHeader>();

            if (!request.ColumnsExcluded.Any(a => a == (int)TableSalaryColumnEnum.GSM))
            {
                var gsmLocal = userDisplaySM.FirstOrDefault(x => x.InternalCode == (int)TableSalaryColumnEnum.GSM);
                var gsmDefault = globalLabels.FirstOrDefault(f => f.Id == (long)TableSalaryColumnEnum.GSM);
                string gsm = gsmDefault == null && gsmLocal != null && !string.IsNullOrEmpty(gsmLocal.Label) ? gsmLocal.Label :
                    gsmDefault != null && string.IsNullOrEmpty(gsmDefault.Alias) ? gsmDefault.Name :
                    gsmDefault != null && !string.IsNullOrEmpty(gsmDefault.Alias) ? gsmDefault.Alias :
                    TableSalaryColumnEnum.GSM.GetDescription();
                headers.Add(new SalaryTableExcelHeader
                {
                    Value = gsm
                });
            }

            if (!request.ColumnsExcluded.Any(a => a == (int)TableSalaryColumnEnum.TableSalaryName))
            {
                var tableLocal = userDisplaySM.FirstOrDefault(x => x.InternalCode == (int)TableSalaryColumnEnum.TableSalaryName);
                string table = tableLocal != null && !string.IsNullOrEmpty(tableLocal.Label) ? tableLocal.Label :
                    TableSalaryColumnEnum.TableSalaryName.GetDescription();
                headers.Add(new SalaryTableExcelHeader
                {
                    Value = table,
                });
            }

            if (!request.ColumnsExcluded.Any(a => a == (int)TableSalaryColumnEnum.Company))
            {
                var companyLocal = userDisplaySM.FirstOrDefault(x => x.InternalCode == (int)TableSalaryColumnEnum.Company);
                string company = companyLocal != null && !string.IsNullOrEmpty(companyLocal.Label) ? companyLocal.Label :
                    TableSalaryColumnEnum.Company.GetDescription();
                headers.Add(new SalaryTableExcelHeader
                {
                    Value = company,
                });
            }

            //dinamic header
            var bodyExcel = new List<List<ExcelBody>>();
            //header dinamic 
            foreach (var item in salaryTableRanges)
            {
                headers.Add(new SalaryTableExcelHeader
                {
                    Value = $"{(Math.Round(item.AmplitudeMidPoint * 100))}%",
                });
            }
            return headers;
        }
    }
}

