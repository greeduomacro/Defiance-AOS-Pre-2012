using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName("a cursed steeds corpse")]
	public class CursedSteed : BaseMount
	{
		public override bool AlwaysMurderer { get { return true; } }

		public override bool HasBreath { get { return true; } }
		public override int BreathFireDamage { get { return 100; } }
		public override int BreathEffectItemID { get { return 0x36D4; } }
		public override int BreathEffectHue { get { return 0x484; } }
		public override int BreathEffectSound { get { return 0x118; } }

		public override string DefaultName{ get{ return "a cursed steed"; } }

		[Constructable]
		public CursedSteed() : base(0x74, 0x3EAF, AIType.AI_Mage, FightMode.Closest, 10, 1, 0.15, 0.4)
		{
			BaseSoundID = 0xA8;
			Body = 0x78;

			SetStr(396, 425);
			SetDex(86, 105);
			SetInt(86, 125);

			SetHits(398, 415);

			SetDamage(16, 22);

			SetDamageType(ResistanceType.Physical, 40);
			SetDamageType(ResistanceType.Fire, 80);
			SetDamageType(ResistanceType.Energy, 20);

			SetResistance(ResistanceType.Physical, 55, 65);
			SetResistance(ResistanceType.Fire, 30, 40);
			SetResistance(ResistanceType.Cold, 30, 40);
			SetResistance(ResistanceType.Poison, 30, 40);
			SetResistance(ResistanceType.Energy, 20, 30);

			SetSkill(SkillName.EvalInt, 10.4, 50.0);
			SetSkill(SkillName.Magery, 10.4, 50.0);
			SetSkill(SkillName.MagicResist, 85.3, 100.0);
			SetSkill(SkillName.Tactics, 97.6, 100.0);
			SetSkill(SkillName.Wrestling, 80.5, 92.5);
			SetSkill(SkillName.Anatomy, 80.0, 100.0);

			Fame = NotorietyHandlers.GetNotorietyByLevel( 3 );
			Karma = NotorietyHandlers.GetNotorietyByLevel( -3 );

			VirtualArmor = 60;
			Tamable = false;
		}

		public override void BreathDealDamage(Mobile target)
		{
			base.BreathDealDamage(target);

			Effects.PlaySound(target.Location, target.Map, 0x1CA);
			//new CustomPool("cursed blood", 0x485, 30, 35, 0, 100, 0, 0, 0).MoveToWorld( target.Location, target.Map );
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Rich);
			AddLoot(LootPack.LowScrolls);
			AddLoot(LootPack.Potions);
		}

		public override int GetAngerSound()
		{
			return 0x16A;
		}

		public override int Meat { get { return 5; } }
		public override int Hides { get { return 10; } }
		public override HideType HideType { get { return HideType.Barbed; } }
		public override FoodType FavoriteFood { get { return FoodType.Meat; } }

		public CursedSteed(Serial serial) : base(serial)
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