using System;
using System.Collections.Generic;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using Server.Engines.RewardSystem;

namespace Server.Events.Riddle
{
	public class RiddleNPC : BaseQuester
	{
		public static List<Riddle> Riddles = new List<Riddle>();
		private TimeSpan m_HiddenDuration = TimeSpan.FromHours(Utility.RandomMinMax(20, 30));
		private Timer m_HidingExpireTimer;

		[Constructable]
		public RiddleNPC()
			: base("the Riddler")
		{
			InitStats(100, 100, 25);
			AccessLevel = AccessLevel.Seer;

			Hue = Utility.RandomSkinHue();

			Body = 0x190;
			Name = NameList.RandomName("male");
			AddItem(new FancyShirt(Utility.RandomBlueHue()));
			AddItem(new Kilt(Utility.RandomGreenHue()));
			AddItem(new FeatheredHat(Utility.RandomGreenHue()));
			AddItem(new ThighBoots());

			Utility.AssignRandomHair(this);
		}

		public override bool ClickTitle { get { return true; } }
		public override void InitBody() { }
		public override void InitOutfit() { }

		public static void Configure()
		{
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(WriteRiddles), new DC.LoadMethod(ReadRiddles)),"riddles");
		}

		private static void WriteRiddles(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(Riddles.Count);
			foreach (Riddle r in Riddles)
			{
				writer.Write(r.Question);
				writer.Write(r.Answer);
				writer.Write(r.RewardAmount);
			}
		}

		private static void ReadRiddles(GenericReader reader)
		{
			int version = reader.ReadInt();

			int count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				Riddles.Add(new Riddle(reader) );
			}
		}

		public override void OnTalk(PlayerMobile talker, bool contextMenu)
		{
			if(talker.AccessLevel >= AccessLevel.Administrator)
				talker.SendGump(new RiddlesGump() );

			else if (talker.AccessLevel == AccessLevel.Player)
			{
				if (Riddles.Count > 0)
				{
					Say(Riddles[0].Question);
					talker.Prompt = new RiddlePrompt(this);
				}
				else
				{
					Say("Currently i am out of things to solve, come back later i may have something for you.");
					Hidden = true;
					Effects.SendLocationEffect( new Point3D( this.X, this.Y, this.Z ), this.Map, 0x36BD, 10 );
					m_HiddenDuration = TimeSpan.FromHours(Utility.RandomMinMax(20, 30));
					m_HidingExpireTimer = new HidingExpireTimer(this);
					m_HidingExpireTimer.Start();
				}

			}
		}

		private class RiddlePrompt : Prompt
		{
			private RiddleNPC m_Riddler;
			public RiddlePrompt(RiddleNPC riddler)
			{
				m_Riddler = riddler;
			}

			public override void OnResponse(Mobile m, string text)
			{
				text = text.Trim();
				if (m != null)
					RiddleNPC.TryAnswer(m, text, m_Riddler);
			}
		}

		public static void TryAnswer(Mobile m, string text, RiddleNPC riddler)
		{
			if (Riddles.Count > 0 && m.Backpack != null)
			{
				if (text.ToLower() == Riddles[0].Answer.ToLower())
				{
					if (riddler != null)
					{
						riddler.BCast(m);
						riddler.m_HiddenDuration = TimeSpan.FromHours(Utility.RandomMinMax(20, 30));
						riddler.m_HidingExpireTimer = new HidingExpireTimer(riddler);
						riddler.m_HidingExpireTimer.Start();
						Effects.SendLocationEffect( new Point3D( riddler.X, riddler.Y, riddler.Z ), riddler.Map, 0x36BD, 10 );
						riddler.Hidden = true;
					}
					m.SendMessage("Congratulations you have solved the riddle!");
					EventRewardSystem.CreateCopperBar(m.Name, m.Backpack, Riddles[0].RewardAmount, "solved a riddle");
					Riddles.RemoveAt(0);
				}
				else
					m.SendMessage("I am sorry that is not the correct answer, try again.");
			}
		}

		private void BCast(Mobile player)
		{
			foreach (NetState ns in NetState.Instances)
			{
				Mobile m = ns.Mobile;
				if (m != null)
				{
					m.SendMessage(0x482, "Message from the Riddler:");
					m.SendMessage(0x482, String.Format("{0} just solved the following riddle: {1}", player.Name, Riddles[0].Question));
					m.SendMessage(0x482, String.Format("The answer was: '{0}'. Expect more riddles when you see me again.", Riddles[0].Answer));
				}
			}

		}

		private class HidingExpireTimer : Timer
		{
			private RiddleNPC m_RiddleMan;
			public HidingExpireTimer( RiddleNPC r) : base( r.m_HiddenDuration )
			{
				m_RiddleMan = r;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				if (m_RiddleMan != null && m_RiddleMan.Hidden && RiddleNPC.Riddles.Count > 0)
				{
					m_RiddleMan.Hidden = false;
					Effects.SendLocationEffect( new Point3D( m_RiddleMan.Location ), m_RiddleMan.Map, 0x36BD, 10 );
					foreach (NetState ns in NetState.Instances)
					{
						Mobile m = ns.Mobile;
						if (m != null)
							m.SendMessage(0x482, "The Riddler is back.");
					}
				}
				Stop();
			}
		}

		public RiddleNPC( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}

	public class Riddle
	{
		public string Question;
		public string Answer;
		public int RewardAmount;

		public Riddle(string q, string a, int rew)
		{
			Question = q;
			Answer = a;
			RewardAmount = rew;
		}

		public Riddle(GenericReader reader)
		{
			Question = reader.ReadString();
			Answer = reader.ReadString();
			RewardAmount = reader.ReadInt();
		}
	}
}