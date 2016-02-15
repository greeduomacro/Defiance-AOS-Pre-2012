using System;
using Server;
using Server.Items;
using System.Text;

namespace Server.Gumps
{
	public class ItemInfoGump : Gump
	{
		public ItemInfoGump( ILevelable item ): base( 50, 50 )
		{
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			this.AddPage(0);
			this.AddBackground(0, 0, 273, 388, 2620);
			this.AddAlphaRegion(5, 7, 262, 373);
			this.AddLabel(45, 15, 955, @"Levelable Weapon Information");
			this.AddLabel(20, 50, 955, @"This item's attributes are level based.");
			this.AddLabel(20, 68, 955, @"You can gain levels by fighting the");
			this.AddLabel(20, 86, 955, @"monsters in the following list.");
			this.AddButton(233, 354, 4017, 4018, (int)Buttons.Close, GumpButtonType.Reply, 0);
			this.AddLabel(192, 354, 955, @"Close");

			StringBuilder builder = new StringBuilder();

			builder.Append("<div align=center><i>MONSTER LIST</i></div>");

			if (item.Types != null)
			{
				for (int i = 0; i < item.Types.Length; ++i)
				{
					builder.Append(item.Types[i].Name);
					builder.Append("<br>");
				}
			}

			builder.Append(item.GetHtml());

			this.AddHtml(13, 110, 247, 235, builder.ToString(), (bool)true, (bool)true);
		}

		public enum Buttons
		{
			Close,
		}
	}
}