using System;
using System.Collections;
using Server;

namespace Server.Items
{
	public class BaseAltar : PentagramAddon
	{
		private BaseSummoningAltar m_SummonAltar;

		public BaseAltar( BaseSummoningAltar summonAltar )
		{
			m_SummonAltar = summonAltar;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if ( m_SummonAltar != null )
				m_SummonAltar.Delete();
		}

		public BaseAltar(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_SummonAltar );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_SummonAltar = reader.ReadItem() as BaseSummoningAltar;

					if ( m_SummonAltar == null )
						Delete();

					break;
				}
			}
		}
	}
}