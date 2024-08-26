using System;
using System.Windows.Input;

namespace Updater
{
	public sealed class DelegateCommand : ICommand
	{
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;
        public event EventHandler CanExecuteChanged;

		public DelegateCommand(Action<object> execute) : this(execute, null) {}

		public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
		{
			this._execute = execute;
			this._canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return this._canExecute == null || this._canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			this._execute(parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			EventHandler canExecuteChanged = this.CanExecuteChanged;
			if (canExecuteChanged == null)
			{
				return;
			}
			canExecuteChanged(this, EventArgs.Empty);
		}
	}
}
