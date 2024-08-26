using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Commands;
using Prism.Mvvm;
using tfmClientHook;
using tfmClientHook.Messages;

namespace tfmStandalone
{
	public sealed class VpnFarmingRoomWindowViewModel : BindableBase
	{
		public Dictionary<string, RoomViewModel> RoomDictionary { get; }
		public ObservableCollection<RoomViewModel> Rooms { get; }
		public DelegateCommand FindRacingRoomsCommand { get; }
		public DelegateCommand FindBootcampRoomsCommand { get; }
		public DelegateCommand<string> CopyRoomCommand { get; }
		public DelegateCommand<string> LsRoomCommand { get; }
		public DelegateCommand<string> ClearMembersCommand { get; }
		public DelegateCommand<RoomViewModel> RemoveRoomCommand { get; }
		public DelegateCommand<string> NinjaPlayerCommand { get; }
		private MessageInterceptor MessageInterceptor { get; }

		public VpnFarmingRoomWindowViewModel(ClientHook clientHook, MessageInterceptor messageInterceptor)
		{
			this.MessageInterceptor = messageInterceptor;
			MessageInterceptor messageInterceptor2 = this.MessageInterceptor;
			messageInterceptor2.ServerMessageReceived = (EventHandler<MessageInterceptor.ServerMessageEventArgs>)Delegate.Combine(messageInterceptor2.ServerMessageReceived, new EventHandler<MessageInterceptor.ServerMessageEventArgs>(this.ServerMessageReceived));
			MessageInterceptor messageInterceptor3 = this.MessageInterceptor;
			messageInterceptor3.WindowDisplayMessageReceived = (EventHandler<MessageInterceptor.WindowDisplayMessageEventArgs>)Delegate.Combine(messageInterceptor3.WindowDisplayMessageReceived, new EventHandler<MessageInterceptor.WindowDisplayMessageEventArgs>(this.WindowDisplayMessageReceived));
			this.Rooms = new ObservableCollection<RoomViewModel>();
			this.RoomDictionary = new Dictionary<string, RoomViewModel>();
			this.FindRacingRoomsCommand = new DelegateCommand(delegate()
			{
				clientHook.SendToServer(ConnectionType.Main, new C_Command
				{
					Command = "ls racing"
				});
			});
			this.FindBootcampRoomsCommand = new DelegateCommand(delegate()
			{
				clientHook.SendToServer(ConnectionType.Main, new C_Command
				{
					Command = "ls bootcamp"
				});
			});
			this.CopyRoomCommand = new DelegateCommand<string>(new Action<string>(Clipboard.SetText));
			this.LsRoomCommand = new DelegateCommand<string>(delegate(string roomName)
			{
				clientHook.SendToServer(ConnectionType.Main, new C_Command
				{
					Command = string.Format("lsroom {0}", roomName)
				});
			});
			this.ClearMembersCommand = new DelegateCommand<string>(delegate(string roomName)
			{
				if (this.RoomDictionary.ContainsKey(roomName))
				{
					this.RoomDictionary[roomName].Members.Clear();
				}
			});
			this.RemoveRoomCommand = new DelegateCommand<RoomViewModel>(delegate(RoomViewModel room)
			{
				this.Rooms.Remove(room);
			});
			this.NinjaPlayerCommand = new DelegateCommand<string>(delegate(string playerName)
			{
				clientHook.SendToServer(ConnectionType.Main, new C_Command
				{
					Command = string.Format("ninja {0}", playerName)
				});
			});
			clientHook.SendToServer(ConnectionType.Main, new C_Command
			{
				Command = "ls racing"
			});
		}

		public void Closing()
		{
			MessageInterceptor messageInterceptor = this.MessageInterceptor;
			messageInterceptor.ServerMessageReceived = (EventHandler<MessageInterceptor.ServerMessageEventArgs>)Delegate.Remove(messageInterceptor.ServerMessageReceived, new EventHandler<MessageInterceptor.ServerMessageEventArgs>(this.ServerMessageReceived));
			MessageInterceptor messageInterceptor2 = this.MessageInterceptor;
			messageInterceptor2.WindowDisplayMessageReceived = (EventHandler<MessageInterceptor.WindowDisplayMessageEventArgs>)Delegate.Remove(messageInterceptor2.WindowDisplayMessageReceived, new EventHandler<MessageInterceptor.WindowDisplayMessageEventArgs>(this.WindowDisplayMessageReceived));
		}

