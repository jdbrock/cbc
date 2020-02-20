using Acr.UserDialogs;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CBC
{
    [ImplementPropertyChanged]
    public class MainTabbedPageViewModel
    {
        // ===========================================================================
        // = Private Constants
        // ===========================================================================


		private const String CBC_INITIAL_INTERNAL_DATA_FILENAME = "ffb-2020-preload-data.json";
		private const String CBC_CURRENT_DATA_FILENAME = "ffb-2020-data.json";
		private const String CBC_CURRENT_METADATA_FILENAME = "ffb-2020-metadata.json";

        private const String CBC_MAIN_DATA_URI = "https://byo.blob.core.windows.net/data-dev/ffb-2020.json";

        // ===========================================================================
        // = Public Properties
        // ===========================================================================

        public Boolean IsBusyModal { get; set; }

        // JBTODO
        public static MainTabbedPageViewModel Instance { get; set; }

        public BeerPageViewModel YellowViewModel { get; set; }
        public BeerPageViewModel BlueViewModel { get; set; }
        public BeerPageViewModel RedViewModel { get; set; }
        public BeerPageViewModel GreenViewModel { get; set; }

		public BeerPageViewModel AllSessionsViewModel { get;set; }

        // ===========================================================================
        // = Construction
        // ===========================================================================

        public MainTabbedPageViewModel()
        {
            Instance = this;

			AllSessionsViewModel = new BeerPageViewModel(this);

            YellowViewModel = new BeerPageViewModel(this);
            BlueViewModel   = new BeerPageViewModel(this);
            RedViewModel    = new BeerPageViewModel(this);
            GreenViewModel = new BeerPageViewModel(this);

            CopyInitialData();
            LoadMainData();
        }

        // ===========================================================================
        // = Public Methods
        // ===========================================================================

        public void Refresh(Action inCallback)
        {
            var tmpPath = Path.Combine(GetDocumentsPath(), CBC_CURRENT_DATA_FILENAME + ".tmp");
            var toPath = Path.Combine(GetDocumentsPath(), CBC_CURRENT_DATA_FILENAME);

            var client = new WebClient();
            client.DownloadFileCompleted += (S, E) => OnDownloadCompleted(tmpPath, toPath, inCallback, E);
            client.DownloadFileAsync(new Uri(CBC_MAIN_DATA_URI), tmpPath);
        }

        public void SetOrder(BeerSortOrder inSortOrder)
        {
			AllSessionsViewModel.SetOrder (inSortOrder);

            YellowViewModel.SetOrder(inSortOrder);
            BlueViewModel.SetOrder(inSortOrder);
            RedViewModel.SetOrder(inSortOrder);
            GreenViewModel.SetOrder(inSortOrder);
        }

        // ===========================================================================
        // = Private Methods
        // ===========================================================================

        private void CopyInitialData()
        {
            var fromPath = CBC_INITIAL_INTERNAL_DATA_FILENAME;
            var toPath = Path.Combine(GetDocumentsPath(), CBC_CURRENT_DATA_FILENAME);

            if (!File.Exists(fromPath))
                return;

			if (File.Exists(toPath))
                return;

            File.Copy(fromPath, toPath);
        }

        private void OnDownloadCompleted(String inFromPath, String inToPath, Action inCallback, AsyncCompletedEventArgs e)
        {
            try
            {
                // Failure
                if (e.Error != null || e.Cancelled)
                {
                    if (File.Exists(inFromPath))
                        File.Delete(inFromPath);

                    inCallback();
                }
                // Success
                else
                {
                    File.Copy(inFromPath, inToPath, true);
                    File.Delete(inFromPath);

                    LoadMainData();
                    inCallback();
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError("Error refreshing beer.");
                inCallback();
            }
        }

        private void LoadMainData()
        {
			var path = Path.Combine(GetDocumentsPath(), CBC_CURRENT_DATA_FILENAME);

            try
            {
				using (var cbcDataFile = File.OpenText(path))
                {
                    var cbcData = JsonConvert.DeserializeObject<CbcData>(cbcDataFile.ReadToEnd());

                    LoadMetaData(cbcData);

					AllSessionsViewModel.SetBeers(cbcData.Beers);
                    AllSessionsViewModel.SetNote(cbcData.Note);

                    YellowViewModel.SetBeers(cbcData.Beers.Where(X => X.Session == CbcSession.Yellow));
                    YellowViewModel.SetNote(cbcData.Note);

                    BlueViewModel.SetBeers(cbcData.Beers.Where(X => X.Session == CbcSession.Blue));
                    BlueViewModel.SetNote(cbcData.Note);

                    RedViewModel.SetBeers(cbcData.Beers.Where(X => X.Session == CbcSession.Red));
                    RedViewModel.SetNote(cbcData.Note);

                    GreenViewModel.SetBeers(cbcData.Beers.Where(X => X.Session == CbcSession.Green));
                    GreenViewModel.SetNote(cbcData.Note);
                }
            }
            catch
            {
                // Just in case we can't load the data (it was corrupted on download).
				File.Delete(path);
				CopyInitialData ();
				LoadMainData (); // Will cause a loop if the packaged data is broken.
            }
        }

        private void LoadMetaData(CbcData inData)
        {
            var documentsPath = GetDocumentsPath();

            if (File.Exists(Path.Combine(documentsPath, CBC_CURRENT_METADATA_FILENAME)))
            {
                var beersById = inData.Beers
                    .ToDictionary(X => X.Id);

                using (var cbcMetaDataFile = File.OpenText(Path.Combine(documentsPath, CBC_CURRENT_METADATA_FILENAME)))
                {
                    var metadata = JsonConvert.DeserializeObject<CbcMetaData>(cbcMetaDataFile.ReadToEnd());

                    foreach (var beerMetaData in metadata.BeerMetaData)
                        beersById[beerMetaData.BeerId].MetaData = beerMetaData;
                }
            }
        }

        public void SaveMetaData()
        {
            var metaData = new CbcMetaData();

            var beerViewModels = new[] 
            {
                YellowViewModel,
                BlueViewModel,
                RedViewModel,
                GreenViewModel
            };

            foreach (var viewModel in beerViewModels)
                foreach (var beer in viewModel.Beers)
                {
                    if (beer.Beer.MetaData.IsEmpty)
                        continue;

                    beer.Beer.MetaData.BeerId = beer.Beer.Id;
                    metaData.BeerMetaData.Add(beer.Beer.MetaData);
                }

            var documentsPath = GetDocumentsPath();

            File.WriteAllText(Path.Combine(documentsPath, CBC_CURRENT_METADATA_FILENAME), JsonConvert.SerializeObject(metaData));
        }

        private String GetDocumentsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
