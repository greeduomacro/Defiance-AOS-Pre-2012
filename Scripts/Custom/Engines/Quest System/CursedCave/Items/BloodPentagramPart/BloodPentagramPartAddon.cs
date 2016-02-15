using System;
using System.Collections;
using Server;

namespace Server.Items
{
    public class BloodPentagramPartAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new BloodPentagramPartDeed(m_PartsArray, m_bBloodSoaked, Hue, LootType); } }

        public static PentagramPartEntry[] PentagramParts = new PentagramPartEntry[]
            {
                new PentagramPartEntry(0x1CF9, 0, 1, 0),
                new PentagramPartEntry(0x1CF8, 0, 2, 0),
                new PentagramPartEntry(0x1CF7, 0, 3, 0),
                new PentagramPartEntry(0x1CF6, 0, 4, 0),
                new PentagramPartEntry(0x1CF5, 0, 5, 0),

                new PentagramPartEntry(0x1CFB, 1, 0, 0),
                new PentagramPartEntry(0x1CFA, 1, 1, 0),
                new PentagramPartEntry(0x1D09, 1, 2, 0),
                new PentagramPartEntry(0x1D08, 1, 3, 0),
                new PentagramPartEntry(0x1D07, 1, 4, 0),
                new PentagramPartEntry(0x1CF4, 1, 5, 0),

                new PentagramPartEntry(0x1CFC, 2, 0, 0),
                new PentagramPartEntry(0x1D0A, 2, 1, 0),
                new PentagramPartEntry(0x1D11, 2, 2, 0),
                new PentagramPartEntry(0x1D10, 2, 3, 0),
                new PentagramPartEntry(0x1D06, 2, 4, 0),
                new PentagramPartEntry(0x1CF3, 2, 5, 0),

                new PentagramPartEntry(0x1CFD, 3, 0, 0),
                new PentagramPartEntry(0x1D0B, 3, 1, 0),
                new PentagramPartEntry(0x1D12, 3, 2, 0),
                new PentagramPartEntry(0x1D0F, 3, 3, 0),
                new PentagramPartEntry(0x1D05, 3, 4, 0),
                new PentagramPartEntry(0x1CF2, 3, 5, 0),

                new PentagramPartEntry(0x1CFE, 4, 0, 0),
                new PentagramPartEntry(0x1D0C, 4, 1, 0),
                new PentagramPartEntry(0x1D0D, 4, 2, 0),
                new PentagramPartEntry(0x1D0E, 4, 3, 0),
                new PentagramPartEntry(0x1D04, 4, 4, 0),
                new PentagramPartEntry(0x1CF1, 4, 5, 0),

                new PentagramPartEntry(0x1CFF, 5, 0, 0),
                new PentagramPartEntry(0x1D00, 5, 1, 0),
                new PentagramPartEntry(0x1D01, 5, 2, 0),
                new PentagramPartEntry(0x1D02, 5, 3, 0),
                new PentagramPartEntry(0x1D03, 5, 4, 0),
            };

        private bool m_bBloodSoaked;

        private BitArray m_PartsArray;
        public BitArray PartsArray
        {
            get { return m_PartsArray; }
            set { m_PartsArray = value; }
        }

        public BloodPentagramPartAddon(BitArray partsArray, bool bloodSoaked, int hue, LootType lootType)
        {
            m_PartsArray = partsArray;
            m_bBloodSoaked = bloodSoaked;
            Hue = hue;
            LootType = lootType;

            for (int i = 0; i < m_PartsArray.Count; ++i)
            {
                if (m_PartsArray[i])
                {
                    AddonComponent addonComp = new AddonComponent(PentagramParts[i].ItemID);
                    addonComp.Hue = Hue;
                    AddComponent(addonComp, PentagramParts[i].xOffset, PentagramParts[i].yOffset, PentagramParts[i].zOffset);
                }
            }
        }

        public BloodPentagramPartAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_bBloodSoaked);

            writer.Write(m_PartsArray.Count);
            for (int i = 0; i < m_PartsArray.Count; ++i)
                writer.Write(m_PartsArray[i]);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_bBloodSoaked = reader.ReadBool();

            int sizeA = reader.ReadInt();
            m_PartsArray = new BitArray(sizeA);
            for (int i = 0; i < sizeA; ++i)
                m_PartsArray[i] = reader.ReadBool();
        }

        #region PentagramPartEntry
        public class PentagramPartEntry
        {
            int m_ItemID;
            public int ItemID
            {
                get { return m_ItemID; }
                set { m_ItemID = value; }
            }

            int m_xOffset;
            public int xOffset
            {
                get { return m_xOffset; }
                set { m_xOffset = value; }
            }

            int m_yOffset;
            public int yOffset
            {
                get { return m_yOffset; }
                set { m_yOffset = value; }
            }

            int m_zOffset;
            public int zOffset
            {
                get { return m_zOffset; }
                set { m_zOffset = value; }
            }

            public PentagramPartEntry(int itemID, int xOffset, int yOffset, int zOffset)
            {
                m_ItemID = itemID;
                m_xOffset = xOffset;
                m_yOffset = yOffset;
                m_zOffset = zOffset;
            }
        }
        #endregion
    }
}