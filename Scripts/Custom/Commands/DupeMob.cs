using System;
using System.Reflection;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
	public class DupeMob
	{
		public static void Initialize()
		{
			CommandSystem.Register( "DupeMob", AccessLevel.GameMaster, new CommandEventHandler( DupeMob_OnCommand ) );
		}

		[Usage( "DupeMob" )]
		[Description( "Dupes a targeted mobile." )]
		private static void DupeMob_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new DupeTarget( );
			e.Mobile.SendMessage( "What do you wish to dupe?" );
		}


		private class DupeTarget : Target
		{
			private int m_Amount;

			public DupeTarget( )
				: base( 15, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				bool done = false;
				if ( !( targ is BaseCreature ) )
				{
					from.SendMessage( "This command works only on BaseCreatures." );
					return;
				}

				CommandLogging.WriteLine( from, "{0} {1} duping Mobile {2}", from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( targ ) );

				BaseCreature copy = (BaseCreature)targ;

				Type t = copy.GetType();

				//ConstructorInfo[] info = t.GetConstructors();

				ConstructorInfo c = t.GetConstructor( Type.EmptyTypes );

				if ( c != null )
				{
					try
					{
						from.SendMessage( "Duping...");
							object o = c.Invoke( null );

							if ( o != null && o is BaseCreature )
							{
								BaseCreature newMob = (BaseCreature)o;
								CopyProperties( newMob, copy );//copy.Dupe( item, copy.Amount );
								//copy.OnAfterDuped( newMob );
								//newMob.Parent = null;
									newMob.MoveToWorld( from.Location, from.Map );

								CommandLogging.WriteLine( from, "{0} {1} duped {2} creating {3}", from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( targ ), CommandLogging.Format( newMob ) );
							}

						from.SendMessage( "Done" );
						done = true;
					}
					catch
					{
						from.SendMessage( "Error!" );
						return;
					}
				}

				if ( !done )
				{
					from.SendMessage( "Unable to dupe.  Mob must have a 0 parameter constructor." );
				}
			}
		}

		private static void CopyProperties( BaseCreature dest, BaseCreature src )
		{
			PropertyInfo[] props = src.GetType().GetProperties();

			for ( int i = 0; i < props.Length; i++ )
			{
				try
				{
					if ( props[i].CanRead && props[i].CanWrite )
					{
						//Console.WriteLine( "Setting {0} = {1}", props[i].Name, props[i].GetValue( src, null ) );
						props[i].SetValue( dest, props[i].GetValue( src, null ), null );
					}
				}
				catch
				{
					//Console.WriteLine( "Denied" );
				}
			}


		}
	}
}