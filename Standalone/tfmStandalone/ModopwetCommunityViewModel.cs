using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class ModopwetCommunityViewModel : BindableBase
	{
        private int _reportCount;
        private bool _isMonitored;
        public string Community { get; set; }

        public int ReportCount
        {
            get => this._reportCount;
            set => this.SetProperty(ref this._reportCount, value, "ReportCount");
        }

        public bool IsMonitored
        {
            get => this._isMonitored;
            set => this.SetProperty(ref this._isMonitored, value, "IsMonitored");
        }
    }
}
