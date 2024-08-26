using System.Collections.Generic;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class LogViewModel : BindableBase
	{
        private bool _isSelected;
        private string _keyColor;
        public bool IsPlayer { get; set; }
        public string Key { get; set; }
        public byte FontStyle { get; set; }
        public string WindowKey { get; set; }
        public string OriginalText { get; set; }
        public List<LoginViewModel> Logins { get; }

        public bool IsSelected
        {
            get => this._isSelected;
            set => this.SetProperty(ref this._isSelected, value, "IsSelected");
        }

        public string KeyColor
        {
            get => this._keyColor;
            set => this.SetProperty(ref this._keyColor, value, "KeyColor");
        }

        public LogViewModel()
		{
			this.Logins = new List<LoginViewModel>();
		}
	}
}
