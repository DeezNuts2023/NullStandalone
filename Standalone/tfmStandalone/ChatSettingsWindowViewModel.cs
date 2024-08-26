using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Prism.Commands;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class ChatSettingsWindowViewModel : BindableBase
	{
        public EventHandler Closed;
        private bool _filterModoChat;
        private bool _alertModoChat;
        private bool _logModoChat;
        private bool _filterArbChat;
        private bool _alertArbChat;
        private bool _logArbChat;
        private bool _filterServeurMessages;
        private bool _alertServeurMessages;
        private bool _logServeurMessages;
        private bool _filterMapCrewChat;
        private bool _alertMapCrewChat;
        private bool _logMapCrewChat;
        private bool _filterLuaTeamChat;
        private bool _alertLuaTeamChat;
        private bool _logLuaTeamChat;
        private bool _filterFunCorpChat;
        private bool _alertFunCorpChat;
        private bool _logFunCorpChat;
        private bool _filterFashionSquadChat;
        private bool _alertFashionSquadChat;
        private bool _logFashionSquadChat;
        private bool _filterTribeChat;
        private bool _alertTribeChat;
        private bool _logTribeChat;
        private bool _filterWhispers;
        private bool _alertWhispers;
        private bool _logWhispers;
        private FontFamily _fontFamily;
        private int _fontSize;
        private GameSettings GameSettings { get; }
        public ObservableCollection<int> FontSizes { get; }

		public DelegateCommand SaveCommand { get; set; }

        public bool FilterModoChat
        {
            get => _filterModoChat;
            set => SetProperty(ref _filterModoChat, value, "FilterModoChat");
        }

        public bool AlertModoChat
        {
            get => _alertModoChat;
            set => SetProperty(ref _alertModoChat, value, "AlertModoChat");
        }

        public bool LogModoChat
        {
            get => _logModoChat;
            set => SetProperty(ref _logModoChat, value, "LogModoChat");
        }

        public bool FilterArbChat
        {
            get => _filterArbChat;
            set => SetProperty(ref _filterArbChat, value, "FilterArbChat");
        }

        public bool AlertArbChat
        {
            get => _alertArbChat;
            set => SetProperty(ref _alertArbChat, value, "AlertArbChat");
        }

        public bool LogArbChat
        {
            get => _logArbChat;
            set => SetProperty(ref _logArbChat, value, "LogArbChat");
        }

        public bool FilterServeurMessages
        {
            get => _filterServeurMessages;
            set => SetProperty(ref _filterServeurMessages, value, "FilterServeurMessages");
        }

        public bool AlertServeurMessages
        {
            get => _alertServeurMessages;
            set => SetProperty(ref _alertServeurMessages, value, "AlertServeurMessages");
        }

        public bool LogServeurMessages
        {
            get => _logServeurMessages;
            set => SetProperty(ref _logServeurMessages, value, "LogServeurMessages");
        }

        public bool FilterMapCrewChat
        {
            get => _filterMapCrewChat;
            set => SetProperty(ref _filterMapCrewChat, value, "FilterMapCrewChat");
        }

        public bool AlertMapCrewChat
        {
            get => _alertMapCrewChat;
            set => SetProperty(ref _alertMapCrewChat, value, "AlertMapCrewChat");
        }

        public bool LogMapCrewChat
        {
            get => _logMapCrewChat;
            set => SetProperty(ref _logMapCrewChat, value, "LogMapCrewChat");
        }

        public bool FilterLuaTeamChat
        {
            get => _filterLuaTeamChat;
            set => SetProperty(ref _filterLuaTeamChat, value, "FilterLuaTeamChat");
        }

        public bool AlertLuaTeamChat
        {
            get => _alertLuaTeamChat;
            set => SetProperty(ref _alertLuaTeamChat, value, "AlertLuaTeamChat");
        }

        public bool LogLuaTeamChat
        {
            get => _logLuaTeamChat;
            set => SetProperty(ref _logLuaTeamChat, value, "LogLuaTeamChat");
        }

        public bool FilterFunCorpChat
        {
            get => _filterFunCorpChat;
            set => SetProperty(ref _filterFunCorpChat, value, "FilterFunCorpChat");
        }

        public bool AlertFunCorpChat
        {
            get => _alertFunCorpChat;
            set => SetProperty(ref _alertFunCorpChat, value, "AlertFunCorpChat");
        }

        public bool LogFunCorpChat
        {
            get => _logFunCorpChat;
            set => SetProperty(ref _logFunCorpChat, value, "LogFunCorpChat");
        }

        public bool FilterFashionSquadChat
        {
            get => _filterFashionSquadChat;
            set => SetProperty(ref _filterFashionSquadChat, value, "FilterFashionSquadChat");
        }

        public bool AlertFashionSquadChat
        {
            get => _alertFashionSquadChat;
            set => SetProperty(ref _alertFashionSquadChat, value, "AlertFashionSquadChat");
        }

        public bool LogFashionSquadChat
        {
            get => _logFashionSquadChat;
            set => SetProperty(ref _logFashionSquadChat, value, "LogFashionSquadChat");
        }

        public bool FilterTribeChat
        {
            get => _filterTribeChat;
            set => SetProperty(ref _filterTribeChat, value, "FilterTribeChat");
        }

        public bool AlertTribeChat
        {
            get => _alertTribeChat;
            set => SetProperty(ref _alertTribeChat, value, "AlertTribeChat");
        }

        public bool LogTribeChat
        {
            get => _logTribeChat;
            set => SetProperty(ref _logTribeChat, value, "LogTribeChat");
        }

        public bool FilterWhispers
        {
            get => _filterWhispers;
            set => SetProperty(ref _filterWhispers, value, "FilterWhispers");
        }

        public bool AlertWhispers
        {
            get => _alertWhispers;
            set => SetProperty(ref _alertWhispers, value, "AlertWhispers");
        }

        public bool LogWhispers
        {
            get => _logWhispers;
            set => SetProperty(ref _logWhispers, value, "LogWhispers");
        }

        public FontFamily FontFamily
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value, "FontFamily");
        }

        public int FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value, "FontSize");
        }

        public ChatSettingsWindowViewModel(GameSettings gameSettings)
		{
			this.GameSettings = gameSettings;
			this.FilterModoChat = this.GameSettings.FilterModoChat;
			this.AlertModoChat = this.GameSettings.AlertModoChat;
			this.LogModoChat = this.GameSettings.LogModoChat;
			this.FilterArbChat = this.GameSettings.FilterArbChat;
			this.AlertArbChat = this.GameSettings.AlertArbChat;
			this.LogArbChat = this.GameSettings.LogArbChat;
			this.FilterServeurMessages = this.GameSettings.FilterServeurMessages;
			this.AlertServeurMessages = this.GameSettings.AlertServeurMessages;
			this.LogServeurMessages = this.GameSettings.LogServeurMessages;
			this.FilterMapCrewChat = this.GameSettings.FilterMapCrewChat;
			this.AlertMapCrewChat = this.GameSettings.AlertMapCrewChat;
			this.LogMapCrewChat = this.GameSettings.LogMapCrewChat;
			this.FilterLuaTeamChat = this.GameSettings.FilterLuaTeamChat;
			this.AlertLuaTeamChat = this.GameSettings.AlertLuaTeamChat;
			this.LogLuaTeamChat = this.GameSettings.LogLuaTeamChat;
			this.FilterFunCorpChat = this.GameSettings.FilterFunCorpChat;
			this.AlertFunCorpChat = this.GameSettings.AlertFunCorpChat;
			this.LogFunCorpChat = this.GameSettings.LogFunCorpChat;
			this.FilterFashionSquadChat = this.GameSettings.FilterFashionSquadChat;
			this.AlertFashionSquadChat = this.GameSettings.AlertFashionSquadChat;
			this.LogFashionSquadChat = this.GameSettings.LogFashionSquadChat;
			this.FilterTribeChat = this.GameSettings.FilterTribeChat;
			this.AlertTribeChat = this.GameSettings.AlertTribeChat;
			this.LogTribeChat = this.GameSettings.LogTribeChat;
			this.FilterWhispers = this.GameSettings.FilterWhispers;
			this.AlertWhispers = this.GameSettings.AlertWhispers;
			this.LogWhispers = this.GameSettings.LogWhispers;
			this.FontFamily = this.GameSettings.FontFamily;
			this.FontSize = this.GameSettings.FontSize;
			this.FontSizes = new ObservableCollection<int>
			{
				22,
				21,
				20,
				19,
				18,
				17,
				16,
				15,
				14,
				13,
				12,
				11,
				10,
				9,
				8,
				7,
				6
			};
			this.SaveCommand = new DelegateCommand(new Action(this.Save));
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x0000E368 File Offset: 0x0000C568
		private void Save()
		{
			this.GameSettings.FilterModoChat = this.FilterModoChat;
			this.GameSettings.AlertModoChat = this.AlertModoChat;
			this.GameSettings.LogModoChat = this.LogModoChat;
			this.GameSettings.FilterArbChat = this.FilterArbChat;
			this.GameSettings.AlertArbChat = this.AlertArbChat;
			this.GameSettings.LogArbChat = this.LogArbChat;
			this.GameSettings.FilterServeurMessages = this.FilterServeurMessages;
			this.GameSettings.AlertServeurMessages = this.AlertServeurMessages;
			this.GameSettings.LogServeurMessages = this.LogServeurMessages;
			this.GameSettings.FilterMapCrewChat = this.FilterMapCrewChat;
			this.GameSettings.AlertMapCrewChat = this.AlertMapCrewChat;
			this.GameSettings.LogMapCrewChat = this.LogMapCrewChat;
			this.GameSettings.FilterLuaTeamChat = this.FilterLuaTeamChat;
			this.GameSettings.AlertLuaTeamChat = this.AlertLuaTeamChat;
			this.GameSettings.LogLuaTeamChat = this.LogLuaTeamChat;
			this.GameSettings.FilterFunCorpChat = this.FilterFunCorpChat;
			this.GameSettings.AlertFunCorpChat = this.AlertFunCorpChat;
			this.GameSettings.LogFunCorpChat = this.LogFunCorpChat;
			this.GameSettings.FilterFashionSquadChat = this.FilterFashionSquadChat;
			this.GameSettings.AlertFashionSquadChat = this.AlertFashionSquadChat;
			this.GameSettings.LogFashionSquadChat = this.LogFashionSquadChat;
			this.GameSettings.FilterTribeChat = this.FilterTribeChat;
			this.GameSettings.AlertTribeChat = this.AlertTribeChat;
			this.GameSettings.LogTribeChat = this.LogTribeChat;
			this.GameSettings.FilterWhispers = this.FilterWhispers;
			this.GameSettings.AlertWhispers = this.AlertWhispers;
			this.GameSettings.LogWhispers = this.LogWhispers;
			this.GameSettings.FontFamily = this.FontFamily;
			this.GameSettings.FontSize = this.FontSize;
			this.GameSettings.Save();
			EventHandler closed = this.Closed;
			if (closed == null)
			{
				return;
			}
			closed(this, new EventArgs());
		}
	}
}
