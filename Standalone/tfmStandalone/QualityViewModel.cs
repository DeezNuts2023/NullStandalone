using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class QualityViewModel : BindableBase
	{
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value, "IsSelected");
        }

        public FlashPlayer.QualityEnum Quality { get; }
		public string Display { get; }

		public QualityViewModel(FlashPlayer.QualityEnum quality)
		{
			this.Quality = quality;
			this.Display = quality.ToString();
		}
	}
}
