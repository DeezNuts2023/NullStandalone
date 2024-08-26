namespace tfmClientHook.Messages
{
	public class C_Generic : C_Message
	{
		public ByteBuffer ByteBuffer { get; }

		public override bool IsEncrypted { get; }

		public C_Generic(bool isEncrypted)
		{
			this.ByteBuffer = new ByteBuffer();
			this.IsEncrypted = isEncrypted;
		}

		public override ByteBuffer GetBuffer()
		{
			return this.ByteBuffer;
		}
	}
}
