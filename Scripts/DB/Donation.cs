using System;
using System.Collections;
using System.Threading;
using System.Data;
using System.Data.Odbc;
using System.IO;
using MySql.Data.MySqlClient;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Multis.Deeds;
using Server.Misc;
using Server.Commands;

namespace Server.Donation
{
	public class Donation
	{
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Item GetItem(int productNumber)
		{
			Item item = null;


			switch (productNumber)
			{
				case 16173:
					item = new EtherealHorseAoS();
					break;
				case 16174:
					item = new EtherealLlamaAoS();
					break;
				case 16175:
					item = new EtherealOstardAoS();
					break;
				case 16176:
					item = new EtherealUnicornAoS();
					break;
				case 16177:
					item = new GoldenDonationBoxAoS();
					break;
				case 16178:
					item = new SilverDonationBoxAoS();
					break;
				case 16179:
					item = new ValoriteDonationBoxAoS();
					break;
				case 16180:
					item = new MembershipTicketAoS();
					((MembershipTicketAoS)item).MemberShipTime = TimeSpan.FromDays(30);
					break;
				case 16181:
					item = new MembershipTicketAoS();
					((MembershipTicketAoS)item).MemberShipTime = TimeSpan.FromDays(90);
					break;
				case 16182:
					item = new MembershipTicketAoS();
					((MembershipTicketAoS)item).MemberShipTime = TimeSpan.FromDays(180);
					break;
				case 16183:
					item = new MembershipTicketAoS();
					((MembershipTicketAoS)item).MemberShipTime = TimeSpan.FromDays(360);
					break;
				case 16184:
					item = null;
					break;
				case 16185:
					item = new PotionBundleAoS();
					break;

				//-------
				//case 16186 TO > 16201 are used by P15 Shard
				//-------

				case 16202:
					item = new DonationSkillBall( 1, false );
					break;
				case 16203:
					item = new DonationSkillBall( 5, false );
					break;
				case 16204:
					item = new DonationSkillBall( 10, false );
					break;
				case 16205:
					item = new DonationSkillBall( 25, false );
					break;
				case 16206:
					item = new DonationSkillBall( 50, false );
					break;

				case 16207:
					item = new SerpentCrestAoS();
					break;
				case 16208:
					item = new IronMaidenAoS();
					break;
				case 16209:
					item = new GuillotineAoS();
					break;
				case 16210:
					item = new BigMushroom1AoS();
					break;
				case 16211:
					item = new BigMushroom2AoS();
					break;
				case 16212:
					item = new BigMushroom3AoS();
					break;
				case 16213:
					item = new BigMushroom4AoS();
					break;
				case 16214:
					item = new LillyPad1AoS();
					break;
				case 16215:
					item = new LillyPad2AoS();
					break;
				case 16216:
					item = new LillyPad3AoS();
					break;
				case 16217:
					item = new DonationDecorArmor1AoS();
					break;
				case 16218:
					item = new DonationDecorArmor2AoS();
					break;
				case 16219:
					item = new DonationDecorArmor3AoS();
					break;
				case 16220:
					item = new DonationDecorArmor4AoS();
					break;
				case 16254:
					item = new LunaWhiteDonationBoxAos();
					break;
				case 16255:
					item = new InvulBlueDonationBoxAoS();
					break;
				case 16256:
					item = new RumRedDonationBoxAos();
					break;
				case 16257:
					item = new ParaGoldDonationBoxAoS();
					break;
				case 16277:
					item = new EtherealPolarBearAOS();
					break;
				case 16278:
					item = new KillResetDeedAOS(true);
					break;
				case 16279:
					item = new OneMillionBankCheckDeedAOS();
					break;
				case 16280:
					item = new NameChangeDeedAOS(true);
					break;
				case 16281:
					item = new WarHorseBondingDeedAOS(true);
					break;
				case 16282:
					item = new SoulstoneToken();
					break;
				case 16283:
					item = new SexChangeDeedAOS(true);
					break;
				case 16284:
					item = new SoulstoneToken();
					break;
				case 16285:
					item = new SkillBall( 10, 120, false );
					break;
				case 16286:
					item = new SkillBall( 20, 120, false );
					break;
				case 16287:
					item = new SkillBall( 1, false );
					break;
				case 16288:
					item = new SkillBall( 5, false );
					break;
				case 16289:
					item = new SkillBall( 10, false );
					break;
				case 16290:
					item = new SkillBall( 25, false );
					break;
				case 16291:
					item = new SkillBall( 50, false );
					break;
				case 16292:
					item = new SkillballBundleLarge_120_AOS();
					break;
				case 16293:
					item = new SkillballBundleSmall_120_AOS();
					break;
				case 16294:
					item = new SkillballBundleExtraLarge_100_AOS();
					break;
				case 16295:
					item = new SkillballBundleLarge_100_AOS();
					break;
				case 16296:
					item = new SkillballBundleSmall_100_AOS();
					break;

				default:
					item = null;
					break;
			}

			return item;
		}
		public static string GetDescription(int productNumber)
		{
			string description = "";

			switch (productNumber)
			{

				case 16173:
					description = "A No Age Ethereal Horse";
					break;
				case 16174:
					description = "A No Age Ethereal Llama";
					break;
				case 16175:
					description = "A No Age Ethereal Ostard";
					break;
				case 16176:
					description = "A No Age Ethereal Unicorn";
					break;
				case 16177:
					description = "Golden Donation Box";
					break;
				case 16178:
					description = "Silver Donation Box";
					break;
				case 16179:
					description = "Valorite Donation Box";
					break;
				case 16180:
					description = "Donation Membership 1 Month";
					break;
				case 16181:
					description = "Donation Membership 3 Months";
					break;
				case 16182:
					description = "Donation Membership 6 Months";
					break;
				case 16183:
					description = "Donation Membership 1 Year";
					break;
				case 16184:
					description = "Donation Membership 1 Year";
					break;
				case 16185:
					description = "Potion Chest";
					break;

				//-------
				//case 16186 TO > 16201 are used by P15 Shard
				//-------

				case 16202:
					description = "A skillball worth 1 point";
					break;
				case 16203:
					description = "A skillball worth 5 point";
					break;
				case 16204:
					description = "A skillball worth 10 point";
					break;
				case 16205:
					description = "A skillball worth 25 point";
					break;
				case 16206:
					description = "A skillball worth 50 point";
					break;
				case 16207:
					description = "A Donation Serpent Crest";
					break;
				case 16208:
					description = "A Donation Iron Maiden";
					break;
				case 16209:
					description = "A Donation Guillotine";
					break;
				case 16210:
					description = "A Big Donation Mushroom 1";
					break;
				case 16211:
					description = "A Big Donation Mushroom 2";
					break;
				case 16212:
					description = "A Big Donation Mushroom 3";
					break;
				case 16213:
					description = "A Big Donation Mushroom 4";
					break;
				case 16214:
					description = "A Donation LillyPad 1";
					break;
				case 16215:
					description = "A Donation LillyPad 2";
					break;
				case 16216:
					description = "A Donation LillyPad 3";
					break;
				case 16217:
					description = "Donation Decoration Armor 1";
					break;
				case 16218:
					description = "Donation Decoration Armor 2";
					break;
				case 16219:
					description = "Donation Decoration Armor 3";
					break;
				case 16220:
					description = "Donation Decoration Armor 4";
					break;
				case 16254:
					description = "Luna White Donation Box";
					break;
				case 16255:
					description = "Invul Blue Donation Box";
					break;
				case 16256:
					description = "RumRed Donation Box";
					break;
				case 16257:
					description = "ParaGold Donation Box";
					break;
				case 16277:
					description = "Ethereal Polar Bear";
					break;
				case 16278:
					description = "Kill Reset Deed";
					break;
				case 16279:
					description = "One Million Gold";
					break;
				case 16280:
					description = "Name Change Deed";
					break;
				case 16281:
					description = "War Horse Bonding Deed";
					break;
				case 16282:
					description = "Promotion Token";
					break;
				case 16283:
					description = "Sex Change Deed";
					break;
				case 16284:
					description = "Promotional Token";
					break;
				case 16285:
					description = "SkillBall 10,120";
					break;
				case 16286:
					description = "Skillball 20,120";
					break;
				case 16287:
					description = "Skillball 1,100";
					break;
				case 16288:
					description = "Skillball 5,100";
					break;
				case 16289:
					description = "Skillball 10,100";
					break;
				case 16290:
					description = "Skillball 25,100";
					break;
				case 16291:
					description = "Skillball 50,100";
					break;
				case 16292:
					description = "Skillball Bundle";
					break;
				case 16293:
					description = "Skillball Bundle";
					break;
				case 16294:
					description = "Skillball Bundle";
					break;
				case 16295:
					description = "Skillball Bundle";
					break;
				case 16296:
					description = "Skillball Bundle";
					break;

				default:
					description = "";
					break;

			}

			return description;
		}

