using System;
using Server;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class CCKey : Item
	{
		[Constructable]
		public CCKey() : base(0x1012)
		{
		}

		public CCKey(Serial serial) : base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!this.IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
				return;
			}

			from.SendLocalizedMessage(501680); // What do you want to unlock?
			from.Target = new CCKeyTarget(this);
		}

		private class CCKeyTarget : Target
		{
			private CCKey m_Key;

			public CCKeyTarget(CCKey key) : base(3, false, TargetFlags.None)
			{
				m_Key = key;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Key.Deleted || !m_Key.IsChildOf(from.Backpack) )
					from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
				else if ( targeted is CCMagicDoor )
				{
					CCMagicDoor magicDoor = (CCMagicDoor)targeted;

					if ( magicDoor.IsInside( from ) )
						from.LocalOverheadMessage( MessageType.Regular, 0x5A, true, "There is no keyhole on this side of the door!" );
					else
					{
						Party p = Party.Get( from );

						if ( p != null )
						{
							for (int i = 0; i < p.Members.Count; ++i)
							{
								PartyMemberInfo pmi = (PartyMemberInfo)p.Members[i];
								Mobile member = pmi.Mobile;

								if ( member != from && member.Map == Map.Felucca && member.Region.IsPartOf("Cursed Cave Level 3") )
								{
									member.CloseGump(typeof(MagicDoorPartyGump));
									member.SendGump(new MagicDoorPartyGump(magicDoor, from, member));
								}
							}
						}

						if ( magicDoor.Active )
						{
							magicDoor.DoTeleport( from );
							from.LocalOverheadMessage( MessageType.Regular, 0x5A, true, "The key disappears and you are teleported into the room." );
							m_Key.Delete();

							if ( magicDoor.Altar != null )
								magicDoor.Altar.MainQueue++;
						}
					}
				}
				else
					from.SendLocalizedMessage(501666); // You can't unlock that!
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class MagicDoorPartyGump : Gump
	{
		private CCMagicDoor m_MagicDoor;
		private Mobile m_Leader;
		private Mobile m_Member;

		public MagicDoorPartyGump( CCMagicDoor door, Mobile leader, Mobile member ) : base( 150, 50 )
		{
			m_MagicDoor = door;
			m_Leader = leader;
			m_Member = member;

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

			AddHtml(30, 20, 360, 35, HtmlUtility.Color("Another player has used a key to enter the God of Fire's chamber:", HtmlUtility.HtmlWhite), false, false);

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
				if ( m_Member.Region.IsPartOf("Cursed Cave Level 3") )
				{
					m_Leader.SendMessage( "{0} has accepted your invitation.", m_Member.Name );

					if ( m_MagicDoor != null )
						m_MagicDoor.DoTeleport( m_Member );
				}
				else
					m_Member.SendLocalizedMessage( 1050051 ); // The invitation has been revoked.
			}
			else
			{
				m_Member.SendLocalizedMessage(1050052); // You have declined their invitation.
				m_Leader.SendMessage( "{0} has declined your invitation.", m_Member.Name );
			}
		}
	}
}