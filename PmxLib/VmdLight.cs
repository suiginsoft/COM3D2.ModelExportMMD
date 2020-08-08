using System;
using System.Collections.Generic;

namespace PmxLib
{
	internal class VmdLight : VmdFrameBase, IBytesConvert, ICloneable
	{
		public Vector3 Color;
		public Vector3 Direction;
		public int ByteCount => 28;

		public VmdLight()
		{
		}

		public VmdLight(VmdLight light)
			: this()
		{
			FrameIndex = light.FrameIndex;
			Color = light.Color;
			Direction = light.Direction;
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(FrameIndex));
			list.AddRange(BitConverter.GetBytes(Color.Red));
			list.AddRange(BitConverter.GetBytes(Color.Green));
			list.AddRange(BitConverter.GetBytes(Color.Blue));
			list.AddRange(BitConverter.GetBytes(Direction.X));
			list.AddRange(BitConverter.GetBytes(Direction.Y));
			list.AddRange(BitConverter.GetBytes(Direction.Z));
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			FrameIndex = BitConverter.ToInt32(bytes, startIndex + 0);
			Color.Red = BitConverter.ToSingle(bytes, startIndex + 4);
			Color.Green = BitConverter.ToSingle(bytes, startIndex + 8);
			Color.Blue = BitConverter.ToSingle(bytes, startIndex + 12);
			Direction.X = BitConverter.ToSingle(bytes, startIndex + 16);
			Direction.Y = BitConverter.ToSingle(bytes, startIndex + 20);
			Direction.Z = BitConverter.ToSingle(bytes, startIndex + 24);
		}

		public object Clone()
		{
			return new VmdLight(this);
		}
	}
}
