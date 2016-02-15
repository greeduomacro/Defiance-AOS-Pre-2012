using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Engines.Quests.ElderWizard;

namespace Server.Mobiles
{
	public class Necromancer : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public Necromancer() : base( "the necromancer" )
		{
			SetSkill( SkillName.EvalInt, 80.0, 100.0 );
			SetSkill( SkillName.Inscribe, 80.0, 100.0 );
			SetSkill( SkillName.Necromancy, 80.0, 100.0 );
			SetSkill( SkillName.Meditation, 80.0, 100.0 );
			SetSkill( SkillName.MagicResist, 80.0, 100.0 );

			Hue = 0x3C6;
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (GabrielleTheElderWizard.RefillBag(this, from, dropped))
				return false;

			return base.OnDragDrop(from, dropped);
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBNecromancer() );
		}

		public override VendorShoeType ShoeType
		{
			get { return Utility.RandomBool() ? VendorShoeType.Shoes : VendorShoeType.Sandals; }
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem(new Server.Items.Robe(0x455));
		}

		public Necromancer( Serial serial ) : base( serial )
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