using System;
using System.Collections.Generic;

namespace PmxLib
{
	public class VmdMorph : VmdFrameBase, IBytesConvert, ICloneable
	{
		private const int NameBytes = 15;

		public string Name = "";

		public float Value;

		public int ByteCount
		{
			get
			{
				return 23;
			}
		}

		public VmdMorph()
		{
		}

		public VmdMorph(VmdMorph skin)
		{
			this.Name = skin.Name;
			base.FrameIndex = skin.FrameIndex;
			this.Value = skin.Value;
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			byte[] array = new byte[15];
			BytesStringProc.SetString(array, this.Name, 0, 253);
			list.AddRange(array);
			list.AddRange(BitConverter.GetBytes(base.FrameIndex));
			list.AddRange(BitConverter.GetBytes(this.Value));
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
			this.Value = BitConverter.ToSingle(bytes, num);
		}

		public object Clone()
		{
			return new VmdMorph(this);
		}
	}
}
