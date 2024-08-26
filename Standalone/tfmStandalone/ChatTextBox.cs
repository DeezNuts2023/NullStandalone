using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace tfmStandalone
{
	public class ChatTextBox : RichTextBox
	{
        public static readonly DependencyProperty ChatListProperty = DependencyProperty.Register("ChatList", typeof(ObservableCollection<ChatMessageViewModel>), typeof(ChatTextBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ChatTextBox.OnChatListChanged)));

        public ObservableCollection<ChatMessageViewModel> ChatList
        {
            get => (ObservableCollection<ChatMessageViewModel>)base.GetValue(ChatTextBox.ChatListProperty);
            set => base.SetValue(ChatTextBox.ChatListProperty, value);
        }

        public static void OnChatListChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			ChatTextBox textBox = (ChatTextBox)obj;
			ObservableCollection<ChatMessageViewModel> observableCollection = args.NewValue as ObservableCollection<ChatMessageViewModel>;
			if (observableCollection != null)
			{
				FlowDocument flowDocument = new FlowDocument();
				BrushConverter brushConverter = new BrushConverter();
				observableCollection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
				{
					foreach (object obj2 in e.NewItems)
					{
						ChatMessageViewModel chatMessageViewModel2 = (ChatMessageViewModel)obj2;
						Paragraph paragraph2 = new Paragraph(new Run(chatMessageViewModel2.Message));
						paragraph2.Foreground = (SolidColorBrush)brushConverter.ConvertFrom("#FF" + chatMessageViewModel2.Color);
						flowDocument.Blocks.Add(paragraph2);
					}
					textBox.ScrollToEnd();
				};
				foreach (ChatMessageViewModel chatMessageViewModel in observableCollection)
				{
					Paragraph paragraph = new Paragraph(new Run(chatMessageViewModel.Message));
					paragraph.Foreground = (SolidColorBrush)brushConverter.ConvertFrom("#FF" + chatMessageViewModel.Color);
					flowDocument.Blocks.Add(paragraph);
				}
				textBox.Document = flowDocument;
				textBox.ScrollToEnd();
				return;
			}
			FlowDocument document = new FlowDocument();
			textBox.Document = document;
		}
	}
}
