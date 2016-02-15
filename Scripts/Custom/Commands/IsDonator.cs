using Server.Targeting;
using Server.Mobiles;

namespace Server.Commands.Generic
{
	public class IsDonatorCommand : BaseCommand
	{
		public static void Initialize()
		{
			TargetCommands.Register(new IsDonatorCommand());
		}

		public IsDonatorCommand()
		{
			AccessLevel = AccessLevel.Counselor;
			Supports = CommandSupport.AllMobiles;
			Commands = new string[] { "IsDonator" };
			ObjectTypes = ObjectTypes.Mobiles;
			Usage = "IsDonator";
			Description = "Returns if a player has donated.";
		}

		public override void Execute(CommandEventArgs e, object obj)
		{
			Mobile from = e.Mobile;
				if (obj is PlayerMobile)
				{
					PlayerMobile pm = (PlayerMobile)obj;
					if (pm.HasDonated)
					{
						from.SendMessage("Is Donator: true");
						from.SendMessage("Days left: {0}", ((int)pm.DonationTimeLeft.TotalDays).ToString());
					}

					else
						from.SendMessage("Is Donator: false");
				}

				else
					from.SendMessage("That can never be a donator.");
			}
	}
}