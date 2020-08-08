using System;
using System.IO;
using System.Text;

namespace PmxLib
{
	internal static class PmxStreamHelper
	{
		public static void WriteString(Stream s, string str, int f)
		{
			switch (f)
			{
			case 0:
				WriteString_v2(s, str, Encoding.Unicode);
				break;
			case 1:
				WriteString_v2(s, str, Encoding.UTF8);
				break;
			}
		}

		public static string ReadString(Stream s, int f)
		{
			string result = "";
			switch (f)
			{
			case 0:
				result = ReadString_v2(s, Encoding.Unicode);
				break;
			case 1:
				result = ReadString_v2(s, Encoding.UTF8);
				break;
			}
			return result;
		}

		public static void WriteString(Stream s, string str, PmxElementFormat f)
		{
			if (f == null)
			{
				f = new PmxElementFormat();
			}
			if (f.Ver <= 1f)
			{
				WriteString_v1(s, str);
			}
			else if (f.Ver <= 2.1f)
			{
				if (f.StringEnc == PmxElementFormat.StringEncType.UTF8)
				{
					WriteString_v2(s, str, Encoding.UTF8);
				}
				else
				{
					WriteString_v2(s, str, Encoding.Unicode);
				}
			}
		}

		public static string ReadString(Stream s, PmxElementFormat f)
		{
			if (f == null)
			{
				f = new PmxElementFormat();
			}
			string result = "";
			if (f.Ver <= 1f)
			{
				result = ReadString_v1(s);
			}
			else if (f.Ver <= 2.1f)
			{
				result = ((f.StringEnc != PmxElementFormat.StringEncType.UTF8) ? ReadString_v2(s, Encoding.Unicode) : ReadString_v2(s, Encoding.UTF8));
			}
			return result;
		}

		public static void WriteString_v1(Stream s, string str)
		{
			byte[] array = BytesStringProc.StringToBuf_SJIS(str);
			byte[] bytes = BitConverter.GetBytes(array.Length);
			s.Write(bytes, 0, bytes.Length);
			if (array.Length != 0)
			{
				s.Write(array, 0, array.Length);
			}
		}

		public static string ReadString_v1(Stream s)
		{
			string result = "";
			byte[] array = new byte[4];
			s.Read(array, 0, 4);
			int num = BitConverter.ToInt32(array, 0);
			if (num > 0)
			{
				array = new byte[num];
				s.Read(array, 0, num);
				try
				{
					return BytesStringProc.BufToString_SJIS(array);
				}
				catch (Exception)
				{
					return "";
				}
			}
			return result;
		}

		public static void WriteString_v2(Stream s, string str, Encoding ec = null)
		{
			if (ec == null)
			{
				ec = Encoding.Unicode;
			}
			byte[] bytes = ec.GetBytes(str);
			byte[] bytes2 = BitConverter.GetBytes(bytes.Length);
			s.Write(bytes2, 0, bytes2.Length);
			if (bytes.Length != 0)
			{
				s.Write(bytes, 0, bytes.Length);
			}
		}

		public static string ReadString_v2(Stream s, Encoding ec = null)
		{
			if (ec == null)
			{
				ec = Encoding.Unicode;
			}
			string result = "";
			byte[] array = new byte[4];
			s.Read(array, 0, 4);
			int num = BitConverter.ToInt32(array, 0);
			if (num > 0)
			{
				array = new byte[num];
				if (s.Read(array, 0, num) > 0)
				{
					try
					{
						return ec.GetString(array, 0, array.Length);
					}
					catch (Exception)
					{
						return "";
					}
				}
			}
			return result;
		}

		public static void WriteElement_Bool(Stream s, bool data)
		{
			WriteElement_Int32(s, data ? 1 : 0, 1, signed: false);
		}

		public static bool ReadElement_Bool(Stream s)
		{
			return ReadElement_Int32(s, 1, signed: false) != 0;
		}

		public static void WriteElement_Int32(Stream s, int data, int bufSize = 4, bool signed = true)
		{
			byte[] array = null;
			switch (bufSize)
			{
			case 1:
				array = ((!signed) ? new byte[1]
				{
					(byte)data
				} : new byte[1]
				{
					(byte)(sbyte)data
				});
				break;
			case 2:
				array = ((!signed) ? BitConverter.GetBytes((ushort)data) : BitConverter.GetBytes((short)data));
				break;
			case 4:
				array = BitConverter.GetBytes(data);
				break;
			}
			s.Write(array, 0, array.Length);
		}

