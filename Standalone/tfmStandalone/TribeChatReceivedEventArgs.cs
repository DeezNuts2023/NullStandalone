using System;

namespace tfmStandalone
{
	public sealed class TribeChatReceivedEventArgs : EventArgs
	{
		public string Message { get; set; }
	}
}
