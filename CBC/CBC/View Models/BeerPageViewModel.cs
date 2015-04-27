using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CBC
{
    public class BeerPageViewModel
    {
        public ObservableCollection<Beer> Beers { get; private set; }

        public BeerPageViewModel(IEnumerable<Beer> inBeers)
        {
            Beers = new ObservableCollection<Beer>(inBeers);
        }
    }
}
