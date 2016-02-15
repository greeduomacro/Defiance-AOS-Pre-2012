using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "an ant lion corpse" )]
	public class AntLion : BaseSolenAnt
	{
		public override bool HasBreath{ get{ return true; } }

		public static TimeSpan m_tsDigDelay = TimeSpan.FromSeconds( 12.0 );
		private static TimeSpan tsCrack = TimeSpan.FromSeconds( 40.0 );
		private static TimeSpan tsHole = TimeSpan.FromSeconds( 20.0 );

		private DateTime m_TimeToDig;
		private Timer m_tDigTimer;
		private Mobile m_Combatant;

		private bool m_bIsDigging;
		public Point3D m_pStartingLoc;
		public Map m_pStartingMap;

		[Constructable]
		public AntLion() : base()
		{
			Name = "an ant lion";
			Body = 787;
			BaseSoundID = 1006;

			SetStr( 296, 320 );
			SetDex( 81, 105 );
			SetInt( 36, 60 );

			SetHits( 151, 162 );

			SetDamage( 7, 21 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Poison, 30 );

			SetResistance( ResistanceType.Physical, 45, 60 );
			SetResistance( ResistanceType.Fire, 25, 35 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 30, 35 );

			SetSkill( SkillName.MagicResist, 70.0 );
			SetSkill( SkillName.Tactics, 90.0 );
			SetSkill( SkillName.Wrestling, 90.0 );

			Fame = 4500;
			Karma = -4500;

			VirtualArmor = 45;

			PackGem();
			PackGem();

			PackItem( new Bone( 3 ) );

			PackItem( new FertileDirt( Utility.RandomMinMax( 1, 5 ) ) );

			if ( Core.ML && Utility.RandomDouble() < .33 )
				PackItem( Engines.Plants.Seed.RandomPeculiarSeed(2) );

			Item orepile = null; /* no trust, no love :( */

			switch (Utility.Random(4))
			{
				case 0: orepile = new DullCopperOre(); break;
				case 1: orepile = new ShadowIronOre(); break;
				case 2: orepile = new CopperOre(); break;
				default: orepile = new BronzeOre(); break;
			}
			orepile.Amount = Utility.RandomMinMax(1, 10);
			orepile.ItemID = 0x19B9;
			PackItem(orepile);

			PackItem( new FertileDirt( 5 ) );

			BoneRemains.PackSmallBonesAndLargeBones( Backpack, Utility.Random( 1, 2 ) );

			SetDigDelay();
		}

		public override void OnAfterDelete()
		{
			if( m_tDigTimer != null )
				m_tDigTimer.Stop();

			base.OnAfterDelete();
		}

		public void SetDigDelay()
		{
			m_TimeToDig = DateTime.Now + m_tsDigDelay;
		}

		private bool CanDig()
		{
			if( DateTime.Now > m_TimeToDig && !m_bIsDigging )
				return true;
			return false;
		}

		public void StopDigging()
		{
			SetDigDelay();
			m_bIsDigging = false;
			CantWalk = false;

			if( m_tDigTimer != null )
				m_tDigTimer.Stop();
		}

		public void StartDigging()
		{
			m_bIsDigging = true;
			CantWalk = true;

			m_tDigTimer = new DigTimer( this, TimeSpan.FromSeconds( 4 ) );
			m_tDigTimer.Start();
		}

		private void CompleteDigging()
		{
			ShowAnt();
			StopDigging();
		}

		public void MoveToStartingLoc()
		{
			MoveToWorld( m_pStartingLoc, m_pStartingMap );
		}

		public override void OnActionWander()
		{
			base.OnActionWander();

			if( CanDig() )
			{
				PublicOverheadMessage( MessageType.Spell, 0, true, "* The ant lion begins tunneling into the ground *" );
				PlaySound( 0x21E );
				FixedEffect( 0x3728, 10, 10, 0x96E, 0x0 );

				StartDigging();
			}
		}

		private void CreateHole( Point3D location, Map map )
		{
			new SelfDeletingItem( "a hole", 1, 0x3EE8, tsHole ).MoveToWorld( location, map );
		}

		private void CreateCrack( Point3D location, Map map )
		{
			new SelfDeletingItem( "a crack", 0, Utility.Random( 6913, 6 ), tsCrack ).MoveToWorld( location, map );
		}

		public void HideAnt()
		{
			m_pStartingLoc = this.Location;
			m_pStartingMap = this.Map;

			Hidden = true;

			m_Combatant = Combatant;
			Combatant = null;

			Point3D p = Location;
			p.Z -= 40;

			this.MoveToWorld( p, this.Map );
		}

		public void ShowAnt()
		{
			Hidden = false;

			// Make sure that the attacker is near the site
			if ( m_Combatant != null && !m_Combatant.Deleted && m_Combatant.Alive && m_Combatant.Map == this.m_pStartingMap && m_Combatant.InRange( m_pStartingLoc, 12 ) )
			{
				this.MoveToWorld( m_Combatant.Location, m_Combatant.Map );

				m_Combatant.DoHarmful( this );
				AOS.Damage( m_Combatant, this, (int)Utility.Random(30,40), 70, 0, 0, 30, 0 );

				Combatant = m_Combatant;

				this.CreateCrack( this.Location, this.Map );
				this.CreateHole( this.Location, this.Map);
			}
			else
				MoveToStartingLoc();

			PlaySound( 0x221 );
			FixedEffect( 0x3728, 10, 10, 0x96E, 0x0 );
		}

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			base.OnDamage( amount, from, willKill );

			if( !Hidden && m_bIsDigging )
			{
				PublicOverheadMessage( MessageType.Spell, 0, true, "* You interrupt the ant lion's digging! *" );
				StopDigging();
			}

			if( willKill && 0.025 > Utility.RandomDouble() )
				PackItem( new SkeletalRemains() );
		}

		public override bool OnBeforeDeath()
		{
			if( Hidden )
				MoveToStartingLoc();
			return base.OnBeforeDeath();
		}

		public override int GetAngerSound(){ return 0x5A; }
		public override int GetIdleSound(){ return 0x5A; }
		public override int GetAttackSound(){ return 0x164; }
		public override int GetHurtSound(){ return 0x187; }
		public override int GetDeathSound(){ return 0x1BA; }

		public AntLion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

			writer.Write( (bool) m_bIsDigging );
			writer.Write( (Point3D) m_pStartingLoc );
			writer.Write( (Map) m_pStartingMap );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			m_bIsDigging = reader.ReadBool();
			m_pStartingLoc = reader.ReadPoint3D();
			m_pStartingMap = reader.ReadMap();

			if( this.Hidden )
				CompleteDigging();
			else
				SetDigDelay();
		}

		private class DigTimer : Timer
		{
			private AntLion m_Ant;
			private int m_iPos;

			public DigTimer( AntLion antlion, TimeSpan startdelay ) : base( startdelay,TimeSpan.FromSeconds( 3 ) )
			{
				Priority = TimerPriority.FiftyMS;
				m_Ant = antlion;
				m_iPos = 0;
			}

			protected override void OnTick()
			{
				if( m_iPos == 0 )
				{
					m_Ant.CreateCrack( m_Ant.Location, m_Ant.Map );
					m_Ant.CreateHole( m_Ant.Location, m_Ant.Map );

					m_Ant.HideAnt();
				}
				else if( m_iPos == 1 )
					m_Ant.Heal( 50 );
				else if( m_iPos >= 5 )
				{
					m_Ant.CompleteDigging();
					this.Stop();
				}

				m_iPos++;
			}
		}
	}
}