using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public abstract class PokerMachine : Item
	{
		public static readonly bool UseCalculateMoney = true;
		public static readonly TimeSpan CalculateMoneyDelay = TimeSpan.FromHours(2.0);
		public static List<PokerMachine> m_PokerMachineRegister = new List<PokerMachine>();

		public virtual int MaxBet { get { return 0; } }
		public virtual int MinBet { get { return 0; } }
		public virtual int BetChange { get { return 0; } }
		public virtual int[] m_WinningsTable { get { return null; } }

		#region PokerMachineTimer
		public static void Initialize()
		{
			if (UseCalculateMoney)
				new PokerMachineTimer().Start();
		}

		public class PokerMachineTimer : Timer
		{
			public PokerMachineTimer()
				: base(CalculateMoneyDelay, CalculateMoneyDelay)
			{
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				foreach (PokerMachine machine in m_PokerMachineRegister)
					Timer.DelayCall(TimeSpan.FromSeconds(Utility.Random(200)), new TimerCallback(machine.CalculateMoney));
			}
		}
		#endregion

		private int m_iGoldInMachine;
		[CommandProperty(AccessLevel.Administrator)]
		public int GoldInMachine
		{
			get { return m_iGoldInMachine; }
			set
			{
				if (value < 0)
					m_iGoldInMachine = 0;
				else
					m_iGoldInMachine = value;
				InvalidateProperties();
			}
		}

		public static readonly string DefaultMessage = "Welcome to Defiance AOS Poker Machine.";
		public static readonly Version CurrentVersion = new Version(1, 0, 0);

		public PokerMachine()
			: base(0x1173)
		{
			Movable = false;
			Name = "a Poker Machine";
			Hue = 0x58;
			m_iGoldInMachine = 30000 + ((Utility.Random(700) + 1) * 100);

			m_PokerMachineRegister.Add(this);
		}

		public void PlayGoldChangeSound()
		{
			Effects.PlaySound(Location, Map, 0x36); // Coin Sound
		}

		/// <summary>
		/// Calculates money in machine and adds more if needed.
		/// </summary>
		public void CalculateMoney()
		{
			if (m_iGoldInMachine < 50000)
			{
				int amountToAdd = 100000 - m_iGoldInMachine;
				m_iGoldInMachine += amountToAdd + Utility.Random(10000);
				this.InvalidateProperties();

				PublicOverheadMessage(MessageType.Regular, 0x0, true, "Calculating gold in machine");

				for (int i = 0; i <= 6; i += 2)
					Timer.DelayCall(TimeSpan.FromSeconds(i), new TimerCallback(PlayGoldChangeSound));
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1060658, "Bets\t{0}gp - {1}gp", MinBet, MaxBet); // ~1_val~: ~2_val~
			list.Add(1060659, "Jackpot\t{0}gp", m_iGoldInMachine/3); // ~1_val~: ~2_val~
		}

		public PokerMachine(Serial serial)
			: base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!from.InRange(GetWorldLocation(), 2))
				from.SendLocalizedMessage(500446); // That is too far away.
			else if (this.m_iGoldInMachine <= 0)
				from.SendMessage("This machine is out of gold.");
			else
			{
				Effects.PlaySound(this.Location, this.Map, 533);
				from.SendGump(new PokerGump(from, this, string.Format("{0} Version {1}", DefaultMessage, CurrentVersion.ToString())));
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
			writer.Write((int)m_iGoldInMachine);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			m_iGoldInMachine = reader.ReadInt();

			m_PokerMachineRegister.Add(this);
		}
	}
}