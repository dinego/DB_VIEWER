using System.Collections.Generic;

namespace SM.Api.ViewModel
{
    public class SaveHourlyBasisViewModel 
    {
        public List<SaveHourlyBasisViewModelItems> HourlyBasis { get; set; }
    }

    public class SaveHourlyBasisViewModelItems
    {
        public long Id { get; set; }
        public bool Display { get; set; }
        public double SelectedValue { get; set; }
    }
}
