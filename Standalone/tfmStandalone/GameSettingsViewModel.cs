using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class GameSettingsViewModel : BindableBase
	{
        private GameSettingsViewModel.GameSettingsCategory _selectedCategory;
        private string _homeRoom;
        private int _gifLength;
        private Theme _theme;
        private bool _useCustomConnectionLogWindow;
        private bool _useCustomCasierWindow;
        public EventHandler Closed;
        public List<Theme> Themes { get; }
		public ObservableCollection<CustomCommandViewModel> CustomCommands { get; }
		public DelegateCommand AddCommandCommand { get; }
		public DelegateCommand<CustomCommandViewModel> RemoveCommandCommand { get; }
		public DelegateCommand<GameSettingsViewModel.GameSettingsCategory?> SelectCategoryCommand { get; }
		public DelegateCommand SaveCommand { get; }
		public DelegateCommand CancelCommand { get; }
		private GameSettings GameSettings { get; }
		private CustomCommandInterface CustomCommandInterface { get; }

        public GameSettingsViewModel.GameSettingsCategory SelectedCategory
        {
            get => this._selectedCategory;
            set => this.SetProperty(ref this._selectedCategory, value, nameof(SelectedCategory));
        }

        public string HomeRoom
        {
            get => this._homeRoom;
            set => this.SetProperty(ref this._homeRoom, value, nameof(HomeRoom));
        }

        public int GifLength
        {
            get => this._gifLength;
            set => this.SetProperty(ref this._gifLength, value, nameof(GifLength));
        }

        public Theme Theme
        {
            get => this._theme;
            set => this.SetProperty(ref this._theme, value, nameof(Theme));
        }

        public bool UseCustomConnectionLogWindow
        {
            get => this._useCustomConnectionLogWindow;
            set => this.SetProperty(ref this._useCustomConnectionLogWindow, value, nameof(UseCustomConnectionLogWindow));
        }

        public bool UseCustomCasierWindow
        {
            get => this._useCustomCasierWindow;
            set => this.SetProperty(ref this._useCustomCasierWindow, value, nameof(UseCustomCasierWindow));
        }

        public GameSettingsViewModel(GameSettings gameSettings, CustomCommandInterface customCommandInterface)
		{
			this.GameSettings = gameSettings;
			this.CustomCommandInterface = customCommandInterface;
			this.SelectedCategory = GameSettingsViewModel.GameSettingsCategory.General;
			this.HomeRoom = this.GameSettings.HomeRoom;
			this.GifLength = this.GameSettings.GifLength;
			this.Theme = this.GameSettings.Theme;
			this.UseCustomConnectionLogWindow = this.GameSettings.UseCustomConnectionLogWindow;
			this.UseCustomCasierWindow = this.GameSettings.UseCustomCasierWindow;
			this.Themes = new List<Theme>
			{
				Theme.Light,
				Theme.Dark,
				Theme.Transformice
			};
			this.CustomCommands = new ObservableCollection<CustomCommandViewModel>();
			foreach (KeyValuePair<string, CustomCommand> keyValuePair in this.CustomCommandInterface.Commands)
			{
				CustomCommandViewModel customCommandViewModel = new CustomCommandViewModel
				{
					Command = keyValuePair.Value.Command
				};
				foreach (CustomCommandStep customCommandStep in keyValuePair.Value.Steps)
				{
					customCommandViewModel.Steps.Add(new CustomCommandStepViewModel
					{
						Command = customCommandStep.Command,
						Delay = customCommandStep.Delay
					});
				}
				this.CustomCommands.Add(customCommandViewModel);
			}
			this.AddCommandCommand = new DelegateCommand(delegate()
			{
				CustomCommandViewModel customCommandViewModel2 = new CustomCommandViewModel();
				customCommandViewModel2.Steps.Add(new CustomCommandStepViewModel());
				this.CustomCommands.Insert(0, customCommandViewModel2);
			});
			this.RemoveCommandCommand = new DelegateCommand<CustomCommandViewModel>(delegate(CustomCommandViewModel c)
			{
				this.CustomCommands.Remove(c);
			});
			this.SelectCategoryCommand = new DelegateCommand<GameSettingsViewModel.GameSettingsCategory?>(delegate(GameSettingsViewModel.GameSettingsCategory? c)
			{
				this.SelectedCategory = (c ?? GameSettingsViewModel.GameSettingsCategory.General);
			});
			this.SaveCommand = new DelegateCommand(new Action(this.Save));
			this.CancelCommand = new DelegateCommand(delegate()
			{
				EventHandler closed = this.Closed;
				if (closed == null)
				{
					return;
				}
				closed(this, new EventArgs());
			});
		}

		private void Save()
		{
			List<CustomCommand> list = new List<CustomCommand>();
			foreach (CustomCommandViewModel customCommandViewModel in this.CustomCommands)
			{
				CustomCommand customCommand = new CustomCommand
				{
					Command = customCommandViewModel.Command
				};
				foreach (CustomCommandStepViewModel customCommandStepViewModel in customCommandViewModel.Steps)
				{
					customCommand.Steps.Add(new CustomCommandStep
					{
						Command = customCommandStepViewModel.Command,
						Delay = customCommandStepViewModel.Delay
					});
				}
				string empty = string.Empty;
				customCommand.Validate(ref empty);
				customCommandViewModel.ValidationError = empty;
				list.Add(customCommand);
			}
			if (list.All((CustomCommand c) => c.IsValid))
			{
				this.CustomCommandInterface.Commands.Clear();
				foreach (CustomCommand customCommand2 in list)
				{
					string key = customCommand2.Command.Split(new char[]
					{
						' '
					})[0].ToLowerInvariant();
					this.CustomCommandInterface.Commands.Add(key, customCommand2);
				}
				this.CustomCommandInterface.Save();
				this.GameSettings.HomeRoom = this.HomeRoom;
				this.GameSettings.GifLength = ((this.GifLength < 5) ? 5 : ((this.GifLength > 60) ? 60 : this.GifLength));
				this.GameSettings.Theme = this.Theme;
				this.GameSettings.UseCustomConnectionLogWindow = this.UseCustomConnectionLogWindow;
				this.GameSettings.UseCustomCasierWindow = this.UseCustomCasierWindow;
				this.GameSettings.Save();
				((App)Application.Current).SetTheme(this.GameSettings.Theme);
				EventHandler closed = this.Closed;
				if (closed == null)
				{
					return;
				}
				closed(this, new EventArgs());
			}
		}

		public enum GameSettingsCategory
		{
			General,
			Commands
		}
	}
}