		private static string m_ConString = "Server=www.defianceuo.com;Database=defiance_eshop;User ID=defiance_ODBC;password=ohyeah;port=3306";
		private static Queue m_ResultQueue;
		private static ArrayList m_ClaimedOrders;
		private static ResultTimer result;

		private static bool ms_ClaimDonationsBlocked;
		public static bool ClaimDonationsBlocked { get { return ms_ClaimDonationsBlocked; } }

		public static void Initialize()
		{
			m_ResultQueue = new Queue();
			m_ClaimedOrders = new ArrayList();
			CommandSystem.Register("ClaimDonations", AccessLevel.Player, new CommandEventHandler(ClaimDonations_OnCommand));
            CommandSystem.Register("SetDonations", AccessLevel.Administrator, new CommandEventHandler(SetDonations_OnCommand));
            Load();
			result = new ResultTimer();
			result.Start();
		}

		#region Serilization
		private static string idxPath = Path.Combine("Saves", Path.Combine("Donation", "ClaimedOrders.idx"));
                private static string binPath = Path.Combine("Saves", Path.Combine("Donation", "ClaimedOrders.bin"));

		public static void Load()
		{
            log.Info("Loading...");
            //Console.Write("Donation: Loading...");

			ms_ClaimDonationsBlocked = false;

            if (File.Exists(idxPath) && File.Exists(binPath))
            {
                // Declare and initialize reader objects.
                FileStream idx = new FileStream(idxPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                FileStream bin = new FileStream(binPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader idxReader = new BinaryReader(idx);
                BinaryFileReader binReader = new BinaryFileReader(new BinaryReader(bin));

                // Start by reading the number of orders from an index file
                int orderCount = idxReader.ReadInt32();
                if (orderCount == 0)
                {
                    log.Warn("Donations save does not contain any entries, [claimdonations deactivated.");
                    //Console.WriteLine("Donations save does not contain any entries, [claimdonations deactivated.");
                    ms_ClaimDonationsBlocked = true;
                }

                for (int i = 0; i < orderCount; ++i)
                {
                    ClaimedOrder c = new ClaimedOrder();
                    // Read start-position and length of current order from index file
                    long startPos = idxReader.ReadInt64();
                    int length = idxReader.ReadInt32();
                    // Point the reading stream to the proper position
                    binReader.Seek(startPos, SeekOrigin.Begin);

                    try
                    {
                        c.Deserialize(binReader);

                        if (binReader.Position != (startPos + length))
                            throw new Exception(String.Format("***** Bad serialize on ClaimedOrder[{0}] *****", i));
                    }
                    catch (Exception e)
                    {
                        log.Fatal("Error deserializing donations, [claimdonations deactivated.", e);
                        //Console.WriteLine("Error deserializing donations, [claimdonations deactivated: {0}", e.Message);
                        ms_ClaimDonationsBlocked = true;
                    }
                    m_ClaimedOrders.Add(c);
                }
                // Remember to close the streams
                idxReader.Close();
                binReader.Close();
            }
            else
            {
                log.Error("Error deserializing donations: idx/bin doesn't exist, [claimdonations deactivated.");
                //Console.WriteLine("Error deserializing donations: idx/bin doesn't exist, [claimdonations deactivated.");
                ms_ClaimDonationsBlocked = true;
            }

			//Console.WriteLine("done");
            log.Info("done.");
		}


		public static void Save()
		{
            log.Info("Saving...");
            log.Info(String.Format("idxPath: '{0}'", idxPath));
            log.Info(String.Format("binPath: '{0}'", binPath));

			if (!Directory.Exists(Path.Combine("Saves", "Donation")))
				Directory.CreateDirectory(Path.Combine("Saves", "Donation"));

			GenericWriter idx = new BinaryFileWriter(idxPath, false);
			GenericWriter bin = new BinaryFileWriter(binPath, true);

            log.Info(String.Format("m_ClaimedOrders.Count: '{0}'", m_ClaimedOrders.Count));
            idx.Write((int)m_ClaimedOrders.Count);
			foreach (ClaimedOrder claimed in m_ClaimedOrders)
			{
				long startPos = bin.Position;
				claimed.Serialize(bin);
				idx.Write((long)startPos);
				idx.Write((int)(bin.Position - startPos));
			}
			idx.Close();
			bin.Close();
            log.Info("Saving done.");
		}

		#endregion

		private class ResultTimer : Timer
		{

			public ResultTimer() : base(TimeSpan.FromSeconds( 0.5 ), TimeSpan.FromSeconds( 0.5 ))
			{
			}

			protected override void OnTick()
			{
				DatabaseResult result = null;

				Monitor.Enter(m_ResultQueue);
				if (m_ResultQueue.Count > 0)
					result = (DatabaseResult)m_ResultQueue.Dequeue();
				Monitor.Exit(m_ResultQueue);

				if (result != null && result.Mobile != null && result.Mobile.NetState != null)
				{
					result.DataItems = GetUnclaimed(result.DataItems);

					if (result.Status == ResultStatus.OK && result.DataItems.Count == 0)
					{
						result.Status = ResultStatus.NoUndeliveredDonationsFound;
					}
					if (!result.Mobile.HasGump(typeof(ClaimDonationsGump)))
					{
						result.Mobile.SendGump(new ClaimDonationsGump(0, 50, 50, result));
					}
					else
					{
						result.Mobile.SendMessage("Your claim command was canceled because you are already in the middle of claiming rewards.");
					}
				}
			}


		}

        [Usage("SetDonations <true | false>")]
        [Description("Toggles [claimdonations availability.")]
        public static void SetDonations_OnCommand(CommandEventArgs e)
        {
            if (e.Length == 1)
            {
                ms_ClaimDonationsBlocked = !e.GetBoolean(0);//Note: true leads to donations not being blocked!
                e.Mobile.SendMessage("Donations have been {0}.", ms_ClaimDonationsBlocked ? "disabled" : "enabled");
                e.Mobile.SendMessage("Warning: Donations will be reactivated on server restart!");
            }
            else
            {
                e.Mobile.SendMessage("Donations are {0}.", ms_ClaimDonationsBlocked ? "disabled" : "enabled");
                e.Mobile.SendMessage("Format: SetDonations <true | false>");
            }
        }

        [Usage("ClaimDonations")]
        [Description("Use this to claim any donation rewards")]
        public static void ClaimDonations_OnCommand(CommandEventArgs e)
        {
            if (ms_ClaimDonationsBlocked)
                e.Mobile.SendMessage("[claimdonations deactivated. Please contact an Administrator.");
            else
                CheckDonations(e.Mobile);
        }

        public static void CheckDonations(Mobile m)
		{
			//Dispatch thread
			DonationChecker dc = new DonationChecker(m);
			Thread t = new Thread(new ThreadStart(dc.BeginCheck));
			t.Name = "Worker Thread";
			t.Start();
		}


		private class DonationChecker
		{

			private Mobile m_Mobile;

			private IDbConnection con;
			private IDbCommand command;
			private IDataReader reader;

			public DonationChecker(Mobile from)
			{
				m_Mobile = from;
			}

			//SELECT s1 FROM t1 WHERE s1 IN    (SELECT s1 FROM t2);
			// SELECT order_id, product_id, amount FROM xlite_order_items WHERE order_id IN (SELECT order_id FROM xlite_orders WHERE profile_id = BAH)
			public void BeginCheck()
			{
                Console.WriteLine("Beginning of thread function");

				if (m_Mobile == null)
					return;

				DatabaseResult result = new DatabaseResult(m_Mobile);
				result.Status = ResultStatus.Unresolved;
				string accountName = (m_Mobile.Account as Account).Username;
				con = null;
				command = null;
				reader = null;

				try
				{

					string query = "SELECT DISTINCT xlite_order_items.order_id, xlite_order_items.product_id, xlite_order_items.amount " +
										"FROM xlite_order_items, xlite_orders, xlite_profiles " +
										"WHERE xlite_order_items.order_id = xlite_orders.order_id " +
										"AND xlite_orders.profile_id = xlite_profiles.profile_id " +
										"AND xlite_profiles.shipping_firstname = '" + accountName + "' " +
										"AND xlite_orders.status = 'P' " +
										"ORDER BY xlite_order_items.order_id";

					con = new MySqlConnection(m_ConString);
					con.Open();
					command = con.CreateCommand();
					command.CommandText = query;
					reader = command.ExecuteReader();

					while (reader.Read())
					{
						DataItem item = new DataItem(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2));
						Console.WriteLine("Order ID: {0}, ProductID: {1}, Amount: {2}", item.OrderID, item.ProductID, item.Amount);
						result.DataItems.Add(item);
					}
					reader.Close();
					if (result.DataItems.Count > 0)
						result.Status = ResultStatus.OK;
					else
						result.Status = ResultStatus.NoDonationsFound;

					/*
					string query = "SELECT profile_id FROM xlite_profiles WHERE shipping_firstname = '"+m_AccountName+"'";

					con = new OdbcConnection(m_ConString);
					command= new OdbcCommand(query,con);
					command.Connection.Open();
					reader = command.ExecuteReader();

					int profileID;
					if (reader.Read())
					{
						profileID = reader.GetInt32(0);
						reader.Close();

						string subQuery =	"SELECT DISTINCT xlite_order_items.order_id, xlite_order_items.product_id, xlite_order_items.amount " +
											"FROM xlite_order_items, xlite_orders " +
											"WHERE xlite_order_items.order_id = xlite_orders.order_id " +
											"AND xlite_orders.profile_id = " + profileID;

						command = new OdbcCommand(subQuery, con);
						reader = command.ExecuteReader();

						while (reader.Read())
						{
							DataItem item = new DataItem(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2));
							Console.WriteLine("Order ID: {0}, ProductID: {1}, Amount: {2}", item.OrderID, item.ProductID, item.Amount);
							result.DataItems.Add(item);

						}
						reader.Close();
						if(result.DataItems.Count > 0)
							result.Status = ResultStatus.OK;
						else
							result.Status = ResultStatus.NoDonationsFound;
					}
					else
					{
						result.Status = ResultStatus.ProfileNotFound;
					}

				*/
				}
				catch (Exception e)
				{
					result.Status = ResultStatus.DatabaseError;
					Console.WriteLine("Error in class DonationChecker : {0}", e.Message);
				}
				finally
				{
					if (con != null)
						con.Close();
				}

				Monitor.Enter(m_ResultQueue);
				m_ResultQueue.Enqueue(result);
				Monitor.Exit(m_ResultQueue);

				Console.WriteLine("End of thread function");
			}
		}

		#region DataBase result datamembers and classes
		private enum ResultStatus
		{
			Unresolved = 0,
			OK,
			NoDonationsFound,
			NoUndeliveredDonationsFound,
			DatabaseError,
		}

		private class DatabaseResult
		{
			private Mobile m_Mobile;
			private ArrayList m_DataItems;
			private ResultStatus m_Status;

			public DatabaseResult(Mobile m)
			{
				m_Mobile = m;
				m_DataItems = new ArrayList();
			}

			public Mobile Mobile
			{
				get { return m_Mobile; }
				set { m_Mobile = value; }
			}

			public ResultStatus Status
			{
				get { return m_Status; }
				set { m_Status = value; }
			}

			public ArrayList DataItems
			{
				get { return m_DataItems; }
				set { m_DataItems = value; }
			}
		}
		private class DataItem
		{
			private int m_OrderID;
			private int m_ProductID;
			private int m_Amount;

			public DataItem(int orderID, int productID, int amount)
			{
				this.m_OrderID = orderID;
				this.m_ProductID = productID;
				this.m_Amount = amount;
			}
			public int OrderID
			{
				get { return m_OrderID; }
//				set { m_OrderID = value; }
			}

			public int ProductID
			{
				get { return m_ProductID; }
//				set { m_ProductID = value; }
			}

			public int Amount
			{
				get { return m_Amount; }
//				set { m_Amount = value; }
			}

		}
		private static ArrayList GetUnclaimed(ArrayList dataItems)
		{
			ArrayList toClaim = new ArrayList();

			foreach (DataItem item in dataItems)
			{
				if (!AlreadyClaimed(item.OrderID))
					toClaim.Add(item);
			}

			return toClaim;
		}
		private static bool AlreadyClaimed(int orderNum)
		{
			foreach (ClaimedOrder order in m_ClaimedOrders)
			{
				if (order.OrderID == orderNum)
					return true;
			}
			return false;
		}
		#endregion

		private static void Log(string error)
		{
		}
		private class ClaimedOrder
		{
			// Constructor for deserilization
			public ClaimedOrder()
			{
				// perhaps some kind of initialization
			}

			private int m_OrderID;
			private DateTime m_DeliverTime;
			private string m_DeliverAccountName;
			private string m_DeliverMobileName;
			private Mobile m_DeliverMobile;

			public ClaimedOrder(int order, Mobile mobile)
			{
				this.m_OrderID = order;
				this.m_DeliverTime = DateTime.Now;
				this.m_DeliverMobile = mobile;
				this.m_DeliverAccountName = (mobile.Account as Account).Username;
				this.m_DeliverMobileName = mobile.RawName;
			}

			public int OrderID
			{
				get { return m_OrderID; }
//				set { m_OrderID = value; }
			}

			public DateTime DeliverTime
			{
				get { return m_DeliverTime; }
//				set { m_DeliverTime = value; }
			}
			public Mobile DeliverMobile
			{
				get { return m_DeliverMobile; }
//				set { m_DeliverMobile = value; }
			}

			public string DeliverAccountName
			{
				get { return m_DeliverAccountName; }
//				set { m_DeliverTime = value; }
			}

			public string DeliverMobileName
			{
				get { return m_DeliverMobileName; }
//				set { m_DeliverTime = value; }
			}

			public void Serialize(GenericWriter writer)
			{
				writer.Write((int)0);
				writer.Write(m_OrderID);
				writer.Write(m_DeliverTime);
				writer.Write(m_DeliverMobile);
				writer.Write(m_DeliverAccountName);
				writer.Write(m_DeliverMobileName);
			}

			public void Deserialize(GenericReader reader)
			{
				int version = reader.ReadInt();
				m_OrderID = reader.ReadInt();
				m_DeliverTime = reader.ReadDateTime();
				m_DeliverMobile = reader.ReadMobile();
				m_DeliverAccountName = reader.ReadString();
				m_DeliverMobileName = reader.ReadString();
			}
		}

		private class ClaimDonationsGump : Gump
		{
			private DatabaseResult m_Result;
			private int m_Page;

			public ClaimDonationsGump(int page, int x, int y, DatabaseResult result) : base( x, y )
			{
				this.m_Result = result;
				this.m_Page = page;
				this.Closable = true;
				this.Disposable = true;
				this.Dragable = true;
				this.Resizable = false;
				this.AddPage(0);

				this.AddBackground(0, 0, 400, 400, 9380);

				this.AddLabel(100, 5, 1259, @"Welcome to Defiance Donation");

				if (m_Result.Status == ResultStatus.Unresolved)
				{
					this.AddLabel(100, 100, 1259, @"An unresolved error occured trying");
					this.AddLabel(100, 120, 1259, @"to claim your rewards. If you keep");
					this.AddLabel(100, 140, 1259, @"getting this message, please contact");
					this.AddLabel(100, 160, 1259, @"an administrator.");
					this.AddLabel(100, 200, 1259, @"Sorry for the inconvenience");
					this.AddButton(170, 340, 241, 242, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
				}
				else if (m_Result.Status == ResultStatus.DatabaseError)
				{
					this.AddLabel(100, 100, 1259, @"A database error occured trying");
					this.AddLabel(100, 120, 1259, @"to claim your rewards. If you keep");
					this.AddLabel(100, 140, 1259, @"getting this message, please contact");
					this.AddLabel(100, 160, 1259, @"an administrator.");
					this.AddLabel(100, 200, 1259, @"Sorry for the inconvenience");
					this.AddButton(170, 340, 241, 242, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
				}
				else if (m_Result.Status == ResultStatus.NoDonationsFound)
				{
					this.AddLabel(100, 60, 1259, @"The database could not find any");
					this.AddLabel(100, 80, 1259, @"donations tied to this account.");
					this.AddLabel(100, 100, 1259, @"If you believe this is a mistake,");
					this.AddLabel(100, 120, 1259, @"please contact an administrator.");
					this.AddLabel(100, 140, 1259, @"Remember that orders are tied to");
					this.AddLabel(100, 160, 1259, @"the account specified in the cart-");
					this.AddLabel(100, 180, 1259, @"system profile, at order time.");
					this.AddLabel(100, 200, 1259, @"If you have changed the account");
					this.AddLabel(100, 220, 1259, @"in your profile since your last");
					this.AddLabel(100, 240, 1259, @"order, it will not be claimable");
					this.AddLabel(100, 260, 1259, @"from this account.");

					this.AddLabel(100, 300, 1259, @"Sorry for the inconvenience");
					this.AddButton(170, 340, 241, 242, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
				}
				else if (m_Result.Status == ResultStatus.NoUndeliveredDonationsFound)
				{
					this.AddLabel(100, 60, 1259, @"No unclaimed donations rewards");
					this.AddLabel(100, 80, 1259, @"could be found for this account.");
					this.AddLabel(100, 100, 1259, @"If you believe this is a mistake,");
					this.AddLabel(100, 120, 1259, @"please contact an administrator.");
					this.AddLabel(100, 140, 1259, @"Remember that orders are tied to");
					this.AddLabel(100, 160, 1259, @"the account specified in the cart-");
					this.AddLabel(100, 180, 1259, @"system profile, at order time.");
					this.AddLabel(100, 200, 1259, @"If you have changed the account");
					this.AddLabel(100, 220, 1259, @"in your profile since your last");
					this.AddLabel(100, 240, 1259, @"order, it will not be claimable");
					this.AddLabel(100, 260, 1259, @"from this account.");

					this.AddLabel(100, 300, 1259, @"Sorry for the inconvenience");
					this.AddButton(170, 340, 241, 242, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
				}
				else if (m_Result.Status == ResultStatus.OK && page >= 0)
				{
					if (page > 0)
					{
						this.AddLabel(60, 375, 1149, @"Prev page");
						this.AddButton(40, 380, 9706, 9707, (int)Buttons.Previous, GumpButtonType.Reply, 0);
					}
					if (m_Result.DataItems.Count > page * 10 + 10)
					{
						this.AddLabel(270, 375, 1149, @"Next page");
						this.AddButton(330, 380, 9702, 9703, (int)Buttons.Next, GumpButtonType.Reply, 0);
					}

					this.AddLabel(40, 40, 1259, @"Congratulations, you have unclaimed donation rewards!!");
					this.AddLabel(40, 80, 1259, @"OrderID:");
					this.AddLabel(100, 80, 1259, @"Item Description:");
					this.AddLabel(320, 80, 1259, @"Amount:");

					int j = 0;
					for (int i = page * 10; i < m_Result.DataItems.Count && j < 10; i++)
					{
						int productID = ((DataItem)m_Result.DataItems[i]).ProductID;
						int orderID = ((DataItem)m_Result.DataItems[i]).OrderID;
						int amount = ((DataItem)m_Result.DataItems[i]).Amount;

						this.AddLabel(50, 100 + j * 20, 1149, @"" + orderID);
						this.AddLabel(100, 100 + j * 20, 1149, @"" + GetDescription(productID));
						this.AddLabel(320, 100 + j * 20, 1149, @"" + amount);

						j++;
					}
					this.AddLabel(80, 320, 1259, @"Claim these rewards with this character?");

					this.AddButton(120, 340, 247, 248, (int)Buttons.OK, GumpButtonType.Reply, 0);
					this.AddButton(220, 340, 241, 242, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
				}
			}
			public enum Buttons
			{
				Cancel = 0,
				Previous = 1,
				Next = 2,
				OK = 3,
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				if (state == null || state.Mobile == null)
					return;

				m_Result.DataItems = GetUnclaimed(m_Result.DataItems);

				if (m_Result.Status == ResultStatus.OK && m_Result.DataItems.Count == 0)
				{
					m_Result.Status = ResultStatus.NoUndeliveredDonationsFound;
				}

				if (info.ButtonID == (int)Buttons.Previous && m_Page > 0)
				{
					state.Mobile.SendGump(new ClaimDonationsGump(m_Page - 1, this.X, this.Y, m_Result));
				}
				else if (info.ButtonID == (int)Buttons.Next)
				{
					state.Mobile.SendGump(new ClaimDonationsGump(m_Page + 1, this.X, this.Y, m_Result));
				}
				else if (info.ButtonID == (int)Buttons.OK && m_Result.Status == ResultStatus.OK)
				{

					state.Mobile.CloseAllGumps();
					if (state.Mobile != null && !state.Mobile.Deleted && state.Mobile.NetState != null)
					{
						if (state.Mobile.BankBox == null)
						{
							state.Mobile.SendMessage("You don't seem to have a bankbox, contact a GM.");
						}
						else
						{

							ArrayList temp = new ArrayList();
							Bag bag = new Bag();
							bag.Hue = 33;
							bag.Name = "a donation claim bag";

							foreach (DataItem item in m_Result.DataItems)
							{
								//Make sure we check amount
								for (int i = 0; i < item.Amount; i++)
								{
									Item toGive = GetItem(item.ProductID);
									if (toGive != null)
									{
										bag.DropItem(toGive);
									}
									else
									{
										state.Mobile.SendMessage("An error ocurred claiming an item in order number: {0}. An errorlog has been created, please contact an administrator.");
										string error = String.Format("An error ocurred trying to fetch item for product number: {0} in order: {1} for {2}({3}).",
																	item.ProductID, item.OrderID, state.Mobile.RawName, (state.Mobile.Account as Account).Username);
										Log(error);
									}
								}

								// Register claim. We only register each order one time
								if (!temp.Contains(item.OrderID))
								{
									temp.Add(item.OrderID);
									ClaimedOrder claim = new ClaimedOrder(item.OrderID, state.Mobile);
									m_ClaimedOrders.Add(claim);
								}

							}
							state.Mobile.BankBox.DropItem(bag);
							state.Mobile.SendMessage("Your have claimed your donations. They have been added to your bankbox. Thank you for donating!");
						}
					}
				}
				else
				{
					//state.Mobile.SendMessage("You could not claim the donations, because you claimed them wile this gump was open");
				}
			}
		}
	}
}