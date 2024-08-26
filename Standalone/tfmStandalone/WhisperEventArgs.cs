using System;

namespace tfmStandalone
{
	public sealed class WhisperEventArgs : EventArgs
	{
		public string Name { get; set; }
		public string Message { get; set; }
		public int Community { get; set; }
	}
}
