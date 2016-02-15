using System;
using Server.Mobiles;
using Server.Regions;

namespace Server.Items
{
	public class YoungPlayerTeleport : Item
	{
		private PlayerMobile m_Young;
		private static Point3D m_Loc = new Point3D(3647, 2681, -2);
		private static Map m_Map = Map.Trammel;

		public YoungPlayerTeleport(PlayerMobile m)
			: base(3630)
		{
			m_Young = m;
			LootType = LootType.Blessed;
			Name = "Crystal Ball Of Teleportation";
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from != m_Young)
			{
				from.SendMessage("This item does not belong to you.");
				return;
			}

			if (from.Backpack != null && !IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}

			Timer.DelayCall(TimeSpan.FromSeconds(10.0), new TimerStateCallback(OnUse_DelayCall), from);
			from.SendMessage("Please wait a moment for your status to be initialised.");
		}

		private void OnUse_DelayCall(object o)
		{
			Mobile from = (Mobile)o;

			if (!from.Criminal && from.Aggressed.Count == 0 && from.Aggressors.Count == 0)
				from.MoveToWorld(new Point3D(3670, 2627, 0), Map.Trammel);

			else
				from.SendMessage("You are currently engaged in a battle, please wait a few moments and try again.");
		}

		public YoungPlayerTeleport(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write(m_Young);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Young = (PlayerMobile)reader.ReadMobile();

			if (!YoungRegionFlag.IsYoung(m_Young))
				Delete();

			if (!Deleted && m_Young != null)
			{
				TimeSpan ts = DateTime.Now - m_Young.CreationTime;

				if (ts > TimeSpan.FromDays(21.0))
					Delete();
			}
		}
	}
}