using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class CaveHoleComponent : AddonComponent
	{
		public override string DefaultName{ get{ return "a hole"; } }
		private bool m_Active, m_AllowPets;
		private Point3D m_PointDest;
		private Map m_MapDest;

		#region CommandProperties
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active
		{
			get { return m_Active; }
			set { m_Active = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AllowPets
		{
			get { return m_AllowPets; }
			set { m_AllowPets = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D PointDest
		{
			get { return m_PointDest; }
			set { m_PointDest = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Map MapDest
		{
			get { return m_MapDest; }
			set { m_MapDest = value; InvalidateProperties(); }
		}
		#endregion

		[Constructable]
		public CaveHoleComponent(int itemID ) : base( itemID )
		{
			m_Active = m_AllowPets = true;
			Hue = 1;
			MapDest = Map.Felucca;
		}

		public override void OnDoubleClick( Mobile m )
		{
			StartTeleport( m );
		}

		private void StartTeleport( Mobile m )
		{
			if ( m.Player && m.InRange( Location, 1 ) )
				DoTeleport( m );
			else
				m.SendLocalizedMessage( 1019045 ); // I can't reach that.
		}

		public virtual void DoTeleport( Mobile m )
		{
			Map map = m_MapDest;

			if ( map == null || map == Map.Internal )
				map = m.Map;

			Point3D p = m_PointDest;

			if ( p == Point3D.Zero )
				p = m.Location;

			if ( m_AllowPets )
				BaseCreature.TeleportPets( m, p, map );
			else
			{
				int pets = 0;

				if ( m.Mounted /*&& !(m.Mount is EtherealMount)*/ )
				{
					m.Mount.Rider = null; //Dismount
					if ( !(m.Mount is EtherealMount) )
						pets++;
				}

				foreach ( Mobile mob in m.GetMobilesInRange( 3 ) )
				{
					if ( mob is BaseCreature )
					{
						BaseCreature pet = (BaseCreature)mob;

						if ( pet.Controlled && pet.ControlMaster == m && ( pet.ControlOrder == OrderType.Guard || pet.ControlOrder == OrderType.Follow || pet.ControlOrder == OrderType.Come ) )
							pets++;
					}
				}
				if ( pets > 0 )
					m.SendMessage( "A magical barrier has prevented your pet{0} from transporting.", pets != 1 ? "s" : "" );
			}

			m.MoveToWorld( p, map );
			Effects.PlaySound( m.Location, m.Map, 0x228 );
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m.Player )
				StartTeleport( m );
				return false;
		}

		public CaveHoleComponent( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 3 ); // version

			// Version 2
			writer.Write( (bool) m_AllowPets );
			writer.Write( (bool) m_Active );
			writer.Write( (Point3D) m_PointDest );
			writer.Write( (Map) m_MapDest );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
				case 2:
				{
					m_AllowPets = reader.ReadBool();
					m_Active = reader.ReadBool();
					if ( version < 3 )
					{
						/*m_OnMoveOver =*/ reader.ReadBool();
						/*m_OnDoubleClick =*/ reader.ReadBool();
					}
					m_PointDest = reader.ReadPoint3D();
					m_MapDest = reader.ReadMap();
					break;
				}
				case 1:
				{
					reader.ReadString();
					goto case 0;
				}
				case 0:
				{
					m_AllowPets = reader.ReadBool();
					m_Active = reader.ReadBool();
					/*m_OnMoveOver =*/ reader.ReadBool();
					/*m_OnDoubleClick =*/ reader.ReadBool();
					m_PointDest = reader.ReadPoint3D();
					m_MapDest = reader.ReadMap();
					break;
				}
			}
		}
	}
}