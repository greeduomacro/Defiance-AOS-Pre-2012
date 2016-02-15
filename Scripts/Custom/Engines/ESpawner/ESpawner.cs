using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Regions;

namespace Server.Mobiles
{
	#region IESpawner
	public interface IESpawner : ISpawner
	{
		bool UnlinkOnSteal { get; }
	}
	#endregion

	#region EclSpawnEntry
	public class EclSpawnEntry
	{
		private string m_SpawnObjectName;
		public string SpawnObjectName
		{
			get { return m_SpawnObjectName; }
			set { m_SpawnObjectName = value; }
		}

		private ArrayList m_SpawnObjects;
		public ArrayList SpawnObjects
		{
			get { return m_SpawnObjects; }
			set { m_SpawnObjects = value; }
		}

		private int m_Amount;
		public int Amount
		{
			get { return m_Amount; }
			set { m_Amount = value; }
		}

		public EclSpawnEntry(string spawnObjectName, ArrayList spawnObjects, int amount)
		{
			m_SpawnObjectName = spawnObjectName;
			m_SpawnObjects = spawnObjects;
			m_Amount = amount;
		}
	}
	#endregion

	public class ESpawner : Item, IESpawner
	{
		public enum SpecialSpawnType
		{
			None, Paragon, Elder, Plagued
		}

		Region ISpawner.Region{ get{ return Region.Find( Location, Map ); } }

		private List<EclSpawnEntry> m_alSpawnEntries = new List<EclSpawnEntry>();
		public List<EclSpawnEntry> SpawnEntries
		{
			get { return m_alSpawnEntries; }
			set { m_alSpawnEntries = value; }
		}

		private ESpawnerTimer m_Timer;
		private DateTime m_End;

		public readonly static int NumOfFields = 18;
		public virtual Version Version { get { return new Version(2, 1, 6); } }

		#region CommandProperties
		private bool m_bTryFlip;
		[CommandProperty(AccessLevel.Administrator)]
		public bool TryFlip
		{
			get { return m_bTryFlip; }
			set { m_bTryFlip = value; }
		}

		private bool m_bIgnoreWorldSpawn;
		[CommandProperty(AccessLevel.Administrator)]
		public bool IgnoreWorldSpawn
		{
			get { return m_bIgnoreWorldSpawn; }
			set { m_bIgnoreWorldSpawn = value; }
		}

		private SpecialSpawnType m_sstSpawnType;
		[CommandProperty(AccessLevel.GameMaster)]
		public SpecialSpawnType SpecialSpawn
		{
			get { return m_sstSpawnType; }
			set { m_sstSpawnType = value; }
		}

		private AccessLevel m_alVisibilityLevel;
		[CommandProperty(AccessLevel.Administrator)]
		public AccessLevel VisibilityLevel
		{
			get { return m_alVisibilityLevel; }
			set { m_alVisibilityLevel = value; }
		}

		private bool m_bUseMaxAmount;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool UseMaxAmount
		{
			get { return m_bUseMaxAmount; }
			set { m_bUseMaxAmount = value; InvalidateProperties(); }
		}

		private int m_MaxAmount;
		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxAmount
		{
			get { return m_MaxAmount; }
			set {
				m_MaxAmount = value;
				if (m_MaxAmount > 0)
					m_bUseMaxAmount = true;
				else
					m_bUseMaxAmount = false;
				InvalidateProperties();
			}
		}

		private WayPoint m_WayPoint;
		[CommandProperty(AccessLevel.GameMaster)]
		public WayPoint WayPoint
		{
			get { return m_WayPoint; }
			set { m_WayPoint = value; }
		}

		private bool m_Running;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Running
		{
			get { return m_Running; }
			set
			{
				if (value)
					Start();
				else
					Stop();

				InvalidateProperties();
			}
		}

		private int m_HomeRange;
		[CommandProperty(AccessLevel.GameMaster)]
		public int HomeRange
		{
			get { return m_HomeRange; }
			set { m_HomeRange = value; InvalidateProperties(); }
		}

