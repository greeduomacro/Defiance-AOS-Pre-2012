using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	public class TheCursedCarpenter : BaseCursedCreature
	{
		public override bool UseRearm { get { return true; } }
		public override bool ClickTitle { get { return false; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }

		[Constructable]
		public TheCursedCarpenter()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Title = "the Cursed Carpenter";
			Body = 0x190;
			Name = "Khal";
			Hue = 2130;

			InitStats(700, 151, 100);
			SetHits(500);

			SetResistance(ResistanceType.Physical, 40, 60);
			SetResistance(ResistanceType.Fire, 20, 30);
			SetResistance(ResistanceType.Cold, 50, 60);
			SetResistance(ResistanceType.Poison, 55, 65);
			SetResistance(ResistanceType.Energy, 40, 50);

			SetSkill(SkillName.Wrestling, 120.0);
			SetSkill(SkillName.Macing, 120.0);
			SetSkill(SkillName.Anatomy, 120.0);
			SetSkill(SkillName.MagicResist, 120.0);
			SetSkill(SkillName.Tactics, 120.0);
			SetSkill(SkillName.Healing, 90.0);

			Fame = NotorietyHandlers.GetNotorietyByLevel( 3 );
			Karma = NotorietyHandlers.GetNotorietyByLevel( -3 );

			VirtualArmor = 70;

			Item wep = CursedCaveUtility.MutateItem(new Club(), 10);
			AddItem(wep);

			AddItem( new Shoes() );
			AddItem( new LongPants() );
			AddItem( new FancyShirt() );
			AddItem( new HalfApron() );

			HairItemID = 0x203D;
			HairHue = 0x3C0;
		}

		public override int GetIdleSound()
		{
			return 0x1CE;
		}

		public override int GetAngerSound()
		{
			return 0x1AC;
		}

		public override int GetDeathSound()
		{
			return 0x182;
		}

		public override int GetHurtSound()
		{
			return 0x28D;
		}

		public override void GenerateLoot()
		{
			base.GenerateLoot();

			AddLoot(LootPack.FilthyRich);
			if (!m_Spawning)
			{
				if (0.10 > Utility.RandomDouble())
				{
					Item saw = new SRCRSturdySaw();
					AddItem(saw);
				}
				else AddItem(new Saw());
			}
		}

		public TheCursedCarpenter( Serial serial ) : base( serial )
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