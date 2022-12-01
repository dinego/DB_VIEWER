using MediatR;
using SM.Application.Interactors.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.DashBoard.Queries
{
    public class GetProposedMovementsTypesRequest
        : IRequest<IEnumerable<GetProposedMovementsTypesResponse>>
    {
        public long UserId { get; set; }
    }

    public class GetProposedMovementsTypesResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool IsDefault { get; set; }
    }
    public class GetProposedMovementsTypesHandler :
        IRequestHandler<GetProposedMovementsTypesRequest, IEnumerable<GetProposedMovementsTypesResponse>>
    {
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        public GetProposedMovementsTypesHandler(IPermissionUserInteractor permissionUserInteractor)
        {
            _permissionUserInteractor = permissionUserInteractor;
        }
        public async Task<IEnumerable<GetProposedMovementsTypesResponse>> Handle(GetProposedMovementsTypesRequest request, CancellationToken cancellationToken)
        {
            var permissionResult = await
                _permissionUserInteractor.Handler(request.UserId);

            return permissionResult.Display.Scenario.Where(sc => sc.IsChecked)
                .Select(res => new GetProposedMovementsTypesResponse
                {
                    Id = res.Id,
                    Title = res.Name,
                    IsDefault = res.IsDefault
                });
        }
    }
}
