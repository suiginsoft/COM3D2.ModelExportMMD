using System;
using System.Collections.Generic;

namespace PmxLib
{
	public class VmdCamera : VmdFrameBase, IBytesConvert, ICloneable
	{
		public float Distance;

		public Vector3 Position;

		public Vector3 Rotate;

		public VmdCameraIPL IPL = new VmdCameraIPL();

		public float Angle;

		public byte Pers;

		public int ByteCount
		{
			get
			{
				return 32 + this.IPL.ByteCount + 1 + 4;
			}
		}

		public VmdCamera()
		{
		}

		public VmdCamera(VmdCamera camera)
		{
			base.FrameIndex = camera.FrameIndex;
			this.Distance = camera.Distance;
			this.Position = camera.Position;
			this.Rotate = camera.Rotate;
			this.IPL = (VmdCameraIPL)camera.IPL.Clone();
			this.Angle = camera.Angle;
			this.Pers = camera.Pers;
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
			list.AddRange(this.IPL.ToBytes());
			list.AddRange(BitConverter.GetBytes((int)this.Angle));
			list.Add(this.Pers);
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
			this.IPL.FromBytes(bytes, num);
			num += this.IPL.ByteCount;
			this.Angle = (float)BitConverter.ToInt32(bytes, num);
			num += 4;
			this.Pers = bytes[num];
		}

		public object Clone()
		{
			return new VmdCamera(this);
		}
	}
}
