using System;
using Server;
using Server.Items;
using Server.Gumps;

namespace Server.ContextMenus
{
	public class LevelInfoEntry : ContextMenuEntry
	{
		private ILevelable m_Item;

		public LevelInfoEntry( ILevelable item ) : base( 98, 3 )
		{
			m_Item = item;
		}

		public override void OnClick()
		{
			Owner.From.CloseGump( typeof( ItemInfoGump ) );
			Owner.From.SendGump( new ItemInfoGump( m_Item ) );
		}
	}
}