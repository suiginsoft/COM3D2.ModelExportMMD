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

		public int ByteCount
		{
			get
			{
				return 36;
			}
		}

		public VmdCamera_v1()
		{
		}

		public VmdCamera_v1(VmdCamera_v1 camera)
		{
			base.FrameIndex = camera.FrameIndex;
			this.Distance = camera.Distance;
			this.Position = camera.Position;
			this.Rotate = camera.Rotate;
			this.CameraIpl = camera.CameraIpl;
		}

		public VmdCamera ToVmdCamera()
		{
			VmdCamera vmdCamera = new VmdCamera();
			vmdCamera.FrameIndex = base.FrameIndex;
			vmdCamera.Distance = this.Distance;
			vmdCamera.Position = this.Position;
			vmdCamera.Rotate = this.Rotate;
			vmdCamera.IPL.MoveX = new VmdIplData(this.CameraIpl);
			vmdCamera.IPL.MoveY = new VmdIplData(this.CameraIpl);
			vmdCamera.IPL.MoveZ = new VmdIplData(this.CameraIpl);
			vmdCamera.IPL.Rotate = new VmdIplData(this.CameraIpl);
			vmdCamera.IPL.Distance = new VmdIplData(this.CameraIpl);
			vmdCamera.IPL.Angle = new VmdIplData(this.CameraIpl);
			vmdCamera.Angle = 45f;
			vmdCamera.Pers = 0;
			return vmdCamera;
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(base.FrameIndex));
			list.AddRange(BitConverter.GetBytes(this.Distance));
			list.AddRange(BitConverter.GetBytes(this.Position.x));
			list.AddRange(BitConverter.GetBytes(this.Position.y));
			list.AddRange(BitConverter.GetBytes(this.Position.z));
			list.AddRange(BitConverter.GetBytes(this.Rotate.x));
			list.AddRange(BitConverter.GetBytes(this.Rotate.y));
			list.AddRange(BitConverter.GetBytes(this.Rotate.z));
			list.Add((byte)this.CameraIpl.P1.X);
			list.Add((byte)this.CameraIpl.P2.X);
			list.Add((byte)this.CameraIpl.P1.Y);
			list.Add((byte)this.CameraIpl.P2.Y);
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			base.FrameIndex = BitConverter.ToInt32(bytes, startIndex);
			int num = startIndex + 4;
			this.Distance = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.Position.x = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.Position.y = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.Position.z = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.Rotate.x = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.Rotate.y = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.Rotate.z = BitConverter.ToSingle(bytes, num);
			num += 4;
			this.CameraIpl.P1.X = bytes[num];
			num++;
			this.CameraIpl.P1.Y = bytes[num];
			num++;
			this.CameraIpl.P2.X = bytes[num];
			num++;
			this.CameraIpl.P2.Y = bytes[num];
			num++;
		}

		public object Clone()
		{
			return new VmdCamera_v1(this);
		}
	}
}
