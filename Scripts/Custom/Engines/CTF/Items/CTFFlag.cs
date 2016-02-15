using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Targeting;

namespace Server.Events.CTF
{
	public class CTFFlag : Item
	{
		#region Members
		private CTFGameStone m_Stone;
		private CTFTeam m_Team;
		private Timer m_Timer;
		private bool m_Home;
		private Point3D m_PlayerSpawn;
		private Point3D m_FlagHome;
		private Map m_FlagHomeMap;
		public CTFTeam Team { get { return m_Team; } set { m_Team = value; Hue = m_Team.Hue; } }
		#endregion

		#region CommandProperties

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Home{ get { return m_Home; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D PlayerSpawn { get { return m_PlayerSpawn; } set { m_PlayerSpawn = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D FlagHome { get { return m_FlagHome; } set { m_FlagHome = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Map FlagHomeMap { get { return m_FlagHomeMap; } set { m_FlagHomeMap = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public CTFGameStone Stone
		{
			get { return m_Stone; }
			set
			{
				CTFGameStone stone = value;
				if(stone.TryAddFlag(this))
					m_Stone = stone;
			}
		}
		#endregion

		[Constructable()]
		public CTFFlag() : base( 0x1627 )
		{
			Movable = false;
			Weight = 1.0;
			LootType = LootType.Cursed;
			m_Home = true;
			Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Flag_Callback));
		}

		private void Flag_Callback()
		{
			m_PlayerSpawn = Location;
			m_FlagHome = Location;
			m_FlagHomeMap = Map;
		}

		public CTFFlag( Serial serial ) : base( serial )
		{
		}

		public override void OnAdded( object parent )
		{
			Mobile m = this.RootParent as Mobile;
			if ( m != null )
			{
				CTFRobe robe = FindCTFRobe( m );
				if ( robe != null )
					robe.Hue = CTFGame.CaptureRobeHue;
			}
		}
		public override bool OnMoveOver(Mobile from)
		{
			if (CTFGame.Running)
			{
				CTFTeam team = CTFGame.GameData.GetPlayerTeam(from);

				if (team != null)
				{
					if (!from.InLOS(GetWorldLocation()))
						from.SendLocalizedMessage(502800); // You can't see that.

					else if (from.Alive)
					{
						if (team != m_Team)
						{
							if (CTFGame.GetFlag(from) != null)
								from.SendMessage(CTFGame.HuePerson, "You can only carry one flag at a time.");

							else if (from.Backpack != null)
							{
								ShowFlagEffect();
								from.RevealingAction();
								from.Backpack.DropItem(this);
								from.SendMessage(CTFGame.HuePerson, "You got the enemy flag!");
								BeginCapture();
								CTFGame.CTFMessage(string.Format("{0} ({1}) got the {2} flag!", from.Name, team.Name, m_Team.Name));
							}
							else
								from.SendMessage(CTFGame.HuePerson, "You have no backpack to carry that flag!");
						}
						else
						{
							if (!m_Home)
							{
								CTFGame.CTFMessage(string.Format("{0} has returned the {1} flag!", from.Name, m_Team.Name));
								ReturnToHome();
								ShowFlagEffect();

								CTFGame.AddScore(from, CTFGame.CTFScoreType.Returns);
							}
						}
					}
				}
			}

			else
				from.SendMessage(CTFGame.HuePerson, "You are not part of the game.");
			return base.OnMoveOver(from);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (CTFGame.Running)
			{
				CTFTeam team = CTFGame.GameData.GetPlayerTeam(from);

				if (team != null)
				{
					if (!from.InLOS(GetWorldLocation()))
						from.SendLocalizedMessage(502800); // You can't see that.

					else if (from.GetDistanceToSqrt(this.GetWorldLocation()) > 3)
						from.SendLocalizedMessage(500446); // That is too far away.

					else if (RootParent is Mobile)
					{
						if (RootParent == from)
						{
							from.Target = new CaptureTarget(this);
							from.SendMessage(CTFGame.HuePerson, "Target your flag to capture, or target a team-mate to pass the flag.");//"What do you wish to do with the flag?" );
						}
					}
				}
			}

			else if (from.AccessLevel >= AccessLevel.GameMaster)
				from.SendGump(new PropertiesGump(from, this));

			else
				from.SendMessage(CTFGame.HuePerson, "You are not part of the game.");

			base.OnDoubleClick(from);
		}

		public override void OnRemoved( object oldParent )
		{
			if ( !CTFGame.Running )
				return;

			Mobile m = null;
			if ( oldParent is Item )
				m = ((Item)oldParent).RootParent as Mobile;
			else
				m = oldParent as Mobile;

			if ( m != null )
			{
				CTFRobe robe = FindCTFRobe( m );
				CTFTeam team = CTFGame.GameData.GetPlayerTeam( m );
				if ( robe != null && team != null )
					robe.Hue = team.Hue;
			}
		}

		public override void Delete()
		{
			if (CTFGame.Running)
				return;

			StopTimer();

			if (m_Stone != null)
			{
				CTFFlag[] array = m_Stone.FlagArray;
				for (int i = 0; i < array.Length; i++)
					if (array[i] == this)
						array[i] = null;
			}

			base.Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)1 );

			writer.Write(m_PlayerSpawn);

			writer.Write(m_Stone);
			writer.Write(m_Team.Number);
			writer.Write(m_FlagHome);
			writer.Write(m_FlagHomeMap);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
					{
						m_PlayerSpawn = reader.ReadPoint3D();
						goto case 0;
					}

				case 0:
				{
					m_Stone = (CTFGameStone)reader.ReadItem(); ;
					m_Team = CTFGame.TeamArray[reader.ReadInt()];
					m_FlagHome = reader.ReadPoint3D();
					m_FlagHomeMap = reader.ReadMap();

					if (version == 0)
						m_PlayerSpawn = m_FlagHome;
					break;
				}
			}

			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( ReturnToHome ) );
		}

		/// <summary>
		/// Move the flag to the flags base.
		/// </summary>
		public void ReturnToHome()
		{
			if ( !m_Home && m_Team != null )
			{
				MoveToWorld(m_FlagHome, m_FlagHomeMap);
				m_Home = true;
			}

			StopTimer();
		}

		/// <summary>
		/// Will show sparkle effect on the flags location.
		/// </summary>
		public void ShowFlagEffect()
		{
			Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x376A, 1, 22, Hue, 7, 9502, 0 );
		}

		/// <summary>
		/// Will move the flag to the players location in the world.
		/// </summary>
		/// <param name="from"></param>
		public void DropFlag( Mobile from )
		{
			MoveToWorld( from.Location, from.Map );
		}

		/// <summary>
		/// Player has captured the flag.
		/// </summary>
		public void BeginCapture()
		{
			if( m_Timer != null && m_Timer.Running )
				return;

			StopTimer();
			//Disabled by Blady:
			// m_Timer = new ReturnTimer( this );
			//m_Timer.Start();

			m_Home = false;
		}

		/// <summary>
		/// Will stop the Return Timer.
		/// </summary>
		public void StopTimer()
		{
			if ( m_Timer != null )
			{
				m_Timer.Stop();
				m_Timer = null;
			}
		}

		/// <summary>
		/// Will try to find a CTF Flag on the player.
		/// </summary>
		/// <param name="from"></param>
		/// <returns></returns>
		private CTFRobe FindCTFRobe( Mobile from )
		{
			for(int i=0;i<from.Items.Count;i++)
				if( from.Items[i] is CTFRobe )
					return (CTFRobe)from.Items[i];
			return null;
		}

		public override void OnParentDeleted(object Parent)
		{
			ReturnToHome();
		}

		public override bool Decays{ get{ return false; } }

		private class CaptureTarget : Target
		{
			private CTFFlag m_Flag;

			public CaptureTarget( CTFFlag flag ) : base( 3, false, TargetFlags.None )
			{
				m_Flag = flag;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				CTFTeam fteam = CTFGame.GameData.GetPlayerTeam(from);
				if ( target is Mobile )
				{
					Mobile targ = (Mobile)target;
					CTFTeam tteam = CTFGame.GameData.GetPlayerTeam(targ);
					if ( targ.Alive && tteam == fteam && from != targ )
					{
						if ( targ.Backpack != null )
						{
							targ.Backpack.DropItem( m_Flag );
							targ.SendMessage( CTFGame.HuePerson, "{0} gave you the {1} flag!", from.Name, m_Flag.Team.Name );
							CTFGame.CTFMessage( string.Format( "{0} passed the {1} flag to {2}!", from.Name, m_Flag.Team.Name, targ.Name ) );
						}
					}
					else
					{
						from.SendMessage( CTFGame.HuePerson, "You cannot give the flag to them!" );
					}
				}
				else if ( target is CTFFlag )
				{
					CTFFlag flag = target as CTFFlag;
					if ( flag.Team == fteam )
					{
						if ( flag.Home )
						{
							from.SendMessage( CTFGame.HuePerson, "You captured the {0} flag!", m_Flag.Team.Name );
							CTFGame.CTFMessage(string.Format("{0} ({1}) captured the {2} flag!", from.Name, fteam.Name, m_Flag.Team.Name));
							flag.ShowFlagEffect();
							m_Flag.ReturnToHome();

							CTFGame.AddScore(from, CTFGame.CTFScoreType.Captures);
							CTFTeamGameData tgd = null;

							foreach (CTFTeamGameData td in CTFGame.GameData.TeamList)
								if (td.Team == m_Flag.Team)
									tgd = td;

							if (tgd != null)
								tgd.FlagLosses++;
						}
						else
							from.SendMessage( CTFGame.HuePerson, "Your flag must be at home to capture!" );
					}
					else
						from.SendMessage( CTFGame.HuePerson, "You can only capture for your own team!" );
				}
			}
		}

		private class ReturnTimer : Timer
		{
			private CTFFlag m_Flag;
			private DateTime m_End;
			private int m_Counter;

			public ReturnTimer( CTFFlag flag ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Flag = flag;
				m_End = DateTime.Now + TimeSpan.FromMinutes( 3.0 );
			}

			protected override void OnTick()
			{
				m_Counter++;

				Mobile owner = m_Flag.RootParent as Mobile;
				if (owner != null && owner.Hidden)
					owner.RevealingAction();

				if (m_Counter == 30)
				{
					m_Counter = 0;

					if (m_End > DateTime.Now)
					{
						TimeSpan timeLeft = m_End - DateTime.Now;
						if (owner != null)
							owner.SendMessage(CTFGame.HuePerson, "You must take the {0} flag to your flag in {1} seconds or be killed!", m_Flag.Team.Name, (int)timeLeft.TotalSeconds);
					}
					else
					{
						if (owner != null && owner.Alive)
							owner.Kill();

						CTFGame.CTFMessage(string.Format("The {0} flag has been returned to base!", m_Flag.Team.Name));
						m_Flag.ReturnToHome();
						m_Flag.ShowFlagEffect();
						Stop();
					}
				}
			}
		}
	}
}