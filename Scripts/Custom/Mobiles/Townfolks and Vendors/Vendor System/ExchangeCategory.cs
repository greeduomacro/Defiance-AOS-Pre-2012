using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Exchange
{
	public class ExchangeCategory
	{
		public List<ExchangeTypeInfo> InfoList;
		public Type[] Types;
		public string Name;
		public int ID;

		public ExchangeCategory(int categoryid, string name, int typesid)
		{
			ID = categoryid;
			Name = name;
			InfoList = new List<ExchangeTypeInfo>(Config.NumeratedTypes[typesid]);
		}

		#region Ser/Deser
		public void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			int count = InfoList.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++)
			{
				writer.Write(InfoList[i].Type.FullName);
				InfoList[i].Serialize(writer);
			}
		}

		public void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			int count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				ExchangeTypeInfo etinfo = null;

				string fullname = reader.ReadString();
				Type type = ScriptCompiler.FindTypeByFullName(fullname);
				foreach (ExchangeTypeInfo eti in InfoList)
				{
					if (eti.Type == type)
					{
						etinfo = eti;
						break;
					}
				}

				if (etinfo == null)
					etinfo = new ExchangeTypeInfo(typeof(Gold), "readerror");

				etinfo.Deserialize(reader);
				etinfo.Category = this;
			}
		}
		#endregion
	}
}