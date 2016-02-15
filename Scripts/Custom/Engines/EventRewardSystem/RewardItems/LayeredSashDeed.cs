using System;
using Server.Targeting;

namespace Server.Items
{
	class LayeredSashDeed : Item
	{
		[Constructable]
		public LayeredSashDeed()
			: base(0x14F0)
		{
			Name = "Layered Sash Deed";
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (IsChildOf(from.Backpack))
			{
				from.Target = new LayeredSashDeedTarget(this);
				from.SendMessage("Please target the Sash, that you wish to enhance.");
			}

			else
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
		}

		public LayeredSashDeed(Serial serial)
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

		private class LayeredSashDeedTarget : Target
		{
			private LayeredSashDeed m_Deed;

			public LayeredSashDeedTarget(LayeredSashDeed deed)
				: base(30, false, TargetFlags.None)
			{
				m_Deed = deed;
			}

			protected override void OnTarget(Mobile from, object target)
			{
				if (target is BodySash)
				{
					BodySash sash = (BodySash)target;

					if (sash.IsChildOf(from.Backpack))
					{
						if (m_Deed != null && !m_Deed.Deleted)
						{
							sash.Layer = Layer.Earrings;
				if ( sash.Name == null )
				sash.Name = String.Format( "Body Sash [Layered]", sash.Name );
				else
				sash.Name = String.Format( "{0} [Layered]", sash.Name );
							from.SendMessage("You have enhanced the item.");
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
					from.SendMessage("You cannot enhance that item.");
			}
		}
	}
}