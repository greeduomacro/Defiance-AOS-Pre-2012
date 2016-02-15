using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class FlamingHead : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new FlamingHeadDeed(); } }
		private bool m_bEast;

		[Constructable]
		public FlamingHead( bool east )
		{
			m_bEast = east;

			if ( east )
				AddComponent( new AddonComponent( 0x10FC ), 0, 0, 0 );
			else
				AddComponent( new AddonComponent( 0x110F ), 0, 0, 0 );
		}

		public virtual bool PassivelyTriggered{ get{ return true; } }
		public virtual TimeSpan PassiveTriggerDelay{ get{ return TimeSpan.FromSeconds( 3.0 ); } }
		public virtual int PassiveTriggerRange{ get{ return 1; } }
		public virtual TimeSpan ResetDelay{ get{ return TimeSpan.FromSeconds( 2.0 ); } }

		private DateTime m_NextPassiveTrigger, m_NextActiveTrigger;

		public virtual void OnTrigger( Mobile from )
		{
			Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), m_bEast ? 0x10FC : 0x110F, 10, 30, 5052 );
			Effects.PlaySound( Location, Map, 0x227 );
		}

		public override bool HandlesOnMovement{ get{ return true; } } // Tell the core that we implement OnMovement

		public virtual int GetEffectHue()
		{
			int hue = this.Hue & 0x3FFF;

			if ( hue < 2 )
				return 0;

			return hue - 1;
		}

		public bool CheckRange( Point3D loc, Point3D oldLoc, int range )
		{
			return CheckRange( loc, range ) && !CheckRange( oldLoc, range );
		}

		public bool CheckRange( Point3D loc, int range )
		{
			return ( (this.Z + 8) >= loc.Z && (loc.Z + 16) > this.Z )
				&& Utility.InRange( GetWorldLocation(), loc, range );
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			base.OnMovement( m, oldLocation );

			if ( m.Location == oldLocation )
				return;

			if (m.AccessLevel > AccessLevel.Player && m.Hidden)
				return;

			if ( CheckRange( m.Location, oldLocation, 0 ) && DateTime.Now >= m_NextActiveTrigger )
			{
				m_NextActiveTrigger = m_NextPassiveTrigger = DateTime.Now + ResetDelay;

				OnTrigger( m );
			}
			else if ( PassivelyTriggered && CheckRange( m.Location, oldLocation, PassiveTriggerRange ) && DateTime.Now >= m_NextPassiveTrigger )
			{
				m_NextPassiveTrigger = DateTime.Now + PassiveTriggerDelay;

				OnTrigger( m );
			}
		}

		public FlamingHead( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( m_bEast );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_bEast = reader.ReadBool();
		}
	}

	public class FlamingHeadDeed : BaseAddonDeed
	{
		private bool m_East;

		public override BaseAddon Addon{ get{ return new FlamingHead( m_East ); } }

		public override int LabelNumber{ get{ return 1041050; } }

		[Constructable]
		public FlamingHeadDeed()
		{
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.CloseGump( typeof( InternalGump ) );
				from.SendGump( new InternalGump( this ) );
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}

		private void SendTarget( Mobile m )
		{
			base.OnDoubleClick( m );
		}

		private class InternalGump : Gump
		{
			private FlamingHeadDeed m_Deed;

			public InternalGump( FlamingHeadDeed deed ) : base( 150, 50 )
			{
				m_Deed = deed;

				AddBackground( 0, 0, 350, 250, 0xA28 );

				AddItem( 112, 35, 0x110F );
				AddButton( 70, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0 );

				AddItem( 202, 35, 0x10FC );
				AddButton( 180, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0 );
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Deed.Deleted || info.ButtonID == 0 )
					return;

				m_Deed.m_East = (info.ButtonID != 1);
				m_Deed.SendTarget( sender.Mobile );
			}
		}

		public FlamingHeadDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}