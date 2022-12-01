using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum InternalLabelsEnum
    {
        [Description("GSM")]
        GSM = 2001,
        [Description("Perfil")]
        Profile = 2002,
        [Description("Cargo Salary Mark")]
        PositionSalaryMark = 2003,
        [Description("Área")]
        Area = 2004,
        [Description("Parâmetro 1")]
        ParameterOne = 2005,
        [Description("Parâmetro 2")]
        ParameterTwo = 2006,
        [Description("Parâmetro 3")]
        ParameterThree = 2007,
        [Description("Unidade")]
        Unit = 2008,
        [Description("Negócio")]
        Business = 2008,
        [Description("Setor")]
        Sector = 2009,
    }

    public enum GlobalLabelEnum
    {
        PositionSalaryMark = 998,
        Level = 999,
        GSM = 1000,
        Area = 1001,
        Parameter1 = 1003,
        Parameter2 = 1004,
        Parameter3 = 1005,
        Profile = 1006,
        CareerAxis = 1007,
    }
}
