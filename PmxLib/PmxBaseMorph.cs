using System;
using System.IO;

namespace PmxLib
{
	public abstract class PmxBaseMorph : IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.BaseMorph;
			}
		}

		public virtual int BaseIndex
		{
			get;
			set;
		}

		public void FromPmxBaseMorph(PmxBaseMorph sv)
		{
			this.BaseIndex = sv.BaseIndex;
		}

		public static PmxBaseMorph CreateOffsetObject(PmxMorph.OffsetKind kind)
		{
			PmxBaseMorph result = null;
			switch (kind)
			{
			case PmxMorph.OffsetKind.Group:
			case PmxMorph.OffsetKind.Flip:
				result = new PmxGroupMorph();
				break;
			case PmxMorph.OffsetKind.Vertex:
				result = new PmxVertexMorph();
				break;
			case PmxMorph.OffsetKind.Bone:
				result = new PmxBoneMorph();
				break;
			case PmxMorph.OffsetKind.UV:
			case PmxMorph.OffsetKind.UVA1:
			case PmxMorph.OffsetKind.UVA2:
			case PmxMorph.OffsetKind.UVA3:
			case PmxMorph.OffsetKind.UVA4:
				result = new PmxUVMorph();
				break;
			case PmxMorph.OffsetKind.Material:
				result = new PmxMaterialMorph();
				break;
			case PmxMorph.OffsetKind.Impulse:
				result = new PmxImpulseMorph();
				break;
			}
			return result;
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public virtual PmxBaseMorph Clone()
		{
			return null;
		}

		public virtual void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
		}

		public virtual void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
		}
	}
}
