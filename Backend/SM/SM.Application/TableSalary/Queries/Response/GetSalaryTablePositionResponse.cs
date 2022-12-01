using System;
using System.Collections.Generic;

namespace SM.Application.TableSalary.Queries.Response
{
    public class GetSalaryTablePositionResponse
    {
        public TablePositionInfo Table { get; set; }
        public int NextPage { get; set; }
    }

    public class TablePositionInfo
    {
        public IEnumerable<HeaderPositionInfo> Header { get; set; }
        public List<List<DataPositionBody>> Body { get; set; }
    }
    public class HeaderPositionInfo
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
        public string SortClass { get; set; } //used only front
        public bool IsDesc { get; set; } //used only front
    }
    public class DataPositionBody
    {
        public int Row { get; set; }
        public double GSM { get; set; }
        public int ColPos { get; set; }
        public int? RowSpan { get; set; }
        public string Value { get; set; }
        public string @Type { get; set; }
        public bool IsChecked { get; set; } //used only front
        public bool isMidPoint { get; set; }
        public bool Expandend { get; set; } //used only front
        public string ActiveRow { get; set; } //used only front
        public long TableRangeId { get; set; }
        public bool OccupantCLT { get; set; }
        public bool OccupantPJ { get; set; }
        public long? PositionId { get; set; }
        public IEnumerable<string> OccupantPositions { get; set; }
    }

    public class SalaryTablesPositionValues
    {
        public long Id { get; set; }
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
        public long GroupId { get; set; }
        public long CompanyId { get; set; }
    }
    public class PositionByTable
    {
        public string Position { get; set; }
        public string Profile { get; set; }
    }
    public class CompanyPosition
    {
        public long TableId { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
    public class SalaryTablesPositionRanges
    {
        public long Id { get; set; }
        public long SalaryRangeId { get; set; }
        public long LocalIdSalaryTable { get; set; }
        public double AmplitudeMidPoint { get; set; }
        public double? Value { get; set; }
    }
    public class SalaryMarkMappingProjectsPositionGroups
    {
        public long GroupId { get; set; }
        public string Group { get; set; }
        public long TableId { get; set; }
        public long CompanyId { get; set; }
        public string Company { get; set; }
        public long RangeUp { get; set; }
        public long RangeDown { get; set; }
        public long? ProjectPositionSMId { get; set; }
    }

    public class ProjectPositionSM
    {
        public long ProjectPositionSMId { get; set; }
        public long GroupId { get; set; }
        public string Position { get; set; }
        public int GSM { get; set; }
        public long CompanyId { get; set; }
        public string Company { get; set; }
    }
}