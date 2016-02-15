using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Commands;

namespace Server.Gumps
{
	public class ESCGump : BaseListGump
	{
		public static void Initialize()
		{
			CommandSystem.Register("ESC", AccessLevel.Administrator, new CommandEventHandler(ESC_OnCommand));
		}

		[Usage( "EInternal" )]
		[Description( "" )]
		private static void ESC_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendGump( new ESCGump() );
		}

		public override string GumpName{ get{ return "Eclipse Spawner Control"; } }
		public override Version VerNum{ get{ return new Version(1,1,2); } }
		public override bool UseButton1{ get{ return true; } }
		public override bool UseButton2{ get{ return true; } }
		public override bool UseFilter{ get{ return true; } }

		public ESCGump() : this( PopulateList(""), 0, "" )
		{
		}

		public ESCGump(ArrayList alList, int loc, string sFilter) : base(alList, loc, sFilter)
		{
			Init();
		}

		public override void Init()
		{
			base.Init();
			this.AddBackground(549, 235, 221, 346, 9200);

			this.AddLabel(563, 260, 0, @"Total Respawn All In List");
			this.AddButton(719, 260, 4008, 4009, (int)13, GumpButtonType.Reply, 0);

			this.AddLabel(563, 300, 0, @"Delete All In List");
			this.AddButton(719, 300, 4008, 4009, (int)16, GumpButtonType.Reply, 0);
		}

		public override string OnPopulateStringList(object obj, int loc)
		{
			if( obj is ESpawner )
			{
				string textEntry = "";

				if (((ESpawner)obj).SpawnEntries == null || ((ESpawner)obj).SpawnEntries.Count <= 0)
					return string.Format("E #{0} *Empty*<BR>", loc.ToString() );

				foreach (EclSpawnEntry entry in ((ESpawner)obj).SpawnEntries)
				{
					if (entry.SpawnObjectName != null)
					{
						if (textEntry.Length + entry.SpawnObjectName.Length >= 35)
						{
							textEntry += "...";
							break;
						}
						else
							textEntry += string.Format("{0} {1}", textEntry.Length > 0 ? "," : "", entry.SpawnObjectName);
					}
				}

				return string.Format("E #{0} Map:{1} Spawn:{2}<BR>", loc.ToString(), ((ESpawner)obj).Map.Name, textEntry);
			}
/*
			else if (obj is Spawner)
			{
				string textEntry = "";

				if (((Spawner)obj).CreaturesName == null || ((Spawner)obj).CreaturesName.Count <= 0)
					return string.Format("R #{0} *Empty*<BR>", loc.ToString());

				foreach (string sCreatureName in ((Spawner)obj).CreaturesName)
				{
					if (textEntry.Length + sCreatureName.Length >= 35)
					{
						textEntry += "...";
						break;
					}
					else
						textEntry += string.Format("{0} {1}", textEntry.Length > 0 ? "," : "", sCreatureName);
				}

				return string.Format("R #{0} Map:{1} Spawn:{2}<BR>", loc.ToString(), ((Spawner)obj).Map.Name, textEntry);
			}
*/
			return "";
		}

		public static ArrayList PopulateList(string sFilter)
		{
			ArrayList list = new ArrayList();

			foreach (object o in World.Items.Values)
			{
				if (o is ESpawner)
				{
					if (sFilter == "")
						list.Add(o);
					else
					{
						foreach (EclSpawnEntry entry in ((ESpawner)o).SpawnEntries)
						{
							string sCreatureName = (string)entry.SpawnObjectName;
							if (sFilter.ToLower() == sCreatureName.ToLower())
							{
								list.Add(o);
								break;
							}
						}
					}
				}
/*
				else if (o is Spawner)
				{
					if (sFilter == "")
						list.Add(o);
					else
					{
						foreach (string sCreatureName in ((Spawner)o).CreaturesName)
						{
							if (sFilter.ToLower() == sCreatureName.ToLower())
							{
								list.Add(o);
								break;
							}
						}
					}
				}
*/
			}

			return list;
		}

		public void ReBuildList(string sFilter)
		{
			m_alList = PopulateList(sFilter);
		}

		public override void OnButton1Click( Mobile from, int pos )
		{
			if(CheckArrayAtLoc(pos))
			{
				((Item)m_alList[pos]).Delete();
				m_alList.RemoveAt(pos);
			}
		}

		public override void OnButton2Click( Mobile from, int pos )
		{
			if(CheckArrayAtLoc(pos))
			{
				from.Location = ((Item)m_alList[pos]).GetWorldLocation();
				from.Map = ((Item)m_alList[pos]).Map;
			}
		}

		public override void OnFilterButtonClick( Mobile from )
		{
			ReBuildList(m_sFilter);
			OnRequestGump(from, m_alList, 0);
		}

		public override void OnRequestGump(Mobile from, ArrayList alList, int loc)
		{
			from.SendGump(new ESCGump(m_alList, loc, m_sFilter));
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 13:
					RespawnAllInList( state.Mobile );
					break;
				case 16:
					DeleteAllInList( state.Mobile );
					break;
				default:
					base.OnResponse( state, info );
					return;
			}

			OnRequestGump(state.Mobile, m_alList, m_iLoc);
		}

		private void RespawnAllInList(Mobile from)
		{
			from.SendMessage("Respawning all spawners in the list...");
			int counter = 0;
			foreach (Item spawner in m_alList)
			{
				if (spawner is ESpawner)
				{
					if (!((ESpawner)spawner).IgnoreWorldSpawn)
					{
						((ESpawner)spawner).Respawn();
						counter++;
					}
				}
				else if (spawner is Spawner)
				{
					((Spawner)spawner).Respawn();
					counter++;
				}
			}
			from.SendMessage( string.Format( "Done... Respawned {0} spawners.", counter ) );
		}

		private void DeleteAllInList(Mobile from)
		{
			from.SendMessage("Deleting all spawners in the list...");
			int counter = 0;
			foreach (Item spawner in m_alList)
			{
				spawner.Delete();
				counter++;
			}
			from.SendMessage( string.Format( "Done... Deleted {0} spawners.", counter ) );
		}
	}
}