using System;


namespace Server.Events.Duel
{
	public class DuelType
	{
		public virtual int TypeNumber { get { return 0; } }
		public static DuelType[] TypeArray = new DuelType[]
		{
			new NDDuelType(), //0
			new NMFDuelType(), // 1
			new TMFDuelType(), // 2
			new UMFDuelType(), // 3
			new NDFDuelType(), // 4
			new TDFDuelType(), // 5
			new UDFDuelType(), // 6
			new DDDuelType() // 7
		};


		public virtual bool PassedSkillCheck(Mobile m) { return true; }
		public virtual void SetOptions(DuelRune rune) { }

		protected void SetTrueOptions(DuelRune rune)
		{
			for (int i = 0; i < 4; i++)
				rune.Options[i] = true;
		}

		protected void SetUltimateOptions(DuelRune rune)
		{
			for (int i = 0; i < 8; i++)
				rune.Options[i] = true;
		}
	}

	public class NDDuelType : DuelType
	{
		public override int TypeNumber { get { return 0; } }

		public override bool PassedSkillCheck(Mobile m)
		{
			return true;
		}
	}

	public class NMFDuelType : DuelType
	{
		public override int TypeNumber { get { return 1; } }
		public override bool PassedSkillCheck(Mobile m)
		{
			if (m.Skills[SkillName.Magery].Value > 80)
				return true;

			return false;
		}
	}

	public class TMFDuelType : NMFDuelType
	{
		public override int TypeNumber { get { return 2; } }
		public override void SetOptions(DuelRune rune) { SetTrueOptions(rune); }

	}

	public class UMFDuelType : TMFDuelType
	{
		public override int TypeNumber { get { return 3; } }
		public override void SetOptions(DuelRune rune) { SetUltimateOptions(rune); }
	}

	public class NDFDuelType : DuelType
	{
		public override int TypeNumber { get { return 4; } }
		public override bool PassedSkillCheck(Mobile m)
		{
			int check = 0;
			if (m.Skills[SkillName.Swords].Base > 80 || m.Skills[SkillName.Macing].Base > 80 || m.Skills[SkillName.Fencing].Base > 80 || m.Skills[SkillName.Archery].Base > 80)
				check = 10;
			if (m.Skills[SkillName.Anatomy].Base > 80)
				check++;
			if (m.Skills[SkillName.Tactics].Base > 80)
				check++;
			if (m.Skills[SkillName.Healing].Base > 80)
				check++;
			if (m.Skills[SkillName.Parry].Base > 80)
				check++;

			if (check > 11 && m.Dex >= 80)
				return true;

			return false;
		}
	}

	public class TDFDuelType : NDFDuelType
	{
		public override int TypeNumber { get { return 5; } }
		public override void SetOptions(DuelRune rune) { SetTrueOptions(rune); }

	}

	public class UDFDuelType : TDFDuelType
	{
		public override int TypeNumber { get { return 6; } }
		public override void SetOptions(DuelRune rune) { SetUltimateOptions(rune); }

	}

	public class DDDuelType : DuelType
	{
		public override int TypeNumber { get { return 7; } }

	}
}