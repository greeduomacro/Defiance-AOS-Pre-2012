using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class UndeadsBane : Club
	{
		public override int InitMinHits{ get{ return 30; } }
		public override int InitMaxHits{ get{ return 30; } }

		[Constructable]
		public UndeadsBane()
		{
			Hue = 2121;
			WeaponAttributes.MageWeapon = 30;
			WeaponAttributes.UseBestSkill = 1;
			Name = "a dark knight slayer";
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = fire = cold = pois = chaos = direct = 0;
			nrgy = 100;
		}

		public UndeadsBane( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}