using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBNecromancer : SBInfo
	{
		private List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBNecromancer()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Type[] types = Loot.RegularScrollTypes;

				int circles = 3;

				for (int i = 0; i < circles * 8 && i < types.Length; ++i)
				{
					int itemID = 0x1F2E + i;

					if (i == 6)
						itemID = 0x1F2D;
					else if (i > 6)
						--itemID;

					Add(new GenericBuyInfo(types[i], 12 + ((i / 8) * 10), 20, itemID, 0));
				}

				Add(new GenericBuyInfo(typeof(BlackPearl), 5, 999, 0xF7A, 0));
				Add(new GenericBuyInfo(typeof(Bloodmoss), 5, 999, 0xF7B, 0));
				Add(new GenericBuyInfo(typeof(MandrakeRoot), 3, 999, 0xF86, 0));
				Add(new GenericBuyInfo(typeof(Garlic), 3, 999, 0xF84, 0));
				Add(new GenericBuyInfo(typeof(Ginseng), 3, 999, 0xF85, 0));
				Add(new GenericBuyInfo(typeof(Nightshade), 3, 999, 0xF88, 0));
				Add(new GenericBuyInfo(typeof(SpidersSilk), 3, 999, 0xF8D, 0));
				Add(new GenericBuyInfo(typeof(SulfurousAsh), 3, 999, 0xF8C, 0));

				if (Core.AOS)
				{
					Add(new GenericBuyInfo(typeof(BatWing), 3, 999, 0xF78, 0));
					Add(new GenericBuyInfo(typeof(GraveDust), 3, 999, 0xF8F, 0));
					Add(new GenericBuyInfo(typeof(DaemonBlood), 6, 999, 0xF7D, 0));
					Add(new GenericBuyInfo(typeof(NoxCrystal), 6, 999, 0xF8E, 0));
					Add(new GenericBuyInfo(typeof(PigIron), 5, 999, 0xF8A, 0));
					Add(new GenericBuyInfo(typeof(NecromancerSpellbook), 115, 10, 0x2253, 0));
				}

				Add(new GenericBuyInfo(typeof(BlankScroll), 5, 20, 0x0E34, 0));
				Add(new GenericBuyInfo(typeof(RecallRune), 25, 10, 0x1f14, 0));
				Add(new GenericBuyInfo(typeof(Spellbook), 50, 10, 0xEFA, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add(typeof(Runebook), 1250);
				Add(typeof(BlackPearl), 3);
				Add(typeof(Bloodmoss), 3);
				Add(typeof(MandrakeRoot), 2);
				Add(typeof(Garlic), 2);
				Add(typeof(Ginseng), 2);
				Add(typeof(Nightshade), 2);
				Add(typeof(SpidersSilk), 2);
				Add(typeof(SulfurousAsh), 2);
				Add(typeof(RecallRune), 8);
				Add(typeof(Spellbook), 9);
				Add(typeof(BlankScroll), 2);

				Add(typeof(TotalRefreshPotion), 7);
				Add(typeof(GreaterAgilityPotion), 7);
				Add(typeof(GreaterHealPotion), 7);
				Add(typeof(GreaterStrengthPotion), 7);
				Add(typeof(GreaterPoisonPotion), 7);
				Add(typeof(GreaterCurePotion), 7);
				Add(typeof(GreaterExplosionPotion), 10);

				Add(typeof(RefreshPotion), 7);
				Add(typeof(AgilityPotion), 7);
				Add(typeof(NightSightPotion), 7);
				Add(typeof(LesserHealPotion), 7);
				Add(typeof(StrengthPotion), 7);
				Add(typeof(LesserPoisonPotion), 7);
				Add(typeof(LesserCurePotion), 7);
				Add(typeof(LesserExplosionPotion), 10);

				if (Core.AOS)
				{
					Add(typeof(ConfusionBlastPotion), 10);
					Add(typeof(ConflagrationPotion), 10);

					Add(typeof(GreaterConflagrationPotion), 10);
					Add(typeof(GreaterConfusionBlastPotion), 10);

					Add(typeof(NoxCrystal), 3);
					Add(typeof(BatWing), 1);
					Add(typeof(GraveDust), 1);
					Add(typeof(DaemonBlood), 3);
					Add(typeof(PigIron), 2);

					Add(typeof(AnimateDeadScroll), 26);
					Add(typeof(BloodOathScroll), 26);
					Add(typeof(CorpseSkinScroll), 26);
					Add(typeof(CurseWeaponScroll), 26);
					Add(typeof(EvilOmenScroll), 26);
					Add(typeof(HorrificBeastScroll), 27);
					Add(typeof(LichFormScroll), 64);
					Add(typeof(MindRotScroll), 39);
					Add(typeof(PainSpikeScroll), 26);
					Add(typeof(PoisonStrikeScroll), 39);
					Add(typeof(StrangleScroll), 64);
					Add(typeof(SummonFamiliarScroll), 26);
					Add(typeof(VampiricEmbraceScroll), 101);
					Add(typeof(VengefulSpiritScroll), 114);
					Add(typeof(WitherScroll), 64);
					Add(typeof(WraithFormScroll), 51);
					Add(typeof(ExorcismScroll), 114);
				}

				Type[] types = Loot.RegularScrollTypes;

				for (int i = 0; i < types.Length; ++i)
					Add(types[i], 6 + ((i / 8) * 5));
			}
		}
	}
}