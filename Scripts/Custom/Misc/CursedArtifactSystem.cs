using Server.Commands;
using Server.Items;
using System;
using System.Collections;
using Server.Targeting;

namespace Server.Items
{
	public class CursedArtifactSystem
	{
		private static int m_DefaultDurationMinutes = 7;
		private Timer m_TimerCursed;

		// public bool BeginCurse(Item art, int DurationMinutes)
		// {
			// if (art != null)
			// {
				// art.LootType = LootType.Cursed;
				// m_TimerCursed = new CursedTimer ( art, (DurationMinutes <= 0) ? m_DefaultDurationMinutes : DurationMinutes);
				// m_TimerCursed.Start();
				// return true;
			// }
			// else return false;
		// }

		public class CursedTimer : Timer
		{

			private Item m_Artifact;
			private Mobile mob;
			private Container cont;
			private int TicksLeft;
			public CursedTimer( Item art, int duration )
				: base ( TimeSpan.Zero, TimeSpan.FromSeconds(1) )
			{
				m_Artifact = art;
				m_Artifact.LootType = LootType.Cursed;
				if ( duration == 0	)
					duration = m_DefaultDurationMinutes;
				TicksLeft = duration * 60;
			}

			protected override void OnTick()
			{
				if ( m_Artifact == null || m_Artifact.Deleted )
				{
					if ( mob != null )
						mob.SolidHueOverride = -1;
					Stop();
					return;
				}
				else if ( TicksLeft <=  0)
				{
					if (m_Artifact is PetBondingDeed || m_Artifact is BankCheck || m_Artifact is ClothingBlessDeed )
						m_Artifact.LootType = LootType.Blessed;
					else if ( !( m_Artifact is PowerScroll && m_Artifact is StatCapScroll ) )
						m_Artifact.LootType = LootType.Regular;

					if (mob != null)
						mob.SolidHueOverride = -1;

					if ( cont != null )
						cont.Hue = 0;
					Stop();
					return;
				}
				else if ( m_Artifact.RootParentEntity is Mobile )
				{
					if (cont != null)
					{
						cont.Hue = 0;
						cont = null;
					}
					else if (mob == null || mob != (Mobile)m_Artifact.RootParentEntity)
					{
						if (mob != null)
							mob.SolidHueOverride = -1;
						mob = (Mobile)m_Artifact.RootParentEntity;

						if ( mob.AccessLevel == AccessLevel.Player ) // If() added by Silver
							mob.SolidHueOverride = 18;
					}
					if (m_Artifact.ParentEntity is Container)
					{
						Container container = (Container)m_Artifact.ParentEntity;
						while (container != null)
						{
							if (container is BankBox)
							{
								if ( mob != null )
								{
									mob.AddToBackpack( m_Artifact );
									mob.SendMessage("You are not allowed to bank event items.");
								}
								else if ( container.ParentEntity is Mobile )
									((Mobile)container.ParentEntity).AddToBackpack( m_Artifact );
								//mob.Kill(); // Disabled by Silver
								container = null;
							}
							else if (container.ParentEntity is Container)
								container = (Container)container.ParentEntity;
							else
								container = null;
						}
					}
					if ( mob != null && mob.AccessLevel == AccessLevel.Player )
						mob.Criminal = true; //refresh criminal status
				}
				else if (m_Artifact.RootParentEntity is Container)
				{
					if (mob != null)
					{
						mob.SolidHueOverride = -1;
						mob = null;
					}
					else if (cont == null || cont != (Container)m_Artifact.RootParentEntity)
					{
						if (cont != null)
							cont.Hue = 0;
						cont = (Container)m_Artifact.RootParentEntity;
						cont.Hue = 18;
					}
				}
				else
				{
					if (mob != null && mob.Holding != m_Artifact )
					{
						mob.SolidHueOverride = -1;
						mob = null;
					}
					if (cont != null)
						cont.Hue = 0;
					cont = null;
				}
				TicksLeft--;
				if (mob != null && mob.Hidden && mob.AccessLevel == AccessLevel.Player )
					mob.Hidden = false;
			}
		}
	}
}

namespace Server.Scripts.Commands
{
	public class CurseArtifact
	{
		public static void Initialize()
		{
			CommandSystem.Register("CurseArtifact", AccessLevel.GameMaster, new CommandEventHandler(Curse_OnCommand));
		}

		[Usage( "CurseArtifact [duration minutes]" )]
		[Description("Temporarily makes an item Cursed and highlights whoever player is carrying it.")]
		public static void Curse_OnCommand( CommandEventArgs e )
		{
			int duration = 7;
			if ( e.Length >= 1 )
				duration = e.GetInt32( 0 );
			Mobile from = e.Mobile;
			from.Target = new InternalTarget( duration );
			from.SendMessage("Target the item you want to curse.");
		}

		private class InternalTarget : Target
		{
			private int m_Duration;
			public InternalTarget( int dur ) : base( 12, true, TargetFlags.None )
			{
				m_Duration = dur;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if (targeted is Item)
				{
					Item item = (Item)targeted;
					if (!item.Movable)
						from.SendMessage("That item is not movable");
					else
					{
						Timer m_TimerCursed = new CursedArtifactSystem.CursedTimer( item, m_Duration );
						m_TimerCursed.Start();
					}
				}
			}
		}
	}
}