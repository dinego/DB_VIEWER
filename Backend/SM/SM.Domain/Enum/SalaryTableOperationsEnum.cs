using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum SalaryTableOperationsEnum
    {
        [Description("Aplicar Atualização (% ou R$)")]
        ApplyUpdate = 1,
        [Description("Editar faixas e valores indivualmente")]
        EditRangesAndValuesIndividually = 2,
        [Description("Importar Excel")]
        ImportExcel = 3
    }
}
