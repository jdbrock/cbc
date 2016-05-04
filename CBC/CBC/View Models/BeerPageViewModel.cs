using PropertyChanged;
using System;
using System.Linq;
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
        public ObservableCollection<BeerViewModel> Beers { get; private set; }

        public ICommand OrderAZCommand { get; set; }
        public ICommand OrderFavCommand { get; set; }

        private MainTabbedPageViewModel _parent;

        public BeerPageViewModel(MainTabbedPageViewModel inParent)
        {
            _parent = inParent;

            Beers = new ObservableCollection<BeerViewModel>();

            OrderAZCommand    = new Command(OnOrderAZ);
            OrderFavCommand   = new Command(OnOrderFav);
        }

        public void SetOrder(BeerSortOrder inSortOrder)
        {
			switch (inSortOrder) {

			case BeerSortOrder.AZ:
				Beers = new ObservableCollection<BeerViewModel> (Beers
                        .OrderBy (X => X.Beer.BreweryName)
					.ThenBy (X => X.Beer.BeerName));
				break;

			case BeerSortOrder.Fav:
				Beers = new ObservableCollection<BeerViewModel> (Beers
                        .OrderByDescending (X => X.Beer.MetaData.IsFavorited)
                        .ThenBy (X => X.Beer.BreweryName)
					.ThenBy (X => X.Beer.BeerName));
				break;
			}
        }

        private void OnOrderFav()
        {
            _parent.SetOrder(BeerSortOrder.Fav);
        }

        private void OnOrderAZ()
        {
            _parent.SetOrder(BeerSortOrder.AZ);
        }

        public void SetBeers(IEnumerable<Beer> inBeers)
        {
            Beers.Clear();

            foreach (var beer in inBeers)
				Beers.Add(new BeerViewModel(beer));
        }

        public void Refresh(Action inCallback)
        {
            _parent.Refresh(inCallback);
        }
    }
}
