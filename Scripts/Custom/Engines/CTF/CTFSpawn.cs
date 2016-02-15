using Server;
using System;
using Server.Items;
using System.Collections.Generic;

namespace Server.Events.CTF
{
	public class CTFSpawn : Item
	{
		private List<Item> m_ItemList = new List<Item>();
		private DateTime m_dtSpawnTime;
		private Timer SpawnTimer;
		private BaseWeapon m_Weapon;
		private Static m_Static;

		private TimeSpan m_MinDelay;
		private TimeSpan m_MaxDelay;


		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan MinDelay
		{
			get { return m_MinDelay; }
			set { m_MinDelay = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan MaxDelay
		{
			get { return m_MaxDelay; }
			set { m_MaxDelay = value; }
		}

		[Constructable]
		public CTFSpawn() : base( 0x1f13 )
		{
			Name = "CTFSpawn";
			Visible = false;
			Movable = false;

			m_MinDelay = TimeSpan.FromSeconds( 10 );
			m_MaxDelay = TimeSpan.FromSeconds( 20 );
			m_dtSpawnTime = DateTime.Now + GetDelay();
		}

		public CTFSpawn( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_MinDelay );
			writer.Write( m_MaxDelay );
			writer.Write(m_Static);
			writer.WriteItemList(m_ItemList);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
					m_MinDelay = reader.ReadTimeSpan();
					m_MaxDelay = reader.ReadTimeSpan();
					m_Static = (Static)reader.ReadItem();
					if (m_Static != null)
						m_Static.Delete();
					List<Item> list = reader.ReadStrongItemList();
					foreach (Item item in list)
						if(item != null && !item.Deleted)
							item.Delete();
					break;
			}
		}

		public void StartTimer()
		{
			SpawnTimer = new CTFSpawnTimer(this);
			SpawnTimer.Start();
		}

		private void StopTimer()
		{
			if (m_Static != null && !m_Static.Deleted)
				m_Static.Delete();

			foreach (Item item in m_ItemList)
				if (item != null && !item.Deleted)
					item.Delete();

			if (SpawnTimer != null)
			{
				SpawnTimer.Stop();
				SpawnTimer = null;
			}
		}

		public override void OnDelete()
		{
			StopTimer();

			base.OnDelete();
		}

		public void Spawn()
		{
			Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 );
			Effects.SendLocationParticles( EffectItem.Create( new Point3D( this.X, this.Y, this.Z - 7 ), this.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 );
			Effects.PlaySound( Location, Map, 0x203 );

			if (Utility.Random(12) == 0)
				m_Weapon = Loot.RandomRangedWeapon();
			else m_Weapon = Loot.RandomWeapon();

			BaseRunicTool.GetElementalDamages(m_Weapon);

			if (Utility.Random(2) == 0)
				m_Weapon.Attributes.SpellChanneling = 1;
			else
			{
				m_Weapon.WeaponAttributes.HitLeechMana = 40;
				m_Weapon.WeaponAttributes.HitLeechStam = 30;
			}
			m_Weapon.Attributes.WeaponDamage = 20;
			m_Weapon.LootType = LootType.Blessed;
			m_Weapon.Name = "[Event Item]";
			m_ItemList.Add(m_Weapon);
			m_Weapon.MoveToWorld( Location, Map );

			m_Static = new Static( 0x3779 );
			m_Static.Hue = m_Weapon.Hue;
			m_Static.MoveToWorld( Location, Map );

			m_dtSpawnTime = DateTime.Now + GetDelay();
		}

		public void Check()
		{
			if (!CTFGame.Running)
				StopTimer();

			else
			{
				bool m_bHasItem = (m_Weapon != null && m_Weapon.Location == this.Location && m_Weapon.Map == this.Map && !m_Weapon.Deleted && m_Weapon.Parent == null);

				if (!m_bHasItem && m_Static != null && !m_Static.Deleted)
					m_Static.Delete();

				if (!m_bHasItem && m_dtSpawnTime < DateTime.Now)
					Spawn();

				if (m_bHasItem && m_dtSpawnTime < DateTime.Now)
					m_dtSpawnTime = DateTime.Now + GetDelay();
			}
		}

		public TimeSpan GetDelay()
		{
			return TimeSpan.FromSeconds( Utility.RandomMinMax( (int)m_MinDelay.TotalSeconds, (int)m_MaxDelay.TotalSeconds ) );
		}

		private class CTFSpawnTimer : Timer
		{
			CTFSpawn m_CTFSpawn;

			public CTFSpawnTimer( CTFSpawn ctfSpawn ) : base( TimeSpan.FromSeconds( 0.5 ), TimeSpan.FromSeconds( 0.5 ) )
			{
				Priority = TimerPriority.FiftyMS;
				m_CTFSpawn = ctfSpawn;
			}

			protected override void OnTick()
			{
				m_CTFSpawn.Check();
			}
		}
	}
}