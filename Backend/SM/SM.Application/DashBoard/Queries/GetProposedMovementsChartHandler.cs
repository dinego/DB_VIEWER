using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.DashBoard.Queries
{
    public class GetProposedMovementsChartRequest
        : IRequest<GetProposedMovementsChartResponse>
    {
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
        public ProposedMovementsEnum ProposedMovements { get; set; } = ProposedMovementsEnum.WithoutProposedAdjustment;
    }

    public class GetProposedMovementsChartResponse
    {
        public double PercentageEmployees { get; set; } = 0;
        public string AmountEmployees { get; set; }

    }

    public class BaseSalaryProposedMovementsChartDTO
    {
        public int? InternalFrequency { get; set; }
        public ProposedMovementsEnum ProposedMovements { get; set; }
        public long? PositionIdDefault { get; set; }
        public string PositionSalaryBase { get; set; }
        public long PositionId { get; set; }
        public long ProfileId { get; set; }
        public string Area { get; set; }

    }

    public class PositionSalaryProposedMovementsDTO
    {
        public long PositionId { get; set; }
        public string PositionSM { get; set; }
        public long GroupId { get; set; }
        public string Area { get; set; }

    }
    public class GetProposedMovementsChartHandler
        : IRequestHandler<GetProposedMovementsChartRequest, GetProposedMovementsChartResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;

        public GetProposedMovementsChartHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
        }
        public async Task<GetProposedMovementsChartResponse> Handle(GetProposedMovementsChartRequest request, CancellationToken cancellationToken)
        {

            var result = await GetResultConsolidated(request);
            if (!result.Any())
                return new GetProposedMovementsChartResponse();

            if (!result.Any(a => a.ProposedMovements == request.ProposedMovements))
                return new GetProposedMovementsChartResponse();

            var employeeAmountTotal = result.Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0)));

            var employeeAmountMovement = result.Where(w => w.ProposedMovements == request.ProposedMovements)
                .Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0)));

            var percentageMovement =
                Math.Round(employeeAmountMovement / employeeAmountTotal * 100, 0);

            return new GetProposedMovementsChartResponse
            {
                AmountEmployees = (Math.Round(employeeAmountMovement, 0).ToString("n0")),
                PercentageEmployees = percentageMovement,
            };

        }

        private async Task<List<BaseSalaryProposedMovementsChartDTO>> GetResultConsolidated(GetProposedMovementsChartRequest request)
        {

            IEnumerable<BaseSalaryProposedMovementsChartDTO> result =
                await GetSalaryBaseXPositionSM(request);

            return result.ToList();
        }
        private async Task<IEnumerable<BaseSalaryProposedMovementsChartDTO>> GetSalaryBaseXPositionSM(GetProposedMovementsChartRequest request)
        {
            IEnumerable<BaseSalaryProposedMovementsChartDTO> salaryBaseList =
                        new List<BaseSalaryProposedMovementsChartDTO>();
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var groupsExp =
                        permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var areaExp = permissionUser.Areas;

            var levelsExp = permissionUser.Levels;
            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            if (request.Scenario == DisplayMMMIEnum.MM)
                salaryBaseList = await _unitOfWork.GetRepository<BaseSalariais, long>()
                    .Include("CmcodeNavigation")
                    .GetListAsync(x => x.Where(bs =>
                           bs.CargoIdSMMM.HasValue &&
                          (request.UnitId.HasValue || request.CompaniesId.Contains(bs.EmpresaId.Value)) &&
                          (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId)) &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active)
                          )?
                         .Select(s => new BaseSalaryProposedMovementsChartDTO
                         {
                             InternalFrequency = s.FrequenciaInterna,
                             PositionIdDefault = s.CargoIdSM,
                             PositionSalaryBase = s.CargoEmpresa,
                             PositionId = s.CargoIdSMMM.Value,
                         }));

            if (request.Scenario == DisplayMMMIEnum.MI)
                salaryBaseList = await _unitOfWork.GetRepository<BaseSalariais, long>()
                    .Include("CmcodeNavigation")
                    .GetListAsync(x => x.Where(bs =>
                            bs.CargoIdSMMI.HasValue &&
                          (request.UnitId.HasValue || request.CompaniesId.Contains(bs.EmpresaId.Value)) &&
                          (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId)) &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active)
                          )?
                         .Select(s => new BaseSalaryProposedMovementsChartDTO
                         {
                             InternalFrequency = s.FrequenciaInterna,
                             PositionIdDefault = s.CargoIdSM,
                             PositionSalaryBase = s.CargoEmpresa,
                             PositionId = s.CargoIdSMMI.Value,
                         }));

            if (!salaryBaseList.Any())
                return new List<BaseSalaryProposedMovementsChartDTO>();

            var positionsIds = salaryBaseList.Select(s => s.PositionId).Distinct();

            var positionsSM = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                .GetListAsync(g => g
                .Where(cp =>
                cp.Ativo.HasValue &&
                cp.Ativo.Value &&
                positionsIds.Contains(cp.CargoProjetoSmidLocal) && cp.ProjetoId == request.ProjectId)?.
                Select(s => new PositionSalaryProposedMovementsDTO
                {
                    PositionId = s.CargoProjetoSmidLocal,
                    //PositionSM = s.CargoSm,
                    GroupId = s.GrupoSmidLocal,
                    //Area = s.Area

                }));

            foreach (var salary in salaryBaseList)
            {
                var positionValues = positionsSM.FirstOrDefault(f => f.PositionId == salary.PositionId);

                salary.ProposedMovements = GetProposedMovementsAnalyse(salary.PositionId,
                salary.PositionIdDefault, /*positionValues.PositionSM,*/ salary.PositionSalaryBase);
                //salary.ProfileId = positionValues.GroupId;
                //salary.Area = positionValues.Area;
            }

            return salaryBaseList.Where(s => (!groupsExp.Safe().Any() || !groupsExp.Contains(s.ProfileId)) &&
                (!areaExp.Safe().Any()));// || !areaExp.Contains(s.Area)));

        }
        private ProposedMovementsEnum GetProposedMovementsAnalyse(long? positionId,
            long? positionDefaultId,
            //string positionSM,
            string positionSalaryBase)
        {
            if (!positionDefaultId.HasValue)
                return ProposedMovementsEnum.ChangeOfPosition;

            if (positionId.Value != positionDefaultId.Value)
                return ProposedMovementsEnum.ChangeOfPosition;

            //if (!positionSM.ToLower().Trim().Equals(positionSalaryBase.ToLower().Trim()))
            //    return ProposedMovementsEnum.AdequacyOfNomenclature;

            return ProposedMovementsEnum.WithoutProposedAdjustment;
        }
    }
}
