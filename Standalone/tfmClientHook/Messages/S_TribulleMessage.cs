namespace tfmClientHook.Messages
{
	public abstract class S_TribulleMessage : S_Message
	{
        public abstract short TribulleId { get; }

        public override ByteBuffer GetBuffer()
		{
			return null;
		}
	}
}
