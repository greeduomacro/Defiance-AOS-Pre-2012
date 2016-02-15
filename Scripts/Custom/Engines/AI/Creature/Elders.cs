using System;
using Server;
using Server.Items;
using Server.Regions;

namespace Server.Mobiles
{
	public class Elders
	{
		public static double Chance      = .05;
		public static Map[] Maps         = new Map[]
		{
			Map.Felucca,
		};

		public static int    Hue   = 0x1B6;

		// Buffs
		public static double HitsBuff   = 5.0;
		public static double StrBuff    = 1.05;
		public static double IntBuff    = 1.20;
		public static double DexBuff    = 1.20;
		public static double SkillsBuff = 1.20;
		public static double SpeedBuff  = 1.20;
		public static double FameBuff   = 1.40;
		public static double KarmaBuff  = 1.40;
		public static int    DamageBuff = 5;

		public static void Convert( BaseCreature bc )
		{
			if ( bc.IsElder )
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
			if ( !bc.IsElder )
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
			return false;
		}
	}
}