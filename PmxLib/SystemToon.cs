using System.Collections.Generic;

namespace PmxLib
{
	internal class SystemToon
	{
		public class ToonInfo
		{
			public string[] ToonNames
			{
				get;
				set;
			}

			public int[] MaterialToon
			{
				get;
				set;
			}

			public bool IsRejection
			{
				get;
				set;
			}

			public ToonInfo(int count)
			{
				ToonNames = GetToonNames();
				MaterialToon = new int[count];
				IsRejection = false;
			}
		}

		private static Dictionary<string, int> m_nametable;

		public const int EnableToonCount = 10;

		public static string GetToonName(int n)
		{
			if (n < 0)
			{
				return "toon0.bmp";
			}
			return "toon" + (n + 1).ToString("00") + ".bmp";
		}

		public static string[] GetToonNames()
		{
			string[] array = new string[10];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = GetToonName(i);
			}
			return array;
		}

		private static void CreateNameTable()
		{
			int num = 10;
			m_nametable = new Dictionary<string, int>(num + 1);
			for (int i = -1; i < num; i++)
			{
				m_nametable.Add(GetToonName(i), i);
			}
		}

		public static bool IsSystemToon(string name)
		{
			if (m_nametable == null)
			{
				CreateNameTable();
			}
			if (!string.IsNullOrEmpty(name))
			{
				return m_nametable.ContainsKey(name);
			}
			return false;
		}

		public static int GetToonIndex(string name)
		{
			if (m_nametable == null)
			{
				CreateNameTable();
			}
			if (!string.IsNullOrEmpty(name) && m_nametable.ContainsKey(name))
			{
				return m_nametable[name];
			}
			return -2;
		}

		public ToonInfo GetToonInfo(List<PmxMaterial> list)
		{
			int count = list.Count;
			if (count < 0)
			{
				return null;
			}
			ToonInfo toonInfo = new ToonInfo(count);
			bool[] array = new bool[10];
			List<string> list2 = new List<string>(count);
			Dictionary<string, int> dictionary = new Dictionary<string, int>(count);
			for (int i = 0; i < count; i++)
			{
				PmxMaterial pmxMaterial = list[i];
				if (string.IsNullOrEmpty(pmxMaterial.Toon))
				{
					toonInfo.MaterialToon[i] = -1;
					continue;
				}
				int toonIndex = GetToonIndex(pmxMaterial.Toon);
				toonInfo.MaterialToon[i] = toonIndex;
				if (-1 <= toonIndex && toonIndex < 10)
				{
					if (toonIndex >= 0)
					{
						array[toonIndex] = true;
					}
				}
				else if (!dictionary.ContainsKey(pmxMaterial.Toon))
				{
					list2.Add(pmxMaterial.Toon);
					dictionary.Add(pmxMaterial.Toon, 0);
				}
			}
			if (list2.Count > 0)
			{
				Dictionary<string, int> dictionary2 = new Dictionary<string, int>(list2.Count);
				int num = 0;
				for (int j = 0; j < list2.Count; j++)
				{
					for (int k = num; k < array.Length; k++)
					{
						if (!array[k])
						{
							toonInfo.ToonNames[k] = list2[j];
							dictionary2.Add(list2[j], k);
							array[k] = true;
							num = k + 1;
							break;
						}
					}
				}
				for (int l = 0; l < count; l++)
				{
					if (toonInfo.MaterialToon[l] < -1)
					{
						PmxMaterial pmxMaterial2 = list[l];
						if (dictionary2.ContainsKey(pmxMaterial2.Toon))
						{
							toonInfo.MaterialToon[l] = dictionary2[pmxMaterial2.Toon];
							continue;
						}
						toonInfo.MaterialToon[l] = -1;
						toonInfo.IsRejection = true;
					}
				}
			}
			return toonInfo;
		}
	}
}
