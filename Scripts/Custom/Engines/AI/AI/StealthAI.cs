using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Items;

//
// Stealth AI
//
//

namespace Server.Mobiles
{
	public class StealthAI : BaseAI
	{
		public StealthAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionWander()
		{
			m_Mobile.DebugSay( "I have no combatant" );

			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				if ( m_Mobile.Hits <= 30 && m_Mobile.AllowedStealthSteps > 0 )
				{
					RunFrom( m_Mobile.FocusMob );
				}
				else
				{
				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
				}
			}
			else
			{
				if ( m_Mobile.Hits < m_Mobile.HitsMax )
				{
					HealOurself( m_Mobile );
					base.DoActionWander();
				}
				else if ( m_Mobile.Skills[SkillName.Hiding].Value >= 60.0 )
				{
					if ( m_Mobile.Hidden == false && m_Mobile.AllowedStealthSteps == 0)
						m_Mobile.UseSkill( SkillName.Hiding );
					else if ( m_Mobile.Skills[SkillName.Stealth].Value >= 70.0 && m_Mobile.AllowedStealthSteps <= 2 )
						m_Mobile.UseSkill( SkillName.Stealth );

					if ( m_Mobile.AllowedStealthSteps > 0 )
						base.DoActionWander();
				}
				else
				{
					base.DoActionWander();
				}
			}

			return true;
		}

		private void HealOurself( Mobile m )
		{
			if ( m.Skills[SkillName.SpiritSpeak].Value >= 60.0 && m.Mana >= 10 )
			{
				if ( m.NextSkillTime <= DateTime.Now )
					m.UseSkill( SkillName.SpiritSpeak );
			}
			if ( m.Skills[SkillName.Veterinary].Value >= 60.0 || m.Skills[SkillName.Healing].Value >= 60.0 )
			{
				BandageContext context = BandageContext.GetContext( m );
				if ( context == null )
					BandageContext.BeginHeal( m, m );
			}
		}

		public void RunFrom( Mobile m )
		{
			Run( (m_Mobile.GetDirectionTo( m ) - 4) & Direction.Mask );
		}

		public void Run( Direction d )
		{
			if ( (m_Mobile.Spell != null && m_Mobile.Spell.IsCasting) || m_Mobile.Paralyzed || m_Mobile.Frozen || m_Mobile.DisallowAllMoves )
				return;

			if ( m_Mobile.AllowedStealthSteps == 0 )
				m_Mobile.Direction = d | Direction.Running;
			else
				m_Mobile.Direction = d;

			DoMove( m_Mobile.Direction, true );

		}

		public override bool DoActionCombat()
		{
			Mobile combatant = m_Mobile.Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map || !combatant.Alive || combatant.IsDeadBondedPet )
			{
				m_Mobile.DebugSay( "My combatant is gone, so my guard is up" );

				Action = ActionType.Guard;

				return true;
			}

			if ( !m_Mobile.InRange( combatant, m_Mobile.RangePerception ) )
			{
				// They are somewhat far away, can we find something else?

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if ( !m_Mobile.InRange( combatant, m_Mobile.RangePerception * 3 ) )
				{
					m_Mobile.Combatant = null;
				}

				combatant = m_Mobile.Combatant;

				if ( combatant == null )
				{
					m_Mobile.DebugSay( "My combatant has fled, so I am on guard" );
					Action = ActionType.Guard;

					return true;
				}
			}

			/*if ( !m_Mobile.InLOS( combatant ) )
			{
				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.Combatant = combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
			}*/

			if ( MoveTo( combatant, true, m_Mobile.RangeFight ) )
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo( combatant );
			}
			else if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;

				return true;
			}
			else if ( m_Mobile.GetDistanceToSqrt( combatant ) > m_Mobile.RangePerception + 1 )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I cannot find {0}, so my guard is up", combatant.Name );

				Action = ActionType.Guard;

				return true;
			}
			else
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I should be closer to {0}", combatant.Name );
			}

			if ( !m_Mobile.Controlled && !m_Mobile.Summoned && !m_Mobile.IsParagon )
			{
				if ( m_Mobile.Hits < m_Mobile.HitsMax * 20/100 )
				{
					// We are low on health, should we flee?

					bool flee = false;

					if ( m_Mobile.Hits < combatant.Hits )
					{
						// We are more hurt than them

						int diff = combatant.Hits - m_Mobile.Hits;

						flee = ( Utility.Random( 0, 100 ) < (10 + diff) ); // (10 + diff)% chance to flee
					}
					else
					{
						flee = Utility.Random( 0, 100 ) < 10; // 10% chance to flee
					}

					if ( flee )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "I am going to flee from {0}", combatant.Name );

						Action = ActionType.Flee;
					}
				}
			}
			if ( m_Mobile.Hits < m_Mobile.HitsMax )
				HealOurself( m_Mobile );

			return true;
		}

		public override bool DoActionGuard()
		{
			if ( AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				if ( m_Mobile.Hits <= 30 && m_Mobile.AllowedStealthSteps > 0 )
				{
					RunFrom( m_Mobile.FocusMob );
				}
				else
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					Action = ActionType.Combat;
				}
			}
			else
			{
				if ( m_Mobile.Hits < m_Mobile.HitsMax )
					HealOurself( m_Mobile );
				base.DoActionGuard();
			}

			return true;
		}

		public override bool DoActionFlee()
		{
			if ( m_Mobile.Hits > m_Mobile.HitsMax/2 )
			{
				m_Mobile.DebugSay( "I am stronger now, so I will continue fighting" );
				Action = ActionType.Combat;
			}
			else
			{
				if ( m_Mobile.Hits < m_Mobile.HitsMax )
					HealOurself( m_Mobile );

				m_Mobile.FocusMob = m_Mobile.Combatant;
				base.DoActionFlee();
			}

			return true;
		}
	}
}