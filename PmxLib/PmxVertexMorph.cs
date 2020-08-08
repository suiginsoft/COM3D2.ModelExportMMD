using System;
using System.IO;

namespace PmxLib
{
	internal class PmxVertexMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public int Index;

		public Vector3 Offset;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.VertexMorph;

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

		public PmxVertexMorph()
		{
			Index = -1;
		}

		public PmxVertexMorph(int index, Vector3 offset)
			: this()
		{
			Index = index;
			Offset = offset;
		}

		public PmxVertexMorph(PmxVertexMorph sv)
			: this()
		{
			FromPmxVertexMorph(sv);
		}

		public void FromPmxVertexMorph(PmxVertexMorph sv)
		{
			FromPmxBaseMorph(sv);
			Offset = sv.Offset;
		}

		public override void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.FromStreamEx(s, f);
			Index = PmxStreamHelper.ReadElement_Int32(s, f.VertexSize, signed: false);
			Offset = V3_BytesConvert.FromStream(s);
		}

		public override void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.ToStreamEx(s, f);
			PmxStreamHelper.WriteElement_Int32(s, Index, f.VertexSize, signed: false);
			V3_BytesConvert.ToStream(s, Offset);
		}

		object ICloneable.Clone()
		{
			return new PmxVertexMorph(this);
		}

		public override PmxBaseMorph Clone()
		{
			return new PmxVertexMorph(this);
		}
	}
}
