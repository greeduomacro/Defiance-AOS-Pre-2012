using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;

namespace Server.Mobiles
{
	public abstract class BaseVampire : BaseCreature
	{

		public BaseVampire( AIType aiType, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed ) : base ( aiType, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed )
		{
		}

		public BaseVampire( Serial serial ) : base( serial )
		{
		}

		public void ConvertFromWisp()
		{
			if ( this is VampireNecro ) {
				this.AI = AIType.AI_Necro;
			}
			this.SetSkill( SkillName.Hiding, 0 );
			this.SetSkill( SkillName.Stealth, 0 );
			this.SetSkill( SkillName.SpiritSpeak, 100 );
		}

			public override double WeaponAbilityChance { get { return 0.5; } }
			public override WeaponAbility GetWeaponAbility()
			{
				if (Weapon is BaseWeapon)
				{
					BaseWeapon wep = (BaseWeapon)Weapon;

					if (Utility.RandomBool())
						return wep.PrimaryAbility;
					else
						return wep.SecondaryAbility;
				}

				return base.GetWeaponAbility();
			}

		private void SpawnZombie( object state )
		{

			Mobile target = (Mobile)state;

			Map map = this.Map;

			if ( map == null )
				return;

			BaseCreature zombie = new MindlessZombie();
			zombie.Name = target.Name;
			zombie.Body = target.Body;
			double targMage = target.Skills[SkillName.Magery].Value;
			double targNecro = target.Skills[SkillName.Necromancy].Value;
			if ( targMage > 40 || targNecro > 40 )
			{
				if ( targMage > targNecro )
					zombie.AI = AIType.AI_Mage;
				else
					zombie.AI = AIType.AI_Necro;
			}
			zombie.SetSkill( SkillName.Anatomy, target.Skills[SkillName.Anatomy].Value );
			zombie.SetSkill( SkillName.Stealth, target.Skills[SkillName.Stealth].Value );
			zombie.SetSkill( SkillName.Hiding, target.Skills[SkillName.Hiding].Value );
			zombie.SetSkill( SkillName.Meditation, target.Skills[SkillName.Meditation].Value );
			zombie.SetSkill( SkillName.Magery, targMage );
			zombie.SetSkill( SkillName.EvalInt, target.Skills[SkillName.EvalInt].Value );
			zombie.SetSkill( SkillName.Necromancy, targNecro );
			zombie.SetSkill( SkillName.SpiritSpeak, target.Skills[SkillName.SpiritSpeak].Value );
			zombie.RawStr = target.RawStr;
			zombie.RawDex = target.RawDex;
			zombie.RawInt = target.RawInt;
			zombie.SetHits( target.HitsMax );
			zombie.Team = this.Team;

			Corpse corpse = (Corpse)target.Corpse;

			corpse.ProcessDelta();
			corpse.SendRemovePacket();
			corpse.ItemID = Utility.Random( 0xECA, 9 ); // bone graphic
			corpse.Hue = 0;
			corpse.ProcessDelta();

				Point3D loc = corpse.GetWorldLocation();
				zombie.MoveToWorld( loc, map );
				zombie.Home = loc;

		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );
			if ( !(defender.Alive) && ( defender is PlayerMobile ) )
			{
				Corpse c = (Corpse)defender.Corpse;
				Point3D p = c.GetWorldLocation();
				Map map = c.Map;
				if ( map != null )
				{
					this.Say( "Rise! Rise my servant!" );
					Effects.PlaySound( p, map, 0x1FB );
					Effects.SendLocationParticles( EffectItem.Create( p, map, EffectItem.DefaultDuration ), 0x3789, 1, 40, 0x3F, 3, 9907, 0 );
					Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerStateCallback( SpawnZombie ), defender );
				}
			}
		}

		public override void OnCombatantChange()
		{
			int bValue = 165;
			if ( this.Combatant != null && !this.Mounted )
			{
				if ( this.Body == 165 )
					ConvertFromWisp();
				bValue = 317;
				this.ActiveSpeed = 0.1;
			}
			else
			{
				if ( this.Female )
					bValue = 401;
				else
					bValue = 400;
				this.ActiveSpeed = 0.2;
			}
			this.Body = bValue;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{

			if ( this.Hits - amount < 30 && !willKill && !this.Mounted )
			{
				if ( this is VampireNecro ) {
					this.AI = AIType.AI_Stealth;
				}
				AIObject.Action = ActionType.Flee;
				this.Combatant = null;
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				this.Body = 165;
				this.Hidden = true;
				this.SetSkill( SkillName.Hiding, 100 );
				this.SetSkill( SkillName.Stealth, 100 );
				this.SetSkill( SkillName.SpiritSpeak, 0 );
				this.AllowedStealthSteps = 24;
			}
			else
			{
				Item toCurse = this.FindItemOnLayer( Layer.OneHanded );
				if ( toCurse == null )
					toCurse = this.FindItemOnLayer( Layer.TwoHanded );
				if ( (toCurse != null) && !(toCurse is BaseShield) && !((BaseWeapon)toCurse).Cursed && this.Combatant != null )
				{
					Spell spell = null;
					spell = new CurseWeaponSpell( this, null );
					if ( spell != null )
						spell.Cast();
				}
			}
		}

		public override void OnThink()
		{

			if ( this.Hidden && this.Hits > 100 )
			{
				this.Body = 317;
				ConvertFromWisp();
			}
			else if ( !this.Hidden && this.Body == 165 )
			{
				this.UseSkill( SkillName.Hiding );
				this.AllowedStealthSteps = 24;
			}
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