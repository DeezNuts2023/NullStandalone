using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Practices.Unity;

namespace tfmStandalone
{
	public partial class VpnFarmingRoomWindow : PinnableWindow, IComponentConnector
	{
		[Dependency]
        public VpnFarmingRoomWindowViewModel ViewModel
        {
            get => base.DataContext as VpnFarmingRoomWindowViewModel;
            set => base.DataContext = value;
        }

        public VpnFarmingRoomWindow()
		{
			this.InitializeComponent();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			this.ViewModel.Closing();
		}
	}
}
