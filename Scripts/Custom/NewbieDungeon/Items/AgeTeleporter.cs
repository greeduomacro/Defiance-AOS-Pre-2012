using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
	public class AgeTeleporter : Teleporter
	{
		private TimeSpan m_GameTimeMax = TimeSpan.FromDays( 3.0 );
		private TimeSpan m_AgeMax = TimeSpan.FromDays( 14.0 );
		private bool m_UseGameTime = true;
		private bool m_UseAge = true;

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan GameTimeMax
		{
			get{ return m_GameTimeMax; }
			set{ m_GameTimeMax = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan AgeMax
		{
			get{ return m_AgeMax; }
			set{ m_AgeMax = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool UseGameTime
		{
			get{ return m_UseGameTime; }
			set{ m_UseGameTime = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool UseAge
		{
			get{ return m_UseAge; }
			set{ m_UseAge = value; InvalidateProperties(); }
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( Active )
			{
				if ( !Creatures && !m.Player )
					return true;
				else if ( CombatCheck && SpellHelper.CheckCombat( m ) )
				{
					m.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
					return true;
				}
				else if ( Factions.Sigil.ExistsOn( m ) )
				{
					m.SendLocalizedMessage( 1061632 ); // You can't do that while carrying the sigil.
					return true;
				}
				else if ( m_UseGameTime && m.Player && ((PlayerMobile)m).GameTime > m_GameTimeMax )
				{
					m.SendMessage( "Your ingame time is too long to enter this place" );
					return true;
				}
				else if ( m_UseAge && ( DateTime.Now - m.CreationTime ) > m_AgeMax )
				{
					m.SendMessage( "Your character is too old to enter this place" );
					return true;
				}

				StartTeleport( m );
				return false;
			}

			return true;
		}

		[Constructable]
		public AgeTeleporter()
		{
		}

		public AgeTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_GameTimeMax );
			writer.Write( m_AgeMax );
			writer.Write( m_UseGameTime );
			writer.Write( m_UseAge );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_GameTimeMax = reader.ReadTimeSpan();
					m_AgeMax = reader.ReadTimeSpan();
					m_UseGameTime = reader.ReadBool();
					m_UseAge = reader.ReadBool();

					break;
				}
			}
		}
	}
}