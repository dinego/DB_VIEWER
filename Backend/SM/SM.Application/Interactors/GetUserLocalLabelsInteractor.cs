using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using SM.Application.Interactors.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Application.Interactors
{
    public class GetUserLocalLabelsResponse
    {
        public int InternalCode { get; set; }
        public string Label { get; set; }
    }
    public class GetUserLocalLabelsInteractor : IGetUserLocalLabelsInteractor
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserLocalLabelsInteractor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GetUserLocalLabelsResponse>> Handler(long userId)
        {
            var userLabels = await _unitOfWork.GetRepository<UsuarioRotulosSM, long>()
                             .GetListAsync(x => x.Where(s => s.UsuarioId == userId)
                             .Select(res => new GetUserLocalLabelsResponse
                             {
                                 InternalCode = res.CodigoInternoRoluto,
                                 Label = res.NovoRotulo
                             }));

            return userLabels;
        }
    }
}
