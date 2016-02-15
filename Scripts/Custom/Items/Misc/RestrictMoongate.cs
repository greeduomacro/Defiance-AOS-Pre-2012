using System;
using Server;
using Server.Mobiles;
using Server.Spells.Second;

namespace Server.Items
{
	public class RestrictMoongate : Moongate
	{
		#region CommandProperties
		private bool m_bAllowPets;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowPets
		{
			get { return m_bAllowPets; }
			set { m_bAllowPets = value; }
		}

		private bool m_bAllowYoungPlayer;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowYoungPlayer
		{
			get { return m_bAllowYoungPlayer; }
			set { m_bAllowYoungPlayer = value; }
		}

		private bool m_bAllowPolymorph;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowPolymorph
		{
			get { return m_bAllowPolymorph; }
			set { m_bAllowPolymorph = value; }
		}

		private bool m_bAllowMounts;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowMounts
		{
			get { return m_bAllowMounts; }
			set { m_bAllowMounts = value; }
		}

		private bool m_bAllowFactionSigil;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowFactionSigil
		{
			get { return m_bAllowFactionSigil; }
			set { m_bAllowFactionSigil = value; }
		}

		private bool m_bAutoUndress;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AutoUndressInBank
		{
			get { return m_bAutoUndress; }
			set { m_bAutoUndress = value; }
		}

		private bool m_bAllowItemsOnChar;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowItemsOnChar
		{
			get { return m_bAllowItemsOnChar; }
			set { m_bAllowItemsOnChar = value; }
		}

		private bool m_bAllowRedChars;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowRedChars
		{
			get { return m_bAllowRedChars; }
			set { m_bAllowRedChars = value; }
		}

		private bool m_bAllowBlueChars;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowBlueChars
		{
			get { return m_bAllowBlueChars; }
			set { m_bAllowBlueChars = value; }
		}

		private bool m_bAllowPetalTrinsic;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowPetalTrinsic
		{
			get { return m_bAllowPetalTrinsic; }
			set { m_bAllowPetalTrinsic = value; }
		}

		private bool m_bAllowPetalOrange;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowPetalOrange
		{
			get { return m_bAllowPetalOrange; }
			set { m_bAllowPetalOrange = value; }
		}

		private bool m_bAllowStatMods;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowStatMods
		{
			get { return m_bAllowStatMods; }
			set { m_bAllowStatMods = value; }
		}

		private bool m_bAllowSkillMods;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowSkillMods
		{
			get { return m_bAllowSkillMods; }
			set { m_bAllowSkillMods = value; }
		}

		private bool m_bAllowResistanceMods;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowResistanceMods
		{
			get { return m_bAllowResistanceMods; }
			set { m_bAllowResistanceMods = value; }
		}

		private int m_HueMod;
		[CommandProperty(AccessLevel.GameMaster)]
		public int HueOverride
		{
			get { return m_HueMod; }
			set { m_HueMod = value; }
		}

		private string m_NameMod;
		[CommandProperty(AccessLevel.GameMaster)]
		public string NameMod
		{
			get { return m_NameMod; }
			set { m_NameMod = value; }
		}

		private int m_BodyValue;
		[CommandProperty(AccessLevel.GameMaster)]
		public int BodyValue
		{
			get { return m_BodyValue; }
			set { m_BodyValue = value; }
		}

		private bool m_bAllowGhosts;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowGhosts
		{
			get { return m_bAllowGhosts; }
			set { m_bAllowGhosts = value; }
		}

		private bool m_bStaffCheck; //if true allows only Administrator to pass
		[CommandProperty(AccessLevel.Administrator)]
		public bool StaffAccessCheck
		{
			get { return m_bStaffCheck; }
			set { m_bStaffCheck = value; }
		}
		#endregion

		[Constructable]
		public RestrictMoongate()
		{
			m_bAllowItemsOnChar = true;
			m_bAllowRedChars = true;
			m_bAllowBlueChars = true;
			m_bAllowStatMods = true;
			m_bAllowSkillMods = true;
			m_bAllowResistanceMods = true;
			m_bAllowGhosts = true;
			m_HueMod = -1;
			Dispellable = false;
		}

		public RestrictMoongate(Serial serial)
			: base(serial)
		{
		}

