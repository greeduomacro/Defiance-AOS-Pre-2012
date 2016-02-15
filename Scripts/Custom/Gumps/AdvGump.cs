//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.1							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using System;
using Server.Network;
using System.Collections.Generic;

namespace Server.Gumps
{
	public class AdvGump : Gump
	{
		public AdvGump() : this(false)
		{
		}

		public AdvGump(bool basics)
			: this(basics, 30, 30)
		{
		}

		public AdvGump(int x, int y) : this(false, x, y)
		{
		}

		public AdvGump(bool basics, int x, int y)
			: base(x, y)
		{
			if (basics)
			{
				Closable = true;
				Disposable = true;
				Dragable = true;
				Resizable = false;
				AddPage(0);
			}
		}

		public void AddTable(int x, int y, int[] collumns, List<string> data)
		{
			//string[] color = new string[collums.Length];
			//for (int i = 0; i < color.Length; i++)
			//	color[i] = "33333";

			AddTable(x, y, collumns, data, null, 0, 0);
		}

		public void AddTable(int x, int y, int[] collumns, List<string> data, string[] color)
		{
			AddTable(x, y, collumns, data, color, 0, 0);
		}

		public void AddTable(int x, int y, int[] collumns, List<string> data, string[] color, int style, int bordersize)
		{
			CreateTable(x, y, collumns, data, color, style, bordersize);
		}

		private void CreateTable(int x, int y, int[] collumns, List<string> data, string[] color, int style, int bordersize)
		{
			bool option2 = false;
			bool option3 = false;
			int CollLength = collumns.Length;
			string[] StringArray = new string[CollLength];
			int TotWidth = bordersize;
			foreach (int colwidth in collumns)
				TotWidth += (colwidth + bordersize);
			const int RowHeight = 18;
			int Rows = (int)(data.Count / CollLength);
			int Height = RowHeight * Rows;


			if (CollLength < 1)
				return;

			else
			{
				switch (style)
				{
					default:
					case 0: break;

					case 1://was 6 ==table with 2 lines
						AddImageTiled(x + collumns[0] + bordersize, y, bordersize, Height + bordersize, 0x0A40);
						AddImageTiled(x, y + RowHeight, TotWidth, bordersize, 0x0A40);
						break;

					case 11:// ==table with full lines

						int X3 = x;

						for (int i = 1; i <= Rows; i++)
							AddImageTiled(x, y + i * RowHeight + bordersize, TotWidth, bordersize, 0x0A40);

						for (int i = 0; i < CollLength; i++)
						{
							X3 += collumns[i] + bordersize;
							AddImageTiled(X3, y, bordersize, Height + bordersize, 0x0A40);
						}
						break;

					case 20://was 2 ==table with collums met gaasje
						int X2 = x;
						int Y2 = y;

						int startrow = 0;
						if(option2)
							startrow = 1;

						for (int j = startrow; j < Rows; j++)
						{
							for (int i = 0; i < CollLength; i++)
							{
								if (!option2 || option2 && i != 0)
								{
									if (option3)
										AddImageTiled(X2, Y2 + j * RowHeight + bordersize, collumns[i], RowHeight, 0x0A40);

									AddAlphaRegion(X2, Y2 + j * RowHeight + bordersize, collumns[i], RowHeight);
								}
							X2 += (collumns[i] + bordersize);
							}
							X2 = x;
						}

						if (option2)
							goto case 1;
						break;

					case 21:// zwart gaasje
						option3 = true;
						goto case 20;

					case 22://was 3 ==table with collums met gaasje and 2 lines
						option2 = true;
						goto case 20;

					case 23:// ==table with collums met "zwart" gaasje and 2 lines
						option2 = true;
						goto case 21;

					case 30://was 4 ==table met om de rij gaasje
						for (int i = 1; i < Rows; i += 2)
						{
							if (option2)
							{
								if (option3)
									AddImageTiled(x + collumns[0] + bordersize, y + i * RowHeight + bordersize, TotWidth - bordersize * 4 - collumns[0], RowHeight - bordersize, 0x0A40);

								AddAlphaRegion(x + collumns[0] + bordersize, y + i * RowHeight + bordersize, TotWidth - bordersize * 4 - collumns[0], RowHeight - bordersize);
							}
							else
							{
								if (option3)
									AddImageTiled(x + bordersize, y + i * RowHeight + bordersize, TotWidth - bordersize, RowHeight - bordersize, 0x0A40);

								AddAlphaRegion(x + bordersize, y + i * RowHeight + bordersize, TotWidth - bordersize, RowHeight - bordersize);
							}
						}
						if (option2)
							goto case 1;
						break;

					case 31://==zwart gaasje
						option3 = true;
						goto case 30;

					case 32://was 5 ==table met om de rij gaasje eerste tabel niet en twee lijntjes
						option2 = true;
						goto case 30;

					case 33://==table met om de rij zwart gaasje eerste tabel niet en twee lijntjes
						option2 = true;
						goto case 31;

					case 100://was 1 like propsgump
						AddImageTiled(x, y, TotWidth, Height + 2, 9354);

						int X1 = x;
						int Y1 = y;

						for (int i = 0; i <= Rows; i++)
							AddImageTiled(X1, Y1 + i * RowHeight, TotWidth, bordersize, 0x0A40);

						AddImageTiled(X1, Y1, bordersize, Height, 0x0A40);

						for (int i = 0; i < CollLength; i++)
						{
							X1 += (collumns[i] + bordersize);
							AddImageTiled(X1, Y1, bordersize, Height, 0x0A40);
						}
						break;
				}

				int coll = 0;
				int nobr = data.Count - CollLength;
				for (int i = 0; i < data.Count; i++ )
				{
					string datastr = (string)data[i];

					StringArray[coll] = StringArray[coll] + datastr + ((i<nobr)?"<br>" : "");
					coll++;
					if (coll == CollLength)
						coll = 0;
				}

				int collwidth = 0;

				for (int i = 0; i < CollLength; i++)
				{
					if (color != null && CollLength == color.GetLength(0))
						StringArray[i] = Colorize(StringArray[i], color[i]);
					int width = collumns[i];
					AddHtml(x + collwidth + bordersize + 1, y + bordersize - 1, width, Height, StringArray[i], false, false);
					collwidth += (width + bordersize);
				}
			}
		}

		public string Bolden(string str)
		{
			return String.Format("<b>{0}</b>", str);
		}

		public string Center(string str)
		{
			return String.Format("<center>{0}</center>", str);
		}

		public string Colorize(string str, string color)
		{
			if (color == "")
				return str;

			return String.Format("<basefont color=#{0}>{1}</basefont>", color, str);
		}
	}
}