//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2006					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using Server.Items;
using Server.Network;

namespace Server.Multis.CustomBuilding
{
	public class FileBasedBuilding : BaseBuilding
	{
		#region Members
		public static SeperateSaveData SeperateData = new SeperateSaveData(Config.Location, "FileBasedBuildings", new DC.SaveMethod(WriteBuildingTable), new DC.LoadMethod(LoadBuildings));
		private static Dictionary<string,BuildingEntry> m_BuildingTable = new Dictionary<string,BuildingEntry>();
		public static Dictionary<string, BuildingEntry> BuildingTable { get { return m_BuildingTable; } set { m_BuildingTable = value; } }

		private BuildingEntry m_Entry;

		public BuildingEntry Entry
		{
			get
			{
				if (m_Entry == null)
					m_BuildingTable.TryGetValue(Name, out m_Entry);

				return m_Entry;
			}
		}

		public override int GetUpdateRange(Mobile m)
		{
			if (Entry != null)
				return Entry.IsSmall ? 18 : 22;

			return 18;
		}
		public override MultiComponentList Components
		{
			get
			{
				if (Entry != null)
					return Entry.ComponentList;

				return
					BaseBuilding.EmptyList;
			}
		}


		#endregion

		[Constructable]
		public FileBasedBuilding(string name) : base()
		{
			Name = name;
			m_BuildingTable.TryGetValue(name, out m_Entry);
		}

		#region Import/Export
		public static bool ImportBuilding(string filename)
		{
			bool finished = false;
			string binpath = Path.Combine(BaseBuilding.SavePath, filename + ".bin");

			if (File.Exists(binpath))
			{
				using (FileStream bin = CustomSaving.GetFileStream(binpath))
				{
					BinaryFileReader reader = new BinaryFileReader(new BinaryReader(bin));
					try
					{
						int count = reader.ReadInt();
						for (int i = 0; i < count; i++)
						{
							string name = reader.ReadString();
							int type = reader.ReadInt();

							switch (type)
							{
								case 0: m_BuildingTable[name] = new BuildingEntry(reader, type); break;
								case 1: m_BuildingTable[name] = new ComponentEntry(reader); break;
								case 2: m_BuildingTable[name] = new AddonEntry(reader); break;
							}
						}
						finished = true;
					}
					catch (Exception error)
					{
						Console.WriteLine(error.ToString());
					}
					finally
					{
						reader.Close();
					}
				}
			}
			return finished;
		}

		public static bool ExportBuilding(string name, Mobile m)
		{
			bool found = false;
			GenericWriter writer = BaseBuilding.GetWriter(BaseBuilding.SavePath, name + ".bin");

			BuildingEntry entry = null;

			if (m_BuildingTable.TryGetValue(name, out entry))
			{
				found = true;
				if (entry is AddonEntry)
				{
					List<string> list = ((AddonEntry)entry).Components;
					writer.Write(list.Count + 1);

					foreach (string str in list)
					{
						BuildingEntry subentry = null;

						if (m_BuildingTable.TryGetValue(str, out subentry))
						{
							writer.Write(str);
							writer.Write(subentry.BuildType);
							subentry.Write(writer);
						}
					}
				}

				else
					writer.Write(1);

				writer.Write(name);
				writer.Write(entry.BuildType);
				entry.Write(writer) ;
			}

			writer.Close();

			m.SendMessage(found? "The building has been succesfully exported.":"The buildingtype could not be found.");
			return found;
		}
		#endregion

		#region Save/Load Components

		public static void LoadBuildings(GenericReader reader)
		{
			try
			{
				m_BuildingTable = new Dictionary<string,BuildingEntry>();

				int count = reader.ReadInt();
				for (int i = 0; i < count; i++)
				{
					string name = reader.ReadString();
					int type = reader.ReadInt();

					switch (type)
					{
						case 0: m_BuildingTable[name] = new BuildingEntry(reader, type); break;
						case 1: m_BuildingTable[name] = new ComponentEntry(reader); break;
						case 2: m_BuildingTable[name] = new AddonEntry(reader); break;
					}
				}
			}

			catch (Exception err)
			{
				m_BuildingTable.Clear();
				Console.WriteLine(err.ToString());
			}
		}

		public static void WriteBuildingTable(GenericWriter writer)
		{
			writer.Write( m_BuildingTable.Count);

			foreach (KeyValuePair<string, BuildingEntry> de in m_BuildingTable)
			{
				writer.Write(de.Key);
				writer.Write(de.Value.BuildType);
				de.Value.Write(writer);
			}
		}
		#endregion

		#region Serialisation
		public FileBasedBuilding(Serial serial) : base(serial)
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
		#endregion
	}

	#region Entry Support Classes
	public class AddonEntry : BuildingEntry
	{
		public List<string> Components;

		public AddonEntry(MultiComponentList list, List<string> components, bool small)
			: base(2, list, small)
		{
			Components = components;
		}

		public AddonEntry(GenericReader reader)
			: base(reader, 2)
		{
			int version = reader.ReadInt();
			Components = CustomSaving.DeserializeStringList(reader);
		}

		public override void Write(GenericWriter writer)
		{
			base.Write(writer);

			writer.Write(0);//version
			CustomSaving.SerializeStringList(Components, writer);
		}
	}

	public class ComponentEntry : BuildingEntry
	{
		public int X_Offset;
		public int Y_Offset;

		public ComponentEntry(MultiComponentList list, bool small, int xoff, int yoff)
			: base(1, list, small )
		{
			X_Offset = xoff;
			Y_Offset = yoff;
		}

		public ComponentEntry(GenericReader reader)
			: base(reader, 1)
		{
			int version = reader.ReadInt();
			X_Offset = reader.ReadInt();
			Y_Offset = reader.ReadInt();
		}

		public override void Write(GenericWriter writer)
		{
			base.Write(writer);

			writer.Write(0);//version
			writer.Write(X_Offset);
			writer.Write(Y_Offset);
		}
	}

	public class BuildingEntry
	{
		public MultiComponentList ComponentList;
		public int BuildType;
		public bool IsSmall;

		public BuildingEntry(int type, MultiComponentList list, bool small)
		{
			ComponentList = list;
			BuildType = type;
			IsSmall = small;
		}

		public BuildingEntry(GenericReader reader, int type)
		{
			BuildType = type;

			int version = reader.ReadInt();
			ComponentList = new MultiComponentList(reader);
			IsSmall = reader.ReadBool();
			if (version == 0)
				for (int i = ComponentList.List.Length - 1; i >= 0; i--)
					ComponentList.List[i].m_OffsetZ += 42;
		}

		public virtual void Write(GenericWriter writer)
		{
			writer.Write(1);//version
			ComponentList.Serialize(writer);
			writer.Write(IsSmall);
		}
	}
	#endregion
}