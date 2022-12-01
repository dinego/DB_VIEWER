using System.Collections.Generic;

namespace SM.Api.ViewModel
{
    public class UpdateDisplayColumnsMapViewModel
    {
        public IEnumerable<DisplayColumnsMapViewModel> DisplayColumns { get; set; }
    }

    public class DisplayColumnsMapViewModel
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }

    public class UpdateDisplayColumnsListViewModel
    {
        public IEnumerable<DisplayColumnsViewModel> DisplayColumns { get; set; }
    }
    public class UpdateDisplayColumnsFrameworkViewModel
    {
        public long UserId { get; set; }
        public IEnumerable<DisplayColumnsViewModel> DisplayColumns { get; set; }
    }

    public class UpdateSalaryTableViewModel
    {
        public SalaryTableViewModel SalaryTable { get; set; }
        public long ProjectId { get; set; }
        public long TableId { get; set; }
    }
    public class SalaryTableViewModel
    {

        public string SalaryTableName { get; set; }
        public int GsmInitial { get; set; }
        public int GsmFinal { get; set; }
        public string Justify { get; set; }
        public double? Multiply { get; set; }
        public int? TypeMultiply { get; set; }
        public List<SalaryTableValuesViewModel> SalaryTableValues { get; set; }
    }


    public class SalaryTableInfoViewModel
    {
        public long ProjectId { get; set; }
        public long TableId { get; set; }
        public string SalaryTableName { get; set; }
        public int GsmInitial { get; set; }
        public int GsmFinal { get; set; }
        public string Justify { get; set; }
        public double? Multiply { get; set; }
        public int? TypeMultiply { get; set; }
    }



    public class SalaryTableValuesViewModel
    {
        public int Gsm { get; set; }
        public long SalaryTableLocalId { get; set; }
        public double? Minor6 { get; set; }
        public double? Minor5 { get; set; }
        public double? Minor4 { get; set; }
        public double? Minor3 { get; set; }
        public double? Minor2 { get; set; }
        public double? Minor1 { get; set; }
        public double Mid { get; set; }
        public double? Plus1 { get; set; }
        public double? Plus2 { get; set; }
        public double? Plus3 { get; set; }
        public double? Plus4 { get; set; }
        public double? Plus5 { get; set; }
        public double? Plus6 { get; set; }
    }

    public class UpdateDisplayColumnsSalaryTableViewModel
    {
        public IEnumerable<DisplayColumnsViewModel> DisplayColumns { get; set; }
    }

    public class DisplayColumnsViewModel
    {
        public int ColumnId { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
