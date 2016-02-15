using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "an orc scout corpse" )]
	public class OrcScout : BaseCreature
	{
		private bool m_Bandage;
		private Timer m_SoundTimer;
		private bool m_HasTeleportedAway;

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.MortalStrike;
		}

		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
		public OrcScout() : base( AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an orc scout";
			Body = 0xB5;
			BaseSoundID = 0x45A;

			SetStr( 96, 120 );
			SetDex( 101, 130 );
			SetInt( 36, 60 );

			SetHits( 101, 130 );

			SetDamage( 28, 31 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 50.1, 75.0 );
			SetSkill( SkillName.Tactics, 55.1, 80.0 );
			SetSkill( SkillName.Veterinary, 50.1, 70.0 );
			SetSkill( SkillName.Hiding, 500.0 );
			SetSkill( SkillName.Stealth, 500.0 );

			SetFameLevel( 2 );
			SetKarmaLevel( 2 );

			VirtualArmor = 98;

			if( Utility.RandomDouble() < 0.2 )
				AddItem( new OrcishBow() );
			else
				AddItem( new Bow() );

			PackItem( new Arrow( Utility.RandomMinMax( 60, 70 ) ) );
		}

		public void GenLoot(Container c)
		{
			c.DropItem( new Bandage(Utility.RandomMinMax(1,15)) );
			c.DropItem( new Apple(Utility.RandomMinMax(3,5)) );
		}

		public override void OnDeath(Container c)
		{
			GenLoot(c);
			base.OnDeath(c);
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 1; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.SavagesAndOrcs; }
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && m.FindItemOnLayer( Layer.Helm ) is OrcishKinMask )
				return false;

			return base.IsEnemy( m );
		}

		public override void AggressiveAction( Mobile aggressor, bool criminal )
		{
			base.AggressiveAction( aggressor, criminal );

			Item item = aggressor.FindItemOnLayer( Layer.Helm );

			if ( item is OrcishKinMask )
			{
				AOS.Damage( aggressor, 50, 0, 100, 0, 0, 0 );
				item.Delete();
				aggressor.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
				aggressor.PlaySound( 0x307 );
			}
		}

		public override void OnCombatantChange()
		{
			base.OnCombatantChange();

			if ( Hidden && Combatant != null )
				Combatant = null;
		}

		public virtual void SendTrackingSound()
		{
			if ( Hidden )
			{
				Effects.PlaySound( this.Location, this.Map, 0x12C );
				Combatant = null;
			}
			else
			{
				Frozen = false;

				if ( m_SoundTimer != null )
					m_SoundTimer.Stop();

				m_SoundTimer = null;
			}
		}

		public override void OnThink()
		{
			// Reveal Players
			foreach ( Mobile mob in this.GetMobilesInRange( 5 ) )
			{
				if ( mob is PlayerMobile && mob.AccessLevel == AccessLevel.Player )
				{
					mob.Hidden = false;
				}
			}

			if ( !m_HasTeleportedAway && Hits < (HitsMax / 2) && !Poisoned )
			{
				Map map = this.Map;

				if ( map != null )
				{
					for ( int i = 0; i < 10; ++i )
					{
						int x = X + (Utility.RandomMinMax( 5, 10 ) * (Utility.RandomBool() ? 1 : -1));
						int y = Y + (Utility.RandomMinMax( 5, 10 ) * (Utility.RandomBool() ? 1 : -1));
						int z = Z;

						if ( !map.CanFit( x, y, z, 16, false, false ) )
							continue;

						Point3D from = this.Location;
						Point3D to = new Point3D( x, y, z );

						this.Location = to;
						this.ProcessDelta();
						this.Hidden = true;
						this.Combatant = null;

						Effects.SendLocationParticles( EffectItem.Create( from, map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
						Effects.SendLocationParticles( EffectItem.Create(   to, map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );

						Effects.PlaySound( to, map, 0x1FE );

						m_HasTeleportedAway = true;
						m_SoundTimer = Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), TimeSpan.FromSeconds( 2.5 ), new TimerCallback( SendTrackingSound ) );

						this.UseSkill( SkillName.Stealth );
						AIObject.Action = ActionType.Flee;

						break;
					}
				}
			}

			if ( Hits < ( HitsMax - 10) && m_Bandage == false )
				TryToHeal(this);

			base.OnThink();
		}

		public int GetRange( PlayerMobile pm )
		{
			return 4;
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( m.Alive && m is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m;
				int range = GetRange( pm );

				if ( range >= 0 && InRange( m, range ) && !InRange( oldLocation, range ) && this.Hits == this.HitsMax && this.Hidden == true && IsEnemy( m ) )
				{
					this.Frozen = false;
					this.Hidden = false;
					this.Combatant = m;
				}
			}
		}

		public override void OnDamage( int amount, Mobile m, bool willKill )
		{
			if ( Hits < ( HitsMax - 10) && m_Bandage == false && Hidden )
				TryToHeal(this);
		}

		private void TryToHeal(OrcScout scout)
		{
			scout.m_Bandage = true;
			Bandage bandage = (Bandage) scout.Backpack.FindItemByType( typeof( Bandage ) );

			if ( bandage != null )
			{
				if ( BandageContext.BeginHeal( (Mobile)scout, (Mobile)scout ) != null )
					bandage.Consume();

				Timer.DelayCall( TimeSpan.FromSeconds( 15 ), new TimerCallback( EnableBanding ) );
			}
		}

		private void EnableBanding()
		{
			this.m_Bandage = false;
		}

		public OrcScout( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}