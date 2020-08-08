using System;
using System.Collections.Generic;

namespace PmxLib
{
	internal class VmdCamera : VmdFrameBase, IBytesConvert, ICloneable
	{
		public float Distance;

		public Vector3 Position;

		public Vector3 Rotate;

		public VmdCameraIPL IPL = new VmdCameraIPL();

		public float Angle;

		public byte Pers;

		public int ByteCount => 32 + IPL.ByteCount + 1 + 4;

		public VmdCamera()
		{
		}

		public VmdCamera(VmdCamera camera)
		{
			FrameIndex = camera.FrameIndex;
			Distance = camera.Distance;
			Position = camera.Position;
			Rotate = camera.Rotate;
			IPL = (VmdCameraIPL)camera.IPL.Clone();
			Angle = camera.Angle;
			Pers = camera.Pers;
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(FrameIndex));
			list.AddRange(BitConverter.GetBytes(Distance));
			list.AddRange(BitConverter.GetBytes(Position.X));
			list.AddRange(BitConverter.GetBytes(Position.Y));
			list.AddRange(BitConverter.GetBytes(Position.Z));
			list.AddRange(BitConverter.GetBytes(Rotate.X));
			list.AddRange(BitConverter.GetBytes(Rotate.Y));
			list.AddRange(BitConverter.GetBytes(Rotate.Z));
			list.AddRange(IPL.ToBytes());
			list.AddRange(BitConverter.GetBytes((int)Angle));
			list.Add(Pers);
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			int num = startIndex;
			FrameIndex = BitConverter.ToInt32(bytes, num);
			num += 4;
			Distance = BitConverter.ToSingle(bytes, num);
			num += 4;
			Position.X = BitConverter.ToSingle(bytes, num);
			num += 4;
			Position.Y = BitConverter.ToSingle(bytes, num);
			num += 4;
			Position.Z = BitConverter.ToSingle(bytes, num);
			num += 4;
			Rotate.X = BitConverter.ToSingle(bytes, num);
			num += 4;
			Rotate.Y = BitConverter.ToSingle(bytes, num);
			num += 4;
			Rotate.Z = BitConverter.ToSingle(bytes, num);
			num += 4;
			IPL.FromBytes(bytes, num);
			num += IPL.ByteCount;
			Angle = BitConverter.ToInt32(bytes, num);
			num += 4;
			Pers = bytes[num];
		}

		public object Clone()
		{
			return new VmdCamera(this);
		}
	}
}
