using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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

        private const String CBC_MAIN_DATA_PATH = "cbc.json";
        private const String CBC_METADATA_PATH = "metadata.json";

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

            using (var cbcDataFile = File.OpenText(CBC_MAIN_DATA_PATH))
            {
                var cbcData = JsonConvert.DeserializeObject<CbcData>(cbcDataFile.ReadToEnd());
                
                YellowViewModel = new BeerPageViewModel(cbcData.Beers.Where(X => X.Session == CbcSession.Yellow));
                BlueViewModel   = new BeerPageViewModel(cbcData.Beers.Where(X => X.Session == CbcSession.Blue));
                RedViewModel    = new BeerPageViewModel(cbcData.Beers.Where(X => X.Session == CbcSession.Red));
                GreenViewModel  = new BeerPageViewModel(cbcData.Beers.Where(X => X.Session == CbcSession.Green));

                LoadMetaData(cbcData);
            }
        }

        private void LoadMetaData(CbcData inData)
        {
            var documentsPath = GetDocumentsPath();

            if (File.Exists(Path.Combine(documentsPath, CBC_METADATA_PATH)))
            {
                var beersById = inData.Beers
                    .ToDictionary(X => X.Id);

                using (var cbcMetaDataFile = File.OpenText(Path.Combine(documentsPath, CBC_METADATA_PATH)))
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
                    if (beer.MetaData.IsEmpty)
                        continue;

                    beer.MetaData.BeerId = beer.Id;
                    metaData.BeerMetaData.Add(beer.MetaData);
                }

            var documentsPath = GetDocumentsPath();

            File.WriteAllText(Path.Combine(documentsPath, CBC_METADATA_PATH), JsonConvert.SerializeObject(metaData));
        }

        private String GetDocumentsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
