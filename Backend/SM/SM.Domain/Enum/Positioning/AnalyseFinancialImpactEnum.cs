using System.ComponentModel;

namespace SM.Domain.Enum.Positioning
{
    public enum AnalyseFinancialImpactEnum
    {
        [Description("Necessário para Min.")]
        IFAMin = 1,
        //NecessaryForTheMinimium = 1,
        [Description("Movimento para Próximo Step")]
        IFPS = 2,
        //MovementToTheNextStep = 2,
        [Description("Movimento Todos para 100% da Tabela")]
        IFMP = 3,
        //MovementForAll100OfTheTable = 3,
        [Description("Excedente Acima Máx.")]
        IFAMax = 4,
        //SurplusAboveMaximum = 4,
    }
}
