using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CBC
{
    [ImplementPropertyChanged]
    public class BeerPageViewModel
    {
        public ObservableCollection<Beer> Beers { get; private set; }
        //public ICommand RefreshCommand { get; private set; }
        //public Boolean IsRefreshing { get; set; }

        private MainTabbedPageViewModel _parent;

        public BeerPageViewModel(MainTabbedPageViewModel inParent)
        {
            _parent = inParent;
            Beers = new ObservableCollection<Beer>();
            //RefreshCommand = new Command(OnRefresh);
        }

        public void SetBeers(IEnumerable<Beer> inBeers)
        {
            Beers.Clear();

            foreach (var beer in inBeers)
                Beers.Add(beer);
        }

        public void Refresh(Action inCallback)
        {
            _parent.Refresh(inCallback);
        }
    }
}
