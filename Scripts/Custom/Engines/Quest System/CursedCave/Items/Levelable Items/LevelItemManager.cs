using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class LevelItemManager
	{
		/// <summary>
		/// The Number of levels our items can go to. If you
		/// change this, be sure the Exp table has the correct
		/// number of Integer values in it.
		/// </summary>
		public const int Levels = 10;

		private static int[] m_Table;

		public static int[] ExpTable
		{
			get{ return m_Table; }
		}

		public static void Initialize()
		{
			/* This is the EXP table. It defines how much EXP is
			 * needed to advance to the next level. */
			m_Table = new int[Levels]
			{
				0, // always atleast level 1
				10000, // level 1 to level 2
				20000, // level 2 to level 3
				30000, // level 3 to level 4
				40000, // level 4 to level 5
				50000, // level 5 to level 6
				60000, // level 6 to level 7
				70000, // level 7 to level 8
				80000, // level 8 to level 9
				90000 // level 9 to level 10
			};
		}

		#region Exp calculation methods

		private static bool IsMageryCreature( BaseCreature bc )
		{
			return ( bc != null && bc.AI == AIType.AI_Mage && bc.Skills[SkillName.Magery].Base > 5.0 );
		}

		private static bool IsFireBreathingCreature( BaseCreature bc )
		{
			if ( bc == null )
				return false;

			return bc.HasBreath;
		}

		private static bool IsPoisonImmune( BaseCreature bc )
		{
			return ( bc != null && bc.PoisonImmune != null );
		}

		private static int GetPoisonLevel( BaseCreature bc )
		{
			if ( bc == null )
				return 0;

			Poison p = bc.HitPoison;

			if ( p == null )
				return 0;

			return p.Level + 1;
		}

		public static int CalcExp( Mobile targ )
		{
			// Edit by Silver: Hits etc replaced with HitsMax etc, added factors
			double val = targ.HitsMax * 1.6 + targ.StamMax * 0.5 + targ.ManaMax * 0.5;

			for ( int i = 0; i < targ.Skills.Length; i++ )
				val += targ.Skills[i].Base;

			//if ( val > 700 )
			//	val = 700 + ((val - 700) / 3.66667);

			BaseCreature bc = targ as BaseCreature;

			if ( IsMageryCreature( bc ) )
				val += 100;

			if ( IsFireBreathingCreature( bc ) )
				val += 100;

			if ( IsPoisonImmune( bc ) )
				val += 100;

			if ( targ is VampireBat || targ is VampireBatFamiliar )
				val += 100;

			val += GetPoisonLevel( bc ) * 20;

			val /= 10;

			// Edit by Silver: Added cap
			if ( val > 333 )
				val = 333;

			return (int)val;
		}

		#endregion

		public static void CheckItems( Mobile killer, Mobile killed )
		{
			if ( killer != null )
			{
				for( int i = 0; i < 25; ++i )
				{
					Item item = killer.FindItemOnLayer( (Layer)i );

					if ( item != null && item is ILevelable )
						CheckLevelable( (ILevelable)item, killer, killed );
				}
			}
		}

		/// <summary>
		/// Calculates the value of the property based on its max
		/// value and current level
		/// </summary>
		/// <param name="max"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		public static int CalculateProperty( int max, int level )
		{
			return (int)( (double)max * ( (double)level / (double)LevelItemManager.Levels ) );
		}

		public static void InvalidateLevel( ILevelable item )
		{
			for( int i = 0; i < ExpTable.Length; ++i )
			{
				if ( item.Experience < ExpTable[i] )
					return;

				item.Level = i + 1;
			}
		}

		public static void CheckLevelable( ILevelable item, Mobile killer, Mobile killed )
		{
			if ( item.Types == null )
				return;

			if ( item.Level >= Levels )
				return;

			for( int i = 0; i < item.Types.Length; ++i )
			{
				if ( killed.GetType() == item.Types[i] )
				{
					int exp = CalcExp( killed );
					int oldLevel = item.Level;

					item.Experience += exp;

					InvalidateLevel( item );

					if ( item.Level != oldLevel )
						item.OnLevel( oldLevel, item.Level );

					if ( item is Item )
						((Item)item).InvalidateProperties();
				}
			}
		}
	}
}