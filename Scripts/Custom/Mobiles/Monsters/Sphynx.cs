//Taken from RunUO Forums - http://www.runuo.com/forums/custom-script-releases/82890-sphynx-re-release.html
//Implemented by Blady.

using System;
using System.Collections.Generic;
using Server;
using Server.Engines.RewardSystem;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Mobiles
{
	[CorpseName( "a sphynx corpse" )]
	public class Sphynx : BaseCreature
	{
		[Constructable]
		public Sphynx() : base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Body = 788;
			Name = "a sphynx";

			SetStr( 1001, 1200 );
			SetDex( 176, 195 );
			SetInt( 301, 400 );
			SetHits( 1001, 1200 );
			SetDamage( 10, 16 );

			SetDamageType( ResistanceType.Physical, 85 );
			SetDamageType( ResistanceType.Energy, 15 );

			SetResistance( ResistanceType.Physical, 60, 80 );
			SetResistance( ResistanceType.Fire, 30, 50 );
			SetResistance( ResistanceType.Cold, 40, 60 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Wrestling, 90.1, 100 );
			SetSkill( SkillName.Tactics, 90.1, 100 );
			SetSkill( SkillName.MagicResist, 100.5, 150 );
			SetSkill( SkillName.Anatomy, 55.1, 100 );
			SetSkill( SkillName.Magery, 95.5, 110 );
			SetSkill( SkillName.EvalInt, 90.1, 110 );
			SetSkill( SkillName.Meditation, 95.1, 120 );

			VirtualArmor = 78;
			Fame = 15000;
			Karma = 0;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override bool OnBeforeDeath()
		{
			if (Map == Map.Felucca)
				PackItem( new CopperBar(1) );
			return base.OnBeforeDeath();
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override bool AutoDispel{ get{ return true; } }

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from is PlayerMobile && !FortuneGump.Told.Contains( (PlayerMobile)from ) )
				list.Add( new FortuneEntry( this ) );
		}

		public class FortuneEntry : ContextMenuEntry
		{
			private Sphynx m_Sphynx;

			public FortuneEntry( Sphynx sphynx ) : base( 6199, 8 )
			{
				m_Sphynx = sphynx;
			}

			public override void OnClick()
			{
				if ( FortuneGump.Told.Contains( (PlayerMobile)Owner.From ) )
					return;

				Owner.From.CloseGump( typeof( FortuneGump ) );
				Owner.From.SendGump( new FortuneGump( m_Sphynx ) );
			}
		}

		public Sphynx( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}