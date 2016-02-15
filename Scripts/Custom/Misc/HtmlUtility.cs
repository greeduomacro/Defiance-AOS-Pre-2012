using System;
using Server;

namespace Server
{
	public class HtmlUtility
	{
		public static string HtmlGreen = "#006600";
		public static string HtmlYellow = "#EFEF5A";
		public static string HtmlBlue = "#0000CC";
		public static string HtmlRed = "#F70839";
		public static string HtmlWhite = "#FFFFFF";
		public static string HtmlBodSelected = "#8080ff";
		public static string HtmlPartyGumpGreen = "#B5CE6B";

		public static string Color(string text, string color)
		{
			return String.Format( "<BASEFONT COLOR={0}>{1}</BASEFONT>", color, text );
		}

		public static string Center( string text )
		{
			return String.Format("<CENTER>{0}</CENTER>", text );
		}
	}
}