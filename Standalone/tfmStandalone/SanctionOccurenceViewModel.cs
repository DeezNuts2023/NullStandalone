using System;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class SanctionOccurenceViewModel : BindableBase
	{
        private string _mute;
        private string _ban;
        private string _other;

        public string Mute
        {
            get => this._mute;
            set => this.SetProperty(ref this._mute, value, "Mute");
        }

        public string Ban
        {
            get => this._ban;
            set => this.SetProperty(ref this._ban, value, "Ban");
        }

        public string Other
        {
            get => this._other;
            set => this.SetProperty(ref this._other, value, "Other");
        }
    }
}
