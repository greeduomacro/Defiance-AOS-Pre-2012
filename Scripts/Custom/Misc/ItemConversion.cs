using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server
{
	public class ItemConversion
	{
		public static void Configure()
		{
			m_DonationConvert = new List<Item>();
			m_DonationAOSConvert = new List<DonationSkillBallAOS>();
			m_RareConvert = new List<Item>();

//			EventSink.WorldLoad += new WorldLoadEventHandler( PurgeList );
//			EventSink.WorldSave += new WorldSaveEventHandler( Save );
		}

		private static List<Item> m_DonationConvert;
		private static List<DonationSkillBallAOS> m_DonationAOSConvert;
		private static List<Item> m_RareConvert;

		private static void MoveItem( Item src, Item target )
		{
			Container pack = null;
			if ( src.Parent is Container )
				pack = (Container)src.Parent;
			else if ( src.Parent is Mobile )
				pack = ((Mobile)src.Parent).Backpack;

			if ( pack != null )
				pack.DropItem( target );
			else
				target.MoveToWorld( src.Location, src.Map );
		}

		public static void PurgeList()
		{
			if ( m_DonationAOSConvert != null && m_DonationAOSConvert.Count > 0 )
			{
				for ( int i = 0; i < m_DonationAOSConvert.Count; ++i )
				{
					DonationSkillBallAOS ball = (DonationSkillBallAOS)m_DonationAOSConvert[i];
					SkillBall copy = new DonationSkillBall( ball.SkillBonus );

					MoveItem( ball, copy );
					copy.IsLockedDown = ball.IsLockedDown;
					copy.IsSecure = ball.IsSecure;
					ball.Delete();
				}
			}

			if ( m_DonationConvert != null && m_DonationConvert.Count > 0 )
			{
				for ( int i = 0; i < m_DonationConvert.Count; ++i )
				{
					SkillBall ball = (SkillBall)m_DonationConvert[i];
					SkillBall copy = new DonationSkillBall( ball.SkillBonus );

					copy.Flags = ball.Flags;
					copy.ExpireDate = ball.ExpireDate;
					copy.OwnerPlayer = ball.OwnerPlayer;
					copy.OwnerAccount = ball.OwnerAccount;

					MoveItem( ball, copy );
					copy.IsLockedDown = ball.IsLockedDown;
					copy.IsSecure = ball.IsSecure;

					ball.Delete();
				}
			}

			if ( m_RareConvert != null && m_RareConvert.Count > 0 )
			{
				for ( int i = 0; i < m_RareConvert.Count; ++i )
				{
					Item src = m_RareConvert[i];
					Type newtype = null;
					for ( int j = 0; j < m_OldTypes.Length; j++ )
					{
						if ( src.GetType() == m_OldTypes[j] )
						{
							newtype = m_NewTypes[j];
							break;
						}
					}
					if ( newtype != null )
					{
						Item item = (Item)Activator.CreateInstance( newtype );

						MoveItem( src, item );
						item.IsLockedDown = src.IsLockedDown;
						item.IsSecure = src.IsSecure;

						PlayerVendor pv = src.RootParent as PlayerVendor;

						if ( pv == null )
							return;

						VendorItem vi = pv.GetVendorItem( src );

						if ( vi != null )
						{
							pv.SetVendorItem( item, vi.Price, vi.Description, vi.Created );
							pv.RemoveVendorItem( src );
						}

						src.Delete();
					}
				}
			}
		}

		private static Type[] m_OldTypes = new Type[]
			{
				typeof( SRDRBloodyWater ), typeof( SRDRCocoon ), typeof( SRDRDamagedBooks ),
				typeof( SRDRRuinedBooks ), typeof( SRDRBottle ), typeof( SRDRBrazier ),
				typeof( SRDRLampPost ), typeof( SRDRRuinedPainting ), typeof( SRDRSaddle ),
				typeof( SRDRTarot ), typeof( SRDREggCase ), typeof( SRDRGruesomeStandard ),
				typeof( SRDRHangingLeatherTunic ), typeof( SRDRHangingStuddedLeggings ), typeof( SRDRHangingStuddedTunic ),
				typeof( SRDRRock ), typeof( SRDRSkinnedDeer ), typeof( SRDRSkinnedGoat ),
				typeof( SRDRSkullCandle ), typeof( SRDRStackedBooks2 ), typeof( SRDRStackedBooks ),
				typeof( SRDRStretchedHide ), typeof( SRDRReversedBackPack ), typeof( SRDriedOnions ),
				typeof( SRDriedHerbs ), typeof( SRDriedFlowers ), typeof( SRHorseShoes ),
				typeof( SREIronWire ), typeof( SRESilverWire ), typeof( SREGoldWire ),
				typeof( SRECopperWire ), typeof( SRArrowShafts )
			};

		private static Type[] m_NewTypes = new Type[]
			{
				typeof( BloodyWaterArtifact ), typeof( CocoonArtifact ), typeof( DamagedBooksArtifact ),
				typeof( BooksFaceDownArtifact ), typeof( BottleArtifact ), typeof( BrazierArtifact ),
				typeof( LampPostArtifact ), typeof( RuinedPaintingArtifact ), typeof( SaddleArtifact ),
				typeof( TarotCardsArtifact ), typeof( EggCaseArtifact ), typeof( GruesomeStandardArtifact ),
				typeof( LeatherTunicArtifact ), typeof( StuddedLeggingsArtifact ), typeof( StuddedTunicArtifact ),
				typeof( RockArtifact ), typeof( SkinnedDeerArtifact ), typeof( SkinnedGoatArtifact ),
				typeof( SkullCandleArtifact ), typeof( BooksNorthArtifact ), typeof( BooksWestArtifact ),
				typeof( StretchedHideArtifact ), typeof( BackpackArtifact ), typeof( DriedOnions ),
				typeof( DriedHerbs ), typeof( GreenDriedFlowers ), typeof( HorseShoes ),
				typeof( IronWire ), typeof( SilverWire ), typeof( GoldWire ),
				typeof( CopperWire ), typeof( ArrowShafts )
			};

		public static void Save( WorldSaveEventArgs e )
		{
			PurgeList();
		}

		public static void AddToDonationConversion( Item item )
		{
			if ( m_DonationConvert == null )
				m_DonationConvert = new List<Item>();

			m_DonationConvert.Add( item );
		}

		public static void AddToRareConversion( Item item )
		{
			if ( m_RareConvert == null )
				m_RareConvert = new List<Item>();

			m_RareConvert.Add( item );
		}

		public static void AddToDonationAOSConversion( DonationSkillBallAOS item )
		{
			if ( m_DonationAOSConvert == null )
				m_DonationAOSConvert = new List<DonationSkillBallAOS>();

			m_DonationAOSConvert.Add( item );
		}
	}
}