using System;
using Server;

namespace Server.Items
{
	public class RarePlantsBook : RedBook
	{
		public static readonly BookContent Content = new BookContent
			(
				"Plant Book First Edition", "Edwin the naturalist",
				new BookPageInfo
				(
					"This book contains, the",
					"locations of some rare",
					"plants in britannia.",
					"",
					"Rare Mandrake Root",
					"This plant has been seen",
					"at Cove Orc Fort.",
					""
				),
				new BookPageInfo
				(
					"Rare Garlic",
					"This plant only grow at a",
					"Temple on the Fire Island.",
					"",
					"Rare Nightshade",
					"This plant has been seen",
					"at Yew Grave.",
					""
				),
				new BookPageInfo
				(
					"Rare Ginseng",
					"This plant only grow near",
					"the Compassion Shrine",
					"(Felucca).",
					"",
					"I also noticed that some",
					"creatures like to build",
					"their nests near these"
				),
				new BookPageInfo
				(
					"plants, so be careful so",
					"you dont disturb them.",
					"So far i have seen",
					"spiders, snakes and",
					"serpent nests near the",
					"plants, they seem to eat",
					"the roots of the plants.",
					""
				)
			);

		public override BookContent DefaultContent { get { return Content; } }

		[Constructable]
		public RarePlantsBook()
			: base(false)
		{
			Hue = 0x89B;
		}

		public RarePlantsBook(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}
}