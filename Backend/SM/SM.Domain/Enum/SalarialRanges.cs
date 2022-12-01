using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum SalarialRanges
    {
        [Description("Faixa-6")]
        RangeMinus6 = 1,
        [Description("Faixa-5")]
        RangeMinus5 = 2,
        [Description("Faixa-4")]
        RangeMinus4 = 3,
        [Description("Faixa-3")]
        RangeMinus3 = 4,
        [Description("Faixa-2")]
        RangeMinus2 = 5,
        [Description("Faixa-1")]
        RangeMinus1 = 6,
        [Description("MidPoint")]
        MidPoint = 7,
        [Description("Faixa+1")]
        RangePlus1 = 8,
        [Description("Faixa+2")]
        RangePlus2 = 9,
        [Description("Faixa+3")]
        RangePlus3 = 10,
        [Description("Faixa+4")]
        RangePlus4 = 11,
        [Description("Faixa+5")]
        RangePlus5 = 12,
        [Description("Faixa+6")]
        RangePlus6 = 13,
    }
}
