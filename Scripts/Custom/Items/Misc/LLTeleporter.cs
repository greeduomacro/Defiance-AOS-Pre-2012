using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells.Second;
using Server.Spells;

namespace Server.Items
{
	public class LLTeleporter : RestrictMoongate
	{
		[Constructable]
		public LLTeleporter()
		{
			AllowPets = false;
			StaffAccessCheck = true;
			AllowItemsOnChar = true;
			AllowRedChars = true;
			AllowBlueChars = true;
			AllowPetalOrange = true;
			AllowPetalTrinsic = true;
			AllowPolymorph = true;
			AllowStatMods = true;
			AllowSkillMods = true;
			AllowResistanceMods = true;
			AllowGhosts = false;
			HueOverride = -1;
			Dispellable = false;
			TargetMap = Map.Felucca;
			ItemID = 14170;
		}

		public LLTeleporter(Serial serial)
			: base(serial)
		{
		}

		private int m_GoldCost = 0;
		private bool m_Entrance;

		public override void UseGate(Mobile m)
		{
			if (StaffAccessCheck && m.AccessLevel < AccessLevel.Administrator && m.AccessLevel > AccessLevel.Player)
				m.SendLocalizedMessage(1019004); //You are not allowed to travel there.
			else if (TargetMap != null && TargetMap != Map.Internal)
			{
				m_Entrance = SpellHelper.IsFeluccaT2A(TargetMap, Target);

				if (!m.Alive)
				{
					m.SendLocalizedMessage(500590); //You're a ghost, and can't do that.
					return;
				}
				m_GoldCost = CalculateGoldCost(m);
				if (m_GoldCost < 0) //They have unallowed items.
					return;
				else if (Banker.GetBalance( m ) >= m_GoldCost)
				{
					string MessageString = string.Format("If you select OKAY, you will {0} the Lost Lands and insurance {1}. {2}<BR><BR>Are you sure you wish to pass?",
															m_Entrance ? "enter":"exit", m_Entrance? "will NOT work anymore, and all your blessed items will become regular" : "will work again", (m_GoldCost > 0) ? string.Format("You will be charged {0} gp for the items you are carrying (1000 gp per item).", m_GoldCost.ToString()) : "");
					int GumpHeight = 210;
					int GumpWidth = 300;
					int TitleNumber = 1019005;
					int TitleColor = 30720;
					int MessageColor = 32500;

					m.CloseGump( typeof( WarningGump ) );
					m.SendGump( new WarningGump( TitleNumber, TitleColor, (object)MessageString, MessageColor, GumpWidth, GumpHeight, new WarningGumpCallback( Warning_Callback ), m ) );
				}
				else
					m.SendMessage("You do not have enough gold in bank to use this gate. It costs 1000 gp per item, so you must either drop your items on the ground, or collect more gold in your bank.");
			}
			else
			{
				m.SendMessage("This moongate does not seem to go anywhere.");
			}
		}

		public int CalculateGoldCost(Mobile m)
		{
			int goldCost = 0;
			if ( m.AccessLevel == AccessLevel.Player && m.Backpack != null)
			{
				if (m.Holding != null)
					m.Backpack.DropItem(m.Holding);

				if (m.IsNaked())
					return 0;

				for (int i = m.Backpack.Items.Count; i > 0; i--)
				{
					Item item = m.Backpack.Items[i - 1];
					if (item is BaseArmor || item is BaseWeapon || item is BaseJewel || item is BaseHat || item is Spellbook || item is BookOfChivalry || item is NecromancerSpellbook || item is EtherealMount || item is Runebook)
						goldCost += 1000;
					else if (!m_Entrance)
						goldCost += 1000;
					else
					{
						m.SendMessage("You can only carry armors, weapons, jewels, spellbooks, runebooks and ethereal mounts through this gate.");
						return -1;
					}
				}
				for (int i = m.Items.Count - 1; i >= 0; --i)
				{
					Item item = (Item)m.Items[i];
					if (item is BaseArmor || item is BaseWeapon || item is BaseJewel || item is BaseHat || item is Spellbook || item is BookOfChivalry || item is NecromancerSpellbook)
						goldCost += 1000;
					else if ( item.Layer != Layer.Backpack & item.Layer != Layer.Bank &&
							item.Layer != Layer.FacialHair && item.Layer != Layer.Hair )
					{
						if (!m_Entrance)
							goldCost += 1000;
						else
						{
							m.SendMessage("You can only wear armors, weapons, jewels and spellbooks through this gate. Please remove all clothing.");
							return -1;
						}
					}
				}
			}
			return goldCost;
		}

		public void Warning_Callback( Mobile from, bool okay, object state )
		{
			if (!okay)
				return;
			base.UseGate( from );
		}

		public bool ChargePlayer(Mobile from)
		{
			int goldCost = CalculateGoldCost(from);
			if (goldCost < 0) //They have illegal items
				return false;
			else if (Banker.Withdraw(from, goldCost))
			{
				if (m_Entrance)
					{
					for (int i = from.Backpack.Items.Count; i > 0; i--)
					{
						Item item = from.Backpack.Items[i - 1];
						if (item is BaseHat && item.LootType == LootType.Blessed)
							item.LootType = LootType.Regular;
						else item.Insured = false;
					}
					for (int i = from.Items.Count - 1; i >= 0; --i)
					{
						Item item = (Item)from.Items[i];
						if (item is BaseHat && item.LootType == LootType.Blessed)
							item.LootType = LootType.Regular;
						else item.Insured = false;
					}
					from.SendMessage("The insurance of all your items has been cancelled and all your blessed armors are regular now.");
				}
				return true;
			}
			else
				from.SendLocalizedMessage(1062507); //You do not have that much money in your bank account.
			return false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

}