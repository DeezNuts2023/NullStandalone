namespace tfmClientHook.Messages
{
	public class C_ModopwetDeleteReport : C_Message
	{
		public string Name { get; set; }
		public bool IsHandled { get; set; }

        public override bool IsEncrypted => false;

        public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(23);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteBool(this.IsHandled);
			return byteBuffer;
		}
	}
}
