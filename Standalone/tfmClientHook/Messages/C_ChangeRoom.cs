using System;

namespace tfmClientHook.Messages
{
	public class C_ChangeRoom : C_Message
	{
		public string RoomName { get; set; }
        public string RoomCommunity { get; set; }
        public string RoomPassword { get; set; }

        public override bool IsEncrypted => false;

        public override ByteBuffer GetBuffer()
		{
			Console.WriteLine(this.RoomName);
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(5);
			byteBuffer.WriteByte(38);
			byteBuffer.WriteString("");
			byteBuffer.WriteString(this.RoomName);
			byteBuffer.WriteString("");
			byteBuffer.WriteByte(0);
			return byteBuffer;
		}
    }
}
