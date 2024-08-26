using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace tfmStandalone
{
	public partial class ChatNotesWindow : PinnableWindow
	{

        public ChatNotesWindow(ChatNotesWindowViewModel viewModel)
		{
			this.InitializeComponent();
			base.DataContext = viewModel;
			viewModel.Closed = (EventHandler)Delegate.Combine(viewModel.Closed, new EventHandler(delegate(object sender, EventArgs e)
			{
				base.CanClose = true;
				base.Close();
			}));
		}
	}
}
