using System;
using System.Collections.Generic;
using SM.Application.Share.Queries;

namespace SM.Application.Position.Queries.Response
{
    public class GetAllPositionsResponse
    {
        public TableInfoPosition Table { get; set; }
        public int NextPage { get; set; }
        public ShareAllPositionsResponse Share { get; set; }
    }

    public class ShareAllPositionsResponse
    {
        public string User { get; set; } = null;
        public DateTime? Date { get; set; } = null;
        public string Table { get; set; }
        public long? TableId { get; set; }
        public string ContractType { get; set; }
        public int ContractTypeId { get; set; }
        public string HoursType { get; set; }
        public int HoursTypeId { get; set; }
        public string Unit { get; set; }
        public long? UnitId { get; set; }
        public string WithOccupants { get; set; }
        public bool IsWithOccupants { get; set; }
        public string Scenario { get; set; }
        public int ScenarioId { get; set; }
        public PermissionShared Permissions { get; set; }
    }

    public class TableInfoPosition
    {
        public IEnumerable<HeaderInfoPosition> Header { get; set; }
        public List<List<DataBodyPosition>> Body { get; set; }
    }

    public class HeaderInfoPosition
    {
        public int ColPos { get; set; }
        public string ColName { get; set; }
        public string NickName { get; set; }
        public bool Disabled { get; set; } = false;
        public bool Editable { get; set; } = true;
        public bool IsChecked { get; set; } = true;
        public bool Visible { get; set; } = true;
        public double ColumnId { get; set; }
        public bool Sortable { get; set; } = false;
    }
    public class DataBodyPosition
    {
        public int ColPos { get; set; }
        public string Value { get; set; }
        public long PositionSmId { get; set; }
        public string @Type { get; set; }
        public bool OccupantPJ { get; set; } = false;
        public bool OccupantCLT { get; set; } = false;
        public List<Tooltip> Tooltips { get; set; } = new List<Tooltip>();
        public long CmCode { get; set; }
    }
    public class Tooltip
    {
        public string Position { get; set; }
        public int Amount { get; set; }
    }

    public class PositionBaseDTO
    {
        public long CargoSmId { get; set; }
        public string OccupantType { get; set; }
        public int? OccupantTypeId { get; set; }
        public string PositionBase { get; set; }
        public long CompanyId { get; set; }
    }

    public class ProjectPositions
    {
        public long PositionSMLocalId { get; set; }
        public string PositionSalaryMark { get; set; }
        public string Profile { get; set; }
        public int LevelId { get; set; }
        public string Level { get; set; }
        public double HourBase { get; set; }
        public int GSM { get; set; }
        public long CmCode { get; set; }
        public string TechnicalAdjustment { get; set; }
        public long GroupId { get; set; }
        public string Smcode { get; set; }
        public string Company { get; set; }
        public string Table { get; set; }
        public long CompanyId { get; set; }
        public IEnumerable<string> Area { get; set; }
        public IEnumerable<string> CareerAxis { get; set; }
        public IEnumerable<string> Parameter01 { get; set; }
        public IEnumerable<string> Parameter02 { get; set; }
        public IEnumerable<string> Parameter03 { get; set; }
        public List<SalaryTableValueByPosition> SalaryTableValues { get; set; }
    }

    public class SalaryTableValueByPosition
    {
        public double Multiplicator { get; set; }
        public double Value { get; set; }
    }

    public class SalaryTableValuesPosition
    {
        public long LocalIdSalaryTable { get; set; }
        public int GSM { get; set; }
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

    }


    public class LevelCompanyAllPositionsDTO
    {
        public int LevelId { get; set; }
        public string Level { get; set; }
    }

    public class ColumnDataPositionDTO
    {
        public long ColumnId { get; set; }
        public bool? IsChecked { get; set; }
        public string Name { get; set; }

    }
}