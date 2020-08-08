using System;
using System.Collections.Generic;
using System.Text;

namespace PmxLib
{
	internal static class BytesStringProc
	{
		private static Encoding m_sjis;

		static BytesStringProc()
		{
			m_sjis = Encoding.GetEncoding("shift_jis");
		}

		public static void SetString(byte[] buf, string s, byte fix = 0, byte fill = 253)
		{
			s = s.Replace("\r\n", "\n");
			List<byte> list = new List<byte>(m_sjis.GetBytes(s));
			list.Add(fix);
			if (list.Count > buf.Length)
			{
				list[buf.Length - 1] = fix;
			}
			for (int i = 0; i < buf.Length; i++)
			{
				buf[i] = fill;
			}
			byte[] array = list.ToArray();
			int length = Math.Min(buf.Length, array.Length);
			Array.Copy(array, buf, length);
		}

		public static string GetString(byte[] buf, byte fix = 0)
		{
			int num = buf.Length;
			int count = buf.Length;
			for (int i = 0; i < num; i++)
			{
				if (buf[i] == fix)
				{
					count = i;
					break;
				}
			}
			string @string = m_sjis.GetString(buf, 0, count);
			if (@string == null)
			{
				return "";
			}
			return @string.Replace("\n", "\r\n");
		}

		public static byte[] StringToBuf_SJIS(string s)
		{
			if (s.Length <= 0)
			{
				return new byte[0];
			}
			return m_sjis.GetBytes(s);
		}

		public static string BufToString_SJIS(byte[] buf)
		{
			if (buf.Length == 0)
			{
				return "";
			}
			return m_sjis.GetString(buf, 0, buf.Length);
		}
	}
}
