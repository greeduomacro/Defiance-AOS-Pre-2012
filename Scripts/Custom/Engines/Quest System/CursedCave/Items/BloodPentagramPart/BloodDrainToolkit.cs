using System;
using Server;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class BloodDrainToolkit : Item
    {
        public static readonly int CreatureMinHits = 30; // Minimum hits on creature that can be drained of blood.
        public static readonly int BloodAmountScale = 100; // Scales blood amount using HitsMax.
        public static readonly int MaxBloodPerCreature = 20;

        public static readonly Type[] CreaturesWithoutBlood =
        {
            // Silver
            typeof(AncientLich), typeof(Bogle), typeof(BoneKnight), typeof(BoneMagi), typeof(DarkGuardian), typeof(DarknightCreeper), typeof(FleshGolem), typeof(Ghoul), typeof(GoreFiend), typeof(HellSteed), typeof(LadyOfTheSnow), typeof(Lich), typeof(LichLord), typeof(Mummy), typeof(Revenant), typeof(RevenantLion), typeof(RottingCorpse), typeof(Shade), typeof(ShadowKnight), typeof(SkeletalKnight), typeof(SkeletalMage), typeof(SkeletalMount), typeof(Skeleton), typeof(Spectre), typeof(Wraith), typeof(Zombie),
            // ElementalBan
            typeof(AgapiteElemental), typeof(AirElemental), typeof(SummonedAirElemental), /*typeof(BloodElemental),*/ typeof(BronzeElemental), typeof(CopperElemental), typeof(CrystalElemental), typeof(DullCopperElemental), typeof(EarthElemental), typeof(SummonedEarthElemental), typeof(Efreet), typeof(FireElemental), typeof(SummonedFireElemental), typeof(GoldenElemental), typeof(IceElemental), typeof(KazeKemono), typeof(PoisonElemental), typeof(RaiJu), typeof(SandVortex), typeof(ShadowIronElemental), typeof(SnowElemental), typeof(ValoriteElemental), typeof(VeriteElemental), typeof(WaterElemental), typeof(SummonedWaterElemental),
            // Misc
            typeof(Golem)
        };

        private int m_UsesRemaining;
        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set
            {
                m_UsesRemaining = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public BloodDrainToolkit()
            : this(50)
        {
        }

        [Constructable]
        public BloodDrainToolkit(int usesRemaining)
            : base(0x1EBA)
        {
            Name = "Blood Draining Tool Kit";
            m_UsesRemaining = usesRemaining;
        }

        public BloodDrainToolkit(Serial serial)
            : base(serial)
        {
        }

        public class BloodDrainTarget : Target
        {
            private BloodDrainToolkit m_ToolKit;

            public BloodDrainTarget(BloodDrainToolkit toolKit)
                : base(18, false, TargetFlags.None)
            {
                m_ToolKit = toolKit;
            }

            /// <summary>
            /// Checks the blood amount in the creature.
            /// </summary>
            /// <param name="bc"></param>
            /// <returns>Returns -1 if not enough blood.</returns>
            private int CheckBloodAmount(BaseCreature bc, out string errorMsg)
            {
                // Default msg
                errorMsg = "There's not enough blood in that corpse.";

                if (bc == null)
                    return -1;

                if (bc.HitsMax < CreatureMinHits)
                    return -1;

                foreach (Type type in CreaturesWithoutBlood)
                {
                    if (bc.GetType() == type)
                    {
                        errorMsg = "You cant seem to find any blood in that corpse.";
                        return -1;
                    }
                }

                int returnValue = 1;
                if (bc.HitsMax > BloodAmountScale)
                    returnValue = (int)(bc.HitsMax / BloodAmountScale);

                if (returnValue > MaxBloodPerCreature)
                    return MaxBloodPerCreature;
                else
                    return returnValue;
            }

            private bool CheckCorpse(Corpse corpse, out string errorMsg)
            {
                errorMsg = "";

                if (corpse.Channeled)
                {
                    errorMsg = "There's not enough blood in that corpse.";
                    return false;
                }

                return true;
            }

            private bool CheckOwner(Mobile m, out string errorMsg)
            {
                BaseCreature bc = null;
                errorMsg = "You cant seem to find any blood in that corpse.";

                if (m is BaseCreature)
                    bc = (BaseCreature)m;
                else
                    return false;

                if (bc.Summoned || bc.IsBonded)
                    return false;

                return true;
            }

            private bool CheckForSpecialBottle(BaseCreature bc, Corpse corpse, BloodBottle bottle)
            {
                double rnd = Utility.RandomDouble();

                if (bc.IsPlagued && 0.01 > rnd && bc.HitsMax >= 100)
                {
                    bottle.ConvertToSpecial("Bottle of plagued blood", 0x558, true, 1001);
                    return true;
                }
                else if (bc.IsParagon && 0.01 > rnd && bc.HitsMax >= 100)
                {
                    bottle.ConvertToSpecial("Bottle of paragon blood", 0x501, true, 1002);
                    return true;
                }
                else if (Region.Find(corpse.Location, corpse.Map) is Regions.CursedCaveRegion && 0.10 > rnd)
                {
                    bottle.ConvertToSpecial("Bottle of mutated blood", 0x96, false, 1003);
                    return true;
                }

                return false;
            }

            private int GetQuality(Mobile from)
            {
                int skill = (int)from.Skills[SkillName.Healing].Base;

                // 10% chance to get quality blood
                if (!(0.10 > Utility.RandomDouble()))
                    return 0;

                return BaseRunicTool.Scale( 0, 10, 0, 100, Math.Min( skill * 12, 1200 ) );
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (m_ToolKit == null || m_ToolKit.Deleted)
                    return;

                if (o is Corpse && !((Corpse)o).Deleted)
                {
                    Corpse corpse = (Corpse)o;

                    if (!from.InRange(corpse.Location, 4))
                    {
                        from.SendLocalizedMessage(500446); // That is too far away.
                        return;
                    }

                    Item bottle = from.Backpack.FindItemByType(typeof(Bottle), true);
                    if (bottle == null)
                    {
                        from.SendLocalizedMessage(1044558); // You don't have any empty bottles.
                        return;
                    }

                    string sErrorMsg = "";

                    if (!CheckCorpse(corpse, out sErrorMsg))
                    {
                        from.SendMessage(sErrorMsg);
                        return;
                    }

                    if (!CheckOwner(corpse.Owner, out sErrorMsg))
                    {
                        from.SendMessage(sErrorMsg);
                        return;
                    }

                    BaseCreature bcOwner = (BaseCreature)corpse.Owner;
                    int bloodAmount = CheckBloodAmount(bcOwner, out sErrorMsg);
                    if (bloodAmount <= 0)
                    {
                        from.SendMessage(sErrorMsg);
                        return;
                    }

                    if (from.BeginAction(typeof(BloodDrainToolkit)))
                    {
                        if (from.CheckSkill(SkillName.Healing, 0, 100))
                        {
                            bool bSpecial = false;

                            corpse.Channeled = true;
                            corpse.Hue = 0x835;

                            bottle.Consume();

                            BloodBottle newBloodBottle = new BloodBottle(bloodAmount, GetQuality(from), bcOwner.Name, bcOwner.GetType());
                            bSpecial = CheckForSpecialBottle(bcOwner, corpse, newBloodBottle);
                            from.AddToBackpack(newBloodBottle);

                            from.PlaySound(0x4E);
                            from.SendMessage("You drain some {0} from the creature.", bSpecial ? "special blood" : "blood");
                        }
                        else
                            from.SendMessage("You fail to drain blood from the creature.");

                        if (--m_ToolKit.UsesRemaining <= 0)
                        {
                            from.SendLocalizedMessage(1044038); // You have worn out your tool!
                            m_ToolKit.Delete();
                        }

                        Timer.DelayCall(TimeSpan.FromSeconds(7.0), new TimerStateCallback(ReleaseToolLock), from);
                    }
                    else
                        from.SendLocalizedMessage(501789); // You must wait before trying again.
                }
                else
                    from.SendLocalizedMessage(1042600); // That is not a corpse!
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
            list.Add("skill required: healing");
        }

        public override void OnDoubleClick(Mobile from)
        {
			return;
        }

        private static void ReleaseToolLock(object state)
        {
            ((Mobile)state).EndAction(typeof(BloodDrainToolkit));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_UsesRemaining = reader.ReadInt();
        }
    }
}