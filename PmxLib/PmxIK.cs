using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	internal class PmxIK : IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public class IKLink : IPmxObjectKey, IPmxStreamIO, ICloneable
		{
			public enum EulerType
			{
				ZXY,
				XYZ,
				YZX
			}

			public enum FixAxisType
			{
				None,
				Fix,
				X,
				Y,
				Z
			}

			public int Bone;

			public bool IsLimit;

			public Vector3 Low;

			public Vector3 High;

			public EulerType Euler;

			public FixAxisType FixAxis;

			PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.IKLink;

			public PmxBone RefBone
			{
				get;
				set;
			}

			public IKLink()
			{
				Bone = -1;
				IsLimit = false;
			}

			public IKLink(IKLink link)
			{
				FromIKLink(link);
			}

			public void FromIKLink(IKLink link)
			{
				Bone = link.Bone;
				IsLimit = link.IsLimit;
				Low = link.Low;
				High = link.High;
				Euler = link.Euler;
			}

			public void NormalizeAngle()
			{
				Vector3 low = default(Vector3);
				low.X = Math.Min(Low.X, High.X);
				Vector3 high = default(Vector3);
				high.X = Math.Max(Low.X, High.X);
				low.Y = Math.Min(Low.Y, High.Y);
				high.Y = Math.Max(Low.Y, High.Y);
				low.Z = Math.Min(Low.Z, High.Z);
				high.Z = Math.Max(Low.Z, High.Z);
				Low = low;
				High = high;
			}

			public void NormalizeEulerAxis()
			{
				if (-(float)Math.PI / 2f < Low.X && High.X < (float)Math.PI / 2f)
				{
					Euler = EulerType.ZXY;
				}
				else if (-(float)Math.PI / 2f < Low.Y && High.Y < (float)Math.PI / 2f)
				{
					Euler = EulerType.XYZ;
				}
				else
				{
					Euler = EulerType.YZX;
				}
				FixAxis = FixAxisType.None;
				if (Low.X == 0f && High.X == 0f && Low.Y == 0f && High.Y == 0f && Low.Z == 0f && High.Z == 0f)
				{
					FixAxis = FixAxisType.Fix;
				}
				else if (Low.Y == 0f && High.Y == 0f && Low.Z == 0f && High.Z == 0f)
				{
					FixAxis = FixAxisType.X;
				}
				else if (Low.X == 0f && High.X == 0f && Low.Z == 0f && High.Z == 0f)
				{
					FixAxis = FixAxisType.Y;
				}
				else if (Low.X == 0f && High.X == 0f && Low.Y == 0f && High.Y == 0f)
				{
					FixAxis = FixAxisType.Z;
				}
			}

			public void FromStreamEx(Stream s, PmxElementFormat f = null)
			{
				Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
				IsLimit = (s.ReadByte() != 0);
				if (IsLimit)
				{
					Low = V3_BytesConvert.FromStream(s);
					High = V3_BytesConvert.FromStream(s);
				}
			}

			public void ToStreamEx(Stream s, PmxElementFormat f = null)
			{
				PmxStreamHelper.WriteElement_Int32(s, Bone, f.BoneSize);
				s.WriteByte((byte)(IsLimit ? 1u : 0u));
				if (IsLimit)
				{
					V3_BytesConvert.ToStream(s, Low);
					V3_BytesConvert.ToStream(s, High);
				}
			}

			object ICloneable.Clone()
			{
				return new IKLink(this);
			}

			public IKLink Clone()
			{
				return new IKLink(this);
			}
		}

		public int Target;

		public int LoopCount;

		public float Angle;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.IK;

		public PmxBone RefTarget
		{
			get;
			set;
		}

		public List<IKLink> LinkList
		{
			get;
			private set;
		}

		public PmxIK()
		{
			Target = -1;
			LoopCount = 0;
			Angle = 1f;
			LinkList = new List<IKLink>();
		}

		public PmxIK(PmxIK ik)
		{
			FromPmxIK(ik);
		}

		public void FromPmxIK(PmxIK ik)
		{
			Target = ik.Target;
			LoopCount = ik.LoopCount;
			Angle = ik.Angle;
			LinkList = new List<IKLink>();
			for (int i = 0; i < ik.LinkList.Count; i++)
			{
				LinkList.Add(ik.LinkList[i].Clone());
			}
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			Target = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
			LoopCount = PmxStreamHelper.ReadElement_Int32(s);
			Angle = PmxStreamHelper.ReadElement_Float(s);
			int num = PmxStreamHelper.ReadElement_Int32(s);
			LinkList.Clear();
			LinkList.Capacity = num;
			for (int i = 0; i < num; i++)
			{
				IKLink iKLink = new IKLink();
				iKLink.FromStreamEx(s, f);
				LinkList.Add(iKLink);
			}
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteElement_Int32(s, Target, f.BoneSize);
			PmxStreamHelper.WriteElement_Int32(s, LoopCount);
			PmxStreamHelper.WriteElement_Float(s, Angle);
			PmxStreamHelper.WriteElement_Int32(s, LinkList.Count);
			for (int i = 0; i < LinkList.Count; i++)
			{
				LinkList[i].ToStreamEx(s, f);
			}
		}

		object ICloneable.Clone()
		{
			return new PmxIK(this);
		}

		public PmxIK Clone()
		{
			return new PmxIK(this);
		}
	}
}
