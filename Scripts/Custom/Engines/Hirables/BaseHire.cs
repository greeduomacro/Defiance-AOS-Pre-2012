using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

namespace Server.Mobiles
{
	public class BaseHire : BaseCreature
	{
		// Stores all npcs that have been hired
		public static List<BaseHire> m_HireNpcRegister = new List<BaseHire>();

		public static void Initialize()
		{
			new PayTimer().Start();
		}

		private static TimeSpan PayDelay = TimeSpan.FromMinutes(30.0);
		private Timer m_DeleteTimer;

		public override bool ShowFameTitle { get { return false; } }
		public override bool ClickTitle { get { return true; } }
		public override bool KeepsItemsOnDeath { get { return true; } }
		public override bool IsBondable { get { return false; } }
		public virtual bool IsInvulnerable { get { return false; } }

		public override bool CanBeDamaged()
		{
			return !IsInvulnerable;
		}

		#region Weapon Ability
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

		#region CommandProperties
		private int m_iSalary;
		public int Salary
		{
			get { return m_iSalary; }
			set { m_iSalary = value; }
		}

		private int m_iHoldGold;
		[CommandProperty(AccessLevel.GameMaster)]
		public int HoldGold
		{
			get { return m_iHoldGold; }
			set { m_iHoldGold = value; }
		}

		private DateTime m_iNextPayCheck;
		public DateTime NextPayCheck
		{
			get { return m_iNextPayCheck; }
			set { m_iNextPayCheck = value; }
		}
		#endregion

		public BaseHire(string title)
			: this(title, AIType.AI_Melee)
		{
		}

		public BaseHire(string title, AIType AI)
			: base(AI, FightMode.Aggressor, 10, 1, 0.2, 0.4)
		{
			InitStats(60, 60, 60);
			Title = title;
			InitBody();
			InitOutfit();
			InitSkills();

			HoldGold = 0;
			m_iSalary = GetSalary();

			ControlSlots = 2;
		}

		public virtual void InitSkills()
		{
		}

		public virtual void InitBody()
		{
			InitStats(100, 100, 25);

			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();

			if (IsInvulnerable && !Core.AOS)
				NameHue = 0x35;

			if (Female = Utility.RandomBool())
			{
				Body = 0x191;
				Name = NameList.RandomName("female");
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName("male");
			}

			int hairHue = GetHairHue();

			Utility.AssignRandomHair(this, hairHue);
			Utility.AssignRandomFacialHair(this, hairHue);
		}

		public virtual int GetRandomHue()
		{
			switch (Utility.Random(5))
			{
				default:
				case 0: return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
			}
		}

		public virtual int GetHairHue()
		{
			return Utility.RandomHairHue();
		}

		public virtual void InitOutfit()
		{
			switch (Utility.Random(3))
			{
				case 0: AddItem(new FancyShirt(GetRandomHue())); break;
				case 1: AddItem(new Doublet(GetRandomHue())); break;
				case 2: AddItem(new Shirt(GetRandomHue())); break;
			}

			if (Female)
			{
				switch (Utility.Random(6))
				{
					case 0: AddItem(new ShortPants(GetRandomHue())); break;
					case 1:
					case 2: AddItem(new Kilt(GetRandomHue())); break;
					case 3:
					case 4:
					case 5: AddItem(new Skirt(GetRandomHue())); break;
				}
			}
			else
			{
				switch (Utility.Random(2))
				{
					case 0: AddItem(new LongPants(GetRandomHue())); break;
					case 1: AddItem(new ShortPants(GetRandomHue())); break;
				}
			}

			PackGold(100, 200);
		}

		public BaseHire(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version

			writer.Write((int)m_iHoldGold);
			writer.Write(m_iNextPayCheck);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			m_iHoldGold = reader.ReadInt();
			m_iNextPayCheck = reader.ReadDateTime();
			m_iSalary = GetSalary();

			if (IsHired)
				m_HireNpcRegister.Add(this);
		}

