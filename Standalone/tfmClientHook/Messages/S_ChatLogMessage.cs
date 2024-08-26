using System;
using System.Collections.Generic;

namespace tfmClientHook.Messages
{
	public sealed class S_ChatLogMessage : S_Message { 
		public string Name { get; set; }
		public List<S_ChatLogMessage.Message> Messages { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(10);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteByte((byte)this.Messages.Count);
			foreach (S_ChatLogMessage.Message message in this.Messages)
			{
				byteBuffer.WriteString(message.Text);
				string text = message.Time.ToString("yyyy/MM/dd HH:mm:ss");
				text = text + " GMT" + message.Time.ToString("zzz").Replace(":", string.Empty);
				byteBuffer.WriteString(text);
			}
			return byteBuffer;
		}

		public sealed class Message
		{
			public DateTime Time { get; set; }
			public string Text { get; set; }
		}
	}
}
