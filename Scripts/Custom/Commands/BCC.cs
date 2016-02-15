using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Gumps;
using Server.Spells;
using Server.Commands;
using System.Collections.Generic;

namespace Server.Commands
{
	public class BCC
	{
		public static Dictionary<Mobile, BaseCreature> CreatureList = new Dictionary<Mobile, BaseCreature>();

		public static void Initialize()
		{
			CommandSystem.Register("BCC", AccessLevel.GameMaster, new CommandEventHandler(BCC_OnCommand));
		}

		[Usage("BCC")]
		[Description("BaseCreature Control command.")]
		private static void BCC_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new BCCGump(e.Mobile, ""));
		}

		public class CMAssignTarget : Target
		{
			public CMAssignTarget()
				: base(-1, false, TargetFlags.None)
			{
			}

			protected override void OnTarget(Mobile from, object targ)
			{
				if (targ is BaseCreature)
				{
					CreatureList.Remove(from);
					CreatureList.Add(from, (BaseCreature)targ);
					from.SendMessage("BaseCreature Assigned.");
				}
				else
					from.SendMessage("That is not a BaseCreature.");

				from.SendGump(new BCCGump(from, ""));
			}
		}

		public class TeleportTarget : Target
		{
			public TeleportTarget()
				: base(-1, true, TargetFlags.None)
			{
			}

			protected override void OnTarget(Mobile from, object targ)
			{
				IPoint3D p = targ as IPoint3D;
				SpellHelper.GetSurfaceTop(ref p);

				Point3D pFrom = CreatureList[from].Location;
				Point3D pTo = new Point3D(p);

				Effects.SendLocationParticles(EffectItem.Create(pFrom, CreatureList[from].Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
				CreatureList[from].MoveToWorld(pTo, from.Map);
				Effects.SendLocationParticles(EffectItem.Create(pTo, CreatureList[from].Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
				CreatureList[from].PlaySound(0x1FE);
			}
		}
	}
}

namespace Server.Gumps
{
	public class BCCGump : Gump
	{
		private static Version VerNum { get { return new Version(1, 1, 0); } }

		public BCCGump(Mobile from, string messageText)
			: base(50, 50)
		{
			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;

			this.AddPage(0);
			this.AddBackground(0, 0, 416, 275, 2620);
			this.AddAlphaRegion(5, 7, 406, 261);
			this.AddLabel(120, 10, 955, string.Format("BaseCreature Control V{0}", VerNum.ToString()));

			this.AddBackground(15, 55, 243, 103, 9350);
			this.AddTextEntry(20, 60, 231, 93, 0, (int)Buttons.MessageEntry, messageText);

			this.AddLabel(15, 35, 955, "Message");
			this.AddButton(225, 165, 4011, 4012, (int)Buttons.SendMessage, GumpButtonType.Reply, 0);
			this.AddLabel(329, 55, 955, "Go To");
			this.AddButton(370, 55, 4014, 4015, (int)Buttons.GoTo, GumpButtonType.Reply, 0);
			this.AddLabel(279, 80, 955, "Toggle Frozen");
			this.AddButton(370, 80, 4014, 4015, (int)Buttons.ToggleFrozen, GumpButtonType.Reply, 0);
			this.AddLabel(280, 105, 955, "Toggle Hidden");
			this.AddButton(370, 105, 4014, 4015, (int)Buttons.ToggleHidden, GumpButtonType.Reply, 0);
			this.AddLabel(55, 202, 955, "Set Creature");
			this.AddButton(20, 200, 4014, 4015, (int)Buttons.SetCreature, GumpButtonType.Reply, 0);
			this.AddLabel(310, 130, 955, "Teleport");
			this.AddButton(370, 130, 4014, 4015, (int)Buttons.Teleport, GumpButtonType.Reply, 0);
			this.AddButton(370, 240, 4017, 4018, (int)Buttons.Close, GumpButtonType.Reply, 0);

			this.AddLabel(21, 235, 955, "Creature Assigned:");
			this.AddLabel(147, 235, 955, CheckCreature(from) ? BCC.CreatureList[from].GetType().Name : "--- None ---");
		}

		public enum Buttons
		{
			Close,
			GoTo,
			SendMessage,
			ToggleFrozen,
			ToggleHidden,
			SetCreature,
			Teleport,
			MessageEntry,
		}

		private bool CheckCreature(Mobile from)
		{
			if (!BCC.CreatureList.ContainsKey(from) || BCC.CreatureList[from] == null || BCC.CreatureList[from].Deleted)
				return false;
			return true;
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			BaseCreature creature = null;
			TextRelay trMessageText = info.GetTextEntry((int)Buttons.MessageEntry);

			if (info.ButtonID == (int)Buttons.Close)
				return;

			if (info.ButtonID != (int)Buttons.SetCreature && !CheckCreature(from))
			{
				from.SendMessage("No Mobile Assigned");
				from.SendGump(new BCCGump(from, trMessageText.Text));
				return;
			}
			else if (info.ButtonID != (int)Buttons.SetCreature)
				creature = BCC.CreatureList[from];

			switch (info.ButtonID)
			{
				default: return;
				case (int)Buttons.SendMessage:
					if (trMessageText.Text.Length >= 1)
						creature.Say(trMessageText.Text);
					from.SendGump(new BCCGump(from, ""));
					return;
				case (int)Buttons.ToggleFrozen:
					creature.Frozen = !creature.Frozen;
					break;
				case (int)Buttons.ToggleHidden:
					creature.Hidden = !creature.Hidden;
					break;
				case (int)Buttons.SetCreature:
					from.SendMessage("Click on the BaseCreature to Assign.");
					from.Target = new BCC.CMAssignTarget();
					return;
				case (int)Buttons.Teleport:
					creature.PublicOverheadMessage(MessageType.Spell, creature.SpeechHue, true, "Rel Por", false);
					from.Target = new BCC.TeleportTarget();
					break;
				case (int)Buttons.GoTo:
					from.MoveToWorld(creature.Location, creature.Map);
					break;
			}

			from.SendGump(new BCCGump(from, trMessageText.Text));
		}
	}
}