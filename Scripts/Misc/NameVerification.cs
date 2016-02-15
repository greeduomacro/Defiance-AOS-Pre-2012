using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Commands;

namespace Server.Misc
{
	public enum NameResultMessage
	{
		Allowed,
		InvalidCharacter,
		TooFewCharacters,
		TooManyCharacters,
		AlreadyExists,
		NotAllowed
	}

	public class NameVerification
	{
		public static readonly char[] SpaceDashPeriodQuote = new char[]
			{
				' ', '-', '.', '\''
			};

		public static readonly char[] Empty = new char[0];

		public static void Initialize()
		{
			EventSink.ValidatePlayerName += new ValidatePlayerNameEventHandler( EventSink_ValidatePlayerName );
			CommandSystem.Register( "ValidateName", AccessLevel.Administrator, new CommandEventHandler( ValidateName_OnCommand ) );
		}

		[Usage( "ValidateName" )]
		[Description( "Checks the result of NameValidation on the specified name." )]
		public static void ValidateName_OnCommand( CommandEventArgs e )
		{
			if ( Validate( e.ArgString, 2, 16, true, false, true, 1, SpaceDashPeriodQuote ) != NameResultMessage.Allowed )
				e.Mobile.SendMessage( 0x59, "That name is considered valid." );
			else
				e.Mobile.SendMessage( 0x22, "That name is considered invalid." );
		}

		public static bool EventSink_ValidatePlayerName( ValidatePlayerNameEventArgs e )
		{
			NetState state = e.State;
			string name = e.Name;
			string lowername = name.ToLower();

			NameResultMessage result = NameVerification.ValidatePlayerName( lowername, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote );

			switch ( result )
			{
				default:
				case NameResultMessage.NotAllowed: SendErrorOnCharacterCreation( state, String.Format( "The name {0} is not allowed.", name ) ); return false;
				case NameResultMessage.InvalidCharacter: SendErrorOnCharacterCreation( state, String.Format( "The name {0} contains invalid characters.", name ) ); return false;
				case NameResultMessage.TooFewCharacters: case NameResultMessage.TooManyCharacters: SendErrorOnCharacterCreation( state, "The name must be between 2-16 characters." ); return false;
				case NameResultMessage.AlreadyExists: SendErrorOnCharacterCreation( state, String.Format( "A player with the name {0} already exists.", name ) ); return false;
				case NameResultMessage.Allowed: return true;
			}
		}

		public static void SendErrorOnCharacterCreation( NetState state, string message )
		{
			Console.WriteLine( "Login: {0}: Character creation failed. {1}", state, message );

			if ( Core.AOS )
				state.Send( SupportedFeatures.Instantiate( state ) );

			state.Send( new CharacterList( state.Account, state.CityInfo ) );

			state.Send( new AsciiMessage( Serial.MinusOne, -1, MessageType.Regular, 38, 0, "System", message ) );
		}

		public static NameResultMessage Validate( string name, int minLength, int maxLength, bool allowLetters, bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions )
		{
			return Validate( name, minLength, maxLength, allowLetters, allowDigits, noExceptionsAtStart, maxExceptions, exceptions, m_Disallowed, m_StartDisallowed );
		}

