using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;

namespace CBC
{
    [ImplementPropertyChanged]
    public class BeerTickPageViewModel
    {
        public Beer Beer { get; set; }

        public Decimal TickRating { get; set; }
        public String TickComments { get; set; }
    }
}
