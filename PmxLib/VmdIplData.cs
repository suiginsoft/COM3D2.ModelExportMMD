using System;

namespace PmxLib
{
	internal class VmdIplData : ICloneable
	{
		public VmdIplPoint P1;

		public VmdIplPoint P2;

		public VmdIplData()
		{
			P1 = new VmdIplPoint(20, 20);
			P2 = new VmdIplPoint(107, 107);
		}

		public VmdIplData(VmdIplData ip)
		{
			P1 = (VmdIplPoint)ip.P1.Clone();
			P2 = (VmdIplPoint)ip.P2.Clone();
		}

		public object Clone()
		{
			return new VmdIplData(this);
		}
	}
}
