using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class CommunityViewModel : BindableBase
	{
        private bool _isSelected;
        private string _community;

        public bool IsSelected
        {
            get => this._isSelected;
            set => this.SetProperty(ref this._isSelected, value, "IsSelected");
        }

        public string Community
        {
            get => this._community;
            set => this.SetProperty(ref this._community, value, "Community");
        }
    }
}
