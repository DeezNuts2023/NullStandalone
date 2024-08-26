using System.Collections.Generic;
using tfmClientHook;

namespace tfmStandalone
{
	public sealed class GameInfo
	{
		public bool IsChatShowing { get; set; }
		public bool IsModopwetShowing { get; set; }
		public string Name { get; set; }
		public int UserId { get; set; }
		public Community Community { get; set; }
		public bool IsArbitre { get; set; }
		public bool IsModerator { get; set; }
		public bool IsSentinelle { get; set; }
		public bool IsAdministrator { get; set; }
		public bool IsMapCrew { get; set; }
		public bool IsLuaDev { get; set; }
		public bool IsFunCorp { get; set; }
		public bool IsFashionSquad { get; set; }
		public bool IsPublic { get; set; }
		public Dictionary<string, Player> ModList { get; private set; }
		public Dictionary<string, Player> ArbList { get; private set; }
		public Dictionary<string, Player> FriendList { get; private set; }
		public List<string> IgnoreList { get; private set; }
		public List<string> TemporaryIgnoreList { get; private set; }
		public List<string> AllowedCommunities { get; private set; }
		public List<string> IgnoredCommunities { get; private set; }

		public GameInfo()
		{
			this.ModList = new Dictionary<string, Player>();
			this.ArbList = new Dictionary<string, Player>();
			this.FriendList = new Dictionary<string, Player>();
			this.IgnoreList = new List<string>();
			this.TemporaryIgnoreList = new List<string>();
			this.AllowedCommunities = new List<string>();
			this.IgnoredCommunities = new List<string>();
			this.IsChatShowing = true;
		}

		public bool IsMatchingCommunity(Community testCommunity)
		{
			return this.Community == testCommunity || (this.Community == Community.en && testCommunity == Community.e2) || (this.Community == Community.e2 && testCommunity == Community.en);
		}

		public void Clear()
		{
			this.Name = null;
			this.UserId = 0;
			this.IsArbitre = false;
			this.IsModerator = false;
			this.IsSentinelle = false;
			this.IsAdministrator = false;
			this.IsMapCrew = false;
			this.IsLuaDev = false;
			this.IsFunCorp = false;
			this.IsFashionSquad = false;
			this.IsPublic = false;
			this.ModList = new Dictionary<string, Player>();
			this.ArbList = new Dictionary<string, Player>();
			this.FriendList = new Dictionary<string, Player>();
		}
	}
}
