using SM.Domain.Enum;
using System.Collections.Generic;

namespace SM.Domain.Options
{
    public class LevelsConfiguration
    {
        public List<LevelStructureConfiguration> Strategic { get; set; }
        public List<LevelStructureConfiguration> Tatic { get; set; }
        public List<LevelStructureConfiguration> Operational { get; set; }
    }

    public class LevelStructureConfiguration
    {
        public long Level { get; set; }
        public string Code { get; set; }
        public ColumnLevelType ColumnLevelType { get; set; }
    }
}
