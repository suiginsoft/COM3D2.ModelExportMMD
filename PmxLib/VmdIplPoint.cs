using System;
using System.Drawing;

namespace PmxLib
{
	internal class VmdIplPoint : ICloneable
	{
		protected int m_x;

		protected int m_y;

		public int X
		{
			get
			{
				return m_x;
			}
			set
			{
				m_x = RangeValue(value);
			}
		}

		public int Y
		{
			get
			{
				return m_y;
			}
			set
			{
				m_y = RangeValue(value);
			}
		}

		public VmdIplPoint()
		{
		}

		public VmdIplPoint(VmdIplPoint ip)
		{
			X = ip.X;
			Y = ip.Y;
		}

		public VmdIplPoint(int x, int y)
		{
			X = x;
			Y = y;
		}

		public VmdIplPoint(Point p)
		{
			Set(p);
		}

		public void Set(Point p)
		{
			X = p.X;
			Y = p.Y;
		}

		protected int RangeValue(int v)
		{
			v = ((v >= 0) ? v : 0);
			v = ((v > 127) ? 127 : v);
			return v;
		}

		public static implicit operator Point(VmdIplPoint p)
		{
			return new Point(p.X, p.Y);
		}

		public static implicit operator VmdIplPoint(Point p)
		{
			return new VmdIplPoint(p);
		}

		public object Clone()
		{
			return new VmdIplPoint(this);
		}
	}
}
