using System.ComponentModel;

namespace SM.Domain.Enum.Positioning
{
    //>=100 and <1000 => common labels
    //>=1000 global labels
    //<100 local labels
    public enum FrameworkFullInfoEnum
    {
        [Description("Salário Base Id")]
        SalaryBaseId = 100,
        [Description("Func Id")]
        EmployeeId = 101,
        [Description("Funcionário")]
        Employee = 6,
        [Description("Cargo Atual")]
        CurrentPosition = 1,
        [Description("Unidade")]
        UnitPlace = 3,
        [Description("Perfil")]
        Profile = 11,
        [Description("Base Horária")]
        HourlyBasis = 8,
        [Description("Salário")]
        Salary = 9,
        [Description("Nível")]
        Level = 10
    }

    public enum FrameworkFullInfoGSM
    {
        [Description("GSM")]
        GSM = 1000,
    }

}
