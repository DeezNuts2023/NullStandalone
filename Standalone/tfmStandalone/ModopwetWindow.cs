using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace tfmStandalone
{
	public partial class ModopwetWindow : PinnableWindow
	{
		public bool IsApplicationClosing { get; set; }

        private ModopwetViewModel ModopwetViewModel
        {
            get => (ModopwetViewModel)base.DataContext;
            set => base.DataContext = value;
        }

        public ModopwetWindow(ModopwetViewModel viewModel)
		{
			this.InitializeComponent();
			this.ModopwetViewModel = viewModel;
			ModopwetViewModel modopwetViewModel = this.ModopwetViewModel;
			modopwetViewModel.NewReportReceived = (EventHandler)Delegate.Combine(modopwetViewModel.NewReportReceived, new EventHandler(delegate(object o, EventArgs args)
			{
				if ((!base.IsActive && !base.Topmost) || base.WindowState == WindowState.Minimized)
				{
					WindowFlash.FlashWindow(this);
				}
			}));
		}

		private void HandleActivated(object sender, EventArgs e)
		{
			WindowFlash.StopFlashing(this);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = !this.IsApplicationClosing;
			base.Hide();
			this.ModopwetViewModel.WindowHidden();
		}

		private void HandleLoaded(object sender, RoutedEventArgs e)
		{
            Random random = new Random();
            PropertyKey propertyKey = new PropertyKey(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);
            WindowProperties.SetWindowProperty(this, propertyKey, "tfmModopwetWindow" + random.Next(1, int.MaxValue));
        }

		private void HandleIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.ModopwetViewModel.WindowVisibilityChanged(base.IsVisible);
		}
	}
}
