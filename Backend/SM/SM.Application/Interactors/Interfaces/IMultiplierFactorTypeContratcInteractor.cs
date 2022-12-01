using SM.Domain.Enum;
using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface IMultiplierFactorTypeContratcInteractor
    {
        Task<double?> Handler(long projectId, ContractTypeEnum contratcType = ContractTypeEnum.CLT);
    }
}
