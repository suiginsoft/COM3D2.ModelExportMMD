using System;

namespace PmxLib
{
	public struct Vector3
	{
		public float x;
		public float y;
		public float z;

		public float X
		{
			get { return x; }
			set { x = value; }
		}

		public float Y
		{
			get { return y; }
			set { y = value; }
		}

		public float Z
		{
			get { return z; }
			set { z = value; }
		}

		public float Red
		{
			get { return x; }
			set { x = value; }
		}

		public float Green
		{
			get { return y; }
			set { y = value; }
		}

		public float Blue
		{
			get { return z; }
			set { z = value; }
		}

		public static Vector3 Zero
		{
			get { return new Vector3(0f, 0f, 0f); }
		}

		public static Vector3 UnitX
		{
			get { return new Vector3(1f, 0f, 0f); }
		}

		public static Vector3 UnitY
		{
			get { return new Vector3(0f, 1f, 0f); }
		}

		public static Vector3 UnitZ
		{
			get { return new Vector3(0f, 0f, 1f); }
		}

        public Vector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3(float x, float y)
		{
			this.x = x;
			this.y = y;
			z = 0f;
		}

		public Vector3(System.Drawing.Color c)
		{
			x = (float)((double)c.R / 255.0);
			y = (float)((double)c.G / 255.0);
			z = (float)((double)c.B / 255.0);
		}

		public Vector3(UnityEngine.Color c)
		{
			x = c.r;
			y = c.g;
			z = c.b;
		}

		public Vector3(UnityEngine.Vector3 v)
		{
			x = v.x;
			y = v.y;
			z = v.z;
		}

		public static float Dot(Vector3 lhs, Vector3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}

		public static float SqrMagnitude(Vector3 a)
		{
			return a.x * a.x + a.y * a.y + a.z * a.z;
		}

		public float Length()
		{
			double num = (double)Y;
			double num2 = (double)X;
			double num3 = (double)Z;
			double num4 = num2;
			double num5 = num4 * num4;
			double num6 = num;
			double num7 = num5 + num6 * num6;
			double num8 = num3;
			return (float)Math.Sqrt(num7 + num8 * num8);
		}

		public void Normalize()
		{
			float num = Length();
			if (num != 0f)
			{
				float num2 = (float)(1.0 / (double)num);
				X = (float)((double)X * (double)num2);
				Y = (float)((double)Y * (double)num2);
				Z = (float)((double)Z * (double)num2);
			}
		}

		public static Vector3 operator+(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Vector3 operator-(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vector3 operator-(Vector3 a)
		{
			return new Vector3(0f - a.x, 0f - a.y, 0f - a.z);
		}

		public static Vector3 operator*(Vector3 a, float d)
		{
			return new Vector3(a.x * d, a.y * d, a.z * d);
		}

		public static Vector3 operator*(float d, Vector3 a)
		{
			return new Vector3(a.x * d, a.y * d, a.z * d);
		}

		public static Vector3 operator/(Vector3 a, float d)
		{
			return new Vector3(a.x / d, a.y / d, a.z / d);
		}

		public static bool operator==(Vector3 lhs, Vector3 rhs)
		{
			return Vector3.SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
		}

		public static bool operator!=(Vector3 lhs, Vector3 rhs)
		{
			return Vector3.SqrMagnitude(lhs - rhs) >= 9.99999944E-11f;
		}

		public override string ToString()
		{
			return string.Format("X:{0} Y:{1} Z:{2}", new object[3]
			{
				X.ToString(),
				Y.ToString(),
				Z.ToString()
			});
		}

		public override bool Equals(object other)
		{
			if (!(other is Vector3))
			{
				return false;
			}
			Vector3 vector = (Vector3)other;
			return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z);
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
		}
	}
}
