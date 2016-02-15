using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Commands;

namespace Server
{
	[Flags]
	public enum CurrencyType
	{
		Coins	= 0x01,
		Checks	= 0x02,
		Both	= 0x03
	}

	public class Currency
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Consume", AccessLevel.GameMaster, new CommandEventHandler( ConsumeGold_OnCommand ) );
		}

		[Usage( "Consume [amount]" )]
		[Description( "Consumes money from a player." )]

		private static void ConsumeGold_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;

			if ( e.Length != 1 )
			{
				from.SendMessage("Invalid Arguements");
				return;
			}

			int amount = Utility.ToInt32(e.Arguments[0]);
			if ( amount <= 0 )
				from.SendMessage( "Invalid amount specified." );
			else
			{
				from.Target = new ConsumeGoldTarget( amount );
				from.SendMessage("Who would you like to consume gold/checks from?");
			}
		}

		private class ConsumeGoldTarget : Target
		{
			private int m_Args;

			public ConsumeGoldTarget( int args ) : base( 15, false, TargetFlags.None )
			{
				m_Args = args;
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				int amt = m_Args;
				if ( amt <= 0 )
					from.SendMessage( "Invalid amount specified." );
				else if (targ is Mobile)
				{
						Mobile m = (Mobile)targ;
						int cnsm = Currency.Consume( m, amt, true, CurrencyType.Both );

						if ( cnsm > 0 )
							from.SendMessage("{0} lacks {1} gp", m.Name, cnsm );
						else if ( cnsm == 0 )
							from.SendMessage("Consumed {0} gp.", amt );
				}
				else
					from.SendMessage("Invalid target specified.");
			}
		}

		public static int Consume( Mobile from, int amount )
		{
			return Consume( from, amount, false );
		}

		public static int Consume( Mobile from, int amount, CurrencyType type )
		{
			return Consume( from, amount, false, type );
		}

		public static int Consume( Mobile from, int amount, CurrencyType type, bool recurse )
		{
			return Consume( new Container[]{ from.Backpack }, amount, type, recurse );
		}

		public static int Consume( Mobile from, int amount, bool bankbox )
		{
			return Consume( from, amount, bankbox, CurrencyType.Coins );
		}

		public static int Consume( Mobile from, int amount, bool bankbox, CurrencyType type )
		{
			return Consume( from, amount, bankbox, type, true );
		}

		public static int Consume( Mobile from, int amount, bool bankbox, CurrencyType type, bool recurse )
		{
			Container[] containers = null;

			if ( bankbox )
				containers = new Container[]{ from.Backpack, from.BankBox };
			else
				containers = new Container[]{ from.Backpack };

			return Consume( containers, amount, type, recurse );
		}

		public static int Consume( Container[] containers, int amount, CurrencyType type, bool recurse )
		{
			if ( amount <= 0 )
				return 0;

			bool checks = (type & CurrencyType.Checks) != 0;
			bool coins = (type & CurrencyType.Coins) != 0;

			int total = 0;
			List<Item[]> list = new List<Item[]>();
			Item[] pack = null;

			for( int i = 0; total < amount && i < containers.Length; i++ )
			{
				Container cont = containers[i] as Container;
				if ( cont != null )
				{
					if ( checks && coins )
						pack = cont.FindItemsByType( new Type[]{ typeof(BankCheck), typeof(Gold) }, recurse );
					else if ( checks )
						pack = cont.FindItemsByType( typeof(BankCheck), recurse );
					else
						pack = cont.FindItemsByType( typeof(Gold), recurse );

					list.Add( pack );

					for( int h = 0; h < pack.Length; h++ )
					{
						Item item = pack[h];

						if ( item is BankCheck)
						{
							if ( checks )
								total += ((BankCheck)item).Worth;
						}
						else if ( coins )
							total += item.Amount;
					}
				}
			}

			if ( total >= amount )
			{
				int need = amount;

				for( int i = 0; need > 0 && i < list.Count; i++ )
				{
					pack = list[i];

					for( int h = 0; h < pack.Length; h++ )
					{
						Item item = pack[h];
						int theirAmount = 0;

						if ( item is BankCheck )
						{
							if ( checks )
								theirAmount = ((BankCheck)item).Worth;
						}
						else if ( coins )
							theirAmount = item.Amount;

						if ( theirAmount < need )
						{
							item.Delete();
							need -= theirAmount;
							continue;
						}
						else
						{
							if ( item is BankCheck )
							{
								if ( checks )
									((BankCheck)item).Worth -= need;
								else
									continue;
							}
							else if ( coins )
								item.Amount -= need;
							else
								continue;

							break;
						}
					}
				}

				return 0;
			}
			else
				return amount - total;
		}
	}
}