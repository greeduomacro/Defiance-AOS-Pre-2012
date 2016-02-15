using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	#region BaseEvilDecor
	public abstract class BaseEvilDecorDeed : BaseAddonDeed
	{
		private bool m_East;
		public bool East
		{
			get { return m_East; }
			set { m_East = value; }
		}

		public BaseEvilDecorDeed()
		{
			LootType = LootType.Blessed;
		}

		public virtual void SendTarget(Mobile m)
		{
			base.OnDoubleClick(m);
		}

		public BaseEvilDecorDeed(Serial serial)
			: base(serial)
		{
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

	public abstract class BaseEvilDecorGump : Gump
	{
		private BaseEvilDecorDeed m_Deed;

		public BaseEvilDecorGump(BaseEvilDecorDeed deed)
			: base(150, 50)
		{
			m_Deed = deed;
			AddBackground(0, 0, 350, 250, 0xA28);

			AddButton(70, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0); // South
			AddButton(185, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0); // East
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (m_Deed.Deleted || info.ButtonID == 0)
				return;

			m_Deed.East = (info.ButtonID != 1);
			m_Deed.SendTarget(sender.Mobile);
		}
	}
	#endregion

	#region BoneThrone
	public class BoneThrone : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new BoneThroneDeed(); } }

		[Constructable]
		public BoneThrone(bool east)
		{
			if (east)
			{
				AddComponent(new LocalizedAddonComponent(0x2A59, 1074476), 0, 0, 0);
			}
			else
			{
				AddComponent(new LocalizedAddonComponent(0x2A58, 1074476), 0, 0, 0);
			}
		}

		public BoneThrone(Serial serial)
			: base(serial)
		{
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

	public class BoneThroneDeed : BaseEvilDecorDeed
	{
		public override BaseAddon Addon { get { return new BoneThrone(East); } }
		public override int LabelNumber { get { return 1074476; } } // Bone throne

		[Constructable]
		public BoneThroneDeed()
		{
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (IsChildOf(from.Backpack))
			{
				from.CloseGump(typeof(BoneThroneGump));
				from.SendGump(new BoneThroneGump(this));
			}
			else
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
		}

		public BoneThroneDeed(Serial serial)
			: base(serial)
		{
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

	public class BoneThroneGump : BaseEvilDecorGump
	{
		public BoneThroneGump(BaseEvilDecorDeed deed)
			: base(deed)
		{
			// South
			AddItem(90, 52, 0x2A58);

			// East
			AddItem(220, 52, 0x2A59);
		}
	}
	#endregion

	#region BoneCouch
	public class BoneCouch : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new BoneCouchDeed(); } }

		[Constructable]
		public BoneCouch(bool east)
		{
			if (east)
			{
				AddComponent(new LocalizedAddonComponent(0x2A7F, 1074477), 0, 0, 0);
				AddComponent(new LocalizedAddonComponent(0x2A80, 1074477), 0, -1, 0);
			}
			else
			{
				AddComponent(new LocalizedAddonComponent(0x2A5A, 1074477), 0, 0, 0);
				AddComponent(new LocalizedAddonComponent(0x2A5B, 1074477), -1, 0, 0);
			}
		}

		public BoneCouch(Serial serial)
			: base(serial)
		{
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

	public class BoneCouchDeed : BaseEvilDecorDeed
	{
		public override BaseAddon Addon { get { return new BoneCouch(East); } }
		public override int LabelNumber { get { return 1074477; } } // Bone couch

		[Constructable]
		public BoneCouchDeed()
		{
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (IsChildOf(from.Backpack))
			{
				from.CloseGump(typeof(BoneCouchDeedGump));
				from.SendGump(new BoneCouchDeedGump(this));
			}
			else
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
		}

		public BoneCouchDeed(Serial serial)
			: base(serial)
		{
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

	public class BoneCouchDeedGump : BaseEvilDecorGump
	{
		public BoneCouchDeedGump(BaseEvilDecorDeed deed)
			: base(deed)
		{
			// South
			AddItem(90, 52, 0x2A5B);
			AddItem(109, 77, 0x2A5A);

			// East
			AddItem(220, 35, 0x2A80);
			AddItem(204, 50, 0x2A7F);
		}
	}
	#endregion

	#region BoneTable
	public class BoneTableAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new BoneTableDeed(); } }

		[Constructable]
		public BoneTableAddon()
		{
			AddComponent(new LocalizedAddonComponent(0x2A5C, 1074478), 0, 0, 0);
		}

		public BoneTableAddon(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	public class BoneTableDeed : BaseAddonDeed
	{
		public override BaseAddon Addon { get { return new BoneTableAddon(); } }
		public override int LabelNumber { get { return 1074478; } } // Bone table

		[Constructable]
		public BoneTableDeed()
		{
			LootType = LootType.Blessed;
		}

		public BoneTableDeed(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
	#endregion
}