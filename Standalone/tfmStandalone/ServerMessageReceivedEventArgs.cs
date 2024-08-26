using System;

namespace tfmStandalone
{
	public sealed class ServerMessageReceivedEventArgs : EventArgs
	{
		public bool IsBuffyBan { get; set; }
		public string Message { get; set; }
	}
}
