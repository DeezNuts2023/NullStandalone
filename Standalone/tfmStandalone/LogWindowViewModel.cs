using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using tfmClientHook;
using tfmClientHook.Messages;

namespace tfmStandalone
{
	public sealed class LogWindowViewModel : BindableBase
	{
        public ObservableCollection<LogViewModel> Logs { get; }
        public DelegateCommand<LogViewModel> SelectLogCommand { get; }
        public DelegateCommand<string> CopyCommand { get; }
        public DelegateCommand<string> SearchCommand { get; }
        public DelegateCommand<string> RelationCommand { get; }
        public DelegateCommand<string> NomipCommand { get; }
        public DelegateCommand<string> IPnomCommand { get; }
        public DelegateCommand CloseCommand { get; }
        public EventHandler NewLogSelected;
        public EventHandler NewLogReceived;
        public EventHandler<LogViewModel> LogClosed;
        public EventHandler Closed;
        private LogHandlerService LogHandlerService { get; }
        private ClientHook ClientHook { get; }
        private LogViewModel _selectedLog;

        public LogViewModel SelectedLog
        {
            get => _selectedLog;
            set
            {
                if (_selectedLog != null) _selectedLog.IsSelected = false;
                SetProperty(ref _selectedLog, value, "SelectedLog");
                if (_selectedLog != null)
                {
                    _selectedLog.IsSelected = true;
                    NewLogSelected?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public LogWindowViewModel(LogHandlerService logHandler, ClientHook clientHook)
		{
			this.LogHandlerService = logHandler;
			this.ClientHook = clientHook;
			this.Logs = new ObservableCollection<LogViewModel>();
			this.SelectLogCommand = new DelegateCommand<LogViewModel>(delegate(LogViewModel l)
			{
				this.SelectedLog = l;
			});
			this.CopyCommand = new DelegateCommand<string>(new Action<string>(Clipboard.SetText));
			this.SearchCommand = new DelegateCommand<string>(delegate(string n)
			{
				this.SendCommand(string.Format("search {0}", n));
			});
			this.RelationCommand = new DelegateCommand<string>(delegate(string n)
			{
				this.SendCommand(string.Format("relation {0}", n));
			});
			this.NomipCommand = new DelegateCommand<string>(delegate(string n)
			{
				this.SendCommand(string.Format("nomip {0}", n));
			});
			this.IPnomCommand = new DelegateCommand<string>(delegate(string n)
			{
				this.SendCommand(string.Format("ipnom {0}", n));
			});
			this.CloseCommand = new DelegateCommand(delegate()
			{
				if (this.Logs.Count != 1)
				{
					int num = this.Logs.IndexOf(this.SelectedLog) - 1;
					if (num < 0)
					{
						num = 0;
					}
					this.Logs.Remove(this.SelectedLog);
					EventHandler<LogViewModel> logClosed = this.LogClosed;
					if (logClosed != null)
					{
						logClosed(this, this.SelectedLog);
					}
					this.SelectedLog = this.Logs[num];
					return;
				}
				EventHandler closed = this.Closed;
				if (closed == null)
				{
					return;
				}
				closed(this, EventArgs.Empty);
			});
		}

		public void RequestLog(string key, int count = 0)
		{
			this.LogHandlerService.RegisterForLog(this, key);
			if (count == 0)
			{
				this.SendCommand(string.Format("l {0}", key));
				return;
			}
			this.SendCommand(string.Format("l {0} {1}", key, count));
		}

		public void SendCommand(string command)
		{
			this.ClientHook.SendToServer(ConnectionType.Main, new C_Command
			{
				Command = command
			});
		}

		public void LogReceived(byte fontInfo, string windowKey, string originalText, bool isPlayer, string key, List<PlayerLogModel> logs)
		{
			LogViewModel logViewModel = new LogViewModel
			{
				IsPlayer = isPlayer,
				Key = key,
				FontStyle = fontInfo,
				WindowKey = windowKey,
				OriginalText = originalText
			};
			foreach (PlayerLogModel playerLogModel in logs)
			{
				LoginViewModel loginViewModel = new LoginViewModel
				{
					Name = playerLogModel.Name,
					Date = playerLogModel.Date,
					IPColor = playerLogModel.IPColor,
					IP = playerLogModel.IP,
					Country = playerLogModel.Country,
					Type = this.GetServiceName(playerLogModel.Type),
					Community = playerLogModel.Community
				};
				if (!logViewModel.IsPlayer && string.IsNullOrEmpty(logViewModel.KeyColor) && loginViewModel.IP == logViewModel.Key)
				{
					logViewModel.KeyColor = loginViewModel.IPColor;
				}
				logViewModel.Logins.Add(loginViewModel);
			}
			this.Logs.Add(logViewModel);
			this.SelectedLog = logViewModel;
			EventHandler newLogReceived = this.NewLogReceived;
			if (newLogReceived == null)
			{
				return;
			}
			newLogReceived(this, EventArgs.Empty);
		}

		private string GetServiceName(string service)
		{
			if (service == "$type_service.4.nom")
			{
				return "Transformice";
			}
			if (service == "$type_service.6.nom")
			{
				return "Fortoresse";
			}
			if (service == "$type_service.7.nom")
			{
				return "Bouboum";
			}
			if (service == "$type_service.9.nom")
			{
				return "Forum";
			}
			if (service == "$type_service.15.nom")
			{
				return "Nekodancer";
			}
			if (!(service == "$type_service.16.nom"))
			{
				return "Unknown";
			}
			return "Run for Cheese";
		}
	}
}
