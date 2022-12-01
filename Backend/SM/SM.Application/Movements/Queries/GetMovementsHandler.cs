using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Movements.Queries
{
    public class GetMovementsRequest : IRequest<IEnumerable<GetMovementsResponse>>
    {
        public long UserId { get; set; }
        public IEnumerable<long> Companies { get; set; }
    }

    public class GetMovementsResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool IsDefault { get; set; }
    }
    public class GetMovementsHandler :
        IRequestHandler<GetMovementsRequest, IEnumerable<GetMovementsResponse>>
    {
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IUnitOfWork _unitOfWork;

        public GetMovementsHandler(IPermissionUserInteractor permissionUserInteractor, IUnitOfWork unitOfWork)
        {
            _permissionUserInteractor = permissionUserInteractor;
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<GetMovementsResponse>> Handle(GetMovementsRequest request, CancellationToken cancellationToken)
        {
            var permissionResult = await
                _permissionUserInteractor.Handler(request.UserId);

            if (permissionResult == null || permissionResult.Display == null) return null;

            var movements = permissionResult.Display.Scenario
                            .Where(sc => sc.IsChecked)
                            .Select(res => new GetMovementsResponse
                            {
                                Id = res.Id,
                                Title = res.Name,
                                IsDefault = res.IsDefault
                            });

            return movements;
        }
    }
}
