using SM.Domain.Enum;
using System;
using System.Collections.Generic;

namespace SM.Api.Security
{
    public class ProductInfo
    {
        public ProductTypesEnum ProductType { get; set; }
        public string ProductName { get; set; }
        public List<long> UserCompanies { get; set; } = new List<long>();
        public long? ProjectId { get; set; }
        public AccessType? AccessType { get; set; }
        public List<Module> Modules { get; set; } = new List<Module>();
        public Permissions Permissions { get; set; }
    }

    public class Module
    {
        public long Id { get; set; }
        public IEnumerable<long> SubItems { get; set; }
    }

    public class Permissions
    {
        public bool CanFilterTypeofContract { get; set; }
        public bool CanFilterMM { get; set; }
        public bool CanFilterMI { get; set; }
        public bool CanFilterOccupants { get; set; }
        public bool CanDownloadExcel { get; set; }
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

        public override bool Equals(object obj)
        {
            if (!(obj is Permissions))
                return false;

            var other = obj as Permissions;

            if (CanFilterTypeofContract != other.CanFilterTypeofContract ||
                CanFilterMM != other.CanFilterMM ||
                CanFilterMI != other.CanFilterMI ||
                CanFilterOccupants != other.CanFilterOccupants ||
                CanDownloadExcel != other.CanDownloadExcel ||
                CanShare != other.CanShare ||
                CanEditLevels != other.CanEditLevels ||
                CanEditSalaryStrategy != other.CanEditSalaryStrategy ||
                CanEditHourlyBasis != other.CanEditHourlyBasis ||
                CanEditConfigPJ != other.CanEditConfigPJ ||
                CanEditUser != other.CanEditUser ||
                CanEditGlobalLabels != other.CanEditGlobalLabels ||
                CanEditLocalLabels != other.CanEditLocalLabels ||
                InactivePerson != other.InactivePerson ||
                CanAddPeople != other.CanAddPeople ||
                CanAddPosition != other.CanAddPosition ||
                CanChooseDefaultParameter != other.CanChooseDefaultParameter ||
                CanDeletePeople != other.CanDeletePeople ||
                CanDeletePosition != other.CanDeletePosition ||
                CanDisplayBossName != other.CanDisplayBossName ||
                CanDisplayEmployeeName != other.CanDisplayEmployeeName ||
                CanEditProjectSalaryTablesValues != other.CanEditProjectSalaryTablesValues ||
                CanMoveNextStep != other.CanMoveNextStep ||
                CanEditPosition != other.CanEditPosition ||
                CanEditPeople != other.CanEditPeople ||
                CanEditListPosition != other.CanEditListPosition ||
                CanEditGSMMappingTable != other.CanEditGSMMappingTable ||
                CanEditSalaryTableValues != other.CanEditSalaryTableValues ||
                CanEditMappingPositionSM != other.CanEditMappingPositionSM)
                return false;

            return true;
        }

        public static bool operator ==(Permissions x, Permissions y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Permissions x, Permissions y)
        {
            return !(x == y);
        }

    }
}
