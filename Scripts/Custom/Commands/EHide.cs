using System;
using System.Reflection;
using Server.Items;
using Server.Targeting;

namespace Server.Commands
{
	public class EHide
	{
		public static void Initialize()
		{
			CommandSystem.Register("EHide", AccessLevel.GameMaster, new CommandEventHandler(EHide_OnCommand));
		}

		[Usage( "EHide [id]" )]
		[Description( "Hide command." )]
		private static void EHide_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new EHideTarget();
		}

		private class EHideTarget : Target
		{
			public EHideTarget() : base( 15, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				if(targ is Mobile)
				{
					if( from.AccessLevel > ((Mobile)targ).AccessLevel || targ == from ) // Added by Silver
						new ExplotionTimer( (Mobile)targ, 0, TimeSpan.FromMilliseconds( 80.0 ) ).Start();
					else
						from.SendMessage( "That is not accessible." );
				}
			}
		}

		public class ExplotionTimer : Timer
		{
			Mobile m_from;
			int m_iLoc;

			public ExplotionTimer( Mobile from, int loc, TimeSpan delay ) : base( delay )
			{
				m_from = from;
				m_iLoc = loc;
				Priority = TimerPriority.TenMS;
			}

			protected override void OnTick()
			{
				if(m_iLoc <= 9)
				{
					Explotion(m_from, m_iLoc);
					m_iLoc++;
					Start();
				}
				else
				{
					m_from.Hidden = !m_from.Hidden;
					Stop();
				}
			}

			private void Explotion(Mobile m, int loc)
			{
				switch( loc )
				{
					case 0:
						Effects.SendLocationEffect( new Point3D( m.X + 1, m.Y, m.Z + 4 ), m.Map, 0x36BD, 10 );
						break;
					case 1:
						Effects.SendLocationEffect( new Point3D( m.X + 1, m.Y, m.Z ), m.Map, 0x36BD, 10 );
						break;
					case 2:
						Effects.SendLocationEffect( new Point3D( m.X + 1, m.Y, m.Z - 4 ), m.Map, 0x36BD, 10 );
						break;
					case 3:
						Effects.SendLocationEffect( new Point3D( m.X, m.Y + 1, m.Z + 4 ), m.Map, 0x36BD, 10 );
						break;
					case 4:
						Effects.SendLocationEffect( new Point3D( m.X, m.Y + 1, m.Z ), m.Map, 0x36BD, 10 );
						break;
					case 5:
						Effects.SendLocationEffect( new Point3D( m.X, m.Y + 1, m.Z - 4 ), m.Map, 0x36BD, 10 );
						break;
					case 6:
						Effects.SendLocationEffect( new Point3D( m.X + 1, m.Y + 1, m.Z + 11 ), m.Map, 0x36BD, 10 );
						break;
					case 7:
						Effects.SendLocationEffect( new Point3D( m.X + 1, m.Y + 1, m.Z + 7 ), m.Map, 0x36BD, 10 );
						break;
					case 8:
						Effects.SendLocationEffect( new Point3D( m.X + 1, m.Y + 1, m.Z + 3 ), m.Map, 0x36BD, 10 );
						break;
					case 9:
						Effects.SendLocationEffect( new Point3D( m.X + 1, m.Y + 1, m.Z - 1 ), m.Map, 0x36BD, 10 );
						break;
				}
				Effects.PlaySound( m.Location, m.Map, 0x307 );
			}
		}
	}
}