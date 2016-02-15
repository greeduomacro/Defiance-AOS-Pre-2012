using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.ContextMenus;
using Server.Commands;
using Server.Engines.Lottery;

namespace Server.Mobiles
{
	public class LotteryNpc : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public LotteryNpc() : base( "the lottery master" )
		{
			SetSkill( SkillName.Inscribe, 90.0, 100.0 );

			LotterySystem.m_NpcRegister.Add(this);
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBLotteryNpc() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return VendorShoeType.Shoes; }
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Robe( Utility.RandomNeutralHue() ) );
			AddItem( new JesterHat( Utility.RandomNeutralHue() ) );
		}

		public LotteryNpc( Serial serial ) : base( serial )
		{
		}

		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.AddCustomContextEntries( from, list );

			if (from.Alive && from is PlayerMobile)
				list.Add( new TalkEntry( this ) );
		}

		public class TalkEntry : ContextMenuEntry
		{
			private LotteryNpc m_LotteryNpc;

			public TalkEntry( LotteryNpc lotteryNpc ) : base( 6146 ) // Talk
			{
				m_LotteryNpc = lotteryNpc;
			}

			public override void OnClick()
			{
				if( !(Owner.From is PlayerMobile) )
					return;

				LotteryEntry entry = LotterySystem.GetPlayerEntry(Owner.From);

				if (!LotterySystem.TryToShowWinInfo(Owner.From, entry))
					Owner.From.SendGump( new LotteryGump( Owner.From, "" ) );
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

			LotterySystem.m_NpcRegister.Add(this);
		}
	}
}