using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace tfmStandalone.Views
{
    public partial class CommunityOverlayWindow : Window
    {
        public CommunityOverlayViewModel ViewModel
        {
            get
            {
                return base.DataContext as CommunityOverlayViewModel;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public CommunityOverlayWindow(CommunityOverlayViewModel viewModel)
        {
            this.InitializeComponent();
            this.ViewModel = viewModel;
        }
		
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            this.ViewModel.IsCollapsed = true;
        }
    }
}
