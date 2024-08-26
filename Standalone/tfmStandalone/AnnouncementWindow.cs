using System.Text.RegularExpressions;
using System;
using System.Windows;
using tfmClientHook.Messages;
using tfmClientHook;
using System.Windows.Markup;

namespace tfmStandalone
{
    /// <summary>
    /// Interaction logic for AnnouncementWindow.xaml
    /// </summary>
    public partial class AnnouncementWindow : PinnableWindow, IComponentConnector
    {
        private ClientHook ClientHook { get; }

        public AnnouncementWindow(ClientHook clientHook)
        {
            this.InitializeComponent();
            this.ClientHook = clientHook;
        }

        private void SendClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Announcement.Text))
            {
                return;
            }
            this.ClientHook.SendToServer(ConnectionType.Main, new C_StaffChatMessage
            {
                Type = StaffChatType.RoomModeration,
                Message = this.Announcement.Text
            });
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            int roomMinimum = -1;
            int.TryParse(this.RoomMinimum.Text, out roomMinimum);
            string[] array = this.Rooms.Text.Split(new string[]
            {
                "\r\n",
                "\n"
            }, StringSplitOptions.RemoveEmptyEntries);
            int num = 0;
            string nextRoom = this.GetNextRoom(array, roomMinimum, ref num);
            if (string.IsNullOrEmpty(nextRoom))
            {
                this.Rooms.Text = string.Empty;
                return;
            }
            this.ClientHook.SendToServer(ConnectionType.Main, new C_Command
            {
                Command = string.Format("room* {0}", nextRoom)
            });
            this.Rooms.Text = ((num == array.Length - 1) ? string.Empty : string.Join("\n", array, num + 1, array.Length - num - 1));
        }

        private string GetNextRoom(string[] rooms, int roomMinimum, ref int index)
        {
            for (int i = 0; i < rooms.Length; i++)
            {
                Match match = Regex.Match(rooms[i], "(?<roomName>.+) \\((?<community>.{2}) / (?<bulle>[^\\s]+)\\) : (?<playerCount>\\d+)");
                if (match.Success && !match.Groups["roomName"].Value.Contains("\u0003") && (roomMinimum == -1 || int.Parse(match.Groups["playerCount"].Value) >= roomMinimum))
                {
                    index = i;
                    return match.Groups["roomName"].Value;
                }
            }
            return null;
        }
    }
}
