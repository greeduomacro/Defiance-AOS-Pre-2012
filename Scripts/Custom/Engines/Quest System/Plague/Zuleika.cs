using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Engines.Quests;

namespace Server.Engines.Quests.Plauge
{
	public class Zuleika : BaseQuester
	{
		public override bool ClickTitle { get { return true; } }
		public override bool IsActiveVendor { get { return true; } }
		public override bool DisallowAllMoves { get { return false; } }

		public override void InitSBInfo()
		{
			m_SBInfos.Add(new SBZuleika());
		}

		[Constructable]
		public Zuleika()
			: base("the Necromancer")
		{
			Name = "Zuleika";
		}

		public Zuleika(Serial serial)
			: base(serial)
		{
		}

		public override void InitBody()
		{
			InitStats(100, 100, 25);
			Body = 0x191;
			Female = true;
		}

		private PlagueSummoningAltar m_Altar;
		[CommandProperty(AccessLevel.GameMaster)]
		public PlagueSummoningAltar Altar { get { return m_Altar; } set { m_Altar = value; } }

		public override void InitOutfit()
		{
			HairItemID = 0x203C; // Long
			HairHue = 0x457;

			EquipItem(new Robe(0x455));

			EquipItem(new Sandals(0x455));

			GnarledStaff staff = new GnarledStaff();
			staff.Hue = 0x455;
			EquipItem(staff);

			GoldBeadNecklace necklace = new GoldBeadNecklace();
			necklace.Hue = 0x455;
			EquipItem(necklace);

			EquipItem(new GoldBracelet());
			EquipItem(new GoldRing());

			EquipItem(new HoodedShroudOfShadows(0x593));
			EquipItem(new BlackStaff());
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			PlayerMobile player = from as PlayerMobile;

			if (player != null)
			{
				QuestSystem qs = player.Quest;

				if (qs is ThePlaugeQuest)
				{
					if (dropped is PoisonFungus)
					{
						PoisonFungus fungus = (PoisonFungus)dropped;

						QuestObjective obj = qs.FindObjective(typeof(CollectFungusObjective));

						if (obj != null && !obj.Completed)
						{
							int need = obj.MaxProgress - obj.CurProgress;

							if (fungus.Amount < need)
							{
								obj.CurProgress += fungus.Amount;
								fungus.Delete();

								qs.ShowQuestLogUpdated();
							}
							else
							{
								obj.Complete();
								fungus.Consume(need);

								if (!fungus.Deleted)
								{
									SayTo(from, "You have already given me all the Poison fungus necessary to weave the spell.  Keep these for a later time.");
								}
							}
						}
						else
						{
							SayTo(from, "You have already given me all the Poison fungus necessary to weave the spell.  Keep these for a later time.");
						}

						return false;
					}
				}
			}

			return base.OnDragDrop(from, dropped);
		}

		public void MovePlayers(Mobile from)
		{
			Party p = PartySystem.Party.Get(from);

			if (p != null)
			{
				for (int i = 0; i < p.Members.Count; ++i)
				{
					PartyMemberInfo pmi = (PartyMemberInfo)p.Members[i];
					Mobile member = pmi.Mobile;

					if (member != from && member.Map == Map.Felucca && member.Region == from.Region)
					{
						member.CloseGump(typeof(ZuleikaPartyGump));
						member.SendGump(new ZuleikaPartyGump(from, member));
					}
				}
			}

			Teleport(from);
		}

