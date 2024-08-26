using System.Collections.Generic;
using tfmClientHook.Messages;

namespace tfmClientHook
{
	public interface IMessageInterceptor {
		bool CommandSent(C_Command commandSentMessage);
		void OSInformationSent();
		bool LoggedIn(S_LoggedIn loggedInMessage);
		bool WhisperReceived(S_Whisper whisperMessage);
		bool TribeMessageReceived(S_TribeMessage tribeMessage);
		bool ServerMessageReceived(S_ServerMessage serverMessage);
		bool StaffChatMessageReceived(S_StaffChatMessage staffChatMessage);
		void StaffListReceived(S_StaffListMessage staffListMessage);
		bool WindowMessageReceiced(S_WindowDisplayMessage windowMessage);
		void ReportList(S_ReportListMessage reportListMessage);
		void ReportSanctioned(S_ReportSanctionedMessage reportSanctionedMessage);
		void ReportDisconnected(S_ReportDisconnectedMessage reportDisconnectedMessage);
		void ReportDeleted(S_ReportDeletedMessage reportDeletedMessage);
		void ReportWatcher(S_ReportWatcherMessage reportWatcherMessage);
		bool ChatLogReceived(S_ChatLogMessage chatLogMessage);
		void ReportRoomUpdated(S_ReportRoomUpdatedMessage roomUpdatedMessage);
		void ReportCountsUpdated(S_ReportCountsUpdateMessage reportsCountsMessage);
		void FriendConnected(Player player);
		void IgnoreListReceived(IEnumerable<string> names);
		void PlayerIgnored(string name);
	}
}
