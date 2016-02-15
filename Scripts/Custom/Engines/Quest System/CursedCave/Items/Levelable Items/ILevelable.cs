using System;

namespace Server.Items
{
	/// <summary>
	/// Summary description for ILevelable.
	/// </summary>
	public interface ILevelable
	{
		int Experience{ get; set; }
		int Level{ get; set; }
		Type[] Types{ get; }

		void OnLevel( int oldLevel, int newLevel );
		string GetHtml();
	}
}