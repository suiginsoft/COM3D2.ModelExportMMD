using System;
using System.Collections.Generic;

namespace PmxLib
{
	internal class VmdCamera_v1 : VmdFrameBase, IBytesConvert, ICloneable
	{
		public float Distance;

		public Vector3 Position;

		public Vector3 Rotate;

		public VmdIplData CameraIpl = new VmdIplData();

		public int ByteCount => 36;

		public VmdCamera_v1()
		{
		}

		public VmdCamera_v1(VmdCamera_v1 camera)
		{
			FrameIndex = camera.FrameIndex;
			Distance = camera.Distance;
			Position = camera.Position;
			Rotate = camera.Rotate;
			CameraIpl = camera.CameraIpl;
		}

		public VmdCamera ToVmdCamera()
		{
			return new VmdCamera
			{
				FrameIndex = FrameIndex,
				Distance = Distance,
				Position = Position,
				Rotate = Rotate,
				IPL = 
				{
					MoveX = new VmdIplData(CameraIpl),
					MoveY = new VmdIplData(CameraIpl),
					MoveZ = new VmdIplData(CameraIpl),
					Rotate = new VmdIplData(CameraIpl),
					Distance = new VmdIplData(CameraIpl),
					Angle = new VmdIplData(CameraIpl)
				},
				Angle = 45f,
				Pers = 0
			};
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
			list.Add((byte)CameraIpl.P1.X);
			list.Add((byte)CameraIpl.P2.X);
			list.Add((byte)CameraIpl.P1.Y);
			list.Add((byte)CameraIpl.P2.Y);
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
			CameraIpl.P1.X = bytes[num];
			num++;
			CameraIpl.P1.Y = bytes[num];
			num++;
			CameraIpl.P2.X = bytes[num];
			num++;
			CameraIpl.P2.Y = bytes[num];
			num++;
		}

		public object Clone()
		{
			return new VmdCamera_v1(this);
		}
	}
}
