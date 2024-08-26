using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class CaseViewModel : BindableBase
	{
        private string _room;
        private bool _isRoomLocked;
        private bool _isRoomTribehouse;
        private string _currentWatchers;
        private bool _isDeleted;
        private string _sanctionRemovalReason;
        private bool _isSelected;
        private string _chatLog;
        public string Name { get; set; }
		public bool IsSameRoom { get; set; }
		public string Community { get; set; }
		public int Hours { get; set; }
		public short NewestReportTime { get; set; }
		public short OldestReportTime { get; set; }
		public string ReportTypes { get; set; }
		public int ReportSeverity { get; set; }
        public ObservableCollection<ReportViewModel> Reports { get; }
        public bool IsSanctioned { get; set; }
        public string SanctionText { get; set; }

        public string Room
        {
            get => this._room;
            set => this.SetProperty(ref this._room, value, "Room");
        }

        public bool IsRoomLocked
        {
            get => this._isRoomLocked;
            set => this.SetProperty(ref this._isRoomLocked, value, "IsRoomLocked");
        }

        public bool IsRoomTribehouse
        {
            get => this._isRoomTribehouse;
            set => this.SetProperty(ref this._isRoomTribehouse, value, "IsRoomTribehouse");
        }

        public string CurrentWatchers
        {
            get => this._currentWatchers;
            set => this.SetProperty(ref this._currentWatchers, value, "CurrentWatchers");
        }

        public bool IsDeleted
        {
            get => this._isDeleted;
            set => this.SetProperty(ref this._isDeleted, value, "IsDeleted");
        }

        public string SanctionRemovalReason
        {
            get => this._sanctionRemovalReason;
            set => this.SetProperty(ref this._sanctionRemovalReason, value, "SanctionRemovalReason");
        }

        public bool IsSelected
        {
            get => this._isSelected;
            set => this.SetProperty(ref this._isSelected, value, "IsSelected");
        }

        public string ChatLog
        {
            get => this._chatLog;
            set => this.SetProperty(ref this._chatLog, value, "ChatLog");
        }

		public CaseViewModel()
		{
			this.Reports = new ObservableCollection<ReportViewModel>();
		}
	}
}
