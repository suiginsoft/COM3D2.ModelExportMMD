using System;
using System.IO;

namespace PmxLib
{
	internal class PmxBone : PmxIDObject, IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
	{
		[Flags]
		public enum BoneFlags
		{
			None = 0x0,
			ToBone = 0x1,
			Rotation = 0x2,
			Translation = 0x4,
			Visible = 0x8,
			Enable = 0x10,
			IK = 0x20,
			AddLocal = 0x80,
			AddRotation = 0x100,
			AddTranslation = 0x200,
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

		public int AddParent;

		public float AddRatio;

		public Vector3 Axis;

		public Vector3 LocalX;

		public Vector3 LocalY;

		public Vector3 LocalZ;

		public int ExtKey;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.Bone;

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

		public bool IsFixAxis => GetFlag(BoneFlags.FixAxis);

		public bool IsLocal => GetFlag(BoneFlags.LocalFrame);

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

		public PmxBone RefAddParent
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
				return Name;
			}
			set
			{
				Name = value;
			}
		}

		public void ClearFlags()
		{
			Flags = (BoneFlags.Rotation | BoneFlags.Visible | BoneFlags.Enable);
		}

		public bool GetFlag(BoneFlags f)
		{
			return (f & Flags) == f;
		}

		public void SetFlag(BoneFlags f, bool val)
		{
			if (val)
			{
				Flags |= f;
			}
			else
			{
				Flags &= ~f;
			}
		}

		public void ClearLocal()
		{
			LocalX = Vector3.UnitX;
			LocalY = Vector3.UnitY;
			LocalZ = Vector3.UnitZ;
		}

		public void NormalizeLocal()
		{
			LocalZ.Normalize();
			LocalX.Normalize();
			LocalY = Vector3.Cross(LocalZ, LocalX);
			LocalZ = Vector3.Cross(LocalX, LocalY);
			LocalY.Normalize();
			LocalZ.Normalize();
		}

		public PmxBone()
		{
			Name = "";
			NameE = "";
			ClearFlags();
			Parent = -1;
			To_Bone = -1;
			To_Offset = Vector3.Zero;
			AddParent = -1;
			AddRatio = 1f;
			Level = 0;
			ClearLocal();
			IK = new PmxIK();
			IKKind = IKKindType.None;
		}

		public PmxBone(PmxBone bone, bool nonStr = false)
			: this()
		{
			FromPmxBone(bone, nonStr);
		}

		public void FromPmxBone(PmxBone bone, bool nonStr = false)
		{
			if (!nonStr)
			{
				Name = bone.Name;
				NameE = bone.NameE;
			}
			Flags = bone.Flags;
			Parent = bone.Parent;
			To_Bone = bone.To_Bone;
			To_Offset = bone.To_Offset;
			Position = bone.Position;
			Level = bone.Level;
			AddParent = bone.AddParent;
			AddRatio = bone.AddRatio;
			Axis = bone.Axis;
			LocalX = bone.LocalX;
			LocalY = bone.LocalY;
			LocalZ = bone.LocalZ;
			ExtKey = bone.ExtKey;
			IK = bone.IK.Clone();
			IKKind = bone.IKKind;
			FromID(bone);
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			Name = PmxStreamHelper.ReadString(s, f);
			NameE = PmxStreamHelper.ReadString(s, f);
			Position = V3_BytesConvert.FromStream(s);
			Parent = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
			Level = PmxStreamHelper.ReadElement_Int32(s);
			Flags = (BoneFlags)PmxStreamHelper.ReadElement_Int32(s, 2, signed: false);
			if (GetFlag(BoneFlags.ToBone))
			{
				To_Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
			}
			else
			{
				To_Offset = V3_BytesConvert.FromStream(s);
			}
			if (GetFlag(BoneFlags.AddRotation) || GetFlag(BoneFlags.AddTranslation))
			{
				AddParent = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
				AddRatio = PmxStreamHelper.ReadElement_Float(s);
			}
			if (GetFlag(BoneFlags.FixAxis))
			{
				Axis = V3_BytesConvert.FromStream(s);
			}
			if (GetFlag(BoneFlags.LocalFrame))
			{
				LocalX = V3_BytesConvert.FromStream(s);
				LocalZ = V3_BytesConvert.FromStream(s);
				if (!f.WithID)
				{
					NormalizeLocal();
				}
			}
			if (GetFlag(BoneFlags.ExtParent))
			{
				ExtKey = PmxStreamHelper.ReadElement_Int32(s);
			}
			if (GetFlag(BoneFlags.IK))
			{
				IK.FromStreamEx(s, f);
			}
			if (f.WithID)
			{
				base.UID = PmxStreamHelper.ReadElement_UInt(s);
				base.CID = PmxStreamHelper.ReadElement_UInt(s);
			}
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteString(s, Name, f);
			PmxStreamHelper.WriteString(s, NameE, f);
			V3_BytesConvert.ToStream(s, Position);
			PmxStreamHelper.WriteElement_Int32(s, Parent, f.BoneSize);
			PmxStreamHelper.WriteElement_Int32(s, Level);
			PmxStreamHelper.WriteElement_Int32(s, (int)Flags, 2, signed: false);
			if (GetFlag(BoneFlags.ToBone))
			{
				PmxStreamHelper.WriteElement_Int32(s, To_Bone, f.BoneSize);
			}
			else
			{
				V3_BytesConvert.ToStream(s, To_Offset);
			}
			if (GetFlag(BoneFlags.AddRotation) || GetFlag(BoneFlags.AddTranslation))
			{
				PmxStreamHelper.WriteElement_Int32(s, AddParent, f.BoneSize);
				PmxStreamHelper.WriteElement_Float(s, AddRatio);
			}
			if (GetFlag(BoneFlags.FixAxis))
			{
				V3_BytesConvert.ToStream(s, Axis);
			}
			if (GetFlag(BoneFlags.LocalFrame))
			{
				if (!f.WithID)
				{
					NormalizeLocal();
				}
				V3_BytesConvert.ToStream(s, LocalX);
				V3_BytesConvert.ToStream(s, LocalZ);
			}
			if (GetFlag(BoneFlags.ExtParent))
			{
				PmxStreamHelper.WriteElement_Int32(s, ExtKey);
			}
			if (GetFlag(BoneFlags.IK))
			{
				IK.ToStreamEx(s, f);
			}
			if (f.WithID)
			{
				PmxStreamHelper.WriteElement_UInt(s, base.UID);
				PmxStreamHelper.WriteElement_UInt(s, base.CID);
			}
		}

		object ICloneable.Clone()
		{
			return new PmxBone(this);
		}

		public PmxBone Clone()
		{
			return new PmxBone(this);
		}
	}
}
