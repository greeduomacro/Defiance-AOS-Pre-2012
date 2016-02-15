
using System;
using System.Collections;
using Server.Targeting;
using Server;
using Server.Spells;
using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;
using Server.Engines.RewardSystem;
using System.Collections.Generic;


namespace Server.Mobiles
{
    [CorpseName("a minotaur corpse")]
    public class FactionSpawn : BaseCreature
    {

        public override double WeaponAbilityChance { get { return 0.9; } }
        public override int TreasureMapLevel{ get{ return 5; } }

        private int starting_health;

        [Constructable]
        public FactionSpawn()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {

            Name = "Argas";
            Title = "the Enraged";

            Body = 262;
            Hue = 654;

            SetStr(1419, 1438);
            SetDex(309, 413);
            SetInt(129, 131);

            SetHits(15000, 16000);

            SetDamage(16, 30);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 90);
            SetResistance(ResistanceType.Fire, 65, 70);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 40, 60);
            SetResistance(ResistanceType.Energy, 50, 55);

            SetSkill(SkillName.Anatomy, 130.0, 130.0);
            SetSkill(SkillName.MagicResist, 107.0, 111.3);
            SetSkill(SkillName.Tactics, 107.0, 117.0);
            SetSkill(SkillName.Wrestling, 100.0, 105.0);

            Fame = 70000;
            Karma = -70000;

            VirtualArmor = 40;

            

