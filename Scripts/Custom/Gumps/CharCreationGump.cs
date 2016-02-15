namespace Server.Gumps
{
	public class CharCreationGump : AdvGump
	{
		public CharCreationGump() : base(60, 60)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(0, 0, 350, 351, 9380);
			AddBackground(36, 82, 275, 150, 9300);
			AddHtml(0, 40, 350, 19, Center("Welcome to Defiance AOS"), false, false);
			AddHtml(41, 88, 265, 134,
			Colorize("Dear Player,<br>" +
			"We hope you will have a pleasant stay at our shard.<br>"+
			"Defiance AOS is mainly based on the gameplay when the AOS expansion came out at OSI."+
			"You are currently residing in the save new player town, where no player can attack you.<br>"+
			"Be aware that you can be attacked by players if you leave this island. You can return to this place "+
			"at any moment by using your teleportation item.<br>"+
			"You will receive some starting funds like gold and reagents within 10 minutes, so please stay online for the while being.", "333333")
			, false, true);
			AddLabel(40, 255, 0, "Greetings,");
			AddLabel(90, 285, 0, "The Defiance AOS staff");
		}
	}
}