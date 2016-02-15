using Server;
using Server.Items;
using Server.Commands;
using Server.Engines.RewardSystem;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "a dark knight corpse" )]
	public class LLKnight : DemonKnight
	{
		private DateTime m_SpawnTime;
		private Timer m_Timer;
		private TimeSpan m_Delay = TimeSpan.FromMinutes( 5.0 ); //Broadcast delay
		//no need of serializing the following values, as they will be only obbasionally set.
		private bool m_Broadcast;
		private bool m_CurseArtifact;
		private int m_CursedDurationMinutes;
		private string m_Location;

		private static Type[] m_ArtifactList = new Type[]
			{
				typeof( TheTaskmaster ),		//0
				typeof( LegacyOfTheDreadLord ),
				typeof( TheDragonSlayer ),
				typeof( GauntletsOfNobility ),
				typeof( HolyKnightsBreastplate ),
				typeof( LeggingsOfBane ),		//5
				typeof( MidnightBracers ),
				typeof( OrnateCrownOfTheHarrower ),
				typeof( ShadowDancerLeggings ),
				typeof( TunicOfFire ),
				typeof( VoiceOfTheFallenKing ),	//10
				typeof( BraceletOfHealth ),
				typeof( RingOfTheElements ),
				typeof( RingOfTheVile ),
				typeof( Aegis ),
				typeof( AxeOfTheHeavens ),		//15
				typeof( BladeOfInsanity ),
				typeof( BoneCrusher ),
				typeof( BreathOfTheDead ),
				typeof( Frostbringer ),
				typeof( SerpentsFang ),			//20
				typeof( StaffOfTheMagi ),
				typeof( TheBerserkersMaul ),
				typeof( TheDryadBow ),
				typeof( DivineCountenance ),
				typeof( ArcaneShield ),			//25 - more valuable
				typeof( ArmorOfFortune ),
				typeof( JackalsCollar ),
				typeof( HelmOfInsight ),
				typeof( OrnamentOfTheMagician ),
				typeof( HatOfTheMagi ),			//30
				typeof( HuntersHeaddress ),
				typeof( SpiritOfTheTotem ),
				typeof( BookOfLostKnowledge ),	//33 - custom
				typeof( BowOfWeakening ),
				typeof( GlovesOfTheLepracon ),
				typeof( GrizzlysCourage )
			};

		#region set_props
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Broadcast
		{
			get{ return m_Broadcast; }
			set
			{
				if (m_Broadcast != value && value )
				{
					if (m_Timer == null)
						m_Timer = new LLKnightAnnounceTimer (this, m_Delay );
					m_Timer.Start();
				}

				m_Broadcast = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int CursedDurationMinutes
		{
			get{ return m_CursedDurationMinutes; }
			set{ m_CursedDurationMinutes = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool CurseArtifact
		{
			get{ return m_CurseArtifact; }
			set{ m_CurseArtifact = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string KnightLocation
		{
			get{ return m_Location; }
			set{ m_Location = value; }
		}
		#endregion

		public static Item LLCreateRandomArtifact()
		{
			if ( !Core.AOS )
				return null;

			int random;
			switch (Utility.Random(4))
			{
				case 0: random = m_ArtifactList.Length; break;
				case 1: random = 33; break;
				default: random = 25; break;

			}
			return Loot.Construct( m_ArtifactList[Utility.Random(random)] );
		}

		public static int LLGetArtifactChance( Mobile boss )
		{
			if ( !Core.AOS )
				return 0;

			int luck = LootPack.GetLuckChanceForKiller( boss );
			int chance;

			TimeSpan timeAlive = DateTime.Now - ((LLKnight)boss).m_SpawnTime;
			if (timeAlive > TimeSpan.FromMinutes(60))
				chance = 40000;
			else if (timeAlive > TimeSpan.FromMinutes(50))
				chance = 50000;
			else if (timeAlive > TimeSpan.FromMinutes(40))
				chance = 60000;
			else if (timeAlive > TimeSpan.FromMinutes(30))
				chance = 70000;
			else if (timeAlive > TimeSpan.FromMinutes(20))
				chance = 80000;
			else if (timeAlive > TimeSpan.FromMinutes(10))
				chance = 90000;
			else
				chance = 100000;
			chance += (luck / 5);

			return chance;
		}

		public static bool LLCheckArtifactChance( Mobile boss )
		{
			return LLGetArtifactChance( boss ) > Utility.Random( 100000 );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			Item artifact = null;;
			if ( !Summoned && !NoKillAwards && LLCheckArtifactChance( this ) )
			{
				artifact = LLCreateRandomArtifact();
				DemonKnight.DistributeArtifact( this, artifact );
				if (m_CurseArtifact)
				{
					Timer m_TimerCursed = new CursedArtifactSystem.CursedTimer( artifact, this.m_CursedDurationMinutes );
					m_TimerCursed.Start();
				}
			}
			else c.DropItem( new CopperBar(Utility.Random(1, 5)));

			CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, "The Forgotten One has been killed.");
			if (artifact != null && artifact.Name != null)
				CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The reward was {0}.", artifact.Name));
			if (m_Timer != null)
				m_Timer.Stop();
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 2 );
			for ( int i = 0; i < 8; i++ )
			{
				Item item = Loot.RandomPossibleReagent();
				item.Amount = Utility.RandomMinMax( 20, 50 );
				PackItem( item );
			}
		}

		[Constructable]
		public LLKnight( int BroadcastDelayMinutes ) : base( )
		{
			Name = NameList.RandomName( "male" );
			Title = "the Forgotten One";
			Body = 256;
			ActiveSpeed = 0.15;

			SetHits( 25000 );
			m_SpawnTime = DateTime.Now;
			m_CursedDurationMinutes = 7;

			m_Timer = new LLKnightAnnounceTimer (this, TimeSpan.FromMinutes( BroadcastDelayMinutes ));
			m_Timer.Start();
			m_Broadcast = false;
		}

		[Constructable]
		public LLKnight() : this( 5 )
		{}

		public LLKnight( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			if (Region != null && Region.IsPartOf( "Khaldun" ))
			{
				m_Timer = new LLKnightAnnounceTimer (this, m_Delay );
				m_Timer.Start();
			}
		}

		private class LLKnightAnnounceTimer : Timer
		{

			private LLKnight m_Knight;
			public LLKnightAnnounceTimer( LLKnight knight, TimeSpan delay )
				: base ( TimeSpan.Zero, delay )
			{
				m_Knight = knight;
			}

			protected override void OnTick()
			{
				int chance = LLGetArtifactChance(m_Knight) / 1000;
				if (m_Knight == null || m_Knight.Deleted)
					Stop();
				else if ( m_Knight.Region != null && m_Knight.Region.IsPartOf( "Khaldun" ) )
				{
					if (chance <= 40 && Utility.Random(3) == 0)
					{
						if (m_Knight.Hits == m_Knight.HitsMax)
							if (Utility.Random(2) == 0)
							{
								CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, "The Forgotten One has vanished.");
								m_Knight.Delete();
								Stop();
								return;
							}
							else
								CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The Forgotten one is still in Khaldun! If you find and kill him, you have {0}% chance to receive an artifact! Hurry up, it will vanish soon!", chance ));
						else CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The Forgotten one is still in Khaldun! If you find and kill him, you have {0}% chance to receive an artifact!", chance ));
					}
					else if (m_Knight.Hits < m_Knight.HitsMax / 4)
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, "The Forgotten one roams in Khaldun, and it has less than 1/4 of its health!");
					else if (m_Knight.Hits < m_Knight.HitsMax / 2)
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, "The Forgotten one roams in Khaldun, and it has less than half of its health!");
					else if (m_Knight.Hits < m_Knight.HitsMax - m_Knight.HitsMax / 4)
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, "The Forgotten one roams in Khaldun, and it has less than 3/4 of its health!");
					else CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, "The Forgotten one roams in Khaldun!");

					CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("If you kill him, you have {0}% chance to receive an artifact!", chance ));
					if (chance >= 90)
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, "The slower you kill him, the lower the chance gets.");
				}
				else if (m_Knight.Broadcast)
				{
					string loc = ( String.IsNullOrEmpty(m_Knight.KnightLocation) ? "the world" : m_Knight.KnightLocation );
					if (m_Knight.Hits < m_Knight.HitsMax / 4)
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The Forgotten one roams in {0}, and it has less than 1/4 of its health!", loc));
					else if (m_Knight.Hits < m_Knight.HitsMax / 2)
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The Forgotten one roams in {0}, and it has less than half of its health!", loc));
					else if (m_Knight.Hits < m_Knight.HitsMax - m_Knight.HitsMax / 4)
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The Forgotten one roams in {0}, and it has less than 3/4 of its health!", loc));
					else CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The Forgotten one roams in {0}!", loc));

					CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("If you kill him, you have {0}% chance to receive an artifact!", chance ));
					if (chance >= 90)
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, "The slower you kill him, the lower the chance gets.");
				}
				else Stop();
			}
		}

	}
}