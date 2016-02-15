using System;
using System.Collections;
using System.Collections.Generic;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Necromancy;
using Server.Misc;
using Server.Regions;
using Server.SkillHandlers;

namespace Server.Mobiles
{
	public class NecroAI : BaseAI
	{
		private DateTime m_NextCastTime;
		private DateTime m_NextHealTime;

		public NecroAI( BaseCreature m ) : base( m )
		{
		}

		public override bool Think()
		{
			if ( m_Mobile.Deleted )
				return false;

			if ( ProcessTarget() )
				return true;
			else
				return base.Think();
		}

		public virtual bool SmartAI
		{
			get{ return ( m_Mobile is BaseVendor || m_Mobile is BaseEscortable ); }
		}

		private const double SpiritSpeakChance = 0.10; // 10% chance to use Spirit Speak at gm necromancy

		public virtual double ScaleByNecromancy( double v )
		{
			return m_Mobile.Skills[SkillName.Necromancy].Value * v * 0.01;
		}

		public override bool DoActionWander()
		{
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
				m_NextCastTime = DateTime.Now;
			}
			else if ( SmartAI && m_Mobile.Mana < m_Mobile.ManaMax )
			{
				m_Mobile.DebugSay( "I am going to meditate" );

				m_Mobile.UseSkill( SkillName.Meditation );
			}
			else
			{
				m_Mobile.DebugSay( "I am wandering" );

				m_Mobile.Warmode = false;

				base.DoActionWander();

				if ( !m_Mobile.Controlled )
				{
					Spell spell = CheckUseSpiritSpeak();

					if ( spell != null )
						spell.Cast();
				}
			}

			return true;
		}

		private Spell CheckUseSpiritSpeak()
		{
			// Summoned creatures never heal themselves.
			if ( m_Mobile.Summoned )
				return null;

			if ( m_Mobile.Controlled )
			{
				if ( DateTime.Now < m_NextHealTime )
					return null;
			}

			if ( !SmartAI )
			{
				if ( ScaleByNecromancy( SpiritSpeakChance ) < Utility.RandomDouble() )
					return null;
			}
			else
			{
				if ( Utility.Random( 0, 4 + (m_Mobile.Hits == 0 ? m_Mobile.HitsMax : (m_Mobile.HitsMax / m_Mobile.Hits)) ) < 3 )
					return null;
			}

			Spell spell = null;

			if (m_Mobile.Hits < (m_Mobile.HitsMax - 10))
			{
				m_Mobile.DebugSay("I am going to use Spirit Speak");

				m_Mobile.UseSkill(SkillName.SpiritSpeak);
			}

			double delay;

			if ( m_Mobile.Int >= 500 )
				delay = Utility.RandomMinMax( 7, 10 );
			else
				delay = Math.Sqrt( 600 - m_Mobile.Int );

			m_NextHealTime = DateTime.Now + TimeSpan.FromSeconds( delay );

			return spell;
		}

		public void RunTo( Mobile m )
		{
			if ( !SmartAI )
			{
				if ( !MoveTo( m, true, m_Mobile.RangeFight ) )
					OnFailedMove();

				return;
			}

			if ( m.Paralyzed || m.Frozen )
			{
				if ( m_Mobile.InRange( m, 1 ) )
					RunFrom( m );
				else if ( !m_Mobile.InRange( m, m_Mobile.RangeFight > 2 ? m_Mobile.RangeFight : 2 ) && !MoveTo( m, true, 1 ) )
					OnFailedMove();
			}
			else
			{
				if ( !m_Mobile.InRange( m, m_Mobile.RangeFight ) )
				{
					if ( !MoveTo( m, true, 1 ) )
						OnFailedMove();
				}
				else if ( m_Mobile.InRange( m, m_Mobile.RangeFight - 1 ) )
				{
					RunFrom( m );
				}
			}
		}

		public void RunFrom( Mobile m )
		{
			Run( (m_Mobile.GetDirectionTo( m ) - 4) & Direction.Mask );
		}

