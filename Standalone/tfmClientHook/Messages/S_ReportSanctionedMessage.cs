namespace tfmClientHook.Messages
{
	public sealed class S_ReportSanctionedMessage : S_Message
	{
		public string Name { get; set; }
		public bool IsBan { get; set; }
		public string SanctionGiver { get; set; }
		public int SanctionLength { get; set; }
		public string SanctionReason { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(5);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteBool(this.IsBan);
			byteBuffer.WriteString(this.SanctionGiver);
			byteBuffer.WriteInt(this.SanctionLength);
			byteBuffer.WriteString(this.SanctionReason);
			return byteBuffer;
		}
	}
}
