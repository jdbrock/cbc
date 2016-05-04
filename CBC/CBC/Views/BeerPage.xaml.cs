using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Syncfusion.SfRating.XForms;

namespace CBC
{
	public partial class BeerPage : ContentPage
    {
        public BeerPageViewModel ViewModel { get { return (BeerPageViewModel)BindingContext; } set { BindingContext = value; } }

		public BeerPage()
		{
			InitializeComponent();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnBeerFavorite(object sender, EventArgs e)
        {
            var item = (BindableObject)sender;
            var beer = (BeerViewModel)item.BindingContext;

            beer.Beer.MetaData.IsFavorited = !beer.Beer.MetaData.IsFavorited;

            MainTabbedPageViewModel.Instance.SaveMetaData();
        }

        private async void OnBeerTapped(object sender, ItemTappedEventArgs e)
        {
            var beer = (BeerViewModel)e.Item;

			// Tapped beer already selected? Deselect it and bail.
			if (beer.IsExpanded)
			{
				beer.IsExpanded = false;
				return;
			}

			// Deselect other beers.
			foreach (var otherBeer in ViewModel.Beers)
			{
				if (otherBeer.IsExpanded) // Avoid needless INotifyPropertyChanged events.
					otherBeer.IsExpanded = false;	
			}

			// Select tapped beer.
			beer.IsExpanded = true;

            //await Navigation.PushAsync(new BeerTickPage(beer.Beer));
        }

        private async void OnBeerSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = (ListView)sender;

            if (listView.SelectedItem == null)
                return;

            listView.SelectedItem = null;
        }

		private async void OnRatingValueChanged(object sender, ValueEventArgs e)
		{
			ViewModel.Parent.SaveMetaData ();
		}

        private async void OnRefreshing(object sender, EventArgs e)
        {
            var listView = (ListView)sender;

            ViewModel.Refresh(() => Device.BeginInvokeOnMainThread(() => listView.EndRefresh()));
        }
	}
}
