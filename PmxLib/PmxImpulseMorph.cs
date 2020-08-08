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

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.ImpulseMorph;
			}
		}

		public override int BaseIndex
		{
			get
			{
				return this.Index;
			}
			set
			{
				this.Index = value;
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
			this.Local = false;
			this.Velocity = Vector3.zero;
			this.Torque = Vector3.zero;
		}

		public PmxImpulseMorph(int index, bool local, Vector3 t, Vector3 r)
		{
			this.Index = index;
			this.Local = local;
			this.Velocity = t;
			this.Torque = r;
		}

		public PmxImpulseMorph(PmxImpulseMorph sv)
			: this()
		{
			this.FromPmxImpulseMorph(sv);
		}

		public void FromPmxImpulseMorph(PmxImpulseMorph sv)
		{
			this.Index = sv.Index;
			this.Local = sv.Local;
			this.Velocity = sv.Velocity;
			this.Torque = sv.Torque;
			this.ZeroFlag = sv.ZeroFlag;
		}

		public bool UpdateZeroFlag()
		{
			this.ZeroFlag = (this.Velocity == Vector3.zero && this.Torque == Vector3.zero);
			return this.ZeroFlag;
		}

		public void Clear()
		{
			this.Velocity = Vector3.zero;
			this.Torque = Vector3.zero;
		}

		public override void FromStreamEx(Stream s, PmxElementFormat size = null)
		{
			this.Index = PmxStreamHelper.ReadElement_Int32(s, size.BodySize, true);
			this.Local = (s.ReadByte() != 0);
			this.Velocity = V3_BytesConvert.FromStream(s);
			this.Torque = V3_BytesConvert.FromStream(s);
		}

		public override void ToStreamEx(Stream s, PmxElementFormat size = null)
		{
			PmxStreamHelper.WriteElement_Int32(s, this.Index, size.BodySize, true);
			s.WriteByte((byte)(this.Local ? 1 : 0));
			V3_BytesConvert.ToStream(s, this.Velocity);
			V3_BytesConvert.ToStream(s, this.Torque);
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
