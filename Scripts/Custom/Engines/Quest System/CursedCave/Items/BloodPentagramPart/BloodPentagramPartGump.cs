using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class BloodPentagramPartGump : Gump
    {
        private BloodPentagramPartDeed m_Deed;

        public BloodPentagramPartGump(Mobile from, BloodPentagramPartDeed deed)
            : base(50, 50)
        {
            from.CloseGump(typeof(BloodPentagramPartGump));
            m_Deed = deed;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddBackground(7, 7, 402, 213, 5054);
            this.AddImageTiled(15, 16, 384, 195, 2624);
            this.AddAlphaRegion(15, 16, 384, 195);
            this.AddImage(0, 0, 10460);
            this.AddImage(0, 198, 10460);
            this.AddImage(385, 0, 10460);
            this.AddImage(385, 198, 10460);
            this.AddLabel(125, 23, 1152, @"Blood Pentagram Part Deed");
            this.AddButton(34, 123, 4005, 4007, (int)Buttons.Add, GumpButtonType.Reply, 0);
            this.AddLabel(69, 123, 1152, @"Add a bottle of blood to the deed.");
            this.AddButton(34, 173, 4005, 4007, (int)Buttons.Exit, GumpButtonType.Reply, 0);
            this.AddLabel(70, 173, 1152, @"Exit");
            this.AddLabel(38, 61, 1152, @"Parts in this deed:");
            this.AddButton(34, 148, 4005, 4007, (int)Buttons.PlaceInHouse, GumpButtonType.Reply, 0);
            this.AddLabel(70, 148, 1152, @"Place in house.");
            this.AddLabel(159, 61, 1152, string.Format("{0} of {1}", deed.NumOfPartsAdded, BloodPentagramPartAddon.PentagramParts.Length));

            if (deed.Complete)
                this.AddLabel(38, 81, 1152, @"This pentagram has been completed.");
            else
            {
                this.AddLabel(38, 81, 1152, @"Blood amount to next part:");
                this.AddLabel(208, 81, 1152, string.Format("{0} of {1}", deed.BloodAmount, BloodPentagramPartDeed.BloodPerPart));
            }
        }

        public enum Buttons
        {
            Exit,
            Add,
            PlaceInHouse,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Deed.Deleted || !m_Deed.IsChildOf(sender.Mobile.Backpack))
                return;

            switch (info.ButtonID)
            {
                case (int)Buttons.Exit:
                    return;
                case (int)Buttons.Add:
                    m_Deed.BeginCombine(sender.Mobile);
                    return;
                case (int)Buttons.PlaceInHouse:
                    if (m_Deed.NumOfPartsAdded > 0)
                    {
                        m_Deed.PlaceInHouse(sender.Mobile);
                        return;
                    }
                    else
                        sender.Mobile.SendMessage("You cannot add the pentagram to a house without any parts in the deed.");
                    break;
            }

            sender.Mobile.SendGump(new BloodPentagramPartGump(sender.Mobile, m_Deed));
        }
    }
}