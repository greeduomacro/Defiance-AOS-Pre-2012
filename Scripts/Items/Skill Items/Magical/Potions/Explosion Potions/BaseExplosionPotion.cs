using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
	public abstract class BaseExplosionPotion : BasePotion
	{
		//public abstract int MinDamage { get; }
		//public abstract int MaxDamage { get; }
		public abstract int Damage{ get; }
		public abstract double Delay { get; }

		public override bool RequireFreeHand{ get{ return false; } }

		private static bool LeveledExplosion = false; // Should explosion potions explode other nearby potions?
		private static bool InstantExplosion = false; // Should explosion potions explode on impact?
		private const int   ExplosionRange   = 2;     // How long is the blast radius?

		public BaseExplosionPotion( PotionEffect effect ) : base( 0xF0D, effect )
		{
		}

		public BaseExplosionPotion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public virtual object FindParent( Mobile from )
		{
			Mobile m = this.HeldBy;

			if ( m != null && m.Holding == this )
				return m;

			object obj = this.RootParent;

			if ( obj != null )
				return obj;

			if ( Map == Map.Internal )
				return from;

			return this;
		}

		private Timer m_Timer;

//		private List<Mobile> m_Users;

		public override void Drink( Mobile from )
		{
			if ( Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)) )
				from.SendLocalizedMessage( 1062725 ); // You can not use that potion while paralyzed.
			else
			{
				ThrowTarget targ = from.Target as ThrowTarget;
				this.Stackable = false; // Scavenged explosion potions won't stack with those ones in backpack, and still will explode.

				if ( targ != null && targ.Potion == this ) //Already have a targeter from this potion
					return;

				from.RevealingAction();

//				if ( m_Users == null )
//					m_Users = new List<Mobile>();

				if ( m_Timer == null ) //Is this already ticking?
				{
					if ( from.BeginAction( typeof( BaseExplosionPotion ) ) ) //Can we throw another potion?
					{
//						if ( !m_Users.Contains( from ) )
//							m_Users.Add( from );

						from.Target = new ThrowTarget( this );

						from.SendLocalizedMessage( 500236 ); // You should throw it now!

						int count = Utility.Random( 3, 2 );

						m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 0.75 ), TimeSpan.FromSeconds( 1.0 ), count+1, new TimerStateCallback( Detonate_OnTick ), new object[]{ from, count } );
						Timer.DelayCall( TimeSpan.FromSeconds( Delay ), new TimerStateCallback( ReleaseExplosionLock ), from );
					}
					else
						from.LocalOverheadMessage( MessageType.Regular, 0x22, false, "You must wait a moment before using another explosion potion." );
				}
				else
					from.Target = new ThrowTarget( this );
			}
		}

		private static void ReleaseExplosionLock( object state )
		{
			if ( state is Mobile )
				((Mobile)state).EndAction( typeof( BaseExplosionPotion ) );
		}

		private void Detonate_OnTick( object state )
		{
			if ( Deleted )
				return;

			object[] states = (object[])state;
			Mobile from = (Mobile)states[0];
			int timer = (int)states[1];

//			from.SendMessage( "Click!" );

			object parent = FindParent( from );

			if ( timer == 0 )
			{
				Point3D loc;
				Map map;

				if ( parent is Item )
				{
					Item item = (Item)parent;

					loc = item.GetWorldLocation();
					map = item.Map;
				}
				else if ( parent is Mobile )
				{
					Mobile m = (Mobile)parent;
//					from.SendMessage( "Ahh you're holding an explosion potion!" );
					loc = m.Location;
					map = m.Map;
				}
				else
				{
//					from.SendMessage( "Fail: No Explosion Potion Explosion" );
					return;
				}

				Explode( from, true, loc, map );
				m_Timer = null;
			}
			else
			{
				if ( parent is Item )
					((Item)parent).PublicOverheadMessage( MessageType.Regular, 0x22, false, timer.ToString() );
				else if ( parent is Mobile )
					((Mobile)parent).PublicOverheadMessage( MessageType.Regular, 0x22, false, timer.ToString() );

				states[1] = timer - 1;
			}
		}

		private void Reposition_OnTick( object state )
		{
			if ( Deleted )
				return;

			object[] states = (object[])state;
			Mobile from = states[0] as Mobile;
			IPoint3D p = (IPoint3D)states[1];
			Map map = (Map)states[2];

			Point3D loc = new Point3D( p );

			if ( InstantExplosion )
				Explode( from, true, loc, map );
			else
				MoveToWorld( loc, map );
		}

		private class ThrowTarget : Target
		{
			private BaseExplosionPotion m_Potion;

			public BaseExplosionPotion Potion
			{
				get{ return m_Potion; }
			}

			public ThrowTarget( BaseExplosionPotion potion ) : base( 12, true, TargetFlags.None )
			{
				m_Potion = potion;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Potion.Deleted || m_Potion.Map == Map.Internal )
					return;

				IPoint3D p = targeted as IPoint3D;

				if ( p == null )
					return;

				Map map = from.Map;

				if ( map == null )
					return;

				SpellHelper.GetSurfaceTop( ref p );

				from.RevealingAction();

				IEntity to;

				if ( p is Mobile )
					to = (Mobile)p;
				else
					to = new Entity( Serial.Zero, new Point3D( p ), map );

				Effects.SendMovingEffect( from, to, m_Potion.ItemID & 0x3FFF, 7, 0, false, false, m_Potion.Hue, 0 );

				if( m_Potion.Amount > 1 )
					Mobile.LiftItemDupe( m_Potion, 1 );

				m_Potion.Internalize();
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( m_Potion.Reposition_OnTick ), new object[]{ from, p, map } );
			}
		}

		public void Explode( Mobile from, bool direct, Point3D loc, Map map )
		{
			if ( Deleted )
				return;

			Consume();

			ThrowTarget targ = from.Target as ThrowTarget;

			if ( targ != null && targ.Potion == this ) //Blew yourself up without throwing it, cancel target now
				Target.Cancel( from );

			if ( map == null )
				return;

			Effects.PlaySound( loc, map, 0x207 );
			Effects.SendLocationEffect( loc, map, 0x36BD, 20 );

			int alchemyBonus = 0;

			if ( direct )
				alchemyBonus = (int)(from.Skills.Alchemy.Value / (Core.AOS ? 5 : 10));

			IPooledEnumerable eable = LeveledExplosion ? map.GetObjectsInRange( loc, ExplosionRange ) : map.GetMobilesInRange( loc, ExplosionRange );
			ArrayList toExplode = new ArrayList();

			int toDamage = 0;

			foreach ( object o in eable )
			{
				if ( o is Mobile )
				{
					toExplode.Add( o );
					++toDamage;
				}
				else if ( o is BaseExplosionPotion && o != this )
				{
					toExplode.Add( o );
				}
			}

			eable.Free();

//			int min = Scale( from, MinDamage );
//			int max = Scale( from, MaxDamage );

			for ( int i = 0; i < toExplode.Count; ++i )
			{
				object o = toExplode[i];

				if ( o is Mobile )
				{
					Mobile m = (Mobile)o;

					if ( from == null || (SpellHelper.ValidIndirectTarget( from, m ) && from.CanBeHarmful( m, false )) )
					{
						if ( from != null )
							from.DoHarmful( m );

						int damage = Scale( from, Damage );

						damage += alchemyBonus;

						if ( !Core.AOS && damage > 40 )
							damage = 40;

						if ( Core.AOS && toDamage > 2 )
							damage /= toDamage - 1;

						if ( !Core.AOS && from is PlayerMobile )
							damage /= 2;

						AOS.Damage( m, from, damage, 0, 100, 0, 0, 0 );
					}
				}
				else if ( o is BaseExplosionPotion )
				{
					BaseExplosionPotion pot = (BaseExplosionPotion)o;

					pot.Explode( from, false, pot.GetWorldLocation(), pot.Map );
				}
			}
		}
	}
}