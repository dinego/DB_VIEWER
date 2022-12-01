using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Application.PositionDetails.Queries.Response;
using SM.Domain.Enum;
using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories.Extensions;
using SM.Application.Interactors;
using System;
using System.Globalization;
using System.Linq;
using SM.Application.Helpers;

namespace SM.Application.PositionDetails.Queries
{
    public class GetSalaryTablePositionDetailsRequest : IRequest<GetSalaryTablePositionDetailsResponse>
    {
        public long ProjectId { get; set; }
        public long PositionId { get; set; }
        public long TableId { get; set; }
        public long? UnitId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public long UserId { get; set; }
        public bool? IsAsc { get; set; } = null;
        public int? SortColumnId { get; set; } = null;
        public ContractTypeEnum ContractType { get; set; } = ContractTypeEnum.CLT;
        public DataBaseSalaryEnum HoursType { get; set; } = DataBaseSalaryEnum.MonthSalary;
    }
    public class GetSalaryTablePositionDetailsHandler : IRequestHandler<GetSalaryTablePositionDetailsRequest, GetSalaryTablePositionDetailsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMultiplierFactorHourlyInteractor _multiplierFactorHourlyInteractor;
        private readonly IMultiplierFactorTypeContratcInteractor _multiplierFactorTypeContratcInteractor;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;
        private readonly IEnumerable<SalaryTablePositionDetailsEnum> _salaryTablePositionDetailsColumns;
        private readonly IGetLocalLabelsInteractor _getLocalLabelsInteractor;

        public GetSalaryTablePositionDetailsHandler(IUnitOfWork unitOfWork,
                                    IMultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor,
                                    IMultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor,
                                    IPermissionUserInteractor permissionUserInteractor,
                                    IGetGlobalLabelsInteractor getGlobalLabelsInteractor,
                                    IGetLocalLabelsInteractor getLocalLabelsInteractor
            )
        {
            _unitOfWork = unitOfWork;
            _multiplierFactorHourlyInteractor = multiplierFactorHourlyInteractor;
            _multiplierFactorTypeContratcInteractor = multiplierFactorTypeContratcInteractor;
            _permissionUserInteractor = permissionUserInteractor;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
            _getLocalLabelsInteractor = getLocalLabelsInteractor;
            _salaryTablePositionDetailsColumns = Enum.GetValues(typeof(SalaryTablePositionDetailsEnum)) as
                IEnumerable<SalaryTablePositionDetailsEnum>;
        }

        public async Task<GetSalaryTablePositionDetailsResponse> Handle(GetSalaryTablePositionDetailsRequest request, CancellationToken cancellationToken)
        {
            #region Load Global Labels and Local Labels
            var globalLabels = await _getGlobalLabelsInteractor.Handler(request.ProjectId);
            var rangeGlobalIds = globalLabels.Safe().Any() ?
                                 globalLabels.Where(co => !co.IsChecked)
                                                   .Select(s => (int)s.Id).ToList() :
                                 new List<int>();

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
            var levelsExcept = permissionUser.Levels.Safe().Any() ? permissionUser.Levels.ToList() : new List<long>();
            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);
            var typeOfContractIdsExp = permissionUser.TypeOfContract;

            var isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;
            var sortColumnProperty = request.SortColumnId.HasValue ?
                _salaryTablePositionDetailsColumns.FirstOrDefault(f => (int)f == request.SortColumnId.Value).ToString() :
                SalaryTablePositionDetailsEnum.GSM.ToString();

            var salaryTableRanges = await _unitOfWork.GetRepository<TabelasSalariaisFaixas, long>()
            .GetListAsync(x => x.Where(str => str.TabelaSalarialIdLocal == request.TableId &&
                                        str.ProjetoId == request.ProjectId)
                                        .Select(res => new SalaryTablesPositionDetailsRanges
                                        {
                                            Id = res.Id,
                                            SalaryRangeId = res.FaixaSalarialId,
                                            AmplitudeMidPoint = res.AmplitudeMidPoint,
                                            LocalIdSalaryTable = res.TabelaSalarialIdLocal
                                        }).OrderBy(o => o.SalaryRangeId));

