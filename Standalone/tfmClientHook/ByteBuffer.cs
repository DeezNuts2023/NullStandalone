using System;
using System.Collections.Generic;
using System.Text;

namespace tfmClientHook
{
	public sealed class ByteBuffer
	{
        private readonly List<byte> _byteList;

        public ByteBuffer()
		{
			this._byteList = new List<byte>();
		}

        public int Count => this._byteList.Count;
        public byte this[int index] => this._byteList[index];

        public void WriteByte(byte b)
		{
			this._byteList.Add(b);
		}

		public void WriteBytes(byte[] byteList)
		{
			foreach (byte item in byteList)
			{
				this._byteList.Add(item);
			}
		}

        public void WriteBool(bool boolean)
        {
            this._byteList.Add((byte)(boolean ? 1 : 0));
        }

        public void WriteShort(short s)
		{
			byte[] bytes = BitConverter.GetBytes(s);
			Array.Reverse(bytes);
			foreach (byte item in bytes)
			{
				this._byteList.Add(item);
			}
		}

		public void WriteMediumInt(int i)
		{
			byte[] bytes = BitConverter.GetBytes(i);
			Array.Reverse(bytes);
			for (int j = 1; j < 4; j++)
			{
				this._byteList.Add(bytes[j]);
			}
		}

		public void WriteInt(int i)
		{
			byte[] bytes = BitConverter.GetBytes(i);
			Array.Reverse(bytes);
			foreach (byte item in bytes)
			{
				this._byteList.Add(item);
			}
		}

		public void WriteString(string s)
		{
			byte[] bytes = new UTF8Encoding().GetBytes(s);
			byte[] bytes2 = BitConverter.GetBytes((short)bytes.Length);
			Array.Reverse(bytes2);
			foreach (byte item in bytes2)
			{
				this._byteList.Add(item);
			}
			foreach (byte item2 in bytes)
			{
				this._byteList.Add(item2);
			}
		}

		public void WriteLongString(string s)
		{
			byte[] bytes = new UTF8Encoding().GetBytes(s);
			this.WriteMediumInt(bytes.Length);
			foreach (byte item in bytes)
			{
				this._byteList.Add(item);
			}
		}

		public void PrependByte(byte b)
		{
			this._byteList.Insert(0, b);
		}

		public void PrependShort(short s)
		{
			foreach (byte b in BitConverter.GetBytes(s))
			{
				this.PrependByte(b);
			}
		}

		public void PrependMediumInt(int i)
		{
			byte[] bytes = BitConverter.GetBytes(i);
			for (int j = 0; j < 3; j++)
			{
				this.PrependByte(bytes[j]);
			}
		}

		public void PrependInt(int i)
		{
			foreach (byte b in BitConverter.GetBytes(i))
			{
				this.PrependByte(b);
			}
		}

		public byte ReadByte()
		{
			byte result = this._byteList[0];
			this._byteList.RemoveAt(0);
			return result;
		}

		public byte[] ReadBytes(int count)
		{
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = this.ReadByte();
			}
			return array;
		}

		public byte[] ReadBytes(uint count)
		{
			byte[] array = new byte[count];
			int num = 0;
			while ((long)num < (long)((ulong)count))
			{
				array[num] = this.ReadByte();
				num++;
			}
			return array;
		}

		public bool ReadBool()
		{
			return this.ReadByte() > 0;
		}

		public short ReadShort()
		{
			byte b = this.ReadByte();
			byte b2 = this.ReadByte();
			return BitConverter.ToInt16(new byte[]{b2,b}, 0);
		}

		public ushort ReadUnsignedShort()
		{
			byte b = this.ReadByte();
			byte b2 = this.ReadByte();
			return BitConverter.ToUInt16(new byte[]{b2,b}, 0);
		}

