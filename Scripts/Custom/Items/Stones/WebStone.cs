using System;
using Server.Network;

namespace Server.Items
{
	public class WebStone : Item
	{
		private string m_sUrl;

		#region CommandProperties
		[CommandProperty( AccessLevel.GameMaster )]
		public string Url
		{
			get { return m_sUrl; }
			set { m_sUrl = value; }
		}
		#endregion

		[Constructable]
		public WebStone() : base( 0xED4 )
		{
			Movable = false;
			Hue = 0x2D1;
			Name = "a Web Stone";
		}

		public WebStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version

			writer.Write( (string) m_sUrl );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_sUrl = reader.ReadString();
					break;
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 2 ) )
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			else
				from.LaunchBrowser( m_sUrl );
		}
	}
}