            var mapPositionSM = await _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>()
                        .Include("CargosProjetosSm")
                        .Include("Empresa")
                        .GetListAsync(x => x.Where(cpm => cpm.ProjetoId == request.ProjectId &&
                        cpm.TabelaSalarialIdLocal == request.TableId &&
                        cpm.CargoProjetoSmidLocal == request.PositionId &&
                        (!groupsExp.Safe().Contains(cpm.CargosProjetosSm.GrupoSmidLocal)))
                        .Select(res => new
                        {
                            ProjectPositionSMId = res.CargoProjetoSmidLocal,
                            GSM = res.Gsm.GetValueOrDefault(),
                            GroupId = res.CargosProjetosSm != null ? res.CargosProjetosSm.GrupoSmidLocal : (long?)null,
                            CompanyId = res.EmpresaId,
                            Company = res.Empresa != null ? res.Empresa.NomeFantasia : string.Empty,
                            TableId = res.TabelaSalarialIdLocal.GetValueOrDefault(),
                        }));

            var lstGsm = mapPositionSM.Select(res => res.GSM);
            var groupsMapping = mapPositionSM.Select(res => res.GroupId);
            var companyByTableId = mapPositionSM.Select(res => res.CompanyId);
            var positionsByGsmSmIdList = mapPositionSM.Select(res => res.ProjectPositionSMId);

            var salaryBase = await _unitOfWork.GetRepository<BaseSalariais, long>()
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

            var salaryTableValues = await _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                                   .Include("TabelasSalariais.GruposProjetosSalaryMarkMapeamento")
                                   .Include("TabelasSalariais.CargosProjetosSmMapeamento")
                                   .GetListAsync(x => x.Where(s => s.ProjetoId == request.ProjectId &&
                                                                lstGsm.Safe().Contains(s.Grade) &&
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
                                                       .Select(s => new SalaryTablesPositionDetailsValues
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
                                                           TableSalaryName = s.TabelasSalariais != null && s.TabelasSalariais.TabelaSalarial != null ? s.TabelasSalariais.TabelaSalarial : String.Empty
                                                       })
                                                 );

            if (!salaryTableValues.Any())
            {
                return new GetSalaryTablePositionDetailsResponse
                {
                    Table = new TableInfoPositionDetail
                    {
                        Header = new List<HeaderInfoPositionDetail>(),
                        Body = new List<List<DataBodyPositionDetail>>()
                    }
                };
            }
            var salaryTableAll = new List<SalaryTablesPositionDetailsValues>();
            mapPositionSM.ForEach(mpsm =>
            {
                salaryTableValues.Where(st => st.GSM == mpsm.GSM).AsParallel().ForAll(sl =>
                {
                    var values = sl.Map().ToANew<SalaryTablesPositionDetailsValues>();
                    lock (salaryTableAll)
                    {
                        values.Company = mpsm.Company;
                        values.CompanyId = mpsm.CompanyId;
                        values.ProjectPositionSMId = mpsm.ProjectPositionSMId;
                        if (!salaryTableAll.Any(st => st.GSM == sl.GSM &&
                        !string.IsNullOrEmpty(st.Company) && st.Company.Equals(mpsm.Company)))
                            salaryTableAll.Add(values);
                    }
                });
            });

            salaryTableAll = salaryTableAll.AsQueryable().OrderBy(sortColumnProperty, isDesc)
                          .Skip((request.Page - 1) * request.PageSize)
                          .Take(request.PageSize).ToList();

            var getSalaryTableResponse = new GetSalaryTablePositionDetailsResponse
            {
                Table = new TableInfoPositionDetail
                {
                    Header = new List<HeaderInfoPositionDetail>(),
                    Body = new List<List<DataBodyPositionDetail>>()
                }
            };

