using System;
using System.Collections.Generic;

namespace PmxLib
{
	public class VmdMotionIPL : IBytesConvert, ICloneable
	{
		public VmdIplData MoveX = new VmdIplData();

		public VmdIplData MoveY = new VmdIplData();

		public VmdIplData MoveZ = new VmdIplData();

		public VmdIplData Rotate = new VmdIplData();

		public int ByteCount
		{
			get
			{
				return 16;
			}
		}

		public VmdMotionIPL()
		{
		}

		public VmdMotionIPL(VmdMotionIPL ipl)
		{
			this.MoveX = (VmdIplData)ipl.MoveX.Clone();
			this.MoveY = (VmdIplData)ipl.MoveY.Clone();
			this.MoveZ = (VmdIplData)ipl.MoveZ.Clone();
			this.Rotate = (VmdIplData)ipl.Rotate.Clone();
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			list.Add((byte)this.MoveX.P1.X);
			list.Add((byte)this.MoveX.P1.Y);
			list.Add((byte)this.MoveX.P2.X);
			list.Add((byte)this.MoveX.P2.Y);
			list.Add((byte)this.MoveY.P1.X);
			list.Add((byte)this.MoveY.P1.Y);
			list.Add((byte)this.MoveY.P2.X);
			list.Add((byte)this.MoveY.P2.Y);
			list.Add((byte)this.MoveZ.P1.X);
			list.Add((byte)this.MoveZ.P1.Y);
			list.Add((byte)this.MoveZ.P2.X);
			list.Add((byte)this.MoveZ.P2.Y);
			list.Add((byte)this.Rotate.P1.X);
			list.Add((byte)this.Rotate.P1.Y);
			list.Add((byte)this.Rotate.P2.X);
			list.Add((byte)this.Rotate.P2.Y);
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			this.MoveX.P1.X = bytes[startIndex];
			int num = startIndex + 1;
			this.MoveX.P1.Y = bytes[num];
			num++;
			this.MoveX.P2.X = bytes[num];
			num++;
			this.MoveX.P2.Y = bytes[num];
			num++;
			this.MoveY.P1.X = bytes[num];
			num++;
			this.MoveY.P1.Y = bytes[num];
			num++;
			this.MoveY.P2.X = bytes[num];
			num++;
			this.MoveY.P2.Y = bytes[num];
			num++;
			this.MoveZ.P1.X = bytes[num];
			num++;
			this.MoveZ.P1.Y = bytes[num];
			num++;
			this.MoveZ.P2.X = bytes[num];
			num++;
			this.MoveZ.P2.Y = bytes[num];
			num++;
			this.Rotate.P1.X = bytes[num];
			num++;
			this.Rotate.P1.Y = bytes[num];
			num++;
			this.Rotate.P2.X = bytes[num];
			num++;
			this.Rotate.P2.Y = bytes[num];
			num++;
		}

		public object Clone()
		{
			return new VmdMotionIPL(this);
		}
	}
}
