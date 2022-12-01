using System.Collections.Generic;
using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface IGetLocalLabelsInteractor
    {
        Task<IEnumerable<LocalLabelsResponse>> Handler(LocalLabelsRequest request);
    }
}
