using System;
using System.IO;

namespace PmxLib
{
	internal class PmxBoneMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public int Index;

		public Vector3 Translation;

		public Quaternion Rotaion;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.BoneMorph;

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
			Index = index;
			Translation = t;
			Rotaion = r;
		}

		public PmxBoneMorph(PmxBoneMorph sv)
			: this()
		{
			FromPmxBoneMorph(sv);
		}

		public void FromPmxBoneMorph(PmxBoneMorph sv)
		{
			FromPmxBaseMorph(sv);
			Translation = sv.Translation;
			Rotaion = sv.Rotaion;
		}

		public void Clear()
		{
			Translation = Vector3.Zero;
			Rotaion = Quaternion.Identity;
		}

		public override void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.FromStreamEx(s, f);
			Index = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
			Translation = V3_BytesConvert.FromStream(s);
			Vector4 vector = V4_BytesConvert.FromStream(s);
			Rotaion = new Quaternion(vector.X, vector.Y, vector.Z, vector.W);
		}

		public override void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.ToStreamEx(s, f);
			PmxStreamHelper.WriteElement_Int32(s, Index, f.BoneSize);
			V3_BytesConvert.ToStream(s, Translation);
			V4_BytesConvert.ToStream(s, new Vector4(Rotaion.X, Rotaion.Y, Rotaion.Z, Rotaion.W));
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
