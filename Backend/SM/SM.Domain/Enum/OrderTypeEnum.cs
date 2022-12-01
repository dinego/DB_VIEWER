using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum OrderTypeEnum
    {
        [Description("Date")]
        DataAsc = 1,
        [Description("Date")]
        DataDes = 2,
        [Description("Title")]
        TitleAsc = 3,
        [Description("Title")]
        TitleDes = 4,
    }
}
