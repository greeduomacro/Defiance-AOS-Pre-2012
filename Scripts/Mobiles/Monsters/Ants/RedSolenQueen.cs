using System;
using System.Collections;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a solen queen corpse" )]
	public class RedSolenQueen : BaseSolenAnt, IRedSolen
	{
		public override bool AcidPoolOnBreath { get { return true; } }
		public override bool UseAcidSack { get { return true; } }
		public override bool UseEggs{ get{ return true; } }
		public override AntEggType EggType{ get{ return AntEggType.RedEgg; } }
		public override bool HasBreath{ get{ return true; } }

		public override string DefaultName{ get{ return "a red solen queen"; } }

		[Constructable]
		public RedSolenQueen() : base()
		{
			Body = 783;
			BaseSoundID = 959;

			SetStr( 296, 320 );
			SetDex( 121, 145 );
			SetInt( 76, 100 );

			SetHits( 151, 162 );

			SetDamage( 10, 15 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Poison, 30 );

			SetResistance( ResistanceType.Physical, 30, 40 );
			SetResistance( ResistanceType.Fire, 30, 35 );
			SetResistance( ResistanceType.Cold, 25, 35 );
			SetResistance( ResistanceType.Poison, 35, 40 );
			SetResistance( ResistanceType.Energy, 25, 30 );

			SetSkill( SkillName.MagicResist, 70.0 );
			SetSkill( SkillName.Tactics, 90.0 );
			SetSkill( SkillName.Wrestling, 90.0 );

			Fame = 4500;
			Karma = -4500;

			VirtualArmor = 45;

			SolenHelper.PackPicnicBasket( this );

			PackItem( new ZoogiFungus( Utility.RandomMinMax( 5, 25 ) ) );

			if( 0.1 > Utility.RandomDouble() ) // Doubled by Silver
				PackItem( new BallOfSummoning() );
		}

		public override int GetAngerSound()
		{
			return 0x259;
		}

		public override int GetIdleSound()
		{
			return 0x259;
		}

		public override int GetAttackSound()
		{
			return 0x195;
		}

		public override int GetHurtSound()
		{
			return 0x250;
		}

		public override int GetDeathSound()
		{
			return 0x25B;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}


		public override bool IsEnemy( Mobile m )
		{
			if ( SolenHelper.CheckRedFriendship( m ) )
				return false;
			else
				return base.IsEnemy( m );
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			SolenHelper.OnRedDamage( from );

			base.OnDamage( amount, from, willKill );
		}

		public RedSolenQueen( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}