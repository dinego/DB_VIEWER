using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface ISaveEnabledLabelsByModulesInteractor
    {
        Task Handler(EnabledLabelsByModulesRequest request);
    }
}
