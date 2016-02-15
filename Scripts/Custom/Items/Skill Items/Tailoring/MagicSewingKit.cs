using Server.Targeting;
using Server.Engines.RewardSystem; // Silver

namespace Server.Items
{
	public class MagicSewingKit : Item, IUsesRemaining
	{
		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		[Constructable]
		public MagicSewingKit() : this(1)
		{
		}

		[Constructable]
		public MagicSewingKit( int uses ) : base( 0xF9D )
		{
			m_UsesRemaining = uses;
			Name = "Magic Sewing Kit";
			Hue = 1165;
		}

		public bool ShowUsesRemaining { get { return true; } set { } }

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (IsChildOf(from.Backpack))
			{
				from.Target = new SewingTarget(this);
				from.SendMessage("Please target the Cloak or Robe that you wish to enhance.");
			}

			else
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
		}

		public MagicSewingKit( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			m_UsesRemaining = reader.ReadInt();
		}

		private class SewingTarget : Target
		{
			private MagicSewingKit m_Kit;

			public SewingTarget(MagicSewingKit kit) : base(30, false, TargetFlags.None)
			{
				m_Kit = kit;
			}

			protected override void OnTarget(Mobile from, object target)
			{
				if (target is Robe || target is Cloak || (target is HoodedShroudOfShadows && !(target is TempShroud) ) ) // Shroud added by Silver
				{
					BaseClothing Clothtarg = (BaseClothing)target;

					if (Clothtarg.IsChildOf(from.Backpack))
					{
						if (Clothtarg.PhysicalResistance == 0 && Clothtarg.BasePhysicalResistance == 0)
						{
							if (m_Kit != null && !m_Kit.Deleted)
							{
								Clothtarg.Resistances.Physical = 3;
								from.SendMessage("You have enhanced the item.");
								m_Kit.UsesRemaining--;
								Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x376A, 1, 29, 0x47D, 2, 9962, 0);
								Effects.SendLocationParticles(EffectItem.Create(new Point3D(from.X, from.Y, from.Z - 7), from.Map, EffectItem.DefaultDuration), 0x37C4, 1, 29, 0x47D, 2, 9502, 0);
								from.PlaySound(0x212);
								from.PlaySound(0x206);
							}

							if (m_Kit.UsesRemaining < 1)
							{
								m_Kit.Delete();
								from.SendMessage("The magic sewing kit broke.");
							}
						}
						else
						   from.SendMessage("That item is already enhanced.");
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