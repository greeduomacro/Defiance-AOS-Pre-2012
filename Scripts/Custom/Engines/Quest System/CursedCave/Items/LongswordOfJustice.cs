using System;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using Server.Gumps;
using Server.ContextMenus;
using System.Collections.Generic;

namespace Server.Items
{
	[FlipableAttribute(0xF61, 0xF60)]
	public class LongswordOfJustice : Longsword, ILevelable
	{
		private static Type[] m_Types = new Type[]
				{
					typeof( TheCursed ),
					typeof( TheCursedArcher ),
					typeof( TheCursedMage ),
					typeof( TheCursedNecro ),
					typeof( CursedSkeleton ),
					typeof( TheCursedWarrior ),
					typeof( TheCursedGuardian ),

					typeof( TheCursedAlchemist ),
   					typeof( TheCursedBlacksmith ),
   					typeof( TheCursedButcher ),
					typeof( TheCursedCarpenter ),

					typeof( DevaTheCursedOne ),
					typeof( CursedSteed ),
					typeof( Hephaestus ),
				};

		private int m_Experience;
		private int m_Level;

		public override int ArtifactRarity { get { return 11; } }

		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		[Constructable]
		public LongswordOfJustice()
			: base()
		{
			Hue = 0x84A;
			Name = "Longsword Of Justice";
			WeaponAttributes.UseBestSkill = 1;
			Slayer = SlayerName.Silver;

			LevelItemManager.InvalidateLevel(this);
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			fire = pois = nrgy = chaos = direct = 0;
			cold = 70;
			phys = 30;
		}

		public override void AddNameProperty(ObjectPropertyList list)
		{
			base.AddNameProperty(list);
			list.Add(1070722, "Trapped Soul");
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			base.OnHit( attacker, defender, damageBonus );

			if ( 0.05 > Utility.RandomDouble() )
				CursedCaveUtility.WeaponSpeak( attacker, defender, this.Name );
		}

		public LongswordOfJustice(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);

			writer.Write(m_Experience);
			writer.Write(m_Level);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			m_Experience = reader.ReadInt();
			m_Level = reader.ReadInt();
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1060658, "Level\t{0}", m_Level);
			list.Add(1060659, "Experience\t{0}", m_Experience);
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (from.Alive && (from.Items.Contains(this) || this.IsChildOf(from.Backpack)))
			{
				list.Add(new CursedCaveUtility.WeaponTalkEntry(from, this));
				list.Add(new LevelInfoEntry(this));
			}
		}

		#region ILevelable Members

		[CommandProperty(AccessLevel.GameMaster)]
		public int Experience
		{
			get
			{
				return m_Experience;
			}
			set
			{
				m_Experience = value;

				if (m_Experience > LevelItemManager.ExpTable[LevelItemManager.Levels - 1])
					m_Experience = LevelItemManager.ExpTable[LevelItemManager.Levels - 1];

				LevelItemManager.InvalidateLevel(this);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Level
		{
			get
			{
				return m_Level;
			}
			set
			{
				if (value > LevelItemManager.Levels)
					value = LevelItemManager.Levels;

				if (value < 1)
					value = 1;

				if (m_Level != value)
				{
					int oldLevel = m_Level;

					m_Level = value;

					OnLevel(oldLevel, m_Level);
				}
			}
		}

		public Type[] Types
		{
			get
			{
				return m_Types;
			}
		}

		/// <summary>
		/// This method is called when the Item's level has changed
		/// </summary>
		/// <param name="oldLevel">The item's old level</param>
		/// <param name="newLevel">The item's new level</param>
		public void OnLevel(int oldLevel, int newLevel)
		{
			WeaponAttributes.HitFireball = LevelItemManager.CalculateProperty(50, m_Level);

			Attributes.BonusHits = LevelItemManager.CalculateProperty(5, m_Level);//Edit by Sunny was 15
			Attributes.AttackChance = LevelItemManager.CalculateProperty(15, m_Level);
			Attributes.WeaponDamage = LevelItemManager.CalculateProperty(50, m_Level);
		}

		/// <summary>
		/// This gets the Html Formatted Info to display in the
		/// information gump.
		/// </summary>
		/// <returns></returns>
		public string GetHtml()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("<br>");
			builder.Append("<div align=center><i>ENHANCEMENTS</i></div>");

			builder.Append("Bonus Hitpoints: 5<br>");
			builder.Append("HitFireball: 50%<br>");
			builder.Append("Attack Chance: 15<br>");
			builder.Append("Increase Damage: 50%<br>");

			builder.Append("<div align=center><i>LEVEL GAIN LIST</i></div>");

			for (int i = 1; i < LevelItemManager.ExpTable.Length; i++)
			{
				int iLevel = i + 1;
				builder.Append(string.Format("Level {0} at {1} EXP<br>", iLevel.ToString(), LevelItemManager.ExpTable[i].ToString()));
			}
			return builder.ToString();
		}

		#endregion
	}
}