//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
// add area spells disactivated.

using System;
using System.Collections;
using Server.Mobiles;
using Server.Spells;
using Server.Items;
using Server.Regions;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;

namespace Server.Events.Duel
{
	public class SpectRegionBanner : Item
	{
		public Rectangle2D Rect { get { return new Rectangle2D(P1, P2); } }
		private SpectRegion m_Region;
		private Point2D P1;
		private Point2D P2;
		private bool m_AllowHarm;

		#region CommandProperties
		[CommandProperty( AccessLevel.GameMaster )]
		public Point2D Point1
		{
			get { return P1; }
			set { P1 = value; ChangeReg(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point2D Point2
		{
			get { return P2; }
			set { P2 = value; ChangeReg(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowHarmFull
		{
			get { return m_AllowHarm; }
			set { m_AllowHarm = value; ChangeReg(); }
		}
		#endregion

		[Constructable]
		public SpectRegionBanner() : base(5609)
		{
			Visible = false;
			Movable = false;
			Name = "Duel Region Banner";
		}

		public void ChangeReg()
		{
			if (m_Region != null)
				m_Region.Unregister();

			if (this != null)
			{
				m_Region = new SpectRegion(this);
				m_Region.Register();
			}
		}

		public SpectRegionBanner( Serial serial ) : base( serial )
		{
		}


		public override void OnMapChange()
		{
			ChangeReg();
			base.OnMapChange();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(P1);
			writer.Write(P2);
			writer.Write(m_AllowHarm);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			P1 = reader.ReadPoint2D();
			P2 = reader.ReadPoint2D();
			m_AllowHarm = reader.ReadBool();

			ChangeReg();
		}

		public override void OnDelete()
		{
			if (m_Region != null)
				m_Region.Unregister();

			base.OnDelete();
		}

		#region SpectRegion
		public class SpectRegion : BaseRegion
		{
			private SpectRegionBanner m_Banner;

			public SpectRegion(SpectRegionBanner banner) : base("a spectator area", banner.Map, 151, new Rectangle2D[] { banner.Rect })
			{
				m_Banner = banner;
			}

			private bool ThisRegion(Mobile m)
			{
				if (m.Region == this)
					return true;

				return false;
			}

			public override bool AllowHousing(Mobile from, Point3D p) { return false; }

			public override bool AllowSpawn() { return false; }

			public override void OnBeneficialAction(Mobile helper, Mobile target)
			{
				if (!ThisRegion(target))
				{
				}

				else
					base.OnBeneficialAction(helper, target);
			}

			public override bool AllowBeneficial(Mobile from, Mobile target)
			{
				if (!ThisRegion(target))
				{
					return false;
				}

				return base.AllowBeneficial(from, target);
			}

			public override bool AllowHarmful(Mobile from, Mobile target)
			{
				// Edit by Silver: no more pet killing
				return ( m_Banner.m_AllowHarm && ThisRegion(target) );

				//if (target is BaseCreature && ((BaseCreature)target).ControlMaster == null)
				//	return true;

				//if (!m_Banner.m_AllowHarm || !ThisRegion(target))
				//	return false;

				//return base.AllowHarmful(from, target);
			}

			public override bool OnBeginSpellCast(Mobile from, ISpell s)
			{
				if (!s.OnCastInTown(this) || s is TeleportSpell || s is MassDispelSpell || s is Server.Spells.Chivalry.DispelEvilSpell || s is MassCurseSpell || s is DispelSpell || s is FireFieldSpell || s is PoisonFieldSpell || s is WallOfStoneSpell || s is DispelFieldSpell || s is ParalyzeFieldSpell || s is EnergyFieldSpell )
				{
					from.SendMessage("You cannot cast that spell here.");
					return false;
				}

				return base.OnBeginSpellCast(from, s);
			}

			public override bool OnDecay(Item item) { return true; }

			public override bool OnSkillUse(Mobile m, int skill)
			{
				if (m.AccessLevel == AccessLevel.Player)
				{
					if (skill == (int)SkillName.Camping)
					{
						m.SendMessage("You cannot use that skill here.");
						return false;
					}
				}

				return base.OnSkillUse(m, skill);
			}

			public override TimeSpan GetLogoutDelay(Mobile m)
			{
				if (m.AccessLevel == AccessLevel.Player)
					return TimeSpan.FromMinutes(10);

				return base.GetLogoutDelay(m);
			}

			public override bool OnDoubleClick(Mobile m, object o)
			{
				if (o is Corpse)
				{
					Corpse c = (Corpse)o;

					bool canLoot;

					if (c.Owner == m)
						canLoot = true;
					else
						canLoot = false;

					if (!canLoot)
						m.SendMessage("You cannot loot that corpse here.");

					if (m.AccessLevel >= AccessLevel.GameMaster && !canLoot)
					{
						m.SendMessage("This is unlootable but you are able to open that with your Godly powers.");
						return true;
					}

					return canLoot;
				}

				return base.OnDoubleClick(m, o);
			}

			public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
			{
				global = 0;
				base.AlterLightLevel(m, ref global, ref personal);
			}
		}
		#endregion
	}
}