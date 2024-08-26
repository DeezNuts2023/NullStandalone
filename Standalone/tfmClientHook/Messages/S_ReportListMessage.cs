using System.Collections.Generic;

namespace tfmClientHook.Messages
{
	public sealed class S_ReportListMessage : S_Message
	{
		public List<S_ReportListMessage.PlayerReport> PlayerReports { get; set; }

		public override ByteBuffer GetBuffer()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteByte(25);
			byteBuffer.WriteByte(2);
			byteBuffer.WriteByte((byte)this.PlayerReports.Count);
			foreach (S_ReportListMessage.PlayerReport playerReport in this.PlayerReports)
			{
				byteBuffer.WriteBool(playerReport.IsGeneral);
				byteBuffer.WriteByte(playerReport.UnknownByte1);
				byteBuffer.WriteByte(playerReport.UnknownByte2);
				byteBuffer.WriteString(playerReport.Community);
				byteBuffer.WriteString(playerReport.PlayerName);
				byteBuffer.WriteString(playerReport.PlayerRoom);
				byteBuffer.WriteByte((byte)playerReport.Watchers.Count);
				foreach (string s in playerReport.Watchers)
				{
					byteBuffer.WriteString(s);
				}
				byteBuffer.WriteInt(playerReport.PlayerHours);
				byteBuffer.WriteByte((byte)playerReport.Reports.Count);
				foreach (S_ReportListMessage.Report report in playerReport.Reports)
				{
					byteBuffer.WriteString(report.ReporterName);
					byteBuffer.WriteShort(report.ReporterKarma);
					byteBuffer.WriteString(report.Comment);
					byteBuffer.WriteByte((byte)report.Type);
					byteBuffer.WriteShort(report.Age);
				}
				byteBuffer.WriteBool(playerReport.IsMuted);
				if (playerReport.IsMuted)
				{
					byteBuffer.WriteString(playerReport.Muter);
					byteBuffer.WriteShort(playerReport.MuteLength);
					byteBuffer.WriteString(playerReport.MuteReason);
				}
			}
			return byteBuffer;
		}

		public sealed class PlayerReport
		{
			public bool IsGeneral { get; set; }
			public byte UnknownByte1 { get; set; }
			public byte UnknownByte2 { get; set; }
			public string Community { get; set; }
			public string PlayerName { get; set; }
			public string PlayerRoom { get; set; }
			public List<string> Watchers { get; set; }
			public int PlayerHours { get; set; }
			public List<S_ReportListMessage.Report> Reports { get; set; }
			public bool IsMuted { get; set; }
			public string Muter { get; set; }
			public short MuteLength { get; set; }
			public string MuteReason { get; set; }
		}

		public sealed class Report
		{
			public string ReporterName { get; set; }
			public short ReporterKarma { get; set; }
			public string Comment { get; set; }
			public ReportType Type { get; set; }
			public short Age { get; set; }
		}
	}
}
