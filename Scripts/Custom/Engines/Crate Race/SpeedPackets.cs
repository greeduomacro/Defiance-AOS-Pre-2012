using System;
using Server.Network;

namespace Server
{
	public sealed class SpeedPacket : Packet
	{
		public static readonly Packet Walk = Packet.SetStatic(new SpeedPacket(2));
		public static readonly Packet Run = Packet.SetStatic(new SpeedPacket(1));
		public static readonly Packet Disabled = Packet.SetStatic(new SpeedPacket(0));

		public SpeedPacket(int mode)
			: base(0xBF)
		{
			EnsureCapacity(3);
			m_Stream.Write((short)0x26);
			m_Stream.Write((byte)mode);
		}
	}
}