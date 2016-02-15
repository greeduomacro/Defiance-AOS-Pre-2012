using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Commands;

namespace Server.Misc
{
	public class AutoSave : Timer
	{
		private static TimeSpan m_MaxBackup = TimeSpan.FromDays( 7.0 ); //No more than a week.
		private static TimeSpan m_Delay = TimeSpan.FromMinutes( 60.0 );
		private static TimeSpan m_Warning = TestCenter.Enabled ? TimeSpan.Zero : TimeSpan.FromSeconds( 60.0 );

		public static void Initialize()
		{
			new AutoSave().Start();
			CommandSystem.Register( "SetSaves", AccessLevel.Administrator, new CommandEventHandler( SetSaves_OnCommand ) );
		}

		private static bool m_SavesEnabled = true;
		private static bool m_BackingUp = false;

		public static bool SavesEnabled
		{
			get{ return m_SavesEnabled; }
			set{ m_SavesEnabled = value; }
		}

		public static bool BackingUp
		{
			get{ return m_BackingUp; }
			set{ m_BackingUp = value; }
		}

		[Usage( "SetSaves <true | false>" )]
		[Description( "Enables or disables automatic shard saving." )]
		public static void SetSaves_OnCommand( CommandEventArgs e )
		{
			if ( e.Length == 1 )
			{
				m_SavesEnabled = e.GetBoolean( 0 );
				e.Mobile.SendMessage( "Saves have been {0}.", m_SavesEnabled ? "enabled" : "disabled" );
			}
			else
			{
				e.Mobile.SendMessage( "Format: SetSaves <true | false>" );
			}
		}

		public AutoSave() : base( m_Delay - m_Warning, m_Delay )
		{
			Priority = TimerPriority.OneMinute;
		}

		protected override void OnTick()
		{
			if ( !m_SavesEnabled || AutoRestart.Restarting )
				return;

			if ( m_Warning == TimeSpan.Zero )
				Save();
			else
			{
				int s = (int)m_Warning.TotalSeconds;
				int m = s / 60;
				s %= 60;

				if ( m > 0 && s > 0 )
					World.Broadcast( 0x35, true, "The world will save in {0} minute{1} and {2} second{3}.", m, m != 1 ? "s" : "", s, s != 1 ? "s" : "" );
				else if ( m > 0 )
					World.Broadcast( 0x35, true, "The world will save in {0} minute{1}.", m, m != 1 ? "s" : "" );
				else
					World.Broadcast( 0x35, true, "The world will save in {0} second{1}.", s, s != 1 ? "s" : "" );

				m_BackingUp = true; //Just in case the new thread is delayed somehow
				Thread thread = new Thread( new ThreadStart( BackupByDate ) );
				thread.Name = "Server.Misc.AutoSave.Backup";
				thread.Priority = ThreadPriority.BelowNormal;
				thread.Start();

				Timer.DelayCall( m_Warning, new TimerCallback( Save ) );
			}
		}

		public static void Save()
		{
			if ( AutoRestart.Restarting || AutoRestart.ServerWars )
				return;

			if ( m_BackingUp )
				Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( Save ) );
			else
			{
				foreach ( Mobile m in World.Mobiles.Values )
				{
					if ( m is PlayerMobile )
					{
						if ( m.Account == null )
						{
							Console.WriteLine( "WARNING: Orphan player deleted - {0} [{1}]: {2}", m.Location, m.Map, m.Name );
							m.Delete();
						}
					}
				}

	//			SaveGump.ShowSaveGump();
				World.Save( false );
	//			SaveGump.CloseSaveGump();

				if ( AutoPublish.Enabled )
					new PublishCheck();
			}
		}

		private static string[] m_Backups = new string[]
			{
				"Third Backup",
				"Second Backup",
				"Most Recent"
			};

		private static void Backup()
		{
			if ( m_Backups.Length == 0 )
				return;

			try
			{
				string root = Path.Combine( Core.BaseDirectory, "Backups/Automatic" );

				if ( !Directory.Exists( root ) )
					Directory.CreateDirectory( root );

				string[] existing = Directory.GetDirectories( root );

				for ( int i = 0; i < m_Backups.Length; ++i )
				{
					DirectoryInfo dir = Match( existing, m_Backups[i] );

					if ( dir == null )
						continue;

					if ( i > 0 )
					{
						string timeStamp = FindTimeStamp( dir.Name );

						if ( timeStamp != null )
						{
							try{ dir.MoveTo( FormatDirectory( root, m_Backups[i - 1], timeStamp ) ); }
							catch{}
						}
					}
					else
					{
						try{ dir.Delete( true ); }
						catch{}
					}
				}

				string saves = Path.Combine( Core.BaseDirectory, "Saves" );

				if ( Directory.Exists( saves ) )
					Directory.Move( saves, FormatDirectory( root, m_Backups[m_Backups.Length - 1], GetTimeStamp() ) );
			}
			catch ( Exception e )
			{
				Console.Write( "WARNING: Automatic backup " );
				Utility.PushColor( ConsoleColor.Red );
				Console.Write( "FAILED" );
				Utility.PopColor();
				Console.WriteLine( ": {0}\nWARNING: Previous save is still in the Saves folder.", e );
			}

			m_BackingUp = false;
		}

