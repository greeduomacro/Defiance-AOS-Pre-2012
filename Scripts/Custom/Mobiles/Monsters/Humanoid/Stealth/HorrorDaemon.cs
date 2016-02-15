using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a burnt corpse" )]
	public class HorrorDaemon : BaseCreature
	{
		[Constructable]
		public HorrorDaemon() : base( AIType.AI_Stealth, FightMode.Closest, 10, 1, 0.1, 0.2 )
		{
			Name = "a horror servant";
			Body = 400;
			Hue = 777;
			Hidden = true;

			SetStr( 150 );
			SetDex( 100 );
			SetInt( 1, 2 );

			SetHits( 80 );
			SetMana( 0 );

			SetDamage( 15, 20 );

			SetDamageType( ResistanceType.Fire, 100 );

			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 100 );
			SetResistance( ResistanceType.Cold, 30, 35 );
			SetResistance( ResistanceType.Poison, 30, 35 );
			SetResistance( ResistanceType.Energy, 30, 35 );

			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );
			SetSkill( SkillName.Hiding, 100.0 );

			Fame = 3500;
			Karma = -3500;

			VirtualArmor = 6;

			Lantern lantern = new Lantern();
			lantern.Name = "Fire bomb";
			lantern.Hue = 1281;
			lantern.Movable = false;
			AddItem( lantern );

			BoneGloves gloves = new BoneGloves();
			gloves.Movable = false;
			gloves.Hue = 32500;
			AddItem( gloves );

			BoneLegs legs = new BoneLegs();
			legs.Movable = false;
			legs.Hue = 32500;
			AddItem( legs );

			BoneChest chest = new BoneChest();
			chest.Movable = false;
			chest.Hue = 32500;
			AddItem( chest );

			BoneHelm helm = new BoneHelm();
			helm.Movable = false;
			helm.Hue = 32500;
			AddItem( helm );

			PackItem( new Bloodmoss( Utility.RandomMinMax( 7, 10 ) ) );
			PackItem( new MandrakeRoot( Utility.RandomMinMax( 7, 10 ) ) );
			PackItem( new Head() );
			PackItem( new ExplosionScroll() );

		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			Explode();
		}

		public void Explode()
		{
			Map map = this.Map;

			if ( map == null )
				return;

			ArrayList targets = new ArrayList();

			foreach ( Mobile m in this.GetMobilesInRange( 4 ) )
			{
				if ( m == this || m is HorrorDaemon || !m.CanBeDamaged() || !this.InLOS( m ) )
					continue;

				if ( m is BaseCreature )
					targets.Add( m );
				else if ( m.Player )
					targets.Add( m );
			}

			Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x36BD, 20, 10, 5044 );
			Effects.PlaySound( this.Location, this.Map, 0x307 );

			for ( int i = 0; i < targets.Count; ++i )
			{
				Mobile m = (Mobile)targets[i];
				int damage;

				if ( m.InRange( this.Location, 1 ) )
					damage = 80;
				else if ( m.InRange( this.Location, 2 ) )
					damage = 60;
				else
					damage = 40;

				DoHarmful( m );

				AOS.Damage( m, this, damage, 0, 100, 0, 0, 0 );
			}

			this.Delete();
		}

		public HorrorDaemon(Serial serial) : base(serial)
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