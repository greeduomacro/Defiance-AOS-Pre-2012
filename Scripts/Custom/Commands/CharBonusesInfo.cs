using System;
using Server;
using Server.Commands;
using Server.Items;

namespace Server.Scripts.Commands
{
	public class CharBonusesInfo
	{
		public static void Initialize()
		{
			CommandSystem.Register("Bonuses", AccessLevel.Player, new CommandEventHandler(Bonuses_OnCommand));
		}

		[Usage( "Bonuses" )]
		[Description("Displays your character equipment bonuses.")]
		public static void Bonuses_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			Item weapon = from.FindItemOnLayer(Layer.OneHanded);
			int bonus = 0;
			if (AosAttributes.GetValue( from, AosAttribute.LowerRegCost ) != 0 )
				from.SendMessage( String.Format("Your LRC is {0}%", AosAttributes.GetValue( from, AosAttribute.LowerRegCost ).ToString() ));
			if (AosAttributes.GetValue( from, AosAttribute.LowerManaCost ) != 0 )
				from.SendMessage( String.Format("Your LMC is {0}%.", AosAttributes.GetValue( from, AosAttribute.LowerManaCost ).ToString() ));
			if (AosAttributes.GetValue( from, AosAttribute.CastSpeed ) != 0 )
				from.SendMessage( String.Format("Your FC is {0}.", AosAttributes.GetValue( from, AosAttribute.CastSpeed ).ToString() ));
			if (AosAttributes.GetValue( from, AosAttribute.CastRecovery ) != 0 )
				from.SendMessage( String.Format("Your FCR is {0}.", AosAttributes.GetValue( from, AosAttribute.CastRecovery ).ToString() ));
			if (AosAttributes.GetValue( from, AosAttribute.SpellDamage ) != 0 )
				from.SendMessage( String.Format("Your Spell Damage Increase is {0}%.", AosAttributes.GetValue( from, AosAttribute.SpellDamage ).ToString() ));

			if (weapon != null && weapon is BaseWeapon && ((BaseWeapon)weapon).Quality == WeaponQuality.Exceptional)
				bonus = 20;
			weapon = from.FindItemOnLayer(Layer.TwoHanded);
			if (weapon != null && weapon is BaseWeapon && ((BaseWeapon)weapon).Quality == WeaponQuality.Exceptional)
				bonus = 20;
			if (AosAttributes.GetValue( from, AosAttribute.WeaponDamage ) + bonus != 0 )
				from.SendMessage( String.Format("Your Damage increase is {0}%.", (AosAttributes.GetValue( from, AosAttribute.WeaponDamage ) + bonus).ToString() ));

			if (weapon != null && weapon is BaseWeapon && weapon.Layer == Layer.TwoHanded && ((BaseWeapon)weapon).MaxRange == 1)
				bonus = 5;
			else
				bonus = 0;
			if (AosAttributes.GetValue( from, AosAttribute.AttackChance ) + bonus != 0)
				from.SendMessage( String.Format("Your Hit Chance Increase is {0}%.", (AosAttributes.GetValue( from, AosAttribute.AttackChance ) + bonus).ToString() ));
			if (AosAttributes.GetValue( from, AosAttribute.DefendChance ) + bonus != 0 )
				from.SendMessage( String.Format("Your Defense Chance Increase is {0}%.", (AosAttributes.GetValue( from, AosAttribute.DefendChance ) + bonus).ToString() ));
			if (AosAttributes.GetValue( from, AosAttribute.ReflectPhysical ) != 0 )
				from.SendMessage( String.Format("Your Reflect Physical Damage is {0}%.", AosAttributes.GetValue( from, AosAttribute.ReflectPhysical ).ToString() ));
			if (AosAttributes.GetValue( from, AosAttribute.RegenHits ) != 0 )
				from.SendMessage( String.Format("Your Hit Point Regeneration is {0}.", AosAttributes.GetValue( from, AosAttribute.RegenHits ).ToString() ));
			if (AosAttributes.GetValue( from, AosAttribute.RegenStam ) != 0 )
				from.SendMessage( String.Format("Your Stamina Regeneration is {0}.", AosAttributes.GetValue( from, AosAttribute.RegenStam ).ToString() ));
			if (AosAttributes.GetValue( from, AosAttribute.RegenMana ) != 0 )
				from.SendMessage( String.Format("Your Mana Regeneration is {0}.", AosAttributes.GetValue( from, AosAttribute.RegenMana ).ToString() ));
		}
	}
}