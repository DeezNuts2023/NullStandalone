namespace tfmStandalone
{
	public sealed class SanctionCasierItem : CasierItem
	{
		public bool IsCurrentlyRunning { get; set; }
		public bool IsCancelled { get; set; }
		public bool IsOverriden { get; set; }
		public string Hours { get; set; }
		public string Target { get; set; }
		public string Author { get; set; }
		public string Reason { get; set; }
		public string CancellationAuthor { get; set; }
		public string CancellationReason { get; set; }
		public string GivenTime { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public string CancellationTime { get; set; }
	}
}
