using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum ModulesEnum
    {
        None = 0,
        [Description("Home")]
        Home = 1,
        [Description("DashBoard")]
        DashBoard = 2,
        [Description("Tabela Salarial")]
        TableSalary = 3,
        [Description("Cargos")]
        Position = 4,
        [Description("Posicionamento")]
        Positioning = 5,
        [Description("Meus Relatórios")]
        Reports = 6,
        [Description("Parâmetros")]
        Parameters = 7
    }

    public enum ModulesSuItemsEnum
    {
        None = 0,
        [Description("Agende um Contato")]
        ScheduleContact = 1,
        [Description("Chat")]
        Chat = 2,
        [Description("Arquitetura")]
        Architecture = 3,
        [Description("Mapa")]
        Map = 4,
        [Description("Enquadramento")]
        Framework = 5,
        [Description("Impacto Financeiro")]
        FinancialImpact = 6,
        [Description("Análise Comparativa")]
        ComparativeAnalysis = 7,
        [Description("Análise de Distribuição")]
        DistributionAnalysis = 8,
        [Description("Movimentos Propostos")]
        ProposedMovements = 9,
        [Description("Níveis")]
        Levels = 10,
        [Description("Estratégia Salarial")]
        SalaryStrategy = 11,
        [Description("Base Horária")]
        HoursBase = 12,
        [Description("Configurações PJ")]
        SettingsPJ = 13,
        [Description("Usuários")]
        Users = 14,
        [Description("Rótulos Globais")]
        GlobalLabels = 15,
        [Description("Configurar Exibição")]
        DisplayConfiguration = 16,
        [Description("Lobbu")]
        Lobby = 17,
        [Description("Trilha de Cargo")]
        PositionTrack = 18
    }
}
