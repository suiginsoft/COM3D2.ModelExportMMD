using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PmxLib
{
	internal static class V3_BytesConvert
	{
		public static readonly int UnitBytes = 12;

		public static int ByteCount
		{
			get
			{
				return V3_BytesConvert.UnitBytes;
			}
		}

		public static byte[] ToBytes(Vector3 v3)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(v3.x));
			list.AddRange(BitConverter.GetBytes(v3.y));
			list.AddRange(BitConverter.GetBytes(v3.z));
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
			Vector3 zero = Vector3.zero;
			byte[] array = new byte[12];
			s.Read(array, 0, 12);
			int num = 0;
			zero.x = BitConverter.ToSingle(array, num);
			num += 4;
			zero.y = BitConverter.ToSingle(array, num);
			num += 4;
			zero.z = BitConverter.ToSingle(array, num);
			return zero;
		}

		public static void ToStream(Stream s, Vector3 v)
		{
			s.Write(BitConverter.GetBytes(v.x), 0, 4);
			s.Write(BitConverter.GetBytes(v.y), 0, 4);
			s.Write(BitConverter.GetBytes(v.z), 0, 4);
		}

		public static Color Vector3ToColor(Vector3 v)
		{
			Color result = default(Color);
			result.r = v.x;
			result.g = v.y;
			result.b = v.z;
			return result;
		}

		public static Vector3 ColorToVector3(Color color)
		{
			Vector3 result = default(Vector3);
			result.x = color.r;
			result.y = color.g;
			result.z = color.b;
			return result;
		}
	}
}
