using System.Collections.Generic;

namespace SM.Application.PositionDetails.Queries.Response
{
    public class GetSalaryTablePositionDetailsResponse
    {
        public TableInfoPositionDetail Table { get; set; }
        public int NextPage { get; set; }

    }
    public class TableInfoPositionDetail
    {
        public IEnumerable<HeaderInfoPositionDetail> Header { get; set; }
        public List<List<DataBodyPositionDetail>> Body { get; set; }
    }
    public class HeaderInfoPositionDetail
    {
        public int ColPos { get; set; }
        public string ColName { get; set; }
        public string NickName { get; set; }
        public bool isMidPoint { get; set; } = false;
        public bool Sortable { get; set; } = false;
        public int? ColumnId { get; set; } = null;
    }
    public class DataBodyPositionDetail
    {
        public int ColPos { get; set; }
        public string Value { get; set; }
        public string @Type { get; set; }
        public bool isMidPoint { get; set; }
        public bool OccupantCLT { get; set; }
        public bool OccupantPJ { get; set; }
    }

    public class SalaryTablesPositionDetailsValues
    {
        public long ProjectPositionSMId { get; set; }
        public long LocalIdSalaryTable { get; set; }
        public string Table { get; set; }
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

    public class SalaryTableGroupPositionDetails
    {
        public long GroupId { get; set; }
        public long CompanyId { get; set; }
    }
    public class SalaryTablesPositionDetailsRanges
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
}
