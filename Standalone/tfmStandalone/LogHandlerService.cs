using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace tfmStandalone
{
	public sealed class LogHandlerService
	{
		private Dictionary<string, List<LogWindowViewModel>> LogRegisters { get; }
		private WindowService WindowService { get; }
		private GameSettings GameSettings { get; }

		public LogHandlerService(MessageInterceptor messageInterceptor, WindowService windowService, GameSettings gameSettings)
		{
			this.WindowService = windowService;
			this.GameSettings = gameSettings;
			messageInterceptor.WindowDisplayMessageReceived = (EventHandler<MessageInterceptor.WindowDisplayMessageEventArgs>)Delegate.Combine(messageInterceptor.WindowDisplayMessageReceived, new EventHandler<MessageInterceptor.WindowDisplayMessageEventArgs>(this.WindowDisplayMessageReceived));
			this.LogRegisters = new Dictionary<string, List<LogWindowViewModel>>();
		}

		public void RegisterForLog(LogWindowViewModel viewModel, string key)
		{
			string key2 = key.ToLowerInvariant();
			if (!this.LogRegisters.ContainsKey(key2))
			{
				this.LogRegisters.Add(key2, new List<LogWindowViewModel>());
			}
			if (!this.LogRegisters[key2].Contains(viewModel))
			{
				this.LogRegisters[key2].Add(viewModel);
			}
		}

		private void WindowDisplayMessageReceived(object sender, MessageInterceptor.WindowDisplayMessageEventArgs e)
		{
			if (this.GameSettings.UseCustomConnectionLogWindow && (e.Message.Text.StartsWith("<p align='center'>Connection logs for player <BL>") || e.Message.Text.StartsWith("<p align='center'>Connection logs for IP address <V>")))
			{
				e.SendToClient = false;
				TaskHelpers.UiInvoke(delegate
				{
					string[] array = e.Message.Text.Split(new string[]
					{
						"\r\n",
						"\n"
					}, StringSplitOptions.RemoveEmptyEntries);
					Match match = Regex.Match(array[0], "<p align='center'>Connection logs for (?<type>player|IP address) (?:<BL>|<V>){1}(?<playerIp>.+)(?:</BL>|</V>){1}</p>");
					string value = match.Groups["playerIp"].Value;
					bool isPlayer = match.Groups["type"].Value == "player";
					List<PlayerLogModel> logs = this.GetLogs(array);
					string key = value.ToLowerInvariant();
					if (this.LogRegisters.ContainsKey(key))
					{
						foreach (LogWindowViewModel logWindowViewModel in this.LogRegisters[key])
						{
							logWindowViewModel.LogReceived(e.Message.FontStyle, e.Message.WindowKey, e.Message.Text, isPlayer, value, logs);
						}
						this.LogRegisters[key].Clear();
						this.LogRegisters.Remove(key);
						return;
					}
					this.WindowService.ShowLogWindow(e.Message.FontStyle, e.Message.WindowKey, e.Message.Text, isPlayer, value, logs);
				});
				return;
			}
		}

		private List<PlayerLogModel> GetLogs(string[] logs)
		{
			List<PlayerLogModel> list = new List<PlayerLogModel>();
			if (logs.Length > 1)
			{
				for (int i = 1; i < logs.Length; i++)
				{
					Match match = Regex.Match(logs[i], "<V>\\[ (?<playerName>.+) \\]</V>  <BL>(?<time>.+)</BL><G>  \\( <font color='(?<color>#.+)'>(?<ip>#.+)</font> -  (?<country>.+) \\)  (?<type>[^\\s]+)(?: - )*(?<community>[^\\s]*)</G>");
					if (match.Success)
					{
						list.Add(new PlayerLogModel
						{
							Name = match.Groups["playerName"].Value.Trim(),
							Date = match.Groups["time"].Value,
							IPColor = match.Groups["color"].Value,
							IP = match.Groups["ip"].Value,
							Country = match.Groups["country"].Value,
							Type = match.Groups["type"].Value,
							Community = match.Groups["community"].Value
						});
					}
				}
			}
			return list;
		}
	}
}
