using System.Collections.Generic;

namespace SM.Domain.Common
{
    public class PermissionJson
    {
        public IEnumerable<long> Levels { get; set; }
        public IEnumerable<long> TypeOfContract { get; set; }
        public IEnumerable<long> DataBase { get; set; }
        public IEnumerable<long> Areas { get; set; }
        public DisplayFieldJson Display { get; set; }
        public IEnumerable<FieldCheckedUserJson> Contents { get; set; }
        public IEnumerable<FieldCheckedUserJson> Modules { get; set; }
        public IEnumerable<PermissionFieldJson> Permission { get; set; }
    }

    public class FieldCheckedUserJson
    {
        public long Id { get; set; }
        public IEnumerable<long> SubItems { get; set; }

    }

    public class DisplayFieldJson
    {
        public IEnumerable<SubFieldCheckedJson> Scenario { get; set; }
        public IEnumerable<FieldCheckedUserJson> DisplaySections { get; set; }
    }

    public class SubFieldCheckedJson
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDefault { get; set; }
    }

    public class PermissionFieldJson
    {
        public long Id { get; set; }
        public IEnumerable<long> SubItems { get; set; } = new List<long>();
        public int? ExpireLinkDays { get; set; }
        public bool IsChecked { get; set; }
    }

    public class UserPermission
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
}