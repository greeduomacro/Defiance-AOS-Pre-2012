using System;
using Server.Items;
using Server.Spells;

namespace Server.Items
{
	public class NecroRegWithdrawStone : Item
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
			get { return "A Necro Reagent Withdraw Stone"; }
		}

		[Constructable]
		public NecroRegWithdrawStone() : base( 0xED4 )
		{
			Movable = false;
			Hue = 701;
		}


		public override void OnDoubleClick( Mobile from )
		{
			Type[] m_Reagents;
			m_Reagents = new Type[5];
			m_Reagents[0] =	Reagent.NoxCrystal;
			m_Reagents[1] =	Reagent.GraveDust;
			m_Reagents[2] =	Reagent.PigIron;
			m_Reagents[3] =	Reagent.BatWing;
			m_Reagents[4] =	Reagent.DaemonBlood;


			int[] m_Amounts;
			m_Amounts = new int[5];
			if (!(m_Amount > 0))
				m_Amount = 30;
			for ( int i = 0; i < 5; ++i )
				m_Amounts[i] = m_Amount;

 			BagOfNecroReagents regBag = new BagOfNecroReagents( m_Amount );
			if ((from.BankBox != null) && (from.BankBox.ConsumeTotal(m_Reagents, m_Amounts) == -1))
			{
				if ( !from.AddToBackpack( regBag ) )
					regBag.Delete();
			} else from.SendMessage("You do not have enough reagents in your bank.");

		}

		public NecroRegWithdrawStone( Serial serial ) : base( serial )
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