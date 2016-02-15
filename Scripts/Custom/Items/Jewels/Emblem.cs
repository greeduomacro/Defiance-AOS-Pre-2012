using Server.Commands;
using Server.Items;
using System;
using System.Collections;
using Server.Targeting;


namespace Server.Items
{


	public class Emblem : GoldEarrings
	{
		private Item m_Artifact;
		private Mobile mob;
		private Container cont;

		public override bool OnEquip( Mobile from )
		{
			this.Movable = false;
			if (from.Kills > 4)
				from.SolidHueOverride = 33;
			else
				from.SolidHueOverride = 1266;
			from.Criminal = true;
			from.Kills = from.Kills + 5;

		if (from.Skills[SkillName.Magery].Value > 50)//&(from.Skills[SkillName.Healing].Value < 50))
			{
				this.Attributes.BonusHits = 15;
				this.Attributes.SpellDamage = 5;
				this.Attributes.LowerManaCost = 5;
				this.Attributes.RegenHits = 5;
				this.Attributes.RegenStam = 10;
//				this.Movable = false;
			}
		else if (from.Skills[SkillName.Necromancy].Value > 50)
		{
			this.Attributes.BonusStam = 20;
			this.Attributes.BonusHits = 10;
			this.Attributes.RegenMana = 2;
			this.Attributes.RegenHits = 2;
			this.Attributes.RegenStam = 5;
		}

		else if (from.Skills[SkillName.Healing].Value > 50)//&(from.Skills[SkillName.Magery].Value < 50))
		{
				this.Attributes.BonusHits = 15;
				this.Attributes.WeaponSpeed = 10;
				this.Attributes.WeaponDamage = 20;
				this.Attributes.RegenMana = 2;
				this.Attributes.RegenHits = 3;
		}

			from.SendMessage( "The voice in your head tells you :  I am thy blessing and thy curse." );
			from.SendMessage( "You feel the power of the Royal Britannian Emblem surge through you!" );
			return base.OnEquip( from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if (from == this.RootParentEntity)
				from.SendMessage( "POWER OVERWHELMING" );
		}

		public override DeathMoveResult OnParentDeath( Mobile parent )
		{
			this.Movable = true;
			this.Attributes.BonusHits = 0;
			this.Attributes.SpellDamage = 0;
			this.Attributes.LowerManaCost = 0;
			this.Attributes.RegenHits = 0;
			this.Attributes.RegenStam = 0;
			this.Attributes.BonusStam = 0;
			this.Attributes.RegenMana = 0;
			parent.SolidHueOverride = -1;
			if (parent.Kills >= 5)
				parent.Kills = parent.Kills -5;
			else parent.Kills = -parent.Kills;

		if ( !Movable )
				return DeathMoveResult.RemainEquiped;
			else if ( parent.KeepsItemsOnDeath )
				return DeathMoveResult.MoveToBackpack;
			else if ( CheckBlessed( parent ) )
				return DeathMoveResult.MoveToBackpack;
			else if ( CheckNewbied() && parent.Kills < 5 )
				return DeathMoveResult.MoveToBackpack;
			else if( parent.Player && Nontransferable )
				return DeathMoveResult.MoveToBackpack;
			else
				return DeathMoveResult.MoveToCorpse;
		}

		public override bool OnDragLift( Mobile from )
		{
			from.SendMessage(" Only death can save thee");
			return true;
		}

		[Constructable]
		public Emblem()
		{
			Name = " Royal Emblem of Britannia ";
			LootType = LootType.Cursed;
			Weight = 0.1;
		}





		public Emblem( Serial serial ) : base( serial )
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