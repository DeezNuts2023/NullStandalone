using System.Collections.Generic;

namespace tfmClientHook.Messages
{
	public sealed class S_StaffListMessage : S_Message
	{
		public StaffChatType Type { get; set; }
		public List<S_StaffListMessage.StaffMember> StaffMembers { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(24);
			byteBuffer.WriteByte(5);
			byteBuffer.WriteByte((byte)this.Type);
			byteBuffer.WriteByte((byte)this.StaffMembers.Count);
			foreach (S_StaffListMessage.StaffMember staffMember in this.StaffMembers)
			{
				byteBuffer.WriteString(staffMember.Community.ToString().ToUpperInvariant());
				byteBuffer.WriteString(staffMember.Name);
				byteBuffer.WriteString(staffMember.Room);
			}
			return byteBuffer;
		}

		public sealed class StaffMember
		{
			public Community Community { get; set; }
			public string Name { get; set; }
			public string Room { get; set; }
		}
	}
}
