using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum RowLevelType
    {
        [Description("Estratégico")]
        Strategic = 1,
        [Description("Tático")]
        Tactical = 2,
        [Description("Operacional")]
        Operational = 3,
    }
}
