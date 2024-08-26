using System;

namespace tfmStandalone
{
	public sealed class Disposable : IDisposable
	{
        private Action _disposeAction;

        public static IDisposable Create(Action disposeAction)
		{
			return new Disposable(disposeAction);
		}

		private Disposable(Action disposeAction)
		{
			this._disposeAction = disposeAction;
		}

		public void Dispose()
		{
			Action disposeAction = this._disposeAction;
			if (disposeAction != null)
			{
				disposeAction();
			}
			this._disposeAction = null;
		}
	}
}
