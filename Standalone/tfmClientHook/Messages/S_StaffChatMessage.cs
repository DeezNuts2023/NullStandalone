using System;

namespace tfmClientHook.Messages
{
	public sealed class S_StaffChatMessage : S_Message
	{
		public StaffChatType Type { get; set; }
		public string Name { get; set; }
		public string Message { get; set; }
		public bool GeneralChatMessage { get; set; }
		public byte[] ExtraBytes { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(6);
			byteBuffer.WriteByte(10);
			byteBuffer.WriteByte((byte)this.Type);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteString(this.Message);
			byteBuffer.WriteBool(this.GeneralChatMessage);
			if (this.ExtraBytes != null && this.ExtraBytes.Length != 0)
			{
				byteBuffer.WriteBytes(this.ExtraBytes);
			}
			return byteBuffer;
		}
	}
}
