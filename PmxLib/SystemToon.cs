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
				this.ToonNames = SystemToon.GetToonNames();
				this.MaterialToon = new int[count];
				this.IsRejection = false;
			}
		}

		public const int EnableToonCount = 10;

		private static Dictionary<string, int> m_nametable;

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
				array[i] = SystemToon.GetToonName(i);
			}
			return array;
		}

		private static void CreateNameTable()
		{
			int num = 10;
			SystemToon.m_nametable = new Dictionary<string, int>(num + 1);
			for (int i = -1; i < num; i++)
			{
				SystemToon.m_nametable.Add(SystemToon.GetToonName(i), i);
			}
		}

		public static bool IsSystemToon(string name)
		{
			if (SystemToon.m_nametable == null)
			{
				SystemToon.CreateNameTable();
			}
			return !string.IsNullOrEmpty(name) && SystemToon.m_nametable.ContainsKey(name);
		}

		public static int GetToonIndex(string name)
		{
			if (SystemToon.m_nametable == null)
			{
				SystemToon.CreateNameTable();
			}
			if (!string.IsNullOrEmpty(name) && SystemToon.m_nametable.ContainsKey(name))
			{
				return SystemToon.m_nametable[name];
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
				}
				else
				{
					int toonIndex = SystemToon.GetToonIndex(pmxMaterial.Toon);
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
			}
			if (list2.Count > 0)
			{
				Dictionary<string, int> dictionary2 = new Dictionary<string, int>(list2.Count);
				int num = 0;
				for (int j = 0; j < list2.Count; j++)
				{
					int num2 = num;
					while (num2 < array.Length)
					{
						if (array[num2])
						{
							num2++;
							continue;
						}
						toonInfo.ToonNames[num2] = list2[j];
						dictionary2.Add(list2[j], num2);
						array[num2] = true;
						num = num2 + 1;
						break;
					}
				}
				for (int k = 0; k < count; k++)
				{
					if (toonInfo.MaterialToon[k] < -1)
					{
						PmxMaterial pmxMaterial2 = list[k];
						if (dictionary2.ContainsKey(pmxMaterial2.Toon))
						{
							toonInfo.MaterialToon[k] = dictionary2[pmxMaterial2.Toon];
						}
						else
						{
							toonInfo.MaterialToon[k] = -1;
							toonInfo.IsRejection = true;
						}
					}
				}
			}
			return toonInfo;
		}
	}
}
