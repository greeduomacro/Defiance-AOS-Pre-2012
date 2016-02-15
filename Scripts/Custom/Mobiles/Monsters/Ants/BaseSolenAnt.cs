using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	public enum AntEggType
	{
		RedEgg,
		BlackEgg
	}

	public class BaseSolenAnt : BaseCreature
	{
		private bool m_BurstSac;
		public bool BurstSac { get { return m_BurstSac; } }

		public virtual bool UseAcidSack{ get{ return false; } }
		public virtual double UseAcidSackChance { get { return 0.4; } }

		public virtual bool UseEggs{ get{ return false; } }
		public virtual AntEggType EggType{ get{ return AntEggType.RedEgg; } }
		public virtual double UseEggsChance{ get{ return 0.4; } }

		#region Acid Spit
		public override int BreathPoisonDamage { get { return 100; } }
		public override int BreathEffectItemID { get { return 0x36D4; } }
		public override int BreathEffectHue { get { return 0x3F; } }
		public override int BreathEffectSound { get { return 0x118; } }

		public virtual bool AcidPoolOnBreath { get { return false; } }

		public override void BreathDealDamage(Mobile target)
		{
			base.BreathDealDamage(target);
			Effects.PlaySound(target.Location, target.Map, 0x1CA);

			if (AcidPoolOnBreath)
				new BasePool("acid", 0x3F, 10, 15, 0, 0, 0, 100, 0).MoveToWorld(target.Location, target.Map);
		}

		public override void BreathStart(Mobile target)
		{
			if (!BurstSac)
				base.BreathStart(target);
		}
		#endregion

		public BaseSolenAnt() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
		}

		public BaseSolenAnt( Serial serial ) : base( serial )
		{
		}

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			SolenHelper.OnBlackDamage(from);

			if (!willKill && UseAcidSack)
			{
				if (!BurstSac )
				{
					if (Hits < 50)
					{
						PublicOverheadMessage(MessageType.Regular, 0x3B2, true, "* The solen's acid sac has burst open! *");
						m_BurstSac = true;
					}
				}
				else if (from != null && from != this && InRange(from, 1) )
				{
					SpillAcid( from, 1 );
				}
			}

			if (UseEggs && UseEggsChance > Utility.RandomDouble())
			{
				new SolenEggSack(EggType).MoveToWorld(Location, Map);
			}

			base.OnDamage(amount, from, willKill);
		}

		public override bool OnBeforeDeath()
		{
			if( UseAcidSack )
				SpillAcid( Utility.RandomMinMax( 1, 4 ) );

			return base.OnBeforeDeath();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

			writer.Write((bool)m_BurstSac);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			m_BurstSac = reader.ReadBool();
		}
	}
}