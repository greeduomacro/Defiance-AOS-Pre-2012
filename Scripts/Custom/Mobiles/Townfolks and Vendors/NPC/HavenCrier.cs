using System;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	public class HavenCrier : Mobile
	{
		public bool Running;

		public string[] Messages = new string[]
			{
				"Greetings young traveler, and welcome to the AOS shard of Defiance.",
				"You are currently resisiding in the young player region, in this region you cannot harm players nor can they harm you.",
				//"You will be able to return to this area by using the teleportation ball you received, untill you have reached the age of 24 play hours.",
				//"The starting gold and reagents will be available to you in your bank account within 10 minutes.",
				"The region around this city is inhabited by armed and dangerous creatures. The further you go the more dangerous the creatures will be.",
				"Behind the blacksmith you will find some training dummies and a range to train your fighting skills up to 50 points.",
				"If you want to go to Felucca, where most players are, follow the road south-east, until you see a sign pointing the Moongate.",
				"Once you leave this region, you will NOT be able to return.",
				"We hope you will have a pleasant stay, and good luck."
			};

		[Constructable]
		public HavenCrier()
			: base()
		{
			InitStats(100, 100, 25);

			Title = "the town crier";
			Hue = Utility.RandomSkinHue();

			if (Female = Utility.RandomBool())
			{
				Body = 0x191;
				Name = NameList.RandomName("female");
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName("male");
			}

			AddItem(new FancyShirt(Utility.RandomBlueHue()));

			Item skirt = new Kilt();
			skirt.Hue = Utility.RandomGreenHue();
			AddItem(skirt);
			AddItem(new FeatheredHat(Utility.RandomGreenHue()));

			Item boots = new ThighBoots();
			AddItem(boots);

			Utility.AssignRandomHair(this);
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if (!Running && m.AccessLevel == AccessLevel.Player)
			{
				Running = true;
				new HavenCrierTimer(this).Start();
			}
		}

		public override bool CanBeDamaged() { return false; }

		public void Cry(int nr)
		{
			PublicOverheadMessage(MessageType.Regular,0x3B2,false,Messages[nr]);
		}

		public HavenCrier(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
		}
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
		}

		private class HavenCrierTimer : Timer
		{
			private HavenCrier m_Crier;
			private int m_Number;
			private int m_Max;

			public HavenCrierTimer(HavenCrier crier)
				: base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10))
			{
				m_Crier = crier;
				m_Max = crier.Messages.Length;
			}

			protected override void OnTick()
			{
				if (m_Number < m_Max)
				{
					m_Crier.Cry(m_Number);
					m_Number++;
				}

				else
				{
					m_Crier.Running = false;
					Stop();
				}
			}
		}
	}
}