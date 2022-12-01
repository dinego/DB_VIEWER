using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Common.Queries
{
    public class GetAllLevelsRequest : IRequest<IEnumerable<GetAllLevelsResponse>>
    {
        public long CompanyId { get; set; }
    }

    public class GetAllLevelsResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
    }

    public class GetAllLevelsHandler : IRequestHandler<GetAllLevelsRequest, IEnumerable<GetAllLevelsResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllLevelsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<GetAllLevelsResponse>> Handle(GetAllLevelsRequest request, CancellationToken cancellationToken)
        {
            var levelsCompany = await _unitOfWork.GetRepository<NivelEmpresas, long>()
                                    .GetListAsync(x => x.Where(ne => ne.EmpresaId == request.CompanyId)
                                               .Select(res => new GetAllLevelsResponse
                                               {
                                                   Id = res.NivelId,
                                                   Title = res.NivelEmpresa
                                               }).OrderBy(o => o.Id));
            if (levelsCompany == null)
                return new List<GetAllLevelsResponse>();
            return levelsCompany;
        }
    }
}