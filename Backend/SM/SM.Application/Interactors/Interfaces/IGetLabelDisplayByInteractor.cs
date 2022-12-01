using SM.Domain.Enum.Positioning;
using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface IGetLabelDisplayByInteractor
    {
        Task<string> Handler(object category, long projectId, DisplayByPositioningEnum displayBy);
    }
}
