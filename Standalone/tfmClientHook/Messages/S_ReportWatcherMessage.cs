using System.Collections.Generic;

namespace tfmClientHook.Messages
{
	public sealed class S_ReportWatcherMessage : S_Message
	{
		public string Name { get; set; }
		public List<string> Watchers { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(8);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteByte((byte)this.Watchers.Count);
			foreach (string s in this.Watchers)
			{
				byteBuffer.WriteString(s);
			}
			return byteBuffer;
		}
	}
}
