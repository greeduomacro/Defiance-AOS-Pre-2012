using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Targeting;

namespace Server.Items
{
	public class DonationSkillBall : SkillBall
	{
		public override string DefaultName{ get{ return "donation skill ball"; } }

		[Constructable]
		public DonationSkillBall() : this( 25 )
		{
		}

		[Constructable]
		public DonationSkillBall( int bonus ) : this( bonus, true )
		{
		}

		[Constructable]
		public DonationSkillBall( int bonus, bool limited ) : this( bonus, 100, limited )
		{
		}

		[Constructable]
		public DonationSkillBall( int bonus, int maxcap ) : this( bonus, maxcap, true )
		{
		}

		[Constructable]
		public DonationSkillBall( int bonus, int maxcap, bool limited ) : this( bonus, maxcap, limited, 0 )
		{
		}

		[Constructable]
		public DonationSkillBall( int bonus, int maxcap, bool limited, int days ) : this( bonus, maxcap, limited, days, 120 )
		{
		}

		[Constructable]
		public DonationSkillBall( int bonus, int maxcap, bool limited, int days, int mincap ) : base( bonus, maxcap, limited, days, mincap )
		{
		}

		public DonationSkillBall( Serial serial ) : base( serial )
		{
		}

		public override void UpdateHue()
		{
			Hue = 33;
		}

		/*public override void IncreaseSkill( Mobile from, Mobile target, Skill skill )
		{
			base.IncreaseSkill( from, target, skill );
			if ( target is PlayerMobile )
				((PlayerMobile)target).Young = false;
		}*/

		public override SkillName[] GetAllowedSkills( Mobile target )
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
				SkillName.Cartography,	//
				SkillName.Cooking,		//
				SkillName.DetectHidden,
				SkillName.Discordance,
				SkillName.EvalInt,
				SkillName.Healing,
				SkillName.Fishing,		//
				SkillName.Forensics,
				SkillName.Herding,
				SkillName.Hiding,
				SkillName.Provocation,	//
				SkillName.Inscribe,		//
				SkillName.Lockpicking,	//
				SkillName.Magery,
				SkillName.MagicResist,
				SkillName.Tactics,
				SkillName.Snooping,
				SkillName.Musicianship,
				SkillName.Poisoning,	//
				SkillName.Archery,
				SkillName.SpiritSpeak,
				SkillName.Stealing,
				SkillName.AnimalTaming,	//
				SkillName.TasteID,
				SkillName.Tracking,
				SkillName.Veterinary,
				SkillName.Swords,
				SkillName.Macing,
				SkillName.Fencing,
				SkillName.Wrestling,
				SkillName.Lumberjacking,	//
				SkillName.Mining,			//
				SkillName.Meditation,
				SkillName.Stealth,
				SkillName.RemoveTrap,
				SkillName.Necromancy,
				SkillName.Focus,
				SkillName.Chivalry
			};

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}