using System.Collections.Generic;

namespace tfmClientHook.Messages
{
	public sealed class S_ServerMessage : S_Message
	{
		public bool IsSelf { get; set; }
		public string Message { get; set; }
		public List<string> TranslationParameters { get; set; }

		public S_ServerMessage()
		{
			this.TranslationParameters = new List<string>();
		}

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(6);
			byteBuffer.WriteByte(20);
			byteBuffer.WriteBool(this.IsSelf);
			byteBuffer.WriteString(this.Message);
			byteBuffer.WriteByte((byte)this.TranslationParameters.Count);
			foreach (string s in this.TranslationParameters)
			{
				byteBuffer.WriteString(s);
			}
			return byteBuffer;
		}
	}
}
