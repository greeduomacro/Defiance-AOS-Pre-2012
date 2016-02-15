namespace Server.Mobiles
{
	public class EtherealSkeletalMount : EtherealMount
	{
		public override int EtherealHue { get { return 0; } }

		[Constructable]
		public EtherealSkeletalMount()
			: base(9751, 16059)
		{
			Name = "Ethereal Hellsteed Statuette";
			Hue = 1109;
		}

		public EtherealSkeletalMount(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}