using System;
using System.IO;

namespace PmxLib
{
	internal class PmxGroupMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public int Index;

		public float Ratio;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.GroupMorph;

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

		public PmxMorph RefMorph
		{
			get;
			set;
		}

		public PmxGroupMorph()
		{
			Ratio = 1f;
		}

		public PmxGroupMorph(int index, float r)
			: this()
		{
			Index = index;
			Ratio = r;
		}

		public PmxGroupMorph(PmxGroupMorph sv)
			: this()
		{
			FromPmxGroupMorph(sv);
		}

		public void FromPmxGroupMorph(PmxGroupMorph sv)
		{
			FromPmxBaseMorph(sv);
			Ratio = sv.Ratio;
		}

		public override void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.FromStreamEx(s, f);
			Index = PmxStreamHelper.ReadElement_Int32(s, f.MorphSize);
			Ratio = PmxStreamHelper.ReadElement_Float(s);
		}

		public override void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.ToStreamEx(s, f);
			PmxStreamHelper.WriteElement_Int32(s, Index, f.MorphSize);
			PmxStreamHelper.WriteElement_Float(s, Ratio);
		}

		object ICloneable.Clone()
		{
			return new PmxGroupMorph(this);
		}

		public override PmxBaseMorph Clone()
		{
			return new PmxGroupMorph(this);
		}
	}
}
