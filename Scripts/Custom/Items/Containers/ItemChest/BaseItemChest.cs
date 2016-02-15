//*****************************************************************
// Name: BaseItemChest
//
// Desc: Written by Eclipse
//
// Comments:
//*****************************************************************
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Items
{
	public abstract class BaseItemChest : LockableContainer, IWorldTimer
	{
		private bool m_ItemsCreated;
		private bool m_CanBeTrapped = true;
		private TimeSpan m_MaxDelay = TimeSpan.FromMinutes(40);
		private TimeSpan m_MinDelay = TimeSpan.FromMinutes(20);
		private int m_iTreasureLevel;
		private static int m_iMaxLevel = 7;

		public override bool IsDecoContainer
		{
			get { return false; }
		}

		#region CommandProperties
		[CommandProperty(AccessLevel.GameMaster)]
		public int Level
		{
			get { return m_iTreasureLevel; }
			set { m_iTreasureLevel = CheckLevel(value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CanBeTrapped
		{
			get { return m_CanBeTrapped; }
			set { m_CanBeTrapped = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan MaxDelay
		{
			get { return m_MaxDelay; }
			set { m_MaxDelay = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan MinDelay
		{
			get { return m_MinDelay; }
			set { m_MinDelay = value; }
		}
		#endregion

		public BaseItemChest(int level, int itemid)
			: base(itemid)
		{
			Movable = false;
			m_iTreasureLevel = CheckLevel(level);

			Key key = (Key)FindItemByType(typeof(Key));
			if (key != null)
				key.Delete();

			SetLockAndTrap();
		}

		public BaseItemChest(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)3);

			// Version 2
			writer.Write((bool)m_ItemsCreated);

			// Version 1
			writer.Write((int)m_iTreasureLevel);

			// Version 0
			writer.Write((TimeSpan)m_MinDelay);
			writer.Write((TimeSpan)m_MaxDelay);
			writer.Write((bool)m_CanBeTrapped);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				case 3:
					goto case 2;
				case 2:
					m_ItemsCreated = reader.ReadBool();
					goto case 1;
				case 1:
					m_iTreasureLevel = reader.ReadInt();
					goto case 0;
				case 0:
					m_MinDelay = reader.ReadTimeSpan();
					m_MaxDelay = reader.ReadTimeSpan();
					m_CanBeTrapped = reader.ReadBool();
					break;
			}

			// Fixes old chests that could have over 100 in locklevel
			if (version < 3 && (RequiredSkill > 100 || LockLevel > 100))
				SetLockAndTrap();

			if (Locked && LockLevel == -255) // check for magic lock (if a player locked the chest)
				AddTimerRequest();
			else if (!Locked)
				AddTimerRequest();
		}

		public TimeSpan GetDelay()
		{
			return TimeSpan.FromSeconds(Utility.RandomMinMax((int)m_MinDelay.TotalSeconds, (int)m_MaxDelay.TotalSeconds));
		}

		private void AddTimerRequest()
		{
			EclWorldTimer.AddTime(this, GetDelay());
		}

		private static int CheckLevel(int level)
		{
			if (level > m_iMaxLevel)
				return m_iMaxLevel;
			else if (level < 0)
				return 1;
			else return level;
		}

		public virtual void SetLockAndTrap()
		{
			if (!Locked || LockLevel == -255) // check for magic lock (if a player locked the chest)
				SpawnGuards();

			Locked = true;
			RequiredSkill = LockLevel = Utility.RandomMinMax(13 * m_iTreasureLevel, 14 * m_iTreasureLevel);
			TrapPower = Utility.RandomMinMax(10 * m_iTreasureLevel, 14 * m_iTreasureLevel);

			if (Utility.RandomBool() && m_CanBeTrapped && m_iTreasureLevel > Utility.Random(5))
				TrapType = (TrapType)Utility.Random(5);

			m_ItemsCreated = false;
		}

		public static bool CheckLootChance(int level, double chance)
		{
			return ((double)level * chance) > Utility.RandomDouble();
		}

		public virtual void GenerateTreasure(Mobile from)
		{
			if (CheckLootChance(m_iTreasureLevel, 0.15))
				BaseItemChest.AddLoot(from, this, m_iTreasureLevel);
		}

		public static void AddLoot(Mobile from, Container c, int iLevel)
		{
			double luckChance = (int)(Math.Pow( Math.Min(1200, from.Luck), 1 / 1.8 ) / 1000); // up to 0.05
			// Add Gold
			if (BaseItemChest.CheckLootChance(iLevel, 0.15 + luckChance))
				c.DropItem(new Gold(iLevel * 40, iLevel * 80));

			// Add Regs
			if (BaseItemChest.CheckLootChance(iLevel, 0.10 + luckChance))
				for (int i = 0; i < iLevel; i++)
					Loot.AddRegs(c, Utility.RandomMinMax(1, 5), Utility.RandomBool());

			// Add Scrolls
			if (BaseItemChest.CheckLootChance(iLevel, 0.10+ luckChance))
				for (int i = 0; i < iLevel; i++)
					Loot.AddScrolls(c, 1, Utility.RandomBool());

			// Add Items (No need for a chance here)
			int minIntensity = (iLevel == 7) ? 50 + (int)(luckChance*200) : iLevel * (int)(luckChance*200);
			int minProps = Math.Min( 4, (iLevel + (int)(luckChance*100)) / 3 );
			if (minProps < 1)
				minProps = 1;
			int precentScale = (int)(luckChance*10) + ((iLevel == 7) ? 30 : iLevel * 3);
			LootPackEntry.AddRandomLoot(c, iLevel, precentScale * 12,  minProps, 5, minIntensity, 100);

			// Add Potions
			if (BaseItemChest.CheckLootChance(iLevel, 0.10))
			{
				if (iLevel == 7)
				{
					int iSEPotionCount = Utility.RandomMinMax(1, 5);
					for (int i = 0; i < iSEPotionCount; ++i)
						c.DropItem(Loot.Construct(m_SEPotionTypes));
				}
				Loot.AddPotions(c, Utility.Random(iLevel) + 1);
			}

			//Add CCKey
			if (iLevel == 7 && 0.05 > Utility.RandomDouble() )
				c.DropItem(new CCKey());

			// Bone Remains
			if (BaseItemChest.CheckLootChance(iLevel, 0.10))
				BoneRemains.PackBoneRemains( c, Utility.Random( 1, 3 ) );
		}

		private static Type[] m_SEPotionTypes = new Type[]
			{
				typeof( ConflagrationPotion ),	typeof( GreaterConflagrationPotion ),
				typeof( ConfusionBlastPotion ),		typeof( GreaterConfusionBlastPotion )
			};

		public override void OnItemLifted(Mobile from, Item item)
		{
			from.RevealingAction();
			base.OnItemLifted(from, item);
		}

		public override void LockPick(Mobile from)
		{
			base.LockPick(from);

			if (!m_ItemsCreated)
			{
				GenerateTreasure(from);
				m_ItemsCreated = true;
				AddTimerRequest();
			}
		}

		public void ClearContents()
		{
			for (int i = Items.Count - 1; i >= 0; --i)
				if (i < Items.Count)
					((Item)Items[i]).Delete();
		}

		public void OnTick(WorldTimerEntry entry)
		{
			ClearContents();
			SetLockAndTrap();
		}

		public void SpawnGuards()
		{
			if (Utility.Random(10) == 0)
			{
				BaseCreature bc = null;
				int level = 0;
				if (Region.Find( Location, Map ) != null)
				{
					if (Region.Find( Location, Map ).IsPartOf( "Khaldun" ) || Region.Find( Location, Map ).IsPartOf( "Deceit" ))
						level = 1;
					else if (Region.Find( Location, Map ).IsPartOf( "Despise"))
						level = 2;
					else if (Region.Find( Location, Map ).IsPartOf( "Shame"))
						level = 3;
					else if (Region.Find( Location, Map ).IsPartOf( "Orc Cave"))
						level = 4;
					else if (Region.Find( Location, Map ).IsPartOf( "Hythloth"))
						level = 5;
					else if (Region.Find( Location, Map ).IsPartOf( "Covetous"))
						level = 6;
					else if (Region.Find( Location, Map ).IsPartOf( "Destard"))
						level = 7;
					else if (Region.Find( Location, Map ).IsPartOf( "Fire"))
						level = 8;
					else if (Region.Find( Location, Map ).IsPartOf( "Ice"))
						level = 9;
					else if (Region.Find( Location, Map ).IsPartOf( "Cursed Cave") || Region.Find( Location, Map ).IsPartOf( "Cursed Cave Level 3") || m_iTreasureLevel == 7)
						level = 10;
					else if (m_iTreasureLevel == 6)
						level = 11;
					else if (m_iTreasureLevel == 5)
						level = 12;
					else if (m_iTreasureLevel == 4)
						level = 13;
				}
				if (level > m_SpawnTypes.Length)
					level = 0;
				try
				{
					bc = (BaseCreature)Activator.CreateInstance( m_SpawnTypes[level][Utility.Random( m_SpawnTypes[level].Length )] );
				}
				catch {}

				if (bc != null)
				{
					bc.Home = Location;
					bc.RangeHome = 5;

					bool spawned = false;

					for ( int i = 0; !spawned && i < 10; ++i )
					{
						int x = X - 3 + Utility.Random( 7 );
						int y = Y - 3 + Utility.Random( 7 );

						if ( Map.CanSpawnMobile( x, y, Z ) )
						{
							bc.MoveToWorld( new Point3D( x, y, Z ), Map );
							spawned = true;
						}
						else
						{
							int z = Map.GetAverageZ( x, y );

							if ( Map.CanSpawnMobile( x, y, z ) )
							{
								bc.MoveToWorld( new Point3D( x, y, z ), Map );
								spawned = true;
							}
						}
					}

					if ( !spawned )
						bc.Delete();
				}
			}
		}

		private static Type[][] m_SpawnTypes = new Type[][]
		{
			new Type[]{ typeof( Mongbat ), typeof( Ratman ), typeof( HeadlessOne ), typeof( Skeleton ), typeof( Zombie ), typeof( Mummy ), typeof( Shade ) }, //0 - default
			new Type[]{ typeof( Lich ), typeof( SpectralArmour ), typeof( BoneKnight ), typeof( BoneMagi ), typeof( SkeletalMage ), typeof( SkeletalKnight ) }, //Khaldun, Deceit
			new Type[]{ typeof( Ettin ), typeof( Lizardman ), typeof( Troll ), typeof( EarthElemental ) }, //Despise
			new Type[]{ typeof( Scorpion ), typeof( EarthElemental ), typeof( AirElemental ), typeof( DullCopperElemental ) }, //Shame
			new Type[]{ typeof( OrcishMage ), typeof( OrcishLord ), typeof( Orc ), typeof( OrcBomber ), typeof( DireWolf ) }, //Orc Cave
			new Type[]{ typeof( Gargoyle ), typeof( FireGargoyle ), typeof( StoneGargoyle ), typeof( Gazer ), typeof( ElderGazer ), typeof( HellHound ), typeof( Daemon ), typeof( Imp ) }, //Hythloth
			new Type[]{ typeof( Lich ), typeof( Mummy ), typeof( Shade ), typeof( SkeletalKnight ), typeof( DreadSpider ), typeof( AirElemental ), typeof( SwampTentacle ), typeof( Gazer ), typeof( ElderGazer ) }, //Covetous
			new Type[]{ typeof( Dragon ), typeof( Drake ), typeof( Wyvern ), typeof( Snake ), typeof( GiantSerpent ), typeof( Lizardman ) }, //Destard
			new Type[]{ typeof( FireElemental ), typeof( LavaSerpent ), typeof( LavaSnake ), typeof( HellCat ), typeof( HellHound ), typeof( EvilMage ) }, //Fire
			new Type[]{ typeof( IceElemental ), typeof( SnowElemental ), typeof( FrostSpider ), typeof( IceSnake ), typeof( FrostOoze ), typeof( FrostTroll ) }, //Ice
			new Type[]{ typeof( TheCursed ), typeof( TheCursedArcher ), typeof( TheCursedWarrior ), typeof( TheCursedGuardian ) }, // level 7 - The Cursed Cave
			new Type[]{ typeof( AncientWyrm ), typeof( Balron ), typeof( BloodElemental ), typeof( PoisonElemental ), typeof( Titan ) }, //level 6
			new Type[]{ typeof( LichLord ), typeof( Daemon ), typeof( ElderGazer ), typeof( PoisonElemental ), typeof( BloodElemental ) }, //level 5
			new Type[]{ typeof( DreadSpider ), typeof( LichLord ), typeof( Daemon ), typeof( ElderGazer ), typeof( OgreLord ) } //level 4
		};
	}
}