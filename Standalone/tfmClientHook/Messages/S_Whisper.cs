namespace tfmClientHook.Messages
{
	public sealed class S_Whisper : S_TribulleMessage
	{
        public override short TribulleId => ServerInfo.IncomingTribulleCodes.WhisperMessage;

        public string SenderName { get; set; }
		public int Community { get; set; }
		public string ReceiverName { get; set; }
		public string Message { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(60);
			byteBuffer.WriteByte(3);
			byteBuffer.WriteShort(this.TribulleId);
			byteBuffer.WriteString(this.SenderName);
			byteBuffer.WriteInt(this.Community);
			byteBuffer.WriteString(this.ReceiverName);
			byteBuffer.WriteString(this.Message);
			return byteBuffer;
		}
	}
}
