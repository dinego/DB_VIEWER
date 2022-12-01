using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.TableSalary.Queries
{
    public class GetRangeSalaryGraphRequest : IRequest<GetRangeSalaryGraphResponse>
    {
        public long ProjectId { get; set; }
        public long TableId { get; set; }
        public long? GroupId { get; set; }
        public long? UnitId { get; set; }
        public long UserId { get; set; }
    }

    public class GetRangeSalaryGraphResponse
    {
        public RangeSalaryGraph Range { get; set; }
        public RangeSalaryGraph DefaultRange { get; set; }
    }

    public class RangeSalaryGraph
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class GetRangeSalaryGraphHandler : IRequestHandler<GetRangeSalaryGraphRequest, GetRangeSalaryGraphResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly ValidatorResponse _validatorResponse;

        public GetRangeSalaryGraphHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _validatorResponse = validatorResponse;
        }
        public async Task<GetRangeSalaryGraphResponse> Handle(GetRangeSalaryGraphRequest request,
            CancellationToken cancellationToken)
        {

            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);
            var groupsExp = permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var tablesExp = permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                        .SubItems;

            var groupsMapping = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                                      .GetListAsync(x => x.Where(gpm => gpm.ProjetoId == request.ProjectId &&
                                                                 gpm.TabelaSalarialIdLocal == request.TableId &&
                                                                 (!request.UnitId.HasValue || gpm.EmpresaId == request.UnitId.Value) &&
                                                                 !groupsExp.Safe().Contains(gpm.GrupoProjetoSmidLocal) &&
                                                                 (!request.GroupId.HasValue || gpm.GrupoProjetoSmidLocal == request.GroupId.Value))
                                                                .Select(res => res.GrupoProjetoSmidLocal));

            var mapPositionSM = await _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>()
                        .Include("CargosProjetosSm")
                        .Include("Empresa")
                        .GetListAsync(x => x.Where(cpm => cpm.ProjetoId == request.ProjectId &&
                        cpm.TabelaSalarialIdLocal == request.TableId &&
                        (!groupsExp.Safe().Contains(cpm.CargosProjetosSm.GrupoSmidLocal)) &&
                        (!request.GroupId.HasValue || cpm.CargosProjetosSm.GrupoSmidLocal == request.GroupId))
                        .Select(res => new
                        {
                            GSM = res.Gsminicial.GetValueOrDefault(),
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
                                                       .Select(res => res.Grade).Distinct());

            if (salaryTableValues == null)
                return null;

            var gsmByGroup = mapPositionSM.Where(mp => groupsMapping.Contains(mp.GroupId.GetValueOrDefault()) &&
                                                     (!request.UnitId.HasValue || mp.CompanyId == request.UnitId.Value) &&
                                                      mp.TableId == request.TableId)
                                              .Select(res => res.GSM).Distinct();
            int positionMin = 0;
            int positionMax = 0;

            if (gsmByGroup.Any())
            {
                positionMin = gsmByGroup.Safe().Min();
                positionMax = gsmByGroup.Safe().Max();
            }

            var result = new GetRangeSalaryGraphResponse
            {
                Range = new RangeSalaryGraph { Min = salaryTableValues.Min(), Max = salaryTableValues.Max() },
                DefaultRange = new RangeSalaryGraph { Min = positionMin, Max = positionMax }
            };
            return result;
        }
    }
}
