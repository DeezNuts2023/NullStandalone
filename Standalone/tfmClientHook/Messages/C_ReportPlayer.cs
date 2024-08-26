namespace tfmClientHook.Messages
{
	public class C_ReportPlayer : C_Message
	{
		public string Name { get; set; }
		public ReportType Type { get; set; }
		public string Comment { get; set; }

        public override bool IsEncrypted => false;

        public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(8);
			byteBuffer.WriteByte(25);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteByte((byte)this.Type);
			byteBuffer.WriteByte(0);
			if (string.IsNullOrEmpty(this.Comment))
			{
				byteBuffer.WriteByte(0);
			}
			else
			{
				byteBuffer.WriteString(this.Comment);
			}
			return byteBuffer;
		}
	}
}
