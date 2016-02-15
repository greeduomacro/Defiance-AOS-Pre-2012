using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Regions;

namespace Server.Mobiles
{
	[CorpseName("an inhuman corpse")]
	public class TheCursedArcher : BaseCursedCreature
	{
		public override bool UseRearm { get { return true; } }
		public override InhumanSpeech SpeechType { get { return CursedCaveSpeech.Cursed; } }
		public override bool AlwaysMurderer { get { return true; } }

		[Constructable]
		public TheCursedArcher()
			: base(AIType.AI_Archer, FightMode.Closest, 10, 3, 0.2, 0.4)
		{
			Body = 183;
			Name = NameList.RandomName("male");
			Title = "the cursed archer";

			Hue = 2130;
			SetStr(120, 140);
			SetDex(120, 140);
			SetInt(61, 70);

			SetResistance(ResistanceType.Physical, 40, 60);
			SetResistance(ResistanceType.Fire, 50, 60);
			SetResistance(ResistanceType.Cold, 50, 60);
			SetResistance(ResistanceType.Poison, 55, 65);
			SetResistance(ResistanceType.Energy, 40, 50);

			SetSkill(SkillName.Archery, 120.0);
			SetSkill(SkillName.Wrestling, 100.0);
			SetSkill(SkillName.MagicResist, 57.5, 80.0);
			SetSkill(SkillName.Tactics, 80.0, 100.0);
			SetSkill(SkillName.Anatomy, 80.2, 100.0);
			SetSkill(SkillName.Parry, 57.5, 80.0);
			SetSkill(SkillName.Healing, 90.0);

			Fame = NotorietyHandlers.GetNotorietyByLevel( 2 );
			Karma = NotorietyHandlers.GetNotorietyByLevel( -2 );

			VirtualArmor = 70;

			BaseRanged wep = (BaseRanged)CursedCaveUtility.MutateItem(Loot.RandomRangedWeapon(), 10);
			AddItem(wep);

			Item ammo = Loot.Construct(wep.AmmoType);
			ammo.Amount = Utility.RandomMinMax(50, 60);
			PackItem(ammo);

			AddItem(CursedCaveUtility.MutateItem(new BoneLegs(), 10));
			AddItem(CursedCaveUtility.MutateItem(new BoneArms(), 10));
			AddItem(CursedCaveUtility.MutateItem(new BoneGloves(), 10));
			AddItem(CursedCaveUtility.MutateItem(new BoneChest(), 10));
			AddItem(CursedCaveUtility.MutateItem(new BoneHelm(), 10));

			if (0.10 > Utility.RandomDouble())
				AddItem(CursedCaveUtility.MutateItem(Loot.RandomJewelry(), 10));
		}

		public override void GenerateLoot()
		{
			base.GenerateLoot();

			AddLoot(LootPack.AosRich);

			if (!m_Spawning)
				BoneRemains.PackSkullsAndSmallBones( Backpack, Utility.Random( 1, 2 ) );
		}

		public TheCursedArcher(Serial serial)
			: base(serial)
		{
		}

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