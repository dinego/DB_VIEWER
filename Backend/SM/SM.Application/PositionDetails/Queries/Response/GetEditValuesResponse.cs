using System.Collections.Generic;

namespace SM.Application.PositionDetails.Queries.Response
{

    public class GetEditTableValuesResponse
    {
        public SalaryTable SalaryTableValues { get; set; }
        public List<long> RangeEdit { get; internal set; }
        public List<HeaderSalaryTableUpdate> Headers { get; set; }
    }
    public class SalaryTableUpdate
    {
        public int GsmInitial { get; set; }
        public int GsmFinal { get; set; }
        public int? TypeMultiply { get; set; }
        public double? Multiply { get; set; }
    }

    public class SalaryTableValues
    {
        public int GSM { get; set; }
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
    public class SalaryTable
    {
        public string SalarialTableName { get; set; }
        public int GsmInitial { get; set; }
        public int GsmFinal { get; set; }
        public List<SalaryTableValues> SalaryTableValues { get; set; } = new List<SalaryTableValues>();
    }

    public class HeaderSalaryTableUpdate
    {
        public int ColPos { get; set; }
        public string ColName { get; set; }
        public bool isMidPoint { get; set; } = false;
    }
}