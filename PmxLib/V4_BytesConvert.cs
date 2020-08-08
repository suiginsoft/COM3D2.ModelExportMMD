using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	internal static class V4_BytesConvert
	{
		public static readonly int UnitBytes = 16;

		public static int ByteCount => UnitBytes;

		public static byte[] ToBytes(Vector4 v4)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(v4.X));
			list.AddRange(BitConverter.GetBytes(v4.Y));
			list.AddRange(BitConverter.GetBytes(v4.Z));
			list.AddRange(BitConverter.GetBytes(v4.W));
			return list.ToArray();
		}

		public static Vector4 FromBytes(byte[] bytes, int startIndex)
		{
			int num = 4;
			float x = BitConverter.ToSingle(bytes, startIndex);
			float y = BitConverter.ToSingle(bytes, startIndex + num);
			float z = BitConverter.ToSingle(bytes, startIndex + num * 2);
			float w = BitConverter.ToSingle(bytes, startIndex + num * 3);
			return new Vector4(x, y, z, w);
		}

		public static Vector4 FromStream(Stream s)
		{
			Vector4 zero = Vector4.Zero;
			byte[] array = new byte[16];
			s.Read(array, 0, 16);
			int num = 0;
			zero.X = BitConverter.ToSingle(array, num);
			num += 4;
			zero.Y = BitConverter.ToSingle(array, num);
			num += 4;
			zero.Z = BitConverter.ToSingle(array, num);
			num += 4;
			zero.W = BitConverter.ToSingle(array, num);
			return zero;
		}

		public static void ToStream(Stream s, Vector4 v)
		{
			s.Write(BitConverter.GetBytes(v.X), 0, 4);
			s.Write(BitConverter.GetBytes(v.Y), 0, 4);
			s.Write(BitConverter.GetBytes(v.Z), 0, 4);
			s.Write(BitConverter.GetBytes(v.W), 0, 4);
		}
	}
}
