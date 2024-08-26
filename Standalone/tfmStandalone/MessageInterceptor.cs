using System;
using System.Collections.Generic;
using System.Globalization;
using tfmClientHook;
using tfmClientHook.Messages;

namespace tfmStandalone
{
	public sealed class MessageInterceptor : IMessageInterceptor
	{
		private ClientHook ClientHook { get; }
		private GameSettings GameSettings { get; }
		private GameInfo GameInfo { get; }
        public EventHandler OSInformationSent;
        public EventHandler<MessageInterceptor.CommandSendEventArgs> CommandSent;
        public EventHandler LoggedIn;
        public EventHandler<S_Whisper> WhisperReceived;
        public EventHandler<S_StaffChatMessage> StaffChatMessageReceived;
        public EventHandler<S_TribeMessage> TribeMessageReceived;
        public EventHandler<MessageInterceptor.ServerMessageEventArgs> ServerMessageReceived;
        public EventHandler<MessageInterceptor.WindowDisplayMessageEventArgs> WindowDisplayMessageReceived;
        public EventHandler<S_ReportListMessage> ReportListMessageReceived;
        public EventHandler<S_ReportSanctionedMessage> ReportSanctionedMessageReceived;
        public EventHandler<S_ReportDisconnectedMessage> ReportDisconnectedReceived;
        public EventHandler<S_ReportDeletedMessage> ReportDeletedReceived;
        public EventHandler<S_ReportWatcherMessage> ReportWatcherReceived;
        public EventHandler<S_ChatLogMessage> ChatLogReceived;
        public EventHandler<S_ReportRoomUpdatedMessage> ReportRoomUpdateReceived;
        public EventHandler<S_ReportCountsUpdateMessage> ReportCountsUpdateReceived;

        public MessageInterceptor(ClientHook clientHook, GameSettings gameSettings, GameInfo gameInfo)
		{
			this.ClientHook = clientHook;
			this.GameSettings = gameSettings;
			this.GameInfo = gameInfo;
		}

		bool IMessageInterceptor.CommandSent(C_Command command)
		{
			MessageInterceptor.CommandSendEventArgs commandSendEventArgs = new MessageInterceptor.CommandSendEventArgs(command);
			EventHandler<MessageInterceptor.CommandSendEventArgs> commandSent = this.CommandSent;
			if (commandSent != null)
			{
				commandSent(this, commandSendEventArgs);
			}
			return commandSendEventArgs.SendToServer;
		}

