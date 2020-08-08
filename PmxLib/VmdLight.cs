using System;
using System.Collections.Generic;
using UnityEngine;

namespace PmxLib
{
	public class VmdLight : VmdFrameBase, IBytesConvert, ICloneable
	{
		public Color Color;

		public Vector3 Direction;

		public int ByteCount
		{
			get
			{
				return 28;
			}
		}

		public VmdLight()
		{
		}

		public VmdLight(VmdLight light)
			: this()
		{
			base.FrameIndex = light.FrameIndex;
			this.Color = light.Color;
			this.Direction = light.Direction;
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(base.FrameIndex));
			list.AddRange(BitConverter.GetBytes(this.Color.r));
			list.AddRange(BitConverter.GetBytes(this.Color.g));
			list.AddRange(BitConverter.GetBytes(this.Color.b));
			list.AddRange(BitConverter.GetBytes(this.Direction.x));
			list.AddRange(BitConverter.GetBytes(this.Direction.y));
			list.AddRange(BitConverter.GetBytes(this.Direction.z));
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			base.FrameIndex = BitConverter.ToInt32(bytes, startIndex);
			int num = startIndex + 4;
			this.Color.r = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.Color.g = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.Color.b = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.Direction.x = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.Direction.y = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.Direction.z = BitConverter.ToSingle(bytes, num);
		}

		public object Clone()
		{
			return new VmdLight(this);
		}
	}
}
