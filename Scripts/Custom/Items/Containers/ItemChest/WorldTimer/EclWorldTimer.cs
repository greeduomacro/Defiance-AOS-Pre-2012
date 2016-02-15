using System;
using System.Collections;
using Server;
using Server.Scripts.Commands;
using Server.Mobiles;

namespace Server.Items
{
	public class WorldTimerEntry
	{
		public Item m_item;
		public DateTime m_time;

		public WorldTimerEntry( Item item, DateTime time )
		{
			m_item = item;
			m_time = time;
		}
	}

	public interface IWorldTimer
	{
		void OnTick( WorldTimerEntry entry );
	}

	public class EclWorldTimer
	{
		public static ArrayList m_SpawnList = new ArrayList();

		/// <summary>
		/// Change interval here for how often it checks for spawn requests
		/// </summary>
		public static TimeSpan m_WorldTimerInterval = TimeSpan.FromSeconds(30.0);

		public static void Initialize()
		{
			new WorldSpawnTimer( m_WorldTimerInterval ).Start();
		}

		/// <summary>
		/// This function returns the WorldTimerEntry for a Item using the items Serial Value.
		/// </summary>
		/// <param name="iSpawnerSerial">The Item.</param>
		/// <returns></returns>
		public static WorldTimerEntry GetEntry( Item item )
		{
			foreach( WorldTimerEntry entry in m_SpawnList)
				if( entry.m_item.Serial.Value == item.Serial.Value )
					return entry;

			return null;
		}

		/// <summary>
		/// This function will add a request to the world timer,
		/// if the item already has a request there it will be updated to the new time.
		/// </summary>
		/// <param name="item">The Item that request a timer call.</param>
		/// <param name="delay">The delay for the OnTick() call.</param>
		public static void AddTime( Item item, TimeSpan delay )
		{
			WorldTimerEntry entry = EclWorldTimer.GetEntry( item );
			if( entry != null )
				entry.m_time = DateTime.Now + delay;
			else
				m_SpawnList.Add( new WorldTimerEntry( item, DateTime.Now + delay ));
		}

		private class WorldSpawnTimer : Timer
		{
			public WorldSpawnTimer( TimeSpan delay ) : base( delay, delay )
			{
				Priority = TimerPriority.FiveSeconds;
			}

			private void Spawn( WorldTimerEntry entry )
			{
				if( entry.m_item is IWorldTimer )
					((IWorldTimer)entry.m_item).OnTick( entry );
			}

			protected override void OnTick()
			{
				for(int i=0;i<m_SpawnList.Count;i++)
				{
					WorldTimerEntry entry = (WorldTimerEntry)m_SpawnList[i];

					// Remove entry from spawn list if item has been deleted
					if( entry.m_item == null || entry.m_item.Deleted )
					{
						m_SpawnList.RemoveAt(i);
						continue;
					}

					// Check if Spawn is possible for item
					if( DateTime.Compare(entry.m_time, DateTime.Now) <= 0 )
					{
						Spawn( entry );
					}
				}
			}
		}
	}
}