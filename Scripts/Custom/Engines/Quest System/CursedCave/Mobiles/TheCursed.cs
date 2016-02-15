using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Regions;

namespace Server.Mobiles
{
	[CorpseName("an inhuman corpse")]
	public class TheCursed : BaseCursedCreature
	{
		public override bool UseRearm { get { return true; } }
		public override InhumanSpeech SpeechType { get { return CursedCaveSpeech.Cursed; } }
		public override bool AlwaysMurderer { get { return true; } }

		[Constructable]
		public TheCursed()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Body = 183;
			Name = NameList.RandomName("male");
			Title = "the cursed";

			Hue = 2130;
			SetStr(120, 140);
			SetDex(86, 105);
			SetInt(61, 70);

			SetResistance(ResistanceType.Physical, 40, 60);
			SetResistance(ResistanceType.Fire, 40, 50);
			SetResistance(ResistanceType.Cold, 50, 60);
			SetResistance(ResistanceType.Poison, 55, 65);
			SetResistance(ResistanceType.Energy, 40, 50);

			SetSkill(SkillName.Fencing, 90.0);
			SetSkill(SkillName.Macing, 90.0);
			SetSkill(SkillName.Swords, 90.0);
			SetSkill(SkillName.Wrestling, 100.0);
			SetSkill(SkillName.Poisoning, 60.0, 82.5);
			SetSkill(SkillName.MagicResist, 57.5, 80.0);
			SetSkill(SkillName.Tactics, 60.0, 82.5);
			SetSkill(SkillName.Anatomy, 60.2, 100.0);
			SetSkill(SkillName.Parry, 57.5, 80.0);
			SetSkill(SkillName.Healing, 90.0);

			Fame = NotorietyHandlers.GetNotorietyByLevel( 2 );
			Karma = NotorietyHandlers.GetNotorietyByLevel( -2 );

			VirtualArmor = 70;

			Item wep = CursedCaveUtility.MutateItem(Loot.RandomWeapon(), 10);
			AddItem(wep);

			AddItem(CursedCaveUtility.MutateItem(new BoneLegs(), 10));
			AddItem(CursedCaveUtility.MutateItem(new BoneArms(), 10));
			AddItem(CursedCaveUtility.MutateItem(new BoneGloves(), 10));
			AddItem(CursedCaveUtility.MutateItem(new BoneChest(), 10));
			AddItem(CursedCaveUtility.MutateItem(new BoneHelm(), 10));

			if (0.10 > Utility.RandomDouble())
				AddItem(CursedCaveUtility.MutateItem(Loot.RandomJewelry(), 10));

			if (this.Weapon != null && ((BaseWeapon)this.Weapon).Layer == Layer.OneHanded)
			{
				if (Utility.RandomBool())
				{
					BaseEquipableLight lightItem;
					switch (Utility.Random(3))
					{
						default:
							lightItem = new Torch();
							break;
						case 1:
							lightItem = new Lantern();
							break;
						case 2:
							lightItem = new Candle();
							break;
					}
					lightItem.Ignite();
					AddItem(lightItem);
				}
				else
					AddItem(CursedCaveUtility.MutateItem(Loot.RandomShield(), 10));
			}
		}

		public override void GenerateLoot()
		{
			base.GenerateLoot();

			AddLoot(LootPack.AosRich);

			if (!m_Spawning)
				BoneRemains.PackSkullsAndSmallBones( Backpack, Utility.Random( 1, 2 ) );
		}

		public TheCursed(Serial serial)
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