using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class PlagueSummoningAltar : BaseSummoningAltar
    {
        public override Type ChampionType { get { return typeof(ThePlague); } }
        public override int HueActive { get { return 0x558; } }
        public override int HueInactive { get { return 0x472; } }

        [Constructable]
        public PlagueSummoningAltar()
        {
        }

        public PlagueSummoningAltar(Serial serial)
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
}