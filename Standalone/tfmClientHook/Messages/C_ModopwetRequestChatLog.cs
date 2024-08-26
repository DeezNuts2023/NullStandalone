namespace tfmClientHook.Messages
{
	public class C_ModopwetRequestChatLog : C_Message
	{
		public string Name { get; set; }

        public override bool IsEncrypted => false;

        public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(27);
			byteBuffer.WriteString(this.Name);
			return byteBuffer;
		}
	}
}
