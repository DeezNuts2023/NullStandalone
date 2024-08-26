using System;
using System.Windows.Markup;

namespace tfmStandalone
{
	public partial class GameSettingsWindow : PinnableWindow
	{
		public GameSettingsWindow(GameSettingsViewModel viewModel)
		{
			this.InitializeComponent();
			base.DataContext = viewModel;
			viewModel.Closed = (EventHandler)Delegate.Combine(viewModel.Closed, new EventHandler(delegate(object sender, EventArgs e)
			{
				base.Hide();
			}));
		}
	}
}
