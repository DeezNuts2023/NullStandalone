using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class MenuOverlayViewModel : BindableBase
	{
        private bool _isCollapsed;
        public ObservableCollection<AlignmentModeViewModel> AlignmentModes { get; }
		public ObservableCollection<ZoomModeViewModel> ZoomModes { get; }
		public ObservableCollection<QualityViewModel> Qualities { get; }
		public DelegateCommand ToggleViewCommand { get; }
		public DelegateCommand<AlignmentModeViewModel> SetAlignmentModeCommand { get; }
		public DelegateCommand<ZoomModeViewModel> SetZoomModeCommand { get; }
		public DelegateCommand<QualityViewModel> SetQualityCommand { get; }
		public DelegateCommand OpenSettingsCommand { get; }
		private FlashPlayer FlashPlayer { get; }
		private GameSettings GameSettings { get; }
		private WindowService WindowService { get; }

        public bool IsCollapsed
        {
            get => this._isCollapsed;
            set => this.SetProperty(ref this._isCollapsed, value, nameof(IsCollapsed));
        }

        public MenuOverlayViewModel(FlashPlayer flashPlayer, GameSettings gameSettings, WindowService windowService)
		{
			this.FlashPlayer = flashPlayer;
			this.GameSettings = gameSettings;
			this.WindowService = windowService;
			this.IsCollapsed = true;
			this.AlignmentModes = new ObservableCollection<AlignmentModeViewModel>();
			foreach (FlashPlayer.AlignmentModeEnum alignmentModeEnum in Enum.GetValues(typeof(FlashPlayer.AlignmentModeEnum)).Cast<FlashPlayer.AlignmentModeEnum>())
			{
				this.AlignmentModes.Add(new AlignmentModeViewModel(alignmentModeEnum)
				{
					IsSelected = (this.GameSettings.AlignmentMode == alignmentModeEnum)
				});
			}
			this.ZoomModes = new ObservableCollection<ZoomModeViewModel>();
			foreach (FlashPlayer.ZoomModeEnum zoomModeEnum in Enum.GetValues(typeof(FlashPlayer.ZoomModeEnum)).Cast<FlashPlayer.ZoomModeEnum>())
			{
				this.ZoomModes.Add(new ZoomModeViewModel(zoomModeEnum)
				{
					IsSelected = (this.GameSettings.ZoomMode == zoomModeEnum)
				});
			}
			this.Qualities = new ObservableCollection<QualityViewModel>();
			foreach (FlashPlayer.QualityEnum qualityEnum in Enum.GetValues(typeof(FlashPlayer.QualityEnum)).Cast<FlashPlayer.QualityEnum>())
			{
				this.Qualities.Add(new QualityViewModel(qualityEnum)
				{
					IsSelected = (this.GameSettings.Quality == qualityEnum)
				});
			}
			this.ToggleViewCommand = new DelegateCommand(delegate()
			{
				this.IsCollapsed = !this.IsCollapsed;
			});
			this.SetAlignmentModeCommand = new DelegateCommand<AlignmentModeViewModel>(delegate(AlignmentModeViewModel a)
			{
				foreach (AlignmentModeViewModel alignmentModeViewModel in this.AlignmentModes)
				{
					alignmentModeViewModel.IsSelected = false;
				}
				a.IsSelected = true;
				this.FlashPlayer.AlignmentMode = a.Mode;
			});
			this.SetZoomModeCommand = new DelegateCommand<ZoomModeViewModel>(delegate(ZoomModeViewModel z)
			{
				foreach (ZoomModeViewModel zoomModeViewModel in this.ZoomModes)
				{
					zoomModeViewModel.IsSelected = false;
				}
				z.IsSelected = true;
				this.FlashPlayer.ZoomMode = z.Mode;
			});
			this.SetQualityCommand = new DelegateCommand<QualityViewModel>(delegate(QualityViewModel q)
			{
				foreach (QualityViewModel qualityViewModel in this.Qualities)
				{
					qualityViewModel.IsSelected = false;
				}
				q.IsSelected = true;
				this.FlashPlayer.Quality = q.Quality;
			});
			this.OpenSettingsCommand = new DelegateCommand(delegate()
			{
				this.WindowService.ShowGameSettingsWindow();
			});
		}
	}
}
