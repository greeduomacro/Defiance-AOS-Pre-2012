using System;
using System.Collections;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Events.CTF
{
	public abstract class CTFTeam
	{
		#region Members
		public abstract string Name { get; }
		public abstract int Hue { get; }
		public abstract int Number { get; }
		#endregion
	}

	public class CTFBlueTeam : CTFTeam
	{
		#region Members
		public override string Name { get { return "Blue Angels"; } }
		public override int Hue { get { return 1266; } }
		public override int Number { get { return 1; } }
		#endregion

		public CTFBlueTeam()
		{
		}
	}

	public class CTFBlackTeam : CTFTeam
	{
		#region Members
		public override string Name { get { return "Black Knights"; } }
		public override int Hue { get { return 1109; } }
		public override int Number { get { return 0; } }
		#endregion

		public CTFBlackTeam()
		{
		}
	}

	public class CTFRedTeam : CTFTeam
	{
		#region Members
		public override string Name { get { return "Red Devils"; } }
		public override int Hue { get { return 33; } }
		public override int Number { get { return 2; } }
		#endregion

		public CTFRedTeam()
		{
		}
	}

	public class CTFWhiteTeam : CTFTeam
	{
		#region Members
		public override string Name { get { return "White Ghosts"; } }
		public override int Hue { get { return 1150; } }
		public override int Number { get { return 3; } }
		#endregion

		public CTFWhiteTeam()
		{
		}
	}
}