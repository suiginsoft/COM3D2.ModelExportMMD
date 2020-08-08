using System;
using System.IO;

namespace PmxLib
{
	public class PmxBody : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
	{
		public enum BoxKind
		{
			Sphere,
			Box,
			Capsule
		}

		public enum ModeType
		{
			Static,
			Dynamic,
			DynamicWithBone
		}

		public int Bone;

		public static string NullBoneName = "-";

		public int Group;

		public PmxBodyPassGroup PassGroup;

		public Vector3 BoxSize;

		public Vector3 Position;

		public Vector3 Rotation;

		public float Mass;

		public float PositionDamping;

		public float RotationDamping;

		public float Restitution;

		public float Friction;

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.Body;
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

		public PmxBone RefBone
		{
			get;
			set;
		}

		public BoxKind BoxType
		{
			get;
			set;
		}

		public ModeType Mode
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

		public PmxBody()
		{
			this.Name = "";
			this.NameE = "";
			this.PassGroup = new PmxBodyPassGroup();
			this.InitializeParameter();
			this.BoxSize = new Vector3(2f, 2f, 2f);
		}

		public PmxBody(PmxBody body, bool nonStr = false)
		{
			this.FromPmxBody(body, nonStr);
		}

		public void FromPmxBody(PmxBody body, bool nonStr = false)
		{
			if (!nonStr)
			{
				this.Name = body.Name;
				this.NameE = body.NameE;
			}
			this.Bone = body.Bone;
			this.Group = body.Group;
			this.PassGroup = body.PassGroup.Clone();
			this.BoxType = body.BoxType;
			this.BoxSize = body.BoxSize;
			this.Position = body.Position;
			this.Rotation = body.Rotation;
			this.Mass = body.Mass;
			this.PositionDamping = body.PositionDamping;
			this.RotationDamping = body.RotationDamping;
			this.Restitution = body.Restitution;
			this.Friction = body.Friction;
			this.Mode = body.Mode;
		}

		public void InitializeParameter()
		{
			this.Mass = 1f;
			this.PositionDamping = 0.5f;
			this.RotationDamping = 0.5f;
			this.Restitution = 0f;
			this.Friction = 0.5f;
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			this.Name = PmxStreamHelper.ReadString(s, f);
			this.NameE = PmxStreamHelper.ReadString(s, f);
			this.Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
			this.Group = PmxStreamHelper.ReadElement_Int32(s, 1, true);
			ushort bits = (ushort)PmxStreamHelper.ReadElement_Int32(s, 2, false);
			this.PassGroup.FromFlagBits(bits);
			this.BoxType = (BoxKind)s.ReadByte();
			this.BoxSize = V3_BytesConvert.FromStream(s);
			this.Position = V3_BytesConvert.FromStream(s);
			this.Rotation = V3_BytesConvert.FromStream(s);
			this.Mass = PmxStreamHelper.ReadElement_Float(s);
			Vector4 vector = V4_BytesConvert.FromStream(s);
			this.PositionDamping = vector.x;
			this.RotationDamping = vector.y;
			this.Restitution = vector.z;
			this.Friction = vector.w;
			this.Mode = (ModeType)s.ReadByte();
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteString(s, this.Name, f);
			PmxStreamHelper.WriteString(s, this.NameE, f);
			PmxStreamHelper.WriteElement_Int32(s, this.Bone, f.BoneSize, true);
			PmxStreamHelper.WriteElement_Int32(s, this.Group, 1, true);
			PmxStreamHelper.WriteElement_Int32(s, this.PassGroup.ToFlagBits(), 2, false);
			s.WriteByte((byte)this.BoxType);
			V3_BytesConvert.ToStream(s, this.BoxSize);
			V3_BytesConvert.ToStream(s, this.Position);
			V3_BytesConvert.ToStream(s, this.Rotation);
			PmxStreamHelper.WriteElement_Float(s, this.Mass);
			V4_BytesConvert.ToStream(s, new Vector4(this.PositionDamping, this.RotationDamping, this.Restitution, this.Friction));
			s.WriteByte((byte)this.Mode);
		}

		object ICloneable.Clone()
		{
			return new PmxBody(this, false);
		}

		public PmxBody Clone()
		{
			return new PmxBody(this, false);
		}
	}
}