		public override void OnDelete()
		{
			m_HireNpcRegister.Remove(this);
			if ( m_DeleteTimer != null )
				m_DeleteTimer.Stop();
			m_DeleteTimer = null;
			base.OnDelete();
		}

		public virtual void TakeSalary()
		{
			HoldGold -= m_iSalary;
			NextPayCheck = DateTime.Now + PayDelay;
		}

		public virtual void OnReport(Mobile from)
		{
			if (IsHired)
			{
				int iDaysLeft = (int)HoldGold / Salary;

				if (ControlMaster != null)
					SayTo(from, 1043232, ControlMaster.Name); // I currently accept orders from ~1_NAME~.

				if (iDaysLeft <= 0)
					SayTo(from, 1043233); // I am confused about my job.
				else
					SayTo(from, 1043233 + (int)(Loyalty / 10));
				// 1043233 -> 1043243
			}
		}

		public virtual bool OnHire(Mobile from)
		{
			if (IsHired)
			{
				SayTo(from, 1042495); // I have already been hired.
				return false;
			}

			if (from.Followers + ControlSlots > from.FollowersMax)
			{
				SayTo(from, 1049672); // Thou must reduce thine followers before I will work for thee!
				return false;
			}

			if (SetControlMaster(from))
			{
				IsBonded = false;
				ControlOrder = OrderType.None;

				HoldGold = 0;
				IsHired = true;
				CanOrder = false;
				SayHireCost(from);

				if (!m_HireNpcRegister.Contains(this))
					m_HireNpcRegister.Add(this);

				InvalidateProperties();
				return true;
			}
			return false;
		}

		public override void OnRelease(bool orderFromPlayer)
		{
			if (this != null && !Deleted && ControlMaster != null)
			{
				// Only say this if released by the player
				if (orderFromPlayer)
				{
					SayTo(ControlMaster, 502034); // I thank thee for thy kindness!
					SayTo(ControlMaster, 502005); // I quit.
				}

				Loyalty = BaseCreature.MaxLoyalty;
				IsBonded = false;
				BondingBegin = DateTime.MinValue;
				OwnerAbandonTime = DateTime.MinValue;
				ControlTarget = null;
				AIObject.DoOrderRelease(); // this will prevent no release of creatures left alone with AI disabled (and consequent bug of Followers)

				IsHired = false;
				CanOrder = false;

				m_HireNpcRegister.Remove(this);
			}

			//Added by Blady - they'd better be removed on release:
			if ( m_DeleteTimer != null )
				m_DeleteTimer.Stop();
			m_DeleteTimer = new DeleteTimer( this, TimeSpan.FromSeconds( 10.0 ));
			m_DeleteTimer.Start();

			InvalidateProperties();
		}

		private class DeleteTimer : Timer
		{
			private Mobile m_Mobile;

			public DeleteTimer( Mobile m, TimeSpan delay ) : base( delay )
			{
				m_Mobile = m;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Mobile.Delete();
			}
		}

		public void SayHireCost(Mobile from)
		{
			SayTo(from, 1043256, string.Format("{0}", m_iSalary)); // "I am available for hire for ~1_AMOUNT~ gold coins a day. If thou dost give me gold, I will work for thee."
		}

		public override bool OnReciveCommand(Mobile from, int keyword)
		{
			// If cannot order only allow *release
			if (!CanOrder && keyword != 0x16D)
			{
				SayTo(from, 1005472); // I will do nothing for thee until I am paid.
				return false;
			}
			else if (CanOrder)
			{
				switch (keyword)
				{
					default: break;
					case 0x15A: SayTo(from, 502026); break; // Who shall I follow? // *follow
					case 0x15B: SayTo(from, 1005480); break; // From whom do you wish me to accept orders? // *friend
					case 0x16E: SayTo(from, 502037); break; // Whom do you wish me to work for? // *transfer

					case 0x163: // *follow me
					case 0x155: // *come
					case 0x15C: SayTo(from, 502002); break; // Very well. // *guard

					case 0x15D: // *kill
					case 0x15E: SayTo(from, 502013); break; // Who should I attack? // *attack

					case 0x161: // *stop
					case 0x16F: SayTo(from, 502035); break; // Very well, I am no longer guarding or following. // *stay
				}
			}

			return base.OnReciveCommand(from, keyword);
		}

