using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace tfmStandalone
{
	public static class TaskHelpers
	{
		public static void UiInvoke(Action a)
		{
			Application.Current.Dispatcher.Invoke(a);
		}

		public static IDisposable Delay(Action action, int delay)
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			CancellationToken token = cancellationTokenSource.Token;
			Task.Delay(delay, token).ContinueWith(delegate(Task task)
			{
				action();
			}, token);
			return Disposable.Create(new Action(cancellationTokenSource.Cancel));
		}
	}
}
