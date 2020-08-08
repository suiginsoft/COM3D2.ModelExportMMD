using System;
using System.IO;

namespace PmxLib
{
	internal class PmxJoint : PmxIDObject, IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
	{
		public enum JointKind
		{
			Sp6DOF,
			G6DOF,
			P2P,
			ConeTwist,
			Slider,
			Hinge
		}

		public JointKind Kind;

		public int BodyA;

		public int BodyB;

		public Vector3 Position;

		public Vector3 Rotation;

		public Vector3 Limit_MoveLow;

		public Vector3 Limit_MoveHigh;

		public Vector3 Limit_AngleLow;

		public Vector3 Limit_AngleHigh;

		public Vector3 SpConst_Move;

		public Vector3 SpConst_Rotate;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.Joint;

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

		public PmxBody RefBodyA
		{
			get;
			set;
		}

		public PmxBody RefBodyB
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

		public PmxJoint()
		{
			Name = "";
			NameE = "";
			BodyA = -1;
			BodyB = -1;
		}

		public PmxJoint(PmxJoint joint, bool nonStr = false)
		{
			FromPmxJoint(joint, nonStr);
		}

		public void FromPmxJoint(PmxJoint joint, bool nonStr = false)
		{
			if (!nonStr)
			{
				Name = joint.Name;
				NameE = joint.NameE;
			}
			Kind = joint.Kind;
			BodyA = joint.BodyA;
			BodyB = joint.BodyB;
			Position = joint.Position;
			Rotation = joint.Rotation;
			Limit_MoveLow = joint.Limit_MoveLow;
			Limit_MoveHigh = joint.Limit_MoveHigh;
			Limit_AngleLow = joint.Limit_AngleLow;
			Limit_AngleHigh = joint.Limit_AngleHigh;
			SpConst_Move = joint.SpConst_Move;
			SpConst_Rotate = joint.SpConst_Rotate;
			FromID(joint);
		}

		public void ClearLimit()
		{
			Limit_AngleLow = Vector3.Zero;
			Limit_AngleHigh = Vector3.Zero;
			if (Kind == JointKind.ConeTwist)
			{
				Limit_MoveLow.Y = 0f;
				Limit_MoveHigh.Y = 0f;
			}
			else
			{
				Limit_MoveLow = Vector3.Zero;
				Limit_MoveHigh = Vector3.Zero;
			}
		}

		public void ClearParameter()
		{
			switch (Kind)
			{
			case JointKind.Sp6DOF:
			case JointKind.G6DOF:
			case JointKind.P2P:
			case JointKind.Slider:
				SpConst_Move = Vector3.Zero;
				SpConst_Rotate = Vector3.Zero;
				break;
			case JointKind.ConeTwist:
				Limit_MoveLow.X = 0f;
				Limit_MoveHigh.X = 1f;
				Limit_MoveLow.Z = 0f;
				Limit_MoveHigh.Z = 0f;
				SpConst_Move = new Vector3(1f, 0.3f, 1f);
				SpConst_Rotate = Vector3.Zero;
				break;
			case JointKind.Hinge:
				SpConst_Move = new Vector3(0.9f, 0.3f, 1f);
				SpConst_Rotate = Vector3.Zero;
				break;
			}
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			Name = PmxStreamHelper.ReadString(s, f);
			NameE = PmxStreamHelper.ReadString(s, f);
			Kind = (JointKind)s.ReadByte();
			BodyA = PmxStreamHelper.ReadElement_Int32(s, f.BodySize);
			BodyB = PmxStreamHelper.ReadElement_Int32(s, f.BodySize);
			Position = V3_BytesConvert.FromStream(s);
			Rotation = V3_BytesConvert.FromStream(s);
			Limit_MoveLow = V3_BytesConvert.FromStream(s);
			Limit_MoveHigh = V3_BytesConvert.FromStream(s);
			Limit_AngleLow = V3_BytesConvert.FromStream(s);
			Limit_AngleHigh = V3_BytesConvert.FromStream(s);
			SpConst_Move = V3_BytesConvert.FromStream(s);
			SpConst_Rotate = V3_BytesConvert.FromStream(s);
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
			if (Kind != 0 && f.Ver < 2.1f)
			{
				s.WriteByte(0);
			}
			else
			{
				s.WriteByte((byte)Kind);
			}
			PmxStreamHelper.WriteElement_Int32(s, BodyA, f.BodySize);
			PmxStreamHelper.WriteElement_Int32(s, BodyB, f.BodySize);
			V3_BytesConvert.ToStream(s, Position);
			V3_BytesConvert.ToStream(s, Rotation);
			V3_BytesConvert.ToStream(s, Limit_MoveLow);
			V3_BytesConvert.ToStream(s, Limit_MoveHigh);
			V3_BytesConvert.ToStream(s, Limit_AngleLow);
			V3_BytesConvert.ToStream(s, Limit_AngleHigh);
			V3_BytesConvert.ToStream(s, SpConst_Move);
			V3_BytesConvert.ToStream(s, SpConst_Rotate);
			if (f.WithID)
			{
				PmxStreamHelper.WriteElement_UInt(s, base.UID);
				PmxStreamHelper.WriteElement_UInt(s, base.CID);
			}
		}

		object ICloneable.Clone()
		{
			return new PmxJoint(this);
		}

		public PmxJoint Clone()
		{
			return new PmxJoint(this);
		}
	}
}
