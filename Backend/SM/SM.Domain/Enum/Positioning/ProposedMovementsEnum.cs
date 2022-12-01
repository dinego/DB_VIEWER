using System.ComponentModel;

namespace SM.Domain.Enum.Positioning
{
    public enum ProposedMovementsEnum
    {
        [Description("Adequação da Nomenclatura")]
        AdequacyOfNomenclature = 1,
        [Description("Alteração Cargo")]
        ChangeOfPosition = 2,
        [Description("Sem Ajuste Proposto")]
        WithoutProposedAdjustment = 3,
    }
}
