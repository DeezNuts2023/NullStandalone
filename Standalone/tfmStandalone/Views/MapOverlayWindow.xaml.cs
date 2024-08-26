using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Effects;

namespace tfmStandalone.Views
{
	public partial class MapOverlayWindow : Window, IStyleConnector
	{
		public MapOverlayViewModel MapOverlayViewModel
		{
			get
			{
				return base.DataContext as MapOverlayViewModel;
			}
			set
			{
				base.DataContext = value;
			}
		}

		public MapOverlayWindow(MapOverlayViewModel viewModel)
		{
			this.InitializeComponent();
			this.MapOverlayViewModel = viewModel;
		}

		private void HandleMapClick(object sender, RoutedEventArgs e)
		{
			this.MapOverlayViewModel.IsCollapsed = true;
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[DebuggerNonUserCode]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 3)
			{
				((MenuItem)target).Click += this.HandleMapClick;
				return;
			}
			if (connectionId != 4)
			{
				return;
			}
			((MenuItem)target).Click += this.HandleMapClick;
		}
	}
}