		private int m_Team;
		[CommandProperty(AccessLevel.GameMaster)]
		public int Team
		{
			get { return m_Team; }
			set { m_Team = value; InvalidateProperties(); }
		}

		private TimeSpan m_MinDelay;
		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan MinDelay
		{
			get { return m_MinDelay; }
			set { m_MinDelay = value; InvalidateProperties(); }
		}

		private TimeSpan m_MaxDelay;
		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan MaxDelay
		{
			get { return m_MaxDelay; }
			set { m_MaxDelay = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan NextSpawn
		{
			get
			{
				if (m_Running)
					return m_End - DateTime.Now;
				else
					return TimeSpan.FromSeconds(0);
			}
			set
			{
				Start();
				DoTimer(value);
			}
		}

		private bool m_Group;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Group
		{
			get { return m_Group; }
			set { m_Group = value; InvalidateProperties(); }
		}
		#endregion

		[Constructable]
		public ESpawner()
			: this(true)
		{
		}

		public ESpawner(bool runOnCreate)
			: this(runOnCreate, 0, TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(3.0), 0, false, 20, new List<EclSpawnEntry>(), false , AccessLevel.GameMaster)
		{
		}

		public ESpawner(int maxAmount, TimeSpan minDelay, TimeSpan maxDelay, int team , bool group, int homeRange, List<EclSpawnEntry> EclSpawnEntries, bool bUseMaxAmount, AccessLevel visibilityLevel)
			: this(true, maxAmount, minDelay, maxDelay, team, group, homeRange, EclSpawnEntries, bUseMaxAmount, visibilityLevel)
		{
		}

		public ESpawner(bool runOnCreate, int maxAmount , TimeSpan minDelay, TimeSpan maxDelay , int team, bool group, int homeRange, List<EclSpawnEntry> EclSpawnEntries, bool bUseMaxAmount, AccessLevel visibilityLevel)
			: base(0x1f13)
		{
			Name = "Eclipse Spawner";
			Visible = false;
			Movable = false;

			m_MaxAmount = maxAmount;
			m_MinDelay = minDelay;
			m_MaxDelay = maxDelay;
			m_Team = team;
			m_Group = group;
			m_HomeRange = homeRange;
			m_alSpawnEntries = EclSpawnEntries;
			m_bUseMaxAmount = bUseMaxAmount;
			m_alVisibilityLevel = visibilityLevel;

			if (runOnCreate)
			{
				m_Running = true;
				DoTimer();
			}
		}

		public ESpawner(Serial serial)
			: base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from.AccessLevel < m_alVisibilityLevel)
				return;

			from.CloseGump(typeof(ESpawnerGump));
			from.SendGump(new ESpawnerGump(this));
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (m_Running)
			{
				list.Add(1060742); // active
				list.Add(1061169, m_HomeRange.ToString()); // range ~1_val~
				list.Add(1060658, "group\t{0}", m_Group); // ~1_val~: ~2_val~
				list.Add(1060659, "team\t{0}", m_Team); // ~1_val~: ~2_val~
				list.Add(1060660, "speed\t{0} to {1}", m_MinDelay, m_MaxDelay); // ~1_val~: ~2_val~
				list.Add(1060661, "current amount\t{0}", GetTotalObjectAmount(false)); // ~1_val~: ~2_val~

				if (m_bUseMaxAmount)
					list.Add(1070722, "max amount: {0}", m_MaxAmount);

				for (int i = 0; i < 2 && i < m_alSpawnEntries.Count; ++i)
					list.Add(1060662 + i, "{0}\t{1}", m_alSpawnEntries[i].SpawnObjectName, m_alSpawnEntries[i].SpawnObjects.Count);
			}
			else
				list.Add(1060743); // inactive
		}

		public override void SendInfoTo(NetState state, bool sendOplPacket)
		{
			if (state != null && state.Mobile != null && state.Mobile is PlayerMobile && ((PlayerMobile)state.Mobile).AccessLevel >= VisibilityLevel)
				base.SendInfoTo(state, sendOplPacket);
		}

