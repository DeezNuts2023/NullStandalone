namespace tfmClientHook.Messages
{
	public class C_ModopwetWatch : C_Message
	{
		public string Name { get; set; }
		public bool IsFollowing { get; set; }

        public override bool IsEncrypted => false;

        public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(24);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteBool(this.IsFollowing);
			return byteBuffer;
		}
	}
}
