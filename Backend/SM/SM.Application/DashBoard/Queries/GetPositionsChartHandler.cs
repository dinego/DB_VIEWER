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

    public class GetPositionsChartRequest : IRequest<GetPositionsChartResponse>
    {
        public long UserId { get; set; }
        public string PositionFilterId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public long ProjectId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? Unit { get; set; }
        public DisplayMMMIEnum Moviments { get; set; }
    }

    public class GetPositionsChartResponse
    {
        public string Name { get; set; }
        public double Percentage { get; set; }
        public double OccupantsPercentage { get; set; }
        public int AmountPositions { get; set; }
    }
    public class GetPositionsChartHandler
        : IRequestHandler<GetPositionsChartRequest, GetPositionsChartResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;

        public GetPositionsChartHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;

        }
        public async Task<GetPositionsChartResponse> Level(GetPositionsChartRequest request, List<long> result)
        {
            return null;
        }
        public async Task<GetPositionsChartResponse> Handle(GetPositionsChartRequest request, CancellationToken cancellationToken)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var levelsExp = permissionUser.Levels;
            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            //base
            //agrupar pelo filtro
            var result = await _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>() //123
                                             .Include("CargosProjetosSm.CmcodeNavigation") //aqui está o agrupamento
                                             .GetListAsync(x => x.Where(cp => cp.ProjetoId == request.ProjectId &&
                                                                              (request.Unit.HasValue || request.CompaniesId.Contains(cp.EmpresaId)) &&
                                                                              (!request.Unit.HasValue || cp.EmpresaId == request.Unit.Value) &&
                                                                              cp.CargosProjetosSm != null &&
                                                                              cp.CargosProjetosSm.Ativo.HasValue &&
                                                                              cp.CargosProjetosSm.Ativo.Value)
                                             .Select(res => res.CargoProjetoSmidLocal));

            int positions = result.Distinct().Count(); //qtd total

            //switch (request.DisplayBy)
            //{
            //    case DisplayByPositioningEnum.ProfileId:
            //        return Profile(request, result);

            //    case DisplayByPositioningEnum.LevelId:
            //        return await Level(request, result);

            //    //case DisplayByPositioningEnum.Area:
            //    //    return FitData(Area(request, result));

            //    //case DisplayByPositioningEnum.Parameter01:
            //    //    return FitData(Parameter01(request, result));

            //    //case DisplayByPositioningEnum.Parameter02:
            //    //    return FitData(Parameter02(request, result));

            //    //case DisplayByPositioningEnum.Parameter03:
            //    //    return FitData(Parameter03(request, result));

            //    //default:
            //    //    return FitData(await Profile(request, result));
            //}

            //return FitData(await Profile(request, result));

            // no select da query, pegaria o CARGO
            // trazer o Nível (baseado no CMCode que está em CargosProjetosSM),
            // trazer o Grupo (Perfil, que está dentro de CargosProjetosSM - Campo GrupoIdSmLocal)
            // trazer o Area (Area, que está dentro de CargosProjetosSM)

            // trazer o Parametro01 (Parametro01, que está dentro de CargosProjetosSM) ->> verificar com Flávio se o dado vai ser alterado no banco
            // trazer o Parametro02 (Parametro02, que está dentro de CargosProjetosSM)
            // trazer o Parametro03 (Parametro03, que está dentro de CargosProjetosSM)

            // Armazenar quantidade total de cargos - Fazer COUNT na lista em memoria e trazer somente o campo (.Select(res => res.CargoProjetoSmidLocal).Distinct())) CargoProjetoSmidLocal com Distinct
            // Criar Switch/Case (Default Perfil)
            // -> Nivel ->> Pegar a lista em memoria, pegar somente os NiveisIds e usar Distinct,
            // fazer um where ((!var2.HasValue || cp.CargosProjetosSm.CmcodeNavigation.NivelId == var2.Value) -> request vindo do front)


            // base salarial é a base para calcular o percentual de quantos ocupam o cargo no filtro
            var mmPositions = await _unitOfWork.GetRepository<BaseSalariais, long>() //90
                                .Include("CmcodeNavigation")
                                .GetListAsync(x => x.Where(bs =>
                                    (request.Moviments == DisplayMMMIEnum.MI ?
                                          result.Contains(bs.CargoIdSMMI.GetValueOrDefault(0))
                                        : result.Contains(bs.CargoIdSMMM.GetValueOrDefault(0))) &&
                                   (request.Unit.HasValue || request.CompaniesId.Contains(bs.EmpresaId.Value)) &&
                                   (!request.Unit.HasValue || bs.EmpresaId.Value == request.Unit.Value) &&
                                   (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId)) &&
                                   (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))?
                                  .Select(s => new
                                  {
                                      PositionId = request.Moviments == DisplayMMMIEnum.MI ? s.CargoIdSMMI : s.CargoIdSMMM
                                  }).Distinct());

            double salaryBasePositions = mmPositions.Count();

            return
                new GetPositionsChartResponse
                {
                    Name = "Cargos com ocupantes",
                    AmountPositions = positions,
                    Percentage = salaryBasePositions == 0 ? salaryBasePositions :
                    Math.Round((salaryBasePositions / positions) * 100, 0),
                    OccupantsPercentage = salaryBasePositions == 0 ? salaryBasePositions :
                    Math.Round((salaryBasePositions / positions) * 100, 0)
                };
        }
    }
}
