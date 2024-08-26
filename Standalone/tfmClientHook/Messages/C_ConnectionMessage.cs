using System;

namespace tfmClientHook.Messages
{
	public class C_ConnectionMessage : C_Message
	{
		public short Version { get; set; }
		public string Key { get; set; }
		public string PlayerType { get; set; }
		public string Browser { get; set; }
		public int LoaderSize { get; set; }
		public string Fonts { get; set; }
		public string ServerString { get; set; }
		public int ReferrerId { get; set; }
		public int CurrentTime { get; set; }
        public string Language { get; set; }

        public override bool IsEncrypted => false;

        public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(28);
			byteBuffer.WriteByte(1);
			byteBuffer.WriteShort(this.Version);
			byteBuffer.WriteString(this.Language);
			byteBuffer.WriteString(this.Key);
			byteBuffer.WriteString(this.PlayerType);
			byteBuffer.WriteString(this.Browser);
			byteBuffer.WriteInt(this.LoaderSize);
			byteBuffer.WriteString("");
			byteBuffer.WriteString(this.Fonts);
			byteBuffer.WriteString(this.ServerString);
			byteBuffer.WriteInt(this.ReferrerId);
			byteBuffer.WriteInt(this.CurrentTime);
			byteBuffer.WriteString("");
			return byteBuffer;
		}
	}
}
