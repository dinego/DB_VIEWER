using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum SalaryStrategyColumnEnum
    {
        [Description("Empresa")]
        Company = 1,
        [Description("Perfil")]
        Group = 2,
        [Description("Painel")]
        Panel = 3,
        [Description("Referência Mediana (em %)")]
        ReferenceMedian = 4,
        [Description("Porcentagem")]
        Percentagens = 5
    }
}
