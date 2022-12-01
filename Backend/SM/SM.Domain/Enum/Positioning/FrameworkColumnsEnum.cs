using System.ComponentModel;

namespace SM.Domain.Enum.Positioning
{
    public enum FrameworkColumnsMainEnum
    {
        [Description("Cargo Atual")]
        CurrentPosition = 1,
        [Description("Empresa")]
        Company = 2,
        [Description("Unidade")]
        UnitPlace = 3,
        [Description("Negócio")]
        Business = 4,
        [Description("Setor")]
        Setor = 5,
        [Description("Funcionário")]
        Employee = 6,
        [Description("Chefia Imediata")]
        ImmediateSupervisor = 7,
        [Description("Cargo Salary Mark")]
        PositionSm = 10,
        [Description("Perfil")]
        Profile = 11,
        [Description("Tipo Contrato")]
        TypeOfContract = 15,
        [Description("GSM")]
        GSM = 1000,
        [Description("Base Horária")]
        HourlyBasis = 8,
        [Description("Salário")]
        Salary = 9,
        [Description("Area da Porcentagem")]
        PercentagemArea = 13,
        [Description("Comparação MidPoint")]
        CompareMidPoint = 14
    }

    public enum FrameworkColumnsMainExcelEnum
    {
        [Description("Cargo Atual")]
        CurrentPosition = 1,
        [Description("Empresa")]
        Company = 2,
        [Description("Unidade")]
        UnitPlace = 3,
        [Description("Negócio")]
        Business = 4,
        [Description("Setor")]
        Setor = 5,
        [Description("Funcionário")]
        Employee = 6,
        [Description("Chefia Imediata")]
        ImmediateSupervisor = 7,
    }
    public enum FrameworkColumnsScenarioExcelEnum
    {
        [Description("Cargo Salary Mark")]
        PositionSm = 10,
        [Description("Perfil")]
        Profile = 11,
        [Description("Tipo Contrato")]
        TypeOfContract = 15,
        [Description("GSM")]
        GSM = 1000,
        [Description("Base Horária")]
        HourlyBasis = 8,
        [Description("Salário")]
        Salary = 9,
        [Description("Area da Porcentagem")]
        PercentagemArea = 13,
        [Description("Comparação MidPoint")]
        CompareMidPoint = 14,
    }

}
