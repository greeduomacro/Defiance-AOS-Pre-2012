using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using System.Data;
using System.IO;
using Server.Commands;

namespace Server.Mobiles
{
	public class ESpawnerXMLFunctions
	{
		private static string SpawnDataSetName = "Spawns";
		private static string SpawnTablePointName = "Spawner";

		private static int m_iNormalHue = 55, m_iErrorHue = 33;

		public static void Initialize()
		{
			CommandSystem.Register("ELoadSpawn", AccessLevel.Administrator, new CommandEventHandler(ELoadSpawn_OnCommand));
			CommandSystem.Register("ESaveSpawn", AccessLevel.Administrator, new CommandEventHandler(ESaveSpawn_OnCommand));
		}

		[Usage("ELoadSpawn [fileName]")]
		[Description("")]
		private static void ELoadSpawn_OnCommand(CommandEventArgs e)
		{
			string fileName;

			if (e.Length > 0)
			{
				fileName = e.GetString(0);
				LoadSpawns(e.Mobile, fileName);
			}
		}

		[Usage("ESaveSpawn")]
		[Description("")]
		private static void ESaveSpawn_OnCommand(CommandEventArgs e)
		{
			SaveSpawns(e.Mobile);
		}

		private static string CreateSpawnString(ESpawner spawner)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			for (int i = 0; i < spawner.SpawnEntries.Count; i++)
			{
				EclSpawnEntry entry = (EclSpawnEntry)spawner.SpawnEntries[i];

				if (sb.Length > 0)
					sb.Append(':');

				sb.AppendFormat("{0}={1}", entry.SpawnObjectName, entry.Amount);
			}
			return sb.ToString();
		}

		public static void LoadSpawns(Mobile from, string fileName)
		{
			from.SendMessage(m_iNormalHue, "Loading spawners to the world... from the {0} file.", fileName);

			if (System.IO.File.Exists(fileName) == true)
			{
				FileStream fs = null;
				try { fs = File.Open(fileName, FileMode.Open); }
				catch { from.SendMessage(m_iErrorHue, "Error opening xml file."); return; }

				int TotalCount = 0;

				DataSet ds = new DataSet(SpawnDataSetName);

				try { ds.ReadXml(fs); }
				catch { from.SendMessage(m_iErrorHue, "Error reading xml file."); return; }

				if (ds.Tables != null && ds.Tables.Count > 0)
				{
					if (ds.Tables[SpawnTablePointName] != null && ds.Tables[SpawnTablePointName].Rows.Count > 0)
						foreach (DataRow dr in ds.Tables[SpawnTablePointName].Rows)
						{
							int iX = 0;
							int iY = 0;
							int iZ = 0;
							Map SpawnMap = null;
							int iMaxAmount = 0;
							int iHomeRange = 0;
							bool iRunning = false;
							int iTeam = 0;
							bool bGroup = false;
							TimeSpan SpawnMinDelay = TimeSpan.FromMinutes(5);
							TimeSpan SpawnMaxDelay = TimeSpan.FromMinutes(10);
							int iContSerial = 0;
							AccessLevel visibilityLevel = 0;
							string sSpawn = "";
							bool bIgnoreWorldSpawn;
							bool bTryFlip = false;
							int iWayPoint = 0;

							try
							{
								iX = int.Parse((string)dr["X"]);
								iY = int.Parse((string)dr["Y"]);
								iZ = int.Parse((string)dr["Z"]);
								SpawnMap = Map.Parse((string)dr["Map"]);
								iMaxAmount = int.Parse((string)dr["MaxAmount"]);
								iHomeRange = int.Parse((string)dr["HomeRange"]);
								iRunning = bool.Parse((string)dr["Running"]);
								iTeam = int.Parse((string)dr["Team"]);
								bGroup = bool.Parse((string)dr["Group"]);
								SpawnMinDelay = TimeSpan.FromSeconds(int.Parse((string)dr["MinDelay"]));
								SpawnMaxDelay = TimeSpan.FromSeconds(int.Parse((string)dr["MaxDelay"]));
								iContSerial = int.Parse((string)dr["ContSerial"]);
								visibilityLevel = (AccessLevel)int.Parse((string)dr["VisibilityLevel"]);
								sSpawn = (string)dr["Spawn"];
								bIgnoreWorldSpawn = bool.Parse((string)dr["IgnoreWorldSpawn"]);
								bTryFlip = bool.Parse((string)dr["TryFlip"]);
								iWayPoint = int.Parse((string)dr["WayPoint"]);
							}
							catch { from.SendMessage(m_iErrorHue, "Error trying to load."); return; }

							TotalCount++;

							List<EclSpawnEntry> alSpawn = new List<EclSpawnEntry>();

							if (sSpawn != null && sSpawn.Length > 0)
							{
								string[] SpawnsSpawn = sSpawn.Split(':');

								for (int i = 0; i < SpawnsSpawn.Length; i++)
								{
									string[] SpawnObjectDetails = SpawnsSpawn[i].Split('=');

									alSpawn.Add(new EclSpawnEntry(SpawnObjectDetails[0], new ArrayList(), int.Parse(SpawnObjectDetails[1])));
								}
							}

							ESpawner spawner = new ESpawner(iMaxAmount, SpawnMinDelay, SpawnMaxDelay, iTeam, bGroup, iHomeRange, alSpawn, iMaxAmount > 0 ? true : false, visibilityLevel);
							spawner.TryFlip = bTryFlip;
							spawner.IgnoreWorldSpawn = bIgnoreWorldSpawn;

							if (iWayPoint != 0)
							{
								bool bFound = false;
								foreach (Item i in World.Items.Values)
								{
									if (i is WayPoint && ((WayPoint)i).Serial.Value == iWayPoint)
									{
										spawner.WayPoint = (WayPoint)i;
										spawner.MoveToWorld(new Point3D(iX, iY, iZ), SpawnMap);
										bFound = true;
									}
								}
								if (!bFound) // Error... waypoint not found
								{
									spawner.Delete();
									from.SendMessage(m_iErrorHue, "Could not find the WayPoint for the spawner... Spawner deleted.");
									continue;
								}
							}
							else
								spawner.MoveToWorld(new Point3D(iX, iY, iZ), SpawnMap);

							if (iContSerial != 0)
							{
								bool bFound = false;
								foreach (Item i in World.Items.Values)
								{
									if (i is Container && ((Container)i).Serial.Value == iContSerial)
									{
										((Container)i).DropItem(spawner);
										bFound = true;
									}
								}
								if (!bFound) // Error... container not found
								{
									spawner.Delete();
									from.SendMessage(m_iErrorHue, "Could not find the parent container for the spawner... Spawner deleted.");
									continue;
								}
							}
							else
								spawner.MoveToWorld(new Point3D(iX, iY, iZ), SpawnMap);
						}
				}
				fs.Close();
				from.SendMessage(m_iNormalHue, string.Format("Done... Loaded {0} spawners from {1}", TotalCount, fileName));
			}
			else
				from.SendMessage(m_iErrorHue, "Could not find file {0}.", fileName);
		}

