using System.ComponentModel;

namespace SM.Domain.Enum.Positioning
{
    public enum DistribuitionAnalyseEnum
    {
        [Description("Acima da Política Salarial")]
        AboveSalaryPolicy = 3,
        [Description("Dentro da Política Salarial")]
        WithinTheSalaryPolicy = 2,
        [Description("Abaixo da Politica Salarial")]
        BelowSalaryPolicy = 1,
    }
}
