using SM.Domain.Enum;
using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface IMultiplierFactorHourlyInteractor
    {
        Task<double> Handler(long projectId, DataBaseSalaryEnum dataBaseType = DataBaseSalaryEnum.MonthSalary, bool isPosition = true);
    }
}
