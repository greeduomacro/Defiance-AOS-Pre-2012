using System;
using Server;

namespace Server.Items
{
	public interface IStealable
	{
		double GetMin();
		double GetMax();
		void OnSteal();
	}
}