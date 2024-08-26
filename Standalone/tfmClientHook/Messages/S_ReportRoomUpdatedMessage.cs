namespace tfmClientHook.Messages
{
	public sealed class S_ReportRoomUpdatedMessage : S_Message
	{
		public string Name { get; set; }
		public string Room { get; set; }
		public bool IsPassworded { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(4);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteString(this.Room);
			byteBuffer.WriteBool(this.IsPassworded);
			return byteBuffer;
		}
	}
}
