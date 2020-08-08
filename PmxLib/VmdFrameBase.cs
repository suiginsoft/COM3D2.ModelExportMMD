using System.Collections.Generic;

namespace PmxLib
{
	internal abstract class VmdFrameBase : IComparer<VmdFrameBase>
	{
		public int FrameIndex;

		public static int Compare(VmdFrameBase x, VmdFrameBase y)
		{
			return x.FrameIndex - y.FrameIndex;
		}

		int IComparer<VmdFrameBase>.Compare(VmdFrameBase x, VmdFrameBase y)
		{
			return Compare(x, y);
		}
	}
}
