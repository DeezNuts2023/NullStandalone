namespace tfmClientHook
{
	public sealed class OutgoingTribullePackets
	{
		public short ChatMessage { get; set; }
		public short TribeMessage { get; set; }
		public short WhisperMessage { get; set; }
		public short FriendListOpening { get; set; }
		public short FriendListClosing { get; set; }
	}
}
