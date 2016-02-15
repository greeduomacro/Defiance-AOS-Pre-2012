using System;
using Server;
using Server.Items;
using Server.Gumps;

namespace Server.Mobiles
{
	[TypeAlias("Server.Mobiles.PayedFullHealer")]
	public class PaidFullHealer : BaseHealer
	{
		private int m_Price;

		[CommandProperty(AccessLevel.GameMaster)]
		public int Price { get { return m_Price; } set { m_Price = value; } }

		[Constructable]
		public PaidFullHealer()
		{
			m_Price = 1000;
			Title = "the Priest Of Evil";
			Karma = -10000;
			Blessed = true;

			SetDamageType(ResistanceType.Cold, 100);

			SetResistance(ResistanceType.Physical, 40, 80);
			SetResistance(ResistanceType.Fire, 40, 80);
			SetResistance(ResistanceType.Cold, 40, 80);
			SetResistance(ResistanceType.Poison, 40, 80);
			SetResistance(ResistanceType.Energy, 40, 80);

			SetSkill(SkillName.Wrestling, 100.0, 140.5);
			SetSkill(SkillName.Anatomy, 75.0, 97.5);
			SetSkill(SkillName.EvalInt, 100.0, 140.0);
			SetSkill(SkillName.Healing, 75.0, 97.5);
			SetSkill(SkillName.Magery, 100.0, 140.0);
			SetSkill(SkillName.MagicResist, 100.0, 140.0);
			SetSkill(SkillName.Tactics, 82.0, 100.0);
		}

		public override bool CanTeach { get { return false; } }
		//public override bool AlwaysMurderer { get { return true; } }

		public override void InitOutfit()
		{
			HoodedShroudOfShadows shroud = new HoodedShroudOfShadows();
			shroud.LootType = LootType.Blessed;
			AddItem(shroud);

			GoldRing ring = new GoldRing();
			ring.Attributes.CastSpeed = 2;
			ring.Attributes.CastRecovery = 6;
			ring.LootType = LootType.Blessed;
			AddItem(ring);
		}

		public override void OfferResurrection(Mobile m)
		{
			Direction = GetDirectionTo(m);

			m.PlaySound(0x214);
			m.FixedEffect(0x376A, 10, 16);

			m.CloseGump(typeof(PaidFullHealerGump));
			m.SendGump(new PaidFullHealerGump(m_Price, this));
		}

		public PaidFullHealer(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
			writer.Write(m_Price);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			m_Price = reader.ReadInt();
		}
	}
}