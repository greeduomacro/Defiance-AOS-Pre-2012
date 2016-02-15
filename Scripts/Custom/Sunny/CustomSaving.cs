using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Server
{
	public static class CustomSaving
	{
		/// <summary>
		/// If set to true you will not be asked to remove the module or stop loading
		/// when encountering an error while loading your data.
		/// </summary>
		private static bool m_IgnoreErrors = false;

		#region Members
		private static string m_FilePath = "Saves/SunnySystem";
		private static string m_FileName = "SunnySystem";
		private static string m_FullPath { get { return Path.Combine(m_FilePath, m_FileName); } }
		private static Dictionary<string, SaveData> m_DataDictionary = new Dictionary<string, SaveData>();
		private static List<SeperateSaveData> m_SeperateDataList = new List<SeperateSaveData>();
		#endregion

		#region Main Methods
		public static event CustomSaveLoadEventHandler CustomSaveLoad;

		private static void InvokeCustomSaveLoad()
		{
			if (CustomSaveLoad != null)
				CustomSaveLoad();
		}

		public static void Configure()
		{
			EventSink.WorldSave += new WorldSaveEventHandler(OnShardSave);
			EventSink.WorldLoad += new WorldLoadEventHandler(OnShardLoad);
		}

		public static void AddSaveModule(SaveData sd, string name)
		{
			if (name == "")
				Console.WriteLine("You have to give the save module a name.");

			else if (m_DataDictionary.ContainsKey(name))
				Console.WriteLine("The custom save system already contains a module with the name \"{0}\".", name);

			else
				m_DataDictionary[name] = sd;
		}

		public static void AddSeperateSave(SeperateSaveData ssd){ m_SeperateDataList.Add(ssd); }

		private static void OnShardSave(WorldSaveEventArgs e) { CustomSave(); }

		private static void OnShardLoad()
		{
			CustomLoad();

			foreach (SeperateSaveData ssd in m_SeperateDataList)
				CustomSeperateLoad(ssd);

			InvokeCustomSaveLoad();
		}
		#endregion

		#region Support Methods
		public static FileStream GetFileStream(string file) { return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read); }

		private static void DirectoryCheck(string loc)
		{
			if (!Directory.Exists(loc))
				Directory.CreateDirectory(loc);
		}

		private static void HandleError(Exception error, string name, object[] loadinfo)
		{
			bool sep = loadinfo == null;
			bool nerr = error == null;
			string type = sep ? "seperate savefile" : "save module";
			string placename = sep ? "the file can be found at" : "this module was indexed under the name";


			Console.WriteLine();
			if (nerr)
			{
				Console.WriteLine("The loading and saving methods of a {0} are inconsistent, {1} \"{2}\".", type, placename, name);
				if (!sep && (bool)loadinfo[3])
					Console.WriteLine("More data was read than written.");
				else
					Console.WriteLine("More data was written than read.");
			}
			else
			{
				Console.WriteLine("During the loading of a {0} an exception was caught, {1} \"{2}\".", type, placename, name);
				Console.WriteLine("The following error was caught:");
				Console.WriteLine(error.ToString());
			}

			Console.WriteLine("Please Review your Save/Load methods for this {0}", sep ? "file" : "module");

			if (!m_IgnoreErrors)
			{
				string str = sep ? "Do you wish to continue loading with faulty data(Y), or stop the program(N)?" : "Do you wish to remove this module and restart(Y), or continue loading with faulty data(N)?";
				Console.WriteLine(str);

				if (Console.ReadKey(true).Key == ConsoleKey.Y)
				{
					if (!sep)
					{
						int oldcount = (int)loadinfo[0];
						int location = (int)loadinfo[1];
						BinaryFileReader idxreader = (BinaryFileReader)loadinfo[2];

						int newcount = oldcount - 1;
						string[] indexarray = new string[newcount];
						long[] binposarray = new long[newcount];
						long[] finposarray = new long[newcount];

						idxreader.Seek(0, SeekOrigin.Begin);
						idxreader.ReadInt();
						int loc = 0;
						for (int j = 0; j < oldcount; j++)
						{
							if (j != location)
							{
								indexarray[loc] = idxreader.ReadString();
								binposarray[loc] = idxreader.ReadLong();
								finposarray[loc] = idxreader.ReadLong();
								loc++;
							}
							else
							{
								idxreader.ReadString();
								idxreader.ReadLong();
								idxreader.ReadLong();
							}
						}
						idxreader.Close();
						GenericWriter idxwriter = new BinaryFileWriter(m_FullPath + ".idx", true);

						idxwriter.Write(newcount);
						for (int j = 0; j < newcount; j++)
						{
							idxwriter.Write(indexarray[j]);
							idxwriter.Write(binposarray[j]);
							idxwriter.Write(finposarray[j]);
						}
						idxwriter.Close();

						Process.Start(Core.ExePath, Core.Arguments);
						Core.Process.Kill();
					}
				}

				else if (sep)
					Core.Process.Kill();
			}
		}
		#endregion

		#region Saving
		public static void CustomSeperateSave(SeperateSaveData data)
		{
			GenericWriter writer = new BinaryFileWriter(Path.Combine(data.SaveLocation, data.SaveName + ".bin"), true);
			DirectoryCheck(data.SaveLocation);

			data.SaveMethod(writer);
			writer.Write(writer.Position);
			writer.Close();
		}

		private static void CustomSave()
		{
			DirectoryCheck(m_FilePath);

			GenericWriter idx = new BinaryFileWriter(m_FullPath + ".idx", true);
			GenericWriter bin = new BinaryFileWriter(m_FullPath + ".bin", true);

			idx.Write(m_DataDictionary.Count);
			foreach (KeyValuePair<string, SaveData> kv in m_DataDictionary)
			{
				idx.Write(kv.Key);
				idx.Write(bin.Position);
				kv.Value.SaveMethod(bin);
				idx.Write(bin.Position);
			}

			idx.Close();
			bin.Close();
		}
		#endregion

		#region Loading
		private static void CustomSeperateLoad(SeperateSaveData data)
		{
			string binpath = Path.Combine(data.SaveLocation, data.SaveName + ".bin");

			if (File.Exists(binpath))
			{
				using (FileStream bin = GetFileStream(binpath))
				{
					BinaryFileReader reader = new BinaryFileReader(new BinaryReader(bin));
					try
					{
						data.LoadMethod(reader);

						long endpos = reader.Position;
						reader.Seek(-8, SeekOrigin.End);
						if (reader.ReadLong() != endpos)
							HandleError(null, binpath, null);
					}
					catch (Exception error)
					{
						HandleError(error, binpath, null);
					}

					reader.Close();
				}
			}
		}

		private static void CustomLoad()
		{
			string binpath = m_FullPath + ".bin";
			string idxpath = m_FullPath + ".idx";

			if (File.Exists(binpath) && File.Exists(idxpath))
			{
				using (FileStream bin = GetFileStream(binpath))
				{
					BinaryFileReader binreader = new BinaryFileReader(new BinaryReader(bin));

					using (FileStream idx = GetFileStream(idxpath))
					{
						BinaryFileReader idxreader = new BinaryFileReader(new BinaryReader(idx));

						int loadmethodscount = idxreader.ReadInt();
						for (int i = 0; i < loadmethodscount; i++)
						{
							string index = idxreader.ReadString();
							long binpos = idxreader.ReadLong();

							SaveData sd;
							if (m_DataDictionary.TryGetValue(index, out sd))
							{
								try
								{
									binreader.Seek(binpos, SeekOrigin.Begin);
									sd.LoadMethod(binreader);
								}
								catch (Exception error)
								{
									HandleError(error, index, new object[] { loadmethodscount, i, idxreader });
								}

								long finpos = idxreader.ReadLong();
								if (binreader.Position != finpos)
									HandleError(null, index, new object[] { loadmethodscount, i, idxreader, binreader.Position > finpos });
							}
							else
							{
								idxreader.ReadLong();
								Console.WriteLine("A module failed to load, the module that could not be found was indexed under the name \"{0}\". Please Review your Save/Load methods for this module.", index);
							}
						}
						idxreader.Close();
					}
					binreader.Close();
				}
			}
		}
		#endregion

		#region Save Methods

		public static void SerializeStringList(List<string> list, GenericWriter writer)
		{
			writer.Write(list.Count);

			for (int i = 0; i < list.Count; i++)
				writer.Write(list[i]);
		}

		public static List<string> DeserializeStringList(GenericReader reader)
		{
			List<string> list = new List<string>();

			int count = reader.ReadInt();
			for (int i = 0; i < count; i++)
				list.Add(reader.ReadString());

			return list;
		}

		public static void SerializeStringArray(string[] array, GenericWriter writer)
		{
			writer.Write(array.Length);

			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		public static string[] DeserializeStringArray(GenericReader reader)
		{
			int length = reader.ReadInt();
			string[] array = new string[length];

			for (int i = 0; i < length; i++)
				array[i] = reader.ReadString();

			return array;
		}

		public static void SerializeBoolArray(bool[] array, GenericWriter writer)
		{
			writer.Write(array.Length);

			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		public static bool[] DeserializeBoolArray(GenericReader reader)
		{
			int length = reader.ReadInt();
			bool[] array = new bool[length];

			for (int i = 0; i < length; i++)
				array[i] = reader.ReadBool();

			return array;
		}
		#endregion
	}

	#region Support classes
	/// <summary>
	/// DelegateClass(DC), delegates are created in this class.
	/// </summary>
	public static class DC
	{
		public delegate void SaveMethod(GenericWriter writer);
		public delegate void LoadMethod(GenericReader reader);
	}

	/// <summary>
	/// Class to pass and hold the methods of savemodules.
	/// </summary>
	public class SaveData
	{
		public DC.SaveMethod SaveMethod;
		public DC.LoadMethod LoadMethod;

		public SaveData(DC.SaveMethod save, DC.LoadMethod load)
		{
			SaveMethod = save;
			LoadMethod = load;
		}
	}

	/// <summary>
	/// Delegate called after customs loaded
	/// </summary>
	public delegate void CustomSaveLoadEventHandler();

	/// <summary>
	/// Class to to pass and save seperate savefiles.
	/// </summary>
	public class SeperateSaveData : SaveData
	{
		public string SaveLocation;
		public string SaveName;

		public SeperateSaveData(string loc, string name, DC.SaveMethod save, DC.LoadMethod load) : base(save, load)
		{
			SaveLocation = loc;
			SaveName = name;
			CustomSaving.AddSeperateSave(this);
		}

		public void Save()
		{
			CustomSaving.CustomSeperateSave(this);
		}
	}
	#endregion
}