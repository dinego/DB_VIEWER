using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum TypeVariableEnum
    {
        [Description("String")]
        String = 1,
        [Description("Int32")]
        Int32 = 2,
        [Description("Int64")]
        Int64 = 3,
        [Description("Double")]
        Double = 4,
        [Description("DateTime")]
        DateTIme = 5,
        [Description("Boolena")]
        Boolean = 6,
        [Description("NotDetermined")]
        NotDetermined =7
    }
}
