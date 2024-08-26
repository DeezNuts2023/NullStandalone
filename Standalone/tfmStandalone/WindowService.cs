using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Practices.Unity;

namespace tfmStandalone
{
	public class WindowService
	{
		private IUnityContainer UnityContainer { get; }
		private Dictionary<string, ChatNotesWindow> NotesWindows { get; }
		public WindowService(IUnityContainer unityContainer)
		{
			this.UnityContainer = unityContainer;
			this.NotesWindows = new Dictionary<string, ChatNotesWindow>();
		}

		public void ShowChatWindow()
		{
			UnityContainerExtensions.Resolve<ChatWindow>(this.UnityContainer, new ResolverOverride[0]).Show();
		}

		public void HideChatWindow()
		{
			UnityContainerExtensions.Resolve<ChatWindow>(this.UnityContainer, new ResolverOverride[0]).Close();
		}

		public void ShowModopwetWindow()
		{
			UnityContainerExtensions.Resolve<ModopwetWindow>(this.UnityContainer, new ResolverOverride[0]).Show();
		}

		public void HideModopwetWindow()
		{
			UnityContainerExtensions.Resolve<ModopwetWindow>(this.UnityContainer, new ResolverOverride[0]).Close();
		}

		public void ShowGameSettingsWindow()
		{
			UnityContainerExtensions.Resolve<GameSettingsWindow>(this.UnityContainer, new ResolverOverride[0]).Show();
		}

		public void ShowChatNotesWindow(string name)
		{
			if (this.NotesWindows.ContainsKey(name))
			{
				this.NotesWindows[name].Activate();
				return;
			}
			ChatNotesWindow chatNotesWindow = new ChatNotesWindow(new ChatNotesWindowViewModel(name));
			chatNotesWindow.Closed += delegate(object sender, EventArgs args)
			{
				this.NotesWindows.Remove(name);
			};
			this.NotesWindows.Add(name, chatNotesWindow);
			chatNotesWindow.Show();
		}

		public void ShowChatSettingsWindow()
		{
			UnityContainerExtensions.Resolve<ChatSettingsWindow>(this.UnityContainer, new ResolverOverride[0]).Show();
		}

		public string ShowNewChatDialogWindow()
		{
			NewChatViewModel newChatViewModel = new NewChatViewModel();
			NewChatDialogWindow newChatDialogWindow = new NewChatDialogWindow(newChatViewModel);
			ChatWindow owner = UnityContainerExtensions.Resolve<ChatWindow>(this.UnityContainer, new ResolverOverride[0]);
			newChatDialogWindow.Owner = owner;
			bool? flag = newChatDialogWindow.ShowDialog();
			if (flag != null && flag.Value)
			{
				return newChatViewModel.Name;
			}
			return null;
		}

		public void ShowVpnFarmingWindow()
		{
			MainWindow owner = UnityContainerExtensions.Resolve<MainWindow>(this.UnityContainer, new ResolverOverride[0]);
			VpnFarmingWindow vpnFarmingWindow = UnityContainerExtensions.Resolve<VpnFarmingWindow>(this.UnityContainer, new ResolverOverride[0]);
			vpnFarmingWindow.Owner = owner;
			vpnFarmingWindow.Show();
		}

		public void ShowVpnFarmingRoomWindow()
		{
			MainWindow owner = UnityContainerExtensions.Resolve<MainWindow>(this.UnityContainer, new ResolverOverride[0]);
			VpnFarmingRoomWindow vpnFarmingRoomWindow = UnityContainerExtensions.Resolve<VpnFarmingRoomWindow>(this.UnityContainer, new ResolverOverride[0]);
			vpnFarmingRoomWindow.Owner = owner;
			vpnFarmingRoomWindow.Show();
		}

		public bool ShowUpdateConfirmationDialog(UpdateService updateService)
		{
			if (MessageBox.Show("Transformice must be closed to update. Are you sure you want to continue?", "Update", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				updateService.Update();
				return true;
			}
			return false;
		}

		public void ShowAnnouncementWindow()
		{
			MainWindow owner = UnityContainerExtensions.Resolve<MainWindow>(this.UnityContainer, new ResolverOverride[0]);
			AnnouncementWindow announcementWindow = UnityContainerExtensions.Resolve<AnnouncementWindow>(this.UnityContainer, new ResolverOverride[0]);
			announcementWindow.Owner = owner;
			announcementWindow.Show();
		}

		public void ShowLogWindow(byte fontInfo, string windowKey, string originalText, bool isPlayer, string key, List<PlayerLogModel> logs)
		{
			LogWindowViewModel logWindowViewModel = UnityContainerExtensions.Resolve<LogWindowViewModel>(this.UnityContainer, new ResolverOverride[0]);
			logWindowViewModel.LogReceived(fontInfo, windowKey, originalText, isPlayer, key, logs);
			MainWindow mainWindow = UnityContainerExtensions.Resolve<MainWindow>(this.UnityContainer, new ResolverOverride[0]);
			LogWindow logWindow = new LogWindow(logWindowViewModel)
			{
				Owner = mainWindow
			};
			this.PositionWindowInPlayableCenter(logWindow, mainWindow);
			logWindow.Show();
		}

		public void ShowCasierWindow(string text)
		{
			MainWindow mainWindow = UnityContainerExtensions.Resolve<MainWindow>(this.UnityContainer, new ResolverOverride[0]);
			CasierWindow casierWindow = new CasierWindow(text)
			{
				Owner = mainWindow
			};
			this.PositionWindowInPlayableCenter(casierWindow, mainWindow);
			casierWindow.Show();
		}

		private void PositionWindowInPlayableCenter(Window window, MainWindow mainWindow)
		{
			double num = mainWindow.GamePosition.Height / 600.0;
			double num2 = 23.0 * num;
			double num3 = 377.0 * num;
			window.Left = mainWindow.GamePosition.X + mainWindow.GamePosition.Width / 2.0 - window.Width / 2.0;
			window.Top = mainWindow.GamePosition.Y + num2 + num3 / 2.0 - window.Height / 2.0;
		}
	}
}
