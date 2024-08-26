using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace tfmStandalone
{
	public class ChatViewModel : BindableBase
	{
        private bool _isPinned;
        private bool _isMuted;
        private bool _newMessage;
        private string _community;
        private bool _isPreviousSelectedWHisper;
        private bool _isSelected;

        public string Name { get; set; }
		public string TabColor { get; set; }
		public string TabNameColor { get; set; }

		public bool IsWhisper { get; set; }
		public ObservableCollection<ChatMessageViewModel> ChatMessages { get; set; }
		public DelegateCommand LogCommand { get; }
		public DelegateCommand CasierCommand { get; }
		public DelegateCommand JoinCommand { get; }
		public DelegateCommand IgnoreCommand { get; }
		public DelegateCommand MumuteCommand { get; }
		public DelegateCommand KickCommand { get; }
		public DelegateCommand CloseAllWhispersCommand { get; }
		public DelegateCommand CloseAllReadWhispersCommand { get; }
		public DelegateCommand CloseChatCommand { get; }
		public DelegateCommand ToggleMuteChatCommand { get; }
		public DelegateCommand TogglePinnedCommand { get; }

        public bool IsPinned
        {
            get => _isPinned;
            set => SetProperty(ref _isPinned, value, "IsPinned");
        }

        public bool IsMuted
        {
            get => _isMuted;
            set => SetProperty(ref _isMuted, value, "IsMuted");
        }

        public bool HasNewMessages
        {
            get => _newMessage;
            set => SetProperty(ref _newMessage, value, "HasNewMessages");
        }

        public string Community
        {
            get => _community;
            set => SetProperty(ref _community, value, "Community");
        }

        public bool IsPreviousSelectedWhisper
        {
            get => _isPreviousSelectedWHisper;
            set => SetProperty(ref _isPreviousSelectedWHisper, value, "IsPreviousSelectedWhisper");
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value, "IsSelected");
        }

        public ChatViewModel(ChatWindowViewModel chatWindowViewModel)
		{
            ChatViewModel current = this;
			this.ChatMessages = new ObservableCollection<ChatMessageViewModel>();
			this.LogCommand = new DelegateCommand(delegate()
			{
				chatWindowViewModel.SendCommand("l " + current.Name);
			});
			this.CasierCommand = new DelegateCommand(delegate()
			{
				chatWindowViewModel.SendCommand("casier " + current.Name);
			});
			this.JoinCommand = new DelegateCommand(delegate()
			{
				chatWindowViewModel.SendCommand("join " + current.Name);
			});
			this.IgnoreCommand = new DelegateCommand(delegate()
			{
				chatWindowViewModel.SendClientCommand("tignore " + current.Name);
			});
			this.MumuteCommand = new DelegateCommand(delegate()
			{
				chatWindowViewModel.SendCommand("mumute " + current.Name);
			});
			this.KickCommand = new DelegateCommand(delegate()
			{
				chatWindowViewModel.SendCommand("kick " + current.Name);
			});
			this.CloseAllWhispersCommand = new DelegateCommand(new Action(chatWindowViewModel.CloseAllWhispers));
			this.CloseAllReadWhispersCommand = new DelegateCommand(new Action(chatWindowViewModel.CloseAllReadWhispers));
			this.CloseChatCommand = new DelegateCommand(delegate()
			{
				chatWindowViewModel.CloseChat(current.Name);
			});
			this.ToggleMuteChatCommand = new DelegateCommand(delegate()
			{
				this.IsMuted = !this.IsMuted;
			});
			this.TogglePinnedCommand = new DelegateCommand(delegate()
			{
				chatWindowViewModel.TogglePinChat(current.Name);
			});
		}

		public void AddMessage(string color, string message, bool showNewMessage)
		{
			this.ChatMessages.Add(new ChatMessageViewModel
			{
				Color = color,
				Message = message
			});
			if (!this.IsSelected && showNewMessage && !this.IsMuted)
			{
				this.HasNewMessages = true;
			}
		}

		public void AddHistory(string color, IEnumerable<string> messages)
		{
			foreach (string message in messages)
			{
				this.ChatMessages.Add(new ChatMessageViewModel
				{
					Color = color,
					Message = message
				});
			}
		}
	}
}
