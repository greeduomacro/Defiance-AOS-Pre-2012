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
using Server.Spells.Seventh;
using Server.Spells.Necromancy;
using Server.Misc;
using Server.Regions;
using Server.SkillHandlers;

namespace Server.Mobiles
{
	public class NecroMageAI : BaseAI
	{
		private DateTime m_NextCastTime;
		private DateTime m_NextHealTime;

		public NecroMageAI( BaseCreature m ) : base( m )
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

		private const double SpiritSpeakChance = 0.10; // 10% chance to use Spirit Speak at gm necromancy
		private const double DispelChance = 0.75; // 75% chance to dispel at gm magery

		public virtual double ScaleByNecromancy( double v )
		{
			return m_Mobile.Skills[SkillName.Necromancy].Value * v * 0.01;
		}

		public virtual double ScaleByMagery( double v )
		{
			return m_Mobile.Skills[SkillName.Magery].Value * v * 0.01;
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
			else if ( m_Mobile.Mana < m_Mobile.ManaMax )
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
					Spell spell = CheckCastHealingSpell();

					if ( spell != null )
						spell.Cast();
				}
			}

			return true;
		}

		private Spell CheckCastHealingSpell()
		{
			// If I'm poisoned, always attempt to cure.
			if ( m_Mobile.Poisoned )
				return new CureSpell( m_Mobile, null );

			// Summoned creatures never heal themselves.
			if ( m_Mobile.Summoned )
				return null;

			if ( m_Mobile.Controlled )
			{
				if ( DateTime.Now < m_NextHealTime )
					return null;
			}

			if ( Utility.Random( 0, 4 + (m_Mobile.Hits == 0 ? m_Mobile.HitsMax : (m_Mobile.HitsMax / m_Mobile.Hits)) ) < 3 )
					return null;

			Spell spell = null;

			if ( 0.3 > Utility.RandomDouble() && m_Mobile.Hits < m_Mobile.HitsMax )
			{
				m_Mobile.DebugSay("I am going to use Spirit Speak");
				m_Mobile.UseSkill(SkillName.SpiritSpeak);
			}
			else if ( m_Mobile.Hits < (m_Mobile.HitsMax - 50) )
			{
				spell = new GreaterHealSpell( m_Mobile, null );

				if ( spell == null )
					spell = new HealSpell( m_Mobile, null );
			}
			else if ( m_Mobile.Hits < (m_Mobile.HitsMax - 10) )
			{
				spell = new HealSpell( m_Mobile, null );
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
			if ( !MoveTo( m, true, m_Mobile.RangeFight ) )
				OnFailedMove();
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
			int maxCircle;
			if (0.4 > Utility.RandomDouble()) // Necro spells
			{
				m_Mobile.DebugSay( "Random necro spell" );
				maxCircle = (int)((m_Mobile.Skills[SkillName.Necromancy].Value + 20.0) / (100.0 / 7.0));

				switch ( Utility.Random( maxCircle*2 ) )
				{
					case  0: case  1: case  2: case  3:
						if (PainSpikeSpell.HasPainSpike( c ) && Utility.Random(3) > 0)
							goto case 10;
						else
							return new PainSpikeSpell( m_Mobile, null );
					case  4: case  5:
						if (BloodOathSpell.HasBloodOath(m_Mobile) || BloodOathSpell.HasBloodOath(c) || c.Combatant != m_Mobile)
							if (GetMobsInWitherRange(m_Mobile) > 2)
								return new WitherSpell( m_Mobile, null );
							else goto case 6;
						else
							return new BloodOathSpell( m_Mobile, null );
					case 6:
						if (CorpseSkinSpell.HasCorpseSkinScalar(c))
							return GetRandomFireSpell();
						else
							return new CorpseSkinSpell( m_Mobile, null );
					case  7:
						if (MindRotSpell.HasMindRotScalar(c))
							if (GetMobsInWitherRange(m_Mobile) > 2)
								return new WitherSpell( m_Mobile, null );
							else goto default;
						else return new MindRotSpell( m_Mobile, null );
					case  8: case  9: case 10:
						if (StrangleSpell.HasStrangle( c ))
							if (PainSpikeSpell.HasPainSpike( c ))
								goto case 6;
							else goto case 3;
						else
							return new StrangleSpell( m_Mobile, null );
					case 11:
						if (GetMobsInWitherRange(m_Mobile) > 0)
							return new WitherSpell( m_Mobile, null );
						else goto case 9;
					case 12: return new VengefulSpiritSpell( m_Mobile, null );
					default: return new PoisonStrikeSpell( m_Mobile, null );
				}
			}
			else // Mage spells
			{
				m_Mobile.DebugSay( "Random mage damaging spell" );
				maxCircle = (int)((m_Mobile.Skills[SkillName.Magery].Value + 20.0) / (100.0 / 7.0));

				if ( maxCircle < 1 )
					maxCircle = 1;

				switch ( Utility.Random( maxCircle*2 ) )
				{
					case  0: case  1: return new MagicArrowSpell( m_Mobile, null );
					case  2: case  3: return new HarmSpell( m_Mobile, null );
					case  4: case  5: return new FireballSpell( m_Mobile, null );
					case  6: case  7: return new LightningSpell( m_Mobile, null );
					case  8: case  9: return new MindBlastSpell( m_Mobile, null );
					case 10: return new EnergyBoltSpell( m_Mobile, null );
					case 11: return new ExplosionSpell( m_Mobile, null );
					default: return new FlameStrikeSpell( m_Mobile, null );
				}
			}
		}

		public virtual Spell GetRandomFireSpell()
		{
			switch ( Utility.Random( 7 ) )
			{
				case  0: case  1: return new FireballSpell( m_Mobile, null );
				case 2: case 3: return new PoisonStrikeSpell( m_Mobile, null );
				case 4: case 5: return new ExplosionSpell( m_Mobile, null );
				default: return new FlameStrikeSpell( m_Mobile, null );
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
			m_Mobile.DebugSay( "Random mage curse" );
			if ( Utility.RandomBool() )
			{
				if ( m_Mobile.Skills[SkillName.Magery].Value >= 40.0 )
					return new CurseSpell( m_Mobile, null );
			}
			switch ( Utility.Random( 10 ) )
			{
				default:
				case 0: case 1: case 2: return new WeakenSpell( m_Mobile, null );
				case 3: case 4: case 5: return new ClumsySpell( m_Mobile, null );
				case 6: case 7: case 8: return new FeeblemindSpell( m_Mobile, null );
				case 9: return GetRandomManaDrainSpell();
			}
		}

		public virtual Spell GetRandomManaDrainSpell()
		{
			m_Mobile.DebugSay( "Mana drain" );
			if ( Utility.RandomBool() )
			{
				if ( m_Mobile.Skills[SkillName.Magery].Value >= 80.0 )
					return new ManaVampireSpell( m_Mobile, null );
			}

			return new ManaDrainSpell( m_Mobile, null );
		}

		public virtual Spell DoDispel( Mobile toDispel )
		{
			if ( ScaleByMagery( DispelChance ) > Utility.RandomDouble() )
				return new DispelSpell( m_Mobile, null );

			return ChooseSpell( toDispel );
		}

		public virtual Spell ChooseSpell( Mobile c )
		{
			Spell spell = null;

			spell = CheckCastHealingSpell();

			if ( spell != null )
				return spell;

			switch (Utility.Random(5))
			{
				default:
				case 0: case 1: case 2: // Deal some damage
					{
						spell = GetRandomDamageSpell(c);
						break;
					}
				case 3: // Curse it
					{
						spell = GetRandomCurseSpell();
						break;
					}
				case 4: // Set up a combo of attacks
					{
						switch ( Utility.Random( 6 ) )
						{
							default: case  0:
							{
								m_Combo = 0;
								spell = new PoisonSpell(m_Mobile, null);
								m_Mobile.DebugSay( "Poison" );
								break;
							}
							case  1:
							{
								m_Combo = 0;
								spell = new EvilOmenSpell(m_Mobile, null);
								m_Mobile.DebugSay( "Evil Omen" );
								break;
							}
							case  2:
							{
								m_Combo = 7;
								spell = new CorpseSkinSpell(m_Mobile, null);
								m_Mobile.DebugSay( "Corpse Skin" );
								break;
							}
							case  3:
							{
								m_Combo = 7;
								spell = new CurseSpell(m_Mobile, null);
								m_Mobile.DebugSay( "Curse" );
								break;
							}
							case  4:
							{
								m_Combo = 10;
								spell = new CurseSpell(m_Mobile, null);
								m_Mobile.DebugSay( "Curse" );
								break;
							}
							case  5:
							{
								m_Combo = 10;
								spell = new ExplosionSpell(m_Mobile, null);
								m_Mobile.DebugSay( "Explosion" );
								break;
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

			if ( m_Mobile.HitsMax > 0 && (m_Mobile.Hits / m_Mobile.HitsMax) < 0.1 && m_Mobile.Hits < 300 )
			{
				spell = CheckCastHealingSpell();
				m_Combo = -1;
				return spell;
			}
			if (m_Combo == 0)
			{
				spell = new ExplosionSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Explosion" );
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 1)
			{
				spell = new CorpseSkinSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Corpse skin" );
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 2)
			{
				if ( !c.Poisoned )
				{
					spell = new PoisonSpell(m_Mobile, null);
					m_Mobile.DebugSay( "Poison" );
				}
				else
				{
					spell = new CurseSpell(m_Mobile, null);
					m_Mobile.DebugSay( "Curse" );
				}

				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 3)
			{
				spell = new StrangleSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Strangle" );
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 4)
			{
				spell = new PainSpikeSpell(m_Mobile, null);
				m_Mobile.DebugSay( "pain spike" );
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 7)
			{
				spell = new ExplosionSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Explosion" );
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 8)
			{
				if ( !c.Poisoned )
				{
					spell = new PoisonSpell(m_Mobile, null);
					m_Mobile.DebugSay( "Poison" );
				}
				else
				{
					spell = new CurseSpell(m_Mobile, null);
					m_Mobile.DebugSay( "Curse" );
				}

				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 9)
			{
				spell = new FlameStrikeSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Flamestrike" );
				m_Combo = -1;
			}
			else if (m_Combo == 10)
			{
				spell = new StrangleSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Strangle" );
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 11)
			{
				spell = new CorpseSkinSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Corpse skin" );
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 12)
			{
				spell = new ExplosionSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Explosion" );
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 13)
			{
				spell = new PoisonStrikeSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Poison strike" );
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 14)
			{
				spell = new PoisonStrikeSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Poison strike" );
				++m_Combo; // Move to next spell
			}
			else if (m_Combo == 15)
			{
				spell = new PainSpikeSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Pain spike" );
				m_Combo = -1;
			}
			if (m_Combo == 5 && spell == null)
			{
				switch (Utility.Random(3))
				{
					default:
					case 0:
						{
							spell = new ExplosionSpell(m_Mobile, null);
							m_Mobile.DebugSay( "Explosion" );
							break;
						}
					case 1:
						{
							spell = new PoisonStrikeSpell(m_Mobile, null);
							m_Mobile.DebugSay( "Poison strike" );
							break;
						}
					case 2:
						{
							spell = new FlameStrikeSpell(m_Mobile, null);
							m_Mobile.DebugSay( "Flamestrike" );
							++m_Combo; // Move to next spell
							break;
						}
				}
			}
			else if (m_Combo == 6 && spell == null)
			{
				spell = new VengefulSpiritSpell(m_Mobile, null);
				m_Mobile.DebugSay( "Revenant" );
				m_Combo = -1;
			}

			return spell;
		}

		private TimeSpan GetDelay()
		{
			double del = ScaleByMagery( 3.0 );
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
				Mobile toDispel = FindDispelTarget( true );

				if ( m_Mobile.Poisoned ) // Top cast priority is cure
				{
					m_Mobile.DebugSay( "I am going to cure myself" );

					spell = new CureSpell( m_Mobile, null );
				}
				else if ( toDispel != null ) // Something dispellable is attacking us
				{
					m_Mobile.DebugSay( "I am going to dispel {0}", toDispel );

					spell = DoDispel( toDispel );
				}
				else if ( m_Combo != -1 ) // We are doing a spell combo
				{
					spell = DoCombo( c );
				}
				else if ( (c.Spell is HealSpell || c.Spell is GreaterHealSpell) && !c.Poisoned ) // They have a heal spell out
				{
					spell = new PoisonSpell( m_Mobile, null );
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

				delay = GetDelay();
				//delay = TimeSpan.FromSeconds( m_Mobile.ActiveSpeed );

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

					Spell spell = CheckCastHealingSpell();

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
					new CureSpell( m_Mobile, null ).Cast();
			}
			else
			{
				m_Mobile.DebugSay( "Area seems clear, but my guard is up" );

				Action = ActionType.Guard;
				m_Mobile.Warmode = true;
			}

			return true;
		}

		public Mobile FindDispelTarget( bool activeOnly )
		{
			if ( m_Mobile.Deleted || m_Mobile.Int < 95 || CanDispel( m_Mobile ) || m_Mobile.AutoDispel )
				return null;

			if ( activeOnly )
			{
				List<AggressorInfo> aggressed = m_Mobile.Aggressed;
				List<AggressorInfo> aggressors = m_Mobile.Aggressors;

				Mobile active = null;
				double activePrio = 0.0;

				Mobile comb = m_Mobile.Combatant;

				if ( comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && m_Mobile.InRange( comb, 12 ) && CanDispel( comb ) )
				{
					active = comb;
					activePrio = m_Mobile.GetDistanceToSqrt( comb );

					if ( activePrio <= 2 )
						return active;
				}

				for ( int i = 0; i < aggressed.Count; ++i )
				{
					AggressorInfo info = aggressed[i];
					Mobile m = info.Defender;

					if ( m != comb && m.Combatant == m_Mobile && m_Mobile.InRange( m, 12 ) && CanDispel( m ) )
					{
						double prio = m_Mobile.GetDistanceToSqrt( m );

						if ( active == null || prio < activePrio )
						{
							active = m;
							activePrio = prio;

							if ( activePrio <= 2 )
								return active;
						}
					}
				}

				for ( int i = 0; i < aggressors.Count; ++i )
				{
					AggressorInfo info = aggressors[i];
					Mobile m = info.Attacker;

					if ( m != comb && m.Combatant == m_Mobile && m_Mobile.InRange( m, 12 ) && CanDispel( m ) )
					{
						double prio = m_Mobile.GetDistanceToSqrt( m );

						if ( active == null || prio < activePrio )
						{
							active = m;
							activePrio = prio;

							if ( activePrio <= 2 )
								return active;
						}
					}
				}

				return active;
			}
			else
			{
				Map map = m_Mobile.Map;

				if ( map != null )
				{
					Mobile active = null, inactive = null;
					double actPrio = 0.0, inactPrio = 0.0;

					Mobile comb = m_Mobile.Combatant;

					if ( comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && CanDispel( comb ) )
					{
						active = inactive = comb;
						actPrio = inactPrio = m_Mobile.GetDistanceToSqrt( comb );
					}

					foreach ( Mobile m in m_Mobile.GetMobilesInRange( 12 ) )
					{
						if ( m != m_Mobile && CanDispel( m ) )
						{
							double prio = m_Mobile.GetDistanceToSqrt( m );

							if ( !activeOnly && (inactive == null || prio < inactPrio) )
							{
								inactive = m;
								inactPrio = prio;
							}

							if ( (m_Mobile.Combatant == m || m.Combatant == m_Mobile) && (active == null || prio < actPrio) )
							{
								active = m;
								actPrio = prio;
							}
						}
					}

					return active != null ? active : inactive;
				}
			}

			return null;
		}

		public bool CanDispel( Mobile m )
		{
			return ( m is BaseCreature && ((BaseCreature)m).Summoned && m_Mobile.CanBeHarmful( m, false ) && !((BaseCreature)m).IsAnimatedDead );
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