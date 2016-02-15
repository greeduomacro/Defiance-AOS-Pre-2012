using Server;
using Server.Items;

namespace Server.Items
{
	public class CCSkeletalCorpse : Item
	{
		[Constructable]
		public CCSkeletalCorpse()
			: base(0xECA + Utility.Random(9))
		{
			Movable = false;
			Weight = 2.0;
			Name = "an unknown paladin guard's skeleton";
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!from.InRange(GetWorldLocation(), 2))
				from.SendLocalizedMessage(500446); // That is too far away.
			else
			{
				Effects.SendLocationEffect(Location, Map, 0x3709, 13, 0x3B2, 0);
				Effects.PlaySound(Location, Map, 455);

				if (0.45 > Utility.RandomDouble())
				{
					new Gold(100, 300).MoveToWorld(Location, Map);
					LootPackEntry.Mutate( Loot.RandomArmorOrShieldOrWeaponOrJewelry(), Utility.RandomMinMax(1, 5), 20, 100, 480 ).MoveToWorld( Location, Map );
				}

				this.Delete();
			}
		}

		public CCSkeletalCorpse(Serial serial)
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