		public bool InRange(Item item, int range)
		{
			return InRange(item.GetWorldLocation(), item.Map, range);
		}

		public bool InRange(Point3D loc, Map map, int range)
		{
			Point3D pSpawner = GetWorldLocation();

			return (pSpawner.X >= (loc.X - range))
				&& (pSpawner.X <= (loc.X + range))
				&& (pSpawner.Y >= (loc.Y - range))
				&& (pSpawner.Y <= (loc.Y + range))
				&& map == this.Map;
		}

		/// <summary>
		/// Checks if any spawn objects should be removed from the spawner.
		/// </summary>
		public void Defrag()
		{
			bool removed = false;

			foreach (EclSpawnEntry entry in m_alSpawnEntries)
			{
				for (int i = 0; i < entry.SpawnObjects.Count; ++i)
				{
					object o = entry.SpawnObjects[i];

					if (o is Item)
					{
						Item item = (Item)o;

						// Check if spawner and item is in same chest
						if (item.Parent is Container && this.Parent is Container)
						{
							if (((Container)item.Parent).Serial != ((Container)this.Parent).Serial)
							{
								item.Spawner = null;
								entry.SpawnObjects.RemoveAt(i);
								--i;
								removed = true;
							}
						}
						else
						{
							if (item.Deleted || item.Parent != null || !InRange(item, m_HomeRange))
							{
								item.Spawner = null;
								entry.SpawnObjects.RemoveAt(i);
								--i;
								removed = true;
							}
						}
					}
					else if (o is Mobile)
					{
						// Mobiles no longer need to be removed here since V2.1.5
					}
					else
					{
						entry.SpawnObjects.RemoveAt(i);
						--i;
						removed = true;
					}
				}
			}

			if (removed)
				InvalidateProperties();
		}

		/// <summary>
		/// Clears and respawns all entries.
		/// </summary>
		public void Respawn()
		{
			RemoveAllSpawnObjects();

			for (int i = 0; i < m_alSpawnEntries.Count; i++)
			{
				EclSpawnEntry entry = (EclSpawnEntry)m_alSpawnEntries[i];
				for (int a = 0; a < entry.Amount; a++)
				{
					Spawn(i, false);
					InvalidateProperties();
				}
			}
		}

		/// <summary>
		/// Clears and respawns all entries using randomize.
		/// </summary>
		public void RandomRespawn()
		{
			RemoveAllSpawnObjects();

			do
			{
				Spawn(true, false);
			} while (!IsFull(false));
		}

		public bool IsFull(bool defrag)
		{
			if (defrag)
				Defrag();

			if (m_alSpawnEntries.Count <= 0)
				return true;
			else if (MaxAmountCheck())
				return true;
			else if (GetTotalObjectAmount(false) >= GetTotalObjectMaxAmount())
				return true;

			return false;
		}

		/// <summary>
		/// Returns the total amount of how many creatures can be spawned.
		/// </summary>
		/// <returns></returns>
		public int GetTotalObjectMaxAmount()
		{
			int counter = 0;
			foreach (EclSpawnEntry entry in m_alSpawnEntries)
				counter += entry.Amount;
			return counter;
		}

		/// <summary>
		/// Gets the amounts of all objects in all entries.
		/// </summary>
		/// <returns></returns>
		public int GetTotalObjectAmount(bool defrag)
		{
			if (defrag)
				Defrag();

			int counter = 0;
			foreach (EclSpawnEntry entry in m_alSpawnEntries)
				counter += entry.SpawnObjects.Count;
			return counter;
		}

		/// <summary>
		/// Checks if the max amount of objects allowed to spawn has been reached.
		/// </summary>
		/// <returns></returns>
		private bool MaxAmountCheck()
		{
			if (m_bUseMaxAmount && GetTotalObjectAmount(false) >= m_MaxAmount)
				return true;
			return false;
		}

		public void ClearSpawner()
		{
			RemoveAllSpawnObjects();
			m_alSpawnEntries.Clear();
		}

		public void ClearObjects()
		{
			RemoveAllSpawnObjects();
		}

