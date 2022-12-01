using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum DataBaseSalaryEnum
    {
        [Description("Salário Mensal")]
        MonthSalary = 0,
        [Description("Salário Hora")]
        HourSalary = 1,
        [Description("Salário Anual")]
        YearSalary = 2,
    }
}
