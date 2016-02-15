using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public class FortuneGump : Gump
	{
		private Sphynx m_Sphynx;

		public FortuneGump( Sphynx sphynx ) : base( 150, 50 )
		{
			m_Sphynx = sphynx;

			Closable=false;
			Disposable=false;
			Dragable=true;
			Resizable=false;
			AddPage( 0 );
			AddImage( 0, 0, 3600 );
			AddImageTiled( 0, 14, 15, 200, 3603 );
			AddImageTiled( 380, 14, 14, 200, 3605 );
			AddImage( 0, 201, 3606 );
			AddImageTiled( 15, 201, 370, 16, 3607 );
			AddImageTiled( 15, 0, 370, 16, 3601 );
			AddImage( 380, 0, 3602 );
			AddImage( 380, 201, 3608 );
			AddImageTiled( 15, 15, 365, 190, 2624);
			AddRadio( 30, 140, 9727, 9730, false, 1 );
			AddHtmlLocalized( 65, 145, 300, 25, 1060863, 32767, false, false ); // Pay for the reading.
			AddRadio( 30, 175, 9727, 9730, true, 2 );
			AddHtmlLocalized( 65, 178, 300, 25, 1060862, 32767, false, false ); // No thanks. I decide my own destiny!
			AddHtmlLocalized( 30, 20, 360, 35, 1060864, 32767, false, false ); // Interested in your fortune, are you?  The ancient Sphynx can read the future for you - for a price of course...
			AddImage( 65, 72, 5605 );
			AddImageTiled( 80, 90, 200, 1, 9107 );
			AddImageTiled( 95, 92, 200, 1, 9157 );
			AddLabel( 90, 70, 140, "5000" );
			AddHtmlLocalized( 140, 70, 100, 25, 1023823, 32767, false, false ); // gold coins
			AddButton( 290, 175, 247, 248, 1, GumpButtonType.Reply, 0 );
			AddImageTiled( 15, 14, 365, 1, 9107 );
			AddImageTiled( 380, 14, 1, 190, 9105 );
			AddImageTiled( 15, 205, 365, 1, 9107 );
			AddImageTiled( 15, 14, 1, 190, 9105 );
			AddImageTiled( 0, 0, 395, 1, 9157 );
			AddImageTiled( 394, 0, 1, 217, 9155 );
			AddImageTiled( 0, 216, 395, 1, 9157 );
			AddImageTiled( 0, 0, 1, 217, 9155 );
			AddHtmlLocalized( 30, 105, 340, 40, 1060865, 0xB5CE6B, false, false ); // Do you accept this offer?  The funds will be withdrawn from your backpack.
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( m_Sphynx == null || m_Sphynx.Deleted )
				return;

			PlayerMobile m = sender.Mobile as PlayerMobile;

			if ( info.ButtonID == 1 && info.IsSwitched( 1 ) )
			{
				Container pack = m.Backpack;

				if ( pack != null && pack.ConsumeTotal( typeof( Gold ), 5000 ) )
				{
					m.SendLocalizedMessage( 1060867 ); // You pay the fee.
					TellFortune( m );
					m_Told.Add( m );
					m.FortuneExpire = DateTime.Now + TimeSpan.FromHours( 24 );
				}
				else
					m.SendLocalizedMessage( 1061006 ); // You haven't got the coin to make the proper donation to the Sphynx.  Your fortune has not been read.
			}
			else
				m.SendLocalizedMessage( 1061007 ); // You decide against having your fortune told.
		}

		public static void TellFortune( PlayerMobile from )
		{
			switch ( Utility.Random( 20 ) )
			{
				case 0: from.ApplyFortune( 1, Utility.RandomMinMax( 1, 15 ) ); break;		//+1 to +15 Phys
				case 1: from.ApplyFortune( 2, Utility.RandomMinMax( 1, 15 ) ); break;		//+1 to +15 Fire
				case 2: from.ApplyFortune( 3, Utility.RandomMinMax( 1, 15 ) ); break;		//+1 to +15 Cold
				case 3: from.ApplyFortune( 4, Utility.RandomMinMax( 1, 15 ) ); break;		//+1 to +15 Poison
				case 4: from.ApplyFortune( 5, Utility.RandomMinMax( 1, 15 ) ); break;		//+1 to +15 Energy
				case 5: from.ApplyFortune( 6, Utility.RandomMinMax( 10, 50 ) ); break;		//+10 to +50 Luck
				case 6: from.ApplyFortune( 7, Utility.RandomMinMax( 1, 5 ) * 5 ); break;	//+5 to +25 Enhance Potions
				case 7: from.ApplyFortune( 8, Utility.RandomMinMax( 50, 100 ) ); break;		//+50 to +100 Luck
				case 8: from.ApplyFortune( 9, Utility.RandomMinMax( 1, 15 ) ); break;		//+1 to +15 Defense
				case 9: from.ApplyFortune( 10, Utility.RandomMinMax( 1, 3 ) ); break;		//+1 to +3 Mana regan
				case 10: from.ApplyFortune( 11, Utility.RandomMinMax( 1, 15 ) ); break;		//-1 to -15 Phys
				case 11: from.ApplyFortune( 12, Utility.RandomMinMax( 1, 15 ) ); break;		//-1 to -15 Fire
				case 12: from.ApplyFortune( 13, Utility.RandomMinMax( 1, 15 ) ); break;		//-1 to -15 Cold
				case 13: from.ApplyFortune( 14, Utility.RandomMinMax( 1, 15 ) ); break;		//-1 to -15 Poison
				case 14: from.ApplyFortune( 15, Utility.RandomMinMax( 1, 15 ) ); break;		//-1 to -15 Energy
				case 15: from.ApplyFortune( 16, Utility.RandomMinMax( 10, 50 ) ); break;	//-10 to -50 Luck
				case 16: from.ApplyFortune( 17, Utility.RandomMinMax( 1, 5 ) * 5 ); break;	//-5 to -25 Enhance Potions
				case 17: from.ApplyFortune( 18, Utility.RandomMinMax( 50, 100 ) ); break;	//-50 to -100 Luck
				case 18: from.ApplyFortune( 19, Utility.RandomMinMax( 1, 10 ) ); break;		//-1 to -10 Defense
				case 19: from.ApplyFortune( 20, Utility.RandomMinMax( 1, 3 ) ); break;		//-1 to -3 Mana Regen
			}
		}

		private static List<PlayerMobile> m_Told = new List<PlayerMobile>();

		public static List<PlayerMobile> Told{ get{ return m_Told; } set{ m_Told = value; } }
	}
}