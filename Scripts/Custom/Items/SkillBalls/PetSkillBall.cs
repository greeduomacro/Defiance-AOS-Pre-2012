using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Targeting;

namespace Server.Items
{
	public class PetSkillBall : SkillBall
	{
		public override string DefaultName{ get{ return "pet skill ball"; } }

		[Constructable]
		public PetSkillBall() : this( 25 )
		{
		}

		[Constructable]
		public PetSkillBall( int bonus ) : this( bonus, true )
		{
		}

		[Constructable]
		public PetSkillBall( int bonus, bool limited ) : this( bonus, 100, limited )
		{
		}

		[Constructable]
		public PetSkillBall( int bonus, int maxcap ) : this( bonus, maxcap, true )
		{
		}

		[Constructable]
		public PetSkillBall( int bonus, int maxcap, bool limited ) : this( bonus, maxcap, limited, 0 )
		{
		}

		[Constructable]
		public PetSkillBall( int bonus, int maxcap, bool limited, int days ) : this( bonus, maxcap, limited, days, 120 )
		{
		}

		[Constructable]
		public PetSkillBall( int bonus, int maxcap, bool limited, int days, int mincap ) : base( bonus, maxcap, limited, days, mincap )
		{
		}

		public PetSkillBall( Serial serial ) : base( serial )
		{
		}

		public override void UpdateHue()
		{
			Hue = 1428;
		}

		public override SkillName[] GetAllowedSkills( Mobile target )
		{
			if ( target.Skills[SkillName.Poisoning].Base > 0 )
				return m_PoisonPetSkills;
			if ( target.Skills[SkillName.Magery].Base > 0 )
				return m_MagePetSkills;
			else
				return m_MeleePetSkills;
		}

		public static readonly SkillName[] m_MeleePetSkills = new SkillName[]
			{
				SkillName.Anatomy,
				SkillName.MagicResist,
				SkillName.Tactics,
				SkillName.Wrestling,
				SkillName.Meditation,
			};

		public static readonly SkillName[] m_MagePetSkills = new SkillName[]
			{
				SkillName.Anatomy,
				SkillName.EvalInt,
				SkillName.Magery,
				SkillName.MagicResist,
				SkillName.Tactics,
				SkillName.Wrestling,
				SkillName.Meditation,
			};

		public static readonly SkillName[] m_PoisonPetSkills = new SkillName[]
			{
				SkillName.Anatomy,
				SkillName.MagicResist,
				SkillName.Tactics,
				SkillName.Wrestling,
				SkillName.Meditation,
				SkillName.Poisoning,
			};

		public override void SendGump( Mobile from )
		{
			from.SendMessage( "Target your pet to use the skill ball." );
			from.Target = new PetSkillBallTarget( from, this );
		}

		public override void IncreaseSkill( Mobile from, Mobile target, Skill skill, int toIncrease )
		{
			base.IncreaseSkill( from, target, skill, toIncrease );
			SendConfirmMessage( from, target, skill, toIncrease / 10 );
		}

		public virtual void SendConfirmMessage( Mobile from, Mobile target, Skill skill, double increased )
		{
			from.SendMessage( "Your pet, {0}'s skill in {1} has been raised by {2}", target.Name, skill.Name, increased );
		}

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

		public class PetSkillBallTarget : Target
		{
			private Mobile m_From;
			private PetSkillBall m_Ball;

			public PetSkillBallTarget( Mobile from, PetSkillBall ball ) : base( 12, false, TargetFlags.None )
			{
				m_From = from;
				m_Ball = ball;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				BaseCreature creature = targeted as BaseCreature;
				if ( creature == null || !( creature.Controlled && creature.ControlMaster == from ) )
					from.SendMessage( "You cannot use this skill ball on that." );
				else if ( from.HasGump( typeof(SkillBall) ) )
					from.SendMessage( "You are already using a skill ball." );
				else if ( m_Ball.Expires && DateTime.Now >= m_Ball.ExpireDate )
					m_Ball.SendLocalizedMessageTo( from, 1042544 ); // This item is out of charges.
				else if ( m_Ball.ValidateUser( from ) )
					from.SendMessage( "Only the owner can use this skill ball." );
				else
					from.SendGump( new SkillBallGump( from, creature, m_Ball ) );
			}
		}
	}
}