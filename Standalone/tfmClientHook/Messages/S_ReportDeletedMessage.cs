namespace tfmClientHook.Messages
{
	public sealed class S_ReportDeletedMessage : S_Message
	{
		public string Name { get; set; }
		public string Deleter { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(7);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteString(this.Deleter);
			return byteBuffer;
		}
	}
}
