using System;
using System.Runtime.InteropServices;

namespace tfmClientHook
{
	internal static class NativeSocketMethod
	{
		[DllImport("Ws2_32.dll")]
		public static extern int connect(IntPtr socketHandle, ref NativeSocketMethod.sockaddr Address, ref int Addresslen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate int DConnect(IntPtr socketHandle, ref NativeSocketMethod.sockaddr Address, ref int Addresslen);

		public struct in_addr
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public byte[] sin_addr;
		}

		public struct sockaddr
		{
			public short sin_family;
			public ushort sin_port;
			public NativeSocketMethod.in_addr sin_addr;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public byte[] sin_zero;
		}
	}
}
