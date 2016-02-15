using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Events.DoomSystem
{
	public class PoisonRoomRegion : BaseDoomSystemRegion
	{
		public List<DarkGuardian> GetDarkGuardians
		{
			get
			{
				List<DarkGuardian> list = new List<DarkGuardian>();

				foreach (Mobile m in DoomSystem.PoisonRoom.GetMobiles())
					if (m is DarkGuardian)
						list.Add((DarkGuardian)m);

				return list;
			}
		}

		public List<DarkWoodDoor> Doors
		{
			get
			{
				List<DarkWoodDoor> list = new List<DarkWoodDoor>();

				for (int i = 0; i < 2; i++)
					for (int j = 0; j < 4; j++)
						foreach (Item item in Map.Malas.GetItemsInRange(new Point3D(355 + i, 13 + j, -1), 0))
							if (item is DarkWoodDoor)
								list.Add((DarkWoodDoor)item);
				return list;
			}
		}

		public PoisonRoomRegion() : base("Poison Room Region", Map.Malas, 80, new Rectangle2D[] { new Rectangle2D(356, 6, 18, 18) }, 3)
		{
		}

		public override void Activate()
		{
			base.Activate();

			foreach (DarkWoodDoor door in Doors)
			{
				door.Open = false;
				door.Locked = true;
			}

			foreach (Mobile m in AliveMobiles)
			{
				for (int j = 0; j < 2; j++)
					new DarkGuardian().MoveToWorld(new Point3D(363 + Utility.Random(5), 13 + Utility.Random(5), -1), Map.Malas);

				m.SendLocalizedMessage(1050000); // The locks on the door click loudly and you begin to hear a faint hissing near the walls.
			}
		}

		public override void Deactivate()
		{
			foreach (DarkWoodDoor door in Doors)
			{
				door.Locked = false;
				door.Open = true;
			}

			List<DarkGuardian> list = GetDarkGuardians;

			foreach (DarkGuardian guardian in list)
				guardian.Delete();

			foreach (Mobile m in AliveMobiles)
				m.SendLocalizedMessage(1050055); // You hear the doors unlocking and the hissing stops.

			Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(PlayersInside_Callback));

			base.Deactivate();
		}

		private static void PlayersInside_Callback()
		{
			if (!DoomSystem.PoisonRoom.NoMobilesAlive && !DoomSystem.PoisonRoom.Active)
				DoomSystem.PoisonRoom.Activate();
		}

		public override void OnLocationChanged(Mobile m, Point3D oldlocation)
		{
			Point3D newloc = m.Location;

			if (!m.Alive && newloc.X == 356 && oldlocation.X > 355 && (newloc.Y == 14 || newloc.Y == 15))
			{
				m.MoveToWorld(new Point3D(343, 176, -1), Map.Malas);
				if (m.Corpse != null)
					m.Corpse.MoveToWorld(new Point3D(343, 176, -1), Map.Malas);
			}

			base.OnLocationChanged(m, oldlocation);
		}
	}
}