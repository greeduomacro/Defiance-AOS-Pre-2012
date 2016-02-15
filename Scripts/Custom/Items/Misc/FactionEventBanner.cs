using System;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Factions;

namespace Server.Regions
{
	public class FactionEventRegionFlag : Item
	{
		private Region m_EventRegion;
		private Rectangle3D m_EventArea = new Rectangle3D();
		private bool m_EventRunning;

		[CommandProperty(AccessLevel.GameMaster)]
		public Rectangle2D EventArea2D
		{
			get { return new Rectangle2D(m_EventArea.Start, m_EventArea.End); }
			set { m_EventArea = new Rectangle3D(value.X, value.Y, -150, value.Width, value.Height, 300); UpdateRegions(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool EventRunning
		{
			get { return m_EventRunning; }
			set { m_EventRunning = value; }
		}

		public Rectangle3D EventArea3D { get { return m_EventArea; } }

		[Constructable]
		public FactionEventRegionFlag()
			: base(5609)
		{
			Visible = false;
			Movable = false;
			Name = "Faction Event Region Flag";
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from.AccessLevel > AccessLevel.Counselor)
				from.SendGump(new PropertiesGump(from, this));

			base.OnDoubleClick(from);
		}

		private void UpdateRegions()
		{
			if (m_EventRegion != null)
				m_EventRegion.Unregister();

			m_EventRegion = new FactionEventRegion(this);
			m_EventRegion.Register();
		}

		public FactionEventRegionFlag(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);//version
			writer.Write(m_EventArea);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			m_EventArea = reader.ReadRect3D();
			UpdateRegions();
		}
	}

	public class FactionEventRegion : Region
	{
		private FactionEventRegionFlag m_Flag;

		public FactionEventRegion(FactionEventRegionFlag flag)
			: base("Faction Event Region", flag.Map, 160, flag.EventArea3D)
		{
			m_Flag = flag;
		}

		public override void OnEnter(Mobile m)
		{
			if (m.Player && m.AccessLevel == AccessLevel.Player && Faction.Find(m) == null)
				Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(OnEnter_Callback), m);

			base.OnEnter(m);
		}

		public override bool AllowHarmful(Mobile from, Mobile target)
		{
			if (m_Flag.EventRunning && (Faction.Find(from, true) != Faction.Find(target, true)))
				return true;

			return false;
		}

		public override bool AllowBeneficial(Mobile from, Mobile target)
		{
			if (m_Flag.EventRunning && (Faction.Find(from, true) != Faction.Find(target, true)))
				return false;

			return true;
		}

		private void OnEnter_Callback(object mob)
		{
			Mobile m = (Mobile)mob;
			m.SendMessage("You have been evicted from this area, as you have grown too old now.");
			m.MoveToWorld(new Point3D(1434, 1707, 18), Map.Felucca);
		}
	}
}