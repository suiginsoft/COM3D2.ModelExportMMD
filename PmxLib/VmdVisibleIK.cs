using System;
using System.Collections.Generic;

namespace PmxLib
{
	public class VmdVisibleIK : VmdFrameBase, IBytesConvert, ICloneable
	{
		public class IK : ICloneable
		{
			public bool Enable;

			public string IKName
			{
				get;
				set;
			}

			public IK()
			{
				this.IKName = "";
				this.Enable = true;
			}

			public IK(IK ik)
			{
				this.IKName = ik.IKName;
				this.Enable = ik.Enable;
			}

			object ICloneable.Clone()
			{
				return new IK(this);
			}
		}

		private const int NameBytes = 20;

		public bool Visible;

		public List<IK> IKList
		{
			get;
			set;
		}

		public int ByteCount
		{
			get
			{
				return 9 + 21 * this.IKList.Count;
			}
		}

		public VmdVisibleIK()
		{
			this.Visible = true;
			this.IKList = new List<IK>();
		}

		public VmdVisibleIK(VmdVisibleIK vik)
		{
			this.Visible = vik.Visible;
			this.IKList = CP.CloneList(vik.IKList);
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(base.FrameIndex));
			list.Add((byte)(this.Visible ? 1 : 0));
			list.AddRange(BitConverter.GetBytes(this.IKList.Count));
			for (int i = 0; i < this.IKList.Count; i++)
			{
				byte[] array = new byte[20];
				BytesStringProc.SetString(array, this.IKList[i].IKName, 0, 253);
				list.AddRange(array);
				list.Add((byte)(this.IKList[i].Enable ? 1 : 0));
			}
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			base.FrameIndex = BitConverter.ToInt32(bytes, startIndex);
			int num = startIndex + 4;
			this.Visible = (bytes[num++] != 0);
			int num3 = BitConverter.ToInt32(bytes, num);
			num += 4;
			byte[] array = new byte[20];
			for (int i = 0; i < num3; i++)
			{
				IK iK = new IK();
				Array.Copy(bytes, num, array, 0, 20);
				iK.IKName = BytesStringProc.GetString(array, 0);
				num += 20;
				iK.Enable = (bytes[num++] != 0);
				this.IKList.Add(iK);
			}
		}

		public object Clone()
		{
			return new VmdVisibleIK(this);
		}
	}
}
