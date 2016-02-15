using System;
using Server;

namespace Server.Mobiles
{
	public class PriestOfMondain : BaseHealer
	{
		public override bool CanTeach{ get{ return true; } }

		public override bool CheckTeach( SkillName skill, Mobile from )
		{
			if ( !base.CheckTeach( skill, from ) )
				return false;

			return ( skill == SkillName.Anatomy )
				|| ( skill == SkillName.Parry )
				|| ( skill == SkillName.EvalInt )
				|| ( skill == SkillName.Healing )
				|| ( skill == SkillName.Magery )
				|| ( skill == SkillName.MagicResist )
				|| ( skill == SkillName.Tactics )
				|| ( skill == SkillName.SpiritSpeak );
		}

		[Constructable]
		public PriestOfMondain()
		{
			Title = "the priest of Mondain";

			Karma = -10000;

			SetSkill( SkillName.Forensics, 80.0, 100.0 );
			SetSkill( SkillName.SpiritSpeak, 80.0, 100.0 );
			SetSkill( SkillName.Swords, 80.0, 100.0 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public override bool CheckResurrect( Mobile m )
		{
			return true;
		}

		public PriestOfMondain( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}