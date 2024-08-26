using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Threading;
using Newtonsoft.Json;
using tfmClientHook;
using tfmClientHook.Messages;

namespace tfmStandalone
{
	public sealed class CommandService
	{
		private ClientHook ClientHook { get; }
		private GameInfo GameInfo { get; }
		private CustomCommandInterface CustomCommandInterface { get; }

		private WindowService WindowService { get; }

		public CommandService(ClientHook clientHook, MessageInterceptor messageInterceptor, GameInfo gameInfo, CustomCommandInterface customCommandInterface, WindowService windowService)
		{
			this.ClientHook = clientHook;
			this.GameInfo = gameInfo;
			this.CustomCommandInterface = customCommandInterface;
			this.WindowService = windowService;
			messageInterceptor.CommandSent = (EventHandler<MessageInterceptor.CommandSendEventArgs>)Delegate.Combine(messageInterceptor.CommandSent, new EventHandler<MessageInterceptor.CommandSendEventArgs>(delegate(object sender, MessageInterceptor.CommandSendEventArgs e)
			{
				e.SendToServer = this.ProcessCommand(e.Message.Command);
			}));
		}

		public bool ProcessCommand(string fullCommand)
		{
			List<string> commandParams = fullCommand.Split(new char[]
			{
				' '
			}).ToList<string>();
			if (commandParams.Count == 0)
			{
				return true;
			}
			string text = commandParams[0].ToLowerInvariant();
			commandParams.RemoveAt(0);
			switch(text)
			{
				case "ibanhack":
				case "banhack":
                case "mute":
                case "imute":
				case "relation":
				case "mumute":
				case "chercher":
				case "search":
				case "iban":
				case "ban":
				case "test":
                case "roomkick":
                case "prison":
                case "l":
                    {
                        if (commandParams.Count == 0)
                        {
                            return true;
                        }
                        int num3;
                        if (int.TryParse(commandParams[commandParams.Count - 1], out num3))
                        {
                            for (int j = 0; j < commandParams.Count - 1; j++)
                            {
                                this.ClientHook.SendToServer(ConnectionType.Main, new C_Command{Command = string.Concat(new object[]{ text," ",commandParams[j]," ",num3})});
                            }
                        }
                        else
                        {
                            foreach (string str3 in commandParams)
                            {
                                this.ClientHook.SendToServer(ConnectionType.Main, new C_Command
                                {
                                    Command = text + " " + str3
                                });
                            }
                        }
                        return false;
                    }
                case "tignore":
                    foreach (string text2 in commandParams)
                    {
                        string text3 = text2.ToLowerInvariant();
                        if (this.GameInfo.TemporaryIgnoreList.Contains(text3))
                        {
                            this.GameInfo.TemporaryIgnoreList.Remove(text3);
                            this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                            {
                                Message = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text3) + " unignored."
                            });
                        }
                        else
                        {
                            this.GameInfo.TemporaryIgnoreList.Add(text3);
                            this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                            {
                                Message = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text3) + " ignored."
                            });
                        }
                    }
					return false;
				case "lc":
                    string value = "• [" + DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat) + "] " + string.Join(" ", commandParams);
                    using (StreamWriter streamWriter = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "Log.txt"))
                    {
                        streamWriter.WriteLine(value);
                    }
                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                    {
                        Message = "Comment logged."
                    });
                    return false;
				case "modinfo":
                    if (commandParams.Count > 0)
                    {
                        string name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(commandParams[0].ToLowerInvariant());
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.Headers[HttpRequestHeader.Authorization] = "Null TnVsbGlmaWNhdG9yXyA2MzRAIz8=";
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                            webClient.Headers["From"] = this.GameInfo.Name;
                            webClient.DownloadStringCompleted += delegate (object sender, DownloadStringCompletedEventArgs args)
                            {
                                if (args.Error == null)
                                {
                                    ModInfo modInfo = JsonConvert.DeserializeObject<ModInfo>(args.Result);
                                    string text7 = string.Format("<BV>Mod Info for <N>{0}<BL>\n• ", name);
                                    foreach (string text8 in modInfo.community)
                                    {
                                        text7 += string.Format("[{0}] ", text8.ToUpperInvariant());
                                    }
                                    text7 += this.GetNameString(modInfo.main, modInfo.role);
                                    if (modInfo.alts != null && modInfo.alts.Count > 0)
                                    {
                                        text7 += "\n<BV>Alts: ";
                                        foreach (ModAlt modAlt in modInfo.alts)
                                        {
                                            text7 += string.Format("\n<BL>• {0}", this.GetNameString(modAlt.nick, modAlt.role));
                                        }
                                    }
                                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                                    {
                                        Message = text7
                                    });
                                    return;
                                }
                                if (args.Error is WebException)
                                {
                                    WebResponse response = ((WebException)args.Error).Response;
                                    Stream stream = (response != null) ? response.GetResponseStream() : null;
                                    if (stream != null)
                                    {
                                        using (StreamReader streamReader = new StreamReader(stream))
                                        {
                                            string code = JsonConvert.DeserializeObject<ModInfoError>(streamReader.ReadToEnd()).code;
                                            if (!(code == "10014") && !(code == "10018") && !(code == "10019"))
                                            {
                                                if (!(code == "10020"))
                                                {
                                                    if (!(code == "10017"))
                                                    {
                                                        this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                                                        {
                                                            Message = "<R>• Error: API unavailable"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                                                        {
                                                            Message = "<R>• Error: Staff member not found"
                                                        });
                                                    }
                                                }
                                                else
                                                {
                                                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                                                    {
                                                        Message = "<R>• Error: Search term is not valid"
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                                                {
                                                    Message = "<R>• Error: Mod info access denied"
                                                });
                                            }
                                        }
                                    }
                                }
                            };
                            webClient.DownloadStringAsync(new Uri(string.Format("https://staff801.com/api/modinfo/{0}", name)));
                        }
                    }
                    return false;
				case "stalk":
                    if (commandParams.Count > 0)
                    {
                        TaskHelpers.UiInvoke(delegate
                        {
                            this.ClientHook.SendToServer(ConnectionType.Main, new C_Command
                            {
                                Command = "ninja " + commandParams[0]
                            });
                            DispatcherTimer timer3 = new DispatcherTimer
                            {
                                Interval = TimeSpan.FromMilliseconds(1000.0)
                            };
                            timer3.Tick += delegate (object sender, EventArgs args)
                            {
                                this.ClientHook.SendToServer(ConnectionType.Main, new C_ReportPlayer
                                {
                                    Type = ReportType.Hack,
                                    Name = commandParams[0]
                                });
                                timer3.Stop();
                            };
                            timer3.Start();
                            DispatcherTimer timer = new DispatcherTimer
                            {
                                Interval = TimeSpan.FromMilliseconds(2000.0)
                            };
                            timer.Tick += delegate (object sender, EventArgs args)
                            {
                                this.ClientHook.SendToServer(ConnectionType.Main, new C_ChangeRoom
                                {
                                    RoomName = string.Format("*{0} bootcamp", new Random().Next(348729, int.MaxValue))
                                });
                                timer.Stop();
                            };
                            timer.Start();
                            DispatcherTimer timer2 = new DispatcherTimer
                            {
                                Interval = TimeSpan.FromMilliseconds(6000.0)
                            };
                            timer2.Tick += delegate (object sender, EventArgs args)
                            {
                                this.ClientHook.SendToServer(ConnectionType.Main, new C_Command
                                {
                                    Command = "ninja " + commandParams[0]
                                });
                                timer2.Stop();
                            };
                            timer2.Start();
                        });
                    }
                    return false;
				case "client":
                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                    {
                        Message = "/lc [comment] - Log comment directly to Log.txt"
                    });
                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                    {
                        Message = "/stalk [name] - Join a player, report for hacking, then ninja"
                    });
                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                    {
                        Message = "/tignore [name1] [name2] - Add/remove specified players to ignore whispers from. The list is cleared when the client is closed"
                    });
                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                    {
                        Message = "/lsign - List temporarily ignored players"
                    });
                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                    {
                        Message = "/igcommu [community1] [community2] - Add/remove specified communities to ignore whispers from"
                    });
                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                    {
                        Message = "/lsigcommu - List ignored communities"
                    });
                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                    {
                        Message = "/wlcommu [community1] [community2] - Add/remove specified communities to the whitelist. Whitelisted communities are the only communities you can receive whispers from."
                    });
                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                    {
                        Message = "/lswlcommu - List whitelisted communities"
                    });
                    return false;
				case "casier":
                    if (commandParams.Count == 0)
                    {
                        return true;
                    }
                    if (commandParams[commandParams.Count - 1].ToLowerInvariant() == "true")
                    {
                        for (int i = 0; i < commandParams.Count - 1; i++)
                        {
                            this.ClientHook.SendToServer(ConnectionType.Main, new C_Command
                            {
                                Command = "casier " + commandParams[i] + " true"
                            });
                        }
                    }
                    else
                    {
                        foreach (string str in commandParams)
                        {
                            this.ClientHook.SendToServer(ConnectionType.Main, new C_Command
                            {
                                Command = "casier " + str
                            });
                        }
                    }
                    return false;
				case "announce":
                    TaskHelpers.UiInvoke(delegate
                    {
                        this.WindowService.ShowAnnouncementWindow();
                    });
                    return false;
                case "lsign":
                    string str2 = string.Join(", ", from n in this.GameInfo.TemporaryIgnoreList
                                                    select CultureInfo.InvariantCulture.TextInfo.ToTitleCase(n));
                    this.ClientHook.SendToClient(ConnectionType.Main, new S_GenericChatMessage
                    {
                        Message = "Ignored Players: " + str2
                    });
                    return false;
                default:
                    return false;

            }
		}

		private string GetNameString(string name, string role)
		{
			string arg = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name.ToLowerInvariant());
			if (role == "admin")
			{
				return string.Format("<font color=\"#EB1D51\">{0} (Administrator)", arg);
			}
			if (role == "public")
			{
				return string.Format("<J>{0} (Public Moderator)", arg);
			}
			if (role == "private")
			{
				return string.Format("<BL>{0} (Private Moderator)", arg);
			}
			if (role == "trial")
			{
				return string.Format("<ROSE>{0} (Trial Moderator)", arg);
			}
			if (!(role == "arb"))
			{
				return string.Format("<N>{0} (Regular Player)", arg);
			}
			return string.Format("<V>{0} (Arbitre)", arg);
		}
	}
}
