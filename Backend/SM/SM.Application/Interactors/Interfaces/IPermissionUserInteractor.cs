using SM.Domain.Common;
using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface IPermissionUserInteractor
    {
        Task<PermissionJson> Handler(long userId);
    }
}