		public int ReadMediumInt()
		{
			byte b = this.ReadByte();
			byte b2 = this.ReadByte();
			byte b3 = this.ReadByte();
			byte[] array = new byte[4];
			array[0] = b3;
			array[1] = b2;
			array[2] = b;
			return BitConverter.ToInt32(array, 0);
		}

		public uint ReadUnsignedMediumInt()
		{
			byte b = this.ReadByte();
			byte b2 = this.ReadByte();
			byte b3 = this.ReadByte();
			byte[] array = new byte[4];
			array[0] = b3;
			array[1] = b2;
			array[2] = b;
			return BitConverter.ToUInt32(array, 0);
		}

		public int ReadInt()
		{
			byte b = this.ReadByte();
			byte b2 = this.ReadByte();
			byte b3 = this.ReadByte();
			byte b4 = this.ReadByte();
			return BitConverter.ToInt32(new byte[]{b4,b3,b2,b}, 0);
		}

		public uint ReadUnsignedInt()
		{
			byte b = this.ReadByte();
			byte b2 = this.ReadByte();
			byte b3 = this.ReadByte();
			byte b4 = this.ReadByte();
			return BitConverter.ToUInt32(new byte[]{b4,b3,b2,b}, 0);
		}

		public string ReadString()
		{
			ushort num = this.ReadUnsignedShort();
			if (num == 0)
			{
				return string.Empty;
			}
			byte[] array = new byte[(int)num];
			for (int i = 0; i < (int)num; i++)
			{
				array[i] = this.ReadByte();
			}
			return Encoding.UTF8.GetString(array);
		}

		public string ReadLongString()
		{
			int num = this.ReadMediumInt();
			if (num == 0)
			{
				return string.Empty;
			}
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = this.ReadByte();
			}
			return Encoding.UTF8.GetString(array);
		}

		public byte PeekByte(int startPos = 0)
		{
			return this._byteList[startPos];
		}

		public short PeekShort(int startPos = 0)
		{
			byte b = this._byteList[startPos];
			byte b2 = this._byteList[startPos + 1];
			return BitConverter.ToInt16(new byte[]{b2,b}, 0);
		}

		public ushort PeekUnsignedShort(int startPos = 0)
		{
			byte b = this._byteList[startPos];
			byte b2 = this._byteList[startPos + 1];
			return BitConverter.ToUInt16(new byte[]{b2,b}, 0);
		}

		public int PeekMediumInt(int startPos = 0)
		{
			byte b = this._byteList[startPos];
			byte b2 = this._byteList[startPos + 1];
			byte b3 = this._byteList[startPos + 2];
			byte[] array = new byte[4];
			array[0] = b3;
			array[1] = b2;
			array[2] = b;
			return BitConverter.ToInt32(array, 0);
		}

		public uint PeekUnsignedMediumInt(int startPos = 0)
		{
			byte b = this._byteList[startPos];
			byte b2 = this._byteList[startPos + 1];
			byte b3 = this._byteList[startPos + 2];
			byte[] array = new byte[4];
			array[0] = b3;
			array[1] = b2;
			array[2] = b;
			return BitConverter.ToUInt32(array, 0);
		}

		public int PeekInt(int startPos = 0)
		{
			byte b = this._byteList[startPos];
			byte b2 = this._byteList[startPos + 1];
			byte b3 = this._byteList[startPos + 2];
			byte b4 = this._byteList[startPos + 3];
			return BitConverter.ToInt32(new byte[]{b4,b3,b2,b}, 0);
		}

		public uint PeekUnsignedInt(int startPos = 0)
		{
			byte b = this._byteList[startPos];
			byte b2 = this._byteList[startPos + 1];
			byte b3 = this._byteList[startPos + 2];
			byte b4 = this._byteList[startPos + 3];
			return BitConverter.ToUInt32(new byte[]{b4,b3,b2,b}, 0);
		}

		public byte[] GetBytes()
		{
			return this._byteList.ToArray();
		}
	}
}
