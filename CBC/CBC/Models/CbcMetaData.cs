using System;
using System.Collections.Generic;
using System.Text;

namespace CBC
{
    public class CbcMetaData
    {
        public List<BeerMetaData> BeerMetaData { get; set; }

        public CbcMetaData()
        {
            BeerMetaData = new List<BeerMetaData>();
        }
    }
}
