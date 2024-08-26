using System;
using System.Collections.Generic;
using System.Globalization;
using tfmClientHook.Messages;

namespace tfmClientHook
{
	internal static class MessageProcessor
	{
		public static S_Message ServerProcessMessage(ProxyConnection proxyConnection, IMessageInterceptor interceptor, ByteBuffer buffer, ref bool sendToClient)
		{
			byte b = buffer[0];
			byte b2 = buffer[1];
			S_Message result;
			if (b == 1 && b2 == 1)
			{
				result = null;
			}
			else if (b == 60 && b2 == 3)
			{
				ByteBuffer byteBuffer = new ByteBuffer();
				byteBuffer.WriteBytes(buffer.GetBytes());
				byteBuffer.ReadBytes(2);
				result = MessageProcessor.ServerProcessTribulle(interceptor, byteBuffer.ReadShort(), byteBuffer, ref sendToClient);
			}
			else
			{
				ByteBuffer byteBuffer2 = new ByteBuffer();
				byteBuffer2.WriteBytes(buffer.GetBytes());
				byteBuffer2.ReadBytes(2);
				result = MessageProcessor.ServerProcessNewProtocol(proxyConnection, interceptor, b, b2, byteBuffer2, ref sendToClient);
			}
			return result;
		}

		private static S_Message ServerProcessOldProtocol(IMessageInterceptor interceptor, byte c, byte cc, Queue<string> messageParams, ref bool sendToClient)
		{
			return null;
		}

		private static S_Message ServerProcessTribulle(IMessageInterceptor interceptor, short tribulleId, ByteBuffer byteBuffer, ref bool sendToClient)
		{
			S_Message result = null;
			if (tribulleId == ServerInfo.IncomingTribulleCodes.WhisperMessage)
			{
				S_Whisper s_Whisper = new S_Whisper
				{
					SenderName = byteBuffer.ReadString(),
					Community = byteBuffer.ReadInt(),
					ReceiverName = byteBuffer.ReadString(),
					Message = byteBuffer.ReadString()
				};
				sendToClient = interceptor.WhisperReceived(s_Whisper);
				result = s_Whisper;
			}
			else if (tribulleId == ServerInfo.IncomingTribulleCodes.TribeMessage)
			{
				S_TribeMessage s_TribeMessage = new S_TribeMessage
				{
					Name = byteBuffer.ReadString(),
					Message = byteBuffer.ReadString()
				};
				sendToClient = interceptor.TribeMessageReceived(s_TribeMessage);
				result = s_TribeMessage;
			}
			return result;
		}

