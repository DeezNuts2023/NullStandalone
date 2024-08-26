using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace tfmStandalone
{
	public static class WindowFlash
	{
        private struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        public const uint FLASHW_STOP = 0;
        public const uint FLASHW_CAPTION = 1;
        public const uint FLASHW_TRAY = 2;
        public const uint FLASHW_ALL = 3;
        public const uint FLASHW_TIMER = 4;
        public const uint FLASHW_TIMERNOFG = 12;

        public static void FlashWindow(Window window, uint count)
		{
			WindowFlash.Flash(new WindowInteropHelper(window).Handle, count);
		}

		public static void FlashWindow(Window window)
		{
			WindowFlash.Flash(new WindowInteropHelper(window).Handle);
		}

		public static void StopFlashing(Window window)
		{
			IntPtr handle = new WindowInteropHelper(window).Handle;
			if (WindowFlash.Win2000OrLater)
			{
				WindowFlash.FLASHWINFO flashwinfo = WindowFlash.CreateFlashInfoStruct(handle, 0U, uint.MaxValue, 0U);
				WindowFlash.FlashWindowEx(ref flashwinfo);
			}
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FlashWindowEx(ref WindowFlash.FLASHWINFO pwfi);

		private static bool Flash(IntPtr hwnd)
		{
			if (WindowFlash.Win2000OrLater)
			{
				WindowFlash.FLASHWINFO flashwinfo = WindowFlash.CreateFlashInfoStruct(hwnd, 3U, uint.MaxValue, 0U);
				return WindowFlash.FlashWindowEx(ref flashwinfo);
			}
			return false;
		}

		private static WindowFlash.FLASHWINFO CreateFlashInfoStruct(IntPtr handle, uint flags, uint count, uint timeout)
		{
			WindowFlash.FLASHWINFO flashwinfo = default(WindowFlash.FLASHWINFO);
			flashwinfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(flashwinfo));
			flashwinfo.hwnd = handle;
			flashwinfo.dwFlags = flags;
			flashwinfo.uCount = count;
			flashwinfo.dwTimeout = timeout;
			return flashwinfo;
		}

		private static bool Flash(IntPtr hwnd, uint count)
		{
			if (WindowFlash.Win2000OrLater)
			{
				WindowFlash.FLASHWINFO flashwinfo = WindowFlash.CreateFlashInfoStruct(hwnd, 15U, count, 0U);
				return WindowFlash.FlashWindowEx(ref flashwinfo);
			}
			return false;
		}

		private static bool Win2000OrLater
		{
			get
			{
				return Environment.OSVersion.Version.Major >= 5;
			}
		}
	}
}
