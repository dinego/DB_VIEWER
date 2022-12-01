using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum ReportTypeFileEnum
    {
        [Description("Relatórios")]
        Reports = 1
    }

    public enum ReportType
    {
        [Description(".pdf, .xls, .xlsx")]
        File = 1,
        [Description("Embutido")]
        Embed = 2,
    }
}
