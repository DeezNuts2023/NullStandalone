using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class CustomCommandStepViewModel : BindableBase
	{
        private string _command;
        private int _delay;

        public string Command
        {
            get => this._command;
            set => this.SetProperty(ref this._command, value, "Command");
        }

        public int Delay
		{
			get
			{
				return this._delay;
			}
			set
			{
				int num = value;
				if (num < 0)
				{
					num = 0;
				}
				if (num > 10000)
				{
					num = 10000;
				}
				this.SetProperty<int>(ref this._delay, num, "Delay");
			}
		}
	}
}