		public override void OnReciveOrder(Mobile from, OrderType order)
		{
			switch (order)
			{
				default: break;
				case OrderType.Follow: SayTo(from, 502026); break; // Who shall I follow?
				case OrderType.Attack: SayTo(from, 502013); break; // Who should I attack?
				case OrderType.Transfer: SayTo(from, 502037); break; // Whom do you wish me to work for?
				case OrderType.Friend: SayTo(from, 1005480); break; // From whom do you wish me to accept orders?
				case OrderType.Unfriend: SayTo(from, 1070949); break; // From whom do you wish me to ignore orders?
				case OrderType.Guard: SayTo(from, 502002); break; // Very well.
				case OrderType.Stay:
				case OrderType.Stop: SayTo(from, 502035); break; // Very well, I am no longer guarding or following.
			}
		}

		public override bool OnTargetOrder(Mobile from, Mobile target, OrderType order)
		{
			bool value = base.OnTargetOrder(from, target, order);

			if (order == OrderType.Attack)
			{
				if (target == this)
				{
					SayTo(from, 502039); // *looks confused*
					return false;
				}
				else if (target is BaseVendor || target is BaseHire || target is BaseEscortable)
				{
					SayTo(from, 502048); // I am no murderer!
					return false;
				}
			}

			if (value)
			{
				switch (order)
				{
					default: break;
					case OrderType.Follow:
					case OrderType.Attack: SayTo(from, 502002); break; // Very well.
				}
			}
			return value;
		}

		public virtual bool WasNamed(string speech)
		{
			string name = this.Name;

			return (name != null && Insensitive.StartsWith(speech, name));
		}

		public override bool HandlesOnSpeech(Mobile from)
		{
			if (from.InRange(this, 3))
				return true;

			return base.HandlesOnSpeech(from);
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			Mobile from = e.Mobile;

			if (!e.Handled && from is PlayerMobile && from.InRange(this.Location, 2) && WasNamed(e.Speech))
			{
				PlayerMobile pm = (PlayerMobile)from;

				if (e.HasKeyword(0x0162)) // *hire
				{
					OnHire(from);
					e.Handled = true;
				}
				else if (e.HasKeyword(0x0160)) // *report
				{
					OnReport(from);
					e.Handled = true;
				}
			}
			base.OnSpeech(e);
		}

		public virtual int GetSalary()
		{
			int salary = 0;
			for (int i = 0; i < Skills.Length; ++i)
				salary += (int)Skills[i].Value;

			if (salary > 0)
				salary /= 35;
			else
				return 1;

			if (salary < 1)
				return 1;

			return salary;
		}

		private void TryToChangeLoyalty(bool increase, int value, double chance)
		{
			if (chance >= Utility.RandomDouble())
			{
				if (increase && Loyalty < BaseCreature.MaxLoyalty)
				{
					Loyalty += value;
					if (Loyalty > BaseCreature.MaxLoyalty)
						Loyalty = BaseCreature.MaxLoyalty;
				}
				else if (Loyalty > 0)
				{
					Loyalty -= value;
					if (Loyalty < 0)
						Loyalty = 0;
				}
			}
		}

		protected Item AddProps(Item item)
		{
			LootPackEntry.Mutate(item, Utility.RandomMinMax(3, 4), 40, 80, 240 );
			return item;
		}

