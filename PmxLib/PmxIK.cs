using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	public class PmxIK : IPmxObjectKey, IPmxStreamIO, ICloneable
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

			PmxObject IPmxObjectKey.ObjectKey
			{
				get
				{
					return PmxObject.IKLink;
				}
			}

			public PmxBone RefBone
			{
				get;
				set;
			}

			public IKLink()
			{
				this.Bone = -1;
				this.IsLimit = false;
			}

			public IKLink(IKLink link)
			{
				this.FromIKLink(link);
			}

			public void FromIKLink(IKLink link)
			{
				this.Bone = link.Bone;
				this.IsLimit = link.IsLimit;
				this.Low = link.Low;
				this.High = link.High;
				this.Euler = link.Euler;
			}

			public void NormalizeAngle()
			{
				Vector3 low = default(Vector3);
				this.Low.x = Math.Min(this.Low.x, this.High.x);
				Vector3 high = default(Vector3);
				this.High.x = Math.Max(this.Low.x, this.High.x);
				this.Low.y = Math.Min(this.Low.y, this.High.y);
				this.High.y = Math.Max(this.Low.y, this.High.y);
				this.Low.z = Math.Min(this.Low.z, this.High.z);
				this.High.z = Math.Max(this.Low.z, this.High.z);
				this.Low = low;
				this.High = high;
			}

			public void NormalizeEulerAxis()
			{
				if (-1.57079637f < this.Low.x && this.High.x < 1.57079637f)
				{
					this.Euler = EulerType.ZXY;
				}
				else if (-1.57079637f < this.Low.y && this.High.y < 1.57079637f)
				{
					this.Euler = EulerType.XYZ;
				}
				else
				{
					this.Euler = EulerType.YZX;
				}
				this.FixAxis = FixAxisType.None;
				if (this.Low.x == 0f && this.High.x == 0f && this.Low.y == 0f && this.High.y == 0f && this.Low.z == 0f && this.High.z == 0f)
				{
					this.FixAxis = FixAxisType.Fix;
				}
				else if (this.Low.y == 0f && this.High.y == 0f && this.Low.z == 0f && this.High.z == 0f)
				{
					this.FixAxis = FixAxisType.X;
				}
				else if (this.Low.x == 0f && this.High.x == 0f && this.Low.z == 0f && this.High.z == 0f)
				{
					this.FixAxis = FixAxisType.Y;
				}
				else if (this.Low.x == 0f && this.High.x == 0f && this.Low.y == 0f && this.High.y == 0f)
				{
					this.FixAxis = FixAxisType.Z;
				}
			}

			public void FromStreamEx(Stream s, PmxElementFormat f = null)
			{
				this.Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
				this.IsLimit = (s.ReadByte() != 0);
				if (this.IsLimit)
				{
					this.Low = V3_BytesConvert.FromStream(s);
					this.High = V3_BytesConvert.FromStream(s);
				}
			}

			public void ToStreamEx(Stream s, PmxElementFormat f = null)
			{
				PmxStreamHelper.WriteElement_Int32(s, this.Bone, f.BoneSize, true);
				s.WriteByte((byte)(this.IsLimit ? 1 : 0));
				if (this.IsLimit)
				{
					V3_BytesConvert.ToStream(s, this.Low);
					V3_BytesConvert.ToStream(s, this.High);
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

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.IK;
			}
		}

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
			this.Target = -1;
			this.LoopCount = 0;
			this.Angle = 1f;
			this.LinkList = new List<IKLink>();
		}

		public PmxIK(PmxIK ik)
		{
			this.FromPmxIK(ik);
		}

		public void FromPmxIK(PmxIK ik)
		{
			this.Target = ik.Target;
			this.LoopCount = ik.LoopCount;
			this.Angle = ik.Angle;
			this.LinkList = new List<IKLink>();
			for (int i = 0; i < ik.LinkList.Count; i++)
			{
				this.LinkList.Add(ik.LinkList[i].Clone());
			}
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			this.Target = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
			this.LoopCount = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.Angle = PmxStreamHelper.ReadElement_Float(s);
			int num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.LinkList.Clear();
			this.LinkList.Capacity = num;
			for (int i = 0; i < num; i++)
			{
				IKLink iKLink = new IKLink();
				iKLink.FromStreamEx(s, f);
				this.LinkList.Add(iKLink);
			}
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteElement_Int32(s, this.Target, f.BoneSize, true);
			PmxStreamHelper.WriteElement_Int32(s, this.LoopCount, 4, true);
			PmxStreamHelper.WriteElement_Float(s, this.Angle);
			PmxStreamHelper.WriteElement_Int32(s, this.LinkList.Count, 4, true);
			for (int i = 0; i < this.LinkList.Count; i++)
			{
				this.LinkList[i].ToStreamEx(s, f);
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