		private static S_Message ServerProcessNewProtocol(ProxyConnection proxyConnection, IMessageInterceptor interceptor, byte c, byte cc, ByteBuffer byteBuffer, ref bool sendToClient)
		{
			S_Message result = null;
			if (c == 6)
			{
				if (cc == 10)
				{
					S_StaffChatMessage s_StaffChatMessage = new S_StaffChatMessage
					{
						Type = (StaffChatType)byteBuffer.ReadByte(),
						Name = byteBuffer.ReadString(),
						Message = byteBuffer.ReadString(),
						GeneralChatMessage = byteBuffer.ReadBool()
					};
					if (byteBuffer.Count > 0)
					{
						s_StaffChatMessage.ExtraBytes = byteBuffer.ReadBytes(byteBuffer.Count);
					}
					sendToClient = interceptor.StaffChatMessageReceived(s_StaffChatMessage);
					result = s_StaffChatMessage;
				}
				else if (cc == 20)
				{
					S_ServerMessage s_ServerMessage = new S_ServerMessage
					{
						IsSelf = byteBuffer.ReadBool(),
						Message = byteBuffer.ReadString()
					};
					byte b = byteBuffer.ReadByte();
					for (int i = 0; i < (int)b; i++)
					{
						s_ServerMessage.TranslationParameters.Add(byteBuffer.ReadString());
					}
					sendToClient = interceptor.ServerMessageReceived(s_ServerMessage);
					result = s_ServerMessage;
				}
			}
			else if (c == 26)
			{
				if (cc == 2)
				{
					Console.WriteLine("Reached Login session");
					S_LoggedIn s_LoggedIn = new S_LoggedIn();
					s_LoggedIn.UserId = byteBuffer.ReadInt();
					s_LoggedIn.Name = byteBuffer.ReadString();
					s_LoggedIn.TimePlayed = byteBuffer.ReadInt();
					s_LoggedIn.Community = byteBuffer.ReadByte();
					s_LoggedIn.PlayerSessionId = byteBuffer.ReadInt();
					s_LoggedIn.IsPlayerAccount = byteBuffer.ReadBool();
					s_LoggedIn.Roles = new List<byte>();
					byte b2 = byteBuffer.ReadByte();
					for (int j = 0; j < (int)b2; j++)
					{
						s_LoggedIn.Roles.Add(byteBuffer.ReadByte());
					}
					s_LoggedIn.HasPublicAuthorization = byteBuffer.ReadBool();
					sendToClient = interceptor.LoggedIn(s_LoggedIn);
				}
			}
			else if (c == 24)
			{
				if (cc == 5)
				{
					S_StaffListMessage s_StaffListMessage = new S_StaffListMessage
					{
						Type = (StaffChatType)byteBuffer.ReadByte()
					};
					byte b3 = byteBuffer.ReadByte();
					s_StaffListMessage.StaffMembers = new List<S_StaffListMessage.StaffMember>();
					for (int k = 0; k < (int)b3; k++)
					{
						s_StaffListMessage.StaffMembers.Add(new S_StaffListMessage.StaffMember
						{
							Community = (Community)Enum.Parse(typeof(Community), byteBuffer.ReadString(), true),
							Name = byteBuffer.ReadString(),
							Room = byteBuffer.ReadString()
						});
					}
					interceptor.StaffListReceived(s_StaffListMessage);
				}
			}
			else if (c == 25)
			{
				if (cc == 2)
				{
					S_ReportListMessage s_ReportListMessage = new S_ReportListMessage
					{
						PlayerReports = new List<S_ReportListMessage.PlayerReport>()
					};
					byte b4 = byteBuffer.ReadByte();
					for (int l = 0; l < (int)b4; l++)
					{
						S_ReportListMessage.PlayerReport playerReport = new S_ReportListMessage.PlayerReport();
						playerReport.IsGeneral = byteBuffer.ReadBool();
						playerReport.UnknownByte1 = byteBuffer.ReadByte();
						playerReport.UnknownByte2 = byteBuffer.ReadByte();
						playerReport.Community = byteBuffer.ReadString();
						playerReport.PlayerName = byteBuffer.ReadString();
						playerReport.PlayerRoom = byteBuffer.ReadString();
						playerReport.Watchers = new List<string>();
						byte b5 = byteBuffer.ReadByte();
						for (int m = 0; m < (int)b5; m++)
						{
							playerReport.Watchers.Add(byteBuffer.ReadString());
						}
						playerReport.PlayerHours = byteBuffer.ReadInt();
						playerReport.Reports = new List<S_ReportListMessage.Report>();
						byte b6 = byteBuffer.ReadByte();
						for (int n = 0; n < (int)b6; n++)
						{
							S_ReportListMessage.Report item = new S_ReportListMessage.Report
							{
								ReporterName = byteBuffer.ReadString(),
								ReporterKarma = byteBuffer.ReadShort(),
								Comment = byteBuffer.ReadString(),
								Type = (ReportType)byteBuffer.ReadByte(),
								Age = byteBuffer.ReadShort()
							};
							playerReport.Reports.Add(item);
						}
						playerReport.IsMuted = byteBuffer.ReadBool();
						if (playerReport.IsMuted)
						{
							playerReport.Muter = byteBuffer.ReadString();
							playerReport.MuteLength = byteBuffer.ReadShort();
							playerReport.MuteReason = byteBuffer.ReadString();
						}
						s_ReportListMessage.PlayerReports.Add(playerReport);
					}
					interceptor.ReportList(s_ReportListMessage);
				}
				else if (cc == 3)
				{
					S_ReportCountsUpdateMessage s_ReportCountsUpdateMessage = new S_ReportCountsUpdateMessage
					{
						ReportCounts = new List<S_ReportCountsUpdateMessage.ReportCount>()
					};
					byte b7 = byteBuffer.ReadByte();
					for (int num = 0; num < (int)b7; num++)
					{
						s_ReportCountsUpdateMessage.ReportCounts.Add(new S_ReportCountsUpdateMessage.ReportCount
						{
							Community = (Community)byteBuffer.ReadByte(),
							Count = (int)byteBuffer.ReadByte()
						});
					}
					interceptor.ReportCountsUpdated(s_ReportCountsUpdateMessage);
				}
				else if (cc == 4)
				{
					S_ReportRoomUpdatedMessage roomUpdatedMessage = new S_ReportRoomUpdatedMessage
					{
						Name = byteBuffer.ReadString(),
						Room = byteBuffer.ReadString(),
						IsPassworded = byteBuffer.ReadBool()
					};
					interceptor.ReportRoomUpdated(roomUpdatedMessage);
				}
				else if (cc == 5)
				{
					S_ReportSanctionedMessage reportSanctionedMessage = new S_ReportSanctionedMessage
					{
						Name = byteBuffer.ReadString(),
						IsBan = byteBuffer.ReadBool(),
						SanctionGiver = byteBuffer.ReadString(),
						SanctionLength = byteBuffer.ReadInt(),
						SanctionReason = byteBuffer.ReadString()
					};
					interceptor.ReportSanctioned(reportSanctionedMessage);
				}
				else if (cc == 6)
				{
					S_ReportDisconnectedMessage reportDisconnectedMessage = new S_ReportDisconnectedMessage
					{
						Name = byteBuffer.ReadString()
					};
					interceptor.ReportDisconnected(reportDisconnectedMessage);
				}
				else if (cc == 7)
				{
					S_ReportDeletedMessage reportDeletedMessage = new S_ReportDeletedMessage
					{
						Name = byteBuffer.ReadString(),
						Deleter = byteBuffer.ReadString()
					};
					interceptor.ReportDeleted(reportDeletedMessage);
				}
				else if (cc == 8)
				{
					S_ReportWatcherMessage s_ReportWatcherMessage = new S_ReportWatcherMessage
					{
						Name = byteBuffer.ReadString(),
						Watchers = new List<string>()
					};
					byte b8 = byteBuffer.ReadByte();
					for (int num2 = 0; num2 < (int)b8; num2++)
					{
						s_ReportWatcherMessage.Watchers.Add(byteBuffer.ReadString());
					}
					interceptor.ReportWatcher(s_ReportWatcherMessage);
				}
				else if (cc == 10)
				{
					S_ChatLogMessage s_ChatLogMessage = new S_ChatLogMessage
					{
						Name = byteBuffer.ReadString(),
						Messages = new List<S_ChatLogMessage.Message>()
					};
					byte b9 = byteBuffer.ReadByte();
					for (int num3 = 0; num3 < (int)b9; num3++)
					{
						S_ChatLogMessage.Message message = new S_ChatLogMessage.Message
						{
							Text = byteBuffer.ReadString()
						};
						string[] array = byteBuffer.ReadString().Split(new string[]
						{
							" GMT"
						}, StringSplitOptions.None);
						message.Time = DateTime.ParseExact(array[0] + " " + array[1].Insert(3, ":"), "yyyy/MM/dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
						s_ChatLogMessage.Messages.Add(message);
					}
					sendToClient = interceptor.ChatLogReceived(s_ChatLogMessage);
				}
			}
			else if (c == 28 && cc == 46)
			{
				S_WindowDisplayMessage windowMessage = new S_WindowDisplayMessage
				{
					FontStyle = byteBuffer.ReadByte(),
					WindowKey = byteBuffer.ReadString(),
					Text = byteBuffer.ReadLongString()
				};
				sendToClient = interceptor.WindowMessageReceiced(windowMessage);
			}
			return result;
		}

		public static C_Message ClientProcessMessage(IMessageInterceptor interceptor, ByteBuffer buffer, int currentGameFingerprint, ref bool sendToServer)
		{
			byte b = buffer[0];
			byte b2 = buffer[1];
			C_Message result = null;
			if (b == 60)
			{
				if (b2 == 1)
				{
					ByteBuffer byteBuffer = new ByteBuffer();
					byteBuffer.WriteBytes(buffer.GetBytes());
					byteBuffer.ReadBytes(2);
					MessageProcessor.DecryptMessage(byteBuffer, currentGameFingerprint);
					byteBuffer.PrependByte(b2);
					byteBuffer.PrependByte(b);
					C_Generic c_Generic = new C_Generic(true);
					c_Generic.ByteBuffer.WriteBytes(byteBuffer.GetBytes());
					result = c_Generic;
				}
				else if (b2 == 3)
				{
					ByteBuffer byteBuffer2 = new ByteBuffer();
					byteBuffer2.WriteBytes(buffer.GetBytes());
					byteBuffer2.ReadBytes(2);
					MessageProcessor.DecryptMessage(byteBuffer2, currentGameFingerprint);
					byteBuffer2.PrependByte(b2);
					byteBuffer2.PrependByte(b);
					C_Generic c_Generic2 = new C_Generic(true);
					c_Generic2.ByteBuffer.WriteBytes(byteBuffer2.GetBytes());
					result = c_Generic2;
				}
			}
			else
			{
				ByteBuffer byteBuffer3 = new ByteBuffer();
				byteBuffer3.WriteBytes(buffer.GetBytes());
				byteBuffer3.ReadBytes(2);
				result = MessageProcessor.ClientProcessNewProtocol(interceptor, b, b2, byteBuffer3, currentGameFingerprint, ref sendToServer);
			}
			return result;
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x00015F70 File Offset: 0x00014170
		private static C_Message ClientProcessNewProtocol(IMessageInterceptor interceptor, byte c, byte cc, ByteBuffer byteBuffer, int currentGameFingerprint, ref bool sendToServer)
		{
			C_Message result = null;
			if (c == 6)
			{
				if (cc == 26)
				{
					MessageProcessor.DecryptMessage(byteBuffer, currentGameFingerprint);
					C_Command c_Command = new C_Command
					{
						Command = byteBuffer.ReadString()
					};
					sendToServer = interceptor.CommandSent(c_Command);
					result = c_Command;
				}
			}
			else if (c == 26)
			{
				if (cc == 7)
				{
					MessageProcessor.DecryptMessage(byteBuffer, currentGameFingerprint);
					C_Generic c_Generic = new C_Generic(true);
					c_Generic.ByteBuffer.WriteByte(26);
					c_Generic.ByteBuffer.WriteByte(7);
					c_Generic.ByteBuffer.WriteString(byteBuffer.ReadString());
					c_Generic.ByteBuffer.WriteByte(byteBuffer.ReadByte());
					c_Generic.ByteBuffer.WriteString(byteBuffer.ReadString());
					c_Generic.ByteBuffer.WriteString(byteBuffer.ReadString());
					result = c_Generic;
				}
				else if (cc == 8)
				{
				}
			}
			else if (c == 28)
			{
				if (cc == 1)
				{
					Console.WriteLine("Loader size sent.");
					C_ConnectionMessage c_ConnectionMessage = new C_ConnectionMessage();
					c_ConnectionMessage.Version = byteBuffer.ReadShort();
					c_ConnectionMessage.Language = byteBuffer.ReadString();
					c_ConnectionMessage.Key = byteBuffer.ReadString();
					c_ConnectionMessage.PlayerType = byteBuffer.ReadString();
					c_ConnectionMessage.Browser = byteBuffer.ReadString();
					c_ConnectionMessage.LoaderSize = 6125;
					byteBuffer.ReadInt();
					byteBuffer.ReadString();
					c_ConnectionMessage.Fonts = byteBuffer.ReadString();
					c_ConnectionMessage.ServerString = byteBuffer.ReadString();
					c_ConnectionMessage.ReferrerId = byteBuffer.ReadInt();
					c_ConnectionMessage.CurrentTime = byteBuffer.ReadInt();
					result = c_ConnectionMessage;
				}
				else if (cc == 17)
				{
					interceptor.OSInformationSent();
				}
			}
			return result;
		}

		private static void DecryptMessage(ByteBuffer byteBuffer, int currentGameFingerprint)
		{
			ByteBuffer byteBuffer2 = new ByteBuffer();
			byteBuffer2.WriteBytes(byteBuffer.ReadBytes(byteBuffer.Count));
			int num = ServerInfo.EncryptionVector.Length;
			int[] encryptionVector = ServerInfo.EncryptionVector;
			while (byteBuffer2.Count > 0)
			{
				currentGameFingerprint = (currentGameFingerprint + 1) % num;
				byteBuffer.WriteByte((byte)((int)byteBuffer2.ReadByte() ^ ServerInfo.EncryptionVector[currentGameFingerprint]));
			}
		}
	}
}
