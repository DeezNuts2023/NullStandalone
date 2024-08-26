using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace tfmStandalone.Views
{
	public partial class MenuOverlayWindow : Window
	{
		public MenuOverlayViewModel ViewModel
		{
			get
			{
				return base.DataContext as MenuOverlayViewModel;
			}
			set
			{
				base.DataContext = value;
			}
		}

		public MenuOverlayWindow(MenuOverlayViewModel viewModel)
		{
			this.InitializeComponent();
			this.ViewModel = viewModel;
		}

		private void OnMouseLeave(object sender, MouseEventArgs e)
		{
		}

		private void OnDeactivated(object sender, EventArgs e)
		{
			this.ViewModel.IsCollapsed = true;
		}
	}
}
