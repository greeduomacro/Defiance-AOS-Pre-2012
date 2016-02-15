using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName("a cursed skeleton corpse")]
	public class CursedSkeleton : BaseCreature
	{
		[Constructable]
		public CursedSkeleton()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "a cursed skeleton";
			Body = 147;
			BaseSoundID = 451;

			Hue = 2130;
			SetStr(196, 250);
			SetDex(76, 95);
			SetInt(36, 60);

			SetHits(118, 150);

			SetDamage(8, 18);

			SetDamageType(ResistanceType.Physical, 40);
			SetDamageType(ResistanceType.Cold, 60);

			SetResistance(ResistanceType.Physical, 35, 45);
			SetResistance(ResistanceType.Fire, 20, 30);
			SetResistance(ResistanceType.Cold, 50, 60);
			SetResistance(ResistanceType.Poison, 20, 30);
			SetResistance(ResistanceType.Energy, 30, 40);

			SetSkill(SkillName.MagicResist, 65.1, 80.0);
			SetSkill(SkillName.Tactics, 85.1, 100.0);
			SetSkill(SkillName.Wrestling, 85.1, 95.0);
			SetSkill(SkillName.Anatomy, 85.0, 100.0);

			Fame = NotorietyHandlers.GetNotorietyByLevel( 3 );
			Karma = NotorietyHandlers.GetNotorietyByLevel( -3 );

			VirtualArmor = 40;
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.AosFilthyRich);

			if (!m_Spawning)
			{
				BoneRemains.PackSkullsAndSmallBones( Backpack, Utility.Random( 1, 2 ) );
				AddItem(new Bandage(Utility.RandomMinMax(10, 20)));
			}
		}

		public CursedSkeleton(Serial serial)
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