	    BattleAxe wep = new BattleAxe();
            wep.Movable = true;
	    AddItem(wep);
            /*HeavyCrossbow weapon = new HeavyCrossbow();

            weapon.Movable = false;
            weapon.Attributes.WeaponDamage = 35;
            weapon.Attributes.AttackChance = 40;
            weapon.Attributes.RegenHits = 30;
            weapon.WeaponAttributes.HitLightning = 100;
            weapon.WeaponAttributes.HitDispel = 100;

            AddItem(weapon);*/
        }


       

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 5);
        }

	 /*  public override WeaponAbility GetWeaponAbility()
        {
            if (Weapon is BaseWeapon)
            {
                BaseWeapon wep = (BaseWeapon)Weapon;
                return wep.SecondaryAbility;
            }
            return base.GetWeaponAbility();
        }*/



	public override WeaponAbility GetWeaponAbility()
		{
			switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: return WeaponAbility.ConcussionBlow;
				case 1: return WeaponAbility.BleedAttack;
			}
		}
	

	private int level = 1;




        public override void AlterMeleeDamageTo(Mobile from, ref int damage)
        {
            if (from is BaseCreature && ((BaseCreature)from).Controlled && !((BaseCreature)from).Summoned)
                damage = damage * 3;
        }

		public override void OnDamagedBySpell( Mobile attacker )
		{
  			if ((int)starting_health * 0.33 > Hits)
            {
                level = 3;
            }
            else if ((int)starting_health*0.66 > Hits)
			{
                level = 2;
            }
           
            
            switch(level)
            {
                case 1: if ( attacker is BaseCreature )
                        {
					           base.OnDamagedBySpell( attacker );
                        }
                        else
                        {
                            base.OnDamagedBySpell( attacker );
                            DoCounter( attacker );
                        }
                    break;
                case 2: if (attacker is BaseCreature)
                    {
                        base.OnDamagedBySpell(attacker);
                        DoCounter(attacker);
                    }
                    else
                        base.OnDamagedBySpell(attacker);
                    break;
                case 3: base.OnDamagedBySpell(attacker);
                    DoCounter(attacker);
                    break;

            }
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{

            if ((int)starting_health * 0.33 > Hits)
            {
                level = 3;
            }
            else if ((int)starting_health * 0.66 > Hits)
            {
                level = 2;
            }

            switch (level)
            {
                case 1: if (attacker is BaseCreature)
                    {
                        base.OnGotMeleeAttack(attacker);
                    }
                    else
                    {
                        base.OnGotMeleeAttack(attacker);
                        DoCounter(attacker);
                    }
                    break;
                case 2: if (attacker is BaseCreature)
                    {
                        base.OnGotMeleeAttack(attacker); ;
                        DoCounter(attacker);
                    }
                    else
                        base.OnGotMeleeAttack(attacker);
                    break;
                case 3: if (attacker is BaseCreature)
                    {
                        base.OnGotMeleeAttack(attacker); ;
                        DoCounter(attacker);
                    }
                    else base.OnGotMeleeAttack(attacker);
                    break;

            }
		}





        public override void OnDeath(Container c)
        {
            
            List<DamageStore> rights = BaseCreature.GetLootingRights(this.DamageEntries, this.HitsMax);
            List<Mobile> toGive = new List<Mobile>();

            for (int i = rights.Count - 1; i >= 0; --i)
            {
                DamageStore ds = rights[i];

                if (ds.m_HasRight)
                    toGive.Add(ds.m_Mobile);
            }

            int chance = Utility.Random(100);
		
	        //World.Broadcast(0x35, true, "{0}is the roll", chance);
            if (toGive.Count > 0)
            {
                Mobile rewardmob = toGive[Utility.Random(toGive.Count)];
                Mobile weapmob = toGive[Utility.Random(toGive.Count)];

                if (rewardmob is PlayerMobile)
                {
                    Mobile m = (Mobile)rewardmob;

                }


                if (chance < 3)
                {
                    RunicHammer hammer = new RunicHammer(CraftResource.Valorite, Utility.Random(5) + 8);
                    hammer.LootType = LootType.Cursed;
                    rewardmob.AddToBackpack(hammer);
                    World.Broadcast(0x35, true, "{0} was rewarded with runic hammer", rewardmob.Name);
                }
                else if (chance < 5)
                {
                    RunicSewingKit kit = new RunicSewingKit(CraftResource.BarbedLeather, Utility.Random(5) + 8);
                    kit.LootType = LootType.Cursed;
                    rewardmob.AddToBackpack(kit);
                    World.Broadcast(0x35, true, "{0} was rewarded with runic sewing kit", rewardmob.Name);
                }
                else if (chance < 25)
                {
                    rewardmob.AddToBackpack(new CopperBar(Utility.Random(2) + 2));
                    rewardmob.SendMessage("You have been rewarded with copper bars");
                }
            }
            else
            {
                 if (chance < 40)
                    c.DropItem(new CopperBar(Utility.Random(2) + 1));
             }

            if (!Summoned && !NoKillAwards && DemonKnight.CheckArtifactChance(this))
                DemonKnight.DistributeArtifact(this);



            base.OnDeath(c);
        }

        public override void OnBeforeSpawn(Point3D location, Map m)
        {
            base.OnBeforeSpawn(location, m);
            starting_health = Hits;
            IsParagon = false;
            IsElder = false;
            IsPlagued = false;
         }


        public override int GetAngerSound()
        {
            return 0x597;
        }

        public override int GetIdleSound()
        {
            return 0x596;
        }

        public override int GetAttackSound()
        {
            return 0x599;
        }

        public override int GetHurtSound()
        {
            return 0x59a;
        }

        public override int GetDeathSound()
        {
            return 0x59c;
        }
		
	private void DoCounter( Mobile attacker )
	{
		if ( this.Map == null || ( attacker is BaseCreature && ((BaseCreature)attacker).BardProvoked ) )
			return;
		if ( 0.2 > Utility.RandomDouble() )
			{
				/* Counterattack with Hit Poison Area
				 * 20-25 damage, unresistable
				 * Lethal poison, 100% of the time
				 * Particle effect: Type: "2" From: "0x4061A107" To: "0x0" ItemId: "0x36BD" ItemIdName: "explosion" FromLocation: "(296 615, 17)" ToLocation: "(296 615, 17)" Speed: "1" Duration: "10" FixedDirection: "True" Explode: "False" Hue: "0xA6" RenderMode: "0x0" Effect: "0x1F78" ExplodeEffect: "0x1" ExplodeSound: "0x0" Serial: "0x4061A107" Layer: "255" Unknown: "0x0"
				 * Doesn't work on provoked monsters
				 */

				Mobile target = null;

				if ( attacker is BaseCreature )
				{
					Mobile m = ((BaseCreature)attacker).GetMaster();

					if ( m != null )
						target = m;
				}

				if ( target == null || !target.InRange( this, 25 ) )
					target = attacker;

				this.Animate( 10, 4, 1, true, false, 0 );

				ArrayList targets = new ArrayList();

				foreach ( Mobile m in target.GetMobilesInRange( 8 ) )
				{
					if ( m == this || !CanBeHarmful( m ) )
						continue;

					if ( m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team) )
						targets.Add( m );
					else if ( m.Player )
						targets.Add( m );
				}

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = (Mobile)targets[i];

					DoHarmful( m );

					AOS.Damage( m, this, Utility.RandomMinMax( 20, 25 ), true, 0, 0, 0, 100, 0 );

					m.FixedParticles( 0x36BD, 1, 10, 0x1F78, 0xA6, 0, (EffectLayer)255 );
					m.ApplyPoison( this, Poison.Lethal );
				}
			}
		}


        public FactionSpawn(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}