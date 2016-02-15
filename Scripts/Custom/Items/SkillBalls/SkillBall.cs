//	Official Defiance(c) skillball - by [Dev]Kamron - casiopiauo@gmail.com

using System;
using System.Text;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using System.Collections.Generic;

namespace Server.Items
{
	[Flags]
	public enum SkillBallFlags
	{
		None			= 0x00,
		Expires			= 0x02,
		Limited			= 0x04,
		MinCap			= 0x08, //Other than 0
		MaxCap			= 0x10,	//Other than 100
		PlayerBound		= 0x20,
		AccountBound	= 0x40
	}

	public class SkillBall : Item
	{
		private int m_SkillBonus;
		private SkillBallFlags m_Flags;
		private DateTime m_ExpireDate;
		private Mobile m_OwnerPlayer;
		private string m_OwnerAccount;

		private int m_MinCap;
		private int m_MaxCap;

		public SkillBallFlags Flags{ get{ return m_Flags; } set{ m_Flags = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime ExpireDate{ get{ return m_ExpireDate; } set{ m_ExpireDate = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int SkillBonus{ get{ return m_SkillBonus; } set{ m_SkillBonus = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int MinCap{ get{ return m_MinCap; } set{ m_MinCap = value; AltMinCap = m_MinCap != 120; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxCap{ get{ return m_MaxCap; } set{ m_MaxCap = value; AltMaxCap = m_MaxCap != 100; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile OwnerPlayer{ get{ return m_OwnerPlayer; } set{ SetFlag( SkillBallFlags.PlayerBound, (m_OwnerPlayer = value) != null ); InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public string OwnerAccount{ get{ return m_OwnerAccount; } set{ SetFlag( SkillBallFlags.AccountBound, !String.IsNullOrEmpty(m_OwnerAccount = value) ); InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Expires{ get{ return GetFlag( SkillBallFlags.Expires ); } set{ SetFlag( SkillBallFlags.Expires, value ); InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Limited{ get{ return GetFlag( SkillBallFlags.Limited ); } set{ SetFlag( SkillBallFlags.Limited, value ); InvalidateProperties(); } }

		public bool AltMinCap{ get{ return GetFlag( SkillBallFlags.MinCap ); } set{ SetFlag( SkillBallFlags.MinCap, value );  } }
		public bool AltMaxCap{ get{ return GetFlag( SkillBallFlags.MaxCap ); } set{ SetFlag( SkillBallFlags.MaxCap, value );  } }

		public int MaxCapFixedPoint{ get{ return MaxCap * 10; } }
		public int MinCapFixedPoint{ get{ return MinCap * 10; } }
		public int SkillBonusFixedPoint{ get{ return SkillBonus * 10; } }

		public bool PlayerBound{ get{ return GetFlag( SkillBallFlags.PlayerBound ); } set{ SetFlag( SkillBallFlags.PlayerBound, value );  } }
		public bool AccountBound{ get{ return GetFlag( SkillBallFlags.AccountBound ); } set{ SetFlag( SkillBallFlags.AccountBound, value );  } }

		public bool GetFlag( SkillBallFlags flag )
		{
			return ( m_Flags & flag ) != 0;
		}

		public void SetFlag( SkillBallFlags flag, bool value )
		{
			if ( value )
				m_Flags |= flag;
			else
				m_Flags &= ~flag;
		}

		[Constructable]
		public SkillBall() : this( 25 )
		{
		}

		[Constructable]
		public SkillBall( int bonus ) : this( bonus, true )
		{
		}

		[Constructable]
		public SkillBall( int bonus, bool limited ) : this( bonus, 100, limited )
		{
		}

		[Constructable]
		public SkillBall( int bonus, int maxcap ) : this( bonus, maxcap, true )
		{
		}

		[Constructable]
		public SkillBall( int bonus, int maxcap, bool limited ) : this( bonus, maxcap, limited, 0 )
		{
		}

		[Constructable]
		public SkillBall( int bonus, int maxcap, bool limited, int days ) : this( bonus, maxcap, limited, days, 120 )
		{
		}

		[Constructable]
		public SkillBall( int bonus, int maxcap, bool limited, int days, int mincap ) : base( 7885 )
		{
			m_SkillBonus = bonus;

			if ( days > 0 )
			{
				ExpireDate = DateTime.Now.AddDays( days );
				Expires = true;
			}
			UpdateLabelName();

			m_OwnerAccount = String.Empty;
			Limited = limited;

			MinCap = mincap;
			MaxCap = maxcap;

			Name = DefaultName;
		}

		public SkillBall( Serial serial ) : base( serial )
		{
		}

		public override string DefaultName{ get{ return "skill ball"; } }
		public override int LabelNumber{ get{ return 0; } }
		public override bool DisplayLootType{ get{ return false; } }
		public virtual bool Rechargable{ get{ return false; } }

		public virtual SkillName[] GetAllowedSkills( Mobile target )
		{
			if ( Limited )
				return m_LimitedAOSSkills;
			else
				return m_AOSSkills;
		}

		private static readonly SkillName[] m_LimitedAOSSkills = new SkillName[]
			{
				SkillName.Anatomy,
				SkillName.AnimalLore,
				SkillName.ItemID,
				SkillName.ArmsLore,
				SkillName.Parry,
				SkillName.Begging,
				SkillName.Peacemaking,
				SkillName.Camping,
				SkillName.DetectHidden,
				SkillName.Discordance,
				SkillName.EvalInt,
				SkillName.Healing,
				SkillName.Forensics,
				SkillName.Herding,
				SkillName.Hiding,
				SkillName.Magery,
				SkillName.MagicResist,
				SkillName.Tactics,
				SkillName.Snooping,
				SkillName.Musicianship,
				SkillName.Archery,
				SkillName.SpiritSpeak,
				SkillName.Stealing,
				SkillName.TasteID,
				SkillName.Tracking,
				SkillName.Veterinary,
				SkillName.Swords,
				SkillName.Macing,
				SkillName.Fencing,
				SkillName.Wrestling,
				SkillName.Meditation,
				SkillName.Stealth,
				SkillName.RemoveTrap,
				SkillName.Necromancy,
				SkillName.Focus,
				SkillName.Chivalry
			};

		public static readonly SkillName[] m_AOSSkills = new SkillName[]
			{
				SkillName.Alchemy,		//crafting
				SkillName.Anatomy,
				SkillName.AnimalLore,
				SkillName.ItemID,
				SkillName.ArmsLore,
				SkillName.Parry,
				SkillName.Begging,
				SkillName.Blacksmith,	//crafting
				SkillName.Fletching,	//crafting
				SkillName.Peacemaking,
				SkillName.Camping,
				SkillName.Carpentry,	//crafting
				SkillName.Cartography,	//donation
				SkillName.Cooking,		//donation
				SkillName.DetectHidden,
				SkillName.Discordance,
				SkillName.EvalInt,
				SkillName.Healing,
				SkillName.Fishing,		//donation
				SkillName.Forensics,
				SkillName.Herding,
				SkillName.Hiding,
				SkillName.Provocation,	//donation
				SkillName.Inscribe,		//donation
				SkillName.Lockpicking,	//donation
				SkillName.Magery,
				SkillName.MagicResist,
				SkillName.Tactics,
				SkillName.Snooping,
				SkillName.Musicianship,
				SkillName.Poisoning,	//donation
				SkillName.Archery,
				SkillName.SpiritSpeak,
				SkillName.Stealing,
				SkillName.Tailoring,	//crafting
				SkillName.AnimalTaming,	//donation
				SkillName.TasteID,
				SkillName.Tinkering,	//crafting
				SkillName.Tracking,
				SkillName.Veterinary,
				SkillName.Swords,
				SkillName.Macing,
				SkillName.Fencing,
				SkillName.Wrestling,
				SkillName.Lumberjacking,	//donation
				SkillName.Mining,			//donation
				SkillName.Meditation,
				SkillName.Stealth,
				SkillName.RemoveTrap,
				SkillName.Necromancy,
				SkillName.Focus,
				SkillName.Chivalry
			};

		public new void InvalidateProperties()
		{
			UpdateHue();
			UpdateLabelName();
			base.InvalidateProperties();
		}

		public virtual void UpdateHue()
		{
			Hue = 3;
		}

		private string m_LabelName;

		public void UpdateLabelName()
		{
			StringBuilder name = new StringBuilder();

			if ( m_SkillBonus > 0 )
				name.Append( "a +{0} " );
			else
				name.Append( "an uncharged " );

			if ( Expires )
				name.Append( "unstable " );

			if ( Limited )
				name.Append( "limited " );

			name.Append( Name );

			m_LabelName = name.ToString();
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			if ( m_SkillBonus > 0 )
				list.Add( 1060658, "bonus\t{0} {1}", m_SkillBonus, Limited ? "[Limited]" : String.Empty );
			else
				list.Add( 1060658, "bonus\t{0}", "uncharged" );

			if ( AltMinCap )
				list.Add( 1060659, "max skill\t{0}", m_MinCap );
			if ( AltMaxCap )
				list.Add( 1060660, "max bonus\t{0}", m_MaxCap );

			if ( Expires )
			{
				if ( DateTime.Now < m_ExpireDate )
					list.Add( 1074884, "{0} days", (m_ExpireDate - DateTime.Now).Days );
				else if ( m_SkillBonus > 0 )
					list.Add( 1074884, "none" );
			}

			if ( m_OwnerPlayer != null )
				list.Add( 1041522, "\t{0}\t\t", "player bound" );
			else if ( !String.IsNullOrEmpty( m_OwnerAccount ) )
				list.Add( 1041522, "\t{0}\t\t", "account bound" );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Deleted || !from.CanSee( this ) )
				return;

			LabelTo( from, String.Format( m_LabelName, m_SkillBonus ) );

			if ( Expires )
			{
				if ( DateTime.Now < m_ExpireDate )
					LabelTo( from, String.Format( "charge time left: {0} days", (m_ExpireDate - DateTime.Now).Days ) );
				else
					LabelTo( from, "charge time left: none" );
			}

			if ( m_OwnerPlayer != null )
				LabelTo( from, "player bound" );
			else if ( m_OwnerAccount.Length > 0 )
				LabelTo( from, "account bound" );
		}

		public virtual void SendGump( Mobile from )
		{
			from.SendGump( new SkillBallGump( from, from, this ) );
		}

		public virtual bool ValidateUser( Mobile from )
		{
			return ( m_OwnerPlayer != null && from != m_OwnerPlayer ) || ( !String.IsNullOrEmpty(m_OwnerAccount) && from.Account.Username != m_OwnerAccount );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_SkillBonus <= 0 || ( Expires && DateTime.Now >= m_ExpireDate ) )
			{
				if ( from.AccessLevel >= AccessLevel.GameMaster )
					from.SendGump( new PropertiesGump( from, this ) );
				SendLocalizedMessageTo( from, 1042544 ); // This item is out of charges.
			}
			else if ( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else if ( ValidateUser( from ) )
				from.SendMessage( "Only the owner can use this skill ball." );
			else if ( !from.HasGump( typeof(SkillBall) ) )
				SendGump( from );
			else
				from.SendMessage( "You are already using a skill ball." );
		}

		public virtual List<Skill> GetDecreasableSkills( Mobile from, Mobile target, int count, int cap, out int decreaseamount )
		{
			Skills skills = from.Skills;
			decreaseamount = 0;

			List<Skill> decreased = new List<Skill>();
			int bonus = SkillBonusFixedPoint;

			if ( (count + bonus) > cap )
			{
				for ( int i = 0; i < skills.Length; i++ )
				{
					int skillamt = skills[i].BaseFixedPoint;
					if ( skills[i].Lock == SkillLock.Down && skillamt > 0 )
					{
						decreased.Add( skills[i] );
						decreaseamount += skillamt;
					}
				}
			}

			return decreased;
		}

		public virtual void DecreaseSkills( Mobile from, Mobile target, List<Skill> decreased, int count, int cap )
		{
			Skills skills = from.Skills;

			int freepool = cap - count;
			int bonus = SkillBonusFixedPoint;

			if ( freepool < bonus )
			{
				bonus -= freepool;

				foreach( Skill s in decreased )
				{
					if ( s.BaseFixedPoint >= bonus )
					{
						s.BaseFixedPoint -= bonus;
						bonus = 0;
					}
					else
					{
						bonus -= s.BaseFixedPoint;
						s.BaseFixedPoint = 0;
					}

					if ( bonus == 0 )
						break;
				}
			}
		}

		public virtual void IncreaseSkill( Mobile from, Mobile target, Skill skill, int toIncrease ) //Silver: added fourth argument
		{
			skill.BaseFixedPoint += toIncrease; //Old: SkillBonusFixedPoint;
			m_SkillBonus = 0;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 4 ); // version

			writer.WriteEncodedInt( (int)m_Flags );

			if ( AltMinCap )
				writer.Write( (int) m_MinCap );

			if ( AltMaxCap )
				writer.Write( (int) m_MaxCap );

			if ( PlayerBound )
				writer.Write( m_OwnerPlayer );

			if ( AccountBound )
				writer.Write( m_OwnerAccount );

			writer.Write( m_ExpireDate );
			writer.Write( m_SkillBonus );

			//We save it just in case, then delete it afterwards
			if ( Expires && DateTime.Now > m_ExpireDate )
				AddToCleanup( this );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
/*
			if ( version == 0 ) //Donation Item
			{
				if ( reader.ReadBool() ) //Yes donation?
					ItemConversion.AddToDonationConversion( this );
				version = reader.ReadInt(); //SkillBall
			}
*/
			if ( version > 3 )
			{
				switch ( version )
				{
					case 4:
					{
						m_Flags = (SkillBallFlags)reader.ReadEncodedInt();

						if ( AltMinCap )
							m_MinCap = reader.ReadInt();
						else m_MinCap = 120;

						if ( AltMaxCap )
							m_MaxCap = reader.ReadInt();
						else m_MaxCap = 100;

						if ( PlayerBound )
							m_OwnerPlayer = reader.ReadMobile();

						if ( AccountBound )
							m_OwnerAccount = reader.ReadString();

						m_ExpireDate = reader.ReadDateTime();
						m_SkillBonus = reader.ReadInt();
						break;
					}
				}

				//We save it just in case, then delete it afterwards
				if ( Expires && DateTime.Now > m_ExpireDate )
					AddToCleanup( this );
			}
			else //Conversion Code
			{
				m_OwnerAccount = reader.ReadString();
				m_OwnerPlayer = reader.ReadMobile();

				SetFlag( SkillBallFlags.Limited,	!reader.ReadBool() );

				m_SkillBonus = reader.ReadInt();
				MaxCap = reader.ReadInt();
			}
		}

		public static void Configure()
		{
			m_Cleanup = new List<Item>();

			EventSink.WorldLoad += new WorldLoadEventHandler( PurgeList );
			EventSink.WorldSave += new WorldSaveEventHandler( Save );
		}

		private static List<Item> m_Cleanup;

		public static void PurgeList()
		{
			if ( m_Cleanup != null && m_Cleanup.Count > 0 )
				for ( int i = 0; i < m_Cleanup.Count; ++i )
					m_Cleanup[i].Delete();

			m_Cleanup.Clear();
		}

		public static void Save( WorldSaveEventArgs e )
		{
			PurgeList();
		}

		public static void AddToCleanup( Item item )
		{
			if ( m_Cleanup == null )
				m_Cleanup = new List<Item>();

			m_Cleanup.Add( item );
		}
	}
}

namespace Server.Gumps
{
	public class SkillBallGump : Gump
	{
		private const int FieldsPerPage = 14;

		private Skill m_Skill;
		private SkillBall m_SkillBall;
		private Mobile m_Target;

		public SkillBallGump ( Mobile from, Mobile target, SkillBall ball ) : base ( 20, 30 )
		{
			m_SkillBall = ball;
			m_Target = target;

			AddPage ( 0 );
			AddBackground( 0, 0, 260, 351, 5054 );

			AddImageTiled( 10, 10, 240, 23, 0x52 );
			AddImageTiled( 11, 11, 238, 21, 0xBBC );

			AddLabel( 45, 11, 0, "Select a skill to raise" );

			AddPage( 1 );

			int page = 1;
			int index = 0;

			Skills skills = m_Target.Skills;
			SkillName[] allowedskills = m_SkillBall.GetAllowedSkills( m_Target );

			for ( int i = 0; i < allowedskills.Length; ++i )
			{
				if ( index >= FieldsPerPage )
				{
					AddButton( 231, 13, 0x15E1, 0x15E5, 0, GumpButtonType.Page, page + 1 );

					++page;
					index = 0;

					AddPage( page );

					AddButton( 213, 13, 0x15E3, 0x15E7, 0, GumpButtonType.Page, page - 1 );
				}

				Skill skill = skills[allowedskills[i]];
				int skillcur = skill.BaseFixedPoint;
				int skillamt = skillcur + m_SkillBall.SkillBonusFixedPoint;
				int skillmax = m_SkillBall.MaxCapFixedPoint;
				int skillmin = m_SkillBall.MinCapFixedPoint;
// SILVER: replaced skillamt requirement by skillcur. added red texthue for skills with high skillamt
				if ( skillcur <= skillmin && skillcur < skillmax && skillcur < skill.CapFixedPoint && skill.Lock == SkillLock.Up &&
						( allowedskills[i] != SkillName.Stealth || target.Skills.Hiding.BaseFixedPoint >= 800 ) )
				{
					int textHue = 0;
					if( skillamt > skillmax || skillamt > skill.CapFixedPoint )
						textHue = 33;

					AddImageTiled( 10, 32 + (index * 22), 240, 23, 0x52 );
					AddImageTiled( 11, 33 + (index * 22), 238, 21, 0xBBC );

					AddLabelCropped( 13, 33 + (index * 22), 150, 21, textHue, skill.Name );
					AddImageTiled( 180, 34 + (index * 22), 50, 19, 0x52 );
					AddImageTiled( 181, 35 + (index * 22), 48, 17, 0xBBC );
					AddLabelCropped( 182, 35 + (index * 22), 234, 21, textHue, skill.Base.ToString( "F1" ) );

					AddButton( 231, 35 + (index * 22), 0x15E1, 0x15E5, i + 1, GumpButtonType.Reply, 0 );

					++index;
				}
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( from == null || m_SkillBall.Deleted )
				return;

			if ( m_SkillBall.Expires && DateTime.Now >= m_SkillBall.ExpireDate )
				m_SkillBall.SendLocalizedMessageTo( from, 1042544 ); // This item is out of charges.
			else if ( !m_SkillBall.IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else if ( m_SkillBall.ValidateUser( from ) )
				from.SendMessage( "Only the owner can use this skill ball." );
			else if ( info.ButtonID > 0 )
			{
				SkillName skillname = (m_SkillBall.GetAllowedSkills( m_Target ))[info.ButtonID-1];
				m_Skill = m_Target.Skills[skillname];

				if ( m_Skill == null )
					return;

				int count = m_Target.SkillsTotal;
				int cap = m_Target.SkillsCap;
				int decreaseamount;
				int bonus = m_SkillBall.SkillBonusFixedPoint; // Fix by Silver, old: m_SkillBall.SkillBonus;

				List<Skill> decreased = m_SkillBall.GetDecreasableSkills( from, m_Target, count, cap, out decreaseamount );

				int skillamt = m_Skill.BaseFixedPoint + m_SkillBall.SkillBonusFixedPoint;
				int skillmax = m_SkillBall.MaxCapFixedPoint;
				int skillmin = m_SkillBall.MinCapFixedPoint;

				if ( skillname == SkillName.Stealth && m_Target.Skills[SkillName.Hiding].BaseFixedPoint < 800 )
					from.SendMessage("You cannot train stealth until you have at least 80% hiding skill." );
				else if ( m_Skill.BaseFixedPoint > skillmin )
					from.SendMessage("You may only choose skills which are below {0}.", m_SkillBall.MinCap );
				else if ( m_Skill.Lock != SkillLock.Up )
					from.SendMessage( "You must set the skill to be increased in order to raise it further." );
				else
				{// SILVER: >= 0 instead of >= bonus. Added fourth argument for IncreaseSkill

					int toIncrease = bonus;
					if( bonus > (cap - count + decreaseamount) )
						toIncrease = cap - count + decreaseamount;
					if( m_Skill.BaseFixedPoint + toIncrease > skillmax )
						toIncrease = skillmax - m_Skill.BaseFixedPoint;
					if( m_Skill.BaseFixedPoint + toIncrease > m_Skill.CapFixedPoint )
						toIncrease = m_Skill.CapFixedPoint - m_Skill.BaseFixedPoint;

					if ( toIncrease > 0 )
					{
						m_SkillBall.DecreaseSkills( from, m_Target, decreased, count, cap );
						m_SkillBall.IncreaseSkill( from, m_Target, m_Skill, toIncrease );
						if ( !m_SkillBall.Rechargable )
							m_SkillBall.Delete();
					}
					else // SILVER: "any skill" instead of "enough skill"
						from.SendMessage( "You have exceeded the skill cap and do not have any skill set to be decreased." );
				}
			}
		}
	}
}