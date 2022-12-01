using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;

namespace SM.Application.Position.Queries
{
    public class GetAllDisplayByRequest : IRequest<IEnumerable<GetAllDisplayByResponse>>
    {
        public long ProjectId { get; set; }
    }

    public class GetAllDisplayByResponse
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class DisplayByDTO
    {
        public string AxisCarreira { get; set; }
        public string Area { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public string Parameter3 { get; set; }

    }


    public class GetAllDisplayByHandler : IRequestHandler<GetAllDisplayByRequest,IEnumerable<GetAllDisplayByResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetAllDisplayByHandler(IUnitOfWork unitOfWork,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
        }
        public async Task<IEnumerable<GetAllDisplayByResponse>> Handle(GetAllDisplayByRequest request, CancellationToken cancellationToken)
        {

            //configuration global labels 
            var configGlobalLabels =
                await _getGlobalLabelsInteractor.Handler(request.ProjectId);

            var result = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                .GetListAsync(g => g.Where(x => x.Ativo.HasValue &&
                x.Ativo.Value &&
                x.ProjetoId == request.ProjectId)?
                .Select(s => new DisplayByDTO
                {
                    //AxisCarreira = s.EixoCarreira,  
                    //Area = s.EixoCarreira,
                    //Parameter3 = s.Parametro3,
                    //Parameter1 = s.Parametro1,
                    //Parameter2 = s.Parametro2
                }));

            if (!result.Any())
                return new List<GetAllDisplayByResponse>();

            var buildList = new List<GetAllDisplayByResponse>();

            //AxisCarreira
            var hasAxisCarreira = result.Any(a => !string.IsNullOrWhiteSpace(a.AxisCarreira));
            if (hasAxisCarreira)

                buildList.Add(new GetAllDisplayByResponse
                {
                    Id = (int)DisplayByMapPositionEnum.AxisCarreira,
                    Name = DisplayByMapPositionEnum.AxisCarreira.GetDescription(),
                });

            //Area
            var hasArea = result.Any(a => !string.IsNullOrWhiteSpace(a.Area));
            var areaGlobalLabel =
                        configGlobalLabels.FirstOrDefault(f => f.Id == (long)DisplayByMapPositionEnum.Area);

            if (hasArea && areaGlobalLabel.IsChecked)
            {
                    buildList.Add(new GetAllDisplayByResponse
                    {
                        Id = (int)DisplayByMapPositionEnum.Area,
                        Name = areaGlobalLabel.Alias,
                    });
            }

            var hasParameter1 = result.Any(a => !string.IsNullOrWhiteSpace(a.Parameter1));
            var parameter1GlobalLabel =
                            configGlobalLabels.FirstOrDefault(f => f.Id ==
                            (long)DisplayByMapPositionEnum.Parameter01);

            if (hasParameter1 && parameter1GlobalLabel.IsChecked)
            {
                    buildList.Add(new GetAllDisplayByResponse
                    {
                        Id = (int)DisplayByMapPositionEnum.Parameter01,
                        Name = parameter1GlobalLabel.Alias,
                    });
            }

            var hasParameter2 = result.Any(a => !string.IsNullOrWhiteSpace(a.Parameter2));

            var parameter2GlobalLabel =
                configGlobalLabels.FirstOrDefault(f => f.Id ==
                (long)DisplayByMapPositionEnum.Parameter02);

            if (hasParameter2 && parameter2GlobalLabel.IsChecked)
            {

                    buildList.Add(new GetAllDisplayByResponse
                    {
                        Id = (int)DisplayByMapPositionEnum.Parameter02,
                        Name = parameter2GlobalLabel.Alias,
                    });
            }

            var hasParameter3 = result.Any(a => !string.IsNullOrWhiteSpace(a.Parameter3));
            var parameter3GlobalLabel =
                        configGlobalLabels.FirstOrDefault(f => f.Id ==
                        (long)DisplayByMapPositionEnum.Parameter03);

            if (hasParameter3 && parameter3GlobalLabel.IsChecked)
            {
                    buildList.Add(new GetAllDisplayByResponse
                    {
                        Id = (int)DisplayByMapPositionEnum.Parameter03,
                        Name = parameter3GlobalLabel.Alias,
                    });
            }

            return buildList;

        }
    }
}