		public static int ReadElement_Int32(Stream s, int bufSize = 4, bool signed = true)
		{
			int result = 0;
			byte[] array = new byte[bufSize];
			s.Read(array, 0, bufSize);
			switch (bufSize)
			{
			case 1:
				result = ((!signed) ? ((int)array[0]) : ((int)(sbyte)array[0]));
				break;
			case 2:
				result = ((!signed) ? ((int)BitConverter.ToUInt16(array, 0)) : ((int)BitConverter.ToInt16(array, 0)));
				break;
			case 4:
				result = BitConverter.ToInt32(array, 0);
				break;
			}
			return result;
		}

		public static void WriteElement_UInt(Stream s, uint data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			s.Write(bytes, 0, bytes.Length);
		}

		public static uint ReadElement_UInt(Stream s)
		{
			byte[] array = new byte[4];
			s.Read(array, 0, 4);
			return BitConverter.ToUInt32(array, 0);
		}

		public static void WriteElement_Float(Stream s, float data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			s.Write(bytes, 0, bytes.Length);
		}

		public static float ReadElement_Float(Stream s)
		{
			byte[] array = new byte[4];
			s.Read(array, 0, 4);
			return BitConverter.ToSingle(array, 0);
		}

		public static void WriteElement_Vector2(Stream s, Vector2 data)
		{
			WriteElement_Float(s, data.X);
			WriteElement_Float(s, data.Y);
		}

		public static Vector2 ReadElement_Vector2(Stream s)
		{
			Vector2 result = default(Vector2);
			result.X = ReadElement_Float(s);
			result.Y = ReadElement_Float(s);
			return result;
		}

		public static void WriteElement_Vector3(Stream s, Vector3 data)
		{
			WriteElement_Float(s, data.X);
			WriteElement_Float(s, data.Y);
			WriteElement_Float(s, data.Z);
		}

		public static Vector3 ReadElement_Vector3(Stream s)
		{
			Vector3 result = default(Vector3);
			result.X = ReadElement_Float(s);
			result.Y = ReadElement_Float(s);
			result.Z = ReadElement_Float(s);
			return result;
		}

		public static void WriteElement_Vector4(Stream s, Vector4 data)
		{
			WriteElement_Float(s, data.X);
			WriteElement_Float(s, data.Y);
			WriteElement_Float(s, data.Z);
			WriteElement_Float(s, data.W);
		}

		public static Vector4 ReadElement_Vector4(Stream s)
		{
			Vector4 result = default(Vector4);
			result.X = ReadElement_Float(s);
			result.Y = ReadElement_Float(s);
			result.Z = ReadElement_Float(s);
			result.W = ReadElement_Float(s);
			return result;
		}

		public static void WriteElement_Quaternion(Stream s, Quaternion data)
		{
			WriteElement_Float(s, data.X);
			WriteElement_Float(s, data.Y);
			WriteElement_Float(s, data.Z);
			WriteElement_Float(s, data.W);
		}

		public static Quaternion ReadElement_Quaternion(Stream s)
		{
			Quaternion result = default(Quaternion);
			result.X = ReadElement_Float(s);
			result.Y = ReadElement_Float(s);
			result.Z = ReadElement_Float(s);
			result.W = ReadElement_Float(s);
			return result;
		}

		public static void WriteElement_Matrix(Stream s, Matrix m)
		{
			float[] array = m.ToArray();
			foreach (float data in array)
			{
				WriteElement_Float(s, data);
			}
		}

		public static Matrix ReadElement_Matrix(Stream s)
		{
			Matrix result = default(Matrix);
			result.M11 = ReadElement_Float(s);
			result.M12 = ReadElement_Float(s);
			result.M13 = ReadElement_Float(s);
			result.M14 = ReadElement_Float(s);
			result.M21 = ReadElement_Float(s);
			result.M22 = ReadElement_Float(s);
			result.M23 = ReadElement_Float(s);
			result.M24 = ReadElement_Float(s);
			result.M31 = ReadElement_Float(s);
			result.M32 = ReadElement_Float(s);
			result.M33 = ReadElement_Float(s);
			result.M34 = ReadElement_Float(s);
			result.M41 = ReadElement_Float(s);
			result.M42 = ReadElement_Float(s);
			result.M43 = ReadElement_Float(s);
			result.M44 = ReadElement_Float(s);
			return result;
		}
	}
}
