namespace tfmClientHook.Messages
{
	public class C_ModopwetReportListRequest : C_Message
	{
		public string Community { get; set; }
		public bool HasOnePersonReports { get; set; }
		public bool IsSortedByTime { get; set; }
		public bool IsForwardSorted { get; set; }

        public override bool IsEncrypted => false;

        public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(26);
			byteBuffer.WriteString(this.Community);
			byteBuffer.WriteBool(this.HasOnePersonReports);
			byteBuffer.WriteBool(this.IsSortedByTime);
			byteBuffer.WriteBool(this.IsForwardSorted);
			return byteBuffer;
		}
	}
}
