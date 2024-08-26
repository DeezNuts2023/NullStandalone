namespace tfmClientHook.Messages
{
	public sealed class S_TribeMessage : S_TribulleMessage
	{
        public override short TribulleId => ServerInfo.IncomingTribulleCodes.TribeMessage;

        public string Name { get; set; }
		public string Message { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(60);
			byteBuffer.WriteByte(3);
			byteBuffer.WriteShort(this.TribulleId);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteString(this.Message);
			return byteBuffer;
		}
	}
}
