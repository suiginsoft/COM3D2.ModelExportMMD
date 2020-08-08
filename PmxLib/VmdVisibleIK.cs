using System;
using System.Collections.Generic;

namespace PmxLib
{
	internal class VmdVisibleIK : VmdFrameBase, IBytesConvert, ICloneable
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
				IKName = "";
				Enable = true;
			}

			public IK(IK ik)
			{
				IKName = ik.IKName;
				Enable = ik.Enable;
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

		public int ByteCount => 9 + 21 * IKList.Count;

		public VmdVisibleIK()
		{
			Visible = true;
			IKList = new List<IK>();
		}

		public VmdVisibleIK(VmdVisibleIK vik)
		{
			Visible = vik.Visible;
			IKList = CP.CloneList(vik.IKList);
		}

		public byte[] ToBytes()
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(FrameIndex));
			list.Add((byte)(Visible ? 1u : 0u));
			list.AddRange(BitConverter.GetBytes(IKList.Count));
			for (int i = 0; i < IKList.Count; i++)
			{
				byte[] array = new byte[20];
				BytesStringProc.SetString(array, IKList[i].IKName, 0, 253);
				list.AddRange(array);
				list.Add((byte)(IKList[i].Enable ? 1u : 0u));
			}
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			int num = startIndex;
			FrameIndex = BitConverter.ToInt32(bytes, num);
			num += 4;
			Visible = (bytes[num++] != 0);
			int num2 = BitConverter.ToInt32(bytes, num);
			num += 4;
			byte[] array = new byte[20];
			for (int i = 0; i < num2; i++)
			{
				IK iK = new IK();
				Array.Copy(bytes, num, array, 0, 20);
				iK.IKName = BytesStringProc.GetString(array, 0);
				num += 20;
				iK.Enable = (bytes[num++] != 0);
				IKList.Add(iK);
			}
		}

		public object Clone()
		{
			return new VmdVisibleIK(this);
		}
	}
}
