namespace tfmClientHook.Messages
{
	public sealed class S_WindowDisplayMessage : S_Message
	{
		public byte FontStyle { get; set; }
		public string WindowKey { get; set; }
		public string Text { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(28);
			byteBuffer.WriteByte(46);
			byteBuffer.WriteByte(this.FontStyle);
			byteBuffer.WriteString(this.WindowKey ?? string.Empty);
			byteBuffer.WriteLongString(this.Text);
			return byteBuffer;
		}
	}
}
