using System;
using System.Collections.Generic;
using Server.Regions;
using Server.Network;

namespace Server.Events.CrateRace
{
	#region CrateRegion
	public class CrateRegion : BaseRegion
	{
		public CrateRegion(Rectangle2D rect, Map map)
			: base("A Crate Race Track", map, 150, new Rectangle2D[] { rect })
		{
		}

		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler(EventSink_Login);
		}

		private static void EventSink_Login(LoginEventArgs e)
		{
			Mobile m = e.Mobile;
			if(m.Region != null)
				m.Region.OnEnter(m);
		}

		public override bool AllowHousing(Mobile from, Point3D p) { return false; }

		public override bool AllowSpawn() { return false; }

		public override bool AllowBeneficial(Mobile from, Mobile target) { return false; }

		public override bool AllowHarmful(Mobile from, Mobile target) { return false; }

		public override bool OnDecay(Item item) { return true; }

		public override bool CanUseStuckMenu(Mobile m) { return false; }

		public override bool OnBeginSpellCast(Mobile from, ISpell s)
		{
			if (from.AccessLevel == AccessLevel.Player)
			{
				from.SendMessage("You cannot cast spells on a race track.");
				return false;
			}
			return base.OnBeginSpellCast(from, s);
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (CrateRace.Running)
			{
				Mobile from = e.Mobile;
				RaceData rd = null;

				if (CrateRace.PartData.TryGetValue(from, out rd))
				{
					if (e.Speech.ToLower().IndexOf("use first crate") >= 0)
					{
						if (rd.Crate1 != null)
						{
							rd.Crate1.OnUsage(from);
							rd.Crate1 = null;
						}
						e.Blocked = true;
						return;
					}

					else if (e.Speech.ToLower().IndexOf("use second crate") >= 0)
					{
						if (rd.Crate2 != null)
						{
							rd.Crate2.OnUsage(from);
							rd.Crate2 = null;
						}
						e.Blocked = true;
						return;
					}

					else if (e.Speech.ToLower().IndexOf("use third crate") >= 0)
					{
						if (rd.Crate3 != null)
						{
							rd.Crate3.OnUsage(from);
							rd.Crate3 = null;
						}
						e.Blocked = true;
						return;
					}
				}
			}

			base.OnSpeech(e);
		}

		public override bool OnSkillUse(Mobile m, int skill)
		{
			if (m.AccessLevel == AccessLevel.Player)
			{
				m.SendMessage("You cannot use that skill here.");
				return false;
			}

			return base.OnSkillUse(m, skill);
		}

		public override void OnExit(Mobile m)
		{
			if (CrateRace.Participant(m))
				m.SendMessage("You have found a bug, please notice the staff.");

			else if (m.Player)
			{
				m.BodyMod = 0;
				m.Send( SpeedControl.Disable );
			}

			base.OnExit(m);
		}

		public override void OnDeath(Mobile m)
		{
			m.Hits = m.HitsMax;
			m.Stam = m.StamMax;

			CrateRace.OnDeath(m);
		}

		public override void OnEnter(Mobile m)
		{
			if (CrateRace.Participant(m) || m.AccessLevel > AccessLevel.Player)
			{
				CrateRace.SetPlayerSpeed(m);
				if (CrateRace.Animalised)
					m.BodyMod = 208;
				base.OnEnter(m);
			}

			else
				Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(Remove_Mobile), m);
		}

		private void Remove_Mobile(object obj)
		{
			((Mobile)obj).MoveToWorld(new Point3D(1434, 1707, 18), Map.Felucca);
		}

		public override TimeSpan GetLogoutDelay(Mobile m)
		{
			if (m.AccessLevel == AccessLevel.Player)
				return TimeSpan.FromMinutes(60);

			return base.GetLogoutDelay(m);
		}

		public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
		{
			global = 50;
			base.AlterLightLevel(m, ref global, ref personal);
		}
	}
	#endregion
}