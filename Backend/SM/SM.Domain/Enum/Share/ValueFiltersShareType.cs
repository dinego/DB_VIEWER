using System.ComponentModel;

namespace SM.Domain.Enum.Share
{
    public enum ValueFiltersShareType
    {
        [Description("Id Tabela")]
        TableId = 1,
        [Description("Perfil Id")]
        GroupId = 2,
        [Description("Tipo de Contrato")]
        ContractType = 3,
        [Description("Base Horária")]
        HoursType = 4,
        [Description("Unidade Id")]
        UnitId = 5,
        [Description("Com/sem ocupantes")]
        ShowJustWithOccupants = 6,
        [Description("Filtrado por")]
        DisplayBy = 7,//check type enum
        //group scenario
        [Description("Cenário")]
        Scenario = 8,
        [Description("MM")]
        IsMM = 9,
        [Description("MI")]
        IsMI = 10,
    }
}
