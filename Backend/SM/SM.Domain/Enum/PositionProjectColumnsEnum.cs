using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum PositionProjectColumnsEnum
    {
        [Description("Cargo Salary Mark")]
        PositionSalaryMark = 2003,
        [Description("Unidade")]
        Company = 2008,
        [Description("Tabela")]
        Table = 3,
        [Description("Perfil")]
        Profile = 2002,
        [Description("Nível")]
        Level = 999,
        [Description("Área")]
        Area = 2004,
        [Description("Eixo Carreira")]
        CareerAxis = 1007,
        [Description("Parâmetro 01")]
        Parameter01 = 2005,
        [Description("Parâmetro 02")]
        Parameter02 = 2006,
        [Description("Parâmetro 03")]
        Parameter03 = 2007,
        [Description("GSM")]
        GSM = 2001,
        [Description("Base Horária")]
        HourBase = 5,
        [Description("SMCode")]
        Smcode = 8,
        [Description("Ajuste Técnico")]
        TechnicalAdjustment = 7
    }
}
