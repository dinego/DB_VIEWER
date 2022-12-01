using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum ParametersProjectsTypes
    {
        [Description("Area")]
        Area = 1,
        [Description("Parâmetro 1")]
        ParameterOne = 2,
        [Description("Parâmetro 2")]
        ParameterTwo = 3,
        [Description("Parâmetro 3")]
        ParameterThree = 4,
        [Description("Eixo de Carreira")]
        CarreiraEixo = 5,

    }

    public enum ParameterDisplay
    {
        [Description("Tipo de Contrato")]
        TypeOfContract = 1001,
        [Description("Base Horária")]
        HourlyBasis = 1002,
        [Description("Cenários")]
        Scenario = 1003,
        [Description("Pesssoas")]
        Person = 1004,
        [Description("Análises")]
        Analysis = 1005,
        [Description("Ajuste Técnico")]
        TecnicalAdjust = 1006
    }
}
