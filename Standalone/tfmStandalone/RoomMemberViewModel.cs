using System.Windows.Media;

namespace tfmStandalone
{
	public sealed class RoomMemberViewModel
	{
		public string Name { get; set; }
		public string Ip { get; set; }
		public SolidColorBrush IpColor { get; set; }
		public string Country { get; set; }
	}
}
