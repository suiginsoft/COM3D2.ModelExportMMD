using System;
using System.IO;

namespace PmxLib
{
	public class PmxBone : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
	{
		[Flags]
		public enum BoneFlags
		{
			None = 0,
			ToBone = 1,
			Rotation = 2,
			Translation = 4,
			Visible = 8,
			Enable = 0x10,
			IK = 0x20,
			AppendLocal = 0x80,
			AppendRotation = 0x100,
			AppendTranslation = 0x200,
			FixAxis = 0x400,
			LocalFrame = 0x800,
			AfterPhysics = 0x1000,
			ExtParent = 0x2000
		}

		public enum IKKindType
		{
			None,
			IK,
			Target,
			Link
		}

		public BoneFlags Flags;

		public int Parent;

		public int To_Bone;

		public Vector3 To_Offset;

		public Vector3 Position;

		public int Level;

		public int AppendParent;

		public float AppendRatio;

		public Vector3 Axis;

		public Vector3 LocalX;

		public Vector3 LocalY;

		public Vector3 LocalZ;

		public int ExtKey;

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.Bone;
			}
		}

		public string Name
		{
			get;
			set;
		}

		public string NameE
		{
			get;
			set;
		}

		public PmxBone RefParent
		{
			get;
			set;
		}

		public PmxBone RefTo_Bone
		{
			get;
			set;
		}

		public PmxBone RefAppendParent
		{
			get;
			set;
		}

		public PmxIK IK
		{
			get;
			private set;
		}

		public IKKindType IKKind
		{
			get;
			set;
		}

		public string NXName
		{
			get
			{
				return this.Name;
			}
			set
			{
				this.Name = value;
			}
		}

		public void ClearFlags()
		{
			this.Flags = (BoneFlags.Rotation | BoneFlags.Visible | BoneFlags.Enable);
		}

		public bool GetFlag(BoneFlags f)
		{
			return (f & this.Flags) == f;
		}

		public void SetFlag(BoneFlags f, bool val)
		{
			if (val)
			{
				this.Flags |= f;
			}
			else
			{
				this.Flags &= ~f;
			}
		}

		public void ClearLocal()
		{
			this.LocalX = new Vector3(1f, 0f, 0f);
			this.LocalY = new Vector3(0f, 1f, 0f);
			this.LocalZ = new Vector3(0f, 0f, 1f);
		}

		public void NormalizeLocal()
		{
			this.LocalZ.Normalize();
			this.LocalX.Normalize();
			this.LocalY = Vector3.Cross(this.LocalZ, this.LocalX);
			this.LocalZ = Vector3.Cross(this.LocalX, this.LocalY);
			this.LocalY.Normalize();
			this.LocalZ.Normalize();
		}

		public PmxBone()
		{
			this.Name = "";
			this.NameE = "";
			this.ClearFlags();
			this.Parent = -1;
			this.To_Bone = -1;
			this.To_Offset = Vector3.zero;
			this.AppendParent = -1;
			this.AppendRatio = 1f;
			this.Level = 0;
			this.ClearLocal();
			this.IK = new PmxIK();
			this.IKKind = IKKindType.None;
		}

		public PmxBone(PmxBone bone, bool nonStr = false)
		{
			this.FromPmxBone(bone, nonStr);
		}

		public void FromPmxBone(PmxBone bone, bool nonStr = false)
		{
			if (!nonStr)
			{
				this.Name = bone.Name;
				this.NameE = bone.NameE;
			}
			this.Flags = bone.Flags;
			this.Parent = bone.Parent;
			this.To_Bone = bone.To_Bone;
			this.To_Offset = bone.To_Offset;
			this.Position = bone.Position;
			this.Level = bone.Level;
			this.AppendParent = bone.AppendParent;
			this.AppendRatio = bone.AppendRatio;
			this.Axis = bone.Axis;
			this.LocalX = bone.LocalX;
			this.LocalY = bone.LocalY;
			this.LocalZ = bone.LocalZ;
			this.ExtKey = bone.ExtKey;
			this.IK = bone.IK.Clone();
			this.IKKind = bone.IKKind;
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			this.Name = PmxStreamHelper.ReadString(s, f);
			this.NameE = PmxStreamHelper.ReadString(s, f);
			this.Position = V3_BytesConvert.FromStream(s);
			this.Parent = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
			this.Level = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.Flags = (BoneFlags)PmxStreamHelper.ReadElement_Int32(s, 2, false);
			if (this.GetFlag(BoneFlags.ToBone))
			{
				this.To_Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
			}
			else
			{
				this.To_Offset = V3_BytesConvert.FromStream(s);
			}
			if (this.GetFlag(BoneFlags.AppendRotation) || this.GetFlag(BoneFlags.AppendTranslation))
			{
				this.AppendParent = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
				this.AppendRatio = PmxStreamHelper.ReadElement_Float(s);
			}
			if (this.GetFlag(BoneFlags.FixAxis))
			{
				this.Axis = V3_BytesConvert.FromStream(s);
			}
			if (this.GetFlag(BoneFlags.LocalFrame))
			{
				this.LocalX = V3_BytesConvert.FromStream(s);
				this.LocalZ = V3_BytesConvert.FromStream(s);
				this.NormalizeLocal();
			}
			if (this.GetFlag(BoneFlags.ExtParent))
			{
				this.ExtKey = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			}
			if (this.GetFlag(BoneFlags.IK))
			{
				this.IK.FromStreamEx(s, f);
			}
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteString(s, this.Name, f);
			PmxStreamHelper.WriteString(s, this.NameE, f);
			V3_BytesConvert.ToStream(s, this.Position);
			PmxStreamHelper.WriteElement_Int32(s, this.Parent, f.BoneSize, true);
			PmxStreamHelper.WriteElement_Int32(s, this.Level, 4, true);
			PmxStreamHelper.WriteElement_Int32(s, (int)this.Flags, 2, false);
			if (this.GetFlag(BoneFlags.ToBone))
			{
				PmxStreamHelper.WriteElement_Int32(s, this.To_Bone, f.BoneSize, true);
			}
			else
			{
				V3_BytesConvert.ToStream(s, this.To_Offset);
			}
			if (this.GetFlag(BoneFlags.AppendRotation) || this.GetFlag(BoneFlags.AppendTranslation))
			{
				PmxStreamHelper.WriteElement_Int32(s, this.AppendParent, f.BoneSize, true);
				PmxStreamHelper.WriteElement_Float(s, this.AppendRatio);
			}
			if (this.GetFlag(BoneFlags.FixAxis))
			{
				V3_BytesConvert.ToStream(s, this.Axis);
			}
			if (this.GetFlag(BoneFlags.LocalFrame))
			{
				this.NormalizeLocal();
				V3_BytesConvert.ToStream(s, this.LocalX);
				V3_BytesConvert.ToStream(s, this.LocalZ);
			}
			if (this.GetFlag(BoneFlags.ExtParent))
			{
				PmxStreamHelper.WriteElement_Int32(s, this.ExtKey, 4, true);
			}
			if (this.GetFlag(BoneFlags.IK))
			{
				this.IK.ToStreamEx(s, f);
			}
		}

		object ICloneable.Clone()
		{
			return new PmxBone(this, false);
		}

		public PmxBone Clone()
		{
			return new PmxBone(this, false);
		}
	}
}
