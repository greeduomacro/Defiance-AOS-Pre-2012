namespace Server.Items
{
	public class SkillBallBox : MetalBox
	{
	  [Constructable]
		public SkillBallBox():base()
		{
			Name = "Donation Skill Ball Box";
			switch (Utility.Random(5))
			{
				case 0: Hue = 2119; break;
				case 1: Hue = 2114; break;
				case 2: Hue = 2127; break;
				case 3: Hue = 1322; break;
				case 4: Hue = 1420; break;
			}
		}

		protected void AddBall(SkillBall ball)
		{
			DropItem(ball);
			ball.Location = new Point3D(20 * Items.Count, 100, 0);
		}

		public SkillBallBox(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	public class SkillballBundleSmall_100_AOS : SkillBallBox
	{
		[Constructable]
		public SkillballBundleSmall_100_AOS()
			: base()
		{
			AddBall(new SkillBall(10, false));
			AddBall(new SkillBall(25, false));
			AddBall(new SkillBall(25, false));
			AddBall(new SkillBall(50, false));
		}

		public SkillballBundleSmall_100_AOS(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}

	public class SkillballBundleLarge_100_AOS : SkillBallBox
	{
		[Constructable]
		public SkillballBundleLarge_100_AOS():base()
		{
			AddBall(new SkillBall(10, false));
			AddBall(new SkillBall(25, false));
			AddBall(new SkillBall(25, false));
			AddBall(new SkillBall(50, false));
			AddBall(new SkillBall(50, false));
			AddBall(new SkillBall(50, false));
		}

		public SkillballBundleLarge_100_AOS(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}

	public class SkillballBundleExtraLarge_100_AOS : SkillBallBox
	{
		[Constructable]
		public SkillballBundleExtraLarge_100_AOS()
			: base()
		{
			for (int i = 0; i < 2; i++)
			{
				AddBall(new SkillBall(10, false));
				AddBall(new SkillBall(25, false));
				AddBall(new SkillBall(50, false));
				AddBall(new SkillBall(75, false));
			}
		}

		public SkillballBundleExtraLarge_100_AOS(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}

	public class SkillballBundleSmall_120_AOS : SkillBallBox
	{
		[Constructable]
		public SkillballBundleSmall_120_AOS()
			: base()
		{
			AddBall(new SkillBall(10, 120, false));
			AddBall(new SkillBall(20, 120, false));
		}

		public SkillballBundleSmall_120_AOS(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}

	public class SkillballBundleLarge_120_AOS : SkillBallBox
	{
		[Constructable]
		public SkillballBundleLarge_120_AOS()
			: base()
		{
			for (int i = 0; i < 2; i++)
			{
				AddBall(new SkillBall(10, 120, false));
				AddBall(new SkillBall(20, 120, false));
			}
		}

		public SkillballBundleLarge_120_AOS(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}