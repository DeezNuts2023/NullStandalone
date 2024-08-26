using System;
using System.IO;
using Prism.Commands;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class ChatNotesWindowViewModel : BindableBase
	{
        private string _name;
        private string _notes;
        public DelegateCommand SaveCommand { get; }
        public EventHandler Closed;

        public string Name
        {
            get => this._name;
            set => this.SetProperty(ref this._name, value, "Name");
        }

        public string Notes
        {
            get => this._notes;
            set => this.SetProperty(ref this._notes, value, "Notes");
        }

        public ChatNotesWindowViewModel(string name)
		{
			this.Name = name;
			string notesFile = AppDomain.CurrentDomain.BaseDirectory + "User Notes\\" + name + ".txt";
			this.SaveCommand = new DelegateCommand(delegate()
			{
				File.WriteAllText(notesFile, this.Notes);
				EventHandler closed = this.Closed;
				if (closed == null)
				{
					return;
				}
				closed(this, new EventArgs());
			});
			if (File.Exists(notesFile))
			{
				this.Notes = File.ReadAllText(notesFile);
			}
		}
	}
}
