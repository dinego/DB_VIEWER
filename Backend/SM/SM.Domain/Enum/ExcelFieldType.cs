using System.ComponentModel;

namespace SM.Domain.Enum
{

    public enum ExcelFieldType
    {
        [Description("#,##0")]
        NumberSimples = 1,
        //[Description("R$ #,##0")]
        [Description("#,##0")]
        Money = 2,
        [Description("0%")]
        Percentagem = 3,
        [Description("dd/MM/yyyy")]
        Date = 4,
        [Description("@")]
        Default = 5,
    }
}
