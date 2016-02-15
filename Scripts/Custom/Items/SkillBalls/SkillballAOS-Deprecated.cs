using System;
using System.Collections.Generic;
using Server;

namespace Server.Items
{
	public class SkillBallAOS : SkillBall
	{
		//[Constructable]
		public SkillBallAOS() : this( 25 )
		{
		}

		//[Constructable]
		public SkillBallAOS( int bonus ) : this( bonus, true )
		{
		}

		//[Constructable]
		public SkillBallAOS( int bonus, bool limited ) : this( bonus, 100, limited )
		{
		}

		//[Constructable]
		public SkillBallAOS( int bonus, int maxcap ) : this( bonus, maxcap, true )
		{
		}

		//[Constructable]
		public SkillBallAOS( int bonus, int maxcap, bool limited ) : this( bonus, maxcap, limited, 0 )
		{
		}

		//[Constructable]
		public SkillBallAOS( int bonus, int maxcap, bool limited, int days ) : this( bonus, maxcap, limited, days, 100 )
		{
		}

		//[Constructable]
		public SkillBallAOS( int bonus, int maxcap, bool limited, int days, int mincap ) : base( 7885 )
		{
		}

		public SkillBallAOS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

//			AddToCleanup( this );
		}

		public static void Configure()
		{
			m_Convert = new List<SkillBallAOS>();

//			EventSink.WorldLoad += new WorldLoadEventHandler( PurgeList );
		}

		private static List<SkillBallAOS> m_Convert;

		public static void PurgeList()
		{
			if ( m_Convert != null && m_Convert.Count > 0 )
			{
				for ( int i = 0; i < m_Convert.Count; ++i )
				{
					SkillBallAOS ball = m_Convert[i];
					SkillBall copy = null;

					copy = new SkillBall( ball.SkillBonus );
					copy.Flags = ball.Flags;
					copy.ExpireDate = ball.ExpireDate;
					copy.OwnerPlayer = ball.OwnerPlayer;
					copy.OwnerAccount = ball.OwnerAccount;
					copy.MoveToWorld( ball.Location, ball.Map );
					copy.Parent = ball.Parent;
				}
			}
		}

		public static void AddToCleanup( SkillBallAOS item )
		{
			if ( m_Convert == null )
				m_Convert = new List<SkillBallAOS>();

			m_Convert.Add( item );
		}
	}
}