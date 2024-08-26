using System.Collections.Generic;

namespace tfmClientHook.Messages
{
	public sealed class S_ReportCountsUpdateMessage : S_Message
	{
		public List<S_ReportCountsUpdateMessage.ReportCount> ReportCounts { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(3);
			byteBuffer.WriteByte((byte)this.ReportCounts.Count);
			foreach (S_ReportCountsUpdateMessage.ReportCount reportCount in this.ReportCounts)
			{
				byteBuffer.WriteByte((byte)reportCount.Community);
				byteBuffer.WriteByte((byte)reportCount.Count);
			}
			return byteBuffer;
		}

		public sealed class ReportCount
		{
			public Community Community { get; set; }
			public int Count { get; set; }
		}
	}
}
