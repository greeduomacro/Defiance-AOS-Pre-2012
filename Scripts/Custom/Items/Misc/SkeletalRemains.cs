//******************************************************
// Name: SkeletalRemains
// Desc: Written by Eclipse
//******************************************************
using Server;
using Server.Items;

namespace Server.Items
{
	public class SkeletalRemains : BaseContainer
	{
		public override int DefaultGumpID { get { return 0x9; } }
		public override int DefaultDropSound { get { return 0x42; } }

		public override Rectangle2D Bounds
		{
			get { return new Rectangle2D(20, 85, 104, 111); }
		}

		[Constructable]
		public SkeletalRemains()
			: base(0xECA + Utility.Random(9))
		{
			Weight = 2.0;
			AddLoot();
		}

		public void AddLoot()
		{
			// Add Gold
			DropItem(new Gold(Utility.RandomMinMax(1000, 1500)));

			// Add Items
			LootPackEntry.AddRandomLoot( this, 3, 480,  3, 5, 30, 100 );

			// Set Name and add type items
			switch (Utility.Random(3))
			{
				case 0:
					Name = "an unknown bard's skeleton";
					DropItem(new ShortPants());
					DropItem(new JesterHat());
					DropItem(new Doublet());
					DropItem(new Bandage(Utility.RandomMinMax(10, 20)));
					DropItem(new BeverageBottle(BeverageType.Ale));

					BaseInstrument inst = Loot.RandomInstrument();
					if (Core.AOS)
						inst.Slayer = BaseRunicTool.GetRandomSlayer();
					DropItem(inst);
					break;
				case 1:
					Name = "an unknown mage's skeleton";
					DropItem(new BagOfReagents(25));
					DropItem(new Robe(Utility.RandomDyedHue()));
					DropItem( LootPackEntry.Mutate( new GnarledStaff(), 480, CraftResource.Verite ) );
					DropItem(new Sandals());
					Loot.AddScrolls(this, Utility.Random(5) + 1, false);
					break;
				case 2:
					Name = "an unknown rogue's skeleton";
					DropItem(new Lockpick(Utility.Random(5) + 1));
					DropItem(new Shovel());
					DropItem(new Torch());
					DropItem(new Dagger());
					DropItem( LootPackEntry.Mutate( new LeatherChest(), 480, CraftResource.Verite ) );
					DropItem(new TreasureMap(Utility.RandomMinMax(1, 5), Map.Felucca));
					break;
			}
		}

		public SkeletalRemains(Serial serial)
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