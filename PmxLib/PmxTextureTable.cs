using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	internal class PmxTextureTable : IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.TexTable;

		public Dictionary<string, int> NameToIndex
		{
			get;
			private set;
		}

		public Dictionary<int, string> IndexToName
		{
			get;
			private set;
		}

		public int Count => NameToIndex.Count;

		public int GetIndex(string name)
		{
			int result = -1;
			if (NameToIndex.ContainsKey(name))
			{
				result = NameToIndex[name];
			}
			return result;
		}

		public string GetName(int ix)
		{
			string result = "";
			if (IndexToName.ContainsKey(ix))
			{
				result = IndexToName[ix];
			}
			return result;
		}

		public PmxTextureTable()
		{
			NameToIndex = new Dictionary<string, int>();
			IndexToName = new Dictionary<int, string>();
		}

		public PmxTextureTable(PmxTextureTable tx)
		{
			FromPmxTextureTable(tx);
		}

		public void FromPmxTextureTable(PmxTextureTable tx)
		{
			string[] array = new string[tx.Count];
			tx.NameToIndex.Keys.CopyTo(array, 0);
			CreateTable(array);
		}

		public PmxTextureTable(List<PmxMaterial> ml)
			: this()
		{
			CreateTable(ml);
		}

		public void CreateTable(List<PmxMaterial> ml)
		{
			NameToIndex.Clear();
			IndexToName.Clear();
			int num = 0;
			for (int i = 0; i < ml.Count; i++)
			{
				PmxMaterial pmxMaterial = ml[i];
				if (!string.IsNullOrEmpty(pmxMaterial.Tex) && !NameToIndex.ContainsKey(pmxMaterial.Tex))
				{
					NameToIndex.Add(pmxMaterial.Tex, num);
					IndexToName.Add(num, pmxMaterial.Tex);
					num++;
				}
				if (!string.IsNullOrEmpty(pmxMaterial.Sphere) && !NameToIndex.ContainsKey(pmxMaterial.Sphere))
				{
					NameToIndex.Add(pmxMaterial.Sphere, num);
					IndexToName.Add(num, pmxMaterial.Sphere);
					num++;
				}
				if (!string.IsNullOrEmpty(pmxMaterial.Toon) && !NameToIndex.ContainsKey(pmxMaterial.Toon) && !SystemToon.IsSystemToon(pmxMaterial.Toon))
				{
					NameToIndex.Add(pmxMaterial.Toon, num);
					IndexToName.Add(num, pmxMaterial.Toon);
					num++;
				}
			}
		}

		public void CreateTable(string[] names)
		{
			NameToIndex.Clear();
			IndexToName.Clear();
			for (int i = 0; i < names.Length; i++)
			{
				if (!NameToIndex.ContainsKey(names[i]))
				{
					NameToIndex.Add(names[i], i);
				}
				IndexToName.Add(i, names[i]);
			}
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			int num = PmxStreamHelper.ReadElement_Int32(s);
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = PmxStreamHelper.ReadString(s, f);
			}
			CreateTable(array);
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			int count = Count;
			PmxStreamHelper.WriteElement_Int32(s, count);
			for (int i = 0; i < count; i++)
			{
				PmxStreamHelper.WriteString(s, IndexToName[i], f);
			}
		}

		object ICloneable.Clone()
		{
			return new PmxTextureTable(this);
		}

		public PmxTextureTable Clone()
		{
			return new PmxTextureTable(this);
		}
	}
}
