using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	public class PmxTextureTable : IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.TexTable;
			}
		}

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

		public int Count
		{
			get
			{
				return this.NameToIndex.Count;
			}
		}

		public int GetIndex(string name)
		{
			int result = -1;
			if (this.NameToIndex.ContainsKey(name))
			{
				result = this.NameToIndex[name];
			}
			return result;
		}

		public string GetName(int ix)
		{
			string result = "";
			if (this.IndexToName.ContainsKey(ix))
			{
				result = this.IndexToName[ix];
			}
			return result;
		}

		public PmxTextureTable()
		{
			this.NameToIndex = new Dictionary<string, int>();
			this.IndexToName = new Dictionary<int, string>();
		}

		public PmxTextureTable(PmxTextureTable tx)
		{
			this.FromPmxTextureTable(tx);
		}

		public void FromPmxTextureTable(PmxTextureTable tx)
		{
			string[] array = new string[tx.Count];
			tx.NameToIndex.Keys.CopyTo(array, 0);
			this.CreateTable(array);
		}

		public PmxTextureTable(List<PmxMaterial> ml)
			: this()
		{
			this.CreateTable(ml);
		}

		public void CreateTable(List<PmxMaterial> ml)
		{
			this.NameToIndex.Clear();
			this.IndexToName.Clear();
			int num = 0;
			for (int i = 0; i < ml.Count; i++)
			{
				PmxMaterial pmxMaterial = ml[i];
				if (!string.IsNullOrEmpty(pmxMaterial.Tex) && !this.NameToIndex.ContainsKey(pmxMaterial.Tex))
				{
					this.NameToIndex.Add(pmxMaterial.Tex, num);
					this.IndexToName.Add(num, pmxMaterial.Tex);
					num++;
				}
				if (!string.IsNullOrEmpty(pmxMaterial.Sphere) && !this.NameToIndex.ContainsKey(pmxMaterial.Sphere))
				{
					this.NameToIndex.Add(pmxMaterial.Sphere, num);
					this.IndexToName.Add(num, pmxMaterial.Sphere);
					num++;
				}
				if (!string.IsNullOrEmpty(pmxMaterial.Toon) && !this.NameToIndex.ContainsKey(pmxMaterial.Toon) && !SystemToon.IsSystemToon(pmxMaterial.Toon))
				{
					this.NameToIndex.Add(pmxMaterial.Toon, num);
					this.IndexToName.Add(num, pmxMaterial.Toon);
					num++;
				}
			}
		}

		public void CreateTable(string[] names)
		{
			this.NameToIndex.Clear();
			this.IndexToName.Clear();
			for (int i = 0; i < names.Length; i++)
			{
				this.NameToIndex.Add(names[i], i);
				this.IndexToName.Add(i, names[i]);
			}
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			int num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = PmxStreamHelper.ReadString(s, f);
			}
			this.CreateTable(array);
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			int count = this.Count;
			PmxStreamHelper.WriteElement_Int32(s, count, 4, true);
			for (int i = 0; i < count; i++)
			{
				PmxStreamHelper.WriteString(s, this.IndexToName[i], f);
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
