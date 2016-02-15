using System;
using System.Collections;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Server.Commands
{
	public class PetBondTime
	{
		public static void Initialize()
		{
			CommandSystem.Register( "PetBondTime", AccessLevel.Player, new CommandEventHandler( PetBondTime_OnCommand ) );
		}

		[Usage( "PetBondTime" )]
		[Description( "Show's how long you have to wait till your pet is bonded." )]
		public static void PetBondTime_OnCommand( CommandEventArgs e )
		{
			if ( e.Mobile is PlayerMobile )
			{
				e.Mobile.Target = new PetBondTimeTarget();
				e.Mobile.SendMessage( "Target your pet." );
			}
		}

		private class PetBondTimeTarget : Target
		{
			public PetBondTimeTarget() : base( -1, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( from is PlayerMobile && targeted is Mobile )
				{
					PlayerMobile pm = (PlayerMobile)from;
					Mobile target = (Mobile)targeted;

					if( target is PlayerMobile )
					{
						pm.SendMessage( "That's not a pet!" );
					}
					else if( target is BaseCreature )
					{
						BaseCreature targ = (BaseCreature)target;
						bool hasSkill = ( targ.MinTameSkill <= 29.1 || pm.Skills[SkillName.AnimalTaming].Value >= targ.MinTameSkill );

						if( targ.ControlMaster == null )
						{
							pm.SendMessage( "That creature is not tamed." );
						}
						else if( targ.ControlMaster != pm )
						{
							pm.SendMessage( "That creature doesn't belong to you." );
						}
						else
						{
							if( !targ.IsBondable )
							{
								pm.SendMessage( "That creature cannot be bonded." );
							}
							else if( targ.IsBonded )
							{
								pm.SendMessage( "That creature is already bonded." );
							}
							else if( targ.BondingBegin == DateTime.MinValue )
							{
								pm.SendMessage( "That creature is not currently in the process of bonding." );
								if( !hasSkill )
								{
									pm.SendMessage( "You do not currently have enough taming skill to finish the bonding process." );
								}
							}
							else if( targ.BondingBegin + targ.BondingDelay < DateTime.Now )
							{
								pm.SendMessage( "That creature is ready to bond." );
								if( !hasSkill )
								{
									pm.SendMessage( "You do not currently have enough taming skill to finish the bonding process." );
								}
							}
							else
							{
								TimeSpan timeRemaining = targ.BondingDelay - ( DateTime.Now - targ.BondingBegin );
								pm.SendMessage( "That creature will be ready to bond in " + timeRemaining.Days + " days and " + timeRemaining.Hours + " hours." );
								if( !hasSkill )
								{
									pm.SendMessage( "You do not currently have enough taming skill to finish the bonding process." );
								}
							}
						}
					}
				}
			}
		}
	}
}