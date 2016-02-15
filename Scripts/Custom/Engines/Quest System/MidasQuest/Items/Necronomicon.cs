//Opticon
using System;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
	public class Necronomicon : Spellbook
	{
		public override SpellbookType SpellbookType{ get{ return SpellbookType.Necromancer; } }
		public override int BookOffset{ get{ return 100; } }
		public override int BookCount{ get{ return ((Core.AOS) ? 17 : 16); } }

		[Constructable]
		public Necronomicon() : this( (ulong)0 )
		{
			Name = "Necronomicon";
			LootType = LootType.Regular;
			Slayer = SlayerName.Fey;
			Attributes.CastRecovery = 2;
			Hue = 0x4A6;
			SkillBonuses.SetValues( 0, SkillName.SpiritSpeak, 25.0 );
		}

		[Constructable]
		public Necronomicon( ulong content ) : base( content, 0x2253 )
		{
			Layer = (Core.AOS ? Layer.OneHanded : Layer.Invalid);
		}
		public Necronomicon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if( version == 0 && Core.AOS )
				Layer = Layer.OneHanded;
		}

		public override bool OnEquip( Mobile from )
		{
			if(from.Karma>-5000)
			{
			from.SendMessage( "You feel very strange." );
			Misc.Titles.AwardKarma( from, -1000, true );
			return base.OnEquip( from );
			}
			else
			{
			from.SendMessage( "You are already cursed enough." );
			return base.OnEquip( from );
			}
		}
	}
}