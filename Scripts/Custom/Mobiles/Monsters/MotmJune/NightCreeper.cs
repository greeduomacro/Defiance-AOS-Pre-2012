using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("a night creeper corpse")]
	[TypeAlias( "Server.Mobiles.NightCreeper" )]
	public class NightCreeper : BaseCreature
	{

		private DateTime nextSpawn;

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ShadowStrike;
		}

		[Constructable]
		public NightCreeper() : base( AIType.AI_Stealth, FightMode.Closest, 10, 1, 0.1, 0.2 )
		{
			Body = 302;
			Name = "a night creeper";
			Hue = 1;
			Hidden = true;
			AllowedStealthSteps = 24;
			nextSpawn = DateTime.Now;

			SetStr( 161, 320 );
			SetDex( 60, 100 );
			SetInt( 21, 40 );

			SetHits( 131, 180 );

			SetDamage( 8, 15 );

			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Poison, 70 );

			SetResistance( ResistanceType.Physical, 50, 60 );
			SetResistance( ResistanceType.Fire, 35, 45 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 35, 45 );

			SetSkill( SkillName.Poisoning, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 95.1, 100.0 );
			SetSkill( SkillName.Wrestling, 110.1, 120.0 );
			SetSkill( SkillName.Hiding, 100.0, 100.0 );
			SetSkill( SkillName.Stealth, 120.0, 120.0 );
			SetSkill( SkillName.Tactics, 50.1, 60.0 );

			Fame = 12000;
			Karma = -12000;

			VirtualArmor = 0;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Gems, 10 );
		}

		public void SpawnUndead( Mobile m )
		{
			Map map = this.Map;
			Effects.SendLocationParticles( EffectItem.Create( this.Location, map, EffectItem.DefaultDuration ), 0x3789, 1, 40, 0x3F, 3, 9907, 0 );
			Mobile spawn;
			switch ( Utility.Random( 12 ) )
			{
				default:
				case 0: spawn = new Skeleton(); break;
				case 1: spawn = new Zombie(); break;
				case 2: spawn = new Wraith(); break;
				case 3: spawn = new Spectre(); break;
				case 4: spawn = new Ghoul(); break;
				case 5: spawn = new Mummy(); break;
				case 6: spawn = new Bogle(); break;
				case 7: spawn = new RottingCorpse(); break;
				case 8: spawn = new BoneKnight(); break;
				case 9: spawn = new SkeletalKnight(); break;
				case 10: spawn = new Lich(); break;
				case 11: spawn = new LichLord(); break;
			}
			spawn.Hidden = true;
			spawn.MoveToWorld( m.Location, m.Map );
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( this.Hidden ) {
				if ( m is PlayerMobile && m.Alive && !(m.Hidden) && nextSpawn < DateTime.Now ) {
					SpawnUndead( m );
					nextSpawn = DateTime.Now + TimeSpan.FromSeconds( 20.0 );
				}
				return false;
			}

			return base.IsEnemy( m );
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }

		public NightCreeper(Serial serial) : base(serial)
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