using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class CustomCommandViewModel : BindableBase
	{
        private string _command;
        private string _validationError;

        public string Command
        {
            get => this._command;
            set => this.SetProperty(ref this._command, value, "Command");
        }

        public string ValidationError
        {
            get => this._validationError;
            set => this.SetProperty(ref this._validationError, value, "ValidationError");
        }

        public ObservableCollection<CustomCommandStepViewModel> Steps { get; }
		public DelegateCommand AddStepCommand { get; }
		public DelegateCommand<CustomCommandStepViewModel> RemoveStepCommand { get; }

		public CustomCommandViewModel()
		{
			this.Steps = new ObservableCollection<CustomCommandStepViewModel>();
			this.AddStepCommand = new DelegateCommand(delegate()
			{
				this.Steps.Add(new CustomCommandStepViewModel());
			});
			this.RemoveStepCommand = new DelegateCommand<CustomCommandStepViewModel>(delegate(CustomCommandStepViewModel step)
			{
				if (this.Steps.Contains(step))
				{
					this.Steps.Remove(step);
				}
			});
		}
	}
}
