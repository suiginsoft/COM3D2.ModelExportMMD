using System;
using System.Collections.Generic;

namespace PmxLib
{
	public class VmdCameraIPL : IBytesConvert, ICloneable
	{
		public VmdIplData MoveX = new VmdIplData();

		public VmdIplData MoveY = new VmdIplData();

		public VmdIplData MoveZ = new VmdIplData();

		public VmdIplData Rotate = new VmdIplData();

		public VmdIplData Distance = new VmdIplData();

		public VmdIplData Angle = new VmdIplData();

		public int ByteCount
		{
			get
			{
				return 24;
			}
		}

		public VmdCameraIPL()
		{
		}

		public VmdCameraIPL(VmdCameraIPL ipl)
		{
			this.MoveX = (VmdIplData)ipl.MoveX.Clone();
			this.MoveY = (VmdIplData)ipl.MoveY.Clone();
			this.MoveZ = (VmdIplData)ipl.MoveZ.Clone();
			this.Rotate = (VmdIplData)ipl.Rotate.Clone();
			this.Distance = (VmdIplData)ipl.Distance.Clone();
			this.Angle = (VmdIplData)ipl.Angle.Clone();
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			list.Add((byte)this.MoveX.P1.X);
			list.Add((byte)this.MoveX.P2.X);
			list.Add((byte)this.MoveX.P1.Y);
			list.Add((byte)this.MoveX.P2.Y);
			list.Add((byte)this.MoveY.P1.X);
			list.Add((byte)this.MoveY.P2.X);
			list.Add((byte)this.MoveY.P1.Y);
			list.Add((byte)this.MoveY.P2.Y);
			list.Add((byte)this.MoveZ.P1.X);
			list.Add((byte)this.MoveZ.P2.X);
			list.Add((byte)this.MoveZ.P1.Y);
			list.Add((byte)this.MoveZ.P2.Y);
			list.Add((byte)this.Rotate.P1.X);
			list.Add((byte)this.Rotate.P2.X);
			list.Add((byte)this.Rotate.P1.Y);
			list.Add((byte)this.Rotate.P2.Y);
			list.Add((byte)this.Distance.P1.X);
			list.Add((byte)this.Distance.P2.X);
			list.Add((byte)this.Distance.P1.Y);
			list.Add((byte)this.Distance.P2.Y);
			list.Add((byte)this.Angle.P1.X);
			list.Add((byte)this.Angle.P2.X);
			list.Add((byte)this.Angle.P1.Y);
			list.Add((byte)this.Angle.P2.Y);
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			this.MoveX.P1.X = bytes[startIndex];
			int num = startIndex + 1;
			this.MoveX.P2.X = bytes[num];
			num++;
			this.MoveX.P1.Y = bytes[num];
			num++;
			this.MoveX.P2.Y = bytes[num];
			num++;
			this.MoveY.P1.X = bytes[num];
			num++;
			this.MoveY.P2.X = bytes[num];
			num++;
			this.MoveY.P1.Y = bytes[num];
			num++;
			this.MoveY.P2.Y = bytes[num];
			num++;
			this.MoveZ.P1.X = bytes[num];
			num++;
			this.MoveZ.P2.X = bytes[num];
			num++;
			this.MoveZ.P1.Y = bytes[num];
			num++;
			this.MoveZ.P2.Y = bytes[num];
			num++;
			this.Rotate.P1.X = bytes[num];
			num++;
			this.Rotate.P2.X = bytes[num];
			num++;
			this.Rotate.P1.Y = bytes[num];
			num++;
			this.Rotate.P2.Y = bytes[num];
			num++;
			this.Distance.P1.X = bytes[num];
			num++;
			this.Distance.P2.X = bytes[num];
			num++;
			this.Distance.P1.Y = bytes[num];
			num++;
			this.Distance.P2.Y = bytes[num];
			num++;
			this.Angle.P1.X = bytes[num];
			num++;
			this.Angle.P2.X = bytes[num];
			num++;
			this.Angle.P1.Y = bytes[num];
			num++;
			this.Angle.P2.Y = bytes[num];
			num++;
		}

		public object Clone()
		{
			return new VmdCameraIPL(this);
		}
	}
}