		public void OnFailedMove()
		{
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				m_Mobile.DebugSay( "I am stuck" );
			}
		}

		public void Run( Direction d )
		{
			if ( (m_Mobile.Spell != null && m_Mobile.Spell.IsCasting) || m_Mobile.Paralyzed || m_Mobile.Frozen || m_Mobile.DisallowAllMoves )
				return;

			m_Mobile.Direction = d | Direction.Running;

			if ( !DoMove( m_Mobile.Direction, true ) )
				OnFailedMove();
		}

		public virtual Spell GetRandomDamageSpell( Mobile c)
		{
			int maxCircle = (int)((m_Mobile.Skills[SkillName.Necromancy].Value + 20.0) / (100.0 / 7.0));

			if ( maxCircle < 1 )
				maxCircle = 1;

			switch ( Utility.Random( maxCircle*2 ) )
			{
				case  0: case  1: case  2: case  3:
					if (PainSpikeSpell.HasPainSpike( c ) && Utility.Random(3) > 0)
						goto case 10;
					else
						return new PainSpikeSpell( m_Mobile, null );
				case  4: case  5: case 6:
					if (BloodOathSpell.HasBloodOath(m_Mobile) || BloodOathSpell.HasBloodOath(c) || c.Combatant != m_Mobile)
						if (GetMobsInWitherRange(m_Mobile) > 2)
							return new WitherSpell( m_Mobile, null );
						else goto case 13;
					else
						return new BloodOathSpell( m_Mobile, null );
				case  7:
					if (MindRotSpell.HasMindRotScalar(c))
						if (GetMobsInWitherRange(m_Mobile) > 2)
							return new WitherSpell( m_Mobile, null );
						else goto case 13;
					else return new MindRotSpell( m_Mobile, null );
				case  8: case  9: case 10:
					if (StrangleSpell.HasStrangle( c ))
						if (PainSpikeSpell.HasPainSpike( c ))
							goto case 12;
						else goto case 3;
					else
						return new StrangleSpell( m_Mobile, null );
				case 11:
					if (GetMobsInWitherRange(m_Mobile) > 0)
						return new WitherSpell( m_Mobile, null );
					else goto case 9;
				case 12: return new VengefulSpiritSpell( m_Mobile, null );
				case 13:
					if ( c is BaseCreature && ((BaseCreature)c).Summoned && m_Mobile.CanBeHarmful( c, false ) && !((BaseCreature)c).IsAnimatedDead && m_Mobile.Skills[SkillName.Magery].Value >= 90)
						return new DispelSpell( m_Mobile, null );
					else goto default;
				default: return new PoisonStrikeSpell( m_Mobile, null );
			}
		}

		public int GetMobsInWitherRange(Mobile mob)
		{
			int witherTargets = 0;
			foreach (Mobile m in mob.GetMobilesInRange(4))
			{
				if (m == mob || !mob.CanBeHarmful(m) || !m.Alive || m.AccessLevel > AccessLevel.Player)
					continue;
				else if (mob is BaseCreature && !((BaseCreature)mob).Controlled && !((BaseCreature)mob).Summoned)
					if (m.Player || (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned)))
						witherTargets++;
				else if (m is BaseCreature && !((BaseCreature)m).Controlled && !((BaseCreature)m).Summoned)
					witherTargets++;
			}
			return witherTargets;
		}

		public virtual Spell GetRandomCurseSpell()
		{
			switch ( Utility.Random( 3 ) )
			{
				default:
				case 0: return new EvilOmenSpell(m_Mobile, null);
				case 1: return new MindRotSpell(m_Mobile, null);
				case 2: return new CorpseSkinSpell(m_Mobile, null);
			}
		}

		public virtual Spell ChooseSpell( Mobile c )
		{
			Spell spell = null;

			if ( !SmartAI )
			{
				spell = CheckUseSpiritSpeak();

				if ( spell != null )
					return spell;

				switch ( Utility.Random( 10 ) )
				{
					case 0: // Curse them.
					{
						m_Mobile.DebugSay( "Attempting to curse" );
						spell = GetRandomCurseSpell();
						break;
					}
					default: // Damage them.
					{
						m_Mobile.DebugSay( "Just doing damage" );
						spell = GetRandomDamageSpell(c);
						break;
					}
				}

				return spell;
			}

			spell = CheckUseSpiritSpeak();

			if ( spell != null )
				return spell;

			switch (Utility.Random(3))
			{
				default:
				case 0:
				case 1: // Deal some damage
					{
						spell = GetRandomDamageSpell(c);

						break;
					}
				case 2: // Set up a combo of attacks
					{
						if (m_Mobile.Mana < 30 && m_Mobile.Mana > 15)
						{
							if (c.Paralyzed && !c.Poisoned)
							{
								m_Mobile.DebugSay("I am going to meditate");

								m_Mobile.UseSkill(SkillName.Meditation);
							}

						}
						else if (m_Mobile.Mana > 30 && m_Mobile.Mana < 80)
						{
							if (Utility.Random(2) == 0 && !c.Paralyzed && !c.Frozen && !c.Poisoned)
							{
								m_Combo = 0;
								spell = new PainSpikeSpell(m_Mobile, null);
							}
							else
							{
								m_Combo = 1;
								spell = new MindRotSpell(m_Mobile, null);
							}

						}
						break;
					}
			}

			return spell;
		}

		protected int m_Combo = -1;

		public virtual Spell DoCombo(Mobile c)
		{
			Spell spell = null;

			if (m_Combo == 0)
			{
				spell = new PoisonStrikeSpell(m_Mobile, null);
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 1)
			{
				spell = new BloodOathSpell(m_Mobile, null);
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 2)
			{
				if (!c.Poisoned)
					spell = new MindRotSpell(m_Mobile, null);

				++m_Combo; // Move to next spell
			}

			if (m_Combo == 3 && spell == null)
			{
				switch (Utility.Random(3))
				{
					default:
					case 0:
						{
							if (c.Int < c.Dex)
								spell = new StrangleSpell(m_Mobile, null);
							else
								spell = new EvilOmenSpell(m_Mobile, null);

							++m_Combo; // Move to next spell

							break;
						}
					case 1:
						{
							spell = new PoisonStrikeSpell(m_Mobile, null);
							m_Combo = -1; // Reset combo state
							break;
						}
					case 2:
						{
							spell = new BloodOathSpell(m_Mobile, null);
							m_Combo = -1; // Reset combo state
							break;
						}
				}
			}
			else if (m_Combo == 4 && spell == null)
			{
				spell = new VengefulSpiritSpell(m_Mobile, null);
				m_Combo = -1;
			}

			return spell;
		}

		private TimeSpan GetDelay()
		{
			double del = ScaleByNecromancy( 3.0 );
			double min = 6.0 - (del * 0.75);
			double max = 6.0 - (del * 1.25);

			return TimeSpan.FromSeconds( min + ((max - min) * Utility.RandomDouble()) );
		}

		public override bool DoActionCombat()
		{
			Mobile c = m_Mobile.Combatant;
			m_Mobile.Warmode = true;

			if ( c == null || c.Deleted || !c.Alive || c.IsDeadBondedPet || !m_Mobile.CanSee( c ) || !m_Mobile.CanBeHarmful( c, false ) || c.Map != m_Mobile.Map )
			{
				// Our combatant is deleted, dead, hidden, or we cannot hurt them
				// Try to find another combatant

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "Something happened to my combatant, so I am going to fight {0}", m_Mobile.FocusMob.Name );

					m_Mobile.Combatant = c = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else
				{
					m_Mobile.DebugSay( "Something happened to my combatant, and nothing is around. I am on guard." );
					Action = ActionType.Guard;
					return true;
				}
			}

			if ( !m_Mobile.InLOS( c ) )
			{
				m_Mobile.DebugSay( "I can't see my target" );

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.DebugSay( "Nobody else is around" );
					m_Mobile.Combatant = c = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
			}

			if ( SmartAI && !m_Mobile.StunReady && m_Mobile.Skills[SkillName.Wrestling].Value >= 80.0 && m_Mobile.Skills[SkillName.Anatomy].Value >= 80.0 )
				EventSink.InvokeStunRequest( new StunRequestEventArgs( m_Mobile ) );

			if ( !m_Mobile.InRange( c, m_Mobile.RangePerception ) )
			{
				// They are somewhat far away, can we find something else?

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if ( !m_Mobile.InRange( c, m_Mobile.RangePerception * 3 ) )
				{
					m_Mobile.Combatant = null;
				}

				c = m_Mobile.Combatant;

				if ( c == null )
				{
					m_Mobile.DebugSay( "My combatant has fled, so I am on guard" );
					Action = ActionType.Guard;

					return true;
				}
			}

			if ( !m_Mobile.Controlled && !m_Mobile.Summoned && !m_Mobile.IsParagon )
			{
				if ( m_Mobile.Hits < m_Mobile.HitsMax * 20/100 )
				{
					// We are low on health, should we flee?

					bool flee = false;

					if ( m_Mobile.Hits < c.Hits )
					{
						// We are more hurt than them

						int diff = c.Hits - m_Mobile.Hits;

						flee = ( Utility.Random( 0, 100 ) > (10 + diff) ); // (10 + diff)% chance to flee
					}
					else
					{
						flee = Utility.Random( 0, 100 ) > 10; // 10% chance to flee
					}

					if ( flee )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "I am going to flee from {0}", c.Name );

						Action = ActionType.Flee;
						return true;
					}
				}
			}

			if ( m_Mobile.Spell == null && DateTime.Now > m_NextCastTime && m_Mobile.InRange( c, 12 ) )
			{
				// We are ready to cast a spell

				Spell spell = null;

				if ( SmartAI && m_Combo != -1 ) // We are doing a spell combo
				{
					spell = DoCombo( c );
				}
				else if ( SmartAI && (c.Spell is HealSpell || c.Spell is GreaterHealSpell) && !c.Poisoned ) // They have a heal spell out
				{
					spell = new PainSpikeSpell( m_Mobile, null );
				}
				else
				{
					spell = ChooseSpell( c );
				}

				// Now we have a spell picked
				// Move first before casting

				RunTo( c );

				if ( spell != null )
					spell.Cast();

				TimeSpan delay;

				if ( SmartAI || ( spell is DispelSpell ) )
					delay = TimeSpan.FromSeconds( m_Mobile.ActiveSpeed );
				else
					delay = GetDelay();

				m_NextCastTime = DateTime.Now + delay;
			}
			else if ( m_Mobile.Spell == null || !m_Mobile.Spell.IsCasting )
			{
				RunTo( c );
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				if ( !m_Mobile.Controlled )
				{
					ProcessTarget();

					Spell spell = CheckUseSpiritSpeak();

					if ( spell != null )
						spell.Cast();
				}

				base.DoActionGuard();
			}

			return true;
		}

		public override bool DoActionFlee()
		{
			Mobile c = m_Mobile.Combatant;

			if ( (m_Mobile.Mana > 20 || m_Mobile.Mana == m_Mobile.ManaMax) && m_Mobile.Hits > (m_Mobile.HitsMax / 2) )
			{
				m_Mobile.DebugSay( "I am stronger now, my guard is up" );
				Action = ActionType.Guard;
			}
			else if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am scared of {0}", m_Mobile.FocusMob.Name );

				RunFrom( m_Mobile.FocusMob );
				m_Mobile.FocusMob = null;

				if ( m_Mobile.Poisoned && Utility.Random( 0, 5 ) == 0 )
					m_Mobile.UseSkill(SkillName.SpiritSpeak);
			}
			else
			{
				m_Mobile.DebugSay( "Area seems clear, but my guard is up" );

				Action = ActionType.Guard;
				m_Mobile.Warmode = true;
			}

			return true;
		}

		private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				 0, -1,
				 0,  1,
				 1, -1,
				 1,  0,
				 1,  1,

				-2, -2,
				-2, -1,
				-2,  0,
				-2,  1,
				-2,  2,
				-1, -2,
				-1,  2,
				 0, -2,
				 0,  2,
				 1, -2,
				 1,  2,
				 2, -2,
				 2, -1,
				 2,  0,
				 2,  1,
				 2,  2
			};

		private bool ProcessTarget()
		{
			Target targ = m_Mobile.Target;

			if (targ == null)
				return false;

			Mobile toTarget;

			toTarget = m_Mobile.Combatant;

			if (toTarget != null)
				RunTo(toTarget);

			if ((targ.Flags & TargetFlags.Harmful) != 0 && toTarget != null)
			{
				if ((targ.Range == -1 || m_Mobile.InRange(toTarget, targ.Range)) && m_Mobile.CanSee(toTarget) && m_Mobile.InLOS(toTarget))
				{
					targ.Invoke(m_Mobile, toTarget);
				}
			}
			else if ((targ.Flags & TargetFlags.Beneficial) != 0)
			{
				targ.Invoke(m_Mobile, m_Mobile);
			}
			else
			{
				targ.Cancel(m_Mobile, TargetCancelType.Canceled);
			}

			return true;
		}
	}
}