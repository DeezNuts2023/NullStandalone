using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace tfmStandalone
{
	public sealed class RoomViewModel : BindableBase
	{
        private int _count;
        public string FullName { get; set; }
		public string Community { get; set; }
		public string RoomName { get; set; }

        public int Count
        {
            get => this._count;
            set => this.SetProperty(ref this._count, value, "Count");
        }
        public bool IsInternational => this.Community == "xx";

        public ObservableCollection<RoomMemberViewModel> Members { get; }

        public RoomViewModel()
		{
			this.Members = new ObservableCollection<RoomMemberViewModel>();
		}
	}
}
