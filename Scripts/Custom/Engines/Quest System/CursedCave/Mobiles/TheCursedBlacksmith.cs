using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	public class TheCursedBlacksmith : BaseCursedCreature
	{
		public override bool UseRearm { get { return true; } }
		public override bool ClickTitle { get { return false; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }

		[Constructable]
		public TheCursedBlacksmith()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Title = "the Cursed Blacksmith";
			Body = 0x190;
			Name = "Jonah";
			Hue = 2130;

			InitStats(700, 151, 100);
			SetHits(500);

			SetResistance(ResistanceType.Physical, 40, 60);
			SetResistance(ResistanceType.Fire, 20, 30);
			SetResistance(ResistanceType.Cold, 50, 60);
			SetResistance(ResistanceType.Poison, 55, 65);
			SetResistance(ResistanceType.Energy, 40, 50);

			SetSkill(SkillName.Wrestling, 120.0);
			SetSkill(SkillName.Fencing, 120.0);
			SetSkill(SkillName.Anatomy, 120.0);
			SetSkill(SkillName.MagicResist, 120.0);
			SetSkill(SkillName.Tactics, 120.0);
			SetSkill(SkillName.Healing, 90.0);

			Fame = NotorietyHandlers.GetNotorietyByLevel( 3 );
			Karma = NotorietyHandlers.GetNotorietyByLevel( -3 );

			VirtualArmor = 70;

			Item wep = CursedCaveUtility.MutateItem(new Kryss(), 30);
			AddItem(wep);

			AddItem(new Shoes());

			CraftResource ArmorCraftResource = GetRandomCraftResource();

			PlateArms arms = new PlateArms();
			arms.Resource = ArmorCraftResource;
			AddItem(CursedCaveUtility.MutateItem(arms, 10));

			PlateChest tunic = new PlateChest();
			tunic.Resource = ArmorCraftResource;
			AddItem(CursedCaveUtility.MutateItem(tunic, 10));

			PlateLegs legs = new PlateLegs();
			legs.Resource = ArmorCraftResource;
			AddItem(CursedCaveUtility.MutateItem(legs, 10));

			HairItemID = 0x203B;
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
					Item hammer = new SRCRSturdySmithHammer();
					AddItem(hammer);
				}
				else AddItem(new SmithHammer());
			}
		}

		public CraftResource GetRandomCraftResource()
		{
			switch (Utility.Random(4))
			{
				default: return CraftResource.Iron;
				case 1: return CraftResource.DullCopper;
				case 2: return CraftResource.ShadowIron;
				case 3: return CraftResource.Copper;
			}
		}

		public TheCursedBlacksmith(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}