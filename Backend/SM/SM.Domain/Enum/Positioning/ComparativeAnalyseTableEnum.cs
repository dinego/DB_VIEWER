using System.ComponentModel;

namespace SM.Domain.Enum.Positioning
{
    public enum ComparativeAnalyseTableEnum
    {
        [Description("Filtro")]
        Filter = 1,
        [Description("Pessoas")]
        People = 2,
        [Description("Cargos")]
        Positions = 3,
        [Description("Geral")]
        General = 4,
        [Description("Eixo Carreira")]
        AxisCarrer = 5,

    }

    public enum ComparativeAnalyseTableExcelEnum
    {
        [Description("Filtro")]
        Filter = 1,
        [Description("Pessoas Quantidade")]
        PeopleAmount = 2,
        [Description("Pessoas Porcentagem")]
        PeoplePercentage = 3,
        [Description("Cargos Quantidade")]
        PositionsAmount = 4,
        [Description("Cargos Porcentagem")]
        PositionsPercentage = 5,
        [Description("Geral")]
        General = 6,
        [Description("Eixo Carreira")]
        AxisCarrer = 7,

    }
}
