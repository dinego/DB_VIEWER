using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum ProductTypesEnum
    {
        [Description("Salary Consultation")]
        CS = 1,
        [Description("Salary Mark")]
        SM = 2,
        [Description("All")]
        All = 3,
    }
}