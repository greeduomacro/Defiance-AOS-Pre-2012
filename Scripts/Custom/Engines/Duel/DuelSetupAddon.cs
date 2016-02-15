using Server.Items;

namespace Server.Events.Duel
{
	public class DuelSetupAddon : BaseAddon
	{
		[Constructable]
		public DuelSetupAddon()
		{
			AddComponent(1266, 0, 0, 0);

			for (int i = -4; i < 6; i++)
			{
				AddComponent(2148, -7, i, 0);
				AddComponent(2148, 7, i, 0);
			}

			for (int i = -6; i < 8; i++)
			{
				if (i != 0)
					AddComponent(2147, i, -5, 0);
				AddComponent(2147, i, 5, 0);
			}

		}

		public void AddComponent(int id, int x, int y, int z)
		{
			AddonComponent ac = new AddonComponent(id);

			ac.Hue = 33;

			AddComponent(ac, x, y, z);
		}

		public DuelSetupAddon(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			Delete();
		}
	}
}