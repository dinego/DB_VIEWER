using SM.Domain.Common;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface IGetPositionProjectSMAndSalaryTableInteractor
    {
        Task<IEnumerable<DataPositionProjectSMResponse>>
            Handler(IEnumerable<DataPositionProjectSMRequest> request, long projectId,
            PermissionJson permissionUser,
            DataBaseSalaryEnum dataBaseType,
            ContractTypeEnum contratcType);
    }
}