		public static void Teleport(Mobile from)
		{
			Point3D loc = new Point3D(6093, 441, -22);
			Map map = Map.Trammel;

			Effects.SendLocationParticles(EffectItem.Create(loc, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 0, 0, 2023, 0);
			Effects.PlaySound(loc, map, 0x1FE);

			BaseCreature.TeleportPets(from, loc, map);

			from.MoveToWorld(loc, map);
		}

		public override void OnTalk(PlayerMobile player, bool contextMenu)
		{
			QuestSystem qs = player.Quest;

			// Doing this quest
			if (qs is ThePlaugeQuest)
			{
				if (qs.IsObjectiveInProgress(typeof(CollectFungusObjective)))
				{
					qs.AddConversation(new DuringCollectFungusConversation());
				}
			}
			else
			{
				// Busy with another quest
				if (qs != null)
				{
					qs.AddConversation(new DontOfferConversation());
				}

					// Offer Quest
				else if (qs == null && QuestSystem.CanOfferQuest(player, typeof(ThePlaugeQuest)))
				{
					Direction = GetDirectionTo(player);
					new ThePlaugeQuest(this, player).SendOffer();
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version

			writer.Write(m_Altar);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_Altar = reader.ReadItem() as PlagueSummoningAltar;
						break;
					}
			}
		}
	}

	public class ZuleikaPartyGump : Gump
	{
		private Mobile m_Leader;
		private Mobile m_Member;
		private Region m_Region;

		public ZuleikaPartyGump(Mobile leader, Mobile member)
			: base(150, 50)
		{
			m_Leader = leader;
			m_Member = member;
			m_Region = member.Region;

			Closable = false;

			AddPage(0);

			AddImage(0, 0, 3600);

			AddImageTiled(0, 14, 15, 200, 3603);
			AddImageTiled(380, 14, 14, 200, 3605);
			AddImage(0, 201, 3606);
			AddImageTiled(15, 201, 370, 16, 3607);
			AddImageTiled(15, 0, 370, 16, 3601);
			AddImage(380, 0, 3602);
			AddImage(380, 201, 3608);
			AddImageTiled(15, 15, 365, 190, 2624);

			AddRadio(30, 140, 9727, 9730, true, 1);
			AddHtmlLocalized(65, 145, 300, 25, 1050050, 0x7FFF, false, false); // Yes, let's go!

			AddRadio(30, 175, 9727, 9730, false, 0);
			AddHtmlLocalized(65, 178, 300, 25, 1050049, 0x7FFF, false, false); // No thanks, I'd rather stay here.

			AddHtml(30, 20, 360, 35, HtmlUtility.Color("Another player has paid Zuleika for your passage to the Alternate Dimension:", HtmlUtility.HtmlWhite), false, false);

			AddHtmlLocalized(30, 105, 345, 40, 1050048, 0x5B2D, false, false); // Do you wish to accept their invitation at this time?

			AddImage(65, 72, 5605);

			AddImageTiled(80, 90, 200, 1, 9107);
			AddImageTiled(95, 92, 200, 1, 9157);

			AddLabel(90, 70, 1645, leader.Name);

			AddButton(290, 175, 247, 248, 2, GumpButtonType.Reply, 0);

			AddImageTiled(15, 14, 365, 1, 9107);
			AddImageTiled(380, 14, 1, 190, 9105);
			AddImageTiled(15, 205, 365, 1, 9107);
			AddImageTiled(15, 14, 1, 190, 9105);
			AddImageTiled(0, 0, 395, 1, 9157);
			AddImageTiled(394, 0, 1, 217, 9155);
			AddImageTiled(0, 216, 395, 1, 9157);
			AddImageTiled(0, 0, 1, 217, 9155);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID == 2 && info.IsSwitched(1))
			{
				if (m_Member.Region == m_Region)
				{
					m_Leader.SendMessage("{0} has accepted your invitation to come to the Alternate Dimension.", m_Member.Name);

					Zuleika.Teleport(m_Member);
				}
				else
				{
					m_Member.SendLocalizedMessage(1050051); // The invitation has been revoked.
				}
			}
			else
			{
				m_Member.SendLocalizedMessage(1050052); // You have declined their invitation.
				m_Leader.SendMessage("{0} has declined your invitation to come to the Alternate Dimension.", m_Member.Name);
			}
		}
	}
}