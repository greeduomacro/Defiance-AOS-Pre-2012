using System;
using Server.Regions;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Chivalry;
using Server.Spells.Necromancy;

namespace Server.Events.Duel
{
	public class DuelRegion : BaseRegion
	{
		private bool NoMounts;
		private bool NoPots;
		private Dueller m_Npc;

		public DuelRegion(Dueller m)
			: base("A Duel Arena", m.Map, 152, new Rectangle3D[] { m.Area })
		{
			m_Npc = m;
			NoPots = m.NoPots;
			NoMounts = m.NoMounts;
		}

		private bool Participant(Mobile m)
		{
			Mobile pc = m;

			if (m is BaseCreature)
			{
				BaseCreature bc = (BaseCreature)m;

				if (bc.Controlled && bc.ControlMaster != null)
					pc = bc.SummonMaster;

				else if (bc.Summoned && bc.SummonMaster != null)
					pc = bc.SummonMaster;

				else
					return false;
			}

			foreach (Mobile b in m_Npc.Participants)
			{
				if (pc == b)
					return true;
			}

			return false;
		}

		public override bool AllowHousing(Mobile from, Point3D p)
		{
			return false;
		}

		public override bool AllowSpawn()
		{
			if (m_Npc.NoSummons)
				return false;

			return true;
		}

		public override void OnBeneficialAction(Mobile helper, Mobile target)
		{
			if (!Participant(target))
			{
			}

			else
				base.OnBeneficialAction(helper, target);
		}

		public override bool AllowBeneficial(Mobile from, Mobile target)
		{
			if (!Participant(target))
				return false;

			return base.AllowBeneficial(from, target);
		}

		public override bool AllowHarmful(Mobile from, Mobile target)
		{
			if (!Participant(target))
				return false;

			if (m_Npc.DuelType is TMFDuelType && from.Weapon is BaseRanged)
			{
				from.SendMessage("You cannot use a weapon in this duel.");
				return false;
			}

			return base.AllowHarmful(from, target);
		}

		public override bool CanUseStuckMenu(Mobile m)
		{
			m.SendMessage("You cannot use the Stuck menu here.");
			return false;
		}

		public override bool OnTarget(Mobile m, Target t, object o)
		{
			if (!Participant(m))
				return false;

			return base.OnTarget(m, t, o);
		}

		public override bool OnResurrect(Mobile m)
		{
			m.SendMessage("You cannot ressurect here.");
			return false;
		}

		private bool DuelTypeCheck(DuelType type, ISpell s)
		{
			if (type is UDFDuelType)
				return false;

			else if (type is TDFDuelType && !(s is PaladinSpell || s is NecromancerSpell))
				return false;

			else if ((type is UMFDuelType || type is TMFDuelType) && (s is PaladinSpell || s is NecromancerSpell))
				return false; // Fix by Silver: TDFDuelType should be TMFDuelType - Not working yet though

			return true;
		}

		public override bool OnBeginSpellCast(Mobile from, ISpell s)
		{
			if (s is MarkSpell || s is RecallSpell || s is GateTravelSpell || s is SacredJourneySpell || s is InvisibilitySpell)
			{
				from.SendMessage("You cannot cast that spell here.");
				return false;
			}

			if (!DuelTypeCheck(m_Npc.DuelType, s))
			{
				from.SendMessage("After reviewing your current type of duel, you choose against using this spell.");
				return false;
			}

			if (m_Npc.NoSummons)
			{
				if (s is SummonFamiliarSpell || s is VengefulSpiritSpell)
				{
					from.SendMessage("You choose against casting a summon here.");
					return false;
				}
			}

			if (m_Npc.NoArea)
			{
				if (s is PoisonFieldSpell || s is FireFieldSpell || s is EnergyFieldSpell || s is ParalyzeFieldSpell || s is WallOfStoneSpell)
				{
					from.SendMessage("You choose against casting an area spell here.");
					return false;
				}
			}

			if (NoMounts && from.AccessLevel == AccessLevel.Player)
			{
				if (((Spell)s).Info.Name == "Ethereal Mount")
				{
					from.SendMessage("You cannot mount your ethereal here.");
					return false;
				}
			}

			if (m_Npc.SpellWatch)
			{
				foreach (SpellWatcher sw in m_Npc.SpellWatchList)
					if (sw.m_Part == from)
						return sw.SpellAllowed(s);
			}

			return true;
		}

		public override void OnDidHarmful(Mobile harmer, Mobile harmed)
		{
			if (harmer.Hidden)
				harmer.RevealingAction();

			base.OnDidHarmful(harmer, harmed);
		}

		public override bool OnDecay(Item item)
		{
			return true;
		}

		public override bool OnSkillUse(Mobile m, int skill)
		{
			if (m.AccessLevel == AccessLevel.Player)
			{
				if (skill == (int)SkillName.Hiding || skill == (int)SkillName.Stealth || skill == (int)SkillName.DetectHidden || skill == (int)SkillName.Camping)
				{
					m.SendMessage("You cannot use that skill here.");
					return false;
				}
			}

			return base.OnSkillUse(m, skill);
		}

		public override void OnExit(Mobile m)
		{
			if (Registered)
			{
				if (Participant(m))
				{
					if (m.FindItemOnLayer(Layer.Bracelet) is BraceletOfBinding)
						m.SendMessage("You have tried to escape from your duel using a bracelet of binding. A message has been sent to the staff. If you repeat to do this you will be jailed.");

					else if (m is PlayerMobile && ((((PlayerMobile)m).LastOnline - DateTime.Now) < TimeSpan.FromSeconds(2.0)))
					{
						m.SendMessage("You have found a bug, please notice the staff.");
						Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(DelayedMove), new object[] { m, 1 });
					}
				}

				Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(UpdateMob), m);
				base.OnExit(m);
			}
		}

		public override void OnEnter(Mobile m)
		{
			if (m != m_Npc && !Participant(m))
			{
				Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(DelayedMove), new object[] { m, 3 });
				m.SendMessage("You are not allowed to enter this arena while someone else is duelling.");
				return;
			}

			Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(UpdateMob), m);
			base.OnEnter(m);
		}

		private void UpdateMob(object state)
		{
			Mobile m = (Mobile)state;

			if (m == null)
				return;

			m.ClearScreen();
			m.SendRemovePacket();

			m.SendEverything();
			m.SendIncomingPacket();
		}

		private void DelayedMove(object arr)
		{
			object[] array = (object[])arr;
			m_Npc.MoveMob((Mobile)array[0], (int)array[1]);
		}

		public override TimeSpan GetLogoutDelay(Mobile m)
		{
			if (m.AccessLevel == AccessLevel.Player)
				return TimeSpan.FromMinutes(10);

			return base.GetLogoutDelay(m);
		}

		public override bool OnDoubleClick(Mobile m, object o)
		{
			if (o is BasePotion && NoPots)
			{
				m.SendMessage("You cannot drink potions here.");
				return false;
			}

			if (o is Corpse)
			{
				Corpse c = (Corpse)o;

				bool canLoot;

				if (c.Owner == m)
					canLoot = true;
				else
					canLoot = false;

				if (!canLoot)
					m.SendMessage("You cannot loot that corpse here.");

				if (m.AccessLevel >= AccessLevel.GameMaster && !canLoot)
				{
					m.SendMessage("This is unlootable but you are able to open that with your Godly powers.");
					return true;
				}

				return canLoot;
			}

			return base.OnDoubleClick(m, o);
		}

		public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
		{
			global = 0;
			base.AlterLightLevel(m, ref global, ref personal);
		}
	}
}