using System;

namespace PmxLib
{
	internal static class PmxLibClass
	{
		private static bool m_lock;

		static PmxLibClass()
		{
			PmxLibClass.m_lock = false;
		}

		public static void Unlock(string key)
		{
			PmxLibClass.m_lock = true;
			if (key == PmxLibClass.RString(-167698971, "UnlockPmxLibClass"))
			{
				PmxLibClass.m_lock = false;
			}
		}

		public static bool IsLocked()
		{
			return PmxLibClass.m_lock;
		}

		public static string RString(int s, string str)
		{
			Random random = new Random(s);
			char[] array = str.ToCharArray();
			int num = array.Length;
			while (num > 1)
			{
				num--;
				int num2 = random.Next(num + 1);
				char c = array[num2];
				array[num2] = array[num];
				array[num] = c;
			}
			return new string(array);
		}
	}
}
