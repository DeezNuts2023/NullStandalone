using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Prism.Commands;
using Prism.Mvvm;

namespace tfmStandalone
{
	public class MainWindowViewModel : BindableBase
	{
        private string _status;
        private string _username;
        private bool _isChatShowing;
        private bool _isModopwetShowing;
        private bool _hasMaps;
        private double _version;
        private bool _isNewVersionAvailable;
        private double _newVersion;
        public DelegateCommand ToggleChatCommand { get; }
        public DelegateCommand ToggleModopwetCommand { get; }
        public DelegateCommand ShowVpnFarmingWindowCommand { get; }
        public DelegateCommand ShowVpnFarmingRoomWindowCommand { get; }
        public DelegateCommand UpdateCommand { get; }
        private MessageInterceptor MessageInterceptor { get; }
        private GameInfo GameInfo { get; }
        private WindowService WindowService { get; }
        private UpdateService UpdateService { get; }

        public string Status
        {
            get => this._status;
            set => this.SetProperty(ref this._status, value, nameof(Status));
        }

        public string Username
        {
            get => this._username;
            set => this.SetProperty(ref this._username, value, nameof(Username));
        }

        public bool IsChatShowing
        {
            get => this._isChatShowing;
            set => this.SetProperty(ref this._isChatShowing, value, nameof(IsChatShowing));
        }

        public bool IsModopwetShowing
        {
            get => this._isModopwetShowing;
            set => this.SetProperty(ref this._isModopwetShowing, value, nameof(IsModopwetShowing));
        }

        public bool HasMaps
        {
            get => this._hasMaps;
            set => this.SetProperty(ref this._hasMaps, value, nameof(HasMaps));
        }

        public double Version
        {
            get => this._version;
            set => this.SetProperty(ref this._version, value, nameof(Version));
        }

        public bool IsNewVersionAvailable
        {
            get => this._isNewVersionAvailable;
            set => this.SetProperty(ref this._isNewVersionAvailable, value, nameof(IsNewVersionAvailable));
        }

        public double NewVersion
        {
            get => this._newVersion;
            set => this.SetProperty(ref this._newVersion, value, nameof(NewVersion));
        }

        public MainWindowViewModel(MessageInterceptor messageInterceptor, GameInfo gameInfo, WindowService windowService, UpdateService updateService)
		{
			this.MessageInterceptor = messageInterceptor;
			this.GameInfo = gameInfo;
			this.WindowService = windowService;
			this.UpdateService = updateService;
			MessageInterceptor messageInterceptor2 = this.MessageInterceptor;
			messageInterceptor2.LoggedIn = (EventHandler)Delegate.Combine(messageInterceptor2.LoggedIn, new EventHandler(delegate(object sender, EventArgs args)
			{
				this.Username = this.GameInfo.Name;
			}));
			UpdateService updateService2 = this.UpdateService;
			updateService2.LatestVersionReceived = (EventHandler<double>)Delegate.Combine(updateService2.LatestVersionReceived, new EventHandler<double>(delegate(object sender, double newVersion)
			{
				this.NewVersion = newVersion;
				this.IsNewVersionAvailable = (this.NewVersion > this.Version);
			}));
			this.UpdateService.Initialize();
			this.ToggleChatCommand = new DelegateCommand(delegate()
			{
				this.IsChatShowing = !this.IsChatShowing;
				this.GameInfo.IsChatShowing = this.IsChatShowing;
				if (this.IsChatShowing)
				{
					this.WindowService.ShowChatWindow();
					return;
				}
				this.WindowService.HideChatWindow();
			});
			this.ToggleModopwetCommand = new DelegateCommand(delegate()
			{
				this.IsModopwetShowing = !this.IsModopwetShowing;
				this.GameInfo.IsModopwetShowing = this.IsModopwetShowing;
				if (this.IsModopwetShowing)
				{
					this.WindowService.ShowModopwetWindow();
					return;
				}
				this.WindowService.HideModopwetWindow();
			});
			this.ShowVpnFarmingWindowCommand = new DelegateCommand(delegate()
			{
				this.WindowService.ShowVpnFarmingWindow();
			});
			this.ShowVpnFarmingRoomWindowCommand = new DelegateCommand(delegate()
			{
				this.WindowService.ShowVpnFarmingRoomWindow();
			});
			this.UpdateCommand = new DelegateCommand(delegate()
			{
				this.WindowService.ShowUpdateConfirmationDialog(this.UpdateService);
			});
			this.IsChatShowing = true;
			if (!Directory.Exists(MapOverlayViewModel.MapsFolder))
			{
				this.HasMaps = false;
			}
			else
			{
				int num = Directory.EnumerateFiles(MapOverlayViewModel.MapsFolder, "*.png", SearchOption.AllDirectories).Count<string>();
				this.HasMaps = (num > 0);
			}
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			double version2;
			if (double.TryParse(string.Format("{0}.{1}", version.Major, version.Minor), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out version2))
			{
				this.Version = version2;
			}
		}
	}
}
