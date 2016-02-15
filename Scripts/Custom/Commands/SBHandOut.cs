using System;
using Server.Commands;
using Server.Network;
using Server.Accounting;

namespace Server.Items
{
	public static class SBHandOut
	{
		public static void Initialize()
		{
			CommandSystem.Register("SBHandOut", AccessLevel.Administrator, new CommandEventHandler(HandOut_OnCommand));
		}

		[Usage("SBHandOut")]
		[Description("Skillball handout.")]
		private static void HandOut_OnCommand(CommandEventArgs e)
		{
			Mobile sender = e.Mobile;
			int args = e.Arguments.Length;
			if (args == 8) // Edit by Silver, used to be 7
			{
				int count = 0;

				int amount = 0;
				int bonus = 0;
				int tempdays = 0;
				int max = 100; // Added by Silver
				bool newplayer = false;
				bool accountbound = false;
				bool characterbound = false;
				bool unlimited = false;
				try
				{
					amount = Convert.ToInt32(e.Arguments[0]);
					bonus = Convert.ToInt32(e.Arguments[1]);
					tempdays = Convert.ToInt32(e.Arguments[2]);
					max = Convert.ToInt32(e.Arguments[3]); // Added by Silver
					unlimited = Convert.ToBoolean(e.Arguments[4]);
					newplayer = Convert.ToBoolean(e.Arguments[5]);
					characterbound = Convert.ToBoolean(e.Arguments[6]);
					accountbound = Convert.ToBoolean(e.Arguments[7]);
				}
				catch
				{
					sender.SendMessage("That command is not formatted correctly, the command consists of Command [int amount] [int bonus] [int tempdays] [bool unlimited] [bool newbs] [bool characterbound] [bool accountbound].");
					return;
				}
				DateTime now = DateTime.Now;
				foreach (NetState ns in NetState.Instances)
				{
					Mobile m = ns.Mobile;

					if (m == null || m.AccessLevel > AccessLevel.Player)
						continue;

					Account account = (Account)m.Account;
					TimeSpan createdspan = now - m.CreationTime;

					bool newbie = createdspan < TimeSpan.FromDays( 1.0 ) && ( ( now - account.Created ) < TimeSpan.FromDays( 1.0 ) || account.Length == 1 );

					if ( !newplayer || newbie )
					{
						for ( int i = 0; i < amount; i++ )
						{
							SkillBall ball = new SkillBall( bonus, max, !unlimited, tempdays ); // Silver: max instead of 100
							if ( accountbound )
								ball.OwnerAccount = account.Username;
							if ( characterbound )
								ball.OwnerPlayer = m;

							if ( newplayer )
							{
								ball.LootType = LootType.Newbied;
								ball.MaxCap = max;
							}

							m.AddToBackpack( ball ); // Added by Silver, was lacking
						}

						if ( newbie )
						{
							m.AddToBackpack( new NewPlayerStatsBall( m ) );
							m.SendMessage( 0x482, "Welcome to Defiance AOS, we are very glad you are attending our new player's join day. A skill ball and a stat ball have been placed into your backpack.");
						}
						else
							m.SendMessage( 0x482, "Thank you for supporting our shard. As a token of gratitude a skill ball has been placed into your backpack." );
						count++;
					}
				}
				sender.SendMessage(count + " Players have received skill balls.");
			}
			else
				sender.SendMessage("That command is not formatted correctly - ex: [sbhandout [int amount] [int bonus] [int tempdays] [int max] [bool unlimited] [bool newbs] [bool characterbound] [bool accountbound].");
		}					// Added by Silver: max
	}
}