		private void ServerMessageReceived(object o, MessageInterceptor.ServerMessageEventArgs e)
		{
			if (e.Message.Message.Contains("Players in room ["))
			{
				TaskHelpers.UiInvoke(delegate
				{
					this.LsRoomMessageReceived(e.Message.Message);
				});
			}
		}

		private void WindowDisplayMessageReceived(object o, MessageInterceptor.WindowDisplayMessageEventArgs e)
		{
			if (e.Message.Text.StartsWith("List of rooms matching [racing]:"))
			{
				e.SendToClient = false;
				TaskHelpers.UiInvoke(delegate
				{
					this.FindRooms(false, e.Message.Text);
				});
				return;
			}
			if (e.Message.Text.StartsWith("List of rooms matching [bootcamp]:"))
			{
				e.SendToClient = false;
				TaskHelpers.UiInvoke(delegate
				{
					this.FindRooms(true, e.Message.Text);
				});
			}
		}

		private void FindRooms(bool isBootcamp, string lsResults)
		{
			this.RoomDictionary.Clear();
			this.Rooms.Clear();
			List<RoomViewModel> list = new List<RoomViewModel>();
			string[] array = lsResults.Split(new string[]
			{
				"\r\n",
				"\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			Regex regex = new Regex("(^|\\s)racing[1-9](\\s|$)");
			Regex regex2 = new Regex("(^|\\s)bootcamp[1-9](\\s|$)");
			foreach (string input in array)
			{
				try
				{
					Match match = Regex.Match(input, "<BL>(?<roomName>.+)</BL> <G>\\((?<community>.{2}) / (?<bulle>[^\\s]+) / (?<otherNumber>[^\\s]+)\\) : <V>(?<playerCount>.+)</V></G>");
					if (match.Success)
					{
						string value = match.Groups["roomName"].Value;
						string value2 = match.Groups["community"].Value;
						string value3 = match.Groups["bulle"].Value;
						string value4 = match.Groups["playerCount"].Value;
						RoomViewModel roomViewModel = new RoomViewModel
						{
							Count = int.Parse(value4),
							Community = value2,
							FullName = value
						};
						roomViewModel.RoomName = roomViewModel.FullName.Substring((roomViewModel.FullName[0] == '*') ? 1 : 3);
						if (!isBootcamp || (roomViewModel.Count >= 4 && (roomViewModel.IsInternational || !regex2.IsMatch(roomViewModel.RoomName))))
						{
							if (isBootcamp || (roomViewModel.Count >= 11 && (roomViewModel.IsInternational || !regex.IsMatch(roomViewModel.RoomName))))
							{
								list.Add(roomViewModel);
								this.RoomDictionary.Add(roomViewModel.FullName, roomViewModel);
							}
						}
					}
				}
				catch
				{
				}
			}
			EnumerableExtensions.ForEach<RoomViewModel>(list.OrderBy(delegate(RoomViewModel r)
			{
				if (!r.IsInternational)
				{
					return 1;
				}
				return 0;
			}).ThenBy((RoomViewModel r) => r.Count), delegate(RoomViewModel r)
			{
				this.Rooms.Add(r);
			});
		}

		private void LsRoomMessageReceived(string lsroomResults)
		{
			List<string> list = lsroomResults.Split(new string[]
			{
				"\r\n",
				"\n"
			}, StringSplitOptions.None).ToList<string>();
			string value = Regex.Match(list[0], "\\[([^]]*)\\]").Groups[1].Value;
			if (this.RoomDictionary.ContainsKey(value))
			{
				list.RemoveAt(0);
				this.RoomDictionary[value].Count = list.Count;
				this.RoomDictionary[value].Members.Clear();
				Regex regex = new Regex("(?<name>[^ ]*) / <font color='(?<fontColor>[^']*)'>(?<ip>[^<]*)</font> <G>\\((?<country>[^<]*)\\)</G>");
				foreach (string input in list)
				{
					Match match = regex.Match(input);
					RoomMemberViewModel item = new RoomMemberViewModel
					{
						Name = match.Groups["name"].Value,
						Ip = match.Groups["ip"].Value,
						IpColor = (SolidColorBrush)new BrushConverter().ConvertFrom(match.Groups["fontColor"].Value),
						Country = match.Groups["country"].Value
					};
					this.RoomDictionary[value].Members.Add(item);
				}
			}
		}
	}
}
