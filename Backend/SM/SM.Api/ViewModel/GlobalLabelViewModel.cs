using System.Collections.Generic;
using SM.Application.Parameters.Command;

namespace SM.Api.ViewModel
{
    public class GlobalLabelViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDefault { get; set; }
    }
}
