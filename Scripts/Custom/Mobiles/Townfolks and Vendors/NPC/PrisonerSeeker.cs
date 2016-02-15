namespace Server.Mobiles
{
	public class PrisonerSeeker : SeekerOfAdventure
	{
		[Constructable]
		public PrisonerSeeker()
		{
		}

		public PrisonerSeeker( Serial serial ) : base( serial )
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