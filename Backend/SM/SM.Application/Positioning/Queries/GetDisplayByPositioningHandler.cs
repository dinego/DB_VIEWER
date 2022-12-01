using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
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

    public class GetDisplayByPositioningRequest
        : IRequest<IEnumerable<GetDisplayByPositioningResponse>>
    {
        public long ProjectId { get; set; }
        public long UserId { get; set; }
    }

    public class GetDisplayByPositioningResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
    }

    public class DisplayByDTO
    {
        public long ProfileId { get; set; }
        public long LevelId { get; set; }
        public string Area { get; set; }
        public string Parameter01 { get; set; }
        public string Parameter02 { get; set; }
        public string Parameter03 { get; set; }
    }
    public class GetDisplayByPositioningHandler :
        IRequestHandler<GetDisplayByPositioningRequest, IEnumerable<GetDisplayByPositioningResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetDisplayByPositioningHandler(IUnitOfWork unitOfWork,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
        }

        public async Task<IEnumerable<GetDisplayByPositioningResponse>> Handle(GetDisplayByPositioningRequest request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.GetRepository<ParametrosProjetosSMLista, long>()
               .Include("ParametrosProjetosSMTipos")
               .GetListAsync(g => g.Where(x => x.Ativo && x.ProjetoId == request.ProjectId)?
               .Select(s => new
               {
                   ProfileId = s.ProjetoId,
                   LevelId = s.ParametroProjetoSMLista,
                   Parameter = s.ParametrosProjetosSMTipos.Parametro
               }));


            //var result = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
            //   .Include("CmcodeNavigation")
            //   .GetListAsync(g => g.Where(x => x.Ativo.HasValue &&
            //   x.Ativo.Value &&
            //   x.ProjetoId == request.ProjectId)?
            //   .Select(s => new 
            //   {
            //       ProfileId = s.ProjetoId,
            //       LevelId = s.CmcodeNavigation.NivelId,
            //       //Area = s.Area,
            //       //Parameter01 = s.Parametro1,
            //       //Parameter02 = s.Parametro2,
            //       //Parameter03 = s.Parametro3
            //   }));

            if (!result.Any())
                return new List<GetDisplayByPositioningResponse>();

            //configuration global labels 
            var configGlobalLabels =
                await _getGlobalLabelsInteractor.Handler(request.ProjectId);


            var listEnums = Enum.GetValues(typeof(DisplayByPositioningEnum))
                as IEnumerable<DisplayByPositioningEnum>;

            var buildList = new List<GetDisplayByPositioningResponse>();

            foreach (DisplayByPositioningEnum item in listEnums)
            {
                var hasItens = result.Any(a => a.Parameter == item.ToString());

                var globalLabel = configGlobalLabels.FirstOrDefault(f => (long)item == f.Id);

                if (hasItens && (globalLabel == null || globalLabel.IsChecked))
                    buildList.Add(new GetDisplayByPositioningResponse
                    {
                        Id = (long)item,
                        Title = globalLabel == null ? item.GetDescription() : globalLabel.Alias
                    });
            }

            return buildList;
        }
    }
}