		public override void UseGate(Mobile m)
		{
			ClientFlags flags = m.NetState == null ? ClientFlags.None : m.NetState.Flags;

			if (m_bStaffCheck && m.AccessLevel < AccessLevel.Administrator && m.AccessLevel > AccessLevel.Player)
				m.SendLocalizedMessage(1019004); //You are not allowed to travel there.
			else if (!m_bAllowGhosts && !m.Alive)
				m.SendLocalizedMessage(1049613); //You may not enter that region as a ghost
			else if (!m_bAllowFactionSigil && Factions.Sigil.ExistsOn(m))
				m.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
			else if (!m_bAllowYoungPlayer && m is PlayerMobile && ((PlayerMobile)m).Young)
				m.SendMessage("Young players are not allowed to use this gate.");
			else if (!m_bAllowMounts && m.Mounted)
				m.SendLocalizedMessage(1042561); // Please dismount first.
			else if (!m_bAllowPolymorph && m.BodyMod != 0)
				m.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
			else if (!m_bAllowRedChars && m.Kills >= 5)
				m.SendMessage("Red players are not allowed to use this gate.");
			else if (!m_bAllowBlueChars && m.Kills < 5)
				m.SendMessage("Blue players are not allowed to use this gate.");
			else if (!m_bAllowPetalTrinsic && m.GetStatMod("RoseOfTrinsicPetal") != null)
				m.SendMessage("Players under the effect of the rose of trinsic petal are not allowed to use this gate.");
			else if (!m_bAllowPetalOrange && OrangePetals.UnderEffect(m))
				m.SendMessage("Players under the effect of the orange petal are not allowed to use this gate.");
			else if (!m_bAllowStatMods && (m.StatMods != null && m.StatMods.Count > 0))
				m.SendMessage("You cannot have any stat bonus items equipped or stat bonus spells active when using this gate.");
			else if (!m_bAllowSkillMods && (m.SkillMods != null && m.SkillMods.Count > 0))
				m.SendMessage("You cannot have any skill bonus items equipped or skill bonus spells active when using this gate.");
			else if (!m_bAllowResistanceMods && (m.ResistanceMods != null && m.ResistanceMods.Count > 0))
				m.SendMessage("You cannot have any resistance bonus spells active when using this gate.");
			else if ( ( TargetMap == Map.Tokuno && (flags & ClientFlags.Tokuno) == 0 ) || ( TargetMap == Map.Malas && (flags & ClientFlags.Malas) == 0 ) || ( TargetMap == Map.Ilshenar && (flags & ClientFlags.Ilshenar) == 0 ) )
				m.SendLocalizedMessage(1019004); // You are not allowed to travel there.
			else if (m.Spell != null)
			{
				m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
			}
			else if (TargetMap != null && TargetMap != Map.Internal)
			{
				if (m.AccessLevel <= AccessLevel.Counselor && !m_bAllowItemsOnChar && !m.IsNaked())
				{
					if (m_bAutoUndress && m.Backpack != null && m.BankBox != null)
					{
						Bag bag;
						bag = new Bag();
						bag.Hue = 87;
						bag.Name = "Undress Bag";
						m.BankBox.DropItem(bag);

						if (m.Holding != null)
							m.Backpack.DropItem(m.Holding);

						for (int i = m.Backpack.Items.Count; i > 0; i--)
						{
							Item item = m.Backpack.Items[i - 1];
								bag.DropItem(item);
						}

						for (int i = m.Items.Count - 1; i >= 0; --i)
						{
							Item item = (Item)m.Items[i];
							if ( item.Layer != Layer.Backpack & item.Layer != Layer.Bank && item.Layer != Layer.FacialHair &&
								  item.Layer != Layer.Mount && item.Layer != Layer.Hair )
								bag.DropItem(item);
						}
					}
					else
					{
						m.SendMessage("Your backpack must be empty and you must also be naked to use this gate.");
						return;
					}
				}
				if (this is LLTeleporter && !((LLTeleporter)this).ChargePlayer(m))
					return;
				if (m_bAllowPets)
					BaseCreature.TeleportPets(m, Target, TargetMap);

				if (m_HueMod > -1)
					m.SolidHueOverride = m_HueMod;
				else
					m.SolidHueOverride = -1;
				if (m_NameMod != null)
					m.NameMod = m_NameMod;
				else
					m.NameMod = null;
				if (m_BodyValue > 0)
					m.BodyValue = m_BodyValue;
				else if (m_BodyValue < 0)
					if (m.Female)
						m.BodyValue = 401;
					else m.BodyValue = 400;

				m.MoveToWorld(Target, TargetMap);
				m.PlaySound(0x1FE);
				OnGateUsed(m);
			}
			else
			{
				m.SendMessage("This moongate does not seem to go anywhere.");
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)1); // version

			// Version 1
			writer.Write(m_bAutoUndress);
			writer.Write(m_HueMod);
			writer.Write(m_NameMod);
			writer.Write(m_BodyValue);
			writer.Write(m_bStaffCheck);
			writer.Write(m_bAllowGhosts);
			// Version 0
			writer.Write(m_bAllowPets);
			writer.Write(m_bAllowYoungPlayer);
			writer.Write(m_bAllowPolymorph);
			writer.Write(m_bAllowMounts);
			writer.Write(m_bAllowFactionSigil);
			writer.Write(m_bAllowItemsOnChar);
			writer.Write(m_bAllowRedChars);
			writer.Write(m_bAllowBlueChars);
			writer.Write(m_bAllowPetalTrinsic);
			writer.Write(m_bAllowPetalOrange);
			writer.Write(m_bAllowStatMods);
			writer.Write(m_bAllowSkillMods);
			writer.Write(m_bAllowResistanceMods);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					m_bAutoUndress = reader.ReadBool();
					m_HueMod = reader.ReadInt();
					m_NameMod = reader.ReadString();
					m_BodyValue = reader.ReadInt();
					m_bStaffCheck = reader.ReadBool();
					m_bAllowGhosts = reader.ReadBool();
					goto case 0;
				case 0:
					m_bAllowPets = reader.ReadBool();
					m_bAllowYoungPlayer = reader.ReadBool();
					m_bAllowPolymorph = reader.ReadBool();
					m_bAllowMounts = reader.ReadBool();
					m_bAllowFactionSigil = reader.ReadBool();
					m_bAllowItemsOnChar = reader.ReadBool();
					m_bAllowRedChars = reader.ReadBool();
					m_bAllowBlueChars = reader.ReadBool();
					m_bAllowPetalTrinsic = reader.ReadBool();
					m_bAllowPetalOrange = reader.ReadBool();
					m_bAllowStatMods = reader.ReadBool();
					m_bAllowSkillMods = reader.ReadBool();
					m_bAllowResistanceMods = reader.ReadBool();
					break;
			}
		}
	}
}