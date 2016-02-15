using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Misc
{
	public static class DelayedNewPlayerItems
	{
		public static List<PlayerMobile> NewPlayers = new List<PlayerMobile>();
		private static int m_Counter;

		public static void Configure()
		{
			new InternalTimer().Start();
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(Serialize), new DC.LoadMethod(Deserialize)), "NewPlayerItems");
		}

		public static void Tick()
		{
			m_Counter++;
			List<PlayerMobile> removal = new List<PlayerMobile>();

			foreach (PlayerMobile m in NewPlayers)
			{
				if(m == null)
					removal.Add(m);

				else if (m.GameTime >= TimeSpan.FromMinutes(10.0) && m.BankBox != null)
				{
					m.BankBox.DropItem(new BankCheck(2500));
					m.BankBox.DropItem(new BagOfAllReagents(50));

					m.SendMessage("Some gold and reagents have been deposited in your bank account.");
					removal.Add(m);
				}
			}

			if (m_Counter == 100)
			{
				m_Counter = 0;
				DateTime now = DateTime.Now;
				foreach (PlayerMobile m in NewPlayers)
				   if( m.CreationTime + TimeSpan.FromDays(1.0) > now)
					   removal.Add(m);
			}

			foreach (PlayerMobile pm in removal)
				NewPlayers.Remove(pm);
		}

		private static void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(NewPlayers.Count);
			foreach (Mobile m in NewPlayers)
				writer.Write(m);
		}

		private static void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			int count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				PlayerMobile m = reader.ReadMobile() as PlayerMobile;
				if(m != null)
					NewPlayers.Add((PlayerMobile)m);
			}
		}

		private class InternalTimer : Timer
		{
			public InternalTimer()
				: base(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0))
			{
			}

			protected override void OnTick()
			{
				DelayedNewPlayerItems.Tick();
			}
		}
	}
}