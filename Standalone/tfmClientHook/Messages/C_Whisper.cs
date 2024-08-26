namespace tfmClientHook.Messages
{
	public class C_Whisper : C_TribulleMessage
	{
        public int SequenceId { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }

        public override short TribulleId => ServerInfo.OutgoingTribulleCodes.WhisperMessage;

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(60);
			byteBuffer.WriteByte(3);
			byteBuffer.WriteShort(this.TribulleId);
			byteBuffer.WriteInt(this.SequenceId);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteString(this.Message);
			return byteBuffer;
		}
	}
}
