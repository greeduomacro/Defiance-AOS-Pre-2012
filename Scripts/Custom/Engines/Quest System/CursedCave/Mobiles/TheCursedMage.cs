using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("a cursed corpse")]
	public class TheCursedMage : BaseCursedCreature
	{
		public override InhumanSpeech SpeechType { get { return CursedCaveSpeech.Cursed; } }
		public override bool AlwaysMurderer { get { return true; } }

		[Constructable]
		public TheCursedMage()
			: base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Body = 183;
			Name = NameList.RandomName("male");
			Title = "the cursed mage";

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

			SetSkill(SkillName.EvalInt, 75.1, 100.0);
			SetSkill(SkillName.Magery, 75.1, 100.0);
			SetSkill(SkillName.Poisoning, 75.1, 100.0);
			SetSkill(SkillName.Meditation, 75.1, 100.0);

			SetSkill(SkillName.MagicResist, 75.0, 97.5);
			SetSkill(SkillName.Tactics, 65.0, 87.5);
			SetSkill(SkillName.Wrestling, 20.2, 60.0);

			Fame = NotorietyHandlers.GetNotorietyByLevel( 2 );
			Karma = NotorietyHandlers.GetNotorietyByLevel( -2 );

			VirtualArmor = 70;

			int hue = Utility.RandomRedHue();
			AddItem(new WizardsHat(hue));
			AddItem(new Robe(hue));
			AddItem(new Sandals());
		}

		public override void GenerateLoot()
		{
			base.GenerateLoot();

			AddLoot(LootPack.AosRich);
			if (!m_Spawning)
			{
				Loot.AddRegs(Backpack, Utility.RandomMinMax(10, 20), false);
				Loot.AddScrolls(Backpack, Utility.RandomMinMax(2, 4), false);
				Loot.AddPotions(Backpack, Utility.RandomMinMax(1, 2));
				BoneRemains.PackSkullsAndSmallBones( Backpack, Utility.Random( 1, 2 ) );
			}
		}

		public TheCursedMage(Serial serial)
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