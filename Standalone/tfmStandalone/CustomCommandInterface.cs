using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace tfmStandalone
{
	public sealed class CustomCommandInterface
	{
        private static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + "CustomCommands.xml";
        private GameSettings GameSettings { get; }
        public Dictionary<string, CustomCommand> Commands { get; }

		public CustomCommandInterface(GameSettings gameSettings)
		{
			this.GameSettings = gameSettings;
			this.Commands = new Dictionary<string, CustomCommand>();
			this.Load();
		}

		public void Save()
		{
			object[] array = new object[1];
			array[0] = new XElement("CustomCommands", this.Commands.Select(delegate(KeyValuePair<string, CustomCommand> c)
			{
				XName name = "CustomCommand";
				object[] array2 = new object[2];
				array2[0] = new XAttribute("Command", c.Value.Command);
				array2[1] = from s in c.Value.Steps
				select new XElement("Step", new object[]
				{
					new XAttribute("Command", s.Command),
					new XAttribute("Delay", s.Delay)
				});
				return new XElement(name, array2);
			}));
			new XDocument(array).Save(CustomCommandInterface.FilePath);
		}

		public void Load()
		{
			if (File.Exists(CustomCommandInterface.FilePath))
			{
				try
				{
					XElement xelement = XDocument.Load(CustomCommandInterface.FilePath).Element("CustomCommands");
					List<CustomCommand> list;
					if (xelement == null)
					{
						list = null;
					}
					else
					{
						list = xelement.Descendants("CustomCommand").Select(delegate(XElement e)
						{
							CustomCommand customCommand6 = new CustomCommand();
							XAttribute xattribute = e.Attribute("Command");
							customCommand6.Command = ((xattribute != null) ? xattribute.Value : null);
							customCommand6.Steps = e.Descendants("Step").Select(delegate(XElement s)
							{
								CustomCommandStep customCommandStep = new CustomCommandStep();
								XAttribute xattribute2 = s.Attribute("Command");
								customCommandStep.Command = ((xattribute2 != null) ? xattribute2.Value : null);
								XAttribute xattribute3 = s.Attribute("Delay");
								customCommandStep.Delay = int.Parse((xattribute3 != null) ? xattribute3.Value : null);
								return customCommandStep;
							}).ToList<CustomCommandStep>();
							return customCommand6;
						}).ToList<CustomCommand>();
					}
					foreach (CustomCommand customCommand in list)
					{
						string key = customCommand.Command.Split(new char[]
						{
							' '
						})[0].ToLowerInvariant();
						this.Commands.Add(key, customCommand);
						string empty = string.Empty;
						customCommand.Validate(ref empty);
					}
					return;
				}
				catch (Exception)
				{
					return;
				}
			}
			string empty2 = string.Empty;
			string arg = string.IsNullOrEmpty(this.GameSettings.BadNameReason) ? "Name violation, please create a new account. Don't log in on this account again or you'll receive a 360h ban." : this.GameSettings.BadNameReason;
			CustomCommand customCommand2 = new CustomCommand
			{
				Command = "badname [name]"
			};
			customCommand2.Steps.Add(new CustomCommandStep
			{
				Command = string.Format("iban [name] 0 {0}", arg),
				Delay = 2000
			});
			customCommand2.Steps.Add(new CustomCommandStep
			{
				Command = "iban [name] 360 Name violation"
			});
			this.Commands.Add("badname", customCommand2);
			customCommand2.Validate(ref empty2);
			CustomCommand customCommand3 = new CustomCommand
			{
				Command = "color [name] {color}"
			};
			customCommand3.Steps.Add(new CustomCommandStep
			{
				Command = "colornick [name] {color}"
			});
			customCommand3.Steps.Add(new CustomCommandStep
			{
				Command = "colormouse [name] {color}"
			});
			this.Commands.Add("color", customCommand3);
			customCommand3.Validate(ref empty2);
			CustomCommand customCommand4 = new CustomCommand
			{
				Command = "case {name}"
			};
			customCommand4.Steps.Add(new CustomCommandStep
			{
				Command = "profile {name}"
			});
			customCommand4.Steps.Add(new CustomCommandStep
			{
				Command = "casier {name} true"
			});
			customCommand4.Steps.Add(new CustomCommandStep
			{
				Command = "relation {name}"
			});
			customCommand4.Steps.Add(new CustomCommandStep
			{
				Command = "l {name} 1000"
			});
			this.Commands.Add("case", customCommand4);
			customCommand4.Validate(ref empty2);
			CustomCommand customCommand5 = new CustomCommand
			{
				Command = "bhnote {name} <note>"
			};
			customCommand5.Steps.Add(new CustomCommandStep
			{
				Command = "ibanhack {name}",
				Delay = 2000
			});
			customCommand5.Steps.Add(new CustomCommandStep
			{
				Command = "imute {name} 0 <note>"
			});
			this.Commands.Add("bhnote", customCommand5);
			customCommand5.Validate(ref empty2);
		}

		public bool Execute(string command, string input, Action<string> executeCommand)
		{
			if (!this.Commands.ContainsKey(command))
			{
				return true;
			}
			CustomCommand customCommand = this.Commands[command];
			if (customCommand.IsValid)
			{
				customCommand.Execute(input, executeCommand);
			}
			return false;
		}
	}
}
