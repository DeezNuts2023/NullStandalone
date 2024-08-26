namespace tfmStandalone
{
	public sealed class Sanction
	{
		public string Description { get; set; }
		public SanctionOccurence FirstOccurence { get; set; }
		public SanctionOccurence SecondOccurence { get; set; }
		public SanctionOccurence ThirdOccurence { get; set; }
		public SanctionOccurence FourthOccurence { get; set; }
		public string Information { get; set; }
		public bool IsInfoImportant { get; set; }
	}
}
