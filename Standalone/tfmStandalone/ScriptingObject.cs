using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace tfmStandalone
{
	[ComVisible(true)]
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public sealed class ScriptingObject
	{
		private FlashPlayer FlashPlayer { get; }

		public ScriptingObject(FlashPlayer flashPlayer)
		{
			this.FlashPlayer = flashPlayer;
		}

		public void EncryptionKey(string key)
		{
			this.FlashPlayer.EncryptionKey(key);
		}

		public void EncryptionVector(string vector)
		{
			this.FlashPlayer.EncryptionVector(vector);
		}
	}
}
