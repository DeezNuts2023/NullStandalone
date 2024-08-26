using System;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class ZoomModeViewModel : BindableBase
	{
        private bool _isSelected;
        public FlashPlayer.ZoomModeEnum Mode { get; }
		public string Display { get; }

        public bool IsSelected
        {
            get => this._isSelected;
            set => this.SetProperty(ref this._isSelected, value, nameof(IsSelected));
        }

        public ZoomModeViewModel(FlashPlayer.ZoomModeEnum mode)
		{
			this.Mode = mode;
			this.Display = mode.ToString();
		}
	}
}
