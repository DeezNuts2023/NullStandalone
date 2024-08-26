using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class SanctionCategoryViewModel : BindableBase
	{
        private string _description;

        public string Description
        {
            get => this._description;
            set => this.SetProperty(ref this._description, value, "Description");
        }

        public ObservableCollection<SanctionViewModel> Sanctions { get; }

		public SanctionCategoryViewModel()
		{
			this.Sanctions = new ObservableCollection<SanctionViewModel>();
		}
	}
}
