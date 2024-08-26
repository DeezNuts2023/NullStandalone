namespace tfmClientHook.Messages
{
	public sealed class S_ReportDisconnectedMessage : S_Message
	{
		public string Name { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(6);
			byteBuffer.WriteString(this.Name);
			return byteBuffer;
		}
	}
}
