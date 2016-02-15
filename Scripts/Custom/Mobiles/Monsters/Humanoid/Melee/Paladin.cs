using System;
using System.Collections;
using Server;
using Server.Misc;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a paladin's corpse" )]
	public class Paladin : BaseCreature
	{
		[Constructable]
		public Paladin() : base( AIType.AI_Paladin, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Title = "the Dark Knight";
			Hue = Utility.RandomSkinHue();

			if ( this.Female = Utility.RandomBool() )
			{
				this.Body = 0x191;
				this.Name = NameList.RandomName( "female" );

				Item chest = new FemalePlateChest();

				chest.Hue = 0x47E;
				chest.Movable = false;

				AddItem (chest );
			}
			else
			{
				this.Body = 0x190;
				this.Name = NameList.RandomName( "male" );

				Item chest = new PlateChest();

				chest.Hue = 0x47E;
				chest.Movable = false;

				AddItem (chest );
			}

			TithingPoints = 0x3E8;

			SetStr( 150 );
			SetDex( 100 );
			SetInt( 100 );

			SetHits( 250, 300 );
			SetMana( 400 );

			SetDamage( 10, 20 );

			SetDamageType( ResistanceType.Physical, 60 );
			SetDamageType( ResistanceType.Fire, 20 );
			SetDamageType( ResistanceType.Energy, 20 );

			SetResistance( ResistanceType.Physical, 65 );
			SetResistance( ResistanceType.Fire, 50 );
			SetResistance( ResistanceType.Cold, 50 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 50 );

			SetSkill( SkillName.Chivalry, 95.1, 100.1 );
			SetSkill( SkillName.Focus, 90.1, 100.1 );
			SetSkill( SkillName.Meditation, 100.0 );
			SetSkill( SkillName.MagicResist, 85.1, 95.0 );
			SetSkill( SkillName.Magery, 95.1, 100.0 );
			SetSkill( SkillName.Anatomy, 90.1, 100.1 );
			SetSkill( SkillName.Parry, 85.1, 95.1 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Swords, 100.0 );

			Fame = 5000;
			Karma = -5000;

			VirtualArmor =35;

			VikingSword weapon = new VikingSword();

			weapon.DamageLevel = (WeaponDamageLevel)Utility.Random( 3, 6 );
			weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random( 0, 1 );
			weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random( 3, 6 );
			weapon.Skill = SkillName.Swords;
			weapon.Speed = 40;
			weapon.Hue = 0x8A5;
			weapon.Weight = 0.5;
			weapon.Movable = false;
			weapon.Attributes.SpellChanneling = 1;
			weapon.Quality = WeaponQuality.Exceptional;
			weapon.Name = "Holy Avenger";

			Item helm = new PlateHelm();

			helm.Hue = 0x47E;
			helm.Movable = false;

			Item neck = new PlateGorget();

			neck.Hue = 0x47E;
			neck.Movable = false;

			Item arms = new PlateArms();

			arms.Hue = 0x47E;
			arms.Movable = false;

			Item legs = new PlateLegs();

			legs.Hue = 0x47E;
			legs.Movable = false;

			Item gloves = new PlateGloves();

			gloves.Hue = 0x47E;
			gloves.Movable = false;

			Item shield = new OrderShield();

			shield.Movable = false;

			Item cape = new Cloak();

			cape.Hue = 0xCF;
			cape.Movable = false;

			Item sash = new BodySash();

			sash.Hue = 0xCF;
			sash.Movable = false;

			Spellbook book = new BookOfChivalry( (ulong)0x3FF );

			book.LootType = LootType.Blessed;

			PackItem( book );


			PackGold( 200, 300 );
			PackGem( 5, 8 );
			PackGem( 3, 6 );
			PackMagicItems( 3, 5, 0.50, 0.75 );
			PackMagicItems( 3, 5, 0.50, 0.75 );


			AddItem( weapon );
			AddItem( gloves );
			AddItem( arms );
			AddItem( neck );
			AddItem( helm );
			AddItem( shield );
			AddItem( legs );
			AddItem( sash );
			AddItem( cape );
			Utility.AssignRandomHair( this );


		}



public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( from is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)from;

				if ( bc.Controlled || bc.BardTarget == this )
					damage = 0; // Immune to pets and provoked creatures
			}
		}
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
        public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool BardImmune{ get{ return true; } }
		public override bool Unprovokable{ get{ return true; } }
		public override bool Uncalmable{ get{ return true; } }

/*                public override void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
			if ( to is Dragon || to is WhiteWyrm || to is FireSteed || to is IceSteed || to is GoldenDragon || to is SwampDragon || to is Drake || to is Nightmare || to is Daemon )
				damage *= 2;
		}
*/
		public Paladin( Serial serial ) : base( serial )
		{
		}

 // start Special Attack Prototype
 // Mortal Strike 10% chance

 public static readonly TimeSpan PlayerDuration = TimeSpan.FromSeconds( 6.0 );
 public static readonly TimeSpan NPCDuration = TimeSpan.FromSeconds( 12.0 );

 public override void OnGaveMeleeAttack(Mobile defender)
 {

 double chanceofspecialmove = .1;
 double random = Utility.RandomDouble();
 if (chanceofspecialmove > random)
 {
 this.SendLocalizedMessage( 1060086 ); // You deliver a mortal wound!
 defender.SendLocalizedMessage( 1060087 ); // You have been mortally wounded!

 defender.PlaySound( 0x1E1 );
 defender.FixedParticles( 0x37B9, 244, 25, 9944, 31, 0, EffectLayer.Waist );

 BeginWound( defender, defender.Player ? PlayerDuration : NPCDuration );
 }
 }

 private static Hashtable m_Table = new Hashtable();

 public static bool IsWounded( Mobile m )
 {
 return m_Table.Contains( m );
 }

 public static void BeginWound( Mobile m, TimeSpan duration )
 {
 Timer t = (Timer)m_Table[m];

 if ( t != null )
 t.Stop();

 t = new InternalTimer( m, duration );
 m_Table[m] = t;

 t.Start();

 m.YellowHealthbar = true;
 }

 public static void EndWound( Mobile m )
 {
 Timer t = (Timer)m_Table[m];

 if ( t != null )
 t.Stop();

 m_Table.Remove( m );

 m.YellowHealthbar = false;
 m.SendLocalizedMessage( 1060208 ); // You are no longer mortally wounded.
 }

 private class InternalTimer : Timer
 {
 private Mobile m_Mobile;

 public InternalTimer( Mobile m, TimeSpan duration ) : base( duration )
 {
 m_Mobile = m;
 Priority = TimerPriority.TwoFiftyMS;
 }

 protected override void OnTick()
 {
 EndWound( m_Mobile );
 }
 }

 // end Special Attack Prototype
 // Dont recall who made this.. but its great, ad makes the pallys a powerful foe to fight.

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