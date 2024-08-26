using System;
using System.Collections.Generic;
using System.Linq;

namespace tfmStandalone
{
	public sealed class CustomCommand
	{
        private string _command;
        public bool IsValid { get; private set; }
		public List<CustomCommandStep> Steps { get; set; }
		public string Alias { get; private set; }
		private List<CustomCommandParameter> CommandParameters { get; set; }

        public string Command
        {
            get => this._command;
            set
            {
                this._command = value;
                this.Read();
            }
        }

        public CustomCommand()
		{
			this.Steps = new List<CustomCommandStep>();
		}

		public bool Validate(ref string error)
		{
			if (string.IsNullOrEmpty(this.Command))
			{
				error = "Command is empty";
				this.IsValid = false;
				return this.IsValid;
			}
			List<CustomCommandParameter> list = (from p in this.CommandParameters
			where p.Type == CustomCommandParameterType.Multiple
			select p).ToList<CustomCommandParameter>();
			List<CustomCommandParameter> list2 = (from p in this.CommandParameters
			where p.Type == CustomCommandParameterType.Long
			select p).ToList<CustomCommandParameter>();
			if (list.Count == 0 && list2.Count == 0)
			{
				this.IsValid = true;
				return this.IsValid;
			}
			if (list.Count == 1 && list2.Count == 0 && this.CommandParameters.Count == 1)
			{
				this.IsValid = true;
				return this.IsValid;
			}
			if (list.Count == 0 && list2.Count == 1 && this.CommandParameters.Count == 1)
			{
				this.IsValid = true;
				return this.IsValid;
			}
			if (list.Count > 0 && list2.Count > 0)
			{
				error = "A multi-parameter and a long parameter can not both be used in the same command.";
				this.IsValid = false;
				return this.IsValid;
			}
			if (list.Count > 1)
			{
				error = "Only one multi-parameter can be used in a command";
				this.IsValid = false;
				return this.IsValid;
			}
			if (list2.Count > 1)
			{
				error = "Only one long parameter can be used in a command";
				this.IsValid = false;
				return this.IsValid;
			}
			if (list.Count == 1 && this.CommandParameters.IndexOf(list[0]) != 0)
			{
				error = "Multi-parameter must be the first parameter in a command";
				this.IsValid = false;
				return this.IsValid;
			}
			if (list2.Count == 1 && this.CommandParameters.IndexOf(list2[0]) != this.CommandParameters.Count - 1)
			{
				error = "Long parameter must be the last parameter in a command";
				this.IsValid = false;
				return this.IsValid;
			}
			this.IsValid = true;
			return this.IsValid;
		}

		private void Read()
		{
			if (string.IsNullOrEmpty(this.Command))
			{
				return;
			}
			string[] array = this.Command.Split(new char[]
			{
				' '
			}, StringSplitOptions.RemoveEmptyEntries);
			this.Alias = array[0];
			this.CommandParameters = new List<CustomCommandParameter>();
			if (array.Length > 1)
			{
				for (int i = 1; i < array.Length; i++)
				{
					string text = array[i];
					if (text[0] == '[' && text[text.Length - 1] == ']')
					{
						this.CommandParameters.Add(new CustomCommandParameter
						{
							Key = text,
							Type = CustomCommandParameterType.Multiple
						});
					}
					else if (text[0] == '{' && text[text.Length - 1] == '}')
					{
						this.CommandParameters.Add(new CustomCommandParameter
						{
							Key = text,
							Type = CustomCommandParameterType.Single
						});
					}
					else if (text[0] == '<' && text[text.Length - 1] == '>')
					{
						this.CommandParameters.Add(new CustomCommandParameter
						{
							Key = text,
							Type = CustomCommandParameterType.Long
						});
					}
				}
			}
		}

		public void Execute(string input, Action<string> commandAction)
		{
			List<string> list = input.Split(new char[]
			{
				' '
			}, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
			list.RemoveAt(0);
			if (list.Count < this.CommandParameters.Count)
			{
				return;
			}
			CommandQueue commandQueue = new CommandQueue(commandAction);
			if (this.CommandParameters.Count > 0)
			{
				if (this.CommandParameters[0].Type == CustomCommandParameterType.Multiple)
				{
					for (int i = 0; i < list.Count - this.CommandParameters.Count + 1; i++)
					{
						List<string> list2 = new List<string>
						{
							list[i]
						};
						for (int j = list.Count - this.CommandParameters.Count + 1; j < list.Count; j++)
						{
							list2.Add(list[j]);
						}
						this.Process(list2, commandQueue);
					}
				}
				else if (this.CommandParameters[this.CommandParameters.Count - 1].Type == CustomCommandParameterType.Long)
				{
					List<string> list3 = new List<string>();
					for (int k = 0; k < this.CommandParameters.Count - 1; k++)
					{
						list3.Add(list[k]);
					}
					string item = string.Join(" ", list.ToArray(), this.CommandParameters.Count - 1, list.Count - this.CommandParameters.Count + 1);
					list3.Add(item);
					this.Process(list3, commandQueue);
				}
				else
				{
					this.Process(list, commandQueue);
				}
			}
			else
			{
				foreach (CustomCommandStep customCommandStep in this.Steps)
				{
					commandQueue.Add(customCommandStep.Command, customCommandStep.Delay);
				}
			}
			commandQueue.Start();
		}

		private void Process(List<string> input, CommandQueue commandQueue)
		{
			if (input.Count != this.CommandParameters.Count)
			{
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < this.CommandParameters.Count; i++)
			{
				dictionary[this.CommandParameters[i].Key] = input[i];
			}
			foreach (CustomCommandStep customCommandStep in this.Steps)
			{
				string text = customCommandStep.Command;
				foreach (CustomCommandParameter customCommandParameter in this.CommandParameters)
				{
					text = text.Replace(customCommandParameter.Key, dictionary[customCommandParameter.Key]);
				}
				commandQueue.Add(text, customCommandStep.Delay);
			}
		}
	}
}
