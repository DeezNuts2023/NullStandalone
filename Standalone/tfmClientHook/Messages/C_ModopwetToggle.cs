namespace tfmClientHook.Messages
{
	public class C_ModopwetToggle : C_Message
	{
		public bool IsRunning { get; set; }

        public override bool IsEncrypted => false;

        public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(2);
			byteBuffer.WriteBool(this.IsRunning);
			return byteBuffer;
		}
	}
}
