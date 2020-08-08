using System;
using System.IO;

namespace PmxLib
{
	internal class PmxUVMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public int Index;

		public Vector4 Offset;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.UVMorph;

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

		public PmxVertex RefVertex
		{
			get;
			set;
		}

		public PmxUVMorph()
		{
			Index = -1;
		}

		public PmxUVMorph(int index, Vector4 offset)
			: this()
		{
			Index = index;
			Offset = offset;
		}

		public PmxUVMorph(PmxUVMorph sv)
			: this()
		{
			FromPmxUVMorph(sv);
		}

		public void FromPmxUVMorph(PmxUVMorph sv)
		{
			FromPmxBaseMorph(sv);
			Offset = sv.Offset;
		}

		public override void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.FromStreamEx(s, f);
			Index = PmxStreamHelper.ReadElement_Int32(s, f.VertexSize, signed: false);
			Offset = V4_BytesConvert.FromStream(s);
		}

		public override void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.ToStreamEx(s, f);
			PmxStreamHelper.WriteElement_Int32(s, Index, f.VertexSize, signed: false);
			V4_BytesConvert.ToStream(s, Offset);
		}

		object ICloneable.Clone()
		{
			return new PmxUVMorph(this);
		}

		public override PmxBaseMorph Clone()
		{
			return new PmxUVMorph(this);
		}
	}
}
