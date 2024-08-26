using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using tfmClientHook.Messages;

namespace tfmClientHook
{
	internal sealed class ProxyServer
	{
		private IMessageInterceptor MessageInterceptor { get; }
		private IPAddress MainServerIP { get; set; }
		private ProxyConnection MainServer { get; set; }
		private List<TcpListenerEx> TcpListeners { get; set; }

		public ProxyServer(IMessageInterceptor messageInterceptor)
		{
			this.MessageInterceptor = messageInterceptor;
		}

		public void Start(IPAddress mainServerIP)
		{
			this.MainServerIP = mainServerIP;
			this.TcpListeners = new List<TcpListenerEx>
			{
				new TcpListenerEx(IPAddress.Any, 11801),
				new TcpListenerEx(IPAddress.Any, 12801),
				new TcpListenerEx(IPAddress.Any, 13801),
				new TcpListenerEx(IPAddress.Any, 14801)
			};
			foreach (TcpListenerEx tcpListenerEx in this.TcpListeners)
			{
				try
				{
					tcpListenerEx.Start();
					tcpListenerEx.BeginAcceptTcpClient(new AsyncCallback(this.OnAcceptTcpClient), tcpListenerEx);
				}
				catch (Exception)
				{
					tcpListenerEx.Stop();
				}
			}
		}

		public void Stop()
		{
			ProxyConnection mainServer = this.MainServer;
			if (mainServer != null)
			{
				mainServer.Stop();
			}
			this.MainServer = null;
		}

		private void OnAcceptTcpClient(IAsyncResult result)
		{
			try
			{
				if (result.IsCompleted && result.AsyncState is TcpListener)
				{
					TcpListenerEx tcpListenerEx = (TcpListenerEx)result.AsyncState;
					if (tcpListenerEx.Active)
					{
						Console.WriteLine("OnAcceptTcpClient");
						TcpClient tcpClient = tcpListenerEx.EndAcceptTcpClient(result);
						if (this.MainServer != null)
						{
							this.Stop();
						}
						this.MainServer = new ProxyConnection(ConnectionType.Main, this.MessageInterceptor);
						ProxyConnection mainServer = this.MainServer;
						mainServer.ConnectionClosed = (EventHandler)Delegate.Combine(mainServer.ConnectionClosed, new EventHandler(delegate(object sender, EventArgs args)
						{
							if (this.MainServer == sender)
							{
								this.MainServer = null;
							}
						}));
						this.MainServer.Start(tcpClient.Client, this.MainServerIP, tcpListenerEx.Port);
						foreach (TcpListenerEx tcpListenerEx2 in this.TcpListeners)
						{
							tcpListenerEx2.Stop();
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public void SendToClient(ConnectionType targetServer, S_Message message)
		{
			if (targetServer == ConnectionType.Main)
			{
				ProxyConnection mainServer = this.MainServer;
				if (mainServer == null)
				{
					return;
				}
				mainServer.SendMessageToClient(message);
			}
		}

		public void SendToServer(ConnectionType targetServer, C_Message message)
		{
			if (targetServer == ConnectionType.Main)
			{
				ProxyConnection mainServer = this.MainServer;
				if (mainServer == null)
				{
					return;
				}
				mainServer.SendMessageToServer(message);
			}
		}
	}
}
