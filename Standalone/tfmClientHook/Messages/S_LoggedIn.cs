using System.Collections.Generic;

namespace tfmClientHook.Messages
{
	public sealed class S_LoggedIn : S_Message
	{
		// User
        public int UserId { get; set; }
		public string Name { get; set; }
		public int TimePlayed { get; set; }
		public byte Community { get; set; }
		public int PlayerSessionId { get; set; }
		public bool IsPlayerAccount { get; set; }
		public List<byte> Roles { get; set; }
		public bool HasPublicAuthorization { get; set; }

        // Roles
        public const byte ArbitreRole = 3;
        public const byte ModeratorRole = 5;
        public const byte SentinelleRole = 7;
        public const byte AdministratorRole = 10;
        public const byte MapCrewRole = 11;
        public const byte LuaDevRole = 12;
        public const byte FunCorpRole = 13;
        public const byte FashionSquadRole = 15;

        public bool IsArbitre => this.Roles != null && this.Roles.Contains(ArbitreRole);
        public bool IsModerator => this.Roles != null && this.Roles.Contains(ModeratorRole);
        public bool IsSentinelle => this.Roles != null && this.Roles.Contains(SentinelleRole);
        public bool IsAdministrator => this.Roles != null && this.Roles.Contains(AdministratorRole);
        public bool IsMapCrew => this.Roles != null && this.Roles.Contains(MapCrewRole);
        public bool IsLuaDev => this.Roles != null && this.Roles.Contains(LuaDevRole);
        public bool IsFunCorp => this.Roles != null && this.Roles.Contains(FunCorpRole);
        public bool IsFashionSquad => this.Roles != null && this.Roles.Contains(FashionSquadRole);

        public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteInt(this.UserId);
			byteBuffer.WriteString(this.Name);
			byteBuffer.WriteInt(this.TimePlayed);
			byteBuffer.WriteByte(this.Community);
			byteBuffer.WriteInt(this.PlayerSessionId);
			byteBuffer.WriteBool(this.IsPlayerAccount);
			byteBuffer.WriteByte((byte)this.Roles.Count);
			this.Roles.ForEach(delegate(byte b)
			{
				byteBuffer.WriteByte(b);
			});
			byteBuffer.WriteBool(this.HasPublicAuthorization);
			return byteBuffer;
		}
	}
}
