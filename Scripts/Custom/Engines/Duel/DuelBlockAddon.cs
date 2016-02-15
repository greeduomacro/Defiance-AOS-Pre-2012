using Server.Items;

namespace Server.Events.Duel
{
	public class DuelBlockAddon : BaseAddon
	{
		[Constructable]
		public DuelBlockAddon()
		{
			AddComponent(1266, 0, 0, 0);
			AddComponent(1805, -1, 0, 0);
			AddComponent(1812, -1, 1, 0);
			AddComponent(1802, 0, 1, 0);
			AddComponent(1811, 1, 1, 0);
			AddComponent(1803, 1, 0, 0);
			AddComponent(1813, 1, -1, 0);
			AddComponent(1804, 0, -1, 0);
			AddComponent(1810, -1, -1, 0);

			AddComponent(8612, 0, -1, 0);
			AddComponent(8612, -1, 0, 0);
			AddComponent(8612, 0, 1, 0);
			AddComponent(8612, 1, 0, 0);
		}

		public void AddComponent(int id, int x, int y, int z)
		{
			AddonComponent ac = new AddonComponent(id);

			ac.Hue = 1072;

			AddComponent(ac, x, y, z);
		}

		public DuelBlockAddon(Serial serial) : base(serial)
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