using System;
using Prism.Commands;
using Prism.Mvvm;

namespace tfmStandalone
{
	public class NewChatViewModel : BindableBase
	{
        public EventHandler Accepted;
        public EventHandler Cancelled;
        private string _name;
        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public string Name
        {
            get => this._name;
            set => this.SetProperty(ref this._name, value, "Name");
        }

        public NewChatViewModel()
		{
			this.OkCommand = new DelegateCommand(delegate()
			{
				EventHandler accepted = this.Accepted;
				if (accepted == null)
				{
					return;
				}
				accepted(this, new EventArgs());
			});
			this.CancelCommand = new DelegateCommand(delegate()
			{
				EventHandler cancelled = this.Cancelled;
				if (cancelled == null)
				{
					return;
				}
				cancelled(this, new EventArgs());
			});
		}
	}
}
