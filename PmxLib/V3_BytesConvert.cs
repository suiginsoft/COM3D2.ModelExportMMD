using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	internal static class V3_BytesConvert
	{
		public static readonly int UnitBytes = 12;

		public static int ByteCount => UnitBytes;

		public static byte[] ToBytes(Vector3 v3)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(v3.X));
			list.AddRange(BitConverter.GetBytes(v3.Y));
			list.AddRange(BitConverter.GetBytes(v3.Z));
			return list.ToArray();
		}

		public static Vector3 FromBytes(byte[] bytes, int startIndex)
		{
			int num = 4;
			float x = BitConverter.ToSingle(bytes, startIndex);
			float y = BitConverter.ToSingle(bytes, startIndex + num);
			float z = BitConverter.ToSingle(bytes, startIndex + num * 2);
			return new Vector3(x, y, z);
		}

		public static Vector3 FromStream(Stream s)
		{
			Vector3 zero = Vector3.Zero;
			byte[] array = new byte[12];
			s.Read(array, 0, 12);
			int num = 0;
			zero.X = BitConverter.ToSingle(array, num);
			num += 4;
			zero.Y = BitConverter.ToSingle(array, num);
			num += 4;
			zero.Z = BitConverter.ToSingle(array, num);
			return zero;
		}

		public static void ToStream(Stream s, Vector3 v)
		{
			s.Write(BitConverter.GetBytes(v.X), 0, 4);
			s.Write(BitConverter.GetBytes(v.Y), 0, 4);
			s.Write(BitConverter.GetBytes(v.Z), 0, 4);
		}
	}
}
