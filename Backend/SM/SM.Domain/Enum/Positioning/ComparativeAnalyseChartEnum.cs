using System.ComponentModel;

namespace SM.Domain.Enum.Positioning
{
    public enum ComparativeAnalyseChartEnum
    {
        [Description("MidPoint a Máximo")]
        MidPointToMaximum = 1,
        [Description("Mínimo a Midpoint")]
        MidpointToMinimum = 2,
        [Description("Pessoas Frente MidPoint")]
        //Comparação MidPoint
        PeopleFrontMidPoint = 3,
    }
}
