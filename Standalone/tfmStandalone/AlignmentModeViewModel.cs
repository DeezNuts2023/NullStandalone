using System;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class AlignmentModeViewModel : BindableBase
	{
        private bool _isSelected;
        public FlashPlayer.AlignmentModeEnum Mode { get; }
        public string Display { get; }

        public bool IsSelected
        {
            get => this._isSelected;
            set => this.SetProperty(ref this._isSelected, value, nameof(IsSelected));
        }

        public AlignmentModeViewModel(FlashPlayer.AlignmentModeEnum mode)
		{
			this.Mode = mode;
			this.Display = mode.ToString();
		}
	}
}
