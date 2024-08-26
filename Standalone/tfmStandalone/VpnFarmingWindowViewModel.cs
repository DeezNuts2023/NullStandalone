using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using tfmClientHook;
using tfmClientHook.Messages;

namespace tfmStandalone
{
	public sealed class VpnFarmingWindowViewModel : BindableBase
	{
        public EventHandler Closed;
        private string _names;
        private string _lsRoomResult;
        public DelegateCommand BanCommand { get; }

        public string Names
        {
            get => this._names;
            set => this.SetProperty(ref this._names, value, "Names");
        }

        public string LsRoomResult
        {
            get => this._lsRoomResult;
            set => this.SetProperty(ref this._lsRoomResult, value, "LsRoomResult");
        }

        public VpnFarmingWindowViewModel(ClientHook clientHook)
		{
			VpnFarmingWindowViewModel current = this;
			this.BanCommand = new DelegateCommand(delegate()
			{
				List<string> list = (from n in current._names.Split(new char[]
				{
					' '
				})
				where !string.IsNullOrEmpty(n)
				select n).ToList<string>();
				string text = string.Join(", ", list);
				List<string> list2 = current._lsRoomResult.Split(new string[] {"\r\n","\n"}, StringSplitOptions.None).ToList<string>();
				List<string> list3 = new List<string>();
				foreach (string str in list)
				{
					list3.Add("banhack " + str);
				}
				foreach (string text2 in list2)
				{
					string text3 = text2.Split(new string[]{" / "}, StringSplitOptions.None)[0];
					if (!list.Contains(text3))
					{
						list3.Add(string.Concat(new string[] {"ban ",text3," 360 VPN Farming (",text,")"}));
					}
				}
				foreach (string str2 in list)
				{
					list3.Add("mute " + str2 + " 0 VPN Farming");
				}
				foreach (string command in list3)
				{
					clientHook.SendToServer(ConnectionType.Main, new C_Command
					{
						Command = command
					});
				}
				EventHandler closed = current.Closed;
				if (closed == null)
				{
					return;
				}
				closed(current, new EventArgs());
			});
		}
	}
}
