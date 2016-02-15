using System;
using Server;

namespace Server.Items
{
	public enum HeadType
	{
		Regular,
		Duel,
		Tournament
	}

	public class Head : Item
	{
		private string m_PlayerName;
		private HeadType m_HeadType;
		public Mobile HeadOwner;
		private Mobile m_HeadKiller;
		public DateTime m_DateKill;

		[CommandProperty( AccessLevel.Administrator )]
		public Mobile HeadKiller
		{
			get { return m_HeadKiller; }
			set { m_HeadKiller = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.Administrator )]
		public DateTime DateKill
		{
			get { return m_DateKill; }
			set { m_DateKill = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string PlayerName
		{
			get { return m_PlayerName; }
			set { m_PlayerName = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public HeadType HeadType
		{
			get { return m_HeadType; }
			set { m_HeadType = value; InvalidateProperties(); }
		}

		public override string DefaultName
		{
			get
			{
				if ( m_PlayerName == null )
					return base.DefaultName;

				switch ( m_HeadType )
				{
					default:
						return String.Format( "the head of {0}", m_PlayerName );

					case HeadType.Duel:
						return String.Format( "the head of {0}, taken in a duel", m_PlayerName );

					case HeadType.Tournament:
						return String.Format( "the head of {0}, taken in a tournament", m_PlayerName );
				}
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			if ( HeadKiller != null && DateKill > DateTime.MinValue )
				list.Add( 1070722, String.Format("Killed by {0}, {1}-{2}-{3}", HeadKiller.Name, DateKill.Year, DateKill.Month, DateKill.Day ));
		}

		[Constructable]
		public Head()
			: this( null )
		{
		}

		[Constructable]
		public Head( string playerName )
			: this( HeadType.Regular, playerName )
		{
		}

		[Constructable]
		public Head( HeadType headType, string playerName )
			: base( 0x1DA0 )
		{
			m_HeadType = headType;
			m_PlayerName = playerName;
		}

		public Head( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			if (HeadKiller != null)
			{
				writer.Write( (int) 2 ); // version
				writer.Write( (Mobile) HeadKiller);
				writer.Write( (Mobile) HeadOwner);
				writer.WriteDeltaTime( (DateTime) DateKill);
			}
			else writer.Write( (int) 1 ); // version

			writer.Write( (string) m_PlayerName );
			writer.WriteEncodedInt( (int) m_HeadType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
					HeadKiller = reader.ReadMobile();
					HeadOwner = reader.ReadMobile();
					DateKill = reader.ReadDeltaTime();
					goto case 1;
				case 1:
					m_PlayerName = reader.ReadString();
					m_HeadType = (HeadType) reader.ReadEncodedInt();
					break;

				case 0:
					string format = this.Name;

					if ( format != null )
					{
						if ( format.StartsWith( "the head of " ) )
							format = format.Substring( "the head of ".Length );

						if ( format.EndsWith( ", taken in a duel" ) )
						{
							format = format.Substring( 0, format.Length - ", taken in a duel".Length );
							m_HeadType = HeadType.Duel;
						}
						else if ( format.EndsWith( ", taken in a tournament" ) )
						{
							format = format.Substring( 0, format.Length - ", taken in a tournament".Length );
							m_HeadType = HeadType.Tournament;
						}
					}

					m_PlayerName = format;
					this.Name = null;

					break;
			}
		}
	}
}