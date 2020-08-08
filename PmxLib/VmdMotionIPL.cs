using System;
using System.Collections.Generic;

namespace PmxLib
{
	internal class VmdMotionIPL : IBytesConvert, ICloneable
	{
		public VmdIplData MoveX = new VmdIplData();

		public VmdIplData MoveY = new VmdIplData();

		public VmdIplData MoveZ = new VmdIplData();

		public VmdIplData Rotate = new VmdIplData();

		public int ByteCount => 16;

		public VmdMotionIPL()
		{
		}

		public VmdMotionIPL(VmdMotionIPL ipl)
		{
			MoveX = (VmdIplData)ipl.MoveX.Clone();
			MoveY = (VmdIplData)ipl.MoveY.Clone();
			MoveZ = (VmdIplData)ipl.MoveZ.Clone();
			Rotate = (VmdIplData)ipl.Rotate.Clone();
		}

		public byte[] ToBytes()
		{
			return new List<byte>
			{
				(byte)MoveX.P1.X,
				(byte)MoveX.P1.Y,
				(byte)MoveX.P2.X,
				(byte)MoveX.P2.Y,
				(byte)MoveY.P1.X,
				(byte)MoveY.P1.Y,
				(byte)MoveY.P2.X,
				(byte)MoveY.P2.Y,
				(byte)MoveZ.P1.X,
				(byte)MoveZ.P1.Y,
				(byte)MoveZ.P2.X,
				(byte)MoveZ.P2.Y,
				(byte)Rotate.P1.X,
				(byte)Rotate.P1.Y,
				(byte)Rotate.P2.X,
				(byte)Rotate.P2.Y
			}.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			int num = startIndex;
			MoveX.P1.X = bytes[num];
			num++;
			MoveX.P1.Y = bytes[num];
			num++;
			MoveX.P2.X = bytes[num];
			num++;
			MoveX.P2.Y = bytes[num];
			num++;
			MoveY.P1.X = bytes[num];
			num++;
			MoveY.P1.Y = bytes[num];
			num++;
			MoveY.P2.X = bytes[num];
			num++;
			MoveY.P2.Y = bytes[num];
			num++;
			MoveZ.P1.X = bytes[num];
			num++;
			MoveZ.P1.Y = bytes[num];
			num++;
			MoveZ.P2.X = bytes[num];
			num++;
			MoveZ.P2.Y = bytes[num];
			num++;
			Rotate.P1.X = bytes[num];
			num++;
			Rotate.P1.Y = bytes[num];
			num++;
			Rotate.P2.X = bytes[num];
			num++;
			Rotate.P2.Y = bytes[num];
			num++;
		}

		public object Clone()
		{
			return new VmdMotionIPL(this);
		}
	}
}
