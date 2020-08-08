using System;
using System.IO;

namespace PmxLib
{
	internal class PmxBoneMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public int Index;

		public Vector3 Translation;

		public Quaternion Rotaion;

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.BoneMorph;
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

		public PmxBone RefBone
		{
			get;
			set;
		}

		public PmxBoneMorph()
		{
		}

		public PmxBoneMorph(int index, Vector3 t, Quaternion r)
			: this()
		{
			this.Index = index;
			this.Translation = t;
			this.Rotaion = r;
		}

		public PmxBoneMorph(PmxBoneMorph sv)
			: this()
		{
			this.FromPmxBoneMorph(sv);
		}

		public void FromPmxBoneMorph(PmxBoneMorph sv)
		{
			this.Index = sv.Index;
			this.Translation = sv.Translation;
			this.Rotaion = sv.Rotaion;
		}

		public void Clear()
		{
			this.Translation = Vector3.zero;
			this.Rotaion = Quaternion.identity;
		}

		public override void FromStreamEx(Stream s, PmxElementFormat size = null)
		{
			this.Index = PmxStreamHelper.ReadElement_Int32(s, size.BoneSize, true);
			this.Translation = V3_BytesConvert.FromStream(s);
			Vector4 vector = V4_BytesConvert.FromStream(s);
			this.Rotaion = new Quaternion(vector.x, vector.y, vector.z, vector.w);
		}

		public override void ToStreamEx(Stream s, PmxElementFormat size = null)
		{
			PmxStreamHelper.WriteElement_Int32(s, this.Index, size.BoneSize, true);
			V3_BytesConvert.ToStream(s, this.Translation);
			V4_BytesConvert.ToStream(s, new Vector4(this.Rotaion.x, this.Rotaion.y, this.Rotaion.z, this.Rotaion.w));
		}

		object ICloneable.Clone()
		{
			return new PmxBoneMorph(this);
		}

		public override PmxBaseMorph Clone()
		{
			return new PmxBoneMorph(this);
		}
	}
}
