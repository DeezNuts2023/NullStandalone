using System.Net;
using System.Net.Sockets;

namespace tfmClientHook
{
	internal sealed class TcpListenerEx : TcpListener
	{
		public int Port { get; }

		public TcpListenerEx(IPAddress localAddress, int port) : base(localAddress, port)
		{
			this.Port = port;
		}

        public new bool Active => base.Active;
    }
}
