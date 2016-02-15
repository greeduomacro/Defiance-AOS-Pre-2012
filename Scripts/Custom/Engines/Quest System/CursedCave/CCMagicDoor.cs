using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class CCMagicDoor : Teleporter
	{
		private DoorFacing m_Facing;
		private CCSummoningAltar m_Altar;

		[CommandProperty( AccessLevel.GameMaster )]
		public DoorFacing Facing
		{
			get{ return m_Facing; }
			set{ m_Facing = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public CCSummoningAltar Altar
		{
			get { return m_Altar; }
			set { m_Altar = value; }
		}

		[Constructable]
		public CCMagicDoor( DoorFacing facing ) : base()
		{
			m_Facing = facing;
			ItemID = 0x677;
			Visible = true;
		}

		public CCMagicDoor( Serial serial ) : base( serial )
		{
		}

		public override void DoTeleport( Mobile m )
		{
			if ( m.Mounted ) //Dismount
				m.Mount.Rider = null;

			base.DoTeleport( m );
		}

		public override void OnDoubleClick(Mobile from)
		{
			base.OnDoubleClick(from);

			if (from.InRange(this.Location, 3))
				from.LocalOverheadMessage(MessageType.Regular, 0x5A, true, "This door seems to be magically locked");
		}

		public override bool OnMoveOver( Mobile m )
		{
			return true;
		}

		public override bool OnMoveOff( Mobile m )
		{
			return true;
		}

		public bool IsInside( Mobile from )
		{
			int x,y,w,h;

			const int r = 2;
			const int bs = r*2+1;
			const int ss = r+1;

			switch ( m_Facing )
			{
				case DoorFacing.WestCW:
				case DoorFacing.EastCCW: x = -r; y = -r; w = bs; h = ss; break;

				case DoorFacing.EastCW:
				case DoorFacing.WestCCW: x = -r; y = 0; w = bs; h = ss; break;

				case DoorFacing.SouthCW:
				case DoorFacing.NorthCCW: x = -r; y = -r; w = ss; h = bs; break;

				case DoorFacing.NorthCW:
				case DoorFacing.SouthCCW: x = 0; y = -r; w = ss; h = bs; break;

				//No way to test the 'insideness' of SE Sliding doors on OSI, so leaving them default to false until further information gained

				default: return false;
			}

			int rx = from.X - X;
			int ry = from.Y - Y;
			int az = Math.Abs( from.Z - Z );

			return ( rx >= x && rx < (x+w) && ry >= y && ry < (y+h) && az <= 4 );
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write( (int)0 ); // version

			// Version 0
			writer.Write( (int)m_Facing );
			writer.Write( m_Altar );
		}

		public override void Deserialize(GenericReader reader)
		{
			//base.SkipDeserialize( reader );

			base.Deserialize( reader );

			int version = reader.ReadInt();
/*
			switch (version)
			{
				case 1:
					m_Altar = reader.ReadItem() as CCSummoningAltar;
					goto case 0;
				case 0:
					// Version 0
					PointDest = reader.ReadPoint3D();
					Name = reader.ReadString();
					Active = reader.ReadBool();
					m_OneSideArea = reader.ReadRect2D();
					break;
			}
*/
			switch (version)
			{
				case 0:
					m_Facing = (DoorFacing)reader.ReadInt();
					m_Altar = reader.ReadItem() as CCSummoningAltar;
					break;
			}

		}
	}
}