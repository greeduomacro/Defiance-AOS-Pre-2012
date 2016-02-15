using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Text;

namespace Server.Items
{
	public class RentChestGump : AdvGump
	{
		private RentalChest m_Chest;

		public RentChestGump(RentalChest chest)
			: base(50, 50)
		{
			m_Chest = chest;
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(0, 0, 400, 210, 2620);
			AddAlphaRegion(5, 5, 490, 200);

			AddHtml(0, 15, 400, 20, Colorize(Center("Rent a chest"), "FFFFFF"), false, false);

			int[] collumns = new int[] { 150, 150 };
			string[] colors = new string[] { "FFFFFF", "FFFFFF" };
			AddHtml(0, 55, 400, 20, Colorize(Center("Do you really want to rent this chest"), "FFFFFF"), false, false);
			AddHtml(0, 75, 400, 20, Colorize(Center(string.Format("for the amount of {0} gp per week?", m_Chest.RentalCost)), "FFFFFF"), false, false);
			AddHtml(0, 95, 400, 20, Colorize(Center("You will be able to use it with all characters"), "FFFFFF"), false, false);
			AddHtml(0, 115, 400, 20, Colorize(Center("on your account, until you release the chest,"), "FFFFFF"), false, false);
			AddHtml(0, 135, 400, 20, Colorize(Center("or until you run out of gold in the bank of this character."), "FFFFFF"), false, false);
			AddButton(250, 170, 247, 248, 1, GumpButtonType.Reply, 0);
			AddButton(90, 170, 242, 243, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 1 )
			{
				PlayerMobile from = (PlayerMobile)sender.Mobile;
				if ( m_Chest.Rented && m_Chest.Owner != null )
					from.SendMessage( "The chest has been rented already by someone else." );
				else if ( Banker.Withdraw(from, m_Chest.RentalCost ) )
					m_Chest.BeginRent( from );
				else
					from.SendLocalizedMessage( 1019022 ); //You do not have enough gold.
			}
		}
	}

	public class ReleaseChestGump : AdvGump
	{
		private RentalChest m_Chest;

		public ReleaseChestGump(RentalChest chest)
			: base(50, 50)
		{
			m_Chest = chest;
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(0, 0, 400, 190, 2620);
			AddAlphaRegion(5, 5, 490, 180);

			AddHtml(0, 15, 400, 20, Colorize(Center("Release a chest"), "FFFFFF"), false, false);

			int[] collumns = new int[] { 150, 150 };
			string[] colors = new string[] { "FFFFFF", "FFFFFF" };
			AddHtml(0, 55, 400, 20, Colorize(Center("Do you really want to release your chest?"), "FFFFFF"), false, false);
			AddHtml(0, 75, 400, 20, Colorize(Center("Anybody will be able to take the items from it then."), "FFFFFF"), false, false);
			AddHtml(0, 115, 400, 20, Colorize(Center("You will be able rent any available chest again."), "FFFFFF"), false, false);
			AddButton(250, 150, 247, 248, 1, GumpButtonType.Reply, 0);
			AddButton(90, 150, 242, 243, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if ( info.ButtonID == 1 )
			{
				PlayerMobile from = (PlayerMobile)sender.Mobile;
				m_Chest.CancelRent();
			}
		}
	}

}