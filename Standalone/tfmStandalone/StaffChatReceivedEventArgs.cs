using System;
using tfmClientHook;

namespace tfmStandalone
{
	public sealed class StaffChatReceivedEventArgs : EventArgs
	{
		public StaffChatType Type { get; set; }
		public string Name { get; set; }
		public string Message { get; set; }
		public bool GeneralChatMessage { get; set; }
	}
}
