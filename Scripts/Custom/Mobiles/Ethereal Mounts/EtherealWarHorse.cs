using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.ContextMenus;
using Server.Factions;
namespace Server.Mobiles
{
public class EtherealWarHorse : EtherealMount
	{
		public override int EtherealHue { get { return 0; } }

		[Constructable]
		public EtherealWarHorse()
			: base(11676, 16018)
		{
			Name = "Ethereal War Horse Statuette";
			OriginalHue = -1;
		}

		public EtherealWarHorse(Serial serial)
			: base(serial)
		{
		}

		/**************************************************/
		public override void OnDoubleClick( Mobile from )	//Added by Opticon
		{
			Faction faction = Faction.Find( from );

			if ( faction == null )
				from.SendMessage("You have to be a faction member to use this item!");
			else
				base.OnDoubleClick(from);
		}
		/*************************************************/

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}