using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Regions;

namespace Server.Mobiles
{
	public abstract class BaseCursedCreature : BaseCreature
	{
		private DateTime m_NextBandageTime;
		public virtual DateTime NextBandageTime { get { return m_NextBandageTime; } }

		private DateTime m_NextRearmTime;
		public virtual DateTime NextRearmTime { get { return m_NextRearmTime; } }

		private bool m_bTryToRearm;

		public virtual bool UseRearm { get { return false; } }
		public virtual int RearmMinDelay { get { return 15; } }
		public virtual int RearmMaxDelay { get { return 20; } }

		public virtual bool UseBandages { get { return true; } }
		public virtual int BandagesMinDelay { get { return 15; } }
		public virtual int BandagesMaxDelay { get { return 20; } }

		public override bool ShowFameTitle { get { return false; } }

		#region WeaponAbility
		public override double WeaponAbilityChance { get { return 0.3; } }
		public override WeaponAbility GetWeaponAbility()
		{
			if (Weapon is BaseWeapon)
			{
				BaseWeapon wep = (BaseWeapon)Weapon;

				if (Utility.RandomBool())
					return wep.PrimaryAbility;
				else
					return wep.SecondaryAbility;
			}

			return base.GetWeaponAbility();
		}
		#endregion

		public BaseCursedCreature(AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed)
			: base(ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed)
		{
		}

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			base.OnDamage(amount, from, willKill);

			if (0.01 > Utility.RandomDouble() && !willKill && from is PlayerMobile)
			{
				if (from.Weapon is LongswordOfJustice)
				{
					Say("He has got the Sword Of Justice!");
					PlaySound(432);
				}
				else if (from.Weapon is HarvesterOfTheGhost)
				{
					Say("He has got the Harvester Of The Ghost!");
					PlaySound(432);
				}
				else if (from.Weapon is BowOfHephaestus)
				{
					Say("He has got the Bow Of Hephaestus!");
					PlaySound(432);
				}
			}
		}

		public override void OnThink()
		{
			base.OnThink();

			#region TryToRearm
			if (m_bTryToRearm && m_NextRearmTime < DateTime.Now)
			{
				bool needArcherWep = (this.AI == AIType.AI_Archer) ? true : false;

				bool EquippedWeapon = false;
				for (int i = 0; i < this.Backpack.Items.Count; ++i)
				{
					if (!(this.Backpack.Items[i] is BaseWeapon))
						continue;

					BaseWeapon wep = (BaseWeapon)this.Backpack.Items[i];

					if (needArcherWep && wep.Skill != SkillName.Archery)
						continue;

					if (this.Skills[wep.Skill].Base > 0)
					{
						if (this.EquipItem(wep))
						{
							EquippedWeapon = true;
							break;
						}
					}
				}

				// We could not find a weapon right now, we will try again later.
				if (!EquippedWeapon)
					m_NextRearmTime = DateTime.Now + TimeSpan.FromHours(1.0);
				else
					m_bTryToRearm = false;
			}
			else if (UseRearm && !m_bTryToRearm && (Weapon == null || Weapon is Fists))
			{
				m_bTryToRearm = true;
				m_NextRearmTime = DateTime.Now + TimeSpan.FromSeconds(Utility.RandomMinMax(RearmMinDelay, RearmMaxDelay));
			}
			#endregion

			#region UseBandages
			if (UseBandages && m_NextBandageTime < DateTime.Now && Hits < (HitsMax - 10))
			{
				Bandage bandage = (Bandage)this.Backpack.FindItemByType(typeof(Bandage));
				if (bandage != null)
				{
					if (BandageContext.BeginHeal(this, this) != null)
						bandage.Consume();

					m_NextBandageTime = DateTime.Now + TimeSpan.FromSeconds(Utility.RandomMinMax(BandagesMinDelay, BandagesMaxDelay));
				}
				else // Try again later.
					m_NextBandageTime = DateTime.Now + TimeSpan.FromHours(1.0);
			}
			#endregion
		}

		public override void GenerateLoot()
		{
			PackItem(new Bandage(Utility.RandomMinMax(10, 30)));
		}

		public BaseCursedCreature(Serial serial)
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