using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class SkullBone : BoneRemains
	{
		public SkullBone() : base( Utility.Random( 0x1AE0, 5 ) )
		{
		}

		public SkullBone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class SmallBone : BoneRemains
	{
		public SmallBone() : base( Utility.Random( 0x1B13, 10 ) )
		{
		}

		public SmallBone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class LargeBone : BoneRemains
	{
		public LargeBone() : base( Utility.Random( 0x1B09, 8 ) )
		{
		}

		public LargeBone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		public override int RandomAmount()
		{
			return Utility.Random( 7, 12 );
		}
	}

	public class BoneRemains : Item, IScissorable
	{
		public BoneRemains( int itemID ) : base( itemID )
		{
			Weight = 0.1;
		}

		public static void PackSkullsAndSmallBones( Container c, int amount )
		{
			PackBoneRemains( c, amount, typeof( SkullBone ), typeof( SmallBone ) );
		}

		public static void PackSkullsAndLargeBones( Container c, int amount )
		{
			PackBoneRemains( c, amount, typeof( SkullBone ), typeof( LargeBone ) );
		}

		public static void PackSmallBonesAndLargeBones( Container c, int amount )
		{
			PackBoneRemains( c, amount, typeof( SmallBone ), typeof( LargeBone ) );
		}

		public static void PackBoneRemains( Container c, int amount )
		{
			PackBoneRemains( c, amount, typeof( SkullBone ), typeof( SmallBone ), typeof( LargeBone ) );
		}

		public static void PackBoneRemains( Container c, int amount, params Type[] bonetypes )
		{
			if ( c != null && amount > 0 && bonetypes != null && bonetypes.Length > 0 )
			{
				for ( int i = 0; i < amount; i++ )
				{
					try
					{
						c.DropItem( (Item)Activator.CreateInstance( bonetypes[Utility.Random( bonetypes.Length )] ) );
					}
					catch
					{
					}
				}
			}
		}

		public virtual int RandomAmount()
		{
			return Utility.Random( 1, 3 );
		}

		public BoneRemains( Serial serial ) : base( serial )
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
		}

		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( Deleted || !from.CanSee( this ) )
				return false;

			int amount = RandomAmount();

			Item bones = new Bone( amount );
			from.PlaySound( 0x21B );

			if ( from.Backpack != null && from.Backpack.TryDropItem( from, bones, false ) )
			{
				from.SendLocalizedMessage( 1008123 ); // You cut the material and place it into your backpack.
				Delete();
			}
			else
				base.ScissorHelper( from, bones, 1 );

			return true;
		}
	}
}