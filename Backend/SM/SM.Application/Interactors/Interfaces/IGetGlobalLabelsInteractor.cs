using SM.Domain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface IGetGlobalLabelsInteractor
    {
        Task<IEnumerable<GlobalLabelsJson>> Handler(long projectId);
    }
}
