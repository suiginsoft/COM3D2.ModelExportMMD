using System;
using System.Collections.Generic;

namespace PmxLib
{
	public class VmdSelfShadow : VmdFrameBase, IBytesConvert, ICloneable
	{
		public int Mode;

		public float Distance;

		public int ByteCount
		{
			get
			{
				return 9;
			}
		}

		public VmdSelfShadow()
		{
			this.Mode = 0;
			this.Distance = 0.011f;
		}

		public VmdSelfShadow(VmdSelfShadow shadow)
		{
			base.FrameIndex = shadow.FrameIndex;
			this.Mode = shadow.Mode;
			this.Distance = shadow.Distance;
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(base.FrameIndex));
			list.Add((byte)this.Mode);
			list.AddRange(BitConverter.GetBytes(this.Distance));
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			base.FrameIndex = BitConverter.ToInt32(bytes, startIndex);
			int num = startIndex + 4;
			this.Mode = bytes[num++];
			this.Distance = BitConverter.ToSingle(bytes, num);
			num += 4;
		}

		public object Clone()
		{
			return new VmdSelfShadow(this);
		}
	}
}
