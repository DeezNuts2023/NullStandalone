using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace tfmStandalone
{
	public partial class SanctionGridWindow : Window, IComponentConnector
	{
        internal HeaderScrollViewer HeaderScrollViewer;

        public SanctionGridViewModel ViewModel
        {
            get => base.DataContext as SanctionGridViewModel;
            set => base.DataContext = value;
        }

        public SanctionGridWindow(SanctionGridViewModel viewModel)
		{
			this.InitializeComponent();
			this.ViewModel = viewModel;
		}

		private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			this.HeaderScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
			this.RowHeaderScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
		}

		private void RowHeaderScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (this.ContentScrollViewer.VerticalOffset != e.VerticalOffset)
			{
				this.ContentScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
			}
		}
	}
}
