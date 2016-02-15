namespace Server.Mobiles
{
	public class EtherealSkeletalMountAOS : EtherealSkeletalMount
	{
		[Constructable]
		public EtherealSkeletalMountAOS()
			: base()
		{
			IsDonationItem = true;
		}

		public EtherealSkeletalMountAOS(Serial serial)
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