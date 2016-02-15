using System;
using Server;

namespace Server
{
	public class MiscUtility
	{
		public static string FormatTimeSpan( TimeSpan ts )
		{
			//Based on a regular scale of 365 days to a year, 30 days to a month, 24 hours to a day, 60 minutes to an hour, and 60 seconds to a minute.
			int years = ts.Days / 365;
			string year = years > 1 ? "years " : (years <= 0) ? "" : "year ";
			string yspace = String.Format("{0}{1}{2}", years > 0 ? years.ToString() : "", years > 0 ? " " : "", year);
			int months = (ts.Days % 365) / 30;
			string month = months > 1 ? "months " : (months / 30 <= 0) ? "" : "month ";
			string mspace = String.Format("{0}{1}{2}", months > 0 ? months.ToString() : "", months > 0 ? " " : "", month);
			int days = ((ts.Days % 365) % 30);
			string day = days > 1 ? "days " : days <= 0 ? "" : "day ";
			string dspace = String.Format("{0}{1}{2}", days > 0 ? days.ToString() : "", days > 0 ? " " : "", day);
			int hours = ts.Hours;
			string hour = hours > 1 ? "hours " : hours <= 0 ? "" : "hour ";
			string hspace = String.Format("{0}{1}{2}", hours > 0 ? hours.ToString() : "", hours > 0 ? " " : "", hour);
			int minutes = ts.Minutes;
			string minute = minutes > 1 ? "minutes " : minutes <= 0 ? "" : "minute ";
			string nspace = String.Format("{0}{1}{2}", minutes > 0 ? minutes.ToString() : "", minutes > 0 ? " " : "", minute);
			int seconds = ts.Seconds;
			string second = seconds > 1 ? "seconds" : seconds <= 0 ? "" : "second";
			string sspace = String.Format("{0} {1}", seconds > 0 ? seconds.ToString() : "", second);
			return String.Format("{0}{1}{2}{3}{4}{5}", yspace, mspace, dspace, hspace, nspace, sspace);
		}
	}
}