using System;

namespace PmxLib
{
	public class VmdIplData : ICloneable
	{
		public VmdIplPoint P1;

		public VmdIplPoint P2;

		public VmdIplData()
		{
			this.P1 = new VmdIplPoint(20, 20);
			this.P2 = new VmdIplPoint(107, 107);
		}

		public VmdIplData(VmdIplData ip)
		{
			this.P1 = (VmdIplPoint)ip.P1.Clone();
			this.P2 = (VmdIplPoint)ip.P2.Clone();
		}

		public object Clone()
		{
			return new VmdIplData(this);
		}
	}
}
