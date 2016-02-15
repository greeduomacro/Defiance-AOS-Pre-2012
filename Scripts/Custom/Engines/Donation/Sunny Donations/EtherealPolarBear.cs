
namespace Server.Mobiles
{
	public class EtherealPolarBear : EtherealMount
	{
		[Constructable]
		public EtherealPolarBear()
			: base(8417, 16069)
		{
			Name = "Ethereal Polar Bear Statuette";
			Hue = 1150;
		}

		public EtherealPolarBear(Serial serial)
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