using System;
using Server;
using Server.Items;
using Server.Regions;

namespace Server.Mobiles
{
	public class Plague
	{
		public static double Chance = .05; // Chance that a newly spawned creature will be a plagued
		public static double ChestChance = .10; // Chance that a plagued will carry a plagued chest
		public static bool Enabled = true;
		public static Map[] Maps = new Map[]
		{
			Map.Felucca,
		};

		public static int	Hue   = 0x558;

		// Buffs
		public static double HitsBuff   = 2.0;
		public static double StrBuff	= 1.05;
		public static double IntBuff	= 1.10;
		public static double DexBuff	= 1.10;
		public static double SkillsBuff = 1.10;
		public static double SpeedBuff  = 1.10;
		public static double FameBuff   = 1.20;
		public static double KarmaBuff  = 1.20;
		public static int	DamageBuff = 5;

		public static void Convert( BaseCreature bc )
		{
			if ( bc.IsPlagued )
				return;

			bc.Hue = Hue;

			bc.HitsMaxSeed = (int)( bc.HitsMaxSeed * HitsBuff );
			bc.Hits = bc.HitsMax;

			bc.RawStr = (int)( bc.RawStr * StrBuff );
			bc.RawInt = (int)( bc.RawInt * IntBuff );
			bc.RawDex = (int)( bc.RawDex * DexBuff );

			for( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = (Skill)bc.Skills[i];
				if ( skill.Base == 0.0 )
					continue;
				else
					skill.Base *= SkillsBuff;
			}

			bc.PassiveSpeed /= SpeedBuff;
			bc.ActiveSpeed /= SpeedBuff;

			bc.DamageMin += DamageBuff;
			bc.DamageMax += DamageBuff;

			if ( bc.Fame > 0 )
				bc.Fame = (int)( bc.Fame * FameBuff );
			if ( bc.Karma != 0 )
				bc.Karma = (int)( bc.Karma * KarmaBuff );
		}

		public static void UnConvert( BaseCreature bc )
		{
			if ( !bc.IsPlagued )
				return;

			bc.Hue = 0;

			bc.HitsMaxSeed = (int)( bc.HitsMaxSeed / HitsBuff );
			bc.Hits = bc.HitsMax;

			bc.RawStr = (int)( bc.RawStr / StrBuff );
			bc.RawInt = (int)( bc.RawInt / IntBuff );
			bc.RawDex = (int)( bc.RawDex / DexBuff );

			for( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = (Skill)bc.Skills[i];
				if ( skill.Base == 0.0 )
					continue;
				else
					skill.Base /= SkillsBuff;
			}

			bc.PassiveSpeed *= SpeedBuff;
			bc.ActiveSpeed *= SpeedBuff;

			bc.DamageMin -= DamageBuff;
			bc.DamageMax -= DamageBuff;

			if ( bc.Fame > 0 )
				bc.Fame = (int)( bc.Fame / FameBuff );
			if ( bc.Karma != 0 )
				bc.Karma = (int)( bc.Karma / KarmaBuff );
		}

		public static bool CheckConvert( BaseCreature bc )
		{
			return CheckConvert( bc, bc.Location, bc.Map );
		}

		public static bool CheckConvert( BaseCreature bc, Point3D location, Map m )
		{
			if( !Enabled )
				return false;

			if ( Array.IndexOf( Maps, m ) == -1 )
				return false;

			if ( bc is BaseChampion || bc is Harrower || bc is BaseVendor )
				return false;

			if (m != Map.Felucca)
				return false;

			if (!(Region.Find(location, m) is DungeonRegion))
				return false;

			return ( Chance > Utility.RandomDouble() );
		}
	}
}