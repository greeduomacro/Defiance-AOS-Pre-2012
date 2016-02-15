using System;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
	public class BookOfLostKnowledge : NecromancerSpellbook
	{
		[Constructable]
		public BookOfLostKnowledge() : base( (ulong)0x1FFFF )
		{
			Name = "book of lost knowledge";
			Hue = 0x51C;
			LootType = LootType.Regular;

			Attributes.LowerManaCost = 10;
			Attributes.RegenMana = 3;

			SkillBonuses.SetValues( 0, SkillName.Necromancy, 10.0 );
			Slayer = SlayerName.Exorcism;
		}

		public BookOfLostKnowledge( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}