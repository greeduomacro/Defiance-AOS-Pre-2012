using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Gumps;
using Server;
using Server.Items;
using Server.Commands;
using Server.Engines.RewardSystem;

namespace Server.Events.LastManPentagram
{
	public class LastManPentagram : Item, ITimableEvent
	{
		private Timer m_Timer;

		private String m_Location;
		private bool m_giveReward;
		private int m_TimerTicksRequired; //to win. Timer ticks every 6 seconds, so we need 10 ticks tor 60 seconds time.
		private int m_Count;
		private PentagramAddon m_Addon;

		[CommandProperty(AccessLevel.GameMaster)]
		public String PentagramLocation { get { return m_Location; } set { m_Location = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool GiveReward { get { return m_giveReward; } set { m_giveReward = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int SecondsRequired
		{
			get { return m_TimerTicksRequired * 6; }
			set { m_TimerTicksRequired = value / 6; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Count { get { return m_Count; } set { m_Count = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Running
		{
			get { return m_Timer != null && m_Timer.Running; }
			set
			{
				if (value != Running)
				{
					if (Running)
					{
						if (m_Timer != null && m_Timer.Running)
							m_Timer.Stop();

						m_Timer = null;
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, "The pentagram event has been cancelled.");
						return;
					}

					else
					{
						m_Timer = new LMPTimer(this, m_Count, m_Location, m_giveReward, m_TimerTicksRequired);
						m_Timer.Start();
					}
				}
			}
		}

		[Constructable]
		public LastManPentagram()
			: base(0x1F1C)
		{
			m_Count = 10;
			m_TimerTicksRequired = 10;
			m_Addon = new PentagramAddon();
			Movable = false;
			Visible = false;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if(from.AccessLevel > AccessLevel.Player)
				from.SendGump(new PropertiesGump(from, this));
			base.OnDoubleClick(from);
		}

		public override void OnDelete()
		{
			if (m_Addon != null && !m_Addon.Deleted)
				m_Addon.Delete();
		}

		public override void OnLocationChange(Point3D oldLocation)
		{
			base.OnLocationChange(oldLocation);
			if (m_Addon != null && !m_Addon.Deleted)
				m_Addon.Location = Location;
		}

		public override void OnMapChange()
		{
			base.OnMapChange();
			if (m_Addon != null && !m_Addon.Deleted)
				m_Addon.Map = Map;
		}

		public LastManPentagram(Serial serial) : base(serial) { }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(2);//version
			//vestion 2:
			writer.WriteEncodedInt(m_TimerTicksRequired);
			//version 1:
			writer.Write(m_Location);
			writer.Write(m_giveReward);
			//version 0:
			writer.Write((Item)m_Addon);
			writer.Write(m_Count);
	   }

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				{
					m_TimerTicksRequired = reader.ReadEncodedInt();
					if (m_TimerTicksRequired == 0)
						m_TimerTicksRequired = 10;
					goto case 1;
				}
				case 1:
				{
					m_Location = reader.ReadString();
					m_giveReward = reader.ReadBool();
					goto case 0;
				}
				case 0:
				{
					m_Addon = reader.ReadItem() as PentagramAddon;
					m_Count = reader.ReadInt();
					break;
				}
			}
		}

		private class LMPTimer : Timer
		{
			private LastManPentagram m_LMP;
			private int m_Count;
			private String m_Location;
			private bool m_GetWinner;
			private Mobile m_Winner;
			private int m_ConsWinner;
			private Item m_Reward = null;
			private bool m_giveReward;
			private int m_TimerTicksRequired;

			public string GiveRewards( Mobile m )
			{
				// Edit by Silver: You can now get Statue + Reward
				int chance = Utility.Random( 1000 );
				bool giveStatue = ( Utility.Random( 1000 ) < 333 );
				string b_message = "";
				DateTime now = DateTime.Now;

				if( giveStatue )
				{
					switch ( Utility.Random( 7 ) )
					{
						case 0: m_Reward = new StatueSouth(); break;
						case 1: m_Reward = new StatueEast(); break;
						case 2: m_Reward = new StatueEast2(); break;
						case 3: m_Reward = new StatueNorth(); break;
						case 4: m_Reward = new StatueSouth2(); break;
						case 5: m_Reward = new StatueWest(); break;
						case 6: m_Reward = new StatueSouthEast(); break;
					}
					m_Reward.Name = m.Name + " - pentagram event winner - " + now.Day + "/" + now.Month + "/" + now.Year;
					m_Reward.LootType = LootType.Blessed;
					m.AddToBackpack( m_Reward );
				}

				m_Reward = null;

				if ( chance < 200 ) //20%
				{
					int bars = Utility.Random(2) + 3;
					EventRewardSystem.CreateCopperBar(m.Name, m.Backpack, bars, "Pentagram event");
					b_message = String.Format("{0} copper bars", bars);
				}
				else if( chance < 350 ) //15%
				{
					int amount = Utility.Random( 4, 3 )*5000;
					m_Reward = new BankCheck(amount);
					b_message = String.Format("a bank check worth {0} gold", amount);
				}
				else if( chance < 400 ) //5%
				{
					int bonus = Utility.Random(4)*5 + 10;
					m_Reward = new SkillBall(bonus);
					b_message = String.Format("a skillball training {0} skill points", bonus);
				}
				else if( chance < 500 ) //10%
				{
					switch (Utility.Random( 6 ))
					{
						case 0: m_Reward = new RunicSewingKit(CraftResource.HornedLeather, 4);	b_message = "4 uses horned runic sewing kit";	break;
						case 1: m_Reward = new RunicSewingKit(CraftResource.BarbedLeather, 2);	b_message = "2 uses barbed runic sewing kit";	break;
						case 2: m_Reward = new RunicHammer(CraftResource.Gold, 3);		b_message = "3 uses gold runic hammer";		break;
						case 3: m_Reward = new RunicHammer(CraftResource.Agapite, 2);		b_message = "2 uses agapite runic hammer"; 	break;
						case 4: m_Reward = new RunicHammer(CraftResource.Verite, 1);		b_message = "1 use verite runic hammer";	break;
						case 5: m_Reward = new RunicHammer(CraftResource.Valorite, 1);		b_message = "1 use valorite runic hammer";	break;
					}
				}
				else if( chance < 550 ) //5%
				{
					switch (Utility.Random( 31 ))
					{
						case 0: m_Reward = new LunaLance(); break;
						case 1: m_Reward = new VioletCourage(); break;
						case 2: m_Reward = new CavortingClub(); break;
						case 3: m_Reward = new NightsKiss(); break;
						case 4: m_Reward = new CaptainQuacklebushsCutlass(); break;
						case 5: m_Reward = new ShipModelOfTheHMSCape(); break;
						case 6: m_Reward = new AdmiralsHeartyRum(); break;
						case 7: m_Reward = new CandelabraOfSouls(); break;
						case 8: m_Reward = new IolosLute(); break;
						case 9: m_Reward = new GwennosHarp(); break;
						case 10: m_Reward = new ArcticDeathDealer(); break;
						case 11: m_Reward = new EnchantedTitanLegBone(); break;
						case 12: m_Reward = new NoxRangersHeavyCrossbow(); break;
						case 13: m_Reward = new BlazeOfDeath(); break;
						case 14: m_Reward = new DreadPirateHat(); break;
						case 15: m_Reward = new BurglarsBandana(); break;
						case 16: m_Reward = new GoldBricks(); break;
						case 17: m_Reward = new AlchemistsBauble(); break;
						case 18: m_Reward = new PhillipsWoodenSteed(); break;
						case 19: m_Reward = new PolarBearMask(); break;
						case 20: m_Reward = new BowOfTheJukaKing(); break;
						case 21: m_Reward = new GlovesOfThePugilist(); break;
						case 22: m_Reward = new OrcishVisage(); break;
						case 23: m_Reward = new StaffOfPower(); break;
						case 24: m_Reward = new ShieldOfInvulnerability(); break;
						case 25: m_Reward = new HeartOfTheLion(); break;
						case 26: m_Reward = new ColdBlood(); break;
						case 27: m_Reward = new GhostShipAnchor(); break;
						case 28: m_Reward = new SeahorseStatuette(); break;
						case 29: m_Reward = new WrathOfTheDryad(); break;
						case 30: m_Reward = new PixieSwatter(); break;
					}
						b_message = "a random minor artifact";
				}
				else if( chance < 600 ) //5%
				{
					m_Reward = new ClothingBlessDeed();
					b_message = "a clothing bless deed";
				}
				else if( chance < 700 ) //10%
				{
					m_Reward = new PowerScroll( PowerScroll.Skills[Utility.Random(PowerScroll.Skills.Count)], (100+(Utility.Random(3)+2)*5) );
					b_message = "a random powerscroll";
				}
				else if( chance < 750 ) //5%
				{
					Barracoon m_Champ = new Barracoon();
					m_Champ.MoveToWorld( m.Location, m.Map );
					b_message = "nothing, but managed to spawn Barracoon at the Pentagram!!!";
				}
				else if( chance < 800 ) //5%
				{
					m_Reward = new StatCapScroll( (225+(Utility.Random(3)+Utility.Random(2)+1)*5) );
					b_message = "a random statscroll";
				}
				else if( chance < 850 ) //5%
				{
					m_Reward = new MagicSewingKit();
					b_message = "a magic sewing kit";
				}
				else if( chance < 900 ) //5%
				{
					m_Reward = new BallOfSummoning();
					b_message = "a crystal ball of pet summoning";
				}
				else if( chance < 950 ) //5%
				{
					int UsesRemaining = Utility.Random(6,10);
					m_Reward = new PowderOfTemperament(UsesRemaining);
					b_message = String.Format("{0} uses of powder of fortifying", UsesRemaining);
				}
				else if( chance < 980 ) //3%
				{
					m_Reward = new PetBondingDeed();
					b_message = "a pet bonding deed";
				}
				else if( chance < 990 ) //1%
				{
					m_Reward = new NameChangeDeed();
					b_message = "a name change deed";
				}
				else //1%
				{
					m_Reward = new SexChangeDeed();
					b_message = "a sex change deed";
				}

				if (m_Reward != null)
				{
					m.AddToBackpack( m_Reward );
					Timer m_TimerCursed = new CursedArtifactSystem.CursedTimer( m_Reward, 6 );
					m_TimerCursed.Start();
				}

				if( giveStatue )
					b_message += " and a statue";

				return b_message;
			}

			public LMPTimer(LastManPentagram lmp, int count, String location, bool giveReward, int ticksRequired)
				: base(TimeSpan.FromSeconds(6.0), TimeSpan.FromSeconds(6.0))
			{
				m_LMP = lmp;
				m_Count = count * 10;
				m_Location = location;
				m_giveReward = giveReward;
				m_TimerTicksRequired = ticksRequired;
				CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The pentagram at {0} will be active in {1} minutes. The first one who stands on it alone for {2} seconds will receive a random reward. Hiding at this pentagram is deadly mistake.", location, count, ticksRequired * 6 ));
			}

			protected override void OnTick()
			{
				if (m_LMP == null || m_LMP.Deleted)
					Stop();

				string message = "";
				string sReward = "";

				if (!m_GetWinner)
				{
					m_Count--;

					if (m_Count < 1)
					{
						m_GetWinner = true;
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The pentagram at {0} is active!", m_Location));
					}

					else if (m_Count % 10 == 0)
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The pentagram at {0} will be active in {1} minutes. The first one who stands on it alone for {2} seconds will receive a random reward. Hiding at this pentagram is deadly mistake.", m_Location, m_Count / 10, m_TimerTicksRequired * 6));
				}

				else
				{
					List<Mobile> moblist = new List<Mobile>();

					foreach (Mobile m in m_LMP.Map.GetMobilesInRange(m_LMP.Location, 1)) // Edit by Silver: Changed range from 2 to 1
						if (m.Player && m.Alive && m.AccessLevel == AccessLevel.Player && m.Z + 2 >= m_LMP.Z && m.Z - 5 <= m_LMP.Z ) // Edit by Silver: Z-check
								moblist.Add(m);

						for (int i = moblist.Count -1; i >= 0; i-- )
						{
							Mobile m = moblist[i];
							if (m.Hidden)
							{
								m.Kill();
								moblist.RemoveAt(i);
							}
						}

					if (moblist.Count == 1)
					{
						if (moblist[0] == m_Winner)
						{
							if (m_ConsWinner == 0)
								CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, m_Winner.Name + String.Format(" was appointed as the sole person standing on the {0} pentagram!", m_Location));
							else
								message = m_Winner.Name + String.Format(" was appointed as the sole person standing on the {0} pentagram!", m_Location);
							m_ConsWinner++;
						}
						else
						{
							m_Winner = moblist[0];
							m_ConsWinner = 0;
						}

						if (m_ConsWinner >= m_TimerTicksRequired)
						{
							if (m_giveReward) {
								sReward = GiveRewards( m_Winner );
								CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, m_Winner.Name + " has won the pentagram event, and received " + sReward + ".");
							}
							else CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, m_Winner.Name + " has won the pentagram event.");
							message = "";
							Stop();
						}
					}

					else
					{
						m_ConsWinner = 0;
						if (Utility.Random(6) == 0)
							CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("No person could be appointed to be the only one standing on the {0} pentagram", m_Location));
						else
							message = String.Format("No person could be appointed to be the only one standing on the {0} pentagram", m_Location);
					}
				}

				List<Mobile> mobilelist = new List<Mobile>();
				foreach (Mobile m in m_LMP.Map.GetMobilesInRange(m_LMP.Location, 20))
					if (m.Player && m.Alive)
						mobilelist.Add(m);

				foreach (Mobile m in mobilelist)
				{
					if (!m.Criminal && m.AccessLevel == AccessLevel.Player)
						m.Criminal = true;

					if (message != "")
						m.SendMessage(message);
				}
			}
		}
	}
}