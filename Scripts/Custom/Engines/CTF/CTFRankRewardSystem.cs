using System;
using Server.Items;
using System.Collections.Generic;

namespace Server.Events.CTF
{
	public static class CTFRankRewardSystem
	{
		public static int[] CTFRankLevel = new int[]
			{
				0,
				50,
				100,
				150,
				200,
				250,
				300,
				350,
				400,
				450,
				500,
			};

		public static int[] CTFOfficerRankLevel = new int[]
			{
				0,
				5,
				10,
				15,
				20,
				25,
				30,
				35,
				40,
				45,
				50,
			};

		public static string[] CTFRank = new string[]
		{
			"Private",
			"Private First Class",
			"Lance Corporal",
			"Corporal",
			"Sergeant",
			"Staff Sergeant",
			"Gunnery Sergeant",
			"Master Sergeant",
			"First Sergeant",
			"Master Gunnery Sergeant",
			"Sergeant Major",
			"2nd Lieutenant",
			"1st Lieutenant",
			"Captain",
			"Major",
			"Lieutenant Colonel",
			"Colonel",
			"Brigadier General",
			"Major General",
			"Lieutenant General",
			"General"
		};
	}
}