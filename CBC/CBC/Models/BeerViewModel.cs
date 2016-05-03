using System;
using PropertyChanged;

namespace CBC
{
	[ImplementPropertyChanged]
	public class BeerViewModel
	{
		public bool IsSelected {
			get;
			set;
		}

		public Beer Beer { get; }

		public BeerViewModel (Beer beer)
		{
			Beer = beer;
		}
	}
}

