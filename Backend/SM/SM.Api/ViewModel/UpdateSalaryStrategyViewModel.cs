using System.Collections.Generic;

namespace SM.Api.ViewModel
{
    public class UpdateSalaryStrategyViewModel
    {
        public long TableId { get; set; }
        public string Table { get; set; }
        public List<SalaryStrategyViewModel> SalaryStrategy { get; set; }
    }

    public class SalaryStrategyViewModel
    {
        public int ColPos { get; set; }
        public string Value { get; set; }
        public string @Type { get; set; }
        public long GroupId { get; set; }
        public long TrackId { get; set; }
    }
}
