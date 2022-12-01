using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using System.Collections.Generic;

namespace SM.Api.ViewModel
{
    public class ShareParametersViewModel
    {
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
        public IEnumerable<object> CategoriesExp { get; set; }
        public bool IsMM { get; set; } = false;
        public bool IsMI { get; set; } = false;
        public ContractTypeEnum ContractType { get; set; } = ContractTypeEnum.CLT;
        public DataBaseSalaryEnum HoursType { get; set; } = DataBaseSalaryEnum.MonthSalary;
        public string Term { get; set; } = null;
        public IEnumerable<int> ColumnsExcluded { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool ShowJustWithOccupants { get; set; } = false;
        public bool RemoveRowsEmpty { get; set; } = false;
        public long? GroupId { get; set; }
        public long? TableId { get; set; }
        public string ScenarioLabel { get; set; }
        public PermissionSharedViewModel Permissions { get; set; }
    }

    public class PermissionSharedViewModel
    {
        public bool CanFilterTypeofContract { get; set; }
        public bool CanFilterMM { get; set; }
        public bool CanFilterMI { get; set; }
        public bool CanFilterOccupants { get; set; }
        public bool CanDownloadExcel { get; set; }
        public bool CanRenameColumns { get; set; }
        public bool CanShare { get; set; }
        public bool CanEditLevels { get; set; }
        public bool CanEditSalaryStrategy { get; set; }
        public bool CanEditHourlyBasis { get; set; }
        public bool CanEditConfigPJ { get; set; }
        public bool CanEditUser { get; set; }
        public bool CanEditGlobalLabels { get; set; }
        public bool CanEditLocalLabels { get; set; }
        public bool InactivePerson { get; set; }
        public bool CanDisplayEmployeeName { get; set; }
        public bool CanDisplayBossName { get; set; }
        public bool CanEditProjectSalaryTablesValues { get; set; }
        public bool CanChooseDefaultParameter { get; set; }
        public bool CanMoveNextStep { get; set; }
        public bool CanAddPosition { get; set; }
        public bool CanEditPosition { get; set; }
        public bool CanDeletePosition { get; set; }
        public bool CanEditListPosition { get; set; }
        public bool CanEditGSMMappingTable { get; set; }
        public bool CanEditSalaryTableValues { get; set; }
        public bool CanAddPeople { get; set; }
        public bool CanDeletePeople { get; set; }
        public bool CanEditPeople { get; set; }
        public bool CanEditMappingPositionSM { get; set; }
    }

    public class ShareMapPositionViewModel
    {
        public long ProjectId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public long UserId { get; set; }
        public bool ShowJustWithOccupants { get; set; }
        public bool RemoveRowsEmpty { get; set; }
        public string Term { get; set; }
        public long? GroupId { get; set; }
        public long? TableId { get; set; }
        public DisplayByMapPositionEnum DisplayBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public IEnumerable<string> Columns { get; set; }
        public PermissionSharedViewModel Permissions { get; set; }
    }

    public class GenerateKeySaveViewModel
    {
        public long ModuleId { get; set; }
        public long? ModuleSubItemId { get; set; }
        public object Parameters { get; set; }
        public IEnumerable<object> ColumnsExcluded { get; set; } = new List<object>(); // columns exp
    }
    public class ShareLinkByEmailViewModel
    {
        public string To { get; set; }
        public string URL { get; set; }
    }
}
