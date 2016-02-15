//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.1							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//

using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Factions;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.ContextMenus;
using Server.Engines.Quests;
using Server.Spells;
using Server.Spells.Necromancy;

namespace Server.Events.Duel
{
	public class Dueller : BaseQuester
	{
		#region Members
		public Mobile[] Participants;
		public Mobile Starter { get { return Participants[0]; } }
		public Mobile Opp1 { get { return Participants[1]; } }
		public Mobile Opp2 { get { return Participants[2]; } }
		public Mobile Ally { get { return Participants[3]; } }
		public bool MultiPlayer { get { return Participants.Length == 4; } }
		public DuelType DuelType;
		private bool m_Active, m_Running;
		public List<SpellWatcher> SpellWatchList = new List<SpellWatcher>();
		public Point3D[] Locations;//SpectLoc, walllocation, starterstartloc, opp1startloc, allystartloc, opp2startloc, starterendloc, opp1endloc, allyendloc, opp2endloc
		public bool NoPots, NoSummons, NoMounts, SpellWatch, NoArea;
		private bool m_NoMagWeps, m_NoPoisWeps, m_NoMagArmor, m_NoArts;
		private bool m_CheckTime, m_FactionOnly, m_SendGlobalMessages, m_RecordScores;
		private DuelBlockAddon blockadd0, blockadd1;
		private DuelRegion m_Region;
		private Item m_Wall;
		private int m_Wue,m_Wid;
		private Point3D m_AreaStart, m_AreaEnd;
		public Rectangle3D Area { get { return new Rectangle3D(m_AreaStart, m_AreaEnd); } }
		public Rectangle2D Area2D { get { return new Rectangle2D(new Point2D(m_AreaStart.X, m_AreaStart.Y), new Point2D(m_AreaEnd.X, m_AreaEnd.Y)); } }
		public bool Running { get { return m_Running; } }
		#endregion

