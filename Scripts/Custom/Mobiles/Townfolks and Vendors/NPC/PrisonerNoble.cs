namespace Server.Mobiles
{
	public class PrisonerNoble : Noble
	{
		[Constructable]
		public PrisonerNoble()
		{
		}

		public PrisonerNoble( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}
}