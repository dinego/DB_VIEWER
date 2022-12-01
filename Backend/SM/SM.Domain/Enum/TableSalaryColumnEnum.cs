using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum TableSalaryColumnEnum
    {
        [Description("GSM")]
        GSM = 2001,
        [Description("Tabela")]
        TableSalaryName = 2,
        [Description("Unidade")]
        Company = 2008
    }
    public enum TableSalaryColumnPositionEnum
    {
        [Description("GSM")]
        GSM = 2001,
        [Description("Cargo SalaryMark")]
        PositionSM = 2003,
        [Description("Perfil")]
        Profile = 2002,
        [Description("Unidade")]
        Company = 2008
    }

    public enum TableSalaryViewEnum
    {
        SalaryTable = 1,
        PositionSalaryTable = 2,
        GraphSalaryTable = 3
    }
}
