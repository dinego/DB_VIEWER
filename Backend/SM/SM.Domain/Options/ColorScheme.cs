using System.Collections.Generic;

namespace SM.Domain.Options
{
    public class ColorScheme
    {
        public List<ColorData> Colors { get; set; }
    }

    public class ColorData
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public string Color { get; set; }
        public string FontColor { get; set; }
    }
}
