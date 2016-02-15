using System;
using System.Collections;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class BloodBottle : Item
    {
        private int m_iBloodQuality;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BloodQuality
        {
            get { return m_iBloodQuality; }
            set { m_iBloodQuality = value; InvalidateProperties(); }
        }

        private int m_iBloodAmount;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BloodAmount
        {
            get { return m_iBloodAmount; }
            set { m_iBloodAmount = value; InvalidateProperties(); }
        }

        private string m_sCreatureName;
        [CommandProperty(AccessLevel.GameMaster)]
        public string CreatureName
        {
            get { return m_sCreatureName; }
            set { m_sCreatureName = value; InvalidateProperties(); }
        }

        private Type m_tCreatureType;
        public Type CreatureType
        {
            get { return m_tCreatureType; }
            set { m_tCreatureType = value; }
        }

        private bool m_bWillColor;
        public bool WillColor
        {
            get { return m_bWillColor; }
            set { m_bWillColor = value; InvalidateProperties(); }
        }

        private int m_iID;
        public int ID
        {
            get { return m_iID; }
            set { m_iID = value; }
        }

        public int CalculatedAmount
        {
            get
            {
                int returnValue = m_iBloodAmount + m_iBloodQuality;

                if (returnValue < 0)
                    return 0;

                return returnValue;
            }
        }

        [Constructable]
        public BloodBottle()
            : this(1, 0, null, null)
        {
        }

        [Constructable]
        public BloodBottle(int bloodAmount, int bloodQuality, string creatureName, Type creatureType)
            : base(0xF06)
        {
            Name = "Bottle of Blood";
            Hue = 0x1B0;

            m_iBloodAmount = bloodAmount;
            m_iBloodQuality = bloodQuality;
            m_sCreatureName = creatureName;
            m_tCreatureType = creatureType;
        }

        public BloodBottle(Serial serial)
            : base(serial)
        {
        }

        public void ConvertToSpecial(string name, int hue, bool willColor, int ID)
        {
            Name = name;
            Hue = hue;
            m_bWillColor = willColor;
            m_iID = ID;
            InvalidateProperties();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "blood amount\t{0}", m_iBloodAmount); // ~1_val~: ~2_val~

            if (m_iBloodQuality > 0)
                list.Add(1060659, "blood quality\t{0}{1}", m_iBloodQuality > 0 ? "+" : "", m_iBloodQuality); // ~1_val~: ~2_val~

            if (m_sCreatureName != null)
                list.Add(1070722, "recovered from {0}", m_sCreatureName); // ~1_NOTHING~

            if (m_bWillColor)
                list.Add(1049644, "will color the pentagram"); // [~1_stuff~]
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_bWillColor);
            writer.Write(m_iID);
            writer.Write(m_tCreatureType.Name);
            writer.Write(m_sCreatureName);
            writer.Write(m_iBloodAmount);
            writer.Write(m_iBloodQuality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_bWillColor = reader.ReadBool();
            m_iID = reader.ReadInt();
            m_tCreatureType = ScriptCompiler.FindTypeByName(reader.ReadString());
            m_sCreatureName = reader.ReadString();
            m_iBloodAmount = reader.ReadInt();
            m_iBloodQuality = reader.ReadInt();
        }
    }
}