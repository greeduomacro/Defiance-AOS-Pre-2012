using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a poisonous rat corpse" )]
	public class PoisonousRat : BaseCreature
	{
		[Constructable]
		public PoisonousRat() : base( AIType.AI_Stealth, FightMode.Closest, 10, 1, 0.1, 0.2 )
		{
			Name = "a poisonous rat";
			Body = 238;
			BaseSoundID = 0xCC;
			Hue = 165;
			Hidden = true;
			AllowedStealthSteps = 24;

			SetStr( 9 );
			SetDex( 25 );
			SetInt( 6, 10 );

			SetHits( 40 );
			SetMana( 0 );

			SetDamage( 1, 2 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 5, 10 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 5, 10 );

			SetSkill( SkillName.MagicResist, 5.0 );
			SetSkill( SkillName.Tactics, 5.0 );
			SetSkill( SkillName.Wrestling, 100.0 );
			SetSkill( SkillName.Hiding, 100.0 );
			SetSkill( SkillName.Stealth, 100.0, 110.0 );

			Fame = 300;
			Karma = -300;

			VirtualArmor = 6;

			PackItem( new GreaterCurePotion() );
		}

		public override int Meat{ get{ return 1; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Eggs | FoodType.FruitsAndVeggies; } }

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			PoisonAll();
		}

		public void PoisonAll()
		{
			Map map = this.Map;

			if ( map == null )
				return;

			ArrayList targets = new ArrayList();

			foreach ( Mobile m in this.GetMobilesInRange( 3 ) )
			{
				if ( m == this || m is PoisonousRat || !m.CanBeDamaged() || !this.InLOS( m ) )
					continue;

				if ( m is BaseCreature )
					targets.Add( m );
				else if ( m.Player )
					targets.Add( m );
			}

			Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x36B0, 1, 14, 63, 7, 9915, 0 );
			Effects.PlaySound( this.Location, this.Map, 0x229 );

			int Level;

			for ( int i = 0; i < targets.Count; ++i )
			{
				Mobile m = (Mobile)targets[i];
				double damage = 10;

				if ( m.InRange( this.Location, 1 ) )
					Level = 4;
				else if ( m.InRange( this.Location, 2 ) )
					Level = 3;
				else
					Level = 2;

				DoHarmful( m );

				AOS.Damage( m, this, (int)damage, 0, 0, 0, 100, 0 );

				if ( m.Alive )
				{
					m.PlaySound( 0xDD );
					m.FixedParticles( 0x3728, 244, 25, 9941, 1266, 0, EffectLayer.Waist );
					m.ApplyPoison( this, Poison.GetPoison( Level ) );
				}

			}

			this.Kill();
			if ( this.Corpse != null )
				this.Corpse.Delete();

		}

		public PoisonousRat(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}