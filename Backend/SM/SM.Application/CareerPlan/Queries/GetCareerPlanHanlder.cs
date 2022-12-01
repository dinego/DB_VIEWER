using CMC.Common.Repositories;
using MediatR;
using SM.Application.TableSalary.Queries.Response;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Domain.Entities;
using System.Linq;
using SM.Application.CareerPlan.Queries.Response;

namespace SM.Application.CareerPlan.Queries
{
    public class CareerPlanRequest : IRequest<CareerPlanResponse>
    {
        public long ProjectId { get; set; }
        public long PositionId { get; set; }
    }

    public class GetCareerPlanHanlder : IRequestHandler<CareerPlanRequest, CareerPlanResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCareerPlanHanlder(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CareerPlanResponse> Handle(CareerPlanRequest request, CancellationToken cancellationToken)
        {
            // var position = await _unitOfWork.GetRepository<TrilhaSMMapeamento, long>()
            //                           .GetListAsync(x => x.Where(c => c.CargoProjetoSmIdLocal == request.PositionId &&
            //                                                      c.ProjetoId == request.ProjectId)
            //                                         .Select(res => new PositionSMDetails
            //                                         {
            //                                         }));


            return new CareerPlanResponse();
        }
    }
}
