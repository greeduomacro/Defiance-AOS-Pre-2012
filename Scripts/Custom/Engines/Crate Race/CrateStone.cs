//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Alpha 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//

using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Regions;
using Server.Network;
using Server.Gumps;
using Server.Multis;
using Server.Items;
using Server.Events;

namespace Server.Events.CrateRace
{
	public class CrateStone : Item, ITimableEvent
	{
		#region Members
		public List<CrateRectangle> Rectangles = new List<CrateRectangle>();
		public Item Track;

		private bool m_Animalised, m_QuickSpeed, m_Rewards;
		private int m_MaxCrates, m_Price, m_Laps, m_MinutesOpen;
		private Rectangle2D m_RegionRect;
		#endregion

		#region CommandProperties
		[CommandProperty(AccessLevel.GameMaster)]
		public bool QuickSpeed
		{
			get { return m_QuickSpeed; }
			set { m_QuickSpeed = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Running
		{
			get
			{
				return EventSystem.Running;
			}
			set
			{
				if (value != EventSystem.Running)
				{
					if(value == true)
						CrateRace.Start(this);
					if (value == false)
						EventSystem.Stop();
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Laps
		{
			get { return m_Laps; }
			set { m_Laps = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Price
		{
			get { return m_Price; }
			set { m_Price = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxCrates
		{
			get { return m_MaxCrates; }
			set
			{
				int i = value;
				if (i > 200)
					i = 200;

				m_MaxCrates = i;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int MinutesOpen
		{
			get { return m_MinutesOpen; }
			set { m_MinutesOpen = value;}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Animalised
		{
			get { return m_Animalised; }
			set { m_Animalised = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Rewards
		{
			get { return m_Rewards; }
			set { m_Rewards = value; }
		}

		[CommandProperty(AccessLevel.Counselor)]
		public int Crates
		{
			get { return CrateRace.Crates; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int BankedMoney
		{
			get { return CrateRace.BankedMoney; }
			set { CrateRace.BankedMoney = value; }
		}

		[CommandProperty(AccessLevel.Counselor)]
		public int Participants
		{
			get
			{
				if (CrateRace.PartData == null)
					return 0;

				return CrateRace.PartData.Count;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public string FirstPlace
		{
			get
			{
				if (CrateRace.FirstPlace == null)
					return "Noone";

				else
					return CrateRace.FirstPlace.Name;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Rectangle2D RegionRect
		{
			get { return m_RegionRect; }
			set { m_RegionRect = value; }
		}
		#endregion

		#region Constructor
		[Constructable]
		public CrateStone() : base(3796)
		{
			Hue = 1122;
			Name = "Crate Race Stone";
			Movable = false;
			Visible = false;
		}
		#endregion

		#region CrateStone Item Overrides
		public override void OnDoubleClick(Mobile from)
		{
			if (from.AccessLevel > AccessLevel.Counselor)
			{
				from.CloseGump(typeof(CrateStoneGump));
				from.SendGump(new CrateStoneGump(this));
			}
		}

		public override void OnDelete()
		{
			if (Track != null)
			{
				Track.Delete();
				Track = null;
			}

			base.OnDelete();
		}
		#endregion

		#region Ser/Deser
		public CrateStone(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(2);//version

			writer.Write(m_MinutesOpen);
			writer.Write(m_Rewards);

			writer.Write(m_RegionRect);
			writer.Write(m_MaxCrates);
			writer.Write(m_Laps);
			writer.Write(Track);
			writer.Write(m_Animalised);
			writer.Write(m_Price);

			int count = Rectangles.Count;
			writer.Write(count);
			foreach (CrateRectangle rect in Rectangles)
				rect.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
					m_MinutesOpen = reader.ReadInt();
					m_Rewards = reader.ReadBool();
					goto case 1;
				case 1:
					m_RegionRect = reader.ReadRect2D();
					goto case 0;
				case 0:
					m_MaxCrates = reader.ReadInt();
					m_Laps = reader.ReadInt();
					Track = reader.ReadItem();
					m_Animalised = reader.ReadBool();
					m_Price = reader.ReadInt();

					int count = reader.ReadInt();
					for (int i = 0; i < count; i++)
						Rectangles.Add(new CrateRectangle(reader));
					break;
			}
		}
		#endregion
	}
}