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
using Server.Items;
using Server.Network;

namespace Server.Multis.CustomBuilding
{
	public class BaseBuilding : BaseMulti
	{
		public static string SavePath = Path.Combine(Core.BaseDirectory, Config.Location); //"Scripts\\Multis\\BaseBuilding"
		public static string EmptyfileName = Config.EmptyFileName;

		private static string m_EmptyfilePath = Path.Combine(SavePath, EmptyfileName);
		private static MultiComponentList m_EmptyList;

		protected Packet m_InfoPacket;
		private int m_Revision;
		private bool m_Recursion;
		public int Revision { get { return m_Revision; } set { m_Revision = value; } }
		public override int GetMaxUpdateRange() { return 24; }
		public override int GetUpdateRange(Mobile m) { return 18; }

		public static MultiComponentList EmptyList
		{
			get
			{
				if (m_EmptyList == null)
				{
					if (!File.Exists(m_EmptyfilePath))
					{
						GenericWriter writer = GetWriter(SavePath, EmptyfileName);

						writer.Write(0);
						writer.Write(new Point2D(-8, -8));
						writer.Write(new Point2D(9, 10));
						writer.Write(new Point2D(8, 8));
						writer.Write(18);
						writer.Write(19);
						writer.Write(0);

						writer.Close();
					}
					using (FileStream fs = new FileStream(m_EmptyfilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						BinaryFileReader reader = new BinaryFileReader(new BinaryReader(fs));
						try
						{
							m_EmptyList = new MultiComponentList(reader);
						}

						catch (Exception err)
						{
							Console.WriteLine(err.ToString());
							return MultiComponentList.Empty;
						}
						finally
						{
							fs.Close();
						}
					}
				}
				return new MultiComponentList(m_EmptyList);
			}
		}

		public BaseBuilding() : base(21627)
		{
		}

		public override void OnLocationChange(Point3D oldLoc)
		{
			if (!m_Recursion)
			{
				m_Recursion = true;
				Z = Z - 50;
				m_Recursion = false;
			}
		}

		public static GenericWriter GetWriter(string path, string filename)
		{
			if (!Directory.Exists(BaseBuilding.SavePath))
				Directory.CreateDirectory(BaseBuilding.SavePath);

			return new BinaryFileWriter(Path.Combine(path, filename), true);
		}

		#region PacketHandling
		public static void Initialize() { Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Add_PacketHandler)); }
		private static void Add_PacketHandler() { PacketHandlers.RegisterExtended(0x1E, true, new OnPacketReceive(QueryDetails)); }

		public static void QueryDetails(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;
			DesignContext context = DesignContext.Find(from);
			int requestserial = pvSrc.ReadInt32();

			HouseFoundation foundation = World.FindItem(requestserial) as HouseFoundation;

			if (foundation != null && from.Map == foundation.Map && from.InRange(foundation.GetWorldLocation(), 24) && from.CanSee(foundation))
			{
				DesignState stateToSend;

				if (context != null && context.Foundation == foundation)
					stateToSend = foundation.DesignState;
				else
					stateToSend = foundation.CurrentState;

				stateToSend.SendDetailedInfoTo(state);
			}

			else
			{
				BaseBuilding building = World.FindItem(requestserial) as BaseBuilding;

				if (building != null && from.Map == building.Map && from.InRange(building.GetWorldLocation(), 24) && from.CanSee(building))
					building.SendDetailedInfoTo(state);
			}
		}

		public override void SendInfoTo(NetState state, bool oplenabled)
		{
			base.SendInfoTo(state, oplenabled);

			state.Send(new BuildingStateGeneral(this));
		}

		public void SendDetailedInfoTo(NetState state)
		{
			if (state != null)
			{
				lock (this)
				{
					if (m_InfoPacket == null)
						m_InfoPacket = new DesignStateDetailed(Serial, 0, Components.Min.X, Components.Min.Y, Components.Max.X, Components.Max.Y, Components.List);
					Packet p = m_InfoPacket;
					p.SetStatic();
					state.Send(p);
				}
			}
		}
		#endregion

		#region Serialisation
		public BaseBuilding(Serial serial) : base(serial)
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

	#region Supporting classes
	public class BuildingStateGeneral : Packet
	{
		public BuildingStateGeneral(BaseBuilding building) : base(0xBF)
		{
			EnsureCapacity(13);

			m_Stream.Write((short)0x1D);
			m_Stream.Write((int)building.Serial);
			m_Stream.Write((int)building.Revision);
		}
	}
	#endregion
}