using System;

namespace tfmStandalone
{
	public sealed class CasierHandlerService
	{
		private WindowService WindowService { get; }
		private GameSettings GameSettings { get; }

		public CasierHandlerService(MessageInterceptor messageInterceptor, WindowService windowService, GameSettings gameSettings)
		{
			this.WindowService = windowService;
			messageInterceptor.WindowDisplayMessageReceived = (EventHandler<MessageInterceptor.WindowDisplayMessageEventArgs>)Delegate.Combine(messageInterceptor.WindowDisplayMessageReceived, new EventHandler<MessageInterceptor.WindowDisplayMessageEventArgs>(this.WindowDisplayMessageReceived));
			this.GameSettings = gameSettings;
		}

		private void WindowDisplayMessageReceived(object sender, MessageInterceptor.WindowDisplayMessageEventArgs e)
		{
			if (this.GameSettings.UseCustomCasierWindow && e.Message.Text.StartsWith("<p align='center'>Sanction logs for <v>"))
			{
				e.SendToClient = false;
				TaskHelpers.UiInvoke(delegate
				{
					this.WindowService.ShowCasierWindow(e.Message.Text);
				});
			}
		}
	}
}
