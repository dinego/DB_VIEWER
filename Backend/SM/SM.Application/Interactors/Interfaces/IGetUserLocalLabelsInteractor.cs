using System.Collections.Generic;
using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface IGetUserLocalLabelsInteractor
    {
        Task<IEnumerable<GetUserLocalLabelsResponse>> Handler(long userId);
    }
}
