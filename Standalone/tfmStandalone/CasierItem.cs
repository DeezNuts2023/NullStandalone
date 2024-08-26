namespace tfmStandalone
{
	public class CasierItem
	{
		public CasierItem.Type ItemType { get; set; }

		public enum Type
		{
			Mute,
			Ban,
			NameChange
		}
	}
}
