using System;
using Server.Misc;
using Server.Network;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	public class TheCursedWarrior : BaseCursedCreature
	{
		public override bool UseRearm { get { return true; } }
		public override InhumanSpeech SpeechType { get { return CursedCaveSpeech.Cursed; } }
		private static int m_iArmorHue = 1441;
		public override bool AlwaysMurderer { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Deadly; } }

		[Constructable]
		public TheCursedWarrior()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Body = 0x190;
			Name = NameList.RandomName("male");
			Title = "the cursed warrior";
			Hue = 2130;

			SetStr(350);
			SetDex(150);
			SetInt(100);

			SetResistance(ResistanceType.Physical, 40, 60);
			SetResistance(ResistanceType.Fire, 40, 50);
			SetResistance(ResistanceType.Cold, 50, 60);
			SetResistance(ResistanceType.Poison, 55, 65);
			SetResistance(ResistanceType.Energy, 40, 50);

			SetSkill(SkillName.Wrestling, 74.0, 80.0);
			SetSkill(SkillName.Swords, 90.0, 95.0);
			SetSkill(SkillName.Anatomy, 120.0, 125.0);
			SetSkill(SkillName.MagicResist, 90.0, 94.0);
			SetSkill(SkillName.Tactics, 90.0, 95.0);
			SetSkill(SkillName.Healing, 90.0);

			Fame = NotorietyHandlers.GetNotorietyByLevel( 2 );
			Karma = NotorietyHandlers.GetNotorietyByLevel( -2 );

			VirtualArmor = 60;

			BoneArms arms = new BoneArms();
			arms.Hue = m_iArmorHue;
			AddItem(CursedCaveUtility.MutateItem(arms, 10));

			BoneGloves gloves = new BoneGloves();
			gloves.Hue = m_iArmorHue;
			AddItem(CursedCaveUtility.MutateItem(gloves, 10));

			BoneChest tunic = new BoneChest();
			tunic.Hue = m_iArmorHue;
			AddItem(CursedCaveUtility.MutateItem(tunic, 10));

			BoneLegs legs = new BoneLegs();
			legs.Hue = m_iArmorHue;
			AddItem(CursedCaveUtility.MutateItem(legs, 10));

			BoneHelm helm = new BoneHelm();
			helm.Hue = m_iArmorHue;
			AddItem(CursedCaveUtility.MutateItem(helm, 10));

			AddItem(new Shoes());
			AddItem(CursedCaveUtility.MutateItem(new HeaterShield(), 10));

			VikingSword weapon = new VikingSword();
			AddItem(CursedCaveUtility.MutateItem(weapon, 10));
		}

		public TheCursedWarrior(Serial serial)
			: base(serial)
		{
		}

		public override bool OnBeforeDeath()
		{
			CursedSkeleton rm = new CursedSkeleton();

			if (this.Combatant != null)
				rm.Combatant = this.Combatant;

			rm.Team = this.Team;
			rm.MoveToWorld(this.Location, this.Map);

			Effects.SendLocationEffect(Location, Map, 0x3709, 13, 0x3B2, 0);

			this.Delete();

			return false;
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