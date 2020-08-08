using System;

namespace PmxLib
{
	public class VmdIplPoint : ICloneable
	{
		protected int m_x;

		protected int m_y;

		public int X
		{
			get
			{
				return this.m_x;
			}
			set
			{
				this.m_x = this.RangeValue(value);
			}
		}

		public int Y
		{
			get
			{
				return this.m_y;
			}
			set
			{
				this.m_y = this.RangeValue(value);
			}
		}

		public VmdIplPoint()
		{
		}

		public VmdIplPoint(VmdIplPoint ip)
		{
			this.X = ip.X;
			this.Y = ip.Y;
		}

		public VmdIplPoint(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public VmdIplPoint(Point p)
		{
			this.Set(p);
		}

		public void Set(Point p)
		{
			this.X = p.X;
			this.Y = p.Y;
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
