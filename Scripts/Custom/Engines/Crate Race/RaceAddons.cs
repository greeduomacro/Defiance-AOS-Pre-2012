using Server.Items;

namespace Server.Events.CrateRace
{
	public class RaceTrackAddon1 : BaseAddon
	{
		[Constructable]
		public RaceTrackAddon1()
		{
			#region horizontal
			AddComponent(new AddonComponent(700), -11, -21, 0);
			AddComponent(new AddonComponent(700), -21, -31, 0);

			for (int x = 0; x < 22; x++)
			{
				AddComponent(new AddonComponent(698), -10 + x, -21, 0);
				AddComponent(new AddonComponent(698), -10 + x, 21, 0);
			}

			for (int x = 0; x < 42; x++)
			{
				AddComponent(new AddonComponent(698), -20 + x, -31, 0);
				AddComponent(new AddonComponent(698), -20 + x, 31, 0);
			}
			#endregion

			#region vertical
			for (int y = 0; y < 42; y++)
			{
				AddComponent(new AddonComponent(699), -11, -20 + y, 0);
				AddComponent(new AddonComponent(699), 11, -20 + y, 0);
			}

			for (int y = 0; y < 62; y++)
			{
				AddComponent(new AddonComponent(699), -21, -30 + y, 0);
				AddComponent(new AddonComponent(699), 21, -30 + y, 0);
			}
			#endregion

			#region Podium
			AddComponent(new AddonComponent(657), 1, -14, 3);
			AddComponent(new AddonComponent(658), 0, -14, 3);
			AddComponent(new AddonComponent(659), 1, -15, 3);
			AddComponent(new AddonComponent(675), 1, -17, 3);
			AddComponent(new AddonComponent(675), 1, -16, 3);
			AddComponent(new AddonComponent(700), 1, -18, 0);

			int ii = 13;

			for (int i = -17; i < -12; i += 2)
			{
				if (i == -15)
					ii = 23;

				if (i == -13)
					ii = 6;

				AddComponent(new AddonComponent(1173), 0, i, ii);
				AddComponent(new AddonComponent(1173), 0, i + 1, ii);
				AddComponent(new AddonComponent(1173), 1, i, ii);
				AddComponent(new AddonComponent(1173), 1, i + 1, ii);

				AddComponent(new AddonComponent(697), 1, i + 1, ii);

				AddComponent(new AddonComponent(699), -1, i, ii);
				AddComponent(new AddonComponent(699), -1, i + 1, ii);
				AddComponent(new AddonComponent(699), 1, i, ii);

				AddComponent(new AddonComponent(700), -1, i - 1, ii);

				AddComponent(new AddonComponent(698), 0, i + 1, ii);
				AddComponent(new AddonComponent(699), 1, i, ii);

				AddComponent(new AddonComponent(698), 0, i - 1, ii);
				AddComponent(new AddonComponent(698), 1, i - 1, ii);
			}

			for (int i = -17; i < -11; i++)
			{
				AddComponent(new AddonComponent(699), 1, i, 0);
				//AddComponent(new AddonComponent(699), 11, i, 0);
			}

			for (int i = 0; i < 2; i++)
			{
				AddComponent(new AddonComponent(677), i, -12, 3);
				AddComponent(new AddonComponent(698), i, -12, 0);
				AddComponent(new AddonComponent(695), 1, -12 - i, 3);

			}

			for (int i = 0; i < 10; i++)
			{
				AddComponent(new AddonComponent(698), i + 2, -18, 0);
				AddComponent(new AddonComponent(698), i + 2, -12, 0);
			}
			#endregion
		}

		public RaceTrackAddon1(Serial serial)
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
	}

	public class RaceTrackAddon : BaseAddon
	{
		[Constructable]
		public RaceTrackAddon()
		{
			#region horizontal
			AddComponent(new AddonComponent(223), -11, -21, 0);
			AddComponent(new AddonComponent(223), -21, -31, 0);


			for (int x = 0; x < 22; x++)
				AddComponent(new AddonComponent(222), -10 + x, -21, 0);

			for (int x = 0; x < 21; x++)
				AddComponent(new AddonComponent(222), -10 + x, 21, 0);


			for (int x = 0; x < 42; x++)
				AddComponent(new AddonComponent(222), -20 + x, -31, 0);

			for (int x = 0; x < 41; x++)
				AddComponent(new AddonComponent(222), -20 + x, 31, 0);

			#endregion

			#region vertical
			for (int y = 0; y < 42; y++)
				AddComponent(new AddonComponent(221), -11, -20 + y, 0);

			for (int y = 0; y < 41; y++)
				if(y != 2 && y != 8)
				AddComponent(new AddonComponent(221), 11, -20 + y, 0);


			for (int y = 0; y < 62; y++)
				AddComponent(new AddonComponent(221), -21, -30 + y, 0);

			for (int y = 0; y < 61; y++)
				AddComponent(new AddonComponent(221), 21, -30 + y, 0);
			#endregion

			#region corners
			AddComponent(new AddonComponent(220), 21, 31, 0);
			AddComponent(new AddonComponent(220), 11, 21, 0);
			AddComponent(new AddonComponent(220), 11, -12, 0);
			AddComponent(new AddonComponent(220), 11, -18, 0);
			AddComponent(new AddonComponent(220), 2, -12, 20);
			AddComponent(new AddonComponent(220), 2, -14, 20);
			AddComponent(new AddonComponent(220), 2, -14, 23);
			#endregion

			#region Podium

			AddComponent(new AddonComponent(221), -1, -14, 23);
			AddComponent(new AddonComponent(221), 2, -15, 23);
			AddComponent(new AddonComponent(221), -1, -15, 23);
			AddComponent(new AddonComponent(221), 2, -14, 23);
			AddComponent(new AddonComponent(223), -1, -18, 20);
			AddComponent(new AddonComponent(223), -1, -16, 23);
			AddComponent(new AddonComponent(204), -1, -18, 0);

			for (int i = 0; i < 6; i+= 2)
			{
				AddComponent(new AddonComponent(206), 2, -12 - i, 0);
				AddComponent(new AddonComponent(208), 2, -13 - i, 0);
				AddComponent(new AddonComponent(201), -1, -12 - i, 0);
				AddComponent(new AddonComponent(201), -1, -13 - i, 0);
			}


			for (int i = 0; i < 12; i++)
			{
				if (i < 3)
				{
					if (i != 2)
						AddComponent(new AddonComponent(200), i, -12, 0);
					AddComponent(new AddonComponent(200), i, -18, 0);
					if (i != 2)
						AddComponent(new AddonComponent(222), i, -12, 20);
					AddComponent(new AddonComponent(222), i, -18, 20);
					if (i != 2)
						AddComponent(new AddonComponent(222), i, -14, 20);
					AddComponent(new AddonComponent(222), i, -16, 20);
					if (i != 2)
						AddComponent(new AddonComponent(222), i, -14, 23);
					AddComponent(new AddonComponent(222), i, -16, 23);

					for (int j = 0; j < 6; j++)
					{
						if (i != 2)
							AddComponent(new AddonComponent(1310), i, -12 - j, 20);
					}
				}

				else
				{
					AddComponent(new AddonComponent(222), i, -12, 0);
					AddComponent(new AddonComponent(222), i, -18, 0);
				}
			}

			for (int i = 12; i < 18; i++)
			{
				AddComponent(new AddonComponent(221), -1, -i, 20);
				AddComponent(new AddonComponent(221), 2, -i, 20);

			}
			#endregion
		}

		public RaceTrackAddon(Serial serial)
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
	}
}