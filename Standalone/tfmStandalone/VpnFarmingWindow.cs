using System;
using System.Windows.Markup;

namespace tfmStandalone
{
	public partial class VpnFarmingWindow : PinnableWindow, IComponentConnector
	{
		public VpnFarmingWindow(VpnFarmingWindowViewModel viewModel)
		{
			this.InitializeComponent();
			base.DataContext = viewModel;
			viewModel.Closed = (EventHandler)Delegate.Combine(viewModel.Closed, new EventHandler(delegate(object sender, EventArgs e)
			{
				base.Close();
			}));
		}
	}
}
