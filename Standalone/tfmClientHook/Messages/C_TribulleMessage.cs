namespace tfmClientHook.Messages
{
	public abstract class C_TribulleMessage : C_Message
	{
        public abstract short TribulleId { get; }

        public override bool IsEncrypted => true;

        public override ByteBuffer GetBuffer()
		{
			return null;
		}
	}
}
