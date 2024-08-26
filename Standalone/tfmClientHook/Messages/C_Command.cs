using System;

namespace tfmClientHook.Messages
{
	public class C_Command : C_Message
	{
		public string Command { get; set; }

        public override bool IsEncrypted => true;

        public override ByteBuffer GetBuffer()
		{
			Console.WriteLine(this.Command);
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(6);
			byteBuffer.WriteByte(26);
			byteBuffer.WriteString(this.Command);
			return byteBuffer;
		}
	}
}