            var listHeader = new List<HeaderInfoPositionDetail>();
            var lowestSalary = await _unitOfWork.GetRepository<TabelasSalariaisGrades, long>()
                .GetAsync(g => g.Where(w => w.TabelaSalarialIdLocal == request.TableId && w.ProjetoId == request.ProjectId)?
                .Select(s => s.MenorSalario));

            var resultLowestSalary = lowestSalary.GetValueOrDefault(0) * factorHour * multiplierFactor.GetValueOrDefault(0);

            var lstSalaryTableRanges = salaryTableRanges
                .Where(x => x.LocalIdSalaryTable == salaryTableValues.FirstOrDefault().LocalIdSalaryTable)
                .OrderBy(x => x.AmplitudeMidPoint).ToList();

            var lineTable = userDisplaySM.Any() ?
                userDisplaySM.FirstOrDefault(f => f.InternalCode == (int)TableSalaryColumnEnum.TableSalaryName) : null;

            var tableLocal = userDisplaySM.FirstOrDefault(x => x.InternalCode == (int)TableSalaryColumnEnum.TableSalaryName);
            string table = tableLocal != null && !string.IsNullOrEmpty(tableLocal.Label) ? tableLocal.Label :
                SalaryTablePositionDetailsEnum.TableSalaryName.GetDescription();
            listHeader.Add(new HeaderInfoPositionDetail
            {
                ColPos = 0,
                ColName = SalaryTablePositionDetailsEnum.TableSalaryName.GetDescription(),
                NickName = table,
                Sortable = true,
                ColumnId = (int)SalaryTablePositionDetailsEnum.TableSalaryName
            });

            var companyLocal = userDisplaySM.FirstOrDefault(x => x.InternalCode == (int)TableSalaryColumnEnum.Company);
            string company = companyLocal != null && !string.IsNullOrEmpty(companyLocal.Label) ? companyLocal.Label :
                SalaryTablePositionDetailsEnum.Company.GetDescription();
            listHeader.Add(new HeaderInfoPositionDetail
            {
                ColPos = 1,
                ColName = SalaryTablePositionDetailsEnum.Company.GetDescription(),
                NickName = SalaryTablePositionDetailsEnum.Company.GetDescription(),
                Sortable = true,
                ColumnId = (int)SalaryTablePositionDetailsEnum.Company
            });

            var gsmLocal = userDisplaySM.FirstOrDefault(x => x.InternalCode == (int)SalaryTablePositionDetailsEnum.GSM);
            var gsmDefault = globalLabels.FirstOrDefault(f => f.Id == (long)SalaryTablePositionDetailsEnum.GSM);
            string gsm = gsmDefault == null && gsmLocal != null && !string.IsNullOrEmpty(gsmLocal.Label) ? gsmLocal.Label :
                gsmDefault != null && string.IsNullOrEmpty(gsmDefault.Alias) ? gsmDefault.Name :
                gsmDefault != null && !string.IsNullOrEmpty(gsmDefault.Alias) ? gsmDefault.Alias :
                SalaryTablePositionDetailsEnum.GSM.GetDescription();
            listHeader.Add(new HeaderInfoPositionDetail
            {
                ColPos = 2,
                ColName = SalaryTablePositionDetailsEnum.GSM.GetDescription(),
                NickName = gsm,
                Sortable = true,
                ColumnId = (int)SalaryTablePositionDetailsEnum.GSM,
            });

            var countHeader = listHeader.Count;

            foreach (var item in lstSalaryTableRanges)
            {
                listHeader.Add(new HeaderInfoPositionDetail
                {
                    isMidPoint = item.SalaryRangeId == (int)SalarialRanges.MidPoint,
                    ColPos = countHeader,
                    ColName = $"{(Math.Round(item.AmplitudeMidPoint * 100)).ToString()}%",
                    NickName = $"{(Math.Round(item.AmplitudeMidPoint * 100)).ToString()}%",
                });
                countHeader++;
            }

