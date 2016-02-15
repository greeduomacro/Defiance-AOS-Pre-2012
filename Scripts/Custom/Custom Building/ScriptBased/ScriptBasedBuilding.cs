//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2006					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using Server;


namespace Server.Multis.CustomBuilding
{
	public class ScriptBasedBuilding : BaseBuilding
	{
		protected MultiComponentList m_Components;
		public override MultiComponentList Components { get { return m_Components; } }

		[Constructable]
		public ScriptBasedBuilding() : base()
		{
			ErectBuilding();
		}

		public virtual void ErectBuilding()
		{
			m_Components = EmptyList;
		}

		#region Serialisation
		public ScriptBasedBuilding(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			m_Components.Serialize(writer);
			base.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			m_Components = new MultiComponentList(reader);
			base.Deserialize(reader);
		}
		#endregion
	}
}