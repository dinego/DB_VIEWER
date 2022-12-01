using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum ContractTypeEnum
    {
        [Description("CLT")]
        CLT = 1,
        [Description("CLT - Prazo Determinado")]
        CltDeadline = 2,
        [Description("CLT Flex")]
        CltFlex = 3,
        [Description("CLT - Intermitente")]
        CltIntermittent = 4,
        [Description("Aprendiz")]
        Apprentice = 5,
        [Description("Aposentado")]
        Retired = 6,
        [Description("Conselheiro")]
        Advisor = 7,
        [Description("Estagiário")]
        Intern = 8,
        [Description("Estatutário")]
        Statutory = 9,
        [Description("Expatriado")]
        Expatriate = 10,
        [Description("PJ")]
        PJ = 11,
        [Description("PJ Executivo")]
        ExecutivePJ = 12,
        [Description("Ignorado")]
        Ignored = 13
    }
}
