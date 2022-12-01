using System.Collections.Generic;

namespace SM.Api.ViewModel
{
    public class SaveLevelViewModel
    {
        public IEnumerable<SaveLevelConfigurationViewModel> Levels { get; set; }
    }
    public class SaveLevelConfigurationViewModel
    {
        public long LevelId { get; set; }
        public string Level { get; set; }
        public bool Enabled { get; set; }
    }
}
