﻿using System;
using PropertyChanged;
using System.ComponentModel;

namespace CBC
{
	[ImplementPropertyChanged]
	public class BeerViewModel : INotifyPropertyChanged
	{
		private bool _isExpanded;

		public bool IsExpanded
		{
			get
			{
				return _isExpanded;
			}
			set
			{
				_isExpanded = value;
				RaisePropertyChanged("IsExpanded");
			}
		}

		public Boolean IsTickedNotExpanded { get { return Beer.MetaData.IsTicked && !IsExpanded; } }

		public Beer Beer { get; }

		public BeerViewModel (Beer beer)
		{
			Beer = beer;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(String propertyName)
		{
			var handler = PropertyChanged;

			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

