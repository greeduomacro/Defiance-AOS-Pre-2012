using System;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;


namespace Server.Events.CTF
{
	public class CTFGameStone : Item, ITimableEvent
	{
		#region Members
		private double /*m_RewardGoldChance,*/ m_RewardItemChance;
		private bool m_GiveRobe, m_MsgStaff, m_GiveRewards;
		private int /*m_RewardGoldAmount,*/ m_Teams, m_MinutesOpen, m_Price;
		private TimeSpan m_DrawLength, m_GameLength;
		private CTFSpawn m_Spawn;
		private Rectangle2D m_GameArea;
		private bool m_NoScore, m_CTFLeagueGame; // Added by Silver

		public CTFFlag[] FlagArray = new CTFFlag[4];

		#endregion

		#region CommandProperties
		[CommandProperty(AccessLevel.GameMaster)]
		public Rectangle2D GameArea { get { return m_GameArea; } set { m_GameArea = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Price
		{
			get { return m_Price; }
			set
			{
				if (value < 1)
					m_Price = 1;
				else
					m_Price = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public CTFSpawn Spawn
		{
			get { return m_Spawn; }
			set { m_Spawn = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public double RewardItemChance
		{
			get { return m_RewardItemChance; }
			set { m_RewardItemChance = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Teams
		{
			get { return m_Teams; }
			set
			{
				if((value >= 2 && value <= 4) || (value == 0))
					m_Teams = value;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public int ActivePlayers
		{
			get
			{
				if (!CTFGame.Running)
					return 0;

				else
				{
					int count = 0;
					foreach (CTFPlayerGameData pgd in CTFGame.GameData.PlayerList)
						if (pgd.InGame)
							count++;

					return count;
				}
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public TimeSpan TimeLeft { get { return CTFGame.TimeLeft; }}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool GiveRobe { get { return m_GiveRobe; } set { m_GiveRobe = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MinutesOpen { get { return m_MinutesOpen; } set { m_MinutesOpen = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool MessageStaff { get { return m_MsgStaff; } set { m_MsgStaff = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool GiveRewards { get { return m_GiveRewards; } set { m_GiveRewards = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan DrawLength { get { return m_DrawLength; } set { m_DrawLength = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan GameLength { get { return m_GameLength; } set { m_GameLength = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Running
		{
			get { return EventSystem.Running; }
			set
			{
				if (value == false && EventSystem.Running)
					EventSystem.Stop();

				else if (value == true && CanRun())
					CTFGame.StartGame(this);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)] // Added by Silver
		public bool CTFLeagueGame
		{
			get { return m_CTFLeagueGame; }
			set { m_CTFLeagueGame = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)] // Added by Silver
		public bool NoScore
		{
			get { return m_NoScore; }
			set { m_NoScore = value; }
		}

		#endregion

		[Constructable]
		public CTFGameStone() : base(0xEDC)
		{
			Movable = false;
			Name = "CTF Game Stone";

			m_RewardItemChance = 0.01;
			m_GiveRobe = true;
			m_GiveRewards = true;
			m_Price = 10000;
			m_Teams = 4;
			m_DrawLength = TimeSpan.FromMinutes(3.0);
			m_GameLength = TimeSpan.FromMinutes(25.0);
			m_GameArea = new Rectangle2D(new Point2D(X - 24, Y - 24), new Point2D(X + 24, Y + 24));
		}

		public override void Delete()
		{
			if (CTFGame.Running)
				return;

			for (int i = 0; i < 4; i++)
			{
				CTFFlag flag = FlagArray[i];
				if (flag != null)
					flag.Delete();
			}


			base.Delete();
		}

		public bool CanRun()
		{
			if (EventSystem.Running)
				return false;

			for (int i = 0; i < m_Teams; i++)
				if (FlagArray[i] == null || FlagArray[i].Deleted)
					return false;

			return true;
		}

		public bool TryAddFlag(CTFFlag flag)
		{
			if (flag != null && !flag.Deleted)
			{
				for (int i = 0; i < 4; i++)
				{
					CTFFlag existingflag = FlagArray[i];
					if (existingflag == null)
					{
						flag.Team = CTFGame.TeamArray[i];
						FlagArray[i] = flag;
						return true;
					}
				}
			}

			return false;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from.AccessLevel >= AccessLevel.GameMaster)
				from.SendGump(new PropertiesGump(from, this));

			else if (CTFGame.Running)
			{
				if (CTFGame.Open)
					if (CTFGame.PlayerJoinList.Contains(from))
					{
						from.CloseGump(typeof(CTFExitGump));
						from.SendGump(new CTFExitGump((PlayerMobile)from));
					} else from.MoveToWorld(new Point3D(1431, 1693, 0),Map.Felucca);
				else from.SendMessage("You are not allowed to leave the game.");
			}
			else
				from.MoveToWorld(new Point3D(1431, 1693, 0),Map.Felucca);
		}

		public CTFGameStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)2); // version

			writer.Write(m_NoScore);
			writer.Write(m_CTFLeagueGame);

			writer.Write(m_Price);

			writer.Write(m_RewardItemChance);
			writer.Write(m_GiveRobe);
			writer.Write(m_MsgStaff);
			writer.Write(m_GiveRewards);
			writer.Write(m_Teams);
			writer.Write(m_DrawLength);
			writer.Write(m_GameLength);
			writer.Write(m_Spawn);
			writer.Write(m_GameArea);
			writer.Write(m_MinutesOpen);

			for (int i = 0; i < 4; i++)
				writer.Write(FlagArray[i]);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);


			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
				{
					m_NoScore = reader.ReadBool();
					m_CTFLeagueGame = reader.ReadBool();
					goto case 1;
				}
				case 1:
				{
					m_Price = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_RewardItemChance = reader.ReadDouble();
					m_GiveRobe = reader.ReadBool();
					m_MsgStaff = reader.ReadBool();
					m_GiveRewards = reader.ReadBool();
					m_Teams = reader.ReadInt();
					m_DrawLength = reader.ReadTimeSpan();
					m_GameLength = reader.ReadTimeSpan();
					m_Spawn = (CTFSpawn)reader.ReadItem();
					m_GameArea = reader.ReadRect2D();
					m_MinutesOpen = reader.ReadInt();

					for (int i = 0; i < 4; i++)
						FlagArray[i] = (CTFFlag)reader.ReadItem();
					break;
				}
			}
		}
	}
}