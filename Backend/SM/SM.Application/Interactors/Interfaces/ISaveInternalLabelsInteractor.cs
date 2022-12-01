using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface ISaveInternalLabelsInteractor
    {
        Task Handler(InternalLabelsRequest request);
    }
}
