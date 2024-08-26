using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace tfmStandalone
{
	public partial class LogWindow : PinnableWindow
	{
        private ScrollViewer _scrollViewer;
        internal LogWindow Window;
        private LogWindow.LogFlowDocument CurrentLogFlowDocument { get; set; }
        private Dictionary<LogViewModel, LogWindow.LogFlowDocument> FlowDocuments { get; }

        public LogWindowViewModel LogWindowViewModel
        {
            get => base.DataContext as LogWindowViewModel;
            set => base.DataContext = value;
        }

        private ScrollViewer ScrollViewer => _scrollViewer ??= FlowDocumentScrollViewer.Template.FindName("PART_ContentHost", FlowDocumentScrollViewer) as ScrollViewer;

		public LogWindow(LogWindowViewModel viewModel)
		{
			this.InitializeComponent();
			this.LogWindowViewModel = viewModel;
			this.FlowDocuments = new Dictionary<LogViewModel, LogWindow.LogFlowDocument>();
			LogWindowViewModel logWindowViewModel = this.LogWindowViewModel;
			logWindowViewModel.NewLogReceived = (EventHandler)Delegate.Combine(logWindowViewModel.NewLogReceived, new EventHandler(this.NewLogReceived));
			LogWindowViewModel logWindowViewModel2 = this.LogWindowViewModel;
			logWindowViewModel2.NewLogSelected = (EventHandler)Delegate.Combine(logWindowViewModel2.NewLogSelected, new EventHandler(this.NewLogSelected));
			LogWindowViewModel logWindowViewModel3 = this.LogWindowViewModel;
			logWindowViewModel3.LogClosed = (EventHandler<LogViewModel>)Delegate.Combine(logWindowViewModel3.LogClosed, new EventHandler<LogViewModel>(this.LogClosed));
			LogWindowViewModel logWindowViewModel4 = this.LogWindowViewModel;
			logWindowViewModel4.Closed = (EventHandler)Delegate.Combine(logWindowViewModel4.Closed, new EventHandler(this.LogsClosed));
			if (viewModel.SelectedLog != null)
			{
				this.NewLogSelected(null, null);
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			LogWindowViewModel logWindowViewModel = this.LogWindowViewModel;
			logWindowViewModel.NewLogReceived = (EventHandler)Delegate.Remove(logWindowViewModel.NewLogReceived, new EventHandler(this.NewLogReceived));
			LogWindowViewModel logWindowViewModel2 = this.LogWindowViewModel;
			logWindowViewModel2.NewLogSelected = (EventHandler)Delegate.Remove(logWindowViewModel2.NewLogSelected, new EventHandler(this.NewLogSelected));
			LogWindowViewModel logWindowViewModel3 = this.LogWindowViewModel;
			logWindowViewModel3.LogClosed = (EventHandler<LogViewModel>)Delegate.Remove(logWindowViewModel3.LogClosed, new EventHandler<LogViewModel>(this.LogClosed));
			LogWindowViewModel logWindowViewModel4 = this.LogWindowViewModel;
			logWindowViewModel4.Closed = (EventHandler)Delegate.Remove(logWindowViewModel4.Closed, new EventHandler(this.LogsClosed));
		}

		private void NewLogReceived(object sender, EventArgs eventArgs)
		{
			if (base.WindowState == WindowState.Minimized)
			{
				base.WindowState = WindowState.Normal;
			}
			base.Activate();
		}

		private void NewLogSelected(object sender, EventArgs eventArgs)
		{
			LogWindowViewModel logWindowViewModel = this.LogWindowViewModel;
			LogViewModel selectedLog = logWindowViewModel.SelectedLog;
			if (selectedLog == null)
			{
				return;
			}
			if (!this.FlowDocuments.ContainsKey(selectedLog))
			{
				FlowDocument flowDocument = new FlowDocument
				{
					FontFamily = new FontFamily("Verdana"),
					PagePadding = new Thickness(10.0)
				};
				Paragraph paragraph = new Paragraph
				{
					FontSize = 12.0,
					Margin = new Thickness(0.0),
					TextAlignment = TextAlignment.Center
				};
				if (selectedLog.IsPlayer)
				{
					paragraph.Inlines.Add(this.CreateRun("Tfm_N", "Connection logs for player "));
					paragraph.Inlines.Add(this.CreateRun("Tfm_BL", selectedLog.Key));
				}
				else
				{
					paragraph.Inlines.Add(this.CreateRun("Tfm_N", "Connection logs for IP address "));
					paragraph.Inlines.Add(this.CreateRun("Tfm_V", selectedLog.Key));
				}
				flowDocument.Blocks.Add(paragraph);
				flowDocument.Blocks.Add(new Paragraph
				{
					Margin = new Thickness(0.0),
					FontSize = 12.0
				});
				using (List<LoginViewModel>.Enumerator enumerator = selectedLog.Logins.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						LoginViewModel l = enumerator.Current;
						Paragraph paragraph2 = new Paragraph
						{
							FontSize = 12.0,
							Margin = new Thickness(0.0, 2.0, 0.0, 0.0)
						};
						paragraph2.Inlines.Add(this.CreateRun("Tfm_V", "[ "));
						Hyperlink hyperlink = new Hyperlink(this.CreateRun("Tfm_V", l.Name))
						{
							TextDecorations = null,
							ContextMenu = this.CreateNameContextMenu(l.Name, false)
						};
						hyperlink.Click += delegate(object o, RoutedEventArgs args)
						{
							logWindowViewModel.RequestLog(l.Name, 0);
						};
						paragraph2.Inlines.Add(hyperlink);
						paragraph2.Inlines.Add(this.CreateRun("Tfm_V", " ]  "));
						paragraph2.Inlines.Add(this.CreateRun("Tfm_BL", string.Format("{0}  ", l.Date)));
						paragraph2.Inlines.Add(this.CreateRun("Tfm_G", "( "));
						Hyperlink hyperlink2 = new Hyperlink(new Run
						{
							Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(l.IPColor),
							Text = l.IP
						})
						{
							TextDecorations = null,
							ContextMenu = this.CreateNameContextMenu(l.IP, true)
						};
						hyperlink2.Click += delegate(object o, RoutedEventArgs args)
						{
							logWindowViewModel.RequestLog(l.IP, 0);
						};
						paragraph2.Inlines.Add(hyperlink2);
						paragraph2.Inlines.Add(this.CreateRun("Tfm_G", string.Format(" - {0} )  {1}", l.Country, l.Type)));
						if (!string.IsNullOrEmpty(l.Community))
						{
							paragraph2.Inlines.Add(this.CreateRun("Tfm_G", string.Format(" - {0}", l.Community)));
						}
						flowDocument.Blocks.Add(paragraph2);
					}
				}
				this.FlowDocuments.Add(selectedLog, new LogWindow.LogFlowDocument
				{
					FlowDocument = flowDocument,
					VerticalOffset = 0.0
				});
			}
			if (this.CurrentLogFlowDocument != null && this.ScrollViewer != null)
			{
				this.CurrentLogFlowDocument.VerticalOffset = this.ScrollViewer.VerticalOffset;
			}
			this.CurrentLogFlowDocument = this.FlowDocuments[selectedLog];
			this.FlowDocumentScrollViewer.Document = this.CurrentLogFlowDocument.FlowDocument;
			if (this.ScrollViewer != null)
			{
				this.ScrollViewer.ScrollToVerticalOffset(this.CurrentLogFlowDocument.VerticalOffset);
				this.ScrollViewer.UpdateLayout();
			}
		}

		private void LogClosed(object sender, LogViewModel logViewModel)
		{
			if (this.FlowDocuments.ContainsKey(logViewModel))
			{
				this.FlowDocuments.Remove(logViewModel);
			}
		}

		private void LogsClosed(object sender, EventArgs eventArgs)
		{
			base.Close();
		}

		private Run CreateRun(string foregroundResourceName, string Text)
		{
			Run run = new Run();
			run.Text = Text;
			run.SetResourceReference(TextElement.ForegroundProperty, foregroundResourceName);
			return run;
		}

		private ContextMenu CreateNameContextMenu(string name, bool isIP)
		{
			ContextMenu contextMenu = new ContextMenu();
			MenuItem menuItem = new MenuItem
			{
				Header = "Casier"
			};
			menuItem.Click += delegate(object sender, RoutedEventArgs args)
			{
				this.LogWindowViewModel.SendCommand(string.Format("casier {0}", name));
			};
			contextMenu.Items.Add(menuItem);
			if (isIP)
			{
				MenuItem menuItem2 = new MenuItem
				{
					Header = "IPNom"
				};
				menuItem2.Click += delegate(object sender, RoutedEventArgs args)
				{
					this.LogWindowViewModel.SendCommand(string.Format("ipnom {0}", name));
				};
				contextMenu.Items.Add(menuItem2);
			}
			else
			{
				MenuItem menuItem3 = new MenuItem
				{
					Header = "NomIP"
				};
				menuItem3.Click += delegate(object sender, RoutedEventArgs args)
				{
					this.LogWindowViewModel.SendCommand(string.Format("nomip {0}", name));
				};
				contextMenu.Items.Add(menuItem3);
			}
			contextMenu.Items.Add(new Separator());
			MenuItem menuItem4 = new MenuItem
			{
				Header = "Log (500)"
			};
			menuItem4.Click += delegate(object sender, RoutedEventArgs args)
			{
				this.LogWindowViewModel.RequestLog(name, 500);
			};
			contextMenu.Items.Add(menuItem4);
			MenuItem menuItem5 = new MenuItem
			{
				Header = "Log (1000)"
			};
			menuItem5.Click += delegate(object sender, RoutedEventArgs args)
			{
				this.LogWindowViewModel.RequestLog(name, 1000);
			};
			contextMenu.Items.Add(menuItem5);
			return contextMenu;
		}
        private sealed class LogFlowDocument
        {
            public FlowDocument FlowDocument { get; set; }
            public double VerticalOffset { get; set; }
        }
    }
}
