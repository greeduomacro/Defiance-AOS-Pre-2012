using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Regions
{
	public class CursedCaveRegion : Region
	{
		public static void Initialize()
		{
			new CursedCaveRegion("Cursed Cave", new Rectangle2D(6026, 266, 107, 108)).Register();
			new CursedCaveRegion("Cursed Cave Level 3", new Rectangle2D(5124, 1412, 246, 101)).Register();
		}

		public CursedCaveRegion(string name, Rectangle2D area)
			: base(name, Map.Felucca, Region.DefaultPriority + 1, area)
		{
		}

		public override bool AllowHousing(Mobile from, Point3D p)
		{
			return false;
		}

		public override void OnEnter(Mobile m)
		{
			if (m.Target != null)
			{
				Targeting.Target.Cancel(m);
				FizzleStrangely(m);
			}
		}

		private static string m_sCantUseSkillMsg = "You can't seem to use that skill here.";

		public override bool OnSkillUse(Mobile m, int Skill)
		{
			switch (Skill)
			{
				case (int)SkillName.Peacemaking:
					m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, m_sCantUseSkillMsg, m.NetState);
					return false;
				case (int)SkillName.Provocation:
					m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, m_sCantUseSkillMsg, m.NetState);
					return false;
				case (int)SkillName.Discordance:
					m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, m_sCantUseSkillMsg, m.NetState);
					return false;
			}
			return true;
		}

		public override bool OnBeginSpellCast(Mobile m, ISpell s)
		{
			if (m.Player && m.AccessLevel < AccessLevel.GameMaster)
			{
				FizzleStrangely(m);
				return false;
			}
			return base.OnBeginSpellCast(m, s);
		}

		public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
		{
			global = LightCycle.DungeonLevel;
		}

		public void FizzleStrangely(Mobile m)
		{
			m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "The spell fizzles strangely.", m.NetState);
			m.FixedParticles(0x3779, 1, 46, 9502, 5, 3, EffectLayer.Waist);
			m.FixedEffect(0x3735, 6, 30);
			m.PlaySound(0x5C);
		}
	}
}