		public static void SaveSpawns(Mobile from)
		{
			from.SendMessage(m_iNormalHue, "Saving spawners to Xml file...");

			int iSpawnerCount = 0;
			int TotalCount = 0;
			string sMapName = "";

			DataSet ds = new DataSet(SpawnDataSetName);
			ds.Tables.Add(SpawnTablePointName);

			ds.Tables[SpawnTablePointName].Columns.Add("X");
			ds.Tables[SpawnTablePointName].Columns.Add("Y");
			ds.Tables[SpawnTablePointName].Columns.Add("Z");
			ds.Tables[SpawnTablePointName].Columns.Add("Map");

			ds.Tables[SpawnTablePointName].Columns.Add("MaxAmount");
			ds.Tables[SpawnTablePointName].Columns.Add("HomeRange");
			ds.Tables[SpawnTablePointName].Columns.Add("Running");
			ds.Tables[SpawnTablePointName].Columns.Add("Team");
			ds.Tables[SpawnTablePointName].Columns.Add("Group");

			ds.Tables[SpawnTablePointName].Columns.Add("MinDelay");
			ds.Tables[SpawnTablePointName].Columns.Add("MaxDelay");

			ds.Tables[SpawnTablePointName].Columns.Add("ContSerial");
			ds.Tables[SpawnTablePointName].Columns.Add("VisibilityLevel");

			ds.Tables[SpawnTablePointName].Columns.Add("IgnoreWorldSpawn");
			ds.Tables[SpawnTablePointName].Columns.Add("TryFlip");
			ds.Tables[SpawnTablePointName].Columns.Add("WayPoint");

			ds.Tables[SpawnTablePointName].Columns.Add("Spawn");

			foreach (Map map in Map.AllMaps)
			{
				if (map == Map.Internal || map == null)
					continue;

				sMapName = map.Name + "-ESS.xml";
				iSpawnerCount = 0;

				ds.Clear();

				foreach (Item item in World.Items.Values)
				{
					if (item is ESpawner && !item.Deleted && item.Map == map)
					{
						ESpawner spawner = (ESpawner)item;

						DataRow dr = ds.Tables[SpawnTablePointName].NewRow();

						dr["X"] = (int)spawner.X;
						dr["Y"] = (int)spawner.Y;
						dr["Z"] = (int)spawner.Z;
						dr["Map"] = (string)spawner.Map.Name;

						dr["MaxAmount"] = (int)(spawner.UseMaxAmount ? spawner.MaxAmount : 0);
						dr["HomeRange"] = (int)spawner.HomeRange;
						dr["Running"] = (bool)spawner.Running;
						dr["Team"] = (int)spawner.Team;
						dr["Group"] = (bool)spawner.Group;

						dr["MinDelay"] = (int)spawner.MinDelay.TotalSeconds;
						dr["MaxDelay"] = (int)spawner.MaxDelay.TotalSeconds;

						if (spawner.Parent is Container)
							dr["ContSerial"] = (int)((Container)spawner.Parent).Serial.Value;
						else
							dr["ContSerial"] = (int)0;

						dr["VisibilityLevel"] = (int)spawner.VisibilityLevel;

						dr["IgnoreWorldSpawn"] = (bool)spawner.IgnoreWorldSpawn;
						dr["TryFlip"] = (bool)spawner.TryFlip;

						if (spawner.WayPoint != null)
							dr["WayPoint"] = (int)spawner.WayPoint.Serial.Value;
						else
							dr["WayPoint"] = (int)0;

						dr["Spawn"] = (string)CreateSpawnString(spawner);

						ds.Tables[SpawnTablePointName].Rows.Add(dr);

						TotalCount++;
						iSpawnerCount++;
					}
				}

				if (iSpawnerCount > 0)
				{
					try { ds.WriteXml(sMapName); }
					catch { from.SendMessage(m_iErrorHue, "Error trying to save."); return; }

					from.SendMessage(m_iNormalHue, string.Format("Done... Saved {0} spawners to {1}", iSpawnerCount, sMapName));
				}
				else
					from.SendMessage(m_iNormalHue, string.Format("Could not find any spawners on the {0} map to save.", map.Name));
			}
		}
	}
}