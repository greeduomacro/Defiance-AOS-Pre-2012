using Server.Gumps;
using Server;
using System;
using System.Collections;
using Server.Mobiles;
using Server.Factions;
using Server.Network;
using Server.Items;
using Server.Regions;

namespace Server.Items
{
	public class FactionScoreBoard : Item
	{
		[Constructable]
		public FactionScoreBoard() : base( 0x1e5e )
		{
			this.Movable = false;
			this.Name = "Faction Score Board";
		}

		public FactionScoreBoard( Serial serial ) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					break;
				}
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			list.Add( Utility.FixHtml( "Faction Score Board" ) );
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.CloseGump( typeof( FactionScoreBoardGump ) );
			from.SendGump( new FactionScoreBoardGump( (PlayerMobile)from ) );
		}
	}
}

namespace Server.Gumps
{
	public class FactionScoreComparer : IComparer
	{
		public int Compare( object a, object b )
		{
			if ( !( a is PlayerState ) || !( b is PlayerState ) )
				return 0;
			PlayerState psa = (PlayerState)a;
			PlayerState psb = (PlayerState)b;
			if ( psa.KillPoints > psb.KillPoints )
				return -1;
			else if ( psa.KillPoints < psb.KillPoints )
				return 1;
			else
				return 0;
		}
	}

	public class FactionScoreBoardGump : Gump
	{
		public FactionScoreBoardGump( PlayerMobile from ) : base( 20, 30 )
		{
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(-8, 2, 649, 399, 9380);

			ArrayList members = GetFactionTopList( from );

			for ( int i = 0; i < members.Count; i++ )
			{
				if ( i >= 20 )
					break;

				Faction faction = ((PlayerState)members[i]).Faction;
				Mobile m = ((PlayerState)members[i]).Mobile;
				string guildabb = null;

				if ( m.Guild != null )
				{
					guildabb = String.Format( "[{0}]", m.Guild.Abbreviation );
				}

				AddHtml( 20, 40, 200, 30, "Rank", false, false );
				AddHtml( 20, 60 + i * 15, 200, 30, (i + 1).ToString(), false, false );

				AddHtml( 60, 40, 200, 30, "Player", false, false );
				AddHtml( 60, 60 + i * 15, 200, 30, ((PlayerState)members[i]).Mobile.Name, false, false );

				AddHtml( 160, 40, 200, 30, "Guild", false, false );
				if ( guildabb != null )
					AddHtml( 160, 60 + i * 15, 200, 30, guildabb, false, false );

				AddHtml( 210, 40, 200, 30, "Faction", false, false );
				AddHtml( 210, 60 + i * 15, 200, 30, faction.Definition.FriendlyName, false, false );

				AddHtml( 340, 40, 180, 30, "Score", false, false );
				AddHtml( 340, 60 + i * 15, 180, 30, ((PlayerState)members[i]).KillPoints.ToString(), false, false );

				AddHtml( 390, 40, 180, 30, "Title", false, false );
				AddHtml( 390, 60 + i * 15, 240, 30, ((PlayerState)members[i]).Rank.Title.String, false, false );
			}
		}

		public ArrayList GetFactionTopList( PlayerMobile from )
		{
			ArrayList members = new ArrayList();

			foreach ( PlayerState playerstate in CouncilOfMages.Instance.Members )
			{
				members.Add( playerstate );
			}

			foreach ( PlayerState playerstate in Shadowlords.Instance.Members )
			{
				members.Add( playerstate );
			}

			//Edit begin
			foreach ( PlayerState playerstate in Minax.Instance.Members )
			{
				members.Add( playerstate );
			}

			foreach ( PlayerState playerstate in TrueBritannians.Instance.Members )
			{
				members.Add( playerstate );
			}
			//Edit end

			FactionScoreComparer comparer = new FactionScoreComparer();
			members.Sort( comparer );

			return members;
		}
	}
}