using System.Collections.Generic;

namespace SM.Application.Parameters.Queries.Response
{
    public class GetDisplayConfigurationResponse
    {
        public DisplayConfigurationResponse DisplayConfiguration { get; set; }
        public PreferenceResponse Preference { get; set; }
    }

    public class DisplayConfigurationResponse
    {
        public List<DisplayTypeResponse> DisplayTypes { get; set; }
    }

    public class PreferenceResponse
    {
        public List<GlobalLabelResponse> GlobalLabels { get; set; }
    }


    public class DisplayTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<DisplayItemResponse> SubItems { get; set; }
    }

    public class DisplayItemResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; } = true;
    }

    public class GlobalLabelResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDefault { get; set; }
        public bool Disabled {get;set;}
    }
}