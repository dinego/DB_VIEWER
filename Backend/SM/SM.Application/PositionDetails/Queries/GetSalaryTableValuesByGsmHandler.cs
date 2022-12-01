using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Application.PositionDetails.Queries.Response;
using SM.Domain.Enum;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories.Extensions;
using System;
using System.Globalization;
using System.Linq;

namespace SM.Application.PositionDetails.Queries
{
    public class GetSalaryTableValuesByGsmRequest : IRequest<List<DataBodyPositionDetail>>
    {
        public long ProjectId { get; set; }
        public long TableId { get; set; }
        public long GSM { get; set; }
        public long UserId { get; set; }
        public long UnitId { get; set; }
        public ContractTypeEnum ContractType { get; set; } = ContractTypeEnum.CLT;
        public DataBaseSalaryEnum HoursType { get; set; } = DataBaseSalaryEnum.MonthSalary;
    }
    public class GetSalaryTableValuesByGsmHandler : IRequestHandler<GetSalaryTableValuesByGsmRequest, List<DataBodyPositionDetail>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMultiplierFactorHourlyInteractor _multiplierFactorHourlyInteractor;
        private readonly IMultiplierFactorTypeContratcInteractor _multiplierFactorTypeContratcInteractor;
        private readonly IPermissionUserInteractor _permissionUserInteractor;

        public GetSalaryTableValuesByGsmHandler(IUnitOfWork unitOfWork,
                                    IMultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor,
                                    IMultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor,
                                    IPermissionUserInteractor permissionUserInteractor
            )
        {
            _unitOfWork = unitOfWork;
            _multiplierFactorHourlyInteractor = multiplierFactorHourlyInteractor;
            _multiplierFactorTypeContratcInteractor = multiplierFactorTypeContratcInteractor;
            _permissionUserInteractor = permissionUserInteractor;
        }

        public async Task<List<DataBodyPositionDetail>> Handle(GetSalaryTableValuesByGsmRequest request, CancellationToken cancellationToken)
        {
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

            var salaryTableValue = await _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                                   .Include("TabelasSalariais.GruposProjetosSalaryMarkMapeamento")
                                   .Include("TabelasSalariais.CargosProjetosSmMapeamento")
                                   .GetAsync(x => x.Where(s => s.ProjetoId == request.ProjectId &&
                                                                request.GSM == s.Grade &&
                                                                s.TabelasSalariais.GruposProjetosSalaryMarkMapeamento
                                                                    .Any(a => a.TabelaSalarialIdLocal == request.TableId &&
                                                                              a.ProjetoId == request.ProjectId &&
                                                                              !groupsExp.Safe().Contains(a.GrupoProjetoSmidLocal)) &&

                                                              (!tablesExp.Safe().Any() || !tablesExp.Contains(s.TabelaSalarialIdLocal)) &&
                                                                s.TabelaSalarialIdLocal == request.TableId &&
                                                                s.TabelasSalariais != null &&
                                                               (s.TabelasSalariais.Ativo.HasValue && s.TabelasSalariais.Ativo.Value))
                                                       .Select(s => new SalaryTablesPositionDetailsValues
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
                                                           LocalIdSalaryTable = s.TabelaSalarialIdLocal,
                                                           TableSalaryName = s.TabelasSalariais != null && s.TabelasSalariais.TabelaSalarial != null ? s.TabelasSalariais.TabelaSalarial : String.Empty
                                                       })
                                                 );

            if (salaryTableValue == null)
                return new List<DataBodyPositionDetail>();

            var lstBody = new List<DataBodyPositionDetail>();

            var lowestSalary = await _unitOfWork.GetRepository<TabelasSalariaisGrades, long>()
                .GetAsync(g => g.Where(w => w.TabelaSalarialIdLocal == request.TableId && w.ProjetoId == request.ProjectId)?
                .Select(s => s.MenorSalario));

            var resultLowestSalary = lowestSalary.GetValueOrDefault(0) * factorHour * multiplierFactor.GetValueOrDefault(0);

            var grade = salaryTableValue.GSM;
            var salaryTable = salaryTableValue.TableSalaryName;
            var colPos = 0;
            lstBody.Add(new DataBodyPositionDetail
            {
                ColPos = colPos,
                Value = salaryTableValue.TableSalaryName,
                @Type = salaryTable.GetType().Name,
            });
            colPos++;

            lstBody.Add(new DataBodyPositionDetail
            {
                ColPos = colPos,
                Value = request.UnitId.ToString(),
                @Type = request.UnitId.ToString().GetType().Name,
            });
            colPos++;

            lstBody.Add(new DataBodyPositionDetail
            {
                ColPos = colPos,
                Value = grade.ToString(),
                @Type = grade.GetType().Name,
            });
            colPos++;

            foreach (var item in salaryTableRanges)
            {
                var bodyPosition = new DataBodyPositionDetail();

                double? rangeValue = SetValueRangeItem(salaryTableValue, item.SalaryRangeId);
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
                    };
                    colPos++;
                    lstBody.Add(bodyPosition);
                    continue;
                }
                bodyPosition = new DataBodyPositionDetail
                {
                    ColPos = colPos,
                    Value = "-",
                    @Type = typeof(double).Name,
                };
                colPos++;
                lstBody.Add(bodyPosition);
            }
            return lstBody;
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

