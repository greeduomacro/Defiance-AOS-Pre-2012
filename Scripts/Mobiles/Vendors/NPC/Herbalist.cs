using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Engines.Quests.ElderWizard;

namespace Server.Mobiles
{
	public class Herbalist : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.MagesGuild; } }

		[Constructable]
		public Herbalist() : base( "the herbalist" )
		{
			SetSkill( SkillName.Alchemy, 80.0, 100.0 );
			SetSkill( SkillName.Cooking, 80.0, 100.0 );
			SetSkill( SkillName.TasteID, 80.0, 100.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBHerbalist() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return Utility.RandomBool() ? VendorShoeType.Shoes : VendorShoeType.Sandals; }
		}

		public Herbalist( Serial serial ) : base( serial )
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

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (GabrielleTheElderWizard.RefillBag(this, from, dropped))
				return false;

			return base.OnDragDrop(from, dropped);
		}
	}
}