using Server.Targeting;

namespace Server.Items
{
	class PersonalisationDeed : Item
	{
		[Constructable]
		public PersonalisationDeed()
			: base(0x14F0)
		{
			Name = "Personalisation Deed";
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (IsChildOf(from.Backpack))
			{
				from.Target = new PersonalisationDeedTarget(this);
				from.SendMessage("Please target the item, that you wish to engrave your name into.");
			}

			else
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
		}

		public PersonalisationDeed(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
		}

		private class PersonalisationDeedTarget : Target
		{
			private PersonalisationDeed m_Deed;

			public PersonalisationDeedTarget(PersonalisationDeed deed)
				: base(30, false, TargetFlags.None)
			{
				m_Deed = deed;
			}

			protected override void OnTarget(Mobile from, object target)
			{
				if (target is Item)
				{
					Item item = (Item)target;

					if (item.IsChildOf(from.Backpack))
					{
						if (m_Deed != null && !m_Deed.Deleted)
						{
							item.Name = from.Name + "'s " + ((item.Name == null || item.Name == string.Empty || item.Name == "") ? item.ItemData.Name : item.Name);
							from.SendMessage("You have engraved your name into the item.");
							Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x376A, 1, 29, 0x47D, 2, 9962, 0);
							Effects.SendLocationParticles(EffectItem.Create(new Point3D(from.X, from.Y, from.Z - 7), from.Map, EffectItem.DefaultDuration), 0x37C4, 1, 29, 0x47D, 2, 9502, 0);
							from.PlaySound(0x212);
							from.PlaySound(0x206);
							m_Deed.Delete();
						}
					}
					else
						from.SendLocalizedMessage(1061005); // The item must be in your backpack to enhance it.

				}
				else
				   from.SendMessage("That is not an item.");
			}
		}
	}
}