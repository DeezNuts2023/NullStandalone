using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Microsoft.Win32;

namespace Updater
{
	public partial class MainWindow : Window
	{
		private Storyboard RotationAnimation { get; }
		private bool AnimationRunning { get; set; }

        public MainWindowViewModel ViewModel { get => base.DataContext as MainWindowViewModel; set => base.DataContext = value; }

        public MainWindow()
		{
			this.InitializeComponent();
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			this.ViewModel = new MainWindowViewModel((commandLineArgs.Length >= 2) ? commandLineArgs[1] : null);
			MainWindowViewModel viewModel = this.ViewModel;
			viewModel.Processing = (EventHandler<bool>)Delegate.Combine(viewModel.Processing, new EventHandler<bool>(delegate(object sender, bool processing)
			{
				if (!processing || this.AnimationRunning)
				{
					if (!processing)
					{
						this.AnimationRunning = false;
					}
					return;
				}
				this.AnimationRunning = true;
				Storyboard rotationAnimation = this.RotationAnimation;
				if (rotationAnimation == null)
				{
					return;
				}
				rotationAnimation.Begin();
			}));
			this.RotationAnimation = (base.FindResource("RotateCheeseStoryboard") as Storyboard);
			if (this.RotationAnimation != null)
			{
				this.RotationAnimation.Completed += delegate(object sender, EventArgs args)
				{
					if (this.AnimationRunning)
					{
						this.RotationAnimation.Begin();
						return;
					}
					this.RotationAnimation.Stop();
				};
			}
		}

		private void BrowseButtonOnClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				InitialDirectory = this.ViewModel.ClientFolder,
				Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*"
			};
			if (openFileDialog.ShowDialog() == true)
			{
				this.ViewModel.ExeLocation = openFileDialog.FileName;
			}
		}

		private void CancelButtonOnClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
