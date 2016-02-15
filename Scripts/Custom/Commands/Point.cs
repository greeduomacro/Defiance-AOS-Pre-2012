using System;
using System.Collections;
using Server;
using Server.Commands;
using Server.Items;
using Server.Spells;
using Server.Targeting;

namespace Server.Scripts.Commands
{
	public class Point
	{
		public static void Initialize()
		{
			CommandSystem.Register("Point", AccessLevel.Player, new CommandEventHandler(Point_OnCommand));
		}

		[Usage( "Point" )]
		[Description("Point at something and everyone around you will see it.")]
		public static void Point_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.Target = new InternalTarget();
			from.SendMessage("Target the item you want to point.");
		}

		public static void Target (IPoint3D p, Mobile from)
		{
			if ( !m_Table.Contains( from ))
			{
				new InternalTimer( from ).Start();
				SpellHelper.Turn( from, p );
				string text = string.Format( "* {0} points here *", from.Name );
				Point3D point = new Point3D( p );
				Map map = from.Map;
				EffectItem ei;
				Effects.SendLocationParticles( ei = EffectItem.Create( point, map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 );

				foreach (Mobile m in from.GetMobilesInRange(18))
						if (m != null && m.Player)
							MessageHelper.SendLocalizedMessageTo((Item)ei, m, 1070722, text, 18);
			}
			else from.SendMessage("You must wait a few seconds until you can point again.");
		}

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 12, true, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				IPoint3D p = targeted as IPoint3D;
				if ( p != null )
					Point.Target( p, from );
			}
		}

		private static Hashtable m_Table = new Hashtable();

		private class InternalTimer : Timer
		{
			private Mobile m_Mobile;

			public InternalTimer( Mobile m ) : base( TimeSpan.FromSeconds( 10.0 ) )
			{
				Priority = TimerPriority.OneSecond;

				m_Mobile = m;
				m_Table[m] = this;
			}

			protected override void OnTick()
			{
				m_Table.Remove( m_Mobile );
			}
		}

	}
}