            foreach (var salaryTableValuesItem in salaryTableAll)
            {
                List<DataBodyPositionDetail> listBody = new List<DataBodyPositionDetail>();
                var grade = salaryTableValuesItem.GSM;
                var salaryTable = salaryTableValuesItem.TableSalaryName;
                var colPos = 0;

                var positionItem = salaryBase
                        .Where(x => x.CompanyId == salaryTableValuesItem.CompanyId &&
                                    x.PositionSMMM.HasValue &&
                                    salaryTableValuesItem.ProjectPositionSMId == x.PositionSMMM.Value &&
                                    x.TypeOfContractId.HasValue &&
                                    !typeOfContractIdsExp.Contains(x.TypeOfContractId.Value));

                bool isOccupantCLT = positionItem.Safe().Any() ?
                        positionItem.ToList().Any(item => PJOrCLT.IsCLT(item?.TypeOfContract).GetValueOrDefault(false)) : false;

                bool isOccupantPJ = positionItem.Safe().Any() ?
                        positionItem.ToList().Any(item => PJOrCLT.IsPJ(item?.TypeOfContract).GetValueOrDefault(false)) : false;

                listBody.Add(new DataBodyPositionDetail
                {
                    ColPos = colPos,
                    Value = salaryTableValuesItem.TableSalaryName,
                    @Type = salaryTable.GetType().Name,
                    OccupantCLT = isOccupantCLT,
                    OccupantPJ = isOccupantPJ
                });
                colPos++;

                listBody.Add(new DataBodyPositionDetail
                {
                    ColPos = colPos,
                    Value = salaryTableValuesItem.Company,
                    @Type = salaryTableValuesItem.Company.GetType().Name,
                    OccupantCLT = isOccupantCLT,
                    OccupantPJ = isOccupantPJ
                });
                colPos++;

                listBody.Add(new DataBodyPositionDetail
                {
                    ColPos = colPos,
                    Value = grade.ToString(),
                    @Type = grade.GetType().Name,
                    OccupantCLT = isOccupantCLT,
                    OccupantPJ = isOccupantPJ
                });
                colPos++;

                foreach (var item in lstSalaryTableRanges)
                {
                    var bodyPosition = new DataBodyPositionDetail();

                    double? rangeValue = SetValueRangeItem(salaryTableValuesItem, item.SalaryRangeId);
                    double valueSalary = rangeValue.HasValue ? rangeValue.Value * factorHour * multiplierFactor.GetValueOrDefault(0) : 0;

                    if (valueSalary >= resultLowestSalary)
                    {
                        bodyPosition = new DataBodyPositionDetail
                        {
                            isMidPoint = item.SalaryRangeId == (int)SalarialRanges.MidPoint,
                            ColPos = colPos,
                            Value = valueSalary == 0 ? "-" : (request.HoursType == DataBaseSalaryEnum.HourSalary ?
                                Math.Round(valueSalary, 2).ToString("N2", CultureInfo.InvariantCulture) :
                                Math.Round(valueSalary, 0).ToString(CultureInfo.InvariantCulture)),
                            @Type = valueSalary.GetType().Name,
                            OccupantCLT = isOccupantCLT,
                            OccupantPJ = isOccupantPJ
                        };
                        colPos++;
                        listBody.Add(bodyPosition);
                        continue;
                    }
                    bodyPosition = new DataBodyPositionDetail
                    {
                        ColPos = colPos,
                        Value = "-",
                        @Type = typeof(double).Name,
                        OccupantCLT = isOccupantCLT,
                        OccupantPJ = isOccupantPJ
                    };
                    colPos++;
                    listBody.Add(bodyPosition);
                }
                getSalaryTableResponse.Table.Header = listHeader;
                if (listBody.Count > 0)
                    getSalaryTableResponse.Table.Body.Add(listBody);
            }
            getSalaryTableResponse.NextPage = getSalaryTableResponse.Table.Body.Count > 0 ? request.Page + 1 : 0;
            return getSalaryTableResponse;
        }

        private double? SetValueRangeItem(SalaryTablesPositionDetailsValues salaryTableValuesItem, long salaryRangeId)
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

