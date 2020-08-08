using System;
using System.IO;

namespace PmxLib
{
	internal class PmxGroupMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public int Index;

		public float Ratio;

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.GroupMorph;
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

		public PmxMorph RefMorph
		{
			get;
			set;
		}

		public PmxGroupMorph()
		{
			this.Ratio = 1f;
		}

		public PmxGroupMorph(int index, float r)
			: this()
		{
			this.Index = index;
			this.Ratio = r;
		}

		public PmxGroupMorph(PmxGroupMorph sv)
			: this()
		{
			this.FromPmxGroupMorph(sv);
		}

		public void FromPmxGroupMorph(PmxGroupMorph sv)
		{
			this.Index = sv.Index;
			this.Ratio = sv.Ratio;
		}

		public override void FromStreamEx(Stream s, PmxElementFormat size = null)
		{
			this.Index = PmxStreamHelper.ReadElement_Int32(s, size.MorphSize, true);
			this.Ratio = PmxStreamHelper.ReadElement_Float(s);
		}

		public override void ToStreamEx(Stream s, PmxElementFormat size = null)
		{
			PmxStreamHelper.WriteElement_Int32(s, this.Index, size.MorphSize, true);
			PmxStreamHelper.WriteElement_Float(s, this.Ratio);
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