		void IMessageInterceptor.OSInformationSent()
		{
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler osinformationSent = this.OSInformationSent;
				if (osinformationSent == null)
				{
					return;
				}
				osinformationSent(this, new EventArgs());
			});
		}

		bool IMessageInterceptor.LoggedIn(S_LoggedIn loggedInMessage)
		{
			string name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(loggedInMessage.Name);
			this.GameInfo.Name = name;
			this.GameInfo.UserId = loggedInMessage.UserId;
			this.GameInfo.Community = (Community)loggedInMessage.Community;
			this.GameInfo.IsArbitre = loggedInMessage.IsArbitre;
			this.GameInfo.IsModerator = loggedInMessage.IsModerator;
			this.GameInfo.IsSentinelle = loggedInMessage.IsSentinelle;
			this.GameInfo.IsAdministrator = loggedInMessage.IsAdministrator;
			this.GameInfo.IsMapCrew = loggedInMessage.IsMapCrew;
			this.GameInfo.IsLuaDev = loggedInMessage.IsLuaDev;
			this.GameInfo.IsFunCorp = loggedInMessage.IsFunCorp;
			this.GameInfo.IsFashionSquad = loggedInMessage.IsFashionSquad;
			EventHandler loggedIn = this.LoggedIn;
			if (loggedIn != null)
			{
				loggedIn(this, new EventArgs());
			}
			return true;
		}

		bool IMessageInterceptor.WhisperReceived(S_Whisper whisperMessage)
		{
			string item = whisperMessage.SenderName.ToLowerInvariant();
			if (this.GameInfo.IgnoreList.Contains(item) || this.GameInfo.TemporaryIgnoreList.Contains(item))
			{
				return false;
			}
			if (this.GameInfo.IgnoredCommunities.Contains(ServerInfo.TribulleCommunities[whisperMessage.Community]))
			{
				return false;
			}
			if (this.GameInfo.AllowedCommunities.Count > 0 && !this.GameInfo.AllowedCommunities.Contains(ServerInfo.TribulleCommunities[whisperMessage.Community]))
			{
				return false;
			}
			S_Whisper uiWhisper = new S_Whisper
			{
				SenderName = whisperMessage.SenderName,
				Community = whisperMessage.Community,
				ReceiverName = whisperMessage.ReceiverName,
				Message = whisperMessage.Message
			};
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler<S_Whisper> whisperReceived = this.WhisperReceived;
				if (whisperReceived == null)
				{
					return;
				}
				whisperReceived(this, uiWhisper);
			});
			return !this.GameSettings.FilterWhispers || !this.GameInfo.IsChatShowing;
		}

		bool IMessageInterceptor.TribeMessageReceived(S_TribeMessage tribeMessage)
		{
			S_TribeMessage uiTribeMessage = new S_TribeMessage
			{
				Name = tribeMessage.Name,
				Message = tribeMessage.Message
			};
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler<S_TribeMessage> tribeMessageReceived = this.TribeMessageReceived;
				if (tribeMessageReceived == null)
				{
					return;
				}
				tribeMessageReceived(this, uiTribeMessage);
			});
			return !this.GameSettings.FilterTribeChat || !this.GameInfo.IsChatShowing;
		}

		bool IMessageInterceptor.ServerMessageReceived(S_ServerMessage serverMessage)
		{
			MessageInterceptor.ServerMessageEventArgs serverMessageEventArgs = new MessageInterceptor.ServerMessageEventArgs(serverMessage);
			EventHandler<MessageInterceptor.ServerMessageEventArgs> serverMessageReceived = this.ServerMessageReceived;
			if (serverMessageReceived != null)
			{
				serverMessageReceived(this, serverMessageEventArgs);
			}
			return serverMessageEventArgs.SendToClient || !this.GameInfo.IsChatShowing;
		}

		bool IMessageInterceptor.StaffChatMessageReceived(S_StaffChatMessage staffChatMessage)
		{
			if (staffChatMessage.GeneralChatMessage)
			{
				Dictionary<string, Player> dictionary;
				if (staffChatMessage.Type == StaffChatType.ArbitreLocal || staffChatMessage.Type == StaffChatType.ArbitreAll)
				{
					dictionary = this.GameInfo.ArbList;
				}
				else
				{
					if (staffChatMessage.Type != StaffChatType.ModerationLocal && staffChatMessage.Type != StaffChatType.ModerationAll)
					{
						return true;
					}
					dictionary = this.GameInfo.ModList;
				}
				if (staffChatMessage.Message.Contains(" just connected."))
				{
					string[] array = staffChatMessage.Message.Split(new char[]
					{
						' '
					});
					if (array.Length > 2)
					{
						Community community;
						Enum.TryParse<Community>(array[1].Substring(1, 2), true, out community);
						string text = array[2].ToLowerInvariant();
						if (dictionary.ContainsKey(text))
						{
							dictionary[text].Community = community;
							dictionary[text].Online = true;
						}
						else
						{
							dictionary.Add(text, new Player
							{
								Name = text,
								Community = community,
								Online = true
							});
						}
					}
				}
				else if (staffChatMessage.Message.Contains(" has disconnected."))
				{
					string[] array2 = staffChatMessage.Message.Split(new char[]
					{
						' '
					});
					if (array2.Length > 1)
					{
						string key = array2[1].ToLowerInvariant();
						if (dictionary.ContainsKey(key))
						{
							dictionary[key].Online = false;
						}
					}
				}
				return true;
			}
			S_StaffChatMessage uiStaffMessage = new S_StaffChatMessage
			{
				Type = staffChatMessage.Type,
				Name = staffChatMessage.Name,
				Message = staffChatMessage.Message,
				GeneralChatMessage = staffChatMessage.GeneralChatMessage
			};
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler<S_StaffChatMessage> staffChatMessageReceived = this.StaffChatMessageReceived;
				if (staffChatMessageReceived == null)
				{
					return;
				}
				staffChatMessageReceived(this, uiStaffMessage);
			});
			switch (staffChatMessage.Type)
			{
			case StaffChatType.ArbitreLocal:
			case StaffChatType.ArbitreAll:
				return !this.GameSettings.FilterArbChat || !this.GameInfo.IsChatShowing;
			case StaffChatType.ModerationLocal:
			case StaffChatType.ModerationAll:
				return !this.GameSettings.FilterModoChat || !this.GameInfo.IsChatShowing;
			case StaffChatType.MapCrew:
				return !this.GameSettings.FilterMapCrewChat || !this.GameInfo.IsChatShowing;
			case StaffChatType.LuaTeam:
				return !this.GameSettings.FilterLuaTeamChat || !this.GameInfo.IsChatShowing;
			case StaffChatType.FunCorp:
				return !this.GameSettings.FilterFunCorpChat || !this.GameInfo.IsChatShowing;
			case StaffChatType.FashionSquad:
				return !this.GameSettings.FilterFashionSquadChat || !this.GameInfo.IsChatShowing;
			}
			return true;
		}

		void IMessageInterceptor.StaffListReceived(S_StaffListMessage staffListMessage)
		{
			Dictionary<string, Player> dictionary;
			if (staffListMessage.Type == StaffChatType.ArbitreAll || staffListMessage.Type == StaffChatType.ArbitreLocal)
			{
				dictionary = this.GameInfo.ArbList;
			}
			else
			{
				if (staffListMessage.Type != StaffChatType.ModerationAll && staffListMessage.Type != StaffChatType.ModerationLocal)
				{
					return;
				}
				dictionary = this.GameInfo.ModList;
			}
			foreach (S_StaffListMessage.StaffMember staffMember in staffListMessage.StaffMembers)
			{
				string text = staffMember.Name.ToLowerInvariant();
				if (dictionary.ContainsKey(text))
				{
					dictionary[text].Community = staffMember.Community;
					dictionary[text].Room = staffMember.Room;
					dictionary[text].Online = true;
				}
				else
				{
					dictionary.Add(text, new Player
					{
						Name = text,
						Community = staffMember.Community,
						Room = staffMember.Room,
						Online = true
					});
				}
			}
		}

		bool IMessageInterceptor.WindowMessageReceiced(S_WindowDisplayMessage windowMessage)
		{
			MessageInterceptor.WindowDisplayMessageEventArgs windowDisplayMessageEventArgs = new MessageInterceptor.WindowDisplayMessageEventArgs(windowMessage);
			EventHandler<MessageInterceptor.WindowDisplayMessageEventArgs> windowDisplayMessageReceived = this.WindowDisplayMessageReceived;
			if (windowDisplayMessageReceived != null)
			{
				windowDisplayMessageReceived(this, windowDisplayMessageEventArgs);
			}
			return windowDisplayMessageEventArgs.SendToClient;
		}

		void IMessageInterceptor.ReportList(S_ReportListMessage reportListMessage)
		{
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler<S_ReportListMessage> reportListMessageReceived = this.ReportListMessageReceived;
				if (reportListMessageReceived == null)
				{
					return;
				}
				reportListMessageReceived(this, reportListMessage);
			});
		}

		void IMessageInterceptor.ReportSanctioned(S_ReportSanctionedMessage reportSanctionedMessage)
		{
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler<S_ReportSanctionedMessage> reportSanctionedMessageReceived = this.ReportSanctionedMessageReceived;
				if (reportSanctionedMessageReceived == null)
				{
					return;
				}
				reportSanctionedMessageReceived(this, reportSanctionedMessage);
			});
		}

		void IMessageInterceptor.ReportDisconnected(S_ReportDisconnectedMessage reportDisconnectedMessage)
		{
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler<S_ReportDisconnectedMessage> reportDisconnectedReceived = this.ReportDisconnectedReceived;
				if (reportDisconnectedReceived == null)
				{
					return;
				}
				reportDisconnectedReceived(this, reportDisconnectedMessage);
			});
		}

		void IMessageInterceptor.ReportDeleted(S_ReportDeletedMessage reportDeletedMessage)
		{
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler<S_ReportDeletedMessage> reportDeletedReceived = this.ReportDeletedReceived;
				if (reportDeletedReceived == null)
				{
					return;
				}
				reportDeletedReceived(this, reportDeletedMessage);
			});
		}

		void IMessageInterceptor.ReportWatcher(S_ReportWatcherMessage reportWatcherMessage)
		{
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler<S_ReportWatcherMessage> reportWatcherReceived = this.ReportWatcherReceived;
				if (reportWatcherReceived == null)
				{
					return;
				}
				reportWatcherReceived(this, reportWatcherMessage);
			});
		}

		bool IMessageInterceptor.ChatLogReceived(S_ChatLogMessage chatLogMessage)
		{
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler<S_ChatLogMessage> chatLogReceived = this.ChatLogReceived;
				if (chatLogReceived == null)
				{
					return;
				}
				chatLogReceived(this, chatLogMessage);
			});
			return !this.GameInfo.IsModopwetShowing;
		}

		void IMessageInterceptor.ReportRoomUpdated(S_ReportRoomUpdatedMessage roomUpdatedMessage)
		{
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler<S_ReportRoomUpdatedMessage> reportRoomUpdateReceived = this.ReportRoomUpdateReceived;
				if (reportRoomUpdateReceived == null)
				{
					return;
				}
				reportRoomUpdateReceived(this, roomUpdatedMessage);
			});
		}

		void IMessageInterceptor.ReportCountsUpdated(S_ReportCountsUpdateMessage reportsCountsMessage)
		{
			TaskHelpers.UiInvoke(delegate
			{
				EventHandler<S_ReportCountsUpdateMessage> reportCountsUpdateReceived = this.ReportCountsUpdateReceived;
				if (reportCountsUpdateReceived == null)
				{
					return;
				}
				reportCountsUpdateReceived(this, reportsCountsMessage);
			});
		}

		void IMessageInterceptor.FriendConnected(Player friend)
		{
			string text = friend.Name.ToLowerInvariant();
			if (this.GameInfo.FriendList.ContainsKey(text))
			{
				this.GameInfo.FriendList[text].Id = friend.Id;
				this.GameInfo.FriendList[text].Room = friend.Room;
				this.GameInfo.FriendList[text].Online = friend.Online;
				return;
			}
			this.GameInfo.FriendList.Add(text, new Player
			{
				Name = text,
				Id = friend.Id,
				Room = friend.Room,
				Online = friend.Online
			});
		}

		void IMessageInterceptor.IgnoreListReceived(IEnumerable<string> names)
		{
			this.GameInfo.IgnoreList.Clear();
			foreach (string text in names)
			{
				this.GameInfo.IgnoreList.Add(text.ToLowerInvariant());
			}
		}

		void IMessageInterceptor.PlayerIgnored(string name)
		{
			this.GameInfo.IgnoreList.Add(name.ToLowerInvariant());
		}

		public sealed class CommandSendEventArgs : EventArgs
		{
			public C_Command Message { get; }

            public bool SendToServer
            {
                get => _sendToServer;
                set => _sendToServer = _sendToServer && value;
            }

            public CommandSendEventArgs(C_Command message)
			{
				this.Message = message;
				this._sendToServer = true;
			}

			private bool _sendToServer;
		}

		public sealed class ServerMessageEventArgs : EventArgs
		{
			public S_ServerMessage Message { get; }
            public bool SendToClient
            {
                get => _sendToClient;
                set => _sendToClient = _sendToClient && value;
            }

            public ServerMessageEventArgs(S_ServerMessage message)
			{
				this.Message = message;
				this._sendToClient = true;
			}

			private bool _sendToClient;
		}

		public sealed class WindowDisplayMessageEventArgs : EventArgs
		{
			public S_WindowDisplayMessage Message { get; }

			public bool SendToClient
			{
				get
				{
					return this._sendToClient;
				}
				set
				{
					this._sendToClient = (this._sendToClient && value);
				}
			}

			public WindowDisplayMessageEventArgs(S_WindowDisplayMessage message)
			{
				this.Message = message;
				this._sendToClient = true;
			}

			private bool _sendToClient;
		}
	}
}
