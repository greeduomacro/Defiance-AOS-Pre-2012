using System;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.ContextMenus;
using System.Collections;
using Server.Gumps;
using System.Collections.Generic;

namespace Server.Items
{
	[FlipableAttribute(0x26BB, 0x26C5)]
	public class HarvesterOfTheGhost : BoneHarvester, ILevelable
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
		public HarvesterOfTheGhost()
			: base()
		{
			Name = "Harvester Of The Ghost";
			Hue = 0x96;

			Attributes.BonusStr = 5;
			Attributes.WeaponDamage = 40;
			Attributes.SpellChanneling = 1;
			Attributes.Luck = 100;
			Attributes.WeaponSpeed = 10;
			Slayer = SlayerName.Silver;
			WeaponAttributes.UseBestSkill = 1;

			LevelItemManager.InvalidateLevel(this);
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = fire = pois = cold = chaos = direct = 0;
			nrgy = 100;
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

		public HarvesterOfTheGhost(Serial serial)
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

			if (ItemID != 0x26BB && ItemID != 0x26C5)
				ItemID = 0x26BB;

			if (Hue == 0x2E)
				Hue = 0x96;
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

				// Beta 2 addition, this keeps gms from setting
				// the level to an outrageous value
				if (m_Experience > LevelItemManager.ExpTable[LevelItemManager.Levels - 1])
					m_Experience = LevelItemManager.ExpTable[LevelItemManager.Levels - 1];

				// Anytime exp is changed, call this method
				LevelItemManager.InvalidateLevel(this);
			}
		}

		/* This one will return our private m_Level
		 * variable */
		[CommandProperty(AccessLevel.GameMaster)]
		public int Level
		{
			get
			{
				return m_Level;
			}
			set
			{
				// Beta 2 addition, this keeps gms from setting
				// the level to an outrageous value
				if (value > LevelItemManager.Levels)
					value = LevelItemManager.Levels;

				// Beta 2 addition, this keeps gms from setting
				// the level to 0 or a negative value
				if (value < 1)
					value = 1;

				// THIS IS EXTREMELY IMPORTANT
				if (m_Level != value)
				{
					int oldLevel = m_Level;

					m_Level = value;

					// Anytime level changes, call this method
					OnLevel(oldLevel, m_Level);
				}
			}
		}

		/* This ILevelable member returns which types we want
		 * our item to level on. So, we'll return our private
		 * static array we declared earlier. */
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
			Attributes.BonusStr = LevelItemManager.CalculateProperty(5, m_Level);
			Attributes.WeaponDamage = LevelItemManager.CalculateProperty(40, m_Level);
			Attributes.Luck = LevelItemManager.CalculateProperty(100, m_Level);
			Attributes.WeaponSpeed = LevelItemManager.CalculateProperty(10, m_Level);
		}

		/// <summary>
		/// This gets the Html Formatted Info to display in the
		/// information gump.
		/// </summary>
		/// <returns></returns>
		public string GetHtml()
		{
			/* This can be done just about any way you want, just
			 * be sure to do it in html format. StringBuilder is
			 * in the System.Text namespace so if you use it,
			 * don't forget to add using System.Text; at the top
			 * of your script. */

			StringBuilder builder = new StringBuilder();

			builder.Append("<br>");
			builder.Append("<div align=center><i>ENHANCEMENTS</i></div>");

			builder.Append("Bonus Strenght: 5<br>");
			builder.Append("WeaponDamage: 40%<br>");
			builder.Append("Luck: 100<br>");
			builder.Append("WeaponSpeed: 10%<br>");

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