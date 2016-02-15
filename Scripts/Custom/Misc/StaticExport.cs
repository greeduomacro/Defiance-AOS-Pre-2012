//By Nerun
using System;
using System.IO;
using System.Text;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Commands;
using Server.Targeting;

namespace Server.Commands
{
	public class StaticExport
	{
		public static void Initialize()
		{
			CommandSystem.Register( "StaticExporter" , AccessLevel.Administrator, new CommandEventHandler( StaticExport_OnCommand ) );
			CommandSystem.Register( "StaEx" , AccessLevel.Administrator, new CommandEventHandler( StaticExport_OnCommand ) );
		}

		[Usage( "StaticExport" )]
		[Aliases( "StaEx" )]
		[Description( "Convert Statics in a cfg decoration file." )]
		public static void StaticExport_OnCommand( CommandEventArgs e )
		{
			if ( e.Arguments.Length == 5 )
			{
				string file = e.Arguments[0];
				int x1 = Utility.ToInt32( e.Arguments[1] );
				int y1 = Utility.ToInt32( e.Arguments[2] );
				int x2 = Utility.ToInt32( e.Arguments[3] );
				int y2 = Utility.ToInt32( e.Arguments[4] );
				Export( e.Mobile, file, x1, y1, x2, y2 );
			}
			else
			{
				if ( e.Arguments.Length == 1 )
				{
					string file = e.Arguments[0];
					BeginStaEx( e.Mobile, file );
				}
				else
				{
					e.Mobile.SendMessage( "Usage: StaticExport filename [X1 Y1 X2 Y2]" );
				}
			}
		}

		public static void BeginStaEx( Mobile from, string file )
		{
			BoundingBoxPicker.Begin( from, new BoundingBoxCallback( StaExBox_Callback ), new object[]{ file } );
		}

		private static void StaExBox_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
		{
			object[] states = (object[])state;
			string file = (string)states[0];

			Export( from, file, start.X, start.Y, end.X, end.Y );
		}

		private static void Export( Mobile from, string file, int X1, int Y1, int X2, int Y2 )
		{
				int x1 = X1;
				int y1 = Y1;
				int x2 = X2;
				int y2 = Y2;

				if(X1 > X2)
				{
					x1 = X2;
					x2 = X1;
				}

				if(Y1 < Y2)
				{
					y1 = Y2;
					y2 = Y1;
				}


			Map map = from.Map;
			ArrayList list = new ArrayList();

			if ( !Directory.Exists( @".\Export\" ) )
				Directory.CreateDirectory( @".\Export\" );

			using ( StreamWriter op = new StreamWriter( String.Format( @".\Export\{0}.cfg", file ) ) )
			{

				from.SendMessage( "Saving Statics..." );

				op.WriteLine( "# Saved By Static Exporter" );
				op.WriteLine( "# StaticExport by Nerun" );
				op.WriteLine( "" );

				foreach ( Item item in World.Items.Values )
				{
					if ( item.Decays == false && item.Movable == false && item.Parent == null && ( ( item.X >= x1 && item.X <= x2 ) && ( item.Y <= y1 && item.Y >= y2 ) && item.Map == map ) )
					{
						list.Add( item );
					}
				}

				foreach ( Item item in list )
				{
					if (item.Hue == 0)
					{
						if (item.ItemID == 3948)
						{
							op.WriteLine( "Moongate 3948 (Target={0}; TargetMap={1})", ((Moongate)item).Target, ((Moongate)item).TargetMap );
							op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
							op.WriteLine( "" );
						}
						if (item is Teleporter)
						{
							op.WriteLine( "Teleporter 7107 (PointDest={0}; MapDestination={1})", ((Teleporter)item).PointDest, ((Teleporter)item).MapDest );
							op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
							op.WriteLine( "" );
						}
						if (item is KeywordTeleporter)
						{
							op.WriteLine( "KeywordTeleporter 7107 (PointDest={0}; MapDestination={1}; Range={2}; Substring={3})", ((KeywordTeleporter)item).PointDest, ((KeywordTeleporter)item).MapDest, ((KeywordTeleporter)item).Range, ((KeywordTeleporter)item).Substring );
							op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
							op.WriteLine( "" );
						}
						else
						{
							op.WriteLine( "static {0}", item.ItemID );
							op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
							op.WriteLine( "" );
						}
					}
					else
					{
						if (item.ItemID == 3948)
						{
							op.WriteLine( "Moongate 3948 (Hue={0}; Target={1}; TargetMap={2})", item.Hue, ((Moongate)item).Target, ((Moongate)item).TargetMap );
							op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
							op.WriteLine( "" );
						}
						if (item is Teleporter)
						{
							op.WriteLine( "Teleporter 7107 (Hue={0}; PointDest={1}; MapDestination={2})", item.Hue, ((Teleporter)item).PointDest, ((Teleporter)item).MapDest );
							op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
							op.WriteLine( "" );
						}
						else
						{
							op.WriteLine( "static {0} (Hue={1})", item.ItemID, item.Hue );
							op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
							op.WriteLine( "" );
						}
					}
				}

				from.SendMessage( String.Format( "You exported {0} Statics from this facet.", list.Count ) );
			}
		}
	}
}