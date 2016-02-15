using Server.Gumps;
using Server.HuePickers;
using Server.Items;

namespace Server.ContextMenus
{
	public class ReleaseChestEntry : ContextMenuEntry
	{
		private RentalChest m_Chest;
		private Mobile m_From;

		public ReleaseChestEntry( Mobile from, RentalChest chest ) : base( 6118, 3 )
		{
			m_Chest = chest;
			m_From = from;
		}

		public override void OnClick()
		{
			m_From.CloseGump(typeof(ReleaseChestGump));
			m_From.SendGump( new ReleaseChestGump(m_Chest));
		}
	}

	public class RentEntry : ContextMenuEntry
	{
		private RentalChest m_Chest;
		private Mobile m_From;

		public RentEntry( Mobile from, RentalChest chest ) : base( 6120, 3 )
		{
			m_Chest = chest;
			m_From = from;
		}

		public override void OnClick()
		{
			m_From.CloseGump(typeof(RentChestGump));
			m_From.SendGump( new RentChestGump(m_Chest));
		}
	}

	public class SelectHueEntry : ContextMenuEntry
	{
		private RentalChest m_Chest;
		private Mobile m_From;

		public SelectHueEntry( Mobile from, RentalChest chest ) : base( 313, 3 )
		{
			m_Chest = chest;
			m_From = from;
		}

		public override void OnClick()
		{
			m_From.SendLocalizedMessage( 1062622 ); // Select a hue you wish to apply to this item.
			m_From.SendHuePicker( new InternalPicker( m_Chest ) );
		}
	}

	public class InternalPicker : HuePicker
	{
		private RentalChest m_Chest;
		public InternalPicker( RentalChest chest ) : base( chest.ItemID )
		{
			m_Chest = chest;
		}

		public override void OnResponse( int hue )
		{
			m_Chest.Hue = hue;
		}
	}

}