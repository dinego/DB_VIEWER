using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Common.Queries
{
    public class GetAllCareerAxisRequest : IRequest<IEnumerable<GetAllCareerAxisResponse>>
    {
        public long ProjectId { get; set; }
        public List<long> Parameters { get; set; }
    }

    public class GetAllCareerAxisResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long? ParentParameterId { get; set; }
    }

    public class GetAllCareerAxisHandler : IRequestHandler<GetAllCareerAxisRequest, IEnumerable<GetAllCareerAxisResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllCareerAxisHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<GetAllCareerAxisResponse>> Handle(GetAllCareerAxisRequest request, CancellationToken cancellationToken)
        {
            var careerAxis = await _unitOfWork.GetRepository<EixoCarreiraSM, long>()
                                    .GetListAsync(x => x.Where(pp => pp.ProjetoId == request.ProjectId &&
                                                                request.Parameters.Contains(pp.ParametroProjetoSMListaId) &&
                                                                pp.Ativo)
                                               .Select(res => new GetAllCareerAxisResponse
                                               {
                                                   Id = res.Id,
                                                   Title = res.EixoCarreira,
                                                   ParentParameterId = res.ParametroProjetoSMListaId
                                               }).OrderBy(o => o.Title));
            if (careerAxis == null)
                return new List<GetAllCareerAxisResponse>();
            return careerAxis;
        }
    }
}