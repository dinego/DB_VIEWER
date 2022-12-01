using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum PermissionItensEnum
    {
        [Description("Excel")]
        Excel = 1,
        [Description("Renomear Colunas")]
        RenameColumn = 2,
        [Description("Editar Parâmetros")]
        Parameter = 3,
        [Description("Compartilhar")]
        Share = 5,
        [Description("Pessoas Inativas")]
        InactivePerson = 6,
        [Description("Movimento para próximo step")]
        MoveNextStep = 7,
        [Description("Gestão Cargo")]
        ManagementPosition = 8,
        [Description("Gestão Pessoas")]
        ManagementPeople = 9
    }

    public enum PermissionSubItemEnum
    {
        [Description("Editar Níveis")]
        EditLevel = 1,
        [Description("Editar Estratégia Salarial")]
        EditSalaryStrategic = 2,
        [Description("Editar Base Horária")]
        EditHourlyBasis = 3,
        [Description("Editar Configurações PJ")]
        EditConfigPj = 4,
        [Description("Editar Usuários")]
        EditUsers = 5,
        [Description("Editar Valores de Tabela Salarial")]
        EditSalaryTables = 8,
        [Description("Escolher Parâmetro Default")]
        EditDefaultParameter = 9
    }

    public enum FieldValueDefault
    {
        [Description("MM - Movimento Moderado")]
        MM = 1,
        [Description("MI - Movimento Ideal")]
        MI = 2,
        [Description("Cargo Salary Mark")]
        PositionSalaryMark = 3,
        [Description("Nomes")]
        Names = 4,
    }
    public enum ContentsSubItens
    {
        [Description("Unidades")]
        Units = 1,
        [Description("Grupo Hierarquico")]
        Group = 2,
        [Description("Tabela Salarial")]
        SalaryResult = 3,
        [Description("Cargos")]
        Positions = 4
    }

    public enum RenameColumnSubItemEnum
    {
        [Description("Rótulos Globais")]
        EditGlobalLabels = 6,
        [Description("Rótulos Locais")]
        EditLocalLabels = 7,
    }

    public enum NamePermissionEnum
    {
        [Description("Exibir Nome Funcionários")]
        DisplayEmployeeName = 8,
        [Description("Exibir Nome Chefia")]
        DisplayBossName = 9,
    }

    public enum ManagementPositionEnum
    {
        [Description("Adicionar Cargos")]
        AddPosition = 10,
        [Description("ExcluirCargos")]
        DeletePosition = 11,
        [Description("Editar Cargos")]
        EditPosition = 12,
        [Description("Editar Lista de Características dos Cargos")]
        EditListPosition = 13,
        [Description("Editar GSM e Mapeamento entre Tabelas")]
        EditGSMMappingTable = 14,
        [Description("Editar Valores da Tabela Salarial")]
        EditSalaryTableValues = 15
    }

    public enum ManagementPeopleEnum
    {
        [Description("Adicionar Pessoas")]
        AddPeople = 16,
        [Description("Excluir Pessoas")]
        DeletePeople = 17,
        [Description("Editar Pessoas")]
        EditPeople = 18,
        [Description("Editar Mapeamento Cargo Salary Mark")]
        EditMappingPositionSM = 19
    }
}
