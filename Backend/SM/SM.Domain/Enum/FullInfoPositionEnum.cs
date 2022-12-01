using System.ComponentModel;

namespace SM.Domain.Enum
{
    //>=100 and <1000 => common labels
    //>=1000 global labels
    //<100 local labels

    public enum FullInfoPositionEnum
    {
        [Description("Id")]
        LocalId = 100,
        [Description("Cargo Salary Mark")]
        PositionSalaryMark = 1,
        [Description("SMCode")]
        Smcode = 8,
        [Description("GSM")]
        GSM = 1000,
        [Description("Nível")]
        Level = 3,
        [Description("Eixo Carreira")]
        AxisCareer = 101,
        [Description("Base Horária")]
        HourBase = 5,
        [Description("Perfil")]
        Profile = 2,
        [Description("Parâmetro 02")]
        Parameter02 = 1004
    }
}
