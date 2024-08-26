using System;
using System.Net;
using EasyHook;
using tfmClientHook.Messages;

namespace tfmClientHook
{
	public sealed class ClientHook
	{
        private int _connectionCount;
        private ProxyServer _proxyServer;
        private LocalHook ConnectHook { get; set; }

		public void Run(IMessageInterceptor messageInterceptor)
		{
			this.ConnectHook = LocalHook.Create(LocalHook.GetProcAddress("Ws2_32.dll", "connect"), new NativeSocketMethod.DConnect(this.ConnectHooked), this);
			this.ConnectHook.ThreadACL.SetExclusiveACL(new int[1]);
			RemoteHooking.WakeUpProcess();
			ServerInfo.Initialize();
			this._proxyServer = new ProxyServer(messageInterceptor);
		}

		public void Stop()
		{
			ProxyServer proxyServer = this._proxyServer;
			if (proxyServer != null)
			{
				proxyServer.Stop();
			}
			this._proxyServer = null;
			ServerInfo.Clear();
			this.ConnectHook = null;
			this._connectionCount = 0;
		}

		public void SendToClient(ConnectionType targetServer, S_Message message)
		{
			ProxyServer proxyServer = this._proxyServer;
			if (proxyServer == null)
			{
				return;
			}
			proxyServer.SendToClient(targetServer, message);
		}

		public void SendToServer(ConnectionType targetServer, C_Message message)
		{
			ProxyServer proxyServer = this._proxyServer;
			if (proxyServer == null)
			{
				return;
			}
			proxyServer.SendToServer(targetServer, message);
		}

		public void SetEncryptionKey(sbyte[] encryptionKey)
		{
			ServerInfo.EncryptionKey = encryptionKey;
		}

		public void SetEncryptionVector(int[] encryptionVector)
		{
			ServerInfo.EncryptionVector = encryptionVector;
		}

		private int ConnectHooked(IntPtr socketHandle, ref NativeSocketMethod.sockaddr name, ref int namelen)
		{
			string text = string.Join<byte>(".", name.sin_addr.sin_addr);
			Console.WriteLine(string.Format("Connect Called: {0}", text));
			if (text == "68.219.177.109")
			{
				int num = this._connectionCount + 1;
				this._connectionCount = num;
				if (num == 2)
				{
					IPAddress mainServerIP = new IPAddress(new byte[]
					{
						name.sin_addr.sin_addr[0],
						name.sin_addr.sin_addr[1],
						name.sin_addr.sin_addr[2],
						name.sin_addr.sin_addr[3]
					});
					this._proxyServer.Start(mainServerIP);
					name.sin_addr.sin_addr[0] = 127;
					name.sin_addr.sin_addr[1] = 0;
					name.sin_addr.sin_addr[2] = 0;
					name.sin_addr.sin_addr[3] = 1;
					int result = NativeSocketMethod.connect(socketHandle, ref name, ref namelen);
					this.ConnectHook.Dispose();
					return result;
				}
			}
			return NativeSocketMethod.connect(socketHandle, ref name, ref namelen);
		}
	}
}
