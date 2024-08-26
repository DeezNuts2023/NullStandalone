using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace tfmStandalone
{
	public partial class ChatSettingsWindow : PinnableWindow
	{
        internal ChatSettingsWindow Window;

        public ChatSettingsWindow(ChatSettingsWindowViewModel viewModel)
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
