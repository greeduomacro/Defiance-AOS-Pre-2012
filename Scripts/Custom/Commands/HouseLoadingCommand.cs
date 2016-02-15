using Server.Mobiles;


namespace Server.Commands
{
	public static class HouseLoadingCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register("ToggleHouseLoading", AccessLevel.GameMaster, new CommandEventHandler(ToggleOSIHouseLoading_OnCommand));
		}

		[Usage("ToggleHouseLoading")]
		[Description("Changes the way of loading items inside houses.")]
		public static void ToggleOSIHouseLoading_OnCommand(CommandEventArgs e)
		{
			PlayerMobile from = (PlayerMobile)e.Mobile;

			from.OSIHouseLoading = !from.OSIHouseLoading;
		}
	}
}