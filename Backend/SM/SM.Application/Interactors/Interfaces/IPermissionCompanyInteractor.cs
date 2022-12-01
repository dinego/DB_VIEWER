using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface IPermissionCompanyInteractor
    {
        Task<PermissionCompanyResponse> Handler(long projectId);
    }
}
