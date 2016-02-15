//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2006					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Network;
using Server.Commands;

namespace Server.Multis.CustomBuilding
{
	public static class BuildingCreator
	{
		public static void CreateBuilding(Mobile from, Map map, Point3D start, Point3D end, object state)
		{
			DateTime started = DateTime.Now;
			from.SendMessage("Building Creation Started:");

			object[] objarr = (object[])state;
			string name = (string)objarr[0];
			bool scriptbased = (bool)objarr[1];
			bool staticsonly = (bool)objarr[2];
			end = new Point3D(end.X + 1, end.Y + 1, end.Z);
			List<Item> staticlist = new List<Item>();
			IPooledEnumerable itemlist = map.GetItemsInBounds(new Rectangle2D(start, end));

			int lowx = end.X;
			int lowy = end.Y;
			int highx = start.X;
			int highy = start.Y;
			int lowz = 150;

			foreach (Item item in itemlist)
			{
				if (!staticsonly || item is Static)
				{
					staticlist.Add(item);
					lowx = Math.Min(lowx, item.Location.X);
					lowy = Math.Min(lowy, item.Location.Y);
					lowz = Math.Min(lowz, item.Location.Z);
					highx = Math.Max(highx, item.Location.X);
					highy = Math.Max(highy, item.Location.Y);
				}
			}

			int totparts = 0;

			if (staticlist.Count != 0)
			{
				lowz -= 51;// was 9
				int totwidth = Math.Max(highx - lowx, 1);
				int totheight = Math.Max(highy - lowy, 1);
				int tothorparts = (int)Math.Ceiling((double)(totwidth / 18.0));
				int totvertparts = (int)Math.Ceiling((double)(totheight / 19.0));
				totparts = tothorparts * totvertparts;

				MultiTileEntry[][][] dataarray = new MultiTileEntry[tothorparts][][];
				bool[][] issmallarray = new bool[tothorparts][];

				for (int i = 0; i < tothorparts; i++)
				{
					dataarray[i] = new MultiTileEntry[totvertparts][];
					issmallarray[i] = new bool[totvertparts];
				}

				for (int i = 0; i < tothorparts; i++)
				{
					for (int j = 0; j < totvertparts; j++)
					{
						List<MultiTileEntry> partlist = new List<MultiTileEntry>();
						int lowpartx = 100;
						int lowparty = 100;
						int highpartx = -100;
						int highparty = -100;

						int minx = lowx + (i * 18);
						int miny = lowy + (j * 19);
						int maxx = minx + 18;
						int maxy = miny + 19;
						int centerx = minx + 8;
						int centery = miny + 8;

						if (totwidth < 18 && totheight < 19)
						{
							centerx = minx + (totwidth / 2);
							centery = miny + (totheight / 2);
						}

						foreach (Item stat in staticlist)
						{
							Point3D loc = stat.Location;

							if (loc.X >= minx && loc.X < maxx && loc.Y >= miny && loc.Y < maxy)
							{
								short x = (short)(stat.X - centerx);
								short y = (short)(stat.Y - centery);
								short z = (short)(stat.Z - lowz);
								partlist.Add(new MultiTileEntry((short)stat.ItemID, x, y, z, 1));

								lowpartx = Math.Min(lowpartx, x);
								lowparty = Math.Min(lowparty, y);
								highpartx = Math.Max(highpartx, x);
								highparty = Math.Max(highparty, y);
							}
						}

						dataarray[i][j] = partlist.ToArray();
						issmallarray[i][j] = ((highpartx - lowpartx) < 16) && ((highparty - lowparty) < 16);
					}
				}
				itemlist.Free();

				Point2D min = new Point2D(-8, -8);
				Point2D max = new Point2D(9, 10);
				Point2D center = new Point2D(8, 8);
				string classname = "Server.Multis." + name;

				if (dataarray.Length == 1 && dataarray[0].Length == 1)
				{
					if (scriptbased)
					{
						string output = Templates.SBBTemplate.Replace("{name}", name);
						output = output.Replace("{namen}", String.Format("\"{0}\"", name));
						output = output.Replace("{addtiles}", GetTileString(dataarray[0][0]));
						output = output.Replace("{updaterange}", issmallarray[0][0]? "18" : "22");
						WriteScript(name, output);
					}
					else
						FileBasedBuilding.BuildingTable[name] = new BuildingEntry(0, GetComponentList(dataarray[0][0]), issmallarray[0][0]);
				}

				else
				{
					if (!scriptbased)
						for (int i = 1; i < totparts; i++)
						{
							string countedname = name +"_"+ i;

							if (FileBasedBuilding.BuildingTable.ContainsKey(name))
							{
								from.SendMessage("One of the names would conflict with an existing building, please choose a different name or remove the existing type.");
								return;
							}
						}

					int number = 1;
					List<string> stringlist = new List<string>();
					System.Text.StringBuilder components = new System.Text.StringBuilder();

					for (int x = 0; x < dataarray.Length; x++)
					{
						for (int y = 0; y < dataarray[x].Length; y++)
						{
							if (x == 0 && y == 0)
								continue;

							else
							{
								string countedname = name + "_" + number;

								if (scriptbased)
								{
									components.AppendFormat("\n			AddComponent( new {0}(), {1}, {2});", countedname, x * 18, y * 19);

									string output = Templates.SBBATemplate.Replace("{name}", countedname);
									output = output.Replace("{namen}", String.Format("\"{0}\"", countedname));
									output = output.Replace("{addtiles}", GetTileString(dataarray[x][y]));
									output = output.Replace("{updaterange}", issmallarray[x][y] ? "18" : "22");

									WriteScript(countedname, output);
								}

								else
								{
									stringlist.Add(countedname);
									FileBasedBuilding.BuildingTable[countedname] = new ComponentEntry(GetComponentList(dataarray[x][y]), issmallarray[x][y], x * 18, y * 19);
								}

								number++;
							}
						}
					}

					if (scriptbased)
					{
						string output = Templates.SBABTemplate.Replace("{name}", name);
						output = output.Replace("{namen}", String.Format("\"{0}\"", name));
						output = output.Replace("{addcomponents}", components.ToString());
						output = output.Replace("{addtiles}", GetTileString(dataarray[0][0]));
						output = output.Replace("{updaterange}", issmallarray[0][0] ? "18" : "22");

						WriteScript(name, output);
					}

					else
						FileBasedBuilding.BuildingTable[name] = new AddonEntry(GetComponentList(dataarray[0][0]), stringlist,issmallarray[0][0]);
				}
				if (!scriptbased)
					FileBasedBuilding.SeperateData.Save();
			}

			TimeSpan duration = DateTime.Now - started;
			from.SendMessage("A total of {0} buildings have been created", totparts);
			from.SendMessage("The proces took {0} seconds", duration.TotalSeconds);
			from.SendMessage("Building Creation Ended");
		}

