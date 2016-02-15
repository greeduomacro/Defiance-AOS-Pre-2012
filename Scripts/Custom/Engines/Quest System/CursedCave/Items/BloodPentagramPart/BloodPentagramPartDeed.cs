using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Targeting;

namespace Server.Items
{
    public class BloodPentagramPartTarget : Target
    {
        private BloodPentagramPartDeed m_Deed;

        public BloodPentagramPartTarget(BloodPentagramPartDeed deed)
            : base(18, false, TargetFlags.None)
        {
            m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_Deed.Deleted || !m_Deed.IsChildOf(from.Backpack))
                return;

            m_Deed.EndCombine(from, targeted);
        }
    }

    public class BloodPentagramPartDeed : BaseAddonDeed
    {
        public static readonly int BloodPerPart = 500;

        public override BaseAddon Addon { get { return new BloodPentagramPartAddon(m_PartsArray, m_bBloodSoaked, Hue, LootType); } }
        public override int LabelNumber { get { return 1044328; } } // pentagram

        private BitArray m_PartsArray;
        public BitArray PartsArray
        {
            get { return m_PartsArray; }
            set { m_PartsArray = value; }
        }

        private bool m_bBloodSoaked;
        public bool BloodSoaked
        {
            get { return m_bBloodSoaked; }
            set
            {
                m_bBloodSoaked = value;
                InvalidateProperties();
            }
        }

        private int m_iBloodAmount;
        public int BloodAmount
        {
            get { return m_iBloodAmount; }
            set { m_iBloodAmount = value; }
        }

        public bool Complete
        {
            get { return (NumOfPartsAdded >= BloodPentagramPartAddon.PentagramParts.Length); }
        }

        public int NumOfPartsAdded
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < m_PartsArray.Count; ++i)
                    if (m_PartsArray[i])
                        counter++;

                return counter;
            }
        }

        [Constructable]
        public BloodPentagramPartDeed()
            : this(0)
        {
        }

        public BloodPentagramPartDeed(params int[] parts)
        {
            m_PartsArray = new BitArray(BloodPentagramPartAddon.PentagramParts.Length);

            for (int i = 0; i < parts.Length; ++i)
                m_PartsArray[parts[i]] = true;

            LootType = LootType.Blessed;
        }

        // Used by addon to create the deed
        public BloodPentagramPartDeed(BitArray partsArray, bool bloodSoaked, int hue, LootType lootType)
        {
            m_PartsArray = partsArray;
            BloodSoaked = bloodSoaked;
            Hue = hue;
            LootType = lootType;
        }

        public BloodPentagramPartDeed(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!Complete)
            {
                list.Add(1060658, "parts added\t{0} of {1}", NumOfPartsAdded, BloodPentagramPartAddon.PentagramParts.Length); // ~1_val~: ~2_val~
                list.Add(1060659, "blood amount to next part\t{0} of {1}", m_iBloodAmount, BloodPerPart); // ~1_val~: ~2_val~
            }
            else
                list.Add(1070722, "(Completed)"); // ~1_NOTHING~

            if (BloodSoaked)
                list.Add(1049644, "Blood Soaked"); // [~1_stuff~]
        }

        public void PlaceInHouse(Mobile from)
        {
            base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
			return;
        }

        public void AddBlood(Mobile from, int amount)
        {
            m_iBloodAmount += amount;

            while (m_iBloodAmount > BloodPerPart && !Complete)
            {
                m_iBloodAmount -= BloodPerPart;
                AddPart();
            }

            InvalidateProperties();
        }

        public bool AddPart()
        {
            for (int i = 0; i < m_PartsArray.Count; ++i)
            {
                if (!m_PartsArray[i])
                {
                    m_PartsArray[i] = true;
                    InvalidateProperties();
                    return true;
                }
            }

            return false;
        }

        public void BeginCombine(Mobile from)
        {
            if (!Complete)
                from.Target = new BloodPentagramPartTarget(this);
            else
                from.SendMessage("This deed has already been completed");
        }

        public void EndCombine(Mobile from, object o)
        {
            if (o is BloodBottle && ((BloodBottle)o).IsChildOf(from.Backpack))
            {
                BloodBottle bottle = (BloodBottle)o;

                AddBlood(from, bottle.CalculatedAmount);
                from.PlaySound(0x240);

                if (bottle.WillColor)
                {
                    Hue = bottle.Hue;
                    from.SendMessage("As you pour the blood on the deed it becomes soaked with blood.");
                }
                else
                    from.SendMessage("You add the blood to the pentagram.");

                bottle.Consume();
            }
            else
            {
                from.SendLocalizedMessage(1045158); // You must have the item in your backpack to target it.
            }

            from.SendGump(new BloodPentagramPartGump(from, this));
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
    }
}