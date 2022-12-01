using System.Collections.Generic;

namespace SM.Application.TableSalary.Queries.Response
{
    public class GetSalaryTableResponse
    {
        public TableInfo Table { get; set; }
        public int NextPage { get; set; }

    }
    public class TableInfo
    {
        public IEnumerable<HeaderInfo> Header { get; set; }
        public List<List<DataBody>> Body { get; set; }
    }
    public class HeaderInfo
    {
        public int ColPos { get; set; }
        public string ColName { get; set; }
        public string NickName { get; set; }
        public bool Disabled { get; set; } = true;
        public bool Editable { get; set; } = true;
        public bool IsChecked { get; set; } = true;
        public bool isMidPoint { get; set; } = false;
        public bool Sortable { get; set; } = false;
        public bool Visible { get; set; } = true;
        public int? ColumnId { get; set; } = null;
        public string SortClass { get; set; }
        public bool IsDesc { get; set; }
    }
    public class DataBody
    {
        public int ColPos { get; set; }
        public string Value { get; set; }
        public string @Type { get; set; }
        public bool IsChecked { get; set; }
        public bool isMidPoint { get; set; }
        public bool Expandend { get; set; }
        public long TableRangeId { get; set; }
    }

    public class SalaryTablesValues
    {
        public long LocalIdSalaryTable { get; set; }
        public int GSM { get; set; }
        public string TableSalaryName { get; set; }
        public double? RangeMinus6 { get; set; }
        public double? RangeMinus5 { get; set; }
        public double? RangePlus6 { get; set; }
        public double? RangePlus5 { get; set; }
        public double? RangePlus4 { get; set; }
        public double? RangePlus3 { get; set; }
        public double? RangePlus2 { get; set; }
        public double? RangePlus1 { get; set; }
        public double? RangeMidPoint { get; set; }
        public double? RangeMinus1 { get; set; }
        public double? RangeMinus4 { get; set; }
        public double? RangeMinus3 { get; set; }
        public double? RangeMinus2 { get; set; }
        public string Company { get; set; }
        public long CompanyId { get; set; }
    }

    public class SalaryTableGroup
    {
        public long GroupId { get; set; }
        public long CompanyId { get; set; }
    }
    public class SalaryTablesRanges
    {
        public long Id { get; set; }
        public long SalaryRangeId { get; set; }
        public long LocalIdSalaryTable { get; set; }
        public double AmplitudeMidPoint { get; set; }
        public double? Value { get; set; }
    }
    public class SalaryMarkMappingProjectsGroups
    {
        public long GroupId { get; set; }
        public string Group { get; set; }
        public long TableId { get; set; }
        public long CompanyId { get; set; }
        public long RangeUp { get; set; }
        public long RangeDown { get; set; }
        public string Company { get; set; }
    }

    public class GsmByTable
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}