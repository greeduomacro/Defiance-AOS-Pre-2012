using System;
using System.Collections.Generic;
using Server.Items;
using Server.Network;

namespace Server.Events.Duel
{
	public static class DuelSystem
	{
		public static List<Dueller> Duellers = new List<Dueller>();
		public static int OpeningHour1, OpeningHour2;
		public static bool AnnounceOpening;
		public static int GoldCost = 2000;
		public static int GoldCost2v2 = 1000;

		public static void Configure()
		{
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(Serialize), new DC.LoadMethod(Deserialize)), "DuelSystem");
			new GlobalAnnounceTimer().Start();
		}

		static DuelStone dsBrit;
		static DuelStone dsBuccDen;

		#region GlobalAnnounceTimer
		private class GlobalAnnounceTimer : Timer
		{
			int m_LastAnnounce;

			public GlobalAnnounceTimer()
				: base(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0))
			{
			}

			protected override void OnTick()
			{
				int thishour = DateTime.Now.Hour;
				if (thishour == OpeningHour1 || thishour == OpeningHour2)
				{
					if (dsBrit == null)
					{
						dsBrit = new DuelStone();
						dsBrit.MoveToWorld(new Point3D(1433, 1691, 20), Map.Felucca);
					}
					if (dsBuccDen == null)
					{
						dsBuccDen = new DuelStone();
						dsBuccDen.MoveToWorld(new Point3D(2677, 2124, 0), Map.Felucca);
					}
					if ((thishour != m_LastAnnounce) && (AnnounceOpening))
					{
						{
							foreach (NetState ns in NetState.Instances)
							{
								Mobile m = ns.Mobile;

								if (m != null  && !(m.Region is Server.Regions.Jail))
								{
									m.SendMessage(0x482, "Message from the Duelmaster:");
									m.SendMessage(0x482, "The Duel Arena has just opened. If you wish to challenge someone to a duel now is the time to do it! The arena will close again after 1 hour.");
								}
							}
							m_LastAnnounce = thishour;
						}
					}
				} else
				{
					if (dsBrit != null) {
						dsBrit.Delete();
						dsBrit = null;
					}
					if (dsBuccDen != null) {
						dsBuccDen.Delete();
						dsBuccDen = null;
					}
				}
			}
		}
		#endregion

		private static void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(OpeningHour1);
			writer.Write(OpeningHour2);
			writer.Write(AnnounceOpening);
		}

		private static void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();
			OpeningHour1 = reader.ReadInt();
			OpeningHour2 = reader.ReadInt();
			AnnounceOpening = reader.ReadBool();
		}
	}

	public static class DuelScoreSystem
	{
		public static DateTime LastPayout;

		private static Dictionary<Mobile, DuelData>[] m_DuelDataDictionaryArray = new Dictionary<Mobile, DuelData>[DuelType.TypeArray.Length];
		private static List<DuelData>[] m_DuelDataListArray = new List<DuelData>[DuelType.TypeArray.Length]; //m_NDArr, m_NMFArr, m_TMFArr, m_UMFArr, m_NDFArr, m_TDFArr, m_UDFArr;
		private static int[] m_PrizeMoneyArray = new int[DuelType.TypeArray.Length];//m_NDMon, m_NMFMon, m_TMFMon, m_UMFMon, m_NDFMon, m_TDFMon, m_UDFMon;

		private static Item[] m_ItemArray = new Item[DuelType.TypeArray.Length];//m_NDObi, m_NMFObi, m_TMFObi, m_UMFObi, m_NDFObi, m_TDFObi, m_UDFObi;

		public static Dictionary<Mobile, DuelData>[] DuelDataDictionaryArray { get { return m_DuelDataDictionaryArray; } }
		public static List<DuelData>[] DuelDataListArray { get { return m_DuelDataListArray; } }

		public static void Configure()
		{
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(DuelDataSerializer), new DC.LoadMethod(DuelDataDeserializer)), "SavedDuelData");
			for ( int i = 0; i < DuelType.TypeArray.Length; i++ )
			{
				m_DuelDataDictionaryArray[i] = new Dictionary<Mobile, DuelData>();
				m_DuelDataListArray[i] = new List<DuelData>();
			}

			Timer.DelayCall(TimeSpan.FromHours(0.5), TimeSpan.FromHours(24.0), new TimerCallback(CheckForPayOut));
		}

		private static void CheckForPayOut()
		{
			if (DateTime.Now.Month != LastPayout.Month)
				Payout();
		}

		public static void Payout()
		{
			LastPayout = DateTime.Now;

			for (int i = 0; i < 7; i++)
			{
				List<DuelData> list = m_DuelDataListArray[i];
				int money = m_PrizeMoneyArray[i];

				list.Sort();
				int moneyone = (int)(money * 250);
				int moneytwo = (int)(money * 187.5);
				int moneythree = (int)(money * 87.5);

				int count = list.Count;

				switch (count)
				{
					case 0: break;
					case 1:
						Mobile first = (Mobile)((DuelData)list[0]).Part;
						if (first != null && first.Backpack != null)
						{
							first.Backpack.DropItem(new BankCheck(moneyone));
							DuelBelt belt = (DuelBelt)m_ItemArray[i];
							string str = "";

							switch (i)
							{
								case 0: str = "Normal Duel Champion"; break;
								case 1: str = "Normal Mage Champion"; break;
								case 2: str = "True Mage Champion"; break;
								case 3: str = "Ultimate Mage Champion"; break;
								case 4: str = "Normal Dex Champion"; break;
								case 5: str = "True Dex Champion"; break;
								case 6: str = "Ultimate Dex Champion"; break;
							}
							DateTime date = DateTime.Now - TimeSpan.FromDays(5.0);
							string finalstr = String.Format(str + ": {0:MMMM}, {1}", date, date.Year);

							if (belt != null)
								belt.Delete();

							belt = new DuelBelt();
							belt.MobileLock = first;
							belt.BeltName = finalstr;
							first.Backpack.DropItem(belt);
							m_ItemArray[i] = (Item)belt;
						}
						break;

					case 2:
						Mobile second = (Mobile)((DuelData)list[1]).Part;
						if (second != null && second.Backpack != null)
							second.Backpack.DropItem(new BankCheck(moneytwo));
						goto case 1;

					default:
						Mobile third = (Mobile)((DuelData)list[2]).Part;
						if (third != null && third.Backpack != null)
							third.Backpack.DropItem(new BankCheck(moneythree));
						goto case 2;
				}
				m_DuelDataListArray[i].Clear();
				m_DuelDataDictionaryArray[i].Clear();
				m_PrizeMoneyArray[i] = 0;
			}
		}

		public static DuelData GetData(Dictionary<Mobile, DuelData> dict, Mobile m)
		{
			DuelData data = null;
			dict.TryGetValue(m, out data);
			return data;
		}

		public static void UpdateScore(Mobile coward, int type)
		{
			List<DuelData> list = m_DuelDataListArray[type];
			Dictionary<Mobile, DuelData> dict = m_DuelDataDictionaryArray[type];
			DuelData cowarddata = null;

			if (dict.TryGetValue(coward, out cowarddata))
			{
				if (cowarddata.Points >= -5)
					cowarddata.Points -= 1;
			}
			else
			{
				cowarddata = new DuelData(coward, -1);
				list.Add(cowarddata);
				dict[coward] = cowarddata;
			}
			UpDateRank(m_DuelDataListArray[type]);
		}

		public static void UpdateScore(Mobile winner, Mobile loser, int type)
		{
			m_PrizeMoneyArray[type]++;

			if (winner.NetState != null && loser.NetState != null && winner.NetState.Address != loser.NetState.Address)
			{
				List<DuelData> list = m_DuelDataListArray[type];
				Dictionary<Mobile, DuelData> dict = m_DuelDataDictionaryArray[type];
				DuelData winnerdata = null;
				DuelData loserdata = null;

				if (!dict.TryGetValue(loser, out loserdata))
				{
					loserdata = new DuelData(loser, 2);
					list.Add(loserdata);
					dict[loser] = loserdata;
				}

				if (!dict.TryGetValue(winner, out winnerdata))
				{
					winnerdata = new DuelData(winner, 2);
					list.Add(winnerdata);
					dict[winner] = winnerdata;
				}

				int score = CreateScore(winnerdata, loserdata);

				loserdata.Points -= score;
				loserdata.Losses++;
				winnerdata.Points += score;
				winnerdata.Wins++;

				UpDateRank(m_DuelDataListArray[type]);
			}
		}

		public static int CreateScore(DuelData winner, DuelData loser)
		{
			double pointone = (double)Math.Max(winner.Points, 1.0);
			double pointtwo = (double)Math.Max(loser.Points, 1.0);
			double scone = pointtwo / pointone;
			scone = Math.Min(scone, 2.0);

			if (scone > 0.02)
				return Math.Min(3, (int)Math.Ceiling((scone * pointtwo * 0.08)));

			return 0;
		}

		public static int GetScore(Mobile m, int type)
		{
			DuelData data = null;
			if (m_DuelDataDictionaryArray[type].TryGetValue(m, out data))
				return data.Points;

			return -100;
		}


		public static void UpDateRank(List<DuelData> list)
		{
			list.Sort();
			int ddi = 1;

			foreach (DuelData dd in list)
			{
				dd.Rank = ddi;
				ddi++;
			}
		}

		public static int GetRank(Mobile m, int type)
		{
			DuelData data = null;
			if (m_DuelDataDictionaryArray[type].TryGetValue(m, out data))
				return data.Rank;

			return 0;
		}

		private static void DuelDataSerializer(GenericWriter writer)
		{
			writer.Write( 0);//version

			for (int i = 0; i < 7; i++)
			{
				List<DuelData> arr = m_DuelDataListArray[i];

				int arrcount = arr.Count;
				writer.Write(arrcount);

				for (int j = 0; j < arrcount; j++)
					((DuelData)arr[j]).Serialize(writer);

				writer.Write(m_PrizeMoneyArray[i]);
				writer.Write((Item)m_ItemArray[i]);
			}

			writer.Write(LastPayout);
		}

		private static void DuelDataDeserializer(GenericReader reader)
		{
			int version = reader.ReadInt();

			for (int i = 0; i < 7; i++)
			{
				List<DuelData> list = new List<DuelData>();
				Dictionary<Mobile, DuelData> dict = new Dictionary<Mobile, DuelData>();

				int count = reader.ReadInt();
				int rank = 1;
				for (int j = 0; j < count; j++)
				{
					DuelData dd = new DuelData();

					dd.Deserialize(reader);
					dd.Rank = rank;
					rank++;
					if (dd.Part != null)
					{
						list.Add(dd);
						dict[dd.Part] = dd;
					}
				}

				m_DuelDataListArray[i] = list;
				m_DuelDataDictionaryArray[i] = dict;

				m_PrizeMoneyArray[i] = reader.ReadInt();
				m_ItemArray[i] = reader.ReadItem();
			}
			LastPayout = reader.ReadDateTime();
		}
	}
}