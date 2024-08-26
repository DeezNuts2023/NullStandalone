using System;
using System.Windows.Markup;

namespace tfmStandalone
{
	public partial class NewChatDialogWindow : PinnableWindow, IComponentConnector
	{
		public NewChatDialogWindow(NewChatViewModel viewModel)
		{
			this.InitializeComponent();
			base.DataContext = viewModel;
			viewModel.Accepted = (EventHandler)Delegate.Combine(viewModel.Accepted, new EventHandler(delegate(object sender, EventArgs args)
			{
				base.CanClose = true;
				base.DialogResult = new bool?(true);
				base.Close();
			}));
			viewModel.Cancelled = (EventHandler)Delegate.Combine(viewModel.Cancelled, new EventHandler(delegate(object sender, EventArgs args)
			{
				base.CanClose = true;
				base.DialogResult = new bool?(false);
				base.Close();
			}));
		}
	}
}
