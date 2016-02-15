using System;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.ContextMenus;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;

namespace Server.Items
{
	public class BowOfWeakening : Bow, ILevelable
	{
		public string[] m_sKillTypeNames = new string[]
		{
			"Abyss",
			"Arachnid",
			"Cold Blood",
			"Forest Lord",
			"Vermin Horde",
			"Unholy Terror"
		};

		private static Type[][] m_KillTypes = new Type[6][]
			{
				new Type[]
				{	// Abyss
					typeof( Mongbat ), typeof( Imp ),
					typeof( Gargoyle ), typeof( Harpy ),
					typeof( FireGargoyle ), typeof( StoneGargoyle ),
					typeof( Daemon ), typeof( Succubus )
				},

				new Type[]
				{	// Arachnid
					typeof( Scorpion ), typeof( GiantSpider ),
					typeof( TerathanDrone ), typeof( TerathanWarrior ),
					typeof( DreadSpider ), typeof( TerathanMatriarch ),
					typeof( PoisonElemental ), typeof( TerathanAvenger )
				},
				new Type[]
				{	// Cold Blood
					typeof( Lizardman ), typeof( Snake ),
					typeof( LavaLizard ), typeof( OphidianWarrior ),
					typeof( Drake ), typeof( OphidianArchmage ),
					typeof( Dragon ), typeof( OphidianKnight )
				},
				new Type[]
				{	// Forest Lord
					typeof( Pixie ), typeof( ShadowWisp ),
					typeof( Kirin ), typeof( Wisp ),
					typeof( Centaur ), typeof( Unicorn ),
					typeof( EtherealWarrior ), typeof( SerpentineDragon )
				},
				new Type[]
				{	// Vermin Horde
					typeof( GiantRat ), typeof( Slime ),
					typeof( DireWolf ), typeof( Ratman ),
					typeof( HellHound ), typeof( RatmanMage ),
					typeof( RatmanArcher ), typeof( SilverSerpent )
				},
				new Type[]
				{	// Unholy Terror
					typeof( Bogle ), typeof( Ghoul ), typeof( Shade ), typeof( Spectre ), typeof( Wraith ),
					typeof( BoneMagi ), typeof( Mummy ), typeof( SkeletalMage ),
					typeof( BoneKnight ), typeof( Lich ), typeof( SkeletalKnight ),
					typeof( LichLord ), typeof( RottingCorpse )
				},
		};

		private int m_iType;
		private int m_Experience;
		private int m_Level;

		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BowOfWeakening() : base()
		{
			Name = "bow of weakening";
			Hue = 0x59E;

			m_iType = Utility.Random( 6 );

			Attributes.WeaponDamage = 40;
			Attributes.WeaponSpeed = 30;

			SkillBonuses.SetValues( 0, SkillName.Archery, 10.0 );

			Slayer = SlayerName.ReptilianDeath;
			WeaponAttributes.HitLowerAttack = 50;

			LevelItemManager.InvalidateLevel( this );
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = pois = cold = nrgy = chaos = direct = 0;
			fire = 100;
		}

		public BowOfWeakening( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

			writer.Write( m_iType );
			writer.Write( m_Experience );
			writer.Write( m_Level );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_iType = reader.ReadInt();
			m_Experience = reader.ReadInt();
			m_Level = reader.ReadInt();
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties (list);

			list.Add( 1060658, "Level\t{0}", m_Level );
			list.Add( 1060659, "Experience\t{0}", m_Experience );
			list.Add( 1060660, "Type\t{0}", m_sKillTypeNames[m_iType] );
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries (from, list);

			if ( from.Alive && ( from.Items.Contains( this ) || this.IsChildOf( from.Backpack ) ) )
			{
				list.Add( new LevelInfoEntry( this ) );
			}
		}

		#region ILevelable Members

		[CommandProperty( AccessLevel.GameMaster )]
		public int Experience
		{
			get
			{
				return m_Experience;
			}
			set
			{
				m_Experience = value;

				if ( m_Experience > LevelItemManager.ExpTable[LevelItemManager.Levels - 1] )
					m_Experience = LevelItemManager.ExpTable[LevelItemManager.Levels - 1];

				LevelItemManager.InvalidateLevel( this );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Level
		{
			get
			{
				return m_Level;
			}
			set
			{
				if ( value > LevelItemManager.Levels )
					value = LevelItemManager.Levels;

				if ( value < 1 )
					value = 1;

				if ( m_Level != value )
				{
					int oldLevel = m_Level;
					m_Level = value;
					OnLevel( oldLevel, m_Level );
				}
			}
		}

		public Type[] Types
		{
			get
			{
				return m_KillTypes[m_iType];
			}
		}

		public void OnLevel( int oldLevel, int newLevel )
		{
			Attributes.WeaponDamage = LevelItemManager.CalculateProperty( 40, m_Level );
			Attributes.WeaponSpeed = LevelItemManager.CalculateProperty( 30, m_Level );
			WeaponAttributes.HitLowerAttack = LevelItemManager.CalculateProperty( 50, m_Level );
			SkillBonuses.SetValues( 0, SkillName.Archery, LevelItemManager.CalculateProperty( 10, m_Level ) );
		}

		public string GetHtml()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append( "<div align=center><i>ENHANCEMENTS</i></div>" );

			builder.Append( "WeaponDamage: 40%<br>" );
			builder.Append( "WeaponSpeed: 30%<br>" );
			builder.Append( "HitLowerAttack: 50%<br>" );
			builder.Append( "Archery: 10<br>" );

			builder.Append( "<div align=center><i>LEVEL GAIN LIST</i></div>" );

			for(int i=1;i<LevelItemManager.ExpTable.Length;i++)
			{
				int iLevel = i+1;
				builder.Append( string.Format( "Level {0} at {1} EXP<br>", iLevel.ToString(), LevelItemManager.ExpTable[i].ToString() ) );
			}
			return builder.ToString();
		}

		#endregion
	}
}