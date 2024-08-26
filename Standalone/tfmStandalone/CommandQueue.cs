using System;
using System.Collections.Generic;
using System.Timers;

namespace tfmStandalone
{
	public sealed class CommandQueue
	{
		private Timer Timer { get; set; }
		private List<CommandQueue.CommandQueueItem> Queue { get; }
		private Action<string> InvokeCommandAction { get; }

		public CommandQueue(Action<string> invokeCommandAction)
		{
			this.InvokeCommandAction = invokeCommandAction;
			this.Timer = new Timer();
			this.Queue = new List<CommandQueue.CommandQueueItem>();
			this.Timer.Elapsed += this.TimerOnElapsed;
		}

		public void Add(string command, int delay)
		{
			this.Queue.Add(new CommandQueue.CommandQueueItem
			{
				Command = command,
				Delay = delay
			});
		}

		public void Start()
		{
			this.StartNextCommand();
		}

		private void StartNextCommand()
		{
			if (this.Queue.Count == 0)
			{
				this.Timer.Elapsed -= this.TimerOnElapsed;
				this.Timer = null;
				return;
			}
			this.InvokeCommandAction(this.Queue[0].Command);
			if (this.Queue[0].Delay == 0)
			{
				this.Queue.RemoveAt(0);
				this.StartNextCommand();
				return;
			}
			this.Timer.Interval = (double)this.Queue[0].Delay;
			this.Queue.RemoveAt(0);
			this.Timer.Start();
		}

		private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			this.Timer.Stop();
			this.StartNextCommand();
		}

		private sealed class CommandQueueItem
		{
			public string Command { get; set; }
			public int Delay { get; set; }
		}
	}
}