		private static MultiComponentList GetComponentList(MultiTileEntry[] array)
		{
			MultiComponentList mcl = new MultiComponentList(BaseBuilding.EmptyList);
			foreach (MultiTileEntry entry in array)
				mcl.Add(entry.m_ItemID, entry.m_OffsetX, entry.m_OffsetY, entry.m_OffsetZ);

			return mcl;
		}

		private static string GetTileString(MultiTileEntry[] array)
		{
			System.Text.StringBuilder tiles = new System.Text.StringBuilder();
			foreach (MultiTileEntry entry in array)
				tiles.AppendFormat("\n			m_Components.Add({0}, {1}, {2}, {3});", entry.m_ItemID, entry.m_OffsetX, entry.m_OffsetY, entry.m_OffsetZ);

			return tiles.ToString();
		}

		private static void WriteScript(string name, string output)
		{
			if (!Directory.Exists(BaseBuilding.SavePath))
				Directory.CreateDirectory(BaseBuilding.SavePath);

			StreamWriter streamwriter = new StreamWriter(BaseBuilding.SavePath + "\\" + name + ".cs", false);

			try
			{
				streamwriter.Write(output);
			}
			catch (Exception err)
			{
				Console.WriteLine(err.ToString());
			}
			finally
			{
				streamwriter.Close();
			}
		}
	}
}