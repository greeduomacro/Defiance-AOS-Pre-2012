using System;
using Server.Items;

namespace Server.Engines.Craft
{
	public class DefCarpentry : CraftSystem
	{
		public override SkillName MainSkill
		{
			get	{ return SkillName.Carpentry;	}
		}

		public override int GumpTitleNumber
		{
			get { return 1044004; } // <CENTER>CARPENTRY MENU</CENTER>
		}

		public static void Initialize()
		{
			m_CraftSystem = new DefCarpentry();
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefCarpentry();

				return m_CraftSystem;
			}
		}

		public override CraftECA ECA{ get{ return CraftECA.ChanceMinusSixtyToFourtyFive; } }

		public override double GetChanceAtMin( CraftItem item )
		{
			return 0.5; // 50%
		}

		private DefCarpentry() : base( 1, 1, 1.25 )// base( 1, 1, 3.0 )
		{
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
			else if ( !BaseTool.CheckAccessible( tool, from ) )
				return 1044263; // The tool must be on your person to use.

			return 0;
		}

		public override bool RetainsColorFrom( CraftItem item, Type type )
		{
			if ( item.ItemType.IsSubclassOf( typeof( BaseWeapon ) ) || item.ItemType.IsSubclassOf( typeof( BaseArmor ) ) )
				return true;

			return false;
		}

		public override void PlayCraftEffect( Mobile from )
		{
			// no animation
			//if ( from.Body.Type == BodyType.Human && !from.Mounted )
			//	from.Animate( 9, 5, 1, true, false, 0 );

			from.PlaySound( 0x23D );
		}

		public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
		{
			if ( toolBroken )
				from.SendLocalizedMessage( 1044038 ); // You have worn out your tool

			if ( failed )
			{
				if ( lostMaterial )
					return 1044043; // You failed to create the item, and some of your materials are lost.
				else
					return 1044157; // You failed to create the item, but no materials were lost.
			}
			else
			{
				if ( quality == 0 )
					return 502785; // You were barely able to make this item.  It's quality is below average.
				else if ( makersMark && quality == 2 )
					return 1044156; // You create an exceptional quality item and affix your maker's mark.
				else if ( quality == 2 )
					return 1044155; // You create an exceptional quality item.
				else
					return 1044154; // You create the item.
			}
		}

		public override void InitCraftList()
		{
			int index = -1;

			// Other Items
			if ( Core.Expansion == Expansion.AOS || Core.Expansion == Expansion.SE )
			{
				index =	AddCraft( typeof( Board ),			1044294, 1027127,	 0.0,   0.0,	typeof( Log ), 1044466,  1, 1044465 );
				SetUseAllRes( index, true );
			}

			AddCraft( typeof( BarrelStaves ),				1044294, 1027857,	00.0,  25.0,	typeof( Log ), 1044041,  5, 1044351 );
			AddCraft( typeof( BarrelLid ),					1044294, 1027608,	11.0,  36.0,	typeof( Log ), 1044041,  4, 1044351 );
			AddCraft( typeof( ShortMusicStand ),			1044294, 1044313,	78.9, 103.9,	typeof( Log ), 1044041, 15, 1044351 ); //Left

			index = AddCraft( typeof( ShortMusicStand ),	1044294, 1044314,	78.9, 103.9,	typeof( Log ), 1044041, 15, 1044351, new object[] { 0xEB8 } ); //Right
			ForceGumpItemID( index, 0xEB8 );

			AddCraft( typeof( TallMusicStand ),				1044294, 1044315,	81.5, 106.5,	typeof( Log ), 1044041, 20, 1044351 ); //Left

			index = AddCraft( typeof( TallMusicStand ),		1044294, 1044316,	81.5, 106.5,	typeof( Log ), 1044041, 20, 1044351, new object[] { 0xEBC } ); //Right
			ForceGumpItemID( index, 0xEBC );

			AddCraft( typeof( EasleWithCanvas ),			1044294, 1044317,	86.8, 111.8,	typeof( Log ), 1044041, 20, 1044351 ); //South

			index = AddCraft( typeof( EasleWithCanvas ),	1044294, 1044318,	86.8, 111.8,	typeof( Log ), 1044041, 20, 1044351, new object[] { 0xF68 } ); //East
			ForceGumpItemID( index, 0xF68 );

			index = AddCraft( typeof( EasleWithCanvas ),	1044294, 1044319,	86.8, 111.8,	typeof( Log ), 1044041, 20, 1044351, new object[] { 0xF6A } ); //North
			ForceGumpItemID( index, 0xF6A );

			if( Core.SE )
			{
				index = AddCraft( typeof( RedHangingLantern ), 1044294, 1029412, 65.0, 90.0, typeof( Log ), 1044041, 5, 1044351 );
				AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( WhiteHangingLantern ), 1044294, 1029416, 65.0, 90.0, typeof( Log ), 1044041, 5, 1044351 );
				AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( ShojiScreen ), 1044294, 1029423, 80.0, 105.0, typeof( Log ), 1044041, 75, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( BambooScreen ), 1044294, 1029428, 80.0, 105.0, typeof( Log ), 1044041, 75, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

			if( Core.AOS )	//Duplicate Entries to preserve ordering depending on era
			{
				index = AddCraft( typeof( FishingPole ), 1044294, 1023519, 68.4, 93.4, typeof( Log ), 1044041, 5, 1044351 ); //This is in the category of Other during AoS
				AddSkill( index, SkillName.Tailoring, 40.0, 45.0 );
				AddRes( index, typeof( Cloth ), 1044286, 5, 1044287 );
			}

			if( Core.ML )
			{
				index = AddCraft( typeof( ArcanistStatueSouthDeed ), 1044294, 1072885, 0.0, 35.0, typeof( Log ), 1044041, 250, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ArcanistStatueEastDeed ), 1044294, 1072886, 0.0, 35.0, typeof( Log ), 1044041, 250, 1044351 );
				SetNeededExpansion( index, Expansion.ML );
			}

