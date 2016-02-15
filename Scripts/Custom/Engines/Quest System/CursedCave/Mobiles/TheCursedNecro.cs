using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("a cursed corpse")]
	public class TheCursedNecro : BaseCursedCreature
	{
		public override InhumanSpeech SpeechType { get { return CursedCaveSpeech.Cursed; } }
		public override bool AlwaysMurderer { get { return true; } }

		[Constructable]
		public TheCursedNecro()
			: base(AIType.AI_Necro, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Body = 183;
			Name = NameList.RandomName("male");
			Title = "the cursed necromancer";

			Hue = 2130;
			SetStr(96, 125);
			SetDex(86, 105);
			SetInt(61, 70);

			SetDamage(19, 31);

			SetResistance(ResistanceType.Physical, 40, 60);
			SetResistance(ResistanceType.Fire, 30, 40);
			SetResistance(ResistanceType.Cold, 50, 60);
			SetResistance(ResistanceType.Poison, 55, 65);
			SetResistance(ResistanceType.Energy, 40, 50);

			SetSkill(SkillName.SpiritSpeak, 75.1, 100.0);
			SetSkill(SkillName.Necromancy, 75.1, 100.0);
			SetSkill(SkillName.Poisoning, 75.1, 100.0);
			SetSkill(SkillName.Meditation, 75.1, 100.0);

			SetSkill(SkillName.MagicResist, 75.0, 97.5);
			SetSkill(SkillName.Tactics, 65.0, 87.5);
			SetSkill(SkillName.Wrestling, 20.2, 60.0);

			Fame = NotorietyHandlers.GetNotorietyByLevel( 2 );
			Karma = NotorietyHandlers.GetNotorietyByLevel( -2 );

			VirtualArmor = 70;

			int hue = Utility.RandomBlueHue();
			AddItem(new WizardsHat(hue));
			AddItem(new Robe(hue));
			AddItem(new Sandals());
			AddItem(CursedCaveUtility.MutateItem(new BlackStaff(), 10));
		}

		public override void GenerateLoot()
		{
			base.GenerateLoot();

			AddLoot(LootPack.AosRich);
			if (!m_Spawning)
			{
				Loot.AddRegs(Backpack, Utility.RandomMinMax(10, 20), true);
				Loot.AddScrolls(Backpack, Utility.RandomMinMax(2, 4), true);
				Loot.AddPotions(Backpack, Utility.RandomMinMax(1, 2));
				BoneRemains.PackSkullsAndSmallBones( Backpack, Utility.Random( 1, 2 ) );
			}
		}

		public TheCursedNecro(Serial serial)
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