using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using tfmClientHook;
using tfmClientHook.Messages;

namespace tfmStandalone
{
	public sealed class ChatModel
	{
		private ClientHook ClientHook { get; }
		private MessageInterceptor MessageInterceptor { get; }
		private GameInfo GameInfo { get; }
		private GameSettings GameSettings { get; }
		private CommandService CommandService { get; }
        private static readonly string ExePath = AppDomain.CurrentDomain.BaseDirectory;
        public EventHandler<WhisperEventArgs> WhisperSent;
        public EventHandler<WhisperEventArgs> WhisperReceived;
        public EventHandler<StaffChatReceivedEventArgs> StaffChatReceived;
        public EventHandler<TribeChatReceivedEventArgs> TribeChatReceived;
        public EventHandler<ServerMessageReceivedEventArgs> ServerMessageReceived;

        public ChatModel(ClientHook clientHook, MessageInterceptor messageInterceptor, GameInfo gameInfo, GameSettings gameSettings, CommandService commandService)
		{
			this.ClientHook = clientHook;
			this.MessageInterceptor = messageInterceptor;
			this.GameInfo = gameInfo;
			this.GameSettings = gameSettings;
			this.CommandService = commandService;
			MessageInterceptor messageInterceptor2 = this.MessageInterceptor;
			messageInterceptor2.LoggedIn = (EventHandler)Delegate.Combine(messageInterceptor2.LoggedIn, new EventHandler(this.LoggedIn));
			MessageInterceptor messageInterceptor3 = this.MessageInterceptor;
			messageInterceptor3.WhisperReceived = (EventHandler<S_Whisper>)Delegate.Combine(messageInterceptor3.WhisperReceived, new EventHandler<S_Whisper>(this.WhisperMessageReceived));
			MessageInterceptor messageInterceptor4 = this.MessageInterceptor;
			messageInterceptor4.StaffChatMessageReceived = (EventHandler<S_StaffChatMessage>)Delegate.Combine(messageInterceptor4.StaffChatMessageReceived, new EventHandler<S_StaffChatMessage>(this.StaffChatMessageReceived));
			MessageInterceptor messageInterceptor5 = this.MessageInterceptor;
			messageInterceptor5.TribeMessageReceived = (EventHandler<S_TribeMessage>)Delegate.Combine(messageInterceptor5.TribeMessageReceived, new EventHandler<S_TribeMessage>(this.TribeMessageReceived));
			MessageInterceptor messageInterceptor6 = this.MessageInterceptor;
			messageInterceptor6.ServerMessageReceived = (EventHandler<MessageInterceptor.ServerMessageEventArgs>)Delegate.Combine(messageInterceptor6.ServerMessageReceived, new EventHandler<MessageInterceptor.ServerMessageEventArgs>(this.ServerMsgReceived));
		}

		private void LoggedIn(object sender, EventArgs eventArgs)
		{
			if (!Directory.Exists(ChatModel.ExePath + "Chat Logs (" + this.GameInfo.Name + ")"))
			{
				Directory.CreateDirectory(ChatModel.ExePath + "Chat Logs (" + this.GameInfo.Name + ")");
			}
			if (!Directory.Exists(ChatModel.ExePath + "User Notes"))
			{
				Directory.CreateDirectory(ChatModel.ExePath + "User Notes");
			}
		}

		public void SendWhisper(string name, string message)
		{
			this.ClientHook.SendToServer(ConnectionType.Main, new C_Whisper
			{
				Name = name,
				Message = message
			});
		}

		public void SendStaffChat(StaffChatType type, string message)
		{
			this.ClientHook.SendToServer(ConnectionType.Main, new C_StaffChatMessage
			{
				Type = type,
				Message = message
			});
		}

		public void SendTribeChat(string message)
		{
			this.ClientHook.SendToServer(ConnectionType.Main, new C_TribeMessage
			{
				Message = message
			});
		}

		public void SendCommand(string command)
		{
			this.ClientHook.SendToServer(ConnectionType.Main, new C_Command
			{
				Command = command
			});
		}

		public void SendClientCommand(string command)
		{
			this.CommandService.ProcessCommand(command);
		}

		public IEnumerable<string> GetChatHistory(string name, int messageCount)
		{
			if (!File.Exists(string.Concat(new string[]
			{
				ChatModel.ExePath,
				"Chat Logs (",
				this.GameInfo.Name,
				")\\",
				name,
				".txt"
			})))
			{
				return null;
			}
			string[] array = File.ReadAllLines(string.Concat(new string[]
			{
				ChatModel.ExePath,
				"Chat Logs (",
				this.GameInfo.Name,
				")\\",
				name,
				".txt"
			}));
			if (array.Length <= messageCount)
			{
				return array;
			}
			string[] array2 = new string[messageCount];
			for (int i = 0; i < messageCount; i++)
			{
				array2[i] = array[array.Length - messageCount + i];
			}
			return array2;
		}

