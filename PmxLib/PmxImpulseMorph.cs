using System;
using System.IO;

namespace PmxLib
{
	internal class PmxImpulseMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public int Index;

		public bool Local;

		public Vector3 Velocity;

		public Vector3 Torque;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.ImpulseMorph;

		public override int BaseIndex
		{
			get
			{
				return Index;
			}
			set
			{
				Index = value;
			}
		}

		public PmxBody RefBody
		{
			get;
			set;
		}

		public bool ZeroFlag
		{
			get;
			set;
		}

		public PmxImpulseMorph()
		{
			Local = false;
			Velocity = Vector3.Zero;
			Torque = Vector3.Zero;
		}

		public PmxImpulseMorph(int index, bool local, Vector3 t, Vector3 r)
		{
			Index = index;
			Local = local;
			Velocity = t;
			Torque = r;
		}

		public PmxImpulseMorph(PmxImpulseMorph sv)
			: this()
		{
			FromPmxImpulseMorph(sv);
		}

		public void FromPmxImpulseMorph(PmxImpulseMorph sv)
		{
			FromPmxBaseMorph(sv);
			Local = sv.Local;
			Velocity = sv.Velocity;
			Torque = sv.Torque;
			ZeroFlag = sv.ZeroFlag;
		}

		public bool UpdateZeroFlag()
		{
			ZeroFlag = (Velocity == Vector3.Zero && Torque == Vector3.Zero);
			return ZeroFlag;
		}

		public void Clear()
		{
			Velocity = Vector3.Zero;
			Torque = Vector3.Zero;
		}

		public override void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.FromStreamEx(s, f);
			Index = PmxStreamHelper.ReadElement_Int32(s, f.BodySize);
			Local = (s.ReadByte() != 0);
			Velocity = V3_BytesConvert.FromStream(s);
			Torque = V3_BytesConvert.FromStream(s);
		}

		public override void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.ToStreamEx(s, f);
			PmxStreamHelper.WriteElement_Int32(s, Index, f.BodySize);
			s.WriteByte((byte)(Local ? 1u : 0u));
			V3_BytesConvert.ToStream(s, Velocity);
			V3_BytesConvert.ToStream(s, Torque);
		}

		object ICloneable.Clone()
		{
			return new PmxImpulseMorph(this);
		}

		public override PmxBaseMorph Clone()
		{
			return new PmxImpulseMorph(this);
		}
	}
}
