using System.Collections.Generic;
using SM.Domain.Enum;

namespace SM.Api.ViewModel
{
    public class PositionDetailViewModel
    {
        public long PositionId { get; set; }
        public string Position { get; set; }
        public string SMCode { get; set; }
        public long LevelId { get; set; }
        public long GroupId { get; set; }
        public ModulesEnum ModuleId { get; set; }
        public List<UpdateProjectParametersViewModel> Parameters { get; set; }
    }

    public class UpdateProjectParametersViewModel
    {
        public long ParameterId { get; set; }
        public List<long> ProjectParameters { get; set; }
        public List<string> NewProjectParameters { get; set; }
    }

    public class UpdateSalaryTablePositionDetailsViewModel
    {
        public ModulesEnum ModuleId { get; set; }
        public List<SalaryTableMappingDataViewModel> SalaryTableMappings { get; set; }
    }

    public class SalaryTableMappingDataViewModel
    {
        public long TableId { get; set; }
        public long UnitId { get; set; }
        public int GSM { get; set; }
        public bool Deleted { get; set; }
        public bool Created { get; set; }
    }
}