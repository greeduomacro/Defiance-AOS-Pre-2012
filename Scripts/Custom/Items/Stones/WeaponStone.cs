using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Menus;
using Server.Menus.Questions;
using Server.Items;
using System.Collections;

namespace Server.Items
{
	public class WeaponStone : Item
	{
		public class WeaponStoneEntry
		{
			public string m_sName;
			public int m_iCount;

			public WeaponStoneEntry( string name, int count )
			{
				m_sName = name;
				m_iCount = count;
			}
		}
		public ArrayList m_alNameList;

		bool m_bUseLimit;
		[CommandProperty( AccessLevel.GameMaster )]
		public bool UseLimit
		{
			get { return m_bUseLimit; }
			set { m_bUseLimit = value; }
		}

		[Constructable]
		public WeaponStone() : base( 0xED4 )
		{
			Hue = 0x5A7;
			Movable = false;
			Name = "a Weapon Stone";
			m_alNameList = new ArrayList();
		}

		public WeaponStone( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 2 ) )
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			else
				from.SendGump( new WeaponStoneGump(from, this, 1) );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			// Version 1
			writer.Write( (bool) m_bUseLimit );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_alNameList = new ArrayList();

			switch( version )
			{
				case 1:
					m_bUseLimit = reader.ReadBool();
					break;
			}
		}
	}
}

namespace Server.Gumps
{
	public class WeaponStoneGump : Gump
	{
		private static int m_iStackableAmount = 100;
		private static int m_iAmountToGive = 5;

		public class ItemEntry
		{
			public Type m_Type;
			public string m_sName;

			public ItemEntry( Type type, string name )
			{
				m_Type = type;
				m_sName = name;
			}
		}

		private static ItemEntry[] m_SwordsTable = new ItemEntry[]
			{
				new ItemEntry( typeof(BoneHarvester), "Bone Harvester" ),
				new ItemEntry( typeof(Broadsword), "Broadsword" ),
				new ItemEntry( typeof(CrescentBlade), "Crescent Blade" ),
				new ItemEntry( typeof(Cutlass), "Cutlass" ),
				new ItemEntry( typeof(Katana), "Katana" ),
				new ItemEntry( typeof(Kryss), "Kryss" ),
				new ItemEntry( typeof(Lance), "Lance" ),
				new ItemEntry( typeof(Longsword), "Longsword" ),
				new ItemEntry( typeof(Scimitar), "Scimitar" ),
				new ItemEntry( typeof(ThinLongsword), "Thin Longsword" ),
				new ItemEntry( typeof(VikingSword), "Viking Sword" ),
			};

		private static ItemEntry[] m_SpearsAndForksTable = new ItemEntry[]
			{
				new ItemEntry( typeof(BladedStaff), "Bladed Staff" ),
				new ItemEntry( typeof(DoubleBladedStaff), "Double Bladed Staff" ),
				new ItemEntry( typeof(Pike), "Pike" ),
				new ItemEntry( typeof(Pitchfork), "Pitchfork" ),
				new ItemEntry( typeof(ShortSpear), "Short Spear" ),
				new ItemEntry( typeof(Spear), "Spear" ),
				new ItemEntry( typeof(TribalSpear), "Tribal Spear" ),
				new ItemEntry( typeof(WarFork), "War Fork" ),
			};

		private static ItemEntry[] m_PoleArmsTable = new ItemEntry[]
			{
				new ItemEntry( typeof(Bardiche), "Bardiche" ),
				new ItemEntry( typeof(Halberd), "Halberd" ),
				new ItemEntry( typeof(Scythe), "Scythe" ),
			};

		private static ItemEntry[] m_MacesTable = new ItemEntry[]
			{
				new ItemEntry( typeof(Club), "Club" ),
				new ItemEntry( typeof(HammerPick), "Hammer Pick" ),
				new ItemEntry( typeof(Mace), "Mace" ),
				new ItemEntry( typeof(Maul), "Maul" ),
				new ItemEntry( typeof(Scepter), "Scepter" ),
				new ItemEntry( typeof(WarHammer), "War Hammer" ),
				new ItemEntry( typeof(WarMace), "War Mace" ),
			};

		private static ItemEntry[] m_KnivesTable = new ItemEntry[]
			{
				new ItemEntry( typeof(ButcherKnife), "Butcher Knife" ),
				new ItemEntry( typeof(Cleaver), "Cleaver" ),
				new ItemEntry( typeof(Dagger), "Dagger" ),
				new ItemEntry( typeof(SkinningKnife), "Skinning Knife" ),
			};

		private static ItemEntry[] m_AxesTable = new ItemEntry[]
			{
				new ItemEntry( typeof(Axe), "Axe" ),
				new ItemEntry( typeof(BattleAxe), "Battle Axe" ),
				new ItemEntry( typeof(DoubleAxe), "Double Axe" ),
				new ItemEntry( typeof(ExecutionersAxe), "Executioners Axe" ),
				new ItemEntry( typeof(Hatchet), "Hatchet" ),
				new ItemEntry( typeof(LargeBattleAxe), "Large Battle Axe" ),
				new ItemEntry( typeof(Pickaxe), "Pickaxe" ),
				new ItemEntry( typeof(TwoHandedAxe), "Two Handed Axe" ),
				new ItemEntry( typeof(WarAxe), "War Axe" )
			};

