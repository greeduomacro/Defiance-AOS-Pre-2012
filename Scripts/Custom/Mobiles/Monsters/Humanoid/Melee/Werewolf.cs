using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a Werewolf corpse" )]
	public class Werewolf : BaseCreature
	{

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.Dismount;
		}

		[Constructable]
		public Werewolf () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Werewolf";
			Body = 0x190;
			Hue = 850;
			BaseSoundID = 0xE5;

			SetStr( 150, 160 );
			SetDex( 121, 125 );
			SetInt( 20, 22 );

			SetHits( 200, 250 );

			SetDamage( 10, 11 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Cold, 25 );

			SetResistance( ResistanceType.Physical, 60, 62 );
			SetResistance( ResistanceType.Fire, 30, 32 );
			SetResistance( ResistanceType.Cold, 50, 52 );
			SetResistance( ResistanceType.Poison, 50, 52 );
			SetResistance( ResistanceType.Energy, 40, 42 );

			SetSkill( SkillName.Anatomy, 90.1, 100.0 );
			SetSkill( SkillName.DetectHidden, 80.1, 90.0 );
			SetSkill( SkillName.Meditation, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 90.5, 100.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			Fame = 7000;
			Karma = -7000;

			VirtualArmor = 10;

			AddItem( new ShortPants( Utility.RandomNeutralHue() ) );

			GoldRing ring = new GoldRing();
			ring.Attributes.RegenHits = 10;
			ring.Movable = false;
			AddItem( ring );

/*			Item hair = new LongHair();
			hair.Hue = 443;
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem( hair );

			Item beard = new Goatee();
			beard.Hue = 443;
			beard.Layer = Layer.FacialHair;
			beard.Movable = false;
			AddItem( beard ); */

			PackItem( new Ribs( Utility.RandomMinMax( 5, 8 ) ) );

		}

		public override void OnCombatantChange()
		{
			if ( this.Combatant == null )
			{
				this.Body = 400;
				SetDamage( 10, 11 );
				this.ActiveSpeed = 0.2;
			}
			else
			{
				this.Body = 23;
				SetDamage( 20, 25 );
				this.ActiveSpeed = 0.02;
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 1 );
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			Item myRing = this.FindItemOnLayer( Layer.Ring );
			if ( myRing != null )
			{
			((GoldRing)myRing).Attributes.RegenHits = (int)(50 - ( ( ( Hits - amount ) * 40 ) / HitsMax ) );
			}
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool ClickTitle{ get{ return false; } }

		public Werewolf( Serial serial ) : base( serial )
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