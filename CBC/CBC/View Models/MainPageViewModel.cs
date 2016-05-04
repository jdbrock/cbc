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

        private const String CBC_MAIN_DATA_FILENAME = "cbc.json";
        private const String CBC_METADATA_FILENAME = "metadata.json";

        private const String CBC_MAIN_DATA_URI = "https://cbc2016.blob.core.windows.net/data-prod/beer.json";

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

        // ===========================================================================
        // = Construction
        // ===========================================================================

        public MainTabbedPageViewModel()
        {
            Instance = this;

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
            var tmpPath = Path.Combine(GetDocumentsPath(), CBC_MAIN_DATA_FILENAME + ".tmp");
            var toPath = Path.Combine(GetDocumentsPath(), CBC_MAIN_DATA_FILENAME);

            var client = new WebClient();
            client.DownloadFileCompleted += (S, E) => OnDownloadCompleted(tmpPath, toPath, inCallback, E);
            client.DownloadFileAsync(new Uri(CBC_MAIN_DATA_URI), tmpPath);
        }

        public void SetOrder(BeerSortOrder inSortOrder)
        {
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
            var fromPath = CBC_MAIN_DATA_FILENAME;
            var toPath = Path.Combine(GetDocumentsPath(), CBC_MAIN_DATA_FILENAME);

            if (File.Exists(fromPath))
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
                inCallback();
            }
        }

        private void LoadMainData()
        {
            try
            {
                using (var cbcDataFile = File.OpenText(Path.Combine(GetDocumentsPath(), CBC_MAIN_DATA_FILENAME)))
                {
                    var cbcData = JsonConvert.DeserializeObject<CbcData>(cbcDataFile.ReadToEnd());

                    LoadMetaData(cbcData);

                    YellowViewModel.SetBeers(cbcData.Beers.Where(X => X.Session == CbcSession.Yellow));
                    BlueViewModel.SetBeers(cbcData.Beers.Where(X => X.Session == CbcSession.Blue));
                    RedViewModel.SetBeers(cbcData.Beers.Where(X => X.Session == CbcSession.Red));
                    GreenViewModel.SetBeers(cbcData.Beers.Where(X => X.Session == CbcSession.Green));
                }
            }
            catch
            {
                // Just in case we can't load the data (it was corrupted on download).
            }
        }

        private void LoadMetaData(CbcData inData)
        {
            var documentsPath = GetDocumentsPath();

            if (File.Exists(Path.Combine(documentsPath, CBC_METADATA_FILENAME)))
            {
                var beersById = inData.Beers
                    .ToDictionary(X => X.Id);

                using (var cbcMetaDataFile = File.OpenText(Path.Combine(documentsPath, CBC_METADATA_FILENAME)))
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

            File.WriteAllText(Path.Combine(documentsPath, CBC_METADATA_FILENAME), JsonConvert.SerializeObject(metaData));
        }

        private String GetDocumentsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
