using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Commands;
using Prism.Mvvm;
using tfmClientHook;
using tfmClientHook.Messages;

namespace tfmStandalone
{
	public sealed class ModopwetViewModel : BindableBase
	{
		public ObservableCollection<ModopwetCommunityViewModel> Communities { get; }
		private Dictionary<Community, ModopwetCommunityViewModel> CommunityDictionary { get; }
        private bool IsShowingOnePersonReports { get; set; }
        private bool IsNotificationsActive { get; set; }
        private List<string> NotificationCommunities { get; }
        public DelegateCommand<string> SetCommunityCommand { get; }
        public DelegateCommand OpenCommunitySelectionPopupCommand { get; }
        public DelegateCommand<ModopwetCommunityViewModel> SetSecondaryCommunityCommand { get; }
        public DelegateCommand SortByTimeCommand { get; }
        public DelegateCommand SortByCountCommand { get; }
        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand ToggleAutoRefreshCommand { get; }
        public DelegateCommand<string> SetAutoRefreshTimeCommand { get; }
        public DelegateCommand OpenSettingsPopoupCommand { get; }
        public DelegateCommand<ModopwetCommunityViewModel> ToggleNotificationCommunityCommand { get; }
        public DelegateCommand ToggleAllNotificationsCommand { get; }
        public DelegateCommand ToggleNoneNotificationsCommand { get; }
        public DelegateCommand CloseSettingsPopupCommand { get; }
        public ObservableCollection<CaseViewModel> Cases { get; }
        private Dictionary<string, CaseViewModel> CasesDictionary { get; }
        public DelegateCommand<CaseViewModel> SelectCase { get; }
        public DelegateCommand<CaseViewModel> WatchCommand { get; }
        public DelegateCommand<CaseViewModel> FollowCommand { get; }
        public DelegateCommand<CaseViewModel> BanhackCommand { get; }
        public DelegateCommand<CaseViewModel> IbanhackCommand { get; }
        public DelegateCommand<CaseViewModel> DeleteCommand { get; }
        public DelegateCommand<CaseViewModel> HandleCommand { get; }
        public DelegateCommand<CaseViewModel> ProfileCommand { get; }
        public DelegateCommand<CaseViewModel> CasierCommand { get; }
        private bool IsForwardSorted { get; set; }
        private DispatcherTimer RefreshTimer { get; set; }
        private MainWindowViewModel MainWindowViewModel { get; }
        private ClientHook ClientHook { get; }
        private MessageInterceptor MessageInterceptor { get; }
        private GameInfo GameInfo { get; }
        public EventHandler NewReportReceived;
        private string _currentCommunity;
        private string _primaryCommunity;
        private string _secondaryCommunity;
        private bool _isCommunitySelectionPopupOpen;
        private bool _isSortedByTime;
        private bool _isAutoRefreshEnabled;
        private bool _isAutoRefreshPopupOpen;
        private bool _isSettingsPopupOpen;
        private bool _hasOnePersonReports;
        private bool _isGettingNotifications;
        private CaseViewModel _selectedCase;