		#region CommandProperties
		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D AreaEnd
		{
			get { return m_AreaEnd; }
			set { m_AreaEnd = value; if (m_AreaStart != Point3D.Zero) Utility.FixPoints(ref m_AreaStart, ref m_AreaEnd); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D AreaStart
		{
			get { return m_AreaStart; }
			set { m_AreaStart = value; if (m_AreaEnd != Point3D.Zero) Utility.FixPoints(ref m_AreaStart, ref m_AreaEnd); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int WallHue
		{
			get { return m_Wue; }
			set { m_Wue = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int WallID
		{
			get { return m_Wid; }
			set { m_Wid = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Active
		{
			get { return m_Active; }
			set { m_Active = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CheckTime
		{
			get { return m_CheckTime; }
			set { m_CheckTime = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool FactionOnly
		{
			get { return m_FactionOnly; }
			set { m_FactionOnly = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AnnounceOpening
		{
			get { return DuelSystem.AnnounceOpening; }
			set { DuelSystem.AnnounceOpening = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int OpeningHour1
		{
			get { return DuelSystem.OpeningHour1; }
			set { DuelSystem.OpeningHour1 = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int OpeningHour2
		{
			get { return DuelSystem.OpeningHour2; }
			set { DuelSystem.OpeningHour2 = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool SendMessages
		{
			get { return m_SendGlobalMessages; }
			set { m_SendGlobalMessages = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool RecordScores
		{
			get { return m_RecordScores; }
			set { m_RecordScores = value; }
		}
		#endregion

		#region Constructable
		[Constructable]
		public Dueller() : base( "The DuelMaster" )
		{
			InitStats(100, 100, 25);

			Hue = Utility.RandomSkinHue();

			Female = false;
			Direction = Direction.Down;
			Body = 0x190;
			Name = NameList.RandomName("male");
			Title = "The DuelMaster";

			AddItem(new Tunic(0x48D));
			AddItem(new LongPants(0x48D));
			AddItem(new SkullCap(0x48D));
			AddItem(new Boots());

			Item hair = new Item(Utility.RandomList(0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2049, 0x204A));
			hair.Hue = Utility.RandomHairHue();
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem(hair);

			Locations = new Point3D[10];
			DuelSystem.Duellers.Add(this);
		}
		public override bool ClickTitle { get { return true; } }
		public override void InitBody() { }
		public override void InitOutfit() { }
		public override bool OnMoveOver(Mobile m) { return true; }
		#endregion

		#region Numerators
		private enum Locs
		{
			Start = 1,
			End = 2,
			Spect = 3
		}
		#endregion

		#region MoveMob
		public void MoveMob(Mobile m, int loc)
		{
			int indexmob = Participants == null ? 0 : Array.IndexOf(Participants, m);
			Point3D Loc = Point3D.Zero;
			int index = 0;

			switch (loc)
			{
				case (int)Locs.Start: index = indexmob + 2; break;
				case (int)Locs.End: index = indexmob + 6; break;
				case (int)Locs.Spect: index = 0; break;
			}

			if (Locations.Length > index)
				Loc = Locations[index];

			if (Loc == Point3D.Zero || (IPoint3D)Loc == null)
				Loc = new Point3D(X + 22, Y, Z);

			m.MoveToWorld(Loc, Map);
		}


		#endregion

		#region SendGumps
		public override void OnTalk(PlayerMobile talker, bool contextMenu)
		{
			RequestDuelGump(talker);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from.AccessLevel > AccessLevel.GameMaster)
				from.SendGump(new DuelSetupGump(this));

			else
				RequestDuelGump(from);
		}

		private void RequestDuelGump(Mobile m)
		{
			if(!m_Active)
			{
				m.SendMessage("The Duel Arena is not activated.");
				return;
			}

			if (m_FactionOnly && Faction.Find(m) == null)
			{
				m.SendMessage("The Duel Arena is only open to faction members.");
				return;
			}

			else if (m_CheckTime)
			{
				DateTime datenow = DateTime.Now;
				int nowday = datenow.Day;
				int nowhour = datenow.Hour;
				bool open = OpeningHour1 == nowhour || OpeningHour2 == nowhour;

				if (!open)
				{
					int next = nowhour < OpeningHour1 ? OpeningHour1 : nowhour < OpeningHour2 ? OpeningHour2 : OpeningHour1;

					Say(String.Format("The Duel Arena will be open again from {0}:00 till {1}:00 GMT.", next, next + 1));
					return;
				}
			}

			CloseGumps(m);
			m.SendGump(new DuelStartGump(m, this));
		}

		public static void CloseGumps(Mobile m)
		{
			m.CloseGump(typeof(DuelStartGump));
			m.CloseGump(typeof(Duel2v2Gump));
			m.CloseGump(typeof(DuelAcceptGump));
			m.CloseGump(typeof(DuelStarterGump));
		}
		#endregion

		#region Imput
		public void Imput(DuelRune rune)
		{
			m_Running = true;
			Hidden = true;

			NoSummons = rune.Options[0];
			NoArea = rune.Options[1];
			NoMounts = rune.Options[2];
			NoPots = rune.Options[3];
			m_NoArts = rune.Options[4];
			m_NoMagArmor = rune.Options[5];
			m_NoMagWeps = rune.Options[6];
			m_NoPoisWeps = rune.Options[7];
			SpellWatch = rune.Options[8];

			Participants = rune.Participants;
			DuelType = rune.DType;

			MoveParticipants();
		}
		#endregion

		#region StartDuel
		private void MoveParticipants()
		{
			m_Region = new DuelRegion(this);
			m_Region.Register();

			List<Item> fieldstoremove = new List<Item>();

			IPooledEnumerable ip = Map.GetItemsInBounds(Area2D);

			foreach (Item item in ip)
				if (item != null && item.GetType().IsDefined(typeof(DispellableFieldAttribute), false))
					fieldstoremove.Add(item);

			ip.Free();

			foreach (Item item in fieldstoremove)
				if(!item.Deleted)
					item.Delete();

			for(int i = 0; i < Participants.Length; i++)
			{
				Mobile m = Participants[i];

					if (DuelType is DDDuelType)
					m.SendLocalizedMessage(1060398, DuelSystem.GoldCost2v2.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

				else
					m.SendLocalizedMessage(1060398, DuelSystem.GoldCost.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

				m.Frozen = true;
				m.Hidden = false;
				m.Map = Map;
				m.Hits = m.HitsMax;
				m.Mana = m.ManaMax;
				m.Stam = m.StamMax;

				TransformationSpellHelper.RemoveContext(m, true);
				for(int j = m.StatMods.Count -1; j >= 0; j--)
				{
					StatMod st = m.StatMods[j];
					if (st.Name.StartsWith("[Magic]"))
						m.StatMods.RemoveAt(j);
				}
				if (m.Spell != null && m.Spell.IsCasting)
					m.Spell.OnCasterHurt();
				Targeting.Target.Cancel(m);
				m.CloseGump(typeof(SummonFamiliarGump));
				List<Item> armthis = new List<Item>();
				MoveMob(m, 1);

				SunnySystem.Undress(m, ItemNotAllowed);

				if (SpellWatch)
				{
					SpellWatcher sw = new SpellWatcher(m);
					SpellWatchList.Add(sw);
				}

				if (DuelType is TMFDuelType || DuelType is UMFDuelType)
				{
					DuelBlockAddon dba = new DuelBlockAddon();
					if (m == Starter)
						blockadd0 = dba;
					else
						blockadd1 = dba;
					dba.MoveToWorld(m.Location, m.Map);

					if (DuelType is UMFDuelType)
					{
						armthis.Add(new Spellbook(ulong.MaxValue));
						if (m.Skills[SkillName.Magery].Value > 80)
						{
							NecromancerSpellbook book = new NecromancerSpellbook();
							book.Content = 0x1FFFF;
							armthis.Add(book);
						}
						GoldRing ring = new GoldRing();
						ring.Attributes.LowerRegCost = 100;
						ring.LootType = LootType.Cursed;
						armthis.Add(ring);
					}
				}

				if (DuelType is UDFDuelType)
				{
					armthis.Add(new Bandage(75));

					if (m.Skills[SkillName.Chivalry].Value > 80)
					{
						BookOfChivalry bookch = new BookOfChivalry();
						bookch.Content = 1023;
						armthis.Add(bookch);
					}

					if (m.Skills[SkillName.Necromancy].Value > 80)
					{
						NecromancerSpellbook book = new NecromancerSpellbook();
						book.Content = 0x1FFFF;
						armthis.Add(book);
					}

					GoldRing ring = new GoldRing();
					ring.Attributes.LowerRegCost = 100;
					armthis.Add(ring);

					if (m.Skills[SkillName.Fencing].Value > 80)
					{
						armthis.Add(new BladedStaff());
						armthis.Add(new DoubleBladedStaff());
						armthis.Add(new Pike());
						armthis.Add(new Pitchfork());
						armthis.Add(new ShortSpear());
						armthis.Add(new Spear());
						armthis.Add(new Kryss());
						armthis.Add(new WarFork());
					}

					if (m.Skills[SkillName.Swords].Value > 80)
					{
						//armthis.Add(new BoneHarvester());
						armthis.Add(new Broadsword());
						//armthis.Add(new Pike());
						armthis.Add(new CrescentBlade());
						//armthis.Add(new Cutlass());
						armthis.Add(new Katana());
						armthis.Add(new Longsword());
						armthis.Add(new Scimitar());
						//armthis.Add(new VikingSword());
						//armthis.Add(new Bardiche());
						//armthis.Add(new Halberd());
						armthis.Add(new BattleAxe());
						armthis.Add(new Pickaxe());
						//armthis.Add(new WarAxe());
					}

					if (m.Skills[SkillName.Macing].Value > 80)
					{
						armthis.Add(new Club());
						armthis.Add(new HammerPick());
						armthis.Add(new Mace());
						armthis.Add(new Maul());
						armthis.Add(new Scepter());
						armthis.Add(new WarHammer());
						armthis.Add(new WarMace());
					}

					if (m.Skills[SkillName.Archery].Value > 80)
					{
						armthis.Add(new Bolt(50));
						armthis.Add(new Arrow(50));
						armthis.Add(new Bow());
						armthis.Add(new CompositeBow());
						armthis.Add(new Crossbow());
						armthis.Add(new HeavyCrossbow());
						armthis.Add(new RepeatingCrossbow());
					}

					if (m.Skills[SkillName.Parry].Value > 80)
						armthis.Add(new MetalKiteShield());
				}

				if (armthis.Count > 0)
					SunnySystem.ArmPlayer(m, armthis);
			}

			m_Wall = new DuelWall();
			if (m_Wid != 0)
				m_Wall.ItemID = m_Wid;
			m_Wall.Hue = m_Wue;
			m_Wall.MoveToWorld(Locations[1], Map);

			new DuelTimer(this, 10, TimeSpan.FromSeconds(1.0)).Start();
		}
		#endregion

		#region ItemRelated
		public bool ItemNotAllowed(Item item)
		{
			if (DuelType is UMFDuelType || DuelType is UDFDuelType)
				return true;

			if (item is BaseClothing)
			{
				BaseClothing cloth = (BaseClothing)item;
				if (m_NoArts && cloth.ArtifactRarity > 0)
					return true;

				else if (m_NoMagArmor)
					if (!cloth.Attributes.IsEmpty || !cloth.ClothingAttributes.IsEmpty)
						return true;
			}

			if (item is BaseArmor)
			{
				BaseArmor armo = (BaseArmor)item;
				if (m_NoArts && armo.ArtifactRarity > 0)
					return true;

				else if (m_NoMagArmor)
					if (!armo.Attributes.IsEmpty || !armo.ArmorAttributes.IsEmpty)
						return true;
			}

			if (item is BaseJewel)
				if (m_NoArts && ((BaseJewel)item).ArtifactRarity > 0)
					return true;

			if (item is BaseWeapon)
			{
				BaseWeapon wep = (BaseWeapon)item;
				if (m_NoArts && wep.ArtifactRarity > 0)
					return true;

				else if (m_NoMagWeps)
					if (!wep.WeaponAttributes.IsEmpty || !wep.Attributes.IsEmpty)
						return true;

					else if (m_NoPoisWeps && wep.PoisonCharges > 0)
						return true;
			}
			return false;
		}
		#endregion

		#region EndDuel
		public void GiveReward(int won)
		{
			if (m_Region != null)
				m_Region.Unregister();

			for (int i = 0; i < Participants.Length; i++ )
			{
				Mobile m = Participants[i];
				MoveMob(m, 2);
				m.Resurrect();
				m.Hits = m.HitsMax;
				m.Criminal = false;
				if (DuelType is DDDuelType)
					Banker.Withdraw(m, DuelSystem.GoldCost2v2);

				else
					Banker.Withdraw(m, DuelSystem.GoldCost);

				if (DuelType is UDFDuelType || DuelType is UMFDuelType)
					SunnySystem.DisArmPlayer(m);
				SunnySystem.ReDress(m);
			}

			Mobile[] winarr = new Mobile[2];
			Mobile[] losearr = new Mobile[2];

			switch (won)
			{
				case 0: //draw
					foreach (Mobile m in Participants)
					{
						GiveCheck(m, (int)(DuelSystem.GoldCost * 0.7));
						m.SendGump(new DuelEndGump(true, true));
					}
					break;

				case 1:
					winarr[0] = Opp1;
					losearr[0] = Starter;

					if (MultiPlayer)
					{
						winarr[1] = Opp2;
						losearr[1] = Ally;
					}
					break;

				case 2:
					winarr[0] = Starter;
					losearr[0] = Opp1;

					if (MultiPlayer)
					{
						winarr[1] = Ally;
						losearr[1] = Opp2;
					}
					break;
			}

			foreach (Mobile m in winarr)
			{
				if (m != null)
				{
					m.SendGump(new DuelEndGump(true, false));
					GiveCheck(m, (int)(DuelSystem.GoldCost * 1.4));
				}
			}

			foreach (Mobile m in losearr)
				if (m != null)
					m.SendGump(new DuelEndGump(false, false));

			if (won != 0)
			{
				string bcm = "";
				string bcmf = "";

				if (MultiPlayer)
				{
					switch (Utility.Random(6))
					{
						default:
						case 0: bcm = "{0} and {1} have beaten {2} and {3} in a 2vs2 duel."; break;
						case 1: bcm = "{0} and {1} have slain {2} and {3} in a teamfight."; break;
						case 2: bcm = "{0} and {1} managed to kill {2} and {3} in the duel arena."; break;
						case 3: bcm = "{2} and {3} have been utterly beaten by {0} and {1}."; break;
						case 4: bcm = "{2} and {3} were smashed by {0} and {1} in the Duel Arena."; break;
						case 5: bcm = "{0} and {1} defeated {2} and {3} in a duel."; break;
					}
					if (winarr[0] != null && winarr[1] != null && losearr[0] != null && losearr[1] != null)
						bcmf = String.Format(bcm, winarr[0].Name, winarr[1].Name, losearr[0].Name, losearr[1].Name);
				}

				else
				{
					switch (Utility.Random(8))
					{
						default:
						case 0: bcm = "{0} has been utterly beaten in a duel versus {1}."; break;
						case 1: bcm = "{1} has slain {0} in an honorable fight."; break;
						case 2: bcm = "{1} defeated {0} in a duel."; break;
						case 3: bcm = "{1} won a victorious battle over {0}."; break;
						case 4: bcm = "{1} managed to kill {0} in a duel."; break;
						case 5: bcm = "{1} defeated {0} in the Duel Arena."; break;
						case 6: bcm = "{0} was smashed by {1} in a duel."; break;
						case 7: bcm = "{0} was owned by {1} in the duel arena."; break;
					}
					if (winarr[0] != null && losearr[0] != null)
						bcmf = String.Format(bcm, losearr[0].Name, winarr[0].Name);

					if (m_RecordScores)
					{
						if (won == 1)
							DuelScoreSystem.UpdateScore(Opp1, Starter, DuelType.TypeNumber);

						else if (won == 2)
							DuelScoreSystem.UpdateScore(Starter, Opp1, DuelType.TypeNumber);
					}
				}

				if(m_SendGlobalMessages)
					foreach (NetState ns in NetState.Instances)
					{
						Mobile m = ns.Mobile;

						if (m != null && m.AccessLevel == AccessLevel.Player && !(m.Region is Server.Regions.Jail))
						{
							m.SendMessage(0x482, String.Format("Message from the Duelmaster:"));
							m.SendMessage(0x482, String.Format(bcmf));
						}
					}
			}

			ArrayList Corpseslist = new ArrayList();
			foreach (Item item in GetItemsInRange(12))
				if (item is Corpse)
					Corpseslist.Add(item);

			foreach (Corpse corpse in Corpseslist)
			{
				corpse.Location = corpse.Owner.Location;
				corpse.Map = corpse.Owner.Map;
			}

			Participants = null;
			if (m_Wall != null)
				m_Wall.Delete();
			if (blockadd0 != null)
				blockadd0.Delete();
			if (blockadd1 != null)
				blockadd1.Delete();
			m_Running = false;
			Hidden = false;

		   SpellWatchList.Clear();
		}

		private static void GiveCheck(Mobile m, int amount)
		{
			Container pack = m.Backpack;

			if (pack != null)
			{
				BankCheck check = (BankCheck)pack.FindItemByType(typeof(BankCheck));
				if (check == null || check.Worth >= 1000000 - amount)
					m.AddToBackpack(new BankCheck(amount));

				else
					check.Worth += amount;
			}
		}
		#endregion

		public override void OnDelete()
		{
			if (m_Region != null)
				m_Region.Unregister();
			if (m_Wall != null)
				m_Wall.Delete();

			DuelSystem.Duellers.Remove(this);

			base.OnDelete();
		}

		#region Serializers
		public Dueller( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write(0);//version

			writer.Write(m_Active);
			writer.Write(m_RecordScores);
			writer.Write(m_SendGlobalMessages);

			writer.Write(m_Wue);
			writer.Write(m_Wid);
			writer.Write(m_Running);
			writer.Write(m_CheckTime);
			writer.Write(m_FactionOnly);
			writer.Write(m_AreaStart);
			writer.Write(m_AreaEnd);

			for (int i = 0; i < 10; i++)
				writer.Write(Locations[i]);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			m_Active = reader.ReadBool();
			m_RecordScores = reader.ReadBool();
			m_SendGlobalMessages = reader.ReadBool();
			m_Wue = reader.ReadInt();
			m_Wid = reader.ReadInt();
			if (reader.ReadBool() == true)
				Hidden = false;
			m_CheckTime = reader.ReadBool();
			m_FactionOnly = reader.ReadBool();
			m_AreaStart = reader.ReadPoint3D();
			m_AreaEnd = reader.ReadPoint3D();

			Locations = new Point3D[10];
			for (int i = 0; i < 10; i++)
				Locations[i] = reader.ReadPoint3D();

			DuelSystem.Duellers.Add(this);
		}
		#endregion

		#region DuelTimer
		private class DuelTimer : Timer
		{
			private int m_TimeLeft;
			private bool m_CountDown;
			private Dueller m_Npc;

			public DuelTimer(Dueller duelnpc, int duration, TimeSpan interval) : base(interval, interval)
			{
				m_TimeLeft = duration;
				m_Npc = duelnpc;

				if (interval == TimeSpan.FromSeconds(1.0))
					m_CountDown = true;
			}

			protected override void OnTick()
			{
				m_TimeLeft--;

				if (m_CountDown && m_Npc != null)
				{
					if (m_TimeLeft > 0)
					{
						foreach (Mobile m in m_Npc.Participants)
						{
							m.SendMessage("Get ready, the duel will start in {0} seconds!", m_TimeLeft);
						}
					}

					else
					{
						foreach (Mobile m in m_Npc.Participants)
						{
							m.SendMessage("Get ready, the duel starts now!");
							m.Frozen = false;
							m.Criminal = true;
						}

						new DuelTimer(m_Npc, 30, TimeSpan.FromSeconds(10.0)).Start();

						Stop();
					}
				}

				else
				{
					if (m_Npc.MultiPlayer)
					{
						if (!m_Npc.Opp1.Alive && !m_Npc.Opp2.Alive && !m_Npc.Ally.Alive && !m_Npc.Starter.Alive)
							SendRewardInfo(0);

						else if (!m_Npc.Ally.Alive && !m_Npc.Starter.Alive)
							SendRewardInfo(1);

						else if (!m_Npc.Opp1.Alive && !m_Npc.Opp2.Alive)
							SendRewardInfo(2);
					}

					else if (!m_Npc.Starter.Alive && !m_Npc.Opp1.Alive)
						SendRewardInfo(0);

					else if (!m_Npc.Starter.Alive)
						SendRewardInfo(1);

					else if (!m_Npc.Opp1.Alive)
						SendRewardInfo(2);

					else if (m_TimeLeft == 0)
					{
						foreach (Mobile m in m_Npc.Participants)
							m.SendMessage("The duel has ended, but noone won, a part of your money will be returned.");

						SendRewardInfo(0);
					}

					else if (m_TimeLeft % 6 == 0) //each minute
					{
						foreach (Mobile m in m_Npc.Participants)
							m.SendMessage("The duel ends in {0} minutes!", (m_TimeLeft / 6));
					}
				}
			}

			private void SendRewardInfo(int winner)
			{
				m_Npc.GiveReward(winner);
				Stop();
			}
		}
		#endregion
	}
}