		private static ItemEntry[] m_ShieldsTable = new ItemEntry[]
			{
				new ItemEntry( typeof(BronzeShield), "Bronze Shield" ),
				new ItemEntry( typeof(Buckler), "Buckler" ),
				new ItemEntry( typeof(ChaosShield), "Chaos Shield" ),
				new ItemEntry( typeof(HeaterShield), "Heater Shield" ),
				new ItemEntry( typeof(MetalKiteShield), "Metal Kite Shield" ),
				new ItemEntry( typeof(MetalShield), "Metal Shield" ),
				new ItemEntry( typeof(OrderShield), "Order Shield" ),
				new ItemEntry( typeof(WoodenKiteShield), "Wooden Kite Shield" ),
				new ItemEntry( typeof(WoodenShield), "Wooden Shield" )
			};

		private static ItemEntry[] m_BowsTable = new ItemEntry[]
			{
				new ItemEntry( typeof(Bow), "Bow" ),
				new ItemEntry( typeof(CompositeBow), "Composite Bow" ),
				new ItemEntry( typeof(Crossbow), "Crossbow" ),
				new ItemEntry( typeof(HeavyCrossbow), "Heavy Crossbow" ),
				new ItemEntry( typeof(RepeatingCrossbow), "Repeating Crossbow" ),
				new ItemEntry( typeof(Arrow), string.Format( "Arrows ({0})", m_iStackableAmount ) ),
				new ItemEntry( typeof(Bolt), string.Format( "Bolts ({0})", m_iStackableAmount ) )
			};

		private static string[] m_MainButtonsTable = new string[]
			{
				"Swords", "Spears And Forks", "Pole Arms", "Maces", "Knives", "Axes", "Shields", "Bows"
			};

		private ItemEntry[] m_MainTable;
		private int m_iPos;
		private WeaponStone m_Stone;

		public WeaponStoneGump( Mobile from, WeaponStone stone, int pos ) : base( 0, 0 )
		{
			from.CloseGump( typeof( WeaponStoneGump ) );

			m_Stone = stone;
			m_iPos = pos;
			switch( pos )
			{
				default:
				case 1: m_MainTable = m_SwordsTable; break;
				case 2: m_MainTable = m_SpearsAndForksTable; break;
				case 3: m_MainTable = m_PoleArmsTable; break;
				case 4: m_MainTable = m_MacesTable; break;
				case 5: m_MainTable = m_KnivesTable; break;
				case 6: m_MainTable = m_AxesTable; break;
				case 7: m_MainTable = m_ShieldsTable; break;
				case 8: m_MainTable = m_BowsTable; break;
			}

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(78, 46, 527, 431, 9200);
			this.AddButton(224, 446, 5200, 5201, 0, GumpButtonType.Reply, 0);
			this.AddBackground(181, 31, 321, 31, 9200);
			this.AddBackground(100, 73, 332, 369, 9350);
			this.AddLabel(295, 35, 0, @"Weapon Stone");
			this.AddImage(27, 26, 10440);
			this.AddLabel(109, 77, 0, @"Choose");
			this.AddLabel(170, 77, 0, @"Name");

			for( int i=1;i<=8;i++ )
			{
				this.AddButton(445, 45+(30*i), 4005, 4006, i, GumpButtonType.Reply, 0);
				this.AddLabel(480, 45+(30*i), 0, m_MainButtonsTable[i-1]);
			}

			int y = 103;
			for(int i=0;i<m_MainTable.Length;i++)
			{
				this.AddButton( 120, y+3, 1209, 1210, i+999, GumpButtonType.Reply, 0);
				this.AddLabel( 170, y, 0, m_MainTable[i].m_sName );
				y += 16;
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			base.OnResponse( state, info );

			if( info.ButtonID >= 1 && info.ButtonID <= 8 )
				state.Mobile.SendGump( new WeaponStoneGump(state.Mobile, m_Stone, info.ButtonID ) );
			else if( info.ButtonID != 0 )
			{
				if( m_Stone.UseLimit )
				{
					bool foundName = false;
					foreach( WeaponStone.WeaponStoneEntry nameEntry in m_Stone.m_alNameList )
					{
						if( nameEntry.m_sName == state.Mobile.Account.ToString() )
						{
							foundName = true;
							if( nameEntry.m_iCount < m_iAmountToGive )
								nameEntry.m_iCount++;
							else
							{
								state.Mobile.SendMessage( "You may not take anymore items from this stone." );
								return;
							}
						}
					}

					if( !foundName )
						m_Stone.m_alNameList.Add( new WeaponStone.WeaponStoneEntry( state.Mobile.Account.ToString(), 1 ) );
				}

				ItemEntry entry = (ItemEntry)m_MainTable[info.ButtonID-999];

				Item item = (Item) Activator.CreateInstance( entry.m_Type );
				if( item.Stackable )
					item.Amount = m_iStackableAmount;

				state.Mobile.AddToBackpack( item );

				state.Mobile.SendMessage( "The {0} has been placed into your packpack.", entry.m_sName );
				state.Mobile.SendGump( new WeaponStoneGump(state.Mobile, m_Stone, m_iPos ) );
			}
		}
	}
}