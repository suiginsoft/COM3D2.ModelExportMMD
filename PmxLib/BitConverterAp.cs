using System;
using System.Collections.Generic;
using System.Linq;

namespace PmxLib
{
	internal static class BitConverterAp
	{
		public static IEnumerable<byte> GetBytes(Vector2 v)
		{
			return BitConverter.GetBytes(v.X).Concat(BitConverter.GetBytes(v.Y));
		}

		public static IEnumerable<byte> GetBytes(Vector3 v)
		{
			return BitConverter.GetBytes(v.X).Concat(BitConverter.GetBytes(v.Y)).Concat(BitConverter.GetBytes(v.Z));
		}

		public static IEnumerable<byte> GetBytes(Vector4 v)
		{
			return BitConverter.GetBytes(v.X).Concat(BitConverter.GetBytes(v.Y)).Concat(BitConverter.GetBytes(v.Z).Concat(BitConverter.GetBytes(v.W)));
		}

		public static IEnumerable<byte> GetBytes(Quaternion q)
		{
			return BitConverter.GetBytes(q.X).Concat(BitConverter.GetBytes(q.Y)).Concat(BitConverter.GetBytes(q.Z).Concat(BitConverter.GetBytes(q.W)));
		}

		public static IEnumerable<byte> GetBytes(Matrix m)
		{
			try
			{
				float[] array3 = m.ToArray();
				foreach (float value in array3)
				{
					byte[] bytes = BitConverter.GetBytes(value);
					try
					{
						byte[] array2 = bytes;
						foreach (byte b in array2)
						{
							yield return b;
						}
					}
					finally
					{
					}
				}
			}
			finally
			{
			}
		}

		public static Vector2 ToVector2(byte[] buf, int startIndex = 0)
		{
			Vector2 result = default(Vector2);
			result.X = BitConverter.ToSingle(buf, startIndex);
			int startIndex2 = startIndex + 4;
			result.Y = BitConverter.ToSingle(buf, startIndex2);
			return result;
		}

		public static Vector3 ToVector3(byte[] buf, int startIndex = 0)
		{
			Vector3 result = default(Vector3);
			result.X = BitConverter.ToSingle(buf, startIndex);
			int num = startIndex + 4;
			result.Y = BitConverter.ToSingle(buf, num);
			num += 4;
			result.Z = BitConverter.ToSingle(buf, num);
			return result;
		}

		public static Vector4 ToVector4(byte[] buf, int startIndex = 0)
		{
			Vector4 result = default(Vector4);
			result.X = BitConverter.ToSingle(buf, startIndex);
			int num = startIndex + 4;
			result.Y = BitConverter.ToSingle(buf, num);
			num += 4;
			result.Z = BitConverter.ToSingle(buf, num);
			num += 4;
			result.W = BitConverter.ToSingle(buf, num);
			return result;
		}

		public static Quaternion ToQuaternion(byte[] buf, int startIndex = 0)
		{
			Quaternion result = default(Quaternion);
			result.X = BitConverter.ToSingle(buf, startIndex);
			int num = startIndex + 4;
			result.Y = BitConverter.ToSingle(buf, num);
			num += 4;
			result.Z = BitConverter.ToSingle(buf, num);
			num += 4;
			result.W = BitConverter.ToSingle(buf, num);
			return result;
		}

		public static Matrix ToMatrix(byte[] buf, int startIndex = 0)
		{
			Matrix result = default(Matrix);
			result.M11 = BitConverter.ToSingle(buf, startIndex);
			int num = startIndex + 4;
			result.M12 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M13 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M14 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M21 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M22 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M23 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M24 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M31 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M32 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M33 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M34 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M41 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M42 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M43 = BitConverter.ToSingle(buf, num);
			num += 4;
			result.M44 = BitConverter.ToSingle(buf, num);
			return result;
		}
	}
}