			// Furniture
			AddCraft( typeof( FootStool ),					1044291, 1022910,	11.0,  36.0,	typeof( Log ), 1044041,  9, 1044351 );
			AddCraft( typeof( Stool ),						1044291, 1022602,	11.0,  36.0,	typeof( Log ), 1044041,  9, 1044351 );
			AddCraft( typeof( BambooChair ),				1044291, 1044300,	21.0,  46.0,	typeof( Log ), 1044041, 13, 1044351 );
			AddCraft( typeof( WoodenChair ),				1044291, 1044301,	21.0,  46.0,	typeof( Log ), 1044041, 13, 1044351 );
			AddCraft( typeof( FancyWoodenChairCushion ),	1044291, 1044302,	42.1,  67.1,	typeof( Log ), 1044041, 15, 1044351 );
			AddCraft( typeof( WoodenChairCushion ),			1044291, 1044303,	42.1,  67.1,	typeof( Log ), 1044041, 13, 1044351 );
			AddCraft( typeof( WoodenBench ),				1044291, 1022860,	52.6,  77.6,	typeof( Log ), 1044041, 17, 1044351 );
			AddCraft( typeof( WoodenThrone ),				1044291, 1044304,	52.6,  77.6,	typeof( Log ), 1044041, 17, 1044351 );
			AddCraft( typeof( Throne ),						1044291, 1044305,	73.6,  98.6,	typeof( Log ), 1044041, 19, 1044351 );
			AddCraft( typeof( Nightstand ),					1044291, 1044306,	42.1,  67.1,	typeof( Log ), 1044041, 17, 1044351 );
			AddCraft( typeof( WritingTable ),				1044291, 1022890,	63.1,  88.1,	typeof( Log ), 1044041, 17, 1044351 );
			AddCraft( typeof( YewWoodTable ),				1044291, 1044307,	63.1,  88.1,	typeof( Log ), 1044041, 23, 1044351 );
			AddCraft( typeof( LargeTable ),					1044291, 1044308,	84.2, 109.2,	typeof( Log ), 1044041, 27, 1044351 );

			if( Core.SE )
			{
				index = AddCraft( typeof( ElegantLowTable ),	1044291, 1030265,	80.0, 105.0,	typeof( Log ), 1044041, 35, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( PlainLowTable ),		1044291, 1030266,	80.0, 105.0,	typeof( Log ), 1044041, 35, 1044351 );
				SetNeededExpansion( index, Expansion.SE );
			}

