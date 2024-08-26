namespace tfmClientHook.Messages
{
	public sealed class S_GenericChatMessage : S_Message
	{
		public string Message { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(6);
			byteBuffer.WriteByte(9);
			byteBuffer.WriteString(this.Message);
			return byteBuffer;
		}
	}
}
