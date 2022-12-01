using CMC.Common.Extensions;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;

namespace SM.Api.ViewModel
{
    public class SalaryTableParameterViewModel
    {
        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public long TableId { get; set; }
        public string Table { get; set; }
        public long? UnitId { get; set; }
        public string Unit { get; set; }
        public long? GroupId { get; set; }
        public string Group { get; set; }
        public ContractTypeEnum ContractTypeId { get; set; } = ContractTypeEnum.CLT;
        public string ContractType { get; set; }
        public DataBaseSalaryEnum HoursTypeId { get; set; } = DataBaseSalaryEnum.MonthSalary;
        public string HoursType { get; set; }
        public IEnumerable<int> ColumnsExcluded { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public PermissionSharedViewModel Permissions { get; set; }
        public TableSalaryViewEnum ViewType { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public int? RangeInit { get; set; }
        public int? RangeFinal { get; set; }
        public bool ShowAllGsm { get; set; }
    }

    public class ShareDataSalaryTableViewModel
    {
        public long TableId { get; set; }
        public string Table { get; set; }
        public long? UnitId { get; set; }
        public string Unit { get; set; } = "Todas";
        public long? GroupId { get; set; }
        public string Group { get; set; } = "Todos";
        public ContractTypeEnum ContractTypeId { get; set; } = ContractTypeEnum.CLT;
        public string ContractType { get; set; } = ContractTypeEnum.CLT.GetDescription();
        public DataBaseSalaryEnum HoursTypeId { get; set; } = DataBaseSalaryEnum.MonthSalary;
        public string HoursType { get; set; } = DataBaseSalaryEnum.MonthSalary.GetDescription();
        public IEnumerable<int> ColumnsExcluded { get; set; }
        public TableSalaryViewEnum ViewType { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public int? RangeInit { get; set; }
        public int? RangeFinal { get; set; }
        public bool ShowAllGsm { get; set; }
        public PermissionSharedViewModel Permissions { get; set; }
    }
}
