using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum.Positioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Positioning.Queries
{
    public class GetFilterPositionDisplayByResponse
    {
        public long? Id { get; set; }
        public string Title { get; set; }
    }

    public class GetFilterPositionDisplayByRequest :
    IRequest<List<GetFilterPositionDisplayByResponse>>
    {
        public long UserId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
    }

    public class GetFilterPositionDisplayByHandler
         : IRequestHandler<GetFilterPositionDisplayByRequest, List<GetFilterPositionDisplayByResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;

        public GetFilterPositionDisplayByHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
        }
        public async Task<List<GetFilterPositionDisplayByResponse>> Handle(GetFilterPositionDisplayByRequest request, CancellationToken cancellationToken)
        {

            if (request.DisplayBy == DisplayByPositioningEnum.ProfileId)
                return await GetProfile(request);
            if (request.DisplayBy == DisplayByPositioningEnum.LevelId)
                return await GetLevel(request);
            if (request.DisplayBy == DisplayByPositioningEnum.Area ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter01 ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter02 ||
                request.DisplayBy == DisplayByPositioningEnum.Parameter03)
                return await GetParameter(request);

            return await GetProfile(request);
        }

        private async Task<List<GetFilterPositionDisplayByResponse>> GetProfile(GetFilterPositionDisplayByRequest request)
        {
            var groupsProfiles = await _unitOfWork.GetRepository<GruposProjetosSalaryMark, long>()
                .GetListAsync(x => x.Where(g => request.ProjectId == g.ProjetoId)?.
                Select(s => new GetFilterPositionDisplayByResponse
                {
                    Id = s.GruposProjetosSmidLocal,
                    Title = s.GrupoSm
                }));

            groupsProfiles.Insert(0, new GetFilterPositionDisplayByResponse { Id = null, Title = "Todos" });

            return groupsProfiles;
        }
        private async Task<List<GetFilterPositionDisplayByResponse>> GetLevel(GetFilterPositionDisplayByRequest request)
        {

            var levels = await _unitOfWork.GetRepository<NivelEmpresas, long>()
                        .GetListAsync(x => x.Where(g => request.CompaniesId.Contains(g.EmpresaId) &&
                                    (!request.UnitId.HasValue || g.EmpresaId == request.UnitId.Value))?.
                        Select(s => new GetFilterPositionDisplayByResponse
                        {
                            Id = s.NivelId,
                            Title = s.NivelEmpresa
                        }));
            levels.Insert(0, new GetFilterPositionDisplayByResponse { Id = null, Title = "Todos" });

            var grouped = levels.GroupBy(g => g.Title, (key, value) => new { key, value }).ToList();

            var levelsUngrouped = new List<GetFilterPositionDisplayByResponse>();
            grouped.ForEach(filter =>
            {
                levelsUngrouped.Add(filter.value.FirstOrDefault());
            });

            return levelsUngrouped;
        }
        private async Task<List<GetFilterPositionDisplayByResponse>> GetArea(GetFilterPositionDisplayByRequest request)
        {
            var areas = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                        .GetListAsync(x => x.Where(g => g.GruposProjetosSalaryMark.Projeto.Id == request.ProjectId).
                        Select(s => new GetFilterPositionDisplayByResponse
                        {
                            Id = s.Id,
                            Title = ""//s.Area
                        }));
            areas.Insert(0, new GetFilterPositionDisplayByResponse { Id = null, Title = "Todos" });

            var areasGrouped = areas.GroupBy(g => g.Title, (key, value) => new { key, value }).ToList();

            var areassUngrouped = new List<GetFilterPositionDisplayByResponse>();
            areasGrouped.ForEach(filter =>
            {
                areassUngrouped.Add(filter.value.FirstOrDefault());
            });

            return areassUngrouped;

        }
        private async Task<List<GetFilterPositionDisplayByResponse>> GetParameter(GetFilterPositionDisplayByRequest request)
        {
            var parameters = await _unitOfWork.GetRepository<CargosProjetoSMParametrosMapeamento, long>()
                .Include("CargosProjetosSm")
                .Include("ParametrosProjetosSMLista.ParametrosProjetosSMTipos")
                .GetListAsync(x => x.Where(g => g.ProjetoId == request.ProjectId).
                Select(s => new GetFilterPositionDisplayByResponse
                {
                    Id = s.CargosProjetosSm.Id,
                    Title = s.ParametrosProjetosSMLista.ParametrosProjetosSMTipos.Parametro
                }));

            parameters.Insert(0, new GetFilterPositionDisplayByResponse { Id = null, Title = "Todos" });

            var paramGrouped = parameters.GroupBy(g => g.Title, (key, value) => new { key, value }).ToList();

            var paramUngrouped = new List<GetFilterPositionDisplayByResponse>();
            paramGrouped.ForEach(filter =>
            {
                paramUngrouped.Add(filter.value.FirstOrDefault());
            });

            return paramUngrouped;
        }
    }
}
