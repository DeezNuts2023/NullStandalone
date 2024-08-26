using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Prism.Commands;
using Prism.Mvvm;
using tfmClientHook;

namespace tfmStandalone
{
    public class ChatWindowViewModel : BindableBase
	{
		private MainWindowViewModel MainWindowViewModel { get; }
        private Dictionary<string, ChatViewModel> KeyedChatList { get; }
        private ChatModel Model { get; }
        private GameSettings GameSettings { get; }
        private GameInfo GameInfo { get; }
        private WindowService WindowService { get; }
        private MessageInterceptor MessageInterceptor { get; }
        private ChatViewModel PreviousSelectedWhisper { get; set; }
        private Dictionary<string, string> ChatColors { get; }
        private Stack<string> ClosedChats { get; }
        public ObservableCollection<ChatViewModel> Chats { get; }
        public DelegateCommand SendChatCommand { get; }
        public DelegateCommand<string> CloseChatCommand { get; }
        public DelegateCommand<string> OpenNotesCommand { get; }
        public DelegateCommand OpenSettingsCommand { get; }
        public DelegateCommand NewChatCommand { get; }
        public DelegateCommand ReopenChatCommand { get; }
        public EventHandler NewMessageReceived;
        private string _username;
        private ChatViewModel _selectedChat;
        private string _chatText;
        private int _frozenTabCount;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value, "Username");
        }

        public ChatViewModel SelectedChat
		{
			get
			{
				return this._selectedChat;
			}
			set
			{
				ChatViewModel selectedChat = this._selectedChat;
				if (this.SetProperty<ChatViewModel>(ref this._selectedChat, value, "SelectedChat"))
				{
					if (selectedChat != null)
					{
						selectedChat.IsSelected = false;
					}
					if (this._selectedChat == null)
					{
						return;
					}
					if (this._selectedChat.IsWhisper)
					{
						if (this.PreviousSelectedWhisper != null)
						{
							this.PreviousSelectedWhisper.IsPreviousSelectedWhisper = false;
						}
						this.PreviousSelectedWhisper = this._selectedChat;
						this.PreviousSelectedWhisper.IsPreviousSelectedWhisper = true;
					}
					if (!this._selectedChat.IsWhisper && selectedChat != null && selectedChat.IsWhisper)
					{
						this.PreviousSelectedWhisper = selectedChat;
						this.PreviousSelectedWhisper.IsPreviousSelectedWhisper = true;
					}
					this._selectedChat.IsSelected = true;
					this._selectedChat.HasNewMessages = false;
				}
			}
		}

        public string ChatText
        {
            get => _chatText;
            set => SetProperty(ref _chatText, value, "ChatText");
        }

        public FontFamily FontFamily => GameSettings.FontFamily;
        public int FontSize => GameSettings.FontSize;

        public ChatWindowViewModel(ChatModel model, MainWindowViewModel mainWindowViewModel, GameSettings gameSettings, GameInfo gameInfo, WindowService windowService, MessageInterceptor messageInterceptor)
		{
			this.Model = model;
			this.MainWindowViewModel = mainWindowViewModel;
			this.GameSettings = gameSettings;
			this.GameInfo = gameInfo;
			this.WindowService = windowService;
			this.MessageInterceptor = messageInterceptor;
			this.ChatColors = new Dictionary<string, string>();
			this.LoadChatColors();
			MessageInterceptor messageInterceptor2 = this.MessageInterceptor;
			messageInterceptor2.LoggedIn = (EventHandler)Delegate.Combine(messageInterceptor2.LoggedIn, new EventHandler(delegate(object sender, EventArgs args)
			{
				this.Username = this.GameInfo.Name;
			}));
			GameSettings gameSettings2 = this.GameSettings;
			gameSettings2.SettingsSaved = (EventHandler)Delegate.Combine(gameSettings2.SettingsSaved, new EventHandler(delegate(object sender, EventArgs args)
			{
				this.OnPropertyChanged(new PropertyChangedEventArgs("FontFamily"));
				this.OnPropertyChanged(new PropertyChangedEventArgs("FontSize"));
				this.LoadChatColors();
			}));
			this.SendChatCommand = new DelegateCommand(new Action(this.SendChat));
			this.CloseChatCommand = new DelegateCommand<string>(new Action<string>(this.CloseChat));
			this.OpenNotesCommand = new DelegateCommand<string>(new Action<string>(this.OpenNotes));
			this.OpenSettingsCommand = new DelegateCommand(delegate()
			{
				this.WindowService.ShowChatSettingsWindow();
			});
			this.NewChatCommand = new DelegateCommand(new Action(this.NewChat));
			this.ReopenChatCommand = new DelegateCommand(new Action(this.ReopenChat));
			this.Chats = new ObservableCollection<ChatViewModel>();
			this.KeyedChatList = new Dictionary<string, ChatViewModel>();
			this.ClosedChats = new Stack<string>();
			ChatModel model2 = this.Model;
			model2.WhisperSent = (EventHandler<WhisperEventArgs>)Delegate.Combine(model2.WhisperSent, new EventHandler<WhisperEventArgs>(this.WhisperSent));
			ChatModel model3 = this.Model;
			model3.WhisperReceived = (EventHandler<WhisperEventArgs>)Delegate.Combine(model3.WhisperReceived, new EventHandler<WhisperEventArgs>(this.WhisperReceived));
			ChatModel model4 = this.Model;
			model4.StaffChatReceived = (EventHandler<StaffChatReceivedEventArgs>)Delegate.Combine(model4.StaffChatReceived, new EventHandler<StaffChatReceivedEventArgs>(this.StaffChatReceived));
			ChatModel model5 = this.Model;
			model5.TribeChatReceived = (EventHandler<TribeChatReceivedEventArgs>)Delegate.Combine(model5.TribeChatReceived, new EventHandler<TribeChatReceivedEventArgs>(this.TribeChatReceived));
			ChatModel model6 = this.Model;
			model6.ServerMessageReceived = (EventHandler<ServerMessageReceivedEventArgs>)Delegate.Combine(model6.ServerMessageReceived, new EventHandler<ServerMessageReceivedEventArgs>(this.ServerMessageReceived));
		}

		private void LoadChatColors()
		{
			this.ChatColors.Clear();
			Theme theme = this.GameSettings.Theme;
			if (theme == Theme.Light)
			{
				this.ChatColors["HistoryMessageColor"] = "5D5D5D";
				this.ChatColors["TabNameColor"] = "222222";
				this.ChatColors["ServerTabColor"] = "6C77C1";
				this.ChatColors["ServerMessageColor"] = "3046D3";
				this.ChatColors["ServerArbMessageColor"] = "9833C6";
				this.ChatColors["ModoTabColor"] = "C565FE";
				this.ChatColors["ModoMessageColor"] = "A425F9";
				this.ChatColors["ModoAllMessageColor"] = "753AFF";
				this.ChatColors["ArbitreTabColor"] = "B993CA";
				this.ChatColors["ArbitreMessageColor"] = "9833C6";
				this.ChatColors["ArbitreAllMessageColor"] = "733ED6";
				this.ChatColors["MapCrewTabColor"] = "60AEFF";
				this.ChatColors["MapCrewMessageColor"] = "1C8DFF";
				this.ChatColors["LuaTeamTabColor"] = "8FE2D1";
				this.ChatColors["LuaTeamMessageColor"] = "00AF86";
				this.ChatColors["FunCorpTabColor"] = "EAA5A5";
				this.ChatColors["FunCorpMessageColor"] = "E55757";
				this.ChatColors["FashionSquadTabColor"] = "FFB6C1";
				this.ChatColors["FashionSquadMessageColor"] = "FF516E";
				this.ChatColors["TribeTabColor"] = "2ECF73";
				this.ChatColors["TribeMessageColor"] = "18B500";
				this.ChatColors["WhisperTabColor"] = "F0A78E";
				this.ChatColors["WhisperModTabNameColor"] = "A425F9";
				this.ChatColors["WhisperArbTabNameColor"] = "9833C6";
				this.ChatColors["WhisperFriendTabNameColor"] = "CE1E00";
				this.ChatColors["WhisperSentMessageColor"] = "C47C00";
				this.ChatColors["WhisperReceivedMessageColor"] = "E2501F";
				return;
			}
			if (theme != Theme.Transformice)
			{
				this.ChatColors["HistoryMessageColor"] = "777777";
				this.ChatColors["TabNameColor"] = "D8D8D8";
				this.ChatColors["ServerTabColor"] = "6C77C1";
				this.ChatColors["ServerMessageColor"] = "6C77C1";
				this.ChatColors["ServerArbMessageColor"] = "C09EFF";
				this.ChatColors["ModoTabColor"] = "C565FE";
				this.ChatColors["ModoMessageColor"] = "C565FE";
				this.ChatColors["ModoAllMessageColor"] = "9263FF";
				this.ChatColors["ArbitreTabColor"] = "B993CA";
				this.ChatColors["ArbitreMessageColor"] = "B993CA";
				this.ChatColors["ArbitreAllMessageColor"] = "A285D7";
				this.ChatColors["MapCrewTabColor"] = "60AEFF";
				this.ChatColors["MapCrewMessageColor"] = "60AEFF";
				this.ChatColors["LuaTeamTabColor"] = "8FE2D1";
				this.ChatColors["LuaTeamMessageColor"] = "8FE2D1";
				this.ChatColors["FunCorpTabColor"] = "EAA5A5";
				this.ChatColors["FunCorpMessageColor"] = "EAA5A5";
				this.ChatColors["FashionSquadTabColor"] = "FFB6C1";
				this.ChatColors["FashionSquadMessageColor"] = "FFB6C1";
				this.ChatColors["TribeTabColor"] = "2ECF73";
				this.ChatColors["TribeMessageColor"] = "A4CF9E";
				this.ChatColors["WhisperTabColor"] = "F0A78E";
				this.ChatColors["WhisperModTabNameColor"] = "C565FE";
				this.ChatColors["WhisperArbTabNameColor"] = "B993CA";
				this.ChatColors["WhisperFriendTabNameColor"] = "FF6C4F";
				this.ChatColors["WhisperSentMessageColor"] = "EFCE8F";
				this.ChatColors["WhisperReceivedMessageColor"] = "F0A78E";
				return;
			}
			this.ChatColors["HistoryMessageColor"] = "777777";
			this.ChatColors["TabNameColor"] = "C2C2DA";
			this.ChatColors["ServerTabColor"] = "6C77C1";
			this.ChatColors["ServerMessageColor"] = "6C77C1";
			this.ChatColors["ServerArbMessageColor"] = "C09EFF";
			this.ChatColors["ModoTabColor"] = "C565FE";
			this.ChatColors["ModoMessageColor"] = "C565FE";
			this.ChatColors["ModoAllMessageColor"] = "9263FF";
			this.ChatColors["ArbitreTabColor"] = "B993CA";
			this.ChatColors["ArbitreMessageColor"] = "B993CA";
			this.ChatColors["ArbitreAllMessageColor"] = "A285D7";
			this.ChatColors["MapCrewTabColor"] = "60AEFF";
			this.ChatColors["MapCrewMessageColor"] = "60AEFF";
			this.ChatColors["LuaTeamTabColor"] = "8FE2D1";
			this.ChatColors["LuaTeamMessageColor"] = "8FE2D1";
			this.ChatColors["FunCorpTabColor"] = "EAA5A5";
			this.ChatColors["FunCorpMessageColor"] = "EAA5A5";
			this.ChatColors["FashionSquadTabColor"] = "FFB6C1";
			this.ChatColors["FashionSquadMessageColor"] = "FFB6C1";
			this.ChatColors["TribeTabColor"] = "2ECF73";
			this.ChatColors["TribeMessageColor"] = "A4CF9E";
			this.ChatColors["WhisperTabColor"] = "F0A78E";
			this.ChatColors["WhisperModTabNameColor"] = "C565FE";
			this.ChatColors["WhisperArbTabNameColor"] = "B993CA";
			this.ChatColors["WhisperFriendTabNameColor"] = "FF6C4F";
			this.ChatColors["WhisperSentMessageColor"] = "EFCE8F";
			this.ChatColors["WhisperReceivedMessageColor"] = "F0A78E";
		}

		public void ChatHidden()
		{
			this.SelectedChat = null;
			this.KeyedChatList.Clear();
			this.Chats.Clear();
			this._frozenTabCount = 0;
			this.GameInfo.IsChatShowing = false;
			this.MainWindowViewModel.IsChatShowing = false;
		}

		private void SendChat()
		{
			if (this.SelectedChat == null)
			{
				return;
			}
			if (string.IsNullOrWhiteSpace(this.ChatText))
			{
				return;
			}
			string text = this.ChatText.Replace("<", "&lt;");
			text = Regex.Replace(text, "\\r\\n?|\\n", " ");
			text = Regex.Replace(text, "\\s+", " ");
			List<string> list = new List<string>();
			bool flag = false;
			if ((this.SelectedChat.Name == "Modo" || this.SelectedChat.Name == "Arbitre") && text[0] == '*')
			{
				flag = true;
				text = text.Substring(1);
			}
			for (int i = 0; i < text.Length; i += 255)
			{
				list.Add(text.Substring(i, Math.Min(255, text.Length - i)));
			}
			string name = this.SelectedChat.Name;
			switch (name)
			{
				case "Modo":
					{
						if (flag)
						{
							using (List<string>.Enumerator enumerator = list.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									string message = enumerator.Current;
									this.Model.SendStaffChat(StaffChatType.ModerationAll, message);
								}
								goto IL_481;
							}
						}
						using (List<string>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								string message2 = enumerator.Current;
								this.Model.SendStaffChat(StaffChatType.ModerationLocal, message2);
							}
							goto IL_481;
						}
					}
				case "LuaTeam":
					goto IL_34B;
				case "Tribe":
					goto IL_404;
				case "Server":
					goto IL_481;
				case "MapCrew":
					goto IL_30E;
				case "FashionSquad":
					goto IL_3C6;
				case "FunCorp":
                    goto IL_388;
				case "Arbitre":
                    goto IL_64A;
            }
            IL_64A:
            if (flag)
			{
				using (List<string>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string message3 = enumerator.Current;
						this.Model.SendStaffChat(StaffChatType.ArbitreAll, message3);
					}
					goto IL_481;
				}
			}
			using (List<string>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string message4 = enumerator.Current;
					this.Model.SendStaffChat(StaffChatType.ArbitreLocal, message4);
				}
				goto IL_481;
			}
			IL_30E:
			using (List<string>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string message5 = enumerator.Current;
					this.Model.SendStaffChat(StaffChatType.MapCrew, message5);
				}
				goto IL_481;
			}
			IL_34B:
			using (List<string>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string message6 = enumerator.Current;
					this.Model.SendStaffChat(StaffChatType.LuaTeam, message6);
				}
				goto IL_481;
			}
			IL_388:
			using (List<string>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string message7 = enumerator.Current;
					this.Model.SendStaffChat(StaffChatType.FunCorp, message7);
				}
				goto IL_481;
			}
			IL_3C6:
			using (List<string>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string message8 = enumerator.Current;
					this.Model.SendStaffChat(StaffChatType.FashionSquad, message8);
				}
				goto IL_481;
			}
			IL_404:
			using (List<string>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string message9 = enumerator.Current;
					this.Model.SendTribeChat(message9);
				}
				goto IL_481;
			}
			IL_43D:
			foreach (string message10 in list)
			{
				this.Model.SendWhisper(this.SelectedChat.Name, message10);
			}
			IL_481:
			this.ChatText = string.Empty;
		}

		public void TogglePinChat(string name)
		{
			ChatViewModel chatViewModel = this.KeyedChatList[name.ToLowerInvariant()];
			if (chatViewModel.IsPinned)
			{
				this._frozenTabCount--;
				if (this.SelectedChat == chatViewModel)
				{
					this.Chats.Move(this.Chats.IndexOf(chatViewModel), this._frozenTabCount);
					this.SelectedChat = chatViewModel;
				}
				else
				{
					this.Chats.Move(this.Chats.IndexOf(chatViewModel), this._frozenTabCount);
				}
				chatViewModel.IsPinned = false;
				return;
			}
			if (this.SelectedChat == chatViewModel)
			{
				this.Chats.Move(this.Chats.IndexOf(chatViewModel), this._frozenTabCount);
				this.SelectedChat = chatViewModel;
			}
			else
			{
				this.Chats.Move(this.Chats.IndexOf(chatViewModel), this._frozenTabCount);
			}
			this._frozenTabCount++;
			chatViewModel.IsPinned = true;
		}

		public void CloseChat(string name)
		{
			if (this.KeyedChatList.ContainsKey(name.ToLowerInvariant()))
			{
				ChatViewModel item = this.KeyedChatList[name.ToLowerInvariant()];
				if (this.Chats.IndexOf(item) < this._frozenTabCount)
				{
					this._frozenTabCount--;
				}
				this.KeyedChatList.Remove(name.ToLowerInvariant());
				this.Chats.Remove(item);
				this.ClosedChats.Push(name.ToLowerInvariant());
			}
		}

		public void CloseAllWhispers()
		{
			for (int i = this.Chats.Count - 1; i >= 0; i--)
			{
				if (this.Chats[i].IsWhisper && !this.Chats[i].IsPinned)
				{
					this.CloseChat(this.Chats[i].Name);
				}
			}
		}

		public void CloseAllReadWhispers()
		{
			for (int i = this.Chats.Count - 1; i >= 0; i--)
			{
				if (this.Chats[i].IsWhisper && !this.Chats[i].IsPinned && !this.Chats[i].HasNewMessages)
				{
					this.CloseChat(this.Chats[i].Name);
				}
			}
		}

		public void SendCommand(string command)
		{
			this.Model.SendCommand(command);
		}

		public void SendClientCommand(string command)
		{
			this.Model.SendClientCommand(command);
		}

		private void OpenNotes(string name)
		{
			if (!string.IsNullOrWhiteSpace(name))
			{
				this.WindowService.ShowChatNotesWindow(name);
			}
		}

		private void NewChat()
		{
			string text = this.WindowService.ShowNewChatDialogWindow();
			if (text != null)
			{
				this.CreateTab(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text.ToLowerInvariant()));
			}
		}

		private void ReopenChat()
		{
			if (this.ClosedChats.Count == 0)
			{
				return;
			}
			string text = this.ClosedChats.Pop();
			this.CreateTab(text.ToLowerInvariant());
			this.SelectedChat = this.KeyedChatList[text.ToLowerInvariant()];
		}

		private void CreateTab(string name)
		{
			if (this.KeyedChatList.ContainsKey(name.ToLowerInvariant()))
			{
				return;
			}
			ChatViewModel chatViewModel = new ChatViewModel(this)
			{
				IsWhisper = false,
				IsPinned = true,
				TabNameColor = this.ChatColors["TabNameColor"]
			};
			int num = -1;
			string text = name.ToLowerInvariant();

			switch(text)
			{
				case "server":
                    chatViewModel.Name = "Server";
                    chatViewModel.TabColor = this.ChatColors["ServerTabColor"];
                    goto IL_37B;
				case "funcorp":
                    chatViewModel.Name = "FunCorp";
                    chatViewModel.TabColor = this.ChatColors["FunCorpTabColor"];
                    goto IL_37B;
				case "mapcrew":
                    chatViewModel.Name = "MapCrew";
                    chatViewModel.TabColor = this.ChatColors["MapCrewTabColor"];
                    goto IL_37B;
				case "luateam":
                    chatViewModel.Name = "LuaTeam";
                    chatViewModel.TabColor = this.ChatColors["LuaTeamTabColor"];
                    goto IL_37B;
				case "tribe":
                    chatViewModel.Name = "Tribe";
                    chatViewModel.TabColor = this.ChatColors["TribeTabColor"];
                    goto IL_37B;
				case "arbitre":
                    chatViewModel.Name = "Arbitre";
                    chatViewModel.TabColor = this.ChatColors["ArbitreTabColor"];
                    goto IL_37B;
				case "modo":
                    chatViewModel.Name = "Modo";
                    chatViewModel.TabColor = this.ChatColors["ModoTabColor"];
                    goto IL_37B;
				case "fashionsquad":
                    chatViewModel.Name = "FashionSquad";
                    chatViewModel.TabColor = this.ChatColors["FashionSquadTabColor"];
                    goto IL_37B;
            }
			chatViewModel.Name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name.ToLowerInvariant());
			if (this.GameInfo.FriendList.ContainsKey(name.ToLowerInvariant()))
			{
				chatViewModel.TabNameColor = this.ChatColors["WhisperFriendTabNameColor"];
			}
			else if (this.GameInfo.ModList.ContainsKey(name.ToLowerInvariant()))
			{
				chatViewModel.TabNameColor = this.ChatColors["WhisperModTabNameColor"];
			}
			else if (this.GameInfo.ArbList.ContainsKey(name.ToLowerInvariant()))
			{
				chatViewModel.TabNameColor = this.ChatColors["WhisperArbTabNameColor"];
			}
			chatViewModel.TabColor = this.ChatColors["WhisperTabColor"];
			chatViewModel.IsWhisper = true;
			chatViewModel.IsPinned = false;
			IL_37B:
			if (!chatViewModel.IsWhisper)
			{
				num = this.FindTabIndex(chatViewModel.Name);
			}
			if (num != -1)
			{
				this.Chats.Insert(num, chatViewModel);
				this._frozenTabCount++;
			}
			else
			{
				this.Chats.Insert(this._frozenTabCount, chatViewModel);
			}
			this.KeyedChatList.Add(name.ToLowerInvariant(), chatViewModel);
			IEnumerable<string> chatHistory = this.Model.GetChatHistory(chatViewModel.Name, 100);
			if (chatHistory != null)
			{
				chatViewModel.AddHistory(this.ChatColors["HistoryMessageColor"], chatHistory);
			}
		}

		private int FindTabIndex(string tabName)
		{
			string[] array = new string[]
			{
				"Server",
				"Modo",
				"Arbitre",
				"Tribe"
			};
			int result = this.Chats.Count((ChatViewModel c) => !c.IsWhisper);
			int num = Array.IndexOf<string>(array, tabName);
			if (num == -1)
			{
				return result;
			}
			for (int i = num - 1; i >= 0; i--)
			{
				if (!this.KeyedChatList.ContainsKey(array[i].ToLowerInvariant()))
				{
					num--;
				}
			}
			return num;
		}

		private void WhisperSent(object s, WhisperEventArgs e)
		{
			if (!this.GameInfo.IsChatShowing)
			{
				return;
			}
			this.CreateTab(e.Name);
			ChatViewModel chatViewModel = this.KeyedChatList[e.Name.ToLowerInvariant()];
			if (!chatViewModel.IsPinned)
			{
				if (this.SelectedChat == chatViewModel)
				{
					ChatViewModel selectedChat = this.SelectedChat;
					this.Chats.Move(this.Chats.IndexOf(chatViewModel), this._frozenTabCount);
					this.SelectedChat = selectedChat;
				}
				else
				{
					this.Chats.Move(this.Chats.IndexOf(chatViewModel), this._frozenTabCount);
				}
			}
			chatViewModel.AddMessage(this.ChatColors["WhisperSentMessageColor"], e.Message, true);
		}

		private void WhisperReceived(object sender, WhisperEventArgs e)
		{
			if (!this.GameInfo.IsChatShowing)
			{
				return;
			}
			this.CreateTab(e.Name);
			ChatViewModel chatViewModel = this.KeyedChatList[e.Name.ToLowerInvariant()];
			if (!chatViewModel.IsPinned)
			{
				if (this.SelectedChat == chatViewModel)
				{
					ChatViewModel selectedChat = this.SelectedChat;
					this.Chats.Move(this.Chats.IndexOf(chatViewModel), this._frozenTabCount);
					this.SelectedChat = selectedChat;
				}
				else
				{
					this.Chats.Move(this.Chats.IndexOf(chatViewModel), this._frozenTabCount);
				}
			}
			chatViewModel.AddMessage(this.ChatColors["WhisperReceivedMessageColor"], e.Message, true);
			chatViewModel.Community = ServerInfo.TribulleCommunities[e.Community];
			if (this.GameSettings.AlertWhispers && !chatViewModel.IsMuted)
			{
				EventHandler newMessageReceived = this.NewMessageReceived;
				if (newMessageReceived == null)
				{
					return;
				}
				newMessageReceived(this, new EventArgs());
			}
		}

		private void StaffChatReceived(object sender, StaffChatReceivedEventArgs e)
		{
			if (!this.GameInfo.IsChatShowing)
			{
				return;
			}
			switch (e.Type)
			{
			case StaffChatType.ArbitreLocal:
				this.CreateTab("Arbitre");
				this.KeyedChatList["arbitre"].AddMessage(this.ChatColors["ArbitreMessageColor"], e.Message, true);
				if (this.GameSettings.AlertArbChat && e.Name != this.GameInfo.Name && !this.KeyedChatList["arbitre"].IsMuted)
				{
					EventHandler newMessageReceived = this.NewMessageReceived;
					if (newMessageReceived == null)
					{
						return;
					}
					newMessageReceived(this, new EventArgs());
					return;
				}
				break;
			case StaffChatType.ModerationLocal:
				this.CreateTab("Modo");
				this.KeyedChatList["modo"].AddMessage(this.ChatColors["ModoMessageColor"], e.Message, true);
				if (this.GameSettings.AlertModoChat && e.Name != this.GameInfo.Name && !this.KeyedChatList["modo"].IsMuted)
				{
					EventHandler newMessageReceived2 = this.NewMessageReceived;
					if (newMessageReceived2 == null)
					{
						return;
					}
					newMessageReceived2(this, new EventArgs());
					return;
				}
				break;
			case StaffChatType.ModerationAll:
				this.CreateTab("Modo");
				this.KeyedChatList["modo"].AddMessage(this.ChatColors["ModoAllMessageColor"], e.Message, true);
				if (this.GameSettings.AlertModoChat && e.Name != this.GameInfo.Name && !this.KeyedChatList["modo"].IsMuted)
				{
					EventHandler newMessageReceived3 = this.NewMessageReceived;
					if (newMessageReceived3 == null)
					{
						return;
					}
					newMessageReceived3(this, new EventArgs());
					return;
				}
				break;
			case StaffChatType.ArbitreAll:
				this.CreateTab("Arbitre");
				this.KeyedChatList["arbitre"].AddMessage(this.ChatColors["ArbitreAllMessageColor"], e.Message, true);
				if (this.GameSettings.AlertArbChat && e.Name != this.GameInfo.Name && !this.KeyedChatList["arbitre"].IsMuted)
				{
					EventHandler newMessageReceived4 = this.NewMessageReceived;
					if (newMessageReceived4 == null)
					{
						return;
					}
					newMessageReceived4(this, new EventArgs());
					return;
				}
				break;
			case StaffChatType.Global:
				break;
			case StaffChatType.MapCrew:
				this.CreateTab("MapCrew");
				this.KeyedChatList["mapcrew"].AddMessage(this.ChatColors["MapCrewMessageColor"], e.Message, true);
				if (this.GameSettings.AlertMapCrewChat && e.Name != this.GameInfo.Name && !this.KeyedChatList["mapcrew"].IsMuted)
				{
					EventHandler newMessageReceived5 = this.NewMessageReceived;
					if (newMessageReceived5 == null)
					{
						return;
					}
					newMessageReceived5(this, new EventArgs());
					return;
				}
				break;
			case StaffChatType.LuaTeam:
				this.CreateTab("LuaTeam");
				this.KeyedChatList["luateam"].AddMessage(this.ChatColors["LuaTeamMessageColor"], e.Message, true);
				if (this.GameSettings.AlertLuaTeamChat && e.Name != this.GameInfo.Name && !this.KeyedChatList["luateam"].IsMuted)
				{
					EventHandler newMessageReceived6 = this.NewMessageReceived;
					if (newMessageReceived6 == null)
					{
						return;
					}
					newMessageReceived6(this, new EventArgs());
					return;
				}
				break;
			case StaffChatType.FunCorp:
				this.CreateTab("FunCorp");
				this.KeyedChatList["funcorp"].AddMessage(this.ChatColors["FunCorpMessageColor"], e.Message, true);
				if (this.GameSettings.AlertFunCorpChat && e.Name != this.GameInfo.Name && !this.KeyedChatList["funcorp"].IsMuted)
				{
					EventHandler newMessageReceived7 = this.NewMessageReceived;
					if (newMessageReceived7 == null)
					{
						return;
					}
					newMessageReceived7(this, new EventArgs());
					return;
				}
				break;
			case StaffChatType.FashionSquad:
				this.CreateTab("FashionSquad");
				this.KeyedChatList["fashionsquad"].AddMessage(this.ChatColors["FashionSquadMessageColor"], e.Message, true);
				if (this.GameSettings.AlertFashionSquadChat && e.Name != this.GameInfo.Name && !this.KeyedChatList["fashionsquad"].IsMuted)
				{
					EventHandler newMessageReceived8 = this.NewMessageReceived;
					if (newMessageReceived8 == null)
					{
						return;
					}
					newMessageReceived8(this, new EventArgs());
				}
				break;
			default:
				return;
			}
		}

		private void TribeChatReceived(object sender, TribeChatReceivedEventArgs e)
		{
			if (!this.GameInfo.IsChatShowing)
			{
				return;
			}
			this.CreateTab("Tribe");
			this.KeyedChatList["tribe"].AddMessage(this.ChatColors["TribeMessageColor"], e.Message, true);
			if (this.GameSettings.AlertTribeChat && !this.KeyedChatList["tribe"].IsMuted)
			{
				EventHandler newMessageReceived = this.NewMessageReceived;
				if (newMessageReceived == null)
				{
					return;
				}
				newMessageReceived(this, new EventArgs());
			}
		}

		private void ServerMessageReceived(object sender, ServerMessageReceivedEventArgs e)
		{
			if (!this.GameInfo.IsChatShowing)
			{
				return;
			}
			this.CreateTab("Server");
			string color = (from a in this.GameInfo.ArbList
			where this.GameInfo.IsMatchingCommunity(a.Value.Community)
			select a.Value.Name).ToList<string>().Any((string a) => e.Message.ToLowerInvariant().Contains(a)) ? this.ChatColors["ServerArbMessageColor"] : this.ChatColors["ServerMessageColor"];
			this.KeyedChatList["server"].AddMessage(color, e.Message, this.GameSettings.AlertServeurMessages && !e.IsBuffyBan);
			if (this.GameSettings.AlertServeurMessages && !e.IsBuffyBan && !this.KeyedChatList["server"].IsMuted)
			{
				EventHandler newMessageReceived = this.NewMessageReceived;
				if (newMessageReceived == null)
				{
					return;
				}
				newMessageReceived(this, new EventArgs());
			}
		}

		// Token: 0x04000205 RID: 517
		public static string[] StaffChats = new string[]
		{
			"Server",
			"Arbitre",
			"Modo",
			"MapCrew",
			"LuaTeam",
			"FunCorp",
			"FashionSquad"
		};
	}
}
