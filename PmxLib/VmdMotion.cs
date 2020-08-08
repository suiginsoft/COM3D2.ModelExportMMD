using System;
using System.Collections.Generic;

namespace PmxLib
{
	public class VmdMotion : VmdFrameBase, IBytesConvert, ICloneable
	{
		private const int NameBytes = 15;

		private const ushort PhysicsOffNum = 3939;

		public string Name = "";

		public Vector3 Position;

		public Quaternion Rotate = Quaternion.identity;

		public VmdMotionIPL IPL = new VmdMotionIPL();

		protected int NoDataCount = 48;

		public bool PhysicsOff
		{
			get;
			set;
		}

		public int ByteCount
		{
			get
			{
				return 47 + this.IPL.ByteCount + this.NoDataCount;
			}
		}

		public VmdMotion()
		{
			this.PhysicsOff = false;
		}

		public VmdMotion(VmdMotion motion)
			: this()
		{
			this.Name = motion.Name;
			base.FrameIndex = motion.FrameIndex;
			this.Position = motion.Position;
			this.Rotate = motion.Rotate;
			this.IPL = (VmdMotionIPL)motion.IPL.Clone();
			this.PhysicsOff = motion.PhysicsOff;
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			byte[] array = new byte[15];
			BytesStringProc.SetString(array, this.Name, 0, 253);
			list.AddRange(array);
			list.AddRange(BitConverter.GetBytes(base.FrameIndex));
			list.AddRange(BitConverter.GetBytes(this.Position.x));
			list.AddRange(BitConverter.GetBytes(this.Position.y));
			list.AddRange(BitConverter.GetBytes(this.Position.z));
			list.AddRange(BitConverter.GetBytes(this.Rotate.x));
			list.AddRange(BitConverter.GetBytes(this.Rotate.y));
			list.AddRange(BitConverter.GetBytes(this.Rotate.z));
			list.AddRange(BitConverter.GetBytes(this.Rotate.w));
			byte[] array2 = new byte[this.IPL.ByteCount * 4];
			byte[] array3 = this.IPL.ToBytes();
			int num = array3.Length;
			for (int i = 0; i < num; i++)
			{
				array2[i * 4] = array3[i];
			}
			if (this.PhysicsOff)
			{
				byte[] bytes = BitConverter.GetBytes(3939);
				array2[2] = bytes[0];
				array2[3] = bytes[1];
			}
			list.AddRange(array2);
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			byte[] array = new byte[15];
			Array.Copy(bytes, startIndex, array, 0, 15);
			this.Name = BytesStringProc.GetString(array, 0);
			int num = startIndex + 15;
			base.FrameIndex = BitConverter.ToInt32(bytes, num);
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
			this.Rotate.w = BitConverter.ToSingle(bytes, num);
			num += 4;
			int byteCount = this.IPL.ByteCount;
			byte[] array2 = new byte[byteCount * 4];
			byte[] array3 = new byte[this.IPL.ByteCount];
			Array.Copy(bytes, num, array2, 0, array2.Length);
			ushort num2 = BitConverter.ToUInt16(array2, 2);
			this.PhysicsOff = (num2 == 3939);
			for (int i = 0; i < byteCount; i++)
			{
				array3[i] = array2[i * 4];
			}
			this.IPL.FromBytes(array3, 0);
		}

		public object Clone()
		{
			return new VmdMotion(this);
		}
	}
}