		/// <summary>
		/// Checks if spawn is possible at the given entry.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		public virtual bool CanSpawnAtEntry(EclSpawnEntry entry)
		{
			if (entry.SpawnObjects.Count >= entry.Amount)
				return false;
			return true;
		}

		/// <summary>
		/// Spawns an object.
		/// </summary>
		/// <param name="Randomize">If set to false it will spawn the first available entry.</param>
		/// <returns></returns>
		public bool Spawn(bool Randomize, bool defrag)
		{
			if (defrag)
				Defrag();

			// Try 20 times to find a random spawn entry
			if (Randomize)
			{
				for (int i = 0; i < 20; i++)
				{
					if (Spawn(Utility.Random(m_alSpawnEntries.Count), false))
					{
						InvalidateProperties();
						return true;
					}
				}
			}

			// Spawn first possible one
			for (int i = 0; i < m_alSpawnEntries.Count; i++)
			{
				if (Spawn(i, false))
				{
					InvalidateProperties();
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Spawns a creature at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool Spawn(int index, bool defrag)
		{
			if (defrag)
				Defrag();

			if (index >= m_alSpawnEntries.Count)
				return false;

			EclSpawnEntry entry = (EclSpawnEntry)m_alSpawnEntries[index];

			if (MaxAmountCheck())
				return false;

			if (!CanSpawnAtEntry(entry))
				return false;

			Type type = ESpawner.GetType(entry.SpawnObjectName);

			if (type != null)
			{
				try
				{
					object o = Activator.CreateInstance(type);

					if (o is Mobile)
					{
						Mobile m = (Mobile)o;
						m.Spawner = this;
						OnSpawn(o);

						Point3D spawnLoc;
						Map spawnMap = null;

						if (this.Parent == null)
						{
							spawnLoc = this.GetSpawnPosition();
							spawnMap = this.Map;
						}
						else
						{
							// Invalid location to spawn object, remove object, entry and it's objects.
							m.Delete();
							RemoveSpawnObjects(entry, true, false);
							m_alSpawnEntries.RemoveAt(index);
							return false;
						}

						m.OnBeforeSpawn(spawnLoc, this.Map);

						if (m is BaseCreature)
						{
							BaseCreature c = (BaseCreature)m;
							c.RangeHome = m_HomeRange;
							c.CurrentWayPoint = m_WayPoint;
							c.Home = spawnLoc;

							if (m_Team > 0)
								c.Team = m_Team;

							if (m_sstSpawnType == SpecialSpawnType.Paragon)
								c.IsParagon = true;
							else if (m_sstSpawnType == SpecialSpawnType.Elder)
								c.IsElder = true;
							else if (m_sstSpawnType == SpecialSpawnType.Plagued)
								c.IsPlagued = true;
						}

						m.MoveToWorld(spawnLoc, spawnMap);

						if (m != null && !m.Deleted)
						{
							entry.SpawnObjects.Add(m);
							InvalidateProperties();
						}
					}
					else if (o is Item)
					{
						Item item = (Item)o;
						item.Spawner = this;
						OnSpawn(o);

						if (m_bTryFlip)
							ESpawner.Flip(item);

						entry.SpawnObjects.Add(item);
						InvalidateProperties();

						if (this.Parent is Container)
							((Container)this.Parent).DropItem(item);
						else if (this.Parent == null)
							item.MoveToWorld(GetSpawnPosition(), this.Map);
						else
						{
							// Invalid location to spawn object, remove object, entry and it's objects.
							item.Delete();
							RemoveSpawnObjects(entry, true, false);
							m_alSpawnEntries.RemoveAt(index);
							return false;
						}
					}
				}
				catch
				{
					return false;
				}


				return true;
			}
			else return false;
		}

		public virtual void OnSpawn(object spawn)
		{
		}

		public static void Flip(Item item)
		{
			Type type = item.GetType();

			FlipableAttribute[] AttributeArray = (FlipableAttribute[])type.GetCustomAttributes(typeof(FlipableAttribute), false);

			if (AttributeArray.Length == 0)
			{
				return;
			}

			FlipableAttribute fa = AttributeArray[0];

			fa.Flip(item);
		}

		/// <summary>
		/// Tryes to find a spawn position for an object, if one is not found it returns the spawners location.
		/// </summary>
		/// <returns></returns>
		public Point3D GetSpawnPosition()
		{
			Map map = Map;

			if (map == null)
				return Location;

			// Try 10 times to find a Spawnable location.
			for (int i = 0; i < 10; i++)
			{
				int x = Location.X + (Utility.Random((m_HomeRange * 2) + 1) - m_HomeRange);
				int y = Location.Y + (Utility.Random((m_HomeRange * 2) + 1) - m_HomeRange);
				int z = Map.GetAverageZ(x, y);

				if (Map.CanSpawnMobile(new Point2D(x, y), this.Z))
					return new Point3D(x, y, this.Z);
				else if (Map.CanSpawnMobile(new Point2D(x, y), z))
					return new Point3D(x, y, z);
			}

			return this.Location;
		}

		public TimeSpan GetDelay()
		{
			int min = (int)m_MinDelay.TotalSeconds;
			int max = (int)m_MaxDelay.TotalSeconds;

			return TimeSpan.FromSeconds(Utility.RandomMinMax(min, max));
		}

		/// <summary>
		/// Lowers the spawn amount for the entry at the given index.
		/// </summary>
		/// <param name="index"></param>
		public void LowerMax(int index)
		{
			Defrag();

			if (Map == null || Map == Map.Internal || index >= m_alSpawnEntries.Count)
				return;

			EclSpawnEntry entry = (EclSpawnEntry)m_alSpawnEntries[index];
			entry.Amount--;

			// Remove creature if creature count is larger than maxcount
			if (entry.SpawnObjects.Count > entry.Amount)
				RemoveSpawnObject(entry, 1, true, false);

			// if max value is 0 remove the entry
			if (entry.Amount <= 0)
			{
				// Remove Creatures and the Entry
				RemoveSpawnObjects(entry, true, false);
				m_alSpawnEntries.RemoveAt(index);
			}
		}

		/// <summary>
		/// Raises the spawn amount for the entry at the given index.
		/// </summary>
		/// <param name="index"></param>
		public void RaiseMax(int index)
		{
			if (index >= m_alSpawnEntries.Count)
				return;

			EclSpawnEntry entry = (EclSpawnEntry)m_alSpawnEntries[index];

			entry.Amount++;
			InvalidateProperties();
		}

		public void RemoveAllSpawnObjects()
		{
			Defrag();
			bool removed = false;

			foreach (EclSpawnEntry entry in m_alSpawnEntries)
				if (RemoveSpawnObjects(entry, false, false))
					removed = true;

			if (removed)
				InvalidateProperties();
		}

		public bool RemoveSpawnObjects(EclSpawnEntry entry, bool invalidate, bool defrag)
		{
			if (defrag)
				Defrag();

			bool removed = false;

			for (int i = entry.SpawnObjects.Count - 1; i >= 0; --i)
			{
				object o = entry.SpawnObjects[i];

				if (o is Item)
				{
					((Item)o).Delete();
					removed = true;
				}
				else if (o is Mobile)
				{
					((Mobile)o).Delete();
					removed = true;
				}
			}

			if (removed && invalidate)
				InvalidateProperties();

			return removed;
		}

		public bool RemoveSpawnObject(object obj, bool invalidate, bool defrag)
		{
			if (defrag)
				Defrag();

			foreach (EclSpawnEntry entry in m_alSpawnEntries)
			{
				if (entry.SpawnObjects.Contains(obj))
				{
					entry.SpawnObjects.Remove(obj);

					if (invalidate)
						InvalidateProperties();
					return true;
				}
			}

			return false;
		}

		public bool RemoveSpawnObject(EclSpawnEntry entry, int amount, bool invalidate, bool defrag)
		{
			if (defrag)
				Defrag();

			bool removed = false;

			for (int i = 0; i < amount && i < entry.SpawnObjects.Count; ++i)
			{
				object o = entry.SpawnObjects[i];

				if (o is Item)
				{
					((Item)o).Delete();
					removed = true;
				}
				else if (o is Mobile)
				{
					((Mobile)o).Delete();
					removed = true;
				}
			}

			if (removed && invalidate)
				InvalidateProperties();

			return removed;
		}

		/// <summary>
		/// Moves all objects to the spawners location.
		/// </summary>
		public void BringToHome()
		{
			Defrag();

			foreach (EclSpawnEntry entry in m_alSpawnEntries)
			{
				for (int i = 0; i < entry.SpawnObjects.Count; ++i)
				{
					object o = entry.SpawnObjects[i];

					if (o is Mobile)
					{
						Mobile m = (Mobile)o;
						m.MoveToWorld(Location, Map);
					}
					else if (o is Item)
					{
						Item item = (Item)o;
						item.MoveToWorld(Location, Map);
					}
				}
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();

			RemoveAllSpawnObjects();

			if (m_Timer != null)
				m_Timer.Stop();
		}

		public virtual void OnRemove(object obj)
		{
		}

		public static Type GetType(string name)
		{
			return ScriptCompiler.FindTypeByName(name);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)4); // version

			// Version 4
			// Removed amount from EclSpawnEntry

			// Version 3
			writer.Write((int)m_sstSpawnType);

			// Version 2
			writer.Write((bool)m_bTryFlip);

			// Version 1
			writer.Write((bool)m_bIgnoreWorldSpawn);

			// Version 0
			writer.Write((int)m_alVisibilityLevel);
			writer.Write((bool)m_bUseMaxAmount);
			writer.Write(m_WayPoint);
			writer.Write(m_Group);

			writer.Write(m_MinDelay);
			writer.Write(m_MaxDelay);
			writer.Write(m_MaxAmount);
			writer.Write(m_Team);
			writer.Write(m_HomeRange);
			writer.Write(m_Running);

			if (m_Running)
				writer.WriteDeltaTime(m_End);

			writer.Write(m_alSpawnEntries.Count);
			foreach (EclSpawnEntry entry in m_alSpawnEntries)
			{
				writer.Write((string)entry.SpawnObjectName);
				writer.Write((int)entry.Amount);

				writer.Write(entry.SpawnObjects.Count);
				for (int i = 0; i < entry.SpawnObjects.Count; i++)
				{
					object o = entry.SpawnObjects[i];

					if (o is Item)
						writer.Write((Item)o);
					else if (o is Mobile)
						writer.Write((Mobile)o);
					else
						writer.Write(Serial.MinusOne);
				}
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				case 4:
					goto case 3;
				case 3:
					m_sstSpawnType = (SpecialSpawnType)reader.ReadInt();
					goto case 2;
				case 2:
					m_bTryFlip = reader.ReadBool();
					goto case 1;
				case 1:
					m_bIgnoreWorldSpawn = reader.ReadBool();
					goto case 0;
				case 0:
					m_alVisibilityLevel = (AccessLevel)reader.ReadInt();
					m_bUseMaxAmount = reader.ReadBool();
					m_WayPoint = reader.ReadItem() as WayPoint;
					m_Group = reader.ReadBool();

					m_MinDelay = reader.ReadTimeSpan();
					m_MaxDelay = reader.ReadTimeSpan();

					m_MaxAmount = reader.ReadInt();
					m_Team = reader.ReadInt();
					m_HomeRange = reader.ReadInt();
					m_Running = reader.ReadBool();

					TimeSpan ts = TimeSpan.Zero;

					if (m_Running)
						ts = reader.ReadDeltaTime() - DateTime.Now;

					int size = reader.ReadInt();
					m_alSpawnEntries = new List<EclSpawnEntry>(size);

					for (int i = 0; i < size; i++)
					{
						if (version < 4)
							reader.ReadInt();

						string typeName = reader.ReadString();
						int amount = reader.ReadInt();

						// Check the type
						if (ESpawner.GetType(typeName) == null)
						{
							if (m_WarnTimer == null)
								m_WarnTimer = new WarnTimer();

							m_WarnTimer.Add(this.GetWorldLocation(), Map, typeName);
						}

						int count = reader.ReadInt();
						ArrayList alSpawnObjects = new ArrayList(count);

						for (int a = 0; a < count; a++)
						{
							IEntity e = World.FindEntity(reader.ReadInt());

							if (e is Item)
								((Item)e).Spawner = this;
							else if (e is Mobile)
								((Mobile)e).Spawner = this;

							if (e != null)
								alSpawnObjects.Add(e);
						}

						m_alSpawnEntries.Add(new EclSpawnEntry(typeName, alSpawnObjects, amount));
					}

					if (m_Running)
						DoTimer(ts);

					break;
			}
		}

		#region Timer
		public void OnTickSpawn()
		{
			DoTimer();

			if (m_Group)
			{
				if (GetTotalObjectAmount(true) == 0)
					Respawn();
				else
					return;
			}
			else
				Spawn(true, true);
		}

		public void Start()
		{
			if (!m_Running)
			{
				if (m_alSpawnEntries.Count > 0)
				{
					m_Running = true;
					DoTimer();
				}
			}
		}

		public void Stop()
		{
			if (m_Running)
			{
				m_Timer.Stop();
				m_Running = false;
			}
		}

		public void DoTimer()
		{
			if (!m_Running)
				return;

			DoTimer(GetDelay());
		}

		public void DoTimer(TimeSpan delay)
		{
			if (!m_Running)
				return;

			m_End = DateTime.Now + delay;

			if (m_Timer != null)
				m_Timer.Stop();

			m_Timer = new ESpawnerTimer(this, delay);
			m_Timer.Start();
		}

		private class ESpawnerTimer : Timer
		{
			private ESpawner m_Spawner;

			public ESpawnerTimer(ESpawner spawner, TimeSpan delay)
				: base(delay)
			{
				if (spawner.IsFull(false))
					Priority = TimerPriority.FiveSeconds;
				else
					Priority = TimerPriority.OneSecond;

				m_Spawner = spawner;
			}

			protected override void OnTick()
			{
				if (m_Spawner != null)
					if (!m_Spawner.Deleted)
						m_Spawner.OnTickSpawn();
			}
		}
		#endregion

		#region IESpawner Functions
		public bool UnlinkOnSteal { get { return true; } }
		public bool UnlinkOnTaming { get { return true; } }
		public Point3D Home { get { return Location; } }
		public int Range { get { return HomeRange; } }

		void ISpawner.Remove(object spawn)
		{
			OnRemove(spawn);
			RemoveSpawnObject(spawn, true, false);
			DoTimer();
		}
		#endregion

		#region WarnTimer
		private static WarnTimer m_WarnTimer;

		private class WarnTimer : Timer
		{
			private ArrayList m_List;

			private class WarnEntry
			{
				public Point3D m_Point;
				public Map m_Map;
				public string m_Name;

				public WarnEntry(Point3D p, Map map, string name)
				{
					m_Point = p;
					m_Map = map;
					m_Name = name;
				}
			}

			public WarnTimer()
				: base(TimeSpan.FromSeconds(1.0))
			{
				m_List = new ArrayList();
				Start();
			}

			public void Add(Point3D p, Map map, string name)
			{
				m_List.Add(new WarnEntry(p, map, name));
			}

			protected override void OnTick()
			{
				try
				{
					Console.WriteLine("Eclipse Spawner Warning: {0} bad spawns detected, logged: 'EclSpawner.log'", m_List.Count);

					using (StreamWriter op = new StreamWriter("ESpawner.log", true))
					{
						op.WriteLine("# Bad spawns : {0}", DateTime.Now);
						op.WriteLine("# Format: X Y Z F Name");
						op.WriteLine();

						foreach (WarnEntry e in m_List)
							op.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", e.m_Point.X, e.m_Point.Y, e.m_Point.Z, e.m_Map, e.m_Name);

						op.WriteLine();
						op.WriteLine();
					}
				}
				catch
				{
				}
			}
		}
		#endregion
	}
}