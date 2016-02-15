using System;
using Server.Network;
using Server.Items;

namespace Server
{
	public class CurrentExpansion
	{
		private static readonly Expansion Expansion = Expansion.AOS;

		public static void Configure()
		{
			ExpansionInfo.Table[5] = new ExpansionInfo( 5, "Age of Shadows", ClientFlags.Malas, FeatureFlags.ExpansionAOS | FeatureFlags.EleventhAgeSplash | FeatureFlags.NinthAgeSplash, CharacterListFlags.ExpansionAOS, 0x02E0 );
			Core.Expansion = Expansion;

			bool Enabled = Core.AOS;

			Mobile.InsuranceEnabled = Enabled;
			ObjectPropertyList.Enabled = Enabled;
			Mobile.VisibleDamageType = Enabled ? VisibleDamageType.Related : VisibleDamageType.None;
			Mobile.GuildClickMessage = !Enabled;
			Mobile.AsciiClickMessage = !Enabled;

			if ( Enabled )
			{
				AOS.DisableStatInfluences();

				if ( ObjectPropertyList.Enabled )
					PacketHandlers.SingleClickProps = true; // single click for everything is overridden to check object property list
			}

			Item.DefaultDyeType = typeof(DyeTub);
		}
	}
}