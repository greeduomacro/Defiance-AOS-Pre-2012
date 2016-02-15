using Server;
using System;
using Server.Regions;
using System.Collections;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Eighth;
using Server.Spells.Fourth;
using Server.Spells.Third;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Necromancy;
using Server.Spells.Chivalry;
using System.Text;
using Server.Network;

namespace Server.Events.CTF
{
	public class CTFGameRegion : Region
	{
		public CTFGameRegion() : base("CTF Region", CTFGame.Stone.Map, 150, CTFGame.GameArea)
		{
		}

		public override bool CanUseStuckMenu( Mobile m )
		{
			return false;
		}

		public override void OnDeath( Mobile m )
		{
			if (CTFGame.GameData.IsInGame(m))
			{
				CTFTeam team = CTFGame.GameData.GetPlayerTeam(m);
				new DeathTimer(m).Start();

				// Add death score
				CTFGame.AddScore(m, CTFGame.CTFScoreType.Deaths);

				if (m.LastKiller != null)
				{
					if (CTFGame.GameData.IsInGame(m.LastKiller))
					{
						// Add kill score
						CTFGame.AddScore(m.LastKiller, CTFGame.CTFScoreType.Kills);
					}
				}

				CTFFlag flag = CTFGame.GetFlag(m);
				if (flag != null)
				{
					flag.DropFlag(m);

					CTFRobe robe = m.FindItemOnLayer(Layer.OuterTorso) as CTFRobe;
					if (robe != null)
						robe.Hue = team.Hue;
				}
			}

			base.OnDeath(m);
		}

		public enum CTFScoreType
		{
			Kills,
			Deaths,
			Captures,
			Returns
		}

		private class DeathTimer : Timer
		{
			private Mobile m_Mob;

			public DeathTimer( Mobile m ) : base( TimeSpan.FromSeconds( 10.0 ) )
			{
				m_Mob = m;
			}

			protected override void OnTick()
			{
				if ( !m_Mob.Alive && CTFGame.Running)
				{
					CTFPlayerGameData pgd = CTFGame.GameData.GetPlayerData(m_Mob);
					CTFGame.ResurrectPlayer(pgd);

					if (m_Mob.Corpse != null && !m_Mob.Corpse.Deleted)
						m_Mob.Corpse.Delete();
				}
			}
		}

		public override bool OnBeginSpellCast( Mobile m, ISpell s )
		{
			if (!CTFGame.Running)
			{
				m.SendMessage( CTFGame.HuePerson, "You cannot cast spells until the game has started." );
				return false;
			}
			else if( CTFGame.GetFlag( m ) != null && s is InvisibilitySpell)
			{
				m.SendMessage( CTFGame.HuePerson, "You cannot use invisibility spell while carrying a flag." );
				return false;
			}
			else if ( m.AccessLevel == AccessLevel.Player &&
				( s is MarkSpell || s is RecallSpell || s is GateTravelSpell || s is PolymorphSpell ||
				s is SummonDaemonSpell || s is AirElementalSpell || s is EarthElementalSpell || s is EnergyVortexSpell ||
				s is FireElementalSpell || s is WaterElementalSpell || s is BladeSpiritsSpell || s is SummonCreatureSpell ||
				s is EnergyFieldSpell || s is ResurrectionSpell || s is LichFormSpell || s is HorrificBeastSpell || s is WraithFormSpell ||
				s is VengefulSpiritSpell || s is SummonFamiliarSpell || s is SacredJourneySpell
				) )
			{
				m.SendMessage( CTFGame.HuePerson, "That spell is not allowed here." );
				return false;
			}

			else if (((Spell)s).Info.Name == "Ethereal Mount")
			{
				m.SendMessage("You cannot mount your ethereal here.");
				return false;
			}

			return base.OnBeginSpellCast( m, s );
		}

		public override bool OnSkillUse ( Mobile m , int Skill )
		{
			switch( Skill )
			{
				case (int)SkillName.Hiding:
					if( CTFGame.GetFlag( m ) != null )
					{
						m.SendMessage( CTFGame.HuePerson, "You can't hide while carrying a flag." );
						return false;
					}
					break;
			}

			return base.OnSkillUse( m, Skill );
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return from.AccessLevel != AccessLevel.Player;
		}

		public override bool AllowBeneficial(Mobile from, Mobile target)
		{
			CTFTeam ft = CTFGame.GameData.GetPlayerTeam(from);
			if ( ft == null )
				return false;

			CTFTeam tt = CTFGame.GameData.GetPlayerTeam(target);
			if ( tt == null )
				return false;

			return ft == tt;
		}

		public override void OnEnter( Mobile m )
		{
			if (m is PlayerMobile)
			{
				m.SendMessage("You have entered CTF Game Area!");
				((PlayerMobile)m).InvalidateProperties();
			}
		}

		public override void OnExit( Mobile m )
		{
			if (m is PlayerMobile)
			{
				m.SendMessage("You have left the CTF Game Area!");
				((PlayerMobile)m).InvalidateProperties();
			}
		}

		public override bool AllowHarmful(Mobile from, Mobile target)
		{
			if (!CTFGame.Running)
				return false;

			CTFTeam ft = CTFGame.GameData.GetPlayerTeam(from);
			if ( ft == null )
				return false;

			CTFTeam tt = CTFGame.GameData.GetPlayerTeam(target);
			if ( tt == null )
				return false;

			return ft != tt;
		}
	}
}