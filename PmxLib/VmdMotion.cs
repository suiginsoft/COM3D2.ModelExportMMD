using System;
using System.Collections.Generic;

namespace PmxLib
{
	internal class VmdMotion : VmdFrameBase, IBytesConvert, ICloneable
	{
		public string Name = "";

		private const int NameBytes = 15;

		public Vector3 Position;

		public Quaternion Rotate = Quaternion.Identity;

		public VmdMotionIPL IPL = new VmdMotionIPL();

		protected int NoDataCount = 48;

		private const ushort PhysicsOffNum = 3939;

		public bool PhysicsOff
		{
			get;
			set;
		}

		public int ByteCount => 47 + IPL.ByteCount + NoDataCount;

		public VmdMotion()
		{
			PhysicsOff = false;
		}

		public VmdMotion(VmdMotion motion)
			: this()
		{
			Name = motion.Name;
			FrameIndex = motion.FrameIndex;
			Position = motion.Position;
			Rotate = motion.Rotate;
			IPL = (VmdMotionIPL)motion.IPL.Clone();
			PhysicsOff = motion.PhysicsOff;
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			byte[] array = new byte[15];
			BytesStringProc.SetString(array, Name, 0, 253);
			list.AddRange(array);
			list.AddRange(BitConverter.GetBytes(FrameIndex));
			list.AddRange(BitConverter.GetBytes(Position.X));
			list.AddRange(BitConverter.GetBytes(Position.Y));
			list.AddRange(BitConverter.GetBytes(Position.Z));
			list.AddRange(BitConverter.GetBytes(Rotate.X));
			list.AddRange(BitConverter.GetBytes(Rotate.Y));
			list.AddRange(BitConverter.GetBytes(Rotate.Z));
			list.AddRange(BitConverter.GetBytes(Rotate.W));
			byte[] array2 = new byte[IPL.ByteCount * 4];
			byte[] array3 = IPL.ToBytes();
			int num = array3.Length;
			for (int i = 0; i < num; i++)
			{
				array2[i * 4] = array3[i];
			}
			if (PhysicsOff)
			{
				byte[] bytes = BitConverter.GetBytes((ushort)3939);
				array2[2] = bytes[0];
				array2[3] = bytes[1];
			}
			list.AddRange(array2);
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			int num = startIndex;
			byte[] array = new byte[15];
			Array.Copy(bytes, num, array, 0, 15);
			Name = BytesStringProc.GetString(array, 0);
			num += 15;
			FrameIndex = BitConverter.ToInt32(bytes, num);
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
			Rotate.W = BitConverter.ToSingle(bytes, num);
			num += 4;
			int byteCount = IPL.ByteCount;
			byte[] array2 = new byte[byteCount * 4];
			byte[] array3 = new byte[IPL.ByteCount];
			Array.Copy(bytes, num, array2, 0, array2.Length);
			ushort num2 = BitConverter.ToUInt16(array2, 2);
			PhysicsOff = (num2 == 3939);
			for (int i = 0; i < byteCount; i++)
			{
				array3[i] = array2[i * 4];
			}
			IPL.FromBytes(array3, 0);
		}

		public object Clone()
		{
			return new VmdMotion(this);
		}
	}
}
