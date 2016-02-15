//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2006					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
namespace Server.Multis.CustomBuilding
{
	public static class Templates
	{
		public const string SBBTemplate =
@"
using System;
using Server.Items;

namespace Server.Multis.CustomBuilding
{
	public class {name} : ScriptBasedBuilding
	{
		public override int GetUpdateRange(Mobile m){ return {updaterange};}

		[Constructable]
		public {name}() : base()
		{
			Name = {namen};
		}

		public {name}( Serial serial ) : base( serial )
		{
		}

		public override void ErectBuilding()
		{
			base.ErectBuilding();
			{addtiles}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}
}";

		public const string SBBATemplate =
@"
using System;
using Server.Items;

namespace Server.Multis.CustomBuilding
{
	public class {name} : ScriptBasedBuildingAddon
	{
		public override int GetUpdateRange(Mobile m){ return {updaterange};}

		[Constructable]
		public {name}() : base()
		{
			Name = {namen};
		}

		public {name}( Serial serial ) : base( serial )
		{
		}

		public override void ErectBuilding()
		{
			base.ErectBuilding();
			{addtiles}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}
}";

		public const string SBABTemplate =
@"
using System;
using Server.Items;

namespace Server.Multis.CustomBuilding
{
	public class {name} : ScriptBasedAddonBuilding
	{
		public override int GetUpdateRange(Mobile m){ return {updaterange};}

		[Constructable]
		public {name}() : base()
		{
			Name = {namen};
			{addcomponents}
		}

		public {name}( Serial serial ) : base( serial )
		{
		}

		public override void ErectBuilding()
		{
			base.ErectBuilding();
			{addtiles}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}
}";
	}
}