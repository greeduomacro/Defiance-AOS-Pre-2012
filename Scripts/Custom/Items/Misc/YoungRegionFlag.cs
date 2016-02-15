using System;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Regions
{
	public class YoungRegionFlag : Item
	{
		private Region m_GuardedRegion;
		private Region m_UnguardedRegion;
		private Rectangle3D m_GuardedArea = new Rectangle3D();
		private Rectangle3D m_UnguardedArea = new Rectangle3D();
		private Point3D m_RemoveLocation = new Point3D();
		private Map m_RemoveMap = Map.Felucca;

		[CommandProperty(AccessLevel.GameMaster)]
		public Rectangle2D GuardedArea2D
		{
			get  { return new Rectangle2D(m_GuardedArea.Start, m_GuardedArea.End); }
			set { m_GuardedArea = new Rectangle3D(value.X, value.Y, -150, value.Width, value.Height, 300); UpdateRegions(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Rectangle2D UnguardedArea2D
		{
			get { return new Rectangle2D(m_UnguardedArea.Start, m_UnguardedArea.End); }
			set { m_UnguardedArea = new Rectangle3D(value.X, value.Y, -150, value.Width, value.Height, 300); UpdateRegions(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D RemoveLocation
		{
			get { return m_RemoveLocation; }
			set { m_RemoveLocation =value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Map RemoveMap
		{
			get { return m_RemoveMap; }
			set { m_RemoveMap = value; }
		}

		public Rectangle3D GuardedArea3D { get { return m_GuardedArea; } }
		public Rectangle3D UnguardedArea3D { get { return m_UnguardedArea; } }

		[Constructable]
		public YoungRegionFlag()
			: base(5609)
		{
			Visible = false;
			Movable = false;
			Name = "Young Region Flag";
		}

		public static bool IsYoung(PlayerMobile youngster)
		{
			if (youngster == null)
				return false;

			if (youngster.AccessLevel > AccessLevel.Player)
				return true;

			if (youngster.Kills > 5)
				return false;

			return youngster.GameTime < TimeSpan.FromDays(2.0);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if(from.AccessLevel > AccessLevel.Counselor)
				from.SendGump(new PropertiesGump(from, this));

			base.OnDoubleClick(from);
		}

		private void UpdateRegions()
		{
			if (m_UnguardedRegion != null)
				m_UnguardedRegion.Unregister();

			if (m_GuardedRegion != null)
				m_GuardedRegion.Unregister();

			m_GuardedRegion = new YoungRegion(this, true);
			m_GuardedRegion.Register();
			m_UnguardedRegion = new YoungRegion(this, false);
			m_UnguardedRegion.Register();
		}

		public YoungRegionFlag( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(m_GuardedArea);
			writer.Write(m_UnguardedArea);
			writer.Write(m_RemoveLocation);
			writer.Write(m_RemoveMap);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			m_GuardedArea = reader.ReadRect3D();
			m_UnguardedArea = reader.ReadRect3D();
			m_RemoveLocation = reader.ReadPoint3D();
			m_RemoveMap = reader.ReadMap();

			UpdateRegions();
		}
	}

	public class YoungRegion : GuardedRegion
	{
		private YoungRegionFlag m_Flag;

		public YoungRegion(YoungRegionFlag flag, bool guarded)
			: base(String.Format("Young {0} Region", guarded? "Guarded" : "Unguarded"), flag.Map, guarded ? 150 : 125, guarded ? flag.GuardedArea3D : flag.UnguardedArea3D)
		{
			m_Flag = flag;
			Disabled = !guarded;
		}

		public override void OnEnter(Mobile m)
		{
			if (m.Player && !YoungRegionFlag.IsYoung((PlayerMobile)m) && m_Flag.RemoveLocation != Point3D.Zero && m.AccessLevel == AccessLevel.Player)
				Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(OnEnter_Callback), m);

			base.OnEnter(m);
		}

		private void OnEnter_Callback(object mob)
		{
			Mobile m = (Mobile)mob;
			m.SendMessage("You have been evicted from this area, as you have grown too old now.");
			m.MoveToWorld(m_Flag.RemoveLocation, m_Flag.RemoveMap);
		}
	}
}