			if( Core.ML )
			{
				index = AddCraft( typeof( OrnateElvenTableSouthDeed ),	1044291, 1072869,	85.0, 110.0,	typeof( Log ), 1044041, 60, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( OrnateElvenTableEastDeed ),	1044291, 1073384,	85.0, 110.0,	typeof( Log ), 1044041, 60, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( FancyElvenTableSouthDeed ),	1044291, 1073385,	80.0, 105.0,	typeof( Log ), 1044041, 50, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( FancyElvenTableEastDeed ),	1044291, 1073386,	80.0, 105.0,	typeof( Log ), 1044041, 50, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ElvenBookStand ),				1044291, 1031741,	80.0, 105.0,	typeof( Log ), 1044041, 20, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( BigElvenChair ),				1044291, 1031755,	85.0, 110.0,	typeof( Log ), 1044041, 40, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ElvenReadingChair ),			1044291, 1072160,	80.0, 105.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.ML );
			}

			// Containers
			AddCraft( typeof( WoodenBox ),					1044292, 1023709,	21.0,  46.0,	typeof( Log ), 1044041, 10, 1044351 );
			AddCraft( typeof( SmallCrate ),					1044292, 1044309,	10.0,  35.0,	typeof( Log ), 1044041, 8 , 1044351 );
			AddCraft( typeof( MediumCrate ),				1044292, 1044310,	31.0,  56.0,	typeof( Log ), 1044041, 15, 1044351 );
			AddCraft( typeof( LargeCrate ),					1044292, 1044311,	47.3,  72.3,	typeof( Log ), 1044041, 18, 1044351 );
			AddCraft( typeof( WoodenChest ),				1044292, 1023650,	73.6,  98.6,	typeof( Log ), 1044041, 20, 1044351 );
			AddCraft( typeof( EmptyBookcase ),				1044292, 1022718,	31.5,  56.5,	typeof( Log ), 1044041, 25, 1044351 );
			AddCraft( typeof( FancyArmoire ),				1044292, 1044312,	84.2, 109.2,	typeof( Log ), 1044041, 35, 1044351 );
			AddCraft( typeof( Armoire ),					1044292, 1022643,	84.2, 109.2,	typeof( Log ), 1044041, 35, 1044351 );

			if( Core.SE )
			{
				index = AddCraft( typeof( PlainWoodenChest ),	1044292, 1030251, 90.0, 115.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( OrnateWoodenChest ),	1044292, 1030253, 90.0, 115.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( GildedWoodenChest ),	1044292, 1030255, 90.0, 115.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( WoodenFootLocker ),	1044292, 1030257, 90.0, 115.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( FinishedWoodenChest ),1044292, 1030259, 90.0, 115.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( TallCabinet ),	1044292, 1030261, 90.0, 115.0,	typeof( Log ), 1044041, 35, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( ShortCabinet ),	1044292, 1030263, 90.0, 115.0,	typeof( Log ), 1044041, 35, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( RedArmoire ),	1044292, 1030328, 90.0, 115.0,	typeof( Log ), 1044041, 40, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( ElegantArmoire ),	1044292, 1030330, 90.0, 115.0,	typeof( Log ), 1044041, 40, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( MapleArmoire ),	1044292, 1030332, 90.0, 115.0,	typeof( Log ), 1044041, 40, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( CherryArmoire ),	1044292, 1030334, 90.0, 115.0,	typeof( Log ), 1044041, 40, 1044351 );
				SetNeededExpansion( index, Expansion.SE );
			}

			index = AddCraft( typeof( Keg ), 1044292, 1023711, 57.8, 82.8, typeof( BarrelStaves ), 1044288, 3, 1044253 );
			AddRes( index, typeof( BarrelHoops ), 1044289, 1, 1044253 );
			AddRes( index, typeof( BarrelLid ), 1044251, 1, 1044253 );
			ForceNonExceptional( index );

			if ( Core.ML )
			{
				index = AddCraft( typeof( ElvenWashBasinSouthDeed ),	1044292, 1072865, 70.0, 95.0,	typeof( Log ), 1044041, 40, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ElvenWashBasinEastDeed ),	1044292, 1073387, 70.0, 95.0,	typeof( Log ), 1044041, 40, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( OrnateElvenChest ),	1044292, 1031761, 80.0, 105.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( OrnateElvenBox ),	1044292, 1031763, 80.0, 105.0,	typeof( Log ), 1044041, 25, 1044351 );
				SetNeededExpansion( index, Expansion.ML );
			}

			// Staves and Shields
			int group;
			if ( Core.ML )
				group = 1044566; // Armor and Weapons are seperated post ML
			else
				group = 1044295;

			AddCraft( typeof( ShepherdsCrook ), group, 1023713, 78.9, 103.9, typeof( Log ), 1044041, 7, 1044351 );
			AddCraft( typeof( QuarterStaff ), group, 1023721, 73.6, 98.6, typeof( Log ), 1044041, 6, 1044351 );
			AddCraft( typeof( GnarledStaff ), group, 1025112, 78.9, 103.9, typeof( Log ), 1044041, 7, 1044351 );
			AddCraft( typeof( WoodenShield ), ( Core.ML ? 1062760 : 1044295 ), 1027034, 52.6, 77.6, typeof( Log ), 1044041, 9, 1044351 );

			if( !Core.AOS )	//Duplicate Entries to preserve ordering depending on era
			{
				index = AddCraft( typeof( FishingPole ), 1044295, 1023519, 68.4, 93.4, typeof( Log ), 1044041, 5, 1044351 ); //This is in the categor of Other during AoS
				AddSkill( index, SkillName.Tailoring, 40.0, 45.0 );
				AddRes( index, typeof( Cloth ), 1044286, 5, 1044287 );
			}

			if( Core.SE )
			{
				index = AddCraft( typeof( Bokuto ), group, 1030227, 70.0, 95.0, typeof( Log ), 1044041, 6, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( Fukiya ), group, 1030229, 60.0, 85.0, typeof( Log ), 1044041, 6, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( Tetsubo ), group, 1030225, 85.0, 110.0, typeof( Log ), 1044041, 8, 1044351 );
				AddSkill( index, SkillName.Tinkering, 40.0, 45.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 5, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
			}

			if ( Core.ML )
			{
				index = AddCraft( typeof( WildStaff ), 1044566, 1031557, 63.8, 144.6, typeof( Log ), 1044041, 16, 1044351 );
				SetNeededExpansion( index, Expansion.ML );
			}

			// Instruments
			index = AddCraft( typeof( LapHarp ), 1044293, 1023762, 63.1, 88.1, typeof( Log ), 1044041, 20, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );

			index = AddCraft( typeof( Harp ), 1044293, 1023761, 78.9, 103.9, typeof( Log ), 1044041, 35, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 15, 1044287 );

			index = AddCraft( typeof( Drums ), 1044293, 1023740, 57.8, 82.8, typeof( Log ), 1044041, 20, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );

			index = AddCraft( typeof( Lute ), 1044293, 1023763, 68.4, 93.4, typeof( Log ), 1044041, 25, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );

			index = AddCraft( typeof( Tambourine ), 1044293, 1023741, 57.8, 82.8, typeof( Log ), 1044041, 15, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );

			index = AddCraft( typeof( TambourineTassel ), 1044293, 1044320, 57.8, 82.8, typeof( Log ), 1044041, 15, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 15, 1044287 );

			if( Core.SE )
			{
				index = AddCraft( typeof( BambooFlute ), 1044293, 1030247, 80.0, 105.0, typeof( Log ), 1044041, 15, 1044351 );
				AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
				SetNeededExpansion( index, Expansion.SE );
			}

			// Misc
			if ( Core.ML )
			{
				index = AddCraft( typeof( PlayerBBEast ), 1044290, 1062420, 85.0, 110.0, typeof( Log ), 1044041, 50, 1044351 );

				index = AddCraft( typeof( PlayerBBSouth ), 1044290, 1062421, 85.0, 110.0, typeof( Log ), 1044041, 50, 1044351 );

				index = AddCraft( typeof( ParrotPerchDeed ),			1044290, 1032214, 50.0, 85.0, typeof( Log ), 1044041, 100, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ArcaneCircleDeed ),			1044290, 1032411, 94.7, 119.6, typeof( Log ), 1044041, 100, 1044351 );
				AddRes( index, typeof( BlueDiamond ), 1032696, 2, 1044253 );
				AddRes( index, typeof( PerfectEmerald ), 1032692, 2, 1044253 );
				AddRes( index, typeof( FireRuby ), 1032695, 2, 1044253 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( TallElvenBedSouthDeed ),		1044290, 1072858, 94.7, 119.6, typeof( Log ), 1044041, 200, 1044351 );
				AddRes( index, typeof( Cloth ), 1044286, 100, 1044287 );
				AddRecipe( index, 116 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( TallElvenBedEastDeed ),		1044290, 1072859, 94.7, 119.6, typeof( Log ), 1044041, 200, 1044351 );
				AddRes( index, typeof( Cloth ), 1044286, 100, 1044287 );
				AddRecipe( index, 117 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ElvenBedSouthDeed ),			1044290, 1072860, 94.7, 119.6, typeof( Log ), 1044041, 100, 1044351 );
				AddRes( index, typeof( Cloth ), 1044286, 100, 1044287 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ElvenBedEastDeed ),			1044290, 1072861, 94.7, 119.6, typeof( Log ), 1044041, 100, 1044351 );
				AddRes( index, typeof( Cloth ), 1044286, 100, 1044287 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ElvenLoveseatSouthDeed ),		1044290, 1072867, 80.0, 105.0, typeof( Log ), 1044041, 50, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ElvenLoveseatEastDeed ),		1044290, 1073372, 80.0, 105.0, typeof( Log ), 1044041, 50, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( AlchemistTableSouthDeed ),	1044290, 1073396, 85.0, 110.0, typeof( Log ), 1044041, 70, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( AlchemistTableEastDeed ),		1044290, 1073397, 85.0, 110.0, typeof( Log ), 1044041, 70, 1044351 );
				SetNeededExpansion( index, Expansion.ML );
			}

			index = AddCraft( typeof( SmallBedSouthDeed ), 1044290, 1044321, 94.7, 119.6, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Tailoring, 75.0, 80.0 );
			AddRes( index, typeof( Cloth ), 1044286, 100, 1044287 );
			index = AddCraft( typeof( SmallBedEastDeed ), 1044290, 1044322, 94.7, 119.6, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Tailoring, 75.0, 80.0 );
			AddRes( index, typeof( Cloth ), 1044286, 100, 1044287 );
			index = AddCraft( typeof( LargeBedSouthDeed ), 1044290,1044323, 94.7, 119.6, typeof( Log ), 1044041, 150, 1044351 );
			AddSkill( index, SkillName.Tailoring, 75.0, 80.0 );
			AddRes( index, typeof( Cloth ), 1044286, 150, 1044287 );
			index = AddCraft( typeof( LargeBedEastDeed ), 1044290, 1044324, 94.7, 119.6, typeof( Log ), 1044041, 150, 1044351 );
			AddSkill( index, SkillName.Tailoring, 75.0, 80.0 );
			AddRes( index, typeof( Cloth ), 1044286, 150, 1044287 );
			AddCraft( typeof( DartBoardSouthDeed ), 1044290, 1044325, 15.7, 40.7, typeof( Log ), 1044041, 5, 1044351 );
			AddCraft( typeof( DartBoardEastDeed ), 1044290, 1044326, 15.7, 40.7, typeof( Log ), 1044041, 5, 1044351 );
			AddCraft( typeof( BallotBoxDeed ), 1044290, 1044327, 47.3, 72.3, typeof( Log ), 1044041, 5, 1044351 );
			index = AddCraft( typeof( PentagramDeed ), 1044290, 1044328, 100.0, 125.0, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Magery, 75.0, 80.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 40, 1044037 );
			index = AddCraft( typeof( AbbatoirDeed ), 1044290, 1044329, 100.0, 125.0, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Magery, 50.0, 55.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 40, 1044037 );

			if ( Core.Expansion == Expansion.AOS || Core.Expansion == Expansion.SE ) //Duplicate Entries to preserve ordering depending on era
			{
				AddCraft( typeof( PlayerBBEast ), 1044290, 1062420, 85.0, 110.0, typeof( Log ), 1044041, 50, 1044351 );
				AddCraft( typeof( PlayerBBSouth ), 1044290, 1062421, 85.0, 110.0, typeof( Log ), 1044041, 50, 1044351 );
			}

			if ( !Core.ML ) //Duplicate Entries to preserve ordering depending on era
			{
				// Blacksmithy
				index = AddCraft( typeof( SmallForgeDeed ), 1044296, 1044330, 73.6, 98.6, typeof( Log ), 1044041, 5, 1044351 );
				AddSkill( index, SkillName.Blacksmith, 75.0, 80.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 75, 1044037 );
				index = AddCraft( typeof( LargeForgeEastDeed ), 1044296, 1044331, 78.9, 103.9, typeof( Log ), 1044041, 5, 1044351 );
				AddSkill( index, SkillName.Blacksmith, 80.0, 85.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 100, 1044037 );
				index = AddCraft( typeof( LargeForgeSouthDeed ), 1044296, 1044332, 78.9, 103.9, typeof( Log ), 1044041, 5, 1044351 );
				AddSkill( index, SkillName.Blacksmith, 80.0, 85.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 100, 1044037 );
				index = AddCraft( typeof( AnvilEastDeed ), 1044296, 1044333, 73.6, 98.6, typeof( Log ), 1044041, 5, 1044351 );
				AddSkill( index, SkillName.Blacksmith, 75.0, 80.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 150, 1044037 );
				index = AddCraft( typeof( AnvilSouthDeed ), 1044296, 1044334, 73.6, 98.6, typeof( Log ), 1044041, 5, 1044351 );
				AddSkill( index, SkillName.Blacksmith, 75.0, 80.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 150, 1044037 );

				// Training
				index = AddCraft( typeof( TrainingDummyEastDeed ), 1044297, 1044335, 68.4, 93.4, typeof( Log ), 1044041, 55, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
				index = AddCraft( typeof( TrainingDummySouthDeed ), 1044297, 1044336, 68.4, 93.4, typeof( Log ), 1044041, 55, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
				index = AddCraft( typeof( PickpocketDipEastDeed ), 1044297, 1044337, 73.6, 98.6, typeof( Log ), 1044041, 65, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
				index = AddCraft( typeof( PickpocketDipSouthDeed ), 1044297, 1044338, 73.6, 98.6, typeof( Log ), 1044041, 65, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
			}

			// Tailoring and Cooking
			index = AddCraft( typeof( Dressform ), 1044298, 1044339, 63.1, 88.1, typeof( Log ), 1044041, 25, 1044351 ); //Front
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );
			index = AddCraft( typeof( Dressform ), 1044298, 1044340, 63.1, 88.1, typeof( Log ), 1044041, 25, 1044351, new object[] { 0xEC7 } ); //Side
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );
			ForceGumpItemID( index, 0xEC7 );

			if ( Core.ML )
			{
				index = AddCraft( typeof( ElvenSpinningwheelEastDeed ), 1044298, 1073393, 75.0, 100, typeof( Log ), 1044041, 60, 1044351 );
				AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
				AddRes( index, typeof( Cloth ), 1044286, 40, 1044287 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ElvenSpinningwheelSouthDeed ), 1044298, 1072878, 75.0, 100, typeof( Log ), 1044041, 60, 1044351 );
				AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
				AddRes( index, typeof( Cloth ), 1044286, 40, 1044287 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ElvenStoveSouthDeed ), 1044298, 1073394, 85.0, 110.0, typeof( Log ), 1044041, 80, 1044351 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ElvenStoveEastDeed ), 1044298, 1073395, 85.0, 110.0, typeof( Log ), 1044041, 80, 1044351 );
				SetNeededExpansion( index, Expansion.ML );
			}

			index = AddCraft( typeof( SpinningwheelEastDeed ), 1044298, 1044341, 73.6, 98.6, typeof( Log ), 1044041, 75, 1044351 );
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddRes( index, typeof( Cloth ), 1044286, 25, 1044287 );
			index = AddCraft( typeof( SpinningwheelSouthDeed ), 1044298, 1044342, 73.6, 98.6, typeof( Log ), 1044041, 75, 1044351 );
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddRes( index, typeof( Cloth ), 1044286, 25, 1044287 );
			index = AddCraft( typeof( LoomEastDeed ), 1044298, 1044343, 84.2, 109.2, typeof( Log ), 1044041, 85, 1044351 );
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddRes( index, typeof( Cloth ), 1044286, 25, 1044287 );
			index = AddCraft( typeof( LoomSouthDeed ), 1044298, 1044344, 84.2, 109.2, typeof( Log ), 1044041, 85, 1044351 );
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddRes( index, typeof( Cloth ), 1044286, 25, 1044287 );

			//Cooking
			if ( Core.ML )
				group = 1044298; // These are merged into the Tailoring and Cooking definition after ML
			else
				group = 1044299;

			index = AddCraft( typeof( StoneOvenEastDeed ), group, 1044345, 68.4, 93.4, typeof( Log ), 1044041, 85, 1044351 );
			AddSkill( index, SkillName.Tinkering, 50.0, 55.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 125, 1044037 );
			index = AddCraft( typeof( StoneOvenSouthDeed ), group, 1044346, 68.4, 93.4, typeof( Log ), 1044041, 85, 1044351 );
			AddSkill( index, SkillName.Tinkering, 50.0, 55.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 125, 1044037 );
			index = AddCraft( typeof( FlourMillEastDeed ), group, 1044347, 94.7, 119.7, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Tinkering, 50.0, 55.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 50, 1044037 );
			index = AddCraft( typeof( FlourMillSouthDeed ), group, 1044348, 94.7, 119.7, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Tinkering, 50.0, 55.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 50, 1044037 );
			AddCraft( typeof( WaterTroughEastDeed ), group, 1044349, 94.7, 119.7, typeof( Log ), 1044041, 150, 1044351 );
			AddCraft( typeof( WaterTroughSouthDeed ), group, 1044350, 94.7, 119.7, typeof( Log ), 1044041, 150, 1044351 );

			if ( Core.ML ) //Duplicate Entries to preserve ordering depending on era
			{
				// Blacksmithy
				index = AddCraft( typeof( ElvenForgeDeed ), 1044296, 1031736, 94.7, 119.7, typeof( Log ), 1044041, 200, 1044351 );
				SetNeededExpansion( index, Expansion.ML );
				index = AddCraft( typeof( SmallForgeDeed ), 1044296, 1044330, 73.6, 98.6, typeof( Log ), 1044041, 5, 1044351 );
				AddSkill( index, SkillName.Blacksmith, 75.0, 80.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 75, 1044037 );
				index = AddCraft( typeof( LargeForgeEastDeed ), 1044296, 1044331, 78.9, 103.9, typeof( Log ), 1044041, 5, 1044351 );
				AddSkill( index, SkillName.Blacksmith, 80.0, 85.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 100, 1044037 );
				index = AddCraft( typeof( LargeForgeSouthDeed ), 1044296, 1044332, 78.9, 103.9, typeof( Log ), 1044041, 5, 1044351 );
				AddSkill( index, SkillName.Blacksmith, 80.0, 85.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 100, 1044037 );
				index = AddCraft( typeof( AnvilEastDeed ), 1044296, 1044333, 73.6, 98.6, typeof( Log ), 1044041, 5, 1044351 );
				AddSkill( index, SkillName.Blacksmith, 75.0, 80.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 150, 1044037 );
				index = AddCraft( typeof( AnvilSouthDeed ), 1044296, 1044334, 73.6, 98.6, typeof( Log ), 1044041, 5, 1044351 );
				AddSkill( index, SkillName.Blacksmith, 75.0, 80.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 150, 1044037 );

				// Training
				index = AddCraft( typeof( TrainingDummyEastDeed ), 1044297, 1044335, 68.4, 93.4, typeof( Log ), 1044041, 55, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
				index = AddCraft( typeof( TrainingDummySouthDeed ), 1044297, 1044336, 68.4, 93.4, typeof( Log ), 1044041, 55, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
				index = AddCraft( typeof( PickpocketDipEastDeed ), 1044297, 1044337, 73.6, 98.6, typeof( Log ), 1044041, 65, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
				index = AddCraft( typeof( PickpocketDipSouthDeed ), 1044297, 1044338, 73.6, 98.6, typeof( Log ), 1044041, 65, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
			}

			MarkOption = true;
			Repair = Core.AOS;
			CanEnhance = Core.ML;

			if ( Core.ML )
			{
				SetSubRes( typeof( Log ), 1072643 );

				// Add every material you want the player to be able to choose from
				// This will override the overridable material	TODO: Verify the required skill amount
				AddSubRes( typeof( Log ), 1072643, 00.0, 1044041, 1072652 );
				AddSubRes( typeof( OakLog ), 1072644, 65.0, 1044041, 1072652 );
				AddSubRes( typeof( AshLog ), 1072645, 80.0, 1044041, 1072652 );
				AddSubRes( typeof( YewLog ), 1072646, 95.0, 1044041, 1072652 );
				AddSubRes( typeof( HeartwoodLog ), 1072647, 100.0, 1044041, 1072652 );
				AddSubRes( typeof( BloodwoodLog ), 1072648, 100.0, 1044041, 1072652 );
				AddSubRes( typeof( FrostwoodLog ), 1072649, 100.0, 1044041, 1072652 );
			}
		}
	}
}