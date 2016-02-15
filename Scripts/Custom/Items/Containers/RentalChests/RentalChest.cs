//		Defiance AOS rental chests - by Blady - kerr4o@gmail.com

using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	[Flipable( 0xe43, 0xe42 )]
	public class RentalChest : LockableContainer
	{
		public override string DefaultName{ get{ return "a rental chest"; } }

		public virtual int RentalCost{ get{ return 4000; } }
		public virtual TimeSpan RentalDuration{ get{ return TimeSpan.FromDays( 7.0 ); } }

		private DateTime m_RentalExpireTime;

		private PlayerMobile m_Owner;
		private bool m_Rented;

		[CommandProperty( AccessLevel.Administrator )]
		public PlayerMobile Owner
		{
			get{ return m_Owner; }
			set
			{
				if ( value == null )
					CancelRent();
				m_Owner = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public bool Rented
		{
			get{ return m_Rented; }
			set
			{
				if ( !value )
					CancelRent();
				else if ( m_Owner != null )
					BeginRent();
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public DateTime RentalExpireTime{ get{ return m_RentalExpireTime; } set{ m_RentalExpireTime = value; InvalidateProperties(); } }

		[Constructable]
		public RentalChest() : base( 0xE43 )
		{
			Movable = false;
		}

		public RentalChest( Serial serial ) : base( serial )
		{
		}

		public void BeginRent( PlayerMobile from )
		{
			m_Owner = from;
			BeginRent();
		}

		public void BeginRent()
		{
			if ( m_Owner != null )
			{
				m_RentalExpireTime = DateTime.Now + RentalDuration;
				m_Rented = true;
				Hue = Utility.Random( 550, 500 );
				InvalidateProperties();
				m_Owner.SendMessage( "You have successfully rented the chest." );
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			if ( m_Owner != null && m_Rented )
				list.Add( 1060847, "Rented to\t{0}", m_Owner.Name );
			else
				list.Add( 1060847, "Available for\tRental" );
		}

		public void CancelRent()
		{
			m_Owner = null;
			m_Rented = false;
			Hue = 0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Owner != null )
			{
				if ( m_Owner.Account == null )
					CancelRent();
				else if ( from.Account.Username != m_Owner.Account.Username && from.AccessLevel < AccessLevel.Seer)
				{
					from.SendLocalizedMessage( 500447 );
					return;
				}
			}

			base.OnDoubleClick( from );
		}

		public override bool CheckLift( Mobile from, Item item, ref LRReason reject )
		{
			if ( m_Owner != null && m_Owner.Account == null )
				CancelRent();

			return ( m_Owner == null || from.Account.Username == m_Owner.Account.Username ) || base.CheckLift( from, item, ref reject );
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			if ( m_Owner != null )
			{
				if ( m_Owner.Account == null )
					CancelRent();
				else if ( m_Owner.Account.Username == m.Account.Username )
				{
					int maxItems = this.MaxItems;
					if ( checkItems && maxItems != 0 && ( this.TotalItems + plusItems + item.TotalItems + (item.IsVirtualItem ? 0 : 1) ) > maxItems )
					{
						if ( message )
							SendFullItemsMessage( m, item );
						return false;
					}
					return true;
				}
			}

			return base.CheckHold( m, item, message, checkItems, plusItems, plusWeight );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( m_Owner == null )
				list.Add( new RentEntry( from, this ) );
			else if ( m_Owner.Account == null )
			{
				CancelRent();
				list.Add( new RentEntry( from, this ) );
			}
			else if ( from.Account.Username == m_Owner.Account.Username )
			{
				list.Add( new ReleaseChestEntry( from, this ) );
				//Disabled, so that they won't be too colorful. I need to decide whether to include this feature or not.
				//list.Add( new SelectHueEntry( from, this ) );
			}

			base.GetContextMenuEntries( from, list );
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
			writer.Write( m_Rented );

			if ( m_Rented )
			{
				writer.Write( (Mobile)m_Owner );
				writer.WriteDeltaTime( (DateTime) m_RentalExpireTime );

				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( this.CheckRenewRental ) );
			}
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();

			if ( m_Rented = reader.ReadBool() )
			{
				m_Owner = (PlayerMobile)reader.ReadMobile();
				m_RentalExpireTime = reader.ReadDeltaTime();
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( this.CheckRenewRental ) );
			}
		}

		public void CheckRenewRental()
		{
			if ( DateTime.Now > m_RentalExpireTime )
			{
				if ( Owner != null && Banker.Withdraw( Owner, RentalCost ) )
					m_RentalExpireTime = DateTime.Now + RentalDuration;
				else
					CancelRent();
			}
		}
	}
}