		private static string[] m_Months = new string[]
			{
				"January", "February", "March", "April", "May",
				"June", "July", "August", "September", "October",
				"November", "December"
			};

		public static void BackupByDate()
		{
			m_BackingUp = true;

			try
			{
				DateTime timestamp = DateTime.Now;

				string root = Path.Combine( Core.BaseDirectory, "Backups/Automatic" );

				if ( !Directory.Exists( root ) )
					Directory.CreateDirectory( root );

				//Lets clean up old saves, more than 1 week ago.
				DateTime maxback = timestamp - m_MaxBackup;

				string[] subdirs = Directory.GetDirectories( root );
				List<string> todel = new List<string>();
				if ( subdirs.Length > 3 ) //More than three prior saves found
				{
					for ( int i = 0; i < subdirs.Length; i++ )
					{
						DirectoryInfo subdir = new DirectoryInfo( subdirs[i] );
						string[] tsplit = subdir.Name.Split( '-' );
						DateTime timeframe = new DateTime( Utility.ToInt32(tsplit[0]),
							Utility.ToInt32(tsplit[1]), Utility.ToInt32(tsplit[2]),
							Utility.ToInt32(tsplit[3]), Utility.ToInt32(tsplit[4]), Utility.ToInt32(tsplit[5]) );
						if ( timeframe < maxback ) //Is it earlier than the maximum backup date?
							todel.Add( subdirs[i] );
					}

					if ( todel.Count > 0 && ( subdirs.Length - todel.Count >= 3 ) ) //At least three saves left
					{
						for ( int i = 0; i < todel.Count; i++ )
							Directory.Delete( todel[i], true );
						Console.WriteLine( "Backups: Removed {0} save{1} from before {2:MMMM dd, yyyy HH':'mm}", todel.Count, todel.Count != 1 ? "s" : "", maxback );
					}
				}

				string folder = Path.Combine( root, GetSaveTimeStamp( timestamp ) );

				if ( Directory.Exists( folder ) ) //Split it into milliseconds if there is already a save for this time frame
					folder = String.Format( "{0}-{1:D3}", folder, timestamp.Millisecond );

				string saves = Path.Combine( Core.BaseDirectory, "Saves" );

				if ( Directory.Exists( saves ) )
					Directory.Move( saves, folder ); //We will make an exception here, or during the actual new save.
			}
			catch ( Exception e )
			{
				Console.Write( "WARNING: Automatic backup " );
				Utility.PushColor( ConsoleColor.Red );
				Console.Write( "FAILED" );
				Utility.PopColor();
				Console.WriteLine( ": {0}\nWARNING: Previous save is still in the Saves folder.", e );
			}

			m_BackingUp = false;
		}

		private static DirectoryInfo Match( string[] paths, string match )
		{
			for ( int i = 0; i < paths.Length; ++i )
			{
				DirectoryInfo info = new DirectoryInfo( paths[i] );

				if ( info.Name.StartsWith( match ) )
					return info;
			}

			return null;
		}

		private static string FormatDirectory( string root, string name, string timeStamp )
		{
			return Path.Combine( root, String.Format( "{0} ({1})", name, timeStamp ) );
		}

		private static string FindTimeStamp( string input )
		{
			int start = input.IndexOf( '(' );

			if ( start >= 0 )
			{
				int end = input.IndexOf( ')', ++start );

				if ( end >= start )
					return input.Substring( start, end-start );
			}

			return null;
		}

		private static string GetSaveTimeStamp( DateTime now )
		{
			return String.Format( "{0}-{1:D2}-{2:D2}-{3:D2}-{4:D2}-{5:D2}",
					now.Year,
					now.Month,
					now.Day,
					now.Hour,
					now.Minute,
					now.Second
				);
		}

		private static string GetTimeStamp()
		{
			return GetTimeStamp( DateTime.Now );
		}

		private static string GetTimeStamp( DateTime now )
		{
			return String.Format( "{0}-{1}-{2}-{3}-{4:D2}-{5:D2}",
					now.Day,
					now.Month,
					now.Year,
					now.Hour,
					now.Minute,
					now.Second
				);
		}
	}
}