		public static NameResultMessage Validate( string name, int minLength, int maxLength, bool allowLetters, bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions, string[] disallowed, string[] startDisallowed )
		{
			if ( name == null || name.Length < minLength )
				return NameResultMessage.TooFewCharacters;

			if ( name.Length > maxLength )
				return NameResultMessage.TooManyCharacters;

			int exceptCount = 0;

			name = name.ToLower();

			if ( !allowLetters || !allowDigits || (exceptions.Length > 0 && (noExceptionsAtStart || maxExceptions < int.MaxValue)) )
			{
				for ( int i = 0; i < name.Length; ++i )
				{
					char c = name[i];

					if ( c >= 'a' && c <= 'z' )
					{
						if ( !allowLetters )
							return NameResultMessage.InvalidCharacter;

						exceptCount = 0;
					}
					else if ( c >= '0' && c <= '9' )
					{
						if ( !allowDigits )
							return NameResultMessage.InvalidCharacter;

						exceptCount = 0;
					}
					else
					{
						bool except = false;

						for ( int j = 0; !except && j < exceptions.Length; ++j )
							if ( c == exceptions[j] )
								except = true;

						if ( !except || (i == 0 && noExceptionsAtStart) )
							return NameResultMessage.InvalidCharacter;

						if ( exceptCount++ == maxExceptions )
							return NameResultMessage.InvalidCharacter;
					}
				}
			}

			for ( int i = 0; i < disallowed.Length; ++i )
			{
				int indexOf = name.IndexOf( disallowed[i] );

				if ( indexOf == -1 )
					continue;

				bool badPrefix = ( indexOf == 0 );

				for ( int j = 0; !badPrefix && j < exceptions.Length; ++j )
					badPrefix = ( name[indexOf - 1] == exceptions[j] );

				if ( !badPrefix )
					continue;

				bool badSuffix = ( (indexOf + disallowed[i].Length) >= name.Length );

				for ( int j = 0; !badSuffix && j < exceptions.Length; ++j )
					badSuffix = ( name[indexOf + disallowed[i].Length] == exceptions[j] );

				if ( badSuffix )
					return NameResultMessage.NotAllowed;
			}

			for ( int i = 0; i < startDisallowed.Length; ++i )
				if ( name.StartsWith( startDisallowed[i] ) )
					return NameResultMessage.NotAllowed;

			return NameResultMessage.Allowed;
		}

		public static NameResultMessage ValidatePlayerName( string name, int minLength, int maxLength, bool allowLetters, bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions )
		{
			return ValidatePlayerName( name, minLength, maxLength, allowLetters, allowDigits, noExceptionsAtStart, maxExceptions, exceptions, m_Disallowed, m_StartDisallowed );
		}

		public static NameResultMessage ValidatePlayerName( string name, int minLength, int maxLength, bool allowLetters, bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions, string[] disallowed, string[] startDisallowed )
		{
			NameResultMessage initial = Validate( name, minLength, maxLength, allowLetters, allowDigits, noExceptionsAtStart, maxExceptions, exceptions, disallowed, startDisallowed );
/*
			if ( initial == NameResultMessage.Allowed )
			{
				string lowername = name.ToLower();

				List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );
				foreach ( Mobile m in mobs )
					if ( m is PlayerMobile && m.AccessLevel == AccessLevel.Player && !String.IsNullOrEmpty( m.RawName ) && m.RawName.Trim().ToLower() == lowername )
						return NameResultMessage.AlreadyExists;

				return NameResultMessage.Allowed;
			}
*/
			return initial;
		}

		public static string[] StartDisallowed { get { return m_StartDisallowed; } }
		public static string[] Disallowed { get { return m_Disallowed; } }

		private static string[] m_StartDisallowed = new string[]
			{
				"seer",
				"counselor",
				"gm",
				"admin",
				"lady",
				"lord",
				"staff",
				"lead",
				"trial",
				"dev",
				"founder",
				"gamemaster"
			};

		private static string[] m_Disallowed = new string[]
			{
				"jigaboo",
				"chigaboo",
				"wop",
				"kyke",
				"kike",
				"tit",
				"spic",
				"prick",
				"piss",
				"lezbo",
				"lesbo",
				"felatio",
				"dyke",
				"dildo",
				"chinc",
				"chink",
				"cunnilingus",
				"cum",
				"cocksucker",
				"cock",
				"clitoris",
				"clit",
				"ass",
				"hitler",
				"penis",
				"nigga",
				"nigger",
				"klit",
				"kunt",
				"jiz",
				"jism",
				"jerkoff",
				"jackoff",
				"goddamn",
				"fag",
				"blowjob",
				"bitch",
				"asshole",
				"dick",
				"pussy",
				"snatch",
				"cunt",
				"twat",
				"shit",
				"fuck",
//				"tailor",
//				"smith",
//				"scholar",
//				"rogue",
//				"novice",
//				"neophyte",
//				"merchant",
//				"medium",
//				"master",
//				"mage",
//				"lb",
//				"journeyman",
//				"grandmaster",
//				"fisherman",
//				"expert",
//				"chef",
//				"carpenter",
				"british",
				"blackthorne",
				"blackthorn",
//				"beggar",
//				"archer",
//				"apprentice",
//				"adept",
				"gamemaster",
//				"frozen",
				"squelched",
//				"invulnerable",
				"osi",
				"origin",
				"kurwa",
				"vittu",
				"faggot"
			};
	}
}