		public override bool OnDragDrop(Mobile from, Item item)
		{
			if (ControlMaster == from && IsHired)
			{
				if (item is Food)
				{
					if (CanOrder)
					{
						Food food = (Food)item;
						bool poisoned = (food.Poison != null);

						if (!poisoned)
						{
							SayTo(from, 501547); // This tasteth good.
							SayTo(from, 501548); // I thank thee.
						}
						else
						{
							ApplyPoison(from, food.Poison);
							SayTo(from, 1010514); // That was poisoned!
						}

						int iLoyaltyChangeValue = food.Amount;
						if (iLoyaltyChangeValue > 10)
							iLoyaltyChangeValue = 10;

						TryToChangeLoyalty(!poisoned, iLoyaltyChangeValue, poisoned ? 0.5 : 0.2);

						// Play a random "eat" sound
						PlaySound(Utility.Random(0x3A, 3));

						if (Body.IsHuman && !Mounted)
							Animate(34, 5, 1, true, false, 0);

						food.Consume();
						return true;
					}

					SayTo(from, 501550); // I am not interested in this.
					return false;
				}
				else if (item is Gold)
				{
					if (item.Amount >= m_iSalary)
					{
						if (m_iHoldGold <= 0)
							NextPayCheck = DateTime.Now + PayDelay;

						m_iHoldGold += item.Amount;

						TryToChangeLoyalty(true, 10, 0.5);

						SayTo(from, 1043258, string.Format("{0}", HoldGold / m_iSalary)); // I thank thee for paying me. I will work for thee for ~1_NUMBER~ days.

						CanOrder = true;
						return true;
					}
					else
					{
						SayTo(from, 502062); // Thou must pay me more than this!
						return false;
					}
				}
				else
				{
					SayTo(from, 501550); // I am not interested in this.
					return false;
				}
			}
			else if (IsHired)
			{
				SayTo(from, 1042495); // I have already been hired.
				return false;
			}

			return base.OnDragDrop(from, item);
		}

		public override bool CanBeRenamedBy(Mobile from)
		{
			return false;
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (!IsHired)
				list.Add(new HireEntry(from, this));
		}

		public class HireEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			private BaseHire m_Hire;

			public HireEntry(Mobile from, BaseHire hire)
				: base(6120, 3)
			{
				m_Hire = hire;
				m_Mobile = from;
			}

			public override void OnClick()
			{
				m_Hire.OnHire(m_Mobile);
			}
		}

		#region PayTimer
		private class PayTimer : Timer
		{
			private static TimeSpan InternalDelay = TimeSpan.FromMinutes(1.0);

			public PayTimer()
				: base(InternalDelay, InternalDelay)
			{
				Priority = TimerPriority.OneMinute;
			}

			private bool HasValidOwner(BaseHire Hire)
			{
				Mobile owner = Hire.ControlMaster;

				if (owner == null || owner.Deleted)
					return false;
				return true;
			}

			protected override void OnTick()
			{
				foreach (BaseHire hireNpc in m_HireNpcRegister)
				{
					if (hireNpc.IsHired)
					{
						if (HasValidOwner(hireNpc))
						{
							Mobile owner = hireNpc.ControlMaster;

							// Release non paid hirable with owner out of range
							if (!hireNpc.CanOrder && (owner.Map != hireNpc.Map || !owner.InRange(hireNpc.Location, 30)))
							{
								hireNpc.Say(1005653); // Hmmm.  I seem to have lost my master.
								hireNpc.OnRelease(false);
								return;
							}

							// Take Payment
							if (hireNpc.CanOrder && hireNpc.NextPayCheck < DateTime.Now)
							{
								hireNpc.TakeSalary();

								if (hireNpc.HoldGold < hireNpc.Salary)
								{
									hireNpc.SayTo(owner, 1043266); // My loyalty hath eroded, for lack of pay.
									hireNpc.OnRelease(false);
									return;
								}

								// TODO: Is this OSI correct?
								int iDaysLeft = (int)hireNpc.HoldGold / hireNpc.Salary;
								if (iDaysLeft <= 1)
									hireNpc.SayTo(owner, 1043267 + Utility.Random(3));
								// 1043267 My term of service is ending, unless I be paid more.
								// 1043268 'Tis crass of me, but I want gold.
								// 1043269 Methinks I shall quit my job soon.
							}
						}
						else // Owner not found
						{
							hireNpc.Say(1005653); // Hmmm.  I seem to have lost my master.
							hireNpc.OnRelease(false);
						}
					}
				}
			}
		}
		#endregion
	}
}