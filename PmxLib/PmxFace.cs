using System;

namespace PmxLib
{
	internal class PmxFace : IPmxObjectKey, ICloneable
	{
		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.Face;

		public PmxVertex V0
		{
			get;
			set;
		}

		public PmxVertex V1
		{
			get;
			set;
		}

		public PmxVertex V2
		{
			get;
			set;
		}

		public PmxFace()
		{
		}

		public PmxFace(PmxFace f)
		{
			FromPmxFace(f);
		}

		public void FromPmxFace(PmxFace f)
		{
			V0 = f.V0;
			V1 = f.V1;
			V2 = f.V2;
		}

		object ICloneable.Clone()
		{
			return new PmxFace(this);
		}

		public PmxFace Clone()
		{
			return new PmxFace(this);
		}
	}
}
