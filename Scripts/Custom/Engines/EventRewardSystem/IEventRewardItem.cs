using System;
using Server.Items;

namespace Server.Engines.RewardSystem
{
	public interface ITempItem
	{
		DateTime RemovalTime { get;}
		string PropertyString { get; set;}
	}
}