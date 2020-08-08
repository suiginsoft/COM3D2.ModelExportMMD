using System;
using System.Collections.Generic;

namespace PmxLib
{
	internal class VmdSelfShadow : VmdFrameBase, IBytesConvert, ICloneable
	{
		public int Mode;

		public float Distance;

		public int ByteCount => 9;

		public VmdSelfShadow()
		{
			Mode = 0;
			Distance = 0.011f;
		}

		public VmdSelfShadow(VmdSelfShadow shadow)
		{
			FrameIndex = shadow.FrameIndex;
			Mode = shadow.Mode;
			Distance = shadow.Distance;
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(FrameIndex));
			list.Add((byte)Mode);
			list.AddRange(BitConverter.GetBytes(Distance));
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			int num = startIndex;
			FrameIndex = BitConverter.ToInt32(bytes, num);
			num += 4;
			Mode = bytes[num++];
			Distance = BitConverter.ToSingle(bytes, num);
			num += 4;
		}

		public object Clone()
		{
			return new VmdSelfShadow(this);
		}
	}
}
