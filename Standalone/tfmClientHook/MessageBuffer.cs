using System;

namespace tfmClientHook
{
	internal class MessageBuffer
	{
		private ByteBuffer ByteBuffer { get; }
        private readonly bool _isClientBuffer;

        public MessageBuffer(bool isClientBuffer)
		{
			this._isClientBuffer = isClientBuffer;
			this.ByteBuffer = new ByteBuffer();
		}

		public void AddBytes(byte[] byteList, int count)
		{
			this.ByteBuffer.WriteBytes(byteList);
		}

		public ByteBuffer GetNextMessage(ref byte fingerPrint)
		{
			if (this.ByteBuffer.Count == 0)
			{
				return null;
			}
			int num = this.DecodeVLQ(this.ByteBuffer).Item1;
			if (this._isClientBuffer)
			{
				num++;
			}
			byte[] byteList = this.ByteBuffer.ReadBytes(num);
            ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteBytes(byteList);
			if (this._isClientBuffer)
			{
				fingerPrint = byteBuffer.ReadByte();
			}
			return byteBuffer;
		}

		public Tuple<int, int> DecodeVLQ(ByteBuffer bytearr)
		{
			int num = 0;
			int i;
			for (i = 0; i < 5; i++)
			{
				byte b = bytearr.ReadByte();
				num |= (int)(b & 127) << i * 7;
				if ((b & 128) == 0)
				{
					break;
				}
			}
			return Tuple.Create<int, int>(num, i);
		}
	}
}
