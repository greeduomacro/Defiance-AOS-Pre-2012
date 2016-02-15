using System;
using Server.Items;
using Server.Spells;

namespace Server.Items
{
	public class RegWithdrawStone : Item
	{
		private int m_Amount;

		[CommandProperty( AccessLevel.GameMaster )]
		public int RegAmount
		{
			get{ return m_Amount; }
			set{ m_Amount = value; }
		}

		public override string DefaultName
		{
			get { return "A Reagent Withdraw Stone"; }
		}

		[Constructable]
		public RegWithdrawStone() : base( 0xED4 )
		{
			Movable = false;
			Hue = 2118;
		}


		public override void OnDoubleClick( Mobile from )
		{
			Type[] m_Reagents;
			m_Reagents = new Type[8];
			m_Reagents[0] =	Reagent.BlackPearl;
			m_Reagents[1] =	Reagent.Bloodmoss;
			m_Reagents[2] =	Reagent.MandrakeRoot;
			m_Reagents[3] =	Reagent.SulfurousAsh;
			m_Reagents[4] =	Reagent.SpidersSilk;
			m_Reagents[5] =	Reagent.Garlic;
			m_Reagents[6] =	Reagent.Ginseng;
			m_Reagents[7] =	Reagent.Nightshade;


			int[] m_Amounts;
			m_Amounts = new int[8];
			if (!(m_Amount > 0))
				m_Amount = 30;
			for ( int i = 0; i < 8; ++i )
				m_Amounts[i] = m_Amount;

 			BagOfReagents regBag = new BagOfReagents( m_Amount );
			if ((from.BankBox != null) && (from.BankBox.ConsumeTotal(m_Reagents, m_Amounts) == -1))
			{
				if ( !from.AddToBackpack( regBag ) )
					regBag.Delete();
			} else from.SendMessage("You do not have enough reagents in your bank.");

		}

		public RegWithdrawStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write( (int) m_Amount );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Amount = reader.ReadInt();
		}
	}
}