using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Application.PositionDetails.Queries.Response;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.TableSalary.Queries
{
    public class GetEditTableValuesRequest : IRequest<GetEditTableValuesResponse>
    {
        public long ProjectId { get; set; }
        public long TableId { get; set; }
        public long? GroupId { get; set; }
        public long UserId { get; set; }
        public long CompanyId { get; set; }
    }


    public class GetEditTableValuesHandler : IRequestHandler<GetEditTableValuesRequest, GetEditTableValuesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;

        public GetEditTableValuesHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
        }

        public async Task<GetEditTableValuesResponse> Handle(GetEditTableValuesRequest request, CancellationToken cancellationToken)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var tablesExp = permissionUser.Contents?
                    .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                    .SubItems;

            var salaryTableRanges = await _unitOfWork.GetRepository<TabelasSalariaisFaixas, long>()
                .GetListAsync(x => x.Where(str =>
                                    str.TabelaSalarialIdLocal == request.TableId &&
                                    (!tablesExp.Safe().Any() || !tablesExp.Contains(str.TabelaSalarialIdLocal)) &&
                                    str.ProjetoId == request.ProjectId)
                                    .Select(res => new
                                    {
                                        SalaryRangeId = res.FaixaSalarialId,
                                        AmplitudeMidPoint = res.AmplitudeMidPoint
                                    }).OrderBy(o => o.SalaryRangeId));

            var salaryTable = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                .Include("TabelasSalariaisValores")
                .GetAsync(table => table.Where(w => w.ProjetoId == request.ProjectId &&
                                               (!tablesExp.Safe().Any() || !tablesExp.Contains(w.TabelaSalarialIdLocal)) &&
                                               w.TabelaSalarialIdLocal == request.TableId)
                .Select(s => new SalaryTable
                {
                    SalarialTableName = s.TabelaSalarial,
                    SalaryTableValues = s.TabelasSalariaisValores
                            .Where(w => w.ProjetoId == request.ProjectId &&
                                        w.TabelaSalarialIdLocal == request.TableId)
                        .Select(stv => new SalaryTableValues
                        {
                            GSM = stv.Grade,
                            SalaryTableLocalId = stv.TabelaSalarialIdLocal,
                            Minor6 = stv.FaixaMenos6,
                            Minor5 = stv.FaixaMenos5,
                            Minor4 = stv.FaixaMenos4,
                            Minor3 = stv.FaixaMenos3,
                            Minor2 = stv.FaixaMenos2,
                            Minor1 = stv.FaixaMenos1,
                            Mid = stv.FaixaMidPoint,
                            Plus1 = stv.FaixaMais1,
                            Plus2 = stv.FaixaMais2,
                            Plus3 = stv.FaixaMais3,
                            Plus4 = stv.FaixaMais4,
                            Plus5 = stv.FaixaMais5,
                            Plus6 = stv.FaixaMais6,
                        }).ToList(),
                }));

            if (salaryTable == null)
                return new GetEditTableValuesResponse();

            int countHeader = 1;
            var headers = new List<HeaderSalaryTableUpdate>();
            foreach (var item in salaryTableRanges)
            {
                headers.Add(new HeaderSalaryTableUpdate
                {
                    isMidPoint = item.SalaryRangeId == (int)SalarialRanges.MidPoint,
                    ColPos = countHeader,
                    ColName = $"{(Math.Round(item.AmplitudeMidPoint * 100)).ToString()}%",
                });
                countHeader++;
            }

            var retSalaryTable = new GetEditTableValuesResponse
            {
                SalaryTableValues = new SalaryTable
                {
                    GsmInitial = salaryTable != null &&
                                 salaryTable.SalaryTableValues != null &&
                                 salaryTable.SalaryTableValues.FirstOrDefault() != null ?
                                 salaryTable.SalaryTableValues.FirstOrDefault().GSM : 0,

                    GsmFinal = salaryTable != null &&
                               salaryTable.SalaryTableValues != null &&
                               salaryTable.SalaryTableValues.LastOrDefault() != null ?
                               salaryTable.SalaryTableValues.LastOrDefault().GSM : 0,
                               
                    SalarialTableName = salaryTable.SalarialTableName,
                    SalaryTableValues = salaryTable.SalaryTableValues
                },
                RangeEdit = salaryTableRanges.Select(res => res.SalaryRangeId).ToList(),
                Headers = headers
            };
            return retSalaryTable;
        }
    }
}
