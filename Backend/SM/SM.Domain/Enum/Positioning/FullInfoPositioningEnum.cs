using System.ComponentModel;

namespace SM.Domain.Enum.Positioning
{
    public enum FullInfoPositioningEnum
    {
        [Description("Empresa")]
        Company = 1,
        [Description("Cargo Atual")]
        CurrentyPosition = 2,
        [Description("Cargos")]
        PositionSM = 3,
        [Description("Funcionário")]
        Employee = 4,
        [Description("Salário")]
        Salary = 5,
        [Description("Base Horária")]
        HoursBase = 8,
        [Description("GSM")]
        GSM = 1000,
        [Description("Porcentagens")]
        Percentages = 6,
        [Description("Comparação")]
        CompareValue = 7
    }

    public enum FullInfoProposedMovementEnum
    {
        [Description("Empresa")]
        Company = 1,
        [Description("Cargo Atual")]
        CurrentyPosition = 2,
        [Description("Cargos")]
        PositionSM = 3,
        [Description("Funcionário")]
        Employee = 4,
        [Description("Salário")]
        Salary = 5,
        [Description("Base Horária")]
        HoursBase = 8,
        [Description("GSM")]
        GSM = 1000,
        [Description("Porcentagens")]
        Percentages = 6,
        [Description("Movimento Proposto")]
        ProposedMovementLabel = 7
    }

    public enum FullInfoFinancialImpactEnum
    {
        [Description("Empresa")]
        Company = 1,
        [Description("Cargo Atual")]
        CurrentyPosition = 2,
        [Description("Cargos")]
        PositionSM = 3,
        [Description("Funcionário")]
        Employee = 4,
        [Description("Salário")]
        Salary = 5,
        [Description("Base Horária")]
        HoursBase = 8,
        [Description("GSM")]
        GSM = 1000,
        [Description("Porcentagens")]
        Percentages = 6,
        [Description("Impacto Financeiro")]
        FinancialImpact = 7
    }
}
