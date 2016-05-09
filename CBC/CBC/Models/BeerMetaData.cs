using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;

namespace CBC
{
    [ImplementPropertyChanged]
    public class BeerMetaData
    {
        public String BeerId { get; set; }

		public Boolean IsTicked { get { return TickRating > 0; } }
        public DateTime TickDate { get; set; }
        public Decimal TickRating { get; set; }
        public String TickComments { get; set; }

        public Boolean IsFavorited { get; set; }

        public String StatusImageUri
        {
            get
            {
				if (IsTicked)
					return "checkmark-white.png";
				else if (IsFavorited)
					return "heart-filled-white.png";
				else
					return "";
            }
        }

		public Boolean IsEmpty { get { return !IsTicked && !IsFavorited && TickRating == default(Decimal); } }
    }
}
