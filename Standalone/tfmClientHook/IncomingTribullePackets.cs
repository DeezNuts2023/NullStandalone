namespace tfmClientHook
{
	public sealed class IncomingTribullePackets
	{
		public short ChatMessageResult { get; set; }
		public short TribeMessageResult { get; set; }
		public short WhisperMessageResult { get; set; }
		public short ChatMessage { get; set; }
		public short TribeMessage { get; set; }
		public short WhisperMessage { get; set; }
		public short FriendListOpeningResult { get; set; }
		public short FriendListClosingResult { get; set; }
		public short FriendConnected { get; set; }
		public short FriendDisconnected { get; set; }
		public short FriendList { get; set; }
	}
}
