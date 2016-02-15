using System;
using Server.Items;

namespace Server.Engines.RewardSystem
{
	public static class AddEventRewardInfo
	{
		public static void Initialize()
		{
			new EventRewardInfo
			(
				RewardType.PlayItem,
				"Magic Sewing Kit",
				"The magic sewing kit will let you enhance one robe or cloak to a maximum of 3% physical resistance.",
				10, 0xF9D,	//Price, ItemID
				0,			//Index
				195, 160	//X, Y
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Potted Cactus",
				"A random potted cactus, which you can place in your house.",
				25, 0x1E0F,
				1,
				210, 140
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Potted Tree",
				"A random potted tree which you can place in your house.",
				30, 0x11C8,
				2,
				205, 135
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Potted Plant",
				"A random potted plant which you can place in your house",
				25, 0x11CA,
				3,
				205, 135
			);

			new EventRewardInfo
			(
				RewardType.Trammelite,
				"Special Hair Dye",
				"A special hair dye that contains bright colors.",
				25, 0xE26,
				4,
				205, 160
			);

			new EventRewardInfo
			(
				RewardType.Trammelite,
				"Special Beard Dye",
				"A special beard dye that contains bright colors.",
				25, 0xE26,
				5,
				205, 160
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"Ethereal Horse",
				"A no-aged ethereal horse with random color that will be removed after 50 days.",
				20, 0x20DD,
				6,
				210, 150
			);

			new EventRewardInfo
			(
				RewardType.Trammelite,
				"Fireworks Wand",
				"A fireworks wand with 99 uses.",
				1, 0xDF2,
				7,
				205, 155
			);

			new EventRewardInfo
			(
				RewardType.Trammelite,
				"Layered Sash Deed",
				"A deed to make one sash layered, so it will show on top of your robe.",
				100, 0x14F0,
				8,
				195, 155
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"+1 Skill Ball",
				"A limited skillball that adds 1 skillpoint to the skill you choose. It will not work for hard and crafting skills.",
				1, 7885, // Silver: Old price 2
				9,
				215, 160
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"+5 Skill Ball",
				"A limited skillball that adds 5 skillpoints to the skill you choose. It will not work for hard and crafting skills.",
				3, 7885, // Silver: Old price 7
				10,
				215, 160
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"+10 Skill Ball",
				"A limited skillball that adds 10 skillpoints to the skill you choose. It will not work for hard and crafting skills.",
				5, 7885, // Silver: Old price 12
				11,
				215, 160
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"+25 Skill Ball",
				"A limited skillball that adds 25 skillpoints to the skill you choose. It will not work for hard and crafting skills.",
				10, 7885, // Silver: Old price 25
				12,
				215, 160
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"+50 Skill Ball",
				"A limited skillball that adds 50 skillpoints to the skill you choose. It will not work for hard and crafting skills.",
				18, 7885, // Silver: Old price 35
				13,
				215, 160
			);

			new EventRewardInfo
			(
				RewardType.Trammelite,
				"Personalisation Deed",
				"This deed will add your name to the name of an item ex:\"Jack's Hooded Shroud\", this does not work on all items.",
				100, 0x14F0,
				14,
				195, 155
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Crystal Pedestal",
				"A crystal pedestal, which you can place in your house.",
				25, 0x2FD4,
				15,
				205, 90
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Stone Fountain",
				"A stone fountain, which you can place in your house.",
				125, 0x1741,
				16,
				202, 135
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Sandstone Fountain",
				"A sandstone fountain, which you can place in your house.",
				125, 0x19D3,
				17,
				210, 140
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Squirrel Statue East",
				"A statue of a squirrel, which you can place in your house.",
				50, 0x2D10,
				18,
				210, 125
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Squirrel Statue South",
				"A statue of a squirrel, which you can place in your house.",
				50, 0x2D11,
				19,
				210, 125
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Arcanist Statue East",
				"A statue, which you can place in your house.",
				50, 0x2D0E,
				20,
				210, 120
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Arcanist Statue South",
				"A statue, which you can place in your house.",
				50, 0x2D0F,
				21,
				210, 120
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Warrior Statue East",
				"A statue of a warrior, which you can place in your house.",
				50, 0x2D12,
				22,
				210, 120
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Warrior Statue South",
				"A statue of a warrior, which you can place in your house.",
				50, 0x2D13,
				23,
				210, 120
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"3 Hit Point Regen Robe",
				"A temp robe with 3 Hit Point Regeneration. It will be deleted in 30 days.",
				10, 7939,
				24,
				200, 140
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"3 Hit Point Regen Cloak",
				"A temp robe with 3 Hit Point Regeneration. It will be deleted in 30 days.",
				10, 5397,
				25,
				200, 140
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Campfire",
				"An animated campfire addon.",
				20, 3555,
				26,
				200, 150
			);

			new EventRewardInfo
			(
				RewardType.Deco,
				"Fired Brazier",
				"A brazier addon with animated fire.",
				25, 6571,
				27,
				200, 150
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"Soulstone Fragment",
				"Bound to account! Transmit a skill from any of your characters onto the stone up to 5 times, and move these skills to any other character on the same account.",
				30, 10915,
				28,
				200, 150
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"Name change deed",
				"A deed to change the name of your one character. That is irreversible (unless with another name change deed).",
				30, 5360,
				29,
				200, 150
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"Sex change deed",
				"A deed to change the gender of your character. Change your gender permanently.",
				25, 5360,
				30,
				200, 150
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"Kill reset deed",
				"Are you tired of waiting to get rid of your murder status? The kill reset deed is your solution! Resets your kill count instantly.",
				30, 5360,
				31,
				200, 150
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"Pet bonding deed",
				"Make your pet bond to you instantly. It still requires you to have enough Taming skill though.",
				20, 5360,
				32,
				200, 150
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"War horse bonding deed",
				"A deed that bonds your faction war horse to you instantly.",
				3, 5360,
				33,
				200, 150
			);

			new EventRewardInfo
			(
				RewardType.Trammelite,
				"Anti-bless deed",
				"A deed that removes the blessing of an item. It is useful when you want to use a holy bless deed on a blessed item.",
				10, 5360,
				34,
				200, 150
			);

			new EventRewardInfo
			(
				RewardType.Trammelite,
				"A Whispering Rose",
				"A Whispering Rose from [Your Name]. A beautiful gift for someone you love. It comes in a random of three forms.",
				30, 6377,
				35,
				200, 150
			);

			new EventRewardInfo
			(
				RewardType.Trammelite,
				"A Wedding Deed",
				"Propose marriage to the person you love. If they accept, both of you will receive blessed wedding rings.",
				30, 8792,
				36,
				200, 150
			);

			new EventRewardInfo
			(
				RewardType.PlayItem,
				"A Kill Book",
				"Keep record of your kills and deaths. It will work ONLY for your character and thus cannot be traded.",
				200, 8787,
				37,
				200, 150
			);
		}
	}
}