        public string CurrentCommunity
        {
            get => this._currentCommunity;
            set
            {
                if (SetProperty(ref this._currentCommunity, value, nameof(CurrentCommunity)))
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsCurrentCommunityPrimary)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsCurrentCommunitySecondary)));
                }
            }
        }

        public string PrimaryCommunity
        {
            get => this._primaryCommunity;
            set
            {
                if (SetProperty(ref this._primaryCommunity, value, nameof(PrimaryCommunity)))
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsCurrentCommunityPrimary)));
                }
            }
        }

        public bool IsCurrentCommunityPrimary => this.CurrentCommunity == this.PrimaryCommunity;

        public string SecondaryCommunity
        {
            get => this._secondaryCommunity;
            set
            {
                if (SetProperty(ref this._secondaryCommunity, value, nameof(SecondaryCommunity)))
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsCurrentCommunitySecondary)));
                }
            }
        }

        public bool IsCurrentCommunitySecondary => this.CurrentCommunity == this.SecondaryCommunity;

        public bool IsCommunitySelectionPopupOpen
        {
            get => this._isCommunitySelectionPopupOpen;
            set => SetProperty(ref this._isCommunitySelectionPopupOpen, value, nameof(IsCommunitySelectionPopupOpen));
        }

        public bool IsSortedByTime
        {
            get => this._isSortedByTime;
            set => SetProperty(ref this._isSortedByTime, value, nameof(IsSortedByTime));
        }

        public bool IsAutoRefreshEnabled
        {
            get => this._isAutoRefreshEnabled;
            set
            {
                if (SetProperty(ref this._isAutoRefreshEnabled, value, nameof(IsAutoRefreshEnabled)))
                {
                    if (!value && this.RefreshTimer != null)
                    {
                        this.RefreshTimer.Tick -= this.RefreshTimerOnTick;
                        this.RefreshTimer.IsEnabled = false;
                        this.RefreshTimer = null;
                    }
                }
            }
        }

        public bool IsAutoRefreshPopupOpen
        {
            get => this._isAutoRefreshPopupOpen;
            set => SetProperty(ref this._isAutoRefreshPopupOpen, value, nameof(IsAutoRefreshPopupOpen));
        }

        public bool IsSettingsPopupOpen
		{
			get
			{
				return this._isSettingsPopupOpen;
			}
			set
			{
				this.SetProperty<bool>(ref this._isSettingsPopupOpen, value, "IsSettingsPopupOpen");
				if (!value)
				{
					if (this.HasOnePersonReports != this.IsShowingOnePersonReports)
					{
						this.IsShowingOnePersonReports = this.HasOnePersonReports;
						this.RequestReportList();
					}
					List<string> list = (from c in this.Communities
					where c.IsMonitored
					select c.Community).ToList<string>();
					if (this.IsGettingNotifications)
					{
						if (this.IsNotificationsActive && this.NotificationCommunities.All(new Func<string, bool>(list.Contains)) && this.NotificationCommunities.Count == list.Count)
						{
							return;
						}
						this.NotificationCommunities.Clear();
						this.NotificationCommunities.AddRange(list);
						this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetSubscribeNotifications
						{
							IsSubscribed = false,
							Communities = new List<string>()
						});
						this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetSubscribeNotifications
						{
							IsSubscribed = (this.NotificationCommunities.Count != 0),
							Communities = this.NotificationCommunities
						});
					}
					else
					{
						if (!this.IsNotificationsActive)
						{
							return;
						}
						this.NotificationCommunities.Clear();
						this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetSubscribeNotifications
						{
							IsSubscribed = false,
							Communities = this.NotificationCommunities
						});
					}
					this.IsNotificationsActive = this.IsGettingNotifications;
				}
			}
		}

        public bool HasOnePersonReports
        {
            get => this._hasOnePersonReports;
            set => SetProperty(ref this._hasOnePersonReports, value, nameof(HasOnePersonReports));
        }

        public bool IsGettingNotifications
        {
            get => this._isGettingNotifications;
            set => SetProperty(ref this._isGettingNotifications, value, nameof(IsGettingNotifications));
        }

        public CaseViewModel SelectedCase
        {
            get => this._selectedCase;
            set => SetProperty(ref this._selectedCase, value, nameof(SelectedCase));
        }

        public ModopwetViewModel(MainWindowViewModel mainWindowViewModel, ClientHook clientHook, MessageInterceptor messageInterceptor, GameInfo gameInfo)
		{
			this.MainWindowViewModel = mainWindowViewModel;
			this.ClientHook = clientHook;
			this.GameInfo = gameInfo;
			this.MessageInterceptor = messageInterceptor;
			MessageInterceptor messageInterceptor2 = this.MessageInterceptor;
			messageInterceptor2.ReportListMessageReceived = (EventHandler<S_ReportListMessage>)Delegate.Combine(messageInterceptor2.ReportListMessageReceived, new EventHandler<S_ReportListMessage>(this.ReportListMessageReceived));
			MessageInterceptor messageInterceptor3 = this.MessageInterceptor;
			messageInterceptor3.ReportSanctionedMessageReceived = (EventHandler<S_ReportSanctionedMessage>)Delegate.Combine(messageInterceptor3.ReportSanctionedMessageReceived, new EventHandler<S_ReportSanctionedMessage>(this.ReportSanctionedMessageReceived));
			MessageInterceptor messageInterceptor4 = this.MessageInterceptor;
			messageInterceptor4.ReportDisconnectedReceived = (EventHandler<S_ReportDisconnectedMessage>)Delegate.Combine(messageInterceptor4.ReportDisconnectedReceived, new EventHandler<S_ReportDisconnectedMessage>(this.ReportDisconnectedReceived));
			MessageInterceptor messageInterceptor5 = this.MessageInterceptor;
			messageInterceptor5.ReportDeletedReceived = (EventHandler<S_ReportDeletedMessage>)Delegate.Combine(messageInterceptor5.ReportDeletedReceived, new EventHandler<S_ReportDeletedMessage>(this.ReportDeletedReceived));
			MessageInterceptor messageInterceptor6 = this.MessageInterceptor;
			messageInterceptor6.ReportWatcherReceived = (EventHandler<S_ReportWatcherMessage>)Delegate.Combine(messageInterceptor6.ReportWatcherReceived, new EventHandler<S_ReportWatcherMessage>(this.ReportWatcherReceived));
			MessageInterceptor messageInterceptor7 = this.MessageInterceptor;
			messageInterceptor7.ChatLogReceived = (EventHandler<S_ChatLogMessage>)Delegate.Combine(messageInterceptor7.ChatLogReceived, new EventHandler<S_ChatLogMessage>(this.ChatLogReceived));
			MessageInterceptor messageInterceptor8 = this.MessageInterceptor;
			messageInterceptor8.ReportRoomUpdateReceived = (EventHandler<S_ReportRoomUpdatedMessage>)Delegate.Combine(messageInterceptor8.ReportRoomUpdateReceived, new EventHandler<S_ReportRoomUpdatedMessage>(this.ReportRoomUpdateReceived));
			MessageInterceptor messageInterceptor9 = this.MessageInterceptor;
			messageInterceptor9.ReportCountsUpdateReceived = (EventHandler<S_ReportCountsUpdateMessage>)Delegate.Combine(messageInterceptor9.ReportCountsUpdateReceived, new EventHandler<S_ReportCountsUpdateMessage>(this.ReportCountsUpdateReceived));
			MessageInterceptor messageInterceptor10 = this.MessageInterceptor;
			messageInterceptor10.LoggedIn = (EventHandler)Delegate.Combine(messageInterceptor10.LoggedIn, new EventHandler(delegate(object sender, EventArgs args)
			{
				this.PrimaryCommunity = this.GameInfo.Community.ToString().ToUpperInvariant();
				if (this.PrimaryCommunity == "EN")
				{
					this.SecondaryCommunity = "E2";
				}
				else if (this.PrimaryCommunity == "E2")
				{
					this.SecondaryCommunity = "EN";
				}
				this.CurrentCommunity = "ALL";
			}));
			this.Cases = new ObservableCollection<CaseViewModel>();
			this.CasesDictionary = new Dictionary<string, CaseViewModel>();
			this.CurrentCommunity = "ALL";
			this.PrimaryCommunity = "XX";
			this.Communities = new ObservableCollection<ModopwetCommunityViewModel>();
			this.CommunityDictionary = new Dictionary<Community, ModopwetCommunityViewModel>();
			foreach (Community key in from Community cm in Enum.GetValues(typeof(Community))
			orderby cm.ToString()
			select cm)
			{
				ModopwetCommunityViewModel modopwetCommunityViewModel = new ModopwetCommunityViewModel
				{
					Community = key.ToString().ToUpperInvariant()
				};
				this.Communities.Add(modopwetCommunityViewModel);
				this.CommunityDictionary.Add(key, modopwetCommunityViewModel);
			}
			this.HasOnePersonReports = false;
			this.IsSortedByTime = true;
			this.IsForwardSorted = true;
			this.NotificationCommunities = new List<string>();
			this.SetCommunityCommand = new DelegateCommand<string>(delegate(string c)
			{
				this.CurrentCommunity = c;
				this.RequestReportList();
			});
			this.OpenCommunitySelectionPopupCommand = new DelegateCommand(delegate()
			{
				this.IsCommunitySelectionPopupOpen = !this.IsCommunitySelectionPopupOpen;
			});
			this.SetSecondaryCommunityCommand = new DelegateCommand<ModopwetCommunityViewModel>(delegate(ModopwetCommunityViewModel vm)
			{
				string community = vm.Community;
				if (this.PrimaryCommunity == community)
				{
					this.SecondaryCommunity = null;
				}
				else if (this.SecondaryCommunity != community)
				{
					this.SecondaryCommunity = community;
				}
				if (this.CurrentCommunity != community)
				{
					this.CurrentCommunity = community;
				}
				this.IsCommunitySelectionPopupOpen = false;
				this.RequestReportList();
			});
			this.SortByTimeCommand = new DelegateCommand(delegate()
			{
				if (this.IsSortedByTime)
				{
					this.IsForwardSorted = !this.IsForwardSorted;
					this.RequestReportList();
					return;
				}
				this.IsSortedByTime = true;
				this.IsForwardSorted = true;
				this.RequestReportList();
			});
			this.SortByCountCommand = new DelegateCommand(delegate()
			{
				if (this.IsSortedByTime)
				{
					this.IsSortedByTime = false;
					this.IsForwardSorted = true;
					this.RequestReportList();
					return;
				}
				this.IsForwardSorted = !this.IsForwardSorted;
				this.RequestReportList();
			});
			this.RefreshCommand = new DelegateCommand(delegate()
			{
				if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
				{
					this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetToggle
					{
						IsRunning = false
					});
					this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetToggle
					{
						IsRunning = true
					});
				}
				this.RequestReportList();
			});
			this.ToggleAutoRefreshCommand = new DelegateCommand(delegate()
			{
				if (this.IsAutoRefreshEnabled)
				{
					this.IsAutoRefreshEnabled = false;
					return;
				}
				this.IsAutoRefreshPopupOpen = true;
			});
			this.SetAutoRefreshTimeCommand = new DelegateCommand<string>(delegate(string t)
			{
				int num = int.Parse(t);
				this.RefreshTimer = new DispatcherTimer
				{
					Interval = TimeSpan.FromSeconds((double)num)
				};
				this.RefreshTimer.Tick += this.RefreshTimerOnTick;
				this.RefreshTimer.Start();
				this.IsAutoRefreshEnabled = true;
				this.IsAutoRefreshPopupOpen = false;
			});
			this.OpenSettingsPopoupCommand = new DelegateCommand(delegate()
			{
				this.IsSettingsPopupOpen = !this.IsSettingsPopupOpen;
			});
			this.ToggleNotificationCommunityCommand = new DelegateCommand<ModopwetCommunityViewModel>(delegate(ModopwetCommunityViewModel c)
			{
				c.IsMonitored = !c.IsMonitored;
			});
			this.ToggleAllNotificationsCommand = new DelegateCommand(delegate()
			{
				EnumerableExtensions.ForEach<ModopwetCommunityViewModel>(this.Communities, delegate(ModopwetCommunityViewModel c)
				{
					c.IsMonitored = true;
				});
			});
			this.ToggleNoneNotificationsCommand = new DelegateCommand(delegate()
			{
				EnumerableExtensions.ForEach<ModopwetCommunityViewModel>(this.Communities, delegate(ModopwetCommunityViewModel c)
				{
					c.IsMonitored = false;
				});
			});
			this.CloseSettingsPopupCommand = new DelegateCommand(delegate()
			{
				this.IsSettingsPopupOpen = false;
			});
			this.SelectCase = new DelegateCommand<CaseViewModel>(delegate(CaseViewModel c)
			{
				if (this.SelectedCase != null)
				{
					this.SelectedCase.IsSelected = false;
				}
				if (this.SelectedCase == c)
				{
					this.SelectedCase = null;
					return;
				}
				this.SelectedCase = c;
				this.SelectedCase.IsSelected = true;
				this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetRequestChatLog
				{
					Name = c.Name
				});
			});
			this.WatchCommand = new DelegateCommand<CaseViewModel>(delegate(CaseViewModel c)
			{
				this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetWatch
				{
					Name = c.Name
				});
			});
			this.FollowCommand = new DelegateCommand<CaseViewModel>(delegate(CaseViewModel c)
			{
				this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetWatch
				{
					Name = c.Name,
					IsFollowing = true
				});
			});
			this.BanhackCommand = new DelegateCommand<CaseViewModel>(delegate(CaseViewModel c)
			{
				this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetBanhack
				{
					Name = c.Name
				});
			});
			this.IbanhackCommand = new DelegateCommand<CaseViewModel>(delegate(CaseViewModel c)
			{
				this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetBanhack
				{
					Name = c.Name,
					IsInvisible = true
				});
			});
			this.DeleteCommand = new DelegateCommand<CaseViewModel>(delegate(CaseViewModel c)
			{
				this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetDeleteReport
				{
					Name = c.Name
				});
			});
			this.HandleCommand = new DelegateCommand<CaseViewModel>(delegate(CaseViewModel c)
			{
				this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetDeleteReport
				{
					Name = c.Name,
					IsHandled = true
				});
			});
			this.ProfileCommand = new DelegateCommand<CaseViewModel>(delegate(CaseViewModel c)
			{
				this.ClientHook.SendToServer(ConnectionType.Main, new C_Command
				{
					Command = string.Format("profile {0}", c.Name)
				});
			});
			this.CasierCommand = new DelegateCommand<CaseViewModel>(delegate(CaseViewModel c)
			{
				this.ClientHook.SendToServer(ConnectionType.Main, new C_Command
				{
					Command = string.Format("casier {0}", c.Name)
				});
			});
		}

		private void RefreshTimerOnTick(object sender, EventArgs eventArgs)
		{
			this.RequestReportList();
		}

		private void ReportListMessageReceived(object sender, S_ReportListMessage reportList)
		{
			List<string> oldCases = new List<string>(this.CasesDictionary.Keys);
			this.Cases.Clear();
			this.CasesDictionary.Clear();
			foreach (S_ReportListMessage.PlayerReport playerReport in reportList.PlayerReports)
			{
				CaseViewModel caseViewModel = new CaseViewModel
				{
					Name = playerReport.PlayerName,
					IsSameRoom = !playerReport.IsGeneral,
					Community = playerReport.Community,
					Hours = playerReport.PlayerHours,
					Room = playerReport.PlayerRoom,
					CurrentWatchers = string.Join(", ", playerReport.Watchers)
				};
				if (caseViewModel.Room.Contains('\u0003'))
				{
					caseViewModel.Room = caseViewModel.Room.Replace("\u0003", string.Empty);
					caseViewModel.IsRoomTribehouse = true;
				}
				short num = -1;
				short num2 = -1;
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (S_ReportListMessage.Report report2 in playerReport.Reports)
				{
					ReportViewModel reportViewModel = new ReportViewModel
					{
						ReporterName = report2.ReporterName,
						ReporterKarma = report2.ReporterKarma,
						Comment = report2.Comment.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&"),
						Type = report2.Type.ToString(),
						Age = report2.Age
					};
					caseViewModel.Reports.Add(reportViewModel);
					if (num == -1 || reportViewModel.Age < num)
					{
						num = reportViewModel.Age;
					}
					if (num2 == -1 || reportViewModel.Age > num2)
					{
						num2 = reportViewModel.Age;
					}
					if (!dictionary.ContainsKey(reportViewModel.Type))
					{
						dictionary.Add(reportViewModel.Type, 0);
					}
					Dictionary<string, int> dictionary2 = dictionary;
					string type = reportViewModel.Type;
					int num3 = dictionary2[type];
					dictionary2[type] = num3 + 1;
				}
				caseViewModel.NewestReportTime = num;
				caseViewModel.OldestReportTime = num2;
				caseViewModel.ReportTypes = string.Join(" / ", from kvp in dictionary.ToList<KeyValuePair<string, int>>()
				orderby kvp.Value descending
				select kvp.Key);
				if (caseViewModel.Reports.Count <= 2)
				{
					caseViewModel.ReportSeverity = 0;
				}
				else if (caseViewModel.Reports.Count <= 4)
				{
					caseViewModel.ReportSeverity = 1;
				}
				else if (caseViewModel.Reports.Count <= 6)
				{
					caseViewModel.ReportSeverity = 2;
				}
				else if (caseViewModel.Reports.Count <= 8)
				{
					caseViewModel.ReportSeverity = 3;
				}
				else
				{
					caseViewModel.ReportSeverity = 4;
				}
				this.Cases.Add(caseViewModel);
				this.CasesDictionary.Add(caseViewModel.Name, caseViewModel);
			}
			if (this.SelectedCase != null)
			{
				if (this.CasesDictionary.ContainsKey(this.SelectedCase.Name))
				{
					this.SelectedCase = this.CasesDictionary[this.SelectedCase.Name];
					this.SelectedCase.IsSelected = true;
					this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetRequestChatLog
					{
						Name = this.SelectedCase.Name
					});
				}
				else
				{
					this.SelectedCase = null;
				}
			}
			if (new List<string>(this.CasesDictionary.Keys).Any((string report) => !oldCases.Contains(report)))
			{
				EventHandler newReportReceived = this.NewReportReceived;
				if (newReportReceived == null)
				{
					return;
				}
				newReportReceived(this, EventArgs.Empty);
			}
		}

		private void ReportSanctionedMessageReceived(object sender, S_ReportSanctionedMessage sanctionMessage)
		{
			if (!this.CasesDictionary.ContainsKey(sanctionMessage.Name))
			{
				return;
			}
			CaseViewModel caseViewModel = this.CasesDictionary[sanctionMessage.Name];
			caseViewModel.IsSanctioned = true;
			if (sanctionMessage.SanctionReason == "$MessageTriche")
			{
				sanctionMessage.SanctionReason = "Hack. Your account will be permanently banned if you keep breaking the rules!";
			}
			if (sanctionMessage.IsBan)
			{
				this.CasesDictionary[sanctionMessage.Name].IsDeleted = true;
				caseViewModel.SanctionText = string.Format("Banned {0} hours by {1}, reason : {2}", sanctionMessage.SanctionLength, sanctionMessage.SanctionGiver, sanctionMessage.SanctionReason);
			}
			else
			{
				caseViewModel.SanctionText = string.Format("Muted {0} hours by {1}, reason : {2}", sanctionMessage.SanctionLength, sanctionMessage.SanctionGiver, sanctionMessage.SanctionReason);
			}
			caseViewModel.SanctionRemovalReason = caseViewModel.SanctionText;
		}

		private void ReportDisconnectedReceived(object sender, S_ReportDisconnectedMessage e)
		{
			if (this.CasesDictionary.ContainsKey(e.Name))
			{
				this.CasesDictionary[e.Name].IsDeleted = true;
				this.CasesDictionary[e.Name].SanctionRemovalReason = "Disconnected";
			}
		}

		private void ReportDeletedReceived(object sender, S_ReportDeletedMessage e)
		{
			if (this.CasesDictionary.ContainsKey(e.Name))
			{
				this.CasesDictionary[e.Name].IsDeleted = true;
				this.CasesDictionary[e.Name].SanctionRemovalReason = string.Format("Deleted by {0}", e.Deleter);
			}
		}

		private void ReportWatcherReceived(object sender, S_ReportWatcherMessage e)
		{
			if (this.CasesDictionary.ContainsKey(e.Name))
			{
				this.CasesDictionary[e.Name].CurrentWatchers = string.Join(", ", e.Watchers);
			}
		}

		private void ChatLogReceived(object sender, S_ChatLogMessage e)
		{
			if (!this.CasesDictionary.ContainsKey(e.Name))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (S_ChatLogMessage.Message message in e.Messages)
			{
				stringBuilder.AppendLine(string.Format("• [{0:H:mm}] [{1}] {2}", message.Time, e.Name, message.Text.Replace("&lt;", "<").Replace("&amp;", "&")));
			}
			this.CasesDictionary[e.Name].ChatLog = stringBuilder.ToString();
		}

		private void ReportRoomUpdateReceived(object sender, S_ReportRoomUpdatedMessage e)
		{
			if (this.CasesDictionary.ContainsKey(e.Name))
			{
				this.CasesDictionary[e.Name].IsRoomLocked = e.IsPassworded;
				this.CasesDictionary[e.Name].Room = e.Room;
				if (e.Room.Contains('\u0003'))
				{
					this.CasesDictionary[e.Name].Room = this.CasesDictionary[e.Name].Room.Replace("'\u0003'", string.Empty);
					this.CasesDictionary[e.Name].IsRoomTribehouse = true;
				}
				else
				{
					this.CasesDictionary[e.Name].IsRoomTribehouse = false;
				}
				if (this.CasesDictionary[e.Name].IsDeleted)
				{
					this.CasesDictionary[e.Name].IsDeleted = false;
					this.CasesDictionary[e.Name].SanctionRemovalReason = (this.CasesDictionary[e.Name].IsSanctioned ? this.CasesDictionary[e.Name].SanctionText : string.Empty);
				}
			}
		}

		private void ReportCountsUpdateReceived(object sender, S_ReportCountsUpdateMessage e)
		{
			foreach (S_ReportCountsUpdateMessage.ReportCount reportCount in e.ReportCounts)
			{
				if (this.CommunityDictionary.ContainsKey(reportCount.Community))
				{
					this.CommunityDictionary[reportCount.Community].ReportCount = reportCount.Count;
				}
			}
		}

		public void WindowHidden()
		{
			this.GameInfo.IsModopwetShowing = false;
			this.MainWindowViewModel.IsModopwetShowing = false;
		}

		public void WindowVisibilityChanged(bool isShowing)
		{
			if (isShowing)
			{
				this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetToggle
				{
					IsRunning = true
				});
				this.RequestReportList();
				return;
			}
			this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetToggle
			{
				IsRunning = false
			});
			if (this.IsAutoRefreshEnabled)
			{
				this.IsAutoRefreshEnabled = false;
			}
		}

		private void RequestReportList()
		{
			this.ClientHook.SendToServer(ConnectionType.Main, new C_ModopwetReportListRequest
			{
				Community = this.CurrentCommunity,
				HasOnePersonReports = this.IsShowingOnePersonReports,
				IsSortedByTime = this.IsSortedByTime,
				IsForwardSorted = this.IsForwardSorted
			});
		}
	}
}
