using System;
using System.Collections.Generic;

namespace tfmClientHook
{
	public sealed class ServerData
	{
		public string AnalyzationTime { get; set; }
		public IncomingTribullePackets IncomingTribullePacketIds { get; set; }
		public OutgoingTribullePackets OutgoingTribullePacketIds { get; set; }
		public List<TribulleCommunity> Communities { get; set; }
	}
}
