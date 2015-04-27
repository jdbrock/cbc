using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CBC
{
	public partial class BeerPage : ContentPage
	{
		public BeerPage()
		{
			InitializeComponent();
		}

        private async void OnBeerFavorite(object sender, EventArgs e)
        {
            var item = (BindableObject)sender;
            var beer = (Beer)item.BindingContext;

            beer.MetaData.IsFavorited = !beer.MetaData.IsFavorited;

            MainTabbedPageViewModel.Instance.SaveMetaData();
        }

        private async void OnBeerTapped(object sender, ItemTappedEventArgs e)
        {
            var beer = (Beer)e.Item;

            await Navigation.PushAsync(new BeerTickPage(beer));
        }

        private async void OnBeerSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = (ListView)sender;

            if (listView.SelectedItem == null)
                return;

            listView.SelectedItem = null;
        }
	}
}
