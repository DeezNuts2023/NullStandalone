using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace tfmStandalone
{
	public partial class SanctionOverlayWindow : Window, IComponentConnector
	{
		public SanctionOverlayWindow()
		{
			this.InitializeComponent();
		}
	}
}