		private void WhisperMessageReceived(object sender, S_Whisper sWhisper)
		{
			bool flag = sWhisper.SenderName.ToLowerInvariant() == this.GameInfo.Name.ToLowerInvariant();
			string text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(flag ? sWhisper.ReceiverName : sWhisper.SenderName);
			string str = string.Concat(new string[]
			{
				"] [",
				ServerInfo.TribulleCommunities[sWhisper.Community],
				"] [",
				CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sWhisper.SenderName),
				"] ",
				sWhisper.Message.Replace("&lt;", "<").Replace("&amp;", "&")
			});
			string message = "• [" + DateTime.Now.ToString("H:mm") + str;
			string message2 = "• [" + DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat) + str;
			WhisperEventArgs e = new WhisperEventArgs
			{
				Name = text,
				Message = message,
				Community = sWhisper.Community
			};
			if (flag)
			{
				EventHandler<WhisperEventArgs> whisperSent = this.WhisperSent;
				if (whisperSent != null)
				{
					whisperSent(this, e);
				}
			}
			else
			{
				EventHandler<WhisperEventArgs> whisperReceived = this.WhisperReceived;
				if (whisperReceived != null)
				{
					whisperReceived(this, e);
				}
			}
			if (this.GameSettings.LogWhispers)
			{
				this.LogMessage(text, message2);
			}
		}

		private void StaffChatMessageReceived(object sender, S_StaffChatMessage staffChatMessage)
		{
			string text = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(staffChatMessage.Name);
			string str = staffChatMessage.Message.Replace("&lt;", "<").Replace("&amp;", "&");
			string message;
			string message2;
			if (staffChatMessage.Type == StaffChatType.ArbitreAll || staffChatMessage.Type == StaffChatType.ModerationAll)
			{
				string str2 = "] [" + text + "] [All] " + str;
				message = "• [" + DateTime.Now.ToString("H:mm") + str2;
				message2 = "• [" + DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat) + str2;
			}
			else
			{
				string str3 = "] [" + text + "] " + str;
				message = "• [" + DateTime.Now.ToString("H:mm") + str3;
				message2 = "• [" + DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat) + str3;
			}
			StaffChatReceivedEventArgs e = new StaffChatReceivedEventArgs
			{
				Type = staffChatMessage.Type,
				Name = text,
				Message = message,
				GeneralChatMessage = staffChatMessage.GeneralChatMessage
			};
			EventHandler<StaffChatReceivedEventArgs> staffChatReceived = this.StaffChatReceived;
			if (staffChatReceived != null)
			{
				staffChatReceived(this, e);
			}
			string fileName;
			switch (staffChatMessage.Type)
			{
			case StaffChatType.ArbitreLocal:
			case StaffChatType.ArbitreAll:
				if (!this.GameSettings.LogArbChat)
				{
					return;
				}
				fileName = "Arbitre";
				break;
			case StaffChatType.ModerationLocal:
			case StaffChatType.ModerationAll:
				if (!this.GameSettings.LogModoChat)
				{
					return;
				}
				fileName = "Modo";
				break;
			case StaffChatType.Global:
				return;
			case StaffChatType.MapCrew:
				if (!this.GameSettings.LogMapCrewChat)
				{
					return;
				}
				fileName = "MapCrew";
				break;
			case StaffChatType.LuaTeam:
				if (!this.GameSettings.LogLuaTeamChat)
				{
					return;
				}
				fileName = "LuaTeam";
				break;
			case StaffChatType.FunCorp:
				if (!this.GameSettings.LogFunCorpChat)
				{
					return;
				}
				fileName = "FunCorp";
				break;
			case StaffChatType.FashionSquad:
				if (!this.GameSettings.LogFashionSquadChat)
				{
					return;
				}
				fileName = "FashionSquad";
				break;
			default:
				return;
			}
			this.LogMessage(fileName, message2);
		}

		private void TribeMessageReceived(object sender, S_TribeMessage tribeMessage)
		{
			string str = "] [" + CultureInfo.InvariantCulture.TextInfo.ToTitleCase(tribeMessage.Name) + "] " + tribeMessage.Message.Replace("&lt;", "<").Replace("&amp;", "&");
			string message = "• [" + DateTime.Now.ToString("H:mm") + str;
			string message2 = "• [" + DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat) + str;
			EventHandler<TribeChatReceivedEventArgs> tribeChatReceived = this.TribeChatReceived;
			if (tribeChatReceived != null)
			{
				tribeChatReceived(this, new TribeChatReceivedEventArgs
				{
					Message = message
				});
			}
			if (this.GameSettings.LogTribeChat)
			{
				this.LogMessage("Tribe", message2);
			}
		}

		private void ServerMsgReceived(object sender, MessageInterceptor.ServerMessageEventArgs e)
		{
			e.SendToClient = (!this.GameSettings.FilterServeurMessages || e.Message.IsSelf);
			TaskHelpers.UiInvoke(delegate
			{
				bool flag = e.Message.Message.IndexOf("Buffy banned", StringComparison.Ordinal) == 0;
				string message = "• [" + DateTime.Now.ToString("H:mm") + "] " + e.Message.Message;
				string text = "• [" + DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat) + "] " + e.Message.Message;
				EventHandler<ServerMessageReceivedEventArgs> serverMessageReceived = this.ServerMessageReceived;
				if (serverMessageReceived != null)
				{
					serverMessageReceived(this, new ServerMessageReceivedEventArgs
					{
						IsBuffyBan = flag,
						Message = message
					});
				}
				if (!flag && this.GameSettings.LogServeurMessages)
				{
					this.LogMessage("Server", text);
				}
				if (e.Message.Message.IndexOf(this.GameInfo.Name, StringComparison.Ordinal) != -1)
				{
					using (StreamWriter streamWriter = File.AppendText(ChatModel.ExePath + "Log.txt"))
					{
						streamWriter.WriteLine(text);
					}
				}
			});
		}

		private void LogMessage(string fileName, string message)
		{
			using (StreamWriter streamWriter = File.AppendText(string.Concat(new string[]
			{
				ChatModel.ExePath,
				"Chat Logs (",
				this.GameInfo.Name,
				")\\",
				fileName,
				".txt"
			})))
			{
				streamWriter.WriteLine(message);
			}
		}
	}
}
