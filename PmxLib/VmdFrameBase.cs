using System.Collections.Generic;

namespace PmxLib
{
	public abstract class VmdFrameBase : IComparer<VmdFrameBase>
	{
		public int FrameIndex;

		public static int Compare(VmdFrameBase x, VmdFrameBase y)
		{
			return x.FrameIndex - y.FrameIndex;
		}

		int IComparer<VmdFrameBase>.Compare(VmdFrameBase x, VmdFrameBase y)
		{
			return VmdFrameBase.Compare(x, y);
		}
	}
}
