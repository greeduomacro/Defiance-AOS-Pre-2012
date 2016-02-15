using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public abstract class BaseSummoningAltar : Item, ISpawner
	{
		private List<Item> m_RedSkulls;
		private List<Item> m_WhiteSkulls;

		private BaseCreature m_Champion;
		public BaseCreature Champion
		{
			get { return m_Champion; }
			set { m_Champion = value; }
		}

		private BaseAltar m_Altar;
		public BaseAltar Altar
		{
			get { return m_Altar; }
			set { m_Altar = value; }
		}

		private BasePlatform m_Platform;
		public BasePlatform Platform
		{
			get { return m_Platform; }
			set { m_Platform = value; }
		}

		#region CommandProperties
		protected bool m_Active;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Active
		{
			get
			{
				return m_Active;
			}
			set
			{
				m_Active = value;
				OnActiveChange(value);
				InvalidateProperties();

				if (m_Active)
					CheckSpawn();
			}
		}

		protected int m_iMainQueue;
		[CommandProperty(AccessLevel.GameMaster)]
		public int MainQueue
		{
			get { return m_iMainQueue; }
			set
			{
				if (value > MaxInQueue)
					m_iMainQueue = MaxInQueue;
				else
					m_iMainQueue = value;

				UpdateCandles();
				CheckSpawn();
			}
		}

		private BaseCreature m_PreCreatedChampion;
		[CommandProperty(AccessLevel.GameMaster)]
		public BaseCreature PreCreatedChampion
		{
			get { return m_PreCreatedChampion; }
			set { m_PreCreatedChampion = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Level
		{
			get { return m_WhiteSkulls.Count; }
		}
		#endregion

		public virtual Type ChampionType { get { return null; } }
		public virtual int HueActive { get { return 0; } }
		public virtual int HueInactive { get { return 0; } }
		public virtual int MaxInQueue { get { return 84; } }

		public BaseSummoningAltar()
			: base(0xBD2)
		{
			Movable = false;
			Visible = false;
			Name = "summoning altar";

			m_Altar = new BaseAltar(this);
			m_Altar.Hue = HueInactive;
			m_Platform = new BasePlatform(this);

			m_RedSkulls = new List<Item>();
			m_WhiteSkulls = new List<Item>();
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (m_Active)
				list.Add(1060742); // active
			else
				list.Add(1060743); // inactive
		}

		public virtual void OnActiveChange(bool value)
		{
		}

		public virtual void CheckAltarHue()
		{
			if (m_Champion == null || !m_Champion.Alive)
			{
				m_Champion = null;
				if (m_Altar != null)
					m_Altar.Hue = HueInactive;
			}
			else
			{
				if (m_Altar != null)
					m_Altar.Hue = HueActive;
			}
		}

		public virtual void UpdateCandles()
		{
			int white = m_iMainQueue >= 17 ? m_iMainQueue / 17 : 0;
			SetRedSkullCount(m_iMainQueue - white * 17);
			SetWhiteSkullCount(white);
		}

		public void SetRedSkullCount(int val)
		{
			for (int i = m_RedSkulls.Count - 1; i >= val; --i)
			{
				((Item)m_RedSkulls[i]).Delete();
				m_RedSkulls.RemoveAt(i);
			}

			for (int i = m_RedSkulls.Count; i < val; ++i)
			{
				Item skull = new Item(0x1854);

				skull.Hue = 0x26;
				skull.Movable = false;
				skull.Light = LightType.Circle150;

				skull.MoveToWorld(GetRedSkullLocation(i), Map);

				m_RedSkulls.Add(skull);

				Effects.PlaySound(skull.Location, skull.Map, 0x29);
				Effects.SendLocationEffect(new Point3D(skull.X + 1, skull.Y + 1, skull.Z), skull.Map, 0x3728, 10);
			}
		}

		public void SetWhiteSkullCount(int val)
		{
			for (int i = m_WhiteSkulls.Count - 1; i >= val; --i)
			{
				((Item)m_WhiteSkulls[i]).Delete();
				m_WhiteSkulls.RemoveAt(i);
			}

			for (int i = m_WhiteSkulls.Count; i < val; ++i)
			{
				Item skull = new Item(0x1854);

				skull.Movable = false;
				skull.Light = LightType.Circle150;

				skull.MoveToWorld(GetWhiteSkullLocation(i), Map);

				m_WhiteSkulls.Add(skull);

				Effects.PlaySound(skull.Location, skull.Map, 0x29);
				Effects.SendLocationEffect(new Point3D(skull.X + 1, skull.Y + 1, skull.Z), skull.Map, 0x3728, 10);
			}
		}

		public Point3D GetRedSkullLocation(int index)
		{
			int x, y;

			if (index < 5)
			{
				x = index - 2;
				y = -2;
			}
			else if (index < 9)
			{
				x = 2;
				y = index - 6;
			}
			else if (index < 13)
			{
				x = 10 - index;
				y = 2;
			}
			else
			{
				x = -2;
				y = 14 - index;
			}

			return new Point3D(X + x, Y + y, Z - 15);
		}

		public Point3D GetWhiteSkullLocation(int index)
		{
			int x, y;

			switch (index)
			{
				default:
				case 0: x = -1; y = -1; break;
				case 1: x = 1; y = -1; break;
				case 2: x = 1; y = 1; break;
				case 3: x = -1; y = 1; break;
			}

			return new Point3D(X + x, Y + y, Z - 15);
		}

		public virtual bool CanChampionSpawn()
		{
			if (m_iMainQueue > 0)
				return true;

			return false;
		}

		public virtual void OnBeforeChampionSpawn()
		{
			m_iMainQueue--;
			UpdateCandles();
		}

		public virtual void OnChampionSpawn()
		{
			CheckAltarHue();
		}

		public virtual void OnChampionDeath()
		{
			CheckAltarHue();
		}

		public virtual void CheckSpawn()
		{
			if (!m_Active || Deleted)
				return;

			if ((m_Champion == null || m_Champion.Deleted) && CanChampionSpawn())
			{
				OnBeforeChampionSpawn();

				if (m_PreCreatedChampion != null)
				{
					m_PreCreatedChampion.MoveToWorld(new Point3D(X, Y, Z - 15), Map);
					m_PreCreatedChampion.Home = Location;
					m_PreCreatedChampion.Spawner = this;
					Champion = m_PreCreatedChampion;
					m_PreCreatedChampion = null;
				}
				else if (ChampionType != null)
				{
					object o = Activator.CreateInstance(ChampionType);
					if (o is BaseCreature)
					{
						BaseCreature champion = (BaseCreature)o;
						champion.MoveToWorld(new Point3D(X, Y, Z - 15), Map);
						champion.Home = Location;
						champion.Spawner = this;
						Champion = champion;
					}
				}

				OnChampionSpawn();
			}
		}

		public override void OnLocationChange(Point3D oldLoc)
		{
			if (Deleted)
				return;

			if (m_Platform != null)
				m_Platform.Location = new Point3D(X, Y, Z - 20);

			if (m_Altar != null)
				m_Altar.Location = new Point3D(X, Y, Z - 15);

			if (m_RedSkulls != null)
			{
				for (int i = 0; i < m_RedSkulls.Count; ++i)
					((Item)m_RedSkulls[i]).Location = GetRedSkullLocation(i);
			}

			if (m_WhiteSkulls != null)
			{
				for (int i = 0; i < m_WhiteSkulls.Count; ++i)
					((Item)m_WhiteSkulls[i]).Location = GetWhiteSkullLocation(i);
			}
		}

		public override void OnMapChange()
		{
			if (Deleted)
				return;

			if (m_Platform != null)
				m_Platform.Map = Map;

			if (m_Altar != null)
				m_Altar.Map = Map;

			if (m_RedSkulls != null)
			{
				for (int i = 0; i < m_RedSkulls.Count; ++i)
					((Item)m_RedSkulls[i]).Map = Map;
			}

			if (m_WhiteSkulls != null)
			{
				for (int i = 0; i < m_WhiteSkulls.Count; ++i)
					((Item)m_WhiteSkulls[i]).Map = Map;
			}
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if (m_Platform != null)
				m_Platform.Delete();

			if (m_Altar != null)
				m_Altar.Delete();

			if (m_RedSkulls != null)
			{
				for (int i = 0; i < m_RedSkulls.Count; ++i)
					((Item)m_RedSkulls[i]).Delete();

				m_RedSkulls.Clear();
			}

			if (m_WhiteSkulls != null)
			{
				for (int i = 0; i < m_WhiteSkulls.Count; ++i)
					((Item)m_WhiteSkulls[i]).Delete();

				m_WhiteSkulls.Clear();
			}
		}

		public BaseSummoningAltar(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version

			writer.Write((Mobile)m_PreCreatedChampion);
			writer.Write((Mobile)m_Champion);
			writer.Write(m_Platform);
			writer.Write(m_Altar);
			writer.Write((bool)m_Active);
			writer.Write(m_iMainQueue);

			writer.WriteItemList(m_RedSkulls, true);
			writer.WriteItemList(m_WhiteSkulls, true);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			m_PreCreatedChampion = reader.ReadMobile() as BaseCreature;
			m_Champion = reader.ReadMobile() as BaseCreature;
			m_Platform = reader.ReadItem() as BasePlatform;
			m_Altar = reader.ReadItem() as BaseAltar;
			m_Active = reader.ReadBool();
			m_iMainQueue = reader.ReadInt();

			m_RedSkulls = reader.ReadStrongItemList();
			m_WhiteSkulls = reader.ReadStrongItemList();

			if (m_Champion != null && !m_Champion.Deleted)
				m_Champion.Spawner = this;

			if (m_Platform == null || m_Altar == null)
				Delete();
			else
				CheckSpawn();
		}

		#region ISpawner Functions
		public bool UnlinkOnTaming { get { return true; } }
		public Point3D Home { get { return Location; } }
		public int Range { get { return 10; } }
		Region ISpawner.Region{ get{ return Region.Find( Location, Map ); } }

		void ISpawner.Remove(object spawn)
		{
			m_Champion = null;
			OnChampionDeath();
			CheckSpawn();
		}
		#endregion
	}
}