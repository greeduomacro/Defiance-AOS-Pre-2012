using System;
using Server.Misc;
using Server.Network;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	public class TheCursedGuardian : BaseCursedCreature
	{
		public override bool UseRearm { get { return true; } }
		public override bool ClickTitle { get { return false; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override InhumanSpeech SpeechType { get { return CursedCaveSpeech.Cursed; } }

		[Constructable]
		public TheCursedGuardian()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Body = 0x190;
			Name = NameList.RandomName("male");
			Title = "the cursed guardian";
			Hue = 2130;

			SetStr(350);
			SetDex(150);
			SetInt(100);

			SetHits(500);

			SetResistance(ResistanceType.Physical, 40, 60);
			SetResistance(ResistanceType.Fire, 20, 30);
			SetResistance(ResistanceType.Cold, 50, 60);
			SetResistance(ResistanceType.Poison, 55, 65);
			SetResistance(ResistanceType.Energy, 40, 50);

			SetSkill(SkillName.Wrestling, 120.0, 125.0);
			SetSkill(SkillName.Macing, 120.0, 125.0);
			SetSkill(SkillName.Anatomy, 120.0, 125.0);
			SetSkill(SkillName.MagicResist, 90.0, 94.0);
			SetSkill(SkillName.Tactics, 120.0, 125.0);
			SetSkill(SkillName.Healing, 90.0);
			SetSkill(SkillName.Parry, 65.0, 80.0);

			Fame = NotorietyHandlers.GetNotorietyByLevel( 3 );
			Karma = NotorietyHandlers.GetNotorietyByLevel( -3 );

			VirtualArmor = 40;

			CraftResource ArmorCraftResource = GetRandomCraftResource();

			DragonArms arms = new DragonArms();
			arms.Resource = ArmorCraftResource;
			AddItem(CursedCaveUtility.MutateItem(arms, 10));

			DragonGloves gloves = new DragonGloves();
			gloves.Resource = ArmorCraftResource;
			AddItem(CursedCaveUtility.MutateItem(gloves, 10));

			DragonChest tunic = new DragonChest();
			tunic.Resource = ArmorCraftResource;
			AddItem(CursedCaveUtility.MutateItem(tunic, 10));

			DragonLegs legs = new DragonLegs();
			legs.Resource = ArmorCraftResource;
			AddItem(CursedCaveUtility.MutateItem(legs, 10));

			DragonHelm helm = new DragonHelm();
			helm.Resource = ArmorCraftResource;
			AddItem(CursedCaveUtility.MutateItem(helm, 10));

			AddItem(new Shoes());
			AddItem(CursedCaveUtility.MutateItem(new ChaosShield(), 10));

			Scepter weapon = new Scepter();
			AddItem(CursedCaveUtility.MutateItem(weapon, 10));
		}

		public TheCursedGuardian(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			base.GenerateLoot();

			AddLoot(LootPack.FilthyRich, 2);
			if (!m_Spawning)
			{
				BoneRemains.PackSkullsAndSmallBones( Backpack, Utility.Random( 1, 2 ) );
				if ( 0.05 > Utility.RandomDouble() )
					this.AddItem(new CCKey());
			}
		}

		public CraftResource GetRandomCraftResource()
		{
			switch (Utility.Random(6))
			{
				default: return CraftResource.BlackScales;
				case 1: return CraftResource.BlueScales;
				case 2: return CraftResource.GreenScales;
				case 3: return CraftResource.RedScales;
				case 4: return CraftResource.WhiteScales;
				case 5: return CraftResource.YellowScales;
			}
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