using System;

namespace PmxLib
{
	public class PmxFace : IPmxObjectKey, ICloneable
	{
		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.Face;
			}
		}

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
			this.FromPmxFace(f);
		}

		public void FromPmxFace(PmxFace f)
		{
			this.V0 = f.V0;
			this.V1 = f.V1;
			this.V2 = f.V2;
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
