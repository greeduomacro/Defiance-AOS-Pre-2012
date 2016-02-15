using System;
using Server;
using System.Collections;
using Server.Mobiles;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
	public class ESpawnerGump : Gump
	{
		private ESpawner m_EclSpawner;

		public ESpawnerGump(ESpawner ESpawner)
			: base(50, 50)
		{
			m_EclSpawner = ESpawner;

			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;
			this.AddPage(0);
			this.AddBackground(50, 0, 332, 450, 2620);
			this.AddBackground(50, 451, 332, 143, 2620);
			this.AddAlphaRegion(55, 6, 322, 437);
			this.AddLabel(155, 459, 955, string.Format("{0} V{1}", m_EclSpawner.Name, m_EclSpawner.Version.ToString()));
			this.AddButton(340, 559, 4017, 4018, (int)Buttons.Close, GumpButtonType.Reply, 0);
			this.AddLabel(75, 484, 955, @"Bring All Home");
			this.AddButton(170, 484, 4023, 4024, (int)Buttons.BringAllHome, GumpButtonType.Reply, 0);
			this.AddButton(170, 509, 4023, 4024, (int)Buttons.TotalRespawn, GumpButtonType.Reply, 0);
			this.AddLabel(76, 509, 955, @"Total Respawn");
			this.AddLabel(227, 484, 955, @"Clear Objects");
			this.AddButton(322, 484, 4023, 4024, (int)Buttons.ClearObjects, GumpButtonType.Reply, 0);
			this.AddLabel(75, 559, 955, @"Clear Spawner");
			this.AddButton(170, 559, 4023, 4024, (int)Buttons.ClearSpawner, GumpButtonType.Reply, 0);

			AddSpawnerList(55, 0);

			this.AddImage(350, 332, 10410);
			this.AddImage(0, 499, 10402);
		}

		public void AddSpawnerList(int m_iGumpX, int m_iGumpY)
		{
			m_EclSpawner.Defrag();

			// Name
			AddLabel(m_iGumpX + 10, m_iGumpY + 9, 94, "Spawn List");

			// Add Current / Max count labels
			AddLabel(m_iGumpX + 234, m_iGumpY + 9, 68, "Count");
			AddLabel(m_iGumpX + 280, m_iGumpY + 9, 33, "Max");

			for (int i = 0; i < m_EclSpawner.SpawnEntries.Count + 1 && i < ESpawner.NumOfFields; i++)
			{
				AddButton(m_iGumpX + 5, (22 * i) + (m_iGumpY + 34), 0x15E0, 0x15E4, 100 + (i * 2), GumpButtonType.Reply, 0);
				AddButton(m_iGumpX + 20, (22 * i) + (m_iGumpY + 34), 0x15E2, 0x15E6, 101 + (i * 2), GumpButtonType.Reply, 0);
				AddBackground(m_iGumpX + 38, (22 * i) + (m_iGumpY + 30), 189, 23, 9400);

				string str = "";

				if (i < m_EclSpawner.SpawnEntries.Count)
				{
					EclSpawnEntry entry = (EclSpawnEntry)m_EclSpawner.SpawnEntries[i];
					str = entry.SpawnObjectName;

					// Add current count
					AddBackground(m_iGumpX + 231, (22 * i) + (m_iGumpY + 30), 40, 23, 9400);
					AddLabel(m_iGumpX + 246, (22 * i) + (m_iGumpY + 33), 68, entry.SpawnObjects.Count.ToString());

					// Add maximum count
					AddBackground(m_iGumpX + 272, (22 * i) + (m_iGumpY + 30), 40, 23, 9400);
					AddLabel(m_iGumpX + 287, (22 * i) + (m_iGumpY + 33), 33, entry.Amount.ToString());
				}
				AddTextEntry(m_iGumpX + 42, (22 * i) + (m_iGumpY + 31), 184, 21, 1149, i, str);
			}
		}

		public void CheckArray(RelayInfo info, Mobile from)
		{
			for (int i = 0; i < ESpawner.NumOfFields; i++)
			{
				TextRelay textRelay = info.GetTextEntry(i);

				if (textRelay != null)
				{
					string str = textRelay.Text;

					if (str.Length > 0)
					{
						str = str.Trim();

						Type type = ESpawner.GetType(str);
						if (type != null && (type.IsSubclassOf(typeof(Mobile)) || from.AccessLevel >= AccessLevel.Administrator))
						{
							bool found = false;
							for (int a = 0; a < m_EclSpawner.SpawnEntries.Count; a++)
							{
								EclSpawnEntry entry2 = (EclSpawnEntry)m_EclSpawner.SpawnEntries[a];
								if (entry2.SpawnObjectName == str)
									found = true;
							}

							if (!found)
								m_EclSpawner.SpawnEntries.Add(new EclSpawnEntry(str, new ArrayList(), 0));
						}
						else
						{
							from.SendMessage("{0} is not a valid type name.", str);
						}
					}
				}
			}
		}

		public enum Buttons
		{
			Close,
			BringAllHome,
			TotalRespawn,
			ClearObjects,
			ClearSpawner,
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (m_EclSpawner == null || m_EclSpawner.Deleted)
				return;

			switch (info.ButtonID)
			{
				case (int)Buttons.Close:
					return;
				case (int)Buttons.BringAllHome:
					m_EclSpawner.BringToHome();
					break;
				case (int)Buttons.TotalRespawn:
					if (m_EclSpawner.UseMaxAmount)
						m_EclSpawner.RandomRespawn();
					else
						m_EclSpawner.Respawn();
					break;
				case (int)Buttons.ClearSpawner:
					m_EclSpawner.ClearSpawner();
					break;
				case (int)Buttons.ClearObjects:
					m_EclSpawner.ClearObjects();
					break;
				default:
					int buttonID = info.ButtonID - 100;
					int index = buttonID / 2;
					int type = buttonID % 2;

					TextRelay entry = info.GetTextEntry(index);

					if (entry != null && entry.Text.Length > 0)
					{
						CheckArray(info, state.Mobile);

						if (type == 0)
							m_EclSpawner.RaiseMax(index);
						else
							m_EclSpawner.LowerMax(index);
					}
					break;
			}

			state.Mobile.SendGump(new ESpawnerGump(m_EclSpawner));
		}
	}
}