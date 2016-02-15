using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;
using Server.Engines.RewardSystem;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "a ghostly corpse" )]
	public class Ghost : BaseCreature
	{

		public override double WeaponAbilityChance { get { return 0.8; } }
		public override WeaponAbility GetWeaponAbility()
		{
			if (Weapon is BaseWeapon)
			{
				BaseWeapon wep = (BaseWeapon)Weapon;
				return wep.SecondaryAbility;
			}
			return base.GetWeaponAbility();
		}

		[Constructable]
		public Ghost() : base( AIType.AI_Stealth, FightMode.Closest, 10, 1, 0.1, 0.2 )
		{

			switch ( Utility.Random( 31 ) )
			{
				case 0: Name = "MUDDICK"; break;
				case 1: Name = "Mr Nonames"; break;
				case 2: Name = "Estraz"; break;
				case 3: Name = "Aurora"; break;
				case 4: Name = "Laputus"; break;
				case 5: Name = "Zapp"; break;
				case 6: Name = "El Diablo"; break;
				case 7: Name = "Killermage"; break;
				case 8: Name = "Lyuze"; break;
				case 9: Name = "El Lobo"; break;
				case 10: Name = "Raven Riley"; break;
				case 11: Name = "TIESTO"; break;
				case 12: Name = "HavoK"; break;
				case 13: Name = "Eager"; break;
				case 14: Name = "Julie"; break;
				case 15: Name = "Sexi Seppo"; break;
				case 16: Name = "Aurelka"; break;
				case 17: Name = "666"; break;
				case 18: Name = "Unas"; break;
				case 19: Name = "Hamul"; break;
				case 20: Name = "Asha Midori"; break;
				case 21: Name = "PLUMP"; break;
				case 22: Name = "FUCKOWN"; break;
				case 23: Name = "Puss-n-boots"; break;
				case 24: Name = "An energy vortex"; break;
				case 25: Name = "Rouche"; break;
				case 26: Name = "Marjatta"; break;
				case 27: Name = "Mayhem"; break;
				case 28: Name = "Toxic"; break;
				case 29: Name = "All Eyez On Me"; break;
				default: Name = "Bellatrix"; break;
			}

			Body = 970;
			Hue = 910;

			SetStr( 650, 900 );
			SetDex( 150, 200 );
			SetInt( 860, 1000 );

			SetHits( 2500, 3000 );

			SetDamage( 22, 25 );

			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Cold, 40 );
			SetDamageType( ResistanceType.Poison, 30 );

			SetResistance( ResistanceType.Physical, 50, 60 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 55 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.Anatomy, 110.2, 120.0 );
			SetSkill( SkillName.Archery, 120.1, 130.0 );
			SetSkill( SkillName.MagicResist, 90.1, 100.0 );
			SetSkill( SkillName.Tactics, 110.1, 120.0 );
			SetSkill( SkillName.Hiding, 100.0, 100.0 );
			SetSkill( SkillName.Stealth, 120.0, 120.0 );
			SetSkill( SkillName.SpiritSpeak, 100.1, 110.0 );
			SetSkill( SkillName.Meditation, 100.1, 110.0 );
			SetSkill( SkillName.EvalInt, 70.1, 80.0 );

			Fame = 25000;
			Karma = -25000;

			VirtualArmor = 40;

			HeavyCrossbow weapon = new HeavyCrossbow();

			weapon.Movable = false;
			weapon.Attributes.WeaponDamage = 35;
			weapon.Attributes.AttackChance = 40;
			weapon.Attributes.RegenHits = 30;
			weapon.WeaponAttributes.HitLightning = 100;
			weapon.WeaponAttributes.HitDispel = 100;

			AddItem( weapon );
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 5 );
		}

		public override void OnDeath( Container c )
		{
			List<DamageStore> rights = BaseCreature.GetLootingRights( this.DamageEntries, this.HitsMax );
			List<Mobile> toGive = new List<Mobile>();

			for ( int i = rights.Count - 1; i >= 0; --i )
			{
				DamageStore ds = rights[i];

				if ( ds.m_HasRight )
					toGive.Add( ds.m_Mobile );
			}

			int chance = Utility.Random( 100 );

			if ( toGive.Count > 0 )
			{
				Mobile rewardmob = toGive[Utility.Random( toGive.Count )];
				Mobile weapmob = toGive[Utility.Random( toGive.Count )];

				if ( chance < 30 )
				{
					rewardmob.AddToBackpack( new PowerScroll( PowerScroll.Skills[Utility.Random(PowerScroll.Skills.Count)], (100+(Utility.Random(3)+2)*5) ) );
					rewardmob.SendMessage( "You have been rewarded with a powerscroll" );
				}
				else if ( chance < 40 )
				{
					rewardmob.AddToBackpack( new CopperBar( Utility.Random( 2 ) + 2 ) );
					rewardmob.SendMessage( "You have been rewarded with copper bars" );
				}
				else if ( chance < 50 )
				{
					rewardmob.AddToBackpack( new ClothingBlessDeed() );
					rewardmob.SendMessage( "You have been rewarded with a Clothing Bless Deed" );
				}

				if ( 10 > Utility.Random( 100 ) )
				{
					weapmob.AddToBackpack( new UndeadsBane() );
					weapmob.SendMessage( "You have received the magical weapon!" );
				}
			}
			else
			{
				if ( chance < 30 )
					c.DropItem( new PowerScroll( PowerScroll.Skills[Utility.Random(PowerScroll.Skills.Count)], (100+(Utility.Random(3)+2)*5) ) );
				else if ( chance < 40 )
					c.DropItem( new CopperBar(Utility.Random( 2 ) + 1) );
				else if ( chance < 50 )
					c.DropItem( new ClothingBlessDeed() );

				if ( 10 > Utility.Random( 100 ) )
					c.DropItem( new UndeadsBane() );
			}

			base.OnDeath( c );
		}

		public override void OnBeforeSpawn( Point3D location, Map m )
		{
			base.OnBeforeSpawn( location, m );

			IsParagon = false;
			IsElder = false;
			IsPlagued = false;
			Hue = 910;
		}

		public Ghost( Serial serial ) : base( serial )
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