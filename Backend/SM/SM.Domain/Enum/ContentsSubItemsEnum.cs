using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum ContentsSubItemsEnum
    {
        [Description("Unidades")]
        Units = 1,
        [Description("Perfil")]
        Group = 2,
        [Description("Tabela Salarial")]
        SalaryTable = 3,
        [Description("Cargos")]
        Positions = 4,
        [Description("Mostrar pessoas sem cargos SalaryMark")]
        ShowPeopleWithoutPositions = 5,
        [Description("Exibir Ajuste Técnico")]
        ShowTecnicalAdjust = 6
    }
}