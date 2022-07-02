namespace PmxLib
{
	public struct Vector4
	{
		public float x;
		public float y;
		public float z;
		public float w;

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

		public float W
		{
			get { return w; }
			set { w = value; }
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

		public float Alpha
		{
			get { return w; }
			set { w = value; }
		}

		public static Vector4 Zero
		{
			get { return new Vector4(0f, 0f, 0f, 0f); }
		}

		public static Vector4 UnitX
		{
			get { return new Vector4(1f, 0f, 0f, 0f); }
		}

		public static Vector4 UnitY
		{
			get { return new Vector4(0f, 1f, 0f, 0f); }
		}

		public static Vector4 UnitZ
		{
			get { return new Vector4(0f, 0f, 1f, 0f); }
		}

		public static Vector4 UnitW
		{
			get { return new Vector4(0f, 0f, 0f, 1f); }
		}

		public Vector4(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Vector4(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			w = 0f;
		}

		public Vector4(float x, float y)
		{
			this.x = x;
			this.y = y;
			z = 0f;
			w = 0f;
		}

		public Vector4(System.Drawing.Color c)
		{
			x = (float)((double)c.R / 255.0);
			y = (float)((double)c.G / 255.0);
			z = (float)((double)c.B / 255.0);
			w = (float)((double)c.A / 255.0);
		}

		public Vector4(UnityEngine.Color c)
		{
			x = c.r;
			y = c.g;
			z = c.b;
			w = c.a;
		}

		public Vector4(UnityEngine.Vector4 v)
		{
			x = v.x;
			y = v.y;
			z = v.z;
			w = v.w;
		}

		public static float Dot(Vector4 a, Vector4 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		public static float SqrMagnitude(Vector4 a)
		{
			return Vector4.Dot(a, a);
		}

		public static Vector4 operator +(Vector4 a, Vector4 b)
		{
			return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Vector4 operator -(Vector4 a, Vector4 b)
		{
			return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static Vector4 operator -(Vector4 a)
		{
			return new Vector4(0f - a.x, 0f - a.y, 0f - a.z, 0f - a.w);
		}

		public static Vector4 operator *(Vector4 a, float d)
		{
			return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
		}

		public static Vector4 operator *(float d, Vector4 a)
		{
			return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
		}

		public static Vector4 operator /(Vector4 a, float d)
		{
			return new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);
		}

		public static bool operator ==(Vector4 lhs, Vector4 rhs)
		{
			return Vector4.SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
		}

		public static bool operator !=(Vector4 lhs, Vector4 rhs)
		{
			return Vector4.SqrMagnitude(lhs - rhs) >= 9.99999944E-11f;
		}

		public static implicit operator Vector4(Vector3 v)
		{
			return new Vector4(v.x, v.y, v.z, 0f);
		}

		public static implicit operator Vector3(Vector4 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		public static implicit operator Vector4(Vector2 v)
		{
			return new Vector4(v.x, v.y, 0f, 0f);
		}

		public static implicit operator Vector2(Vector4 v)
		{
			return new Vector2(v.x, v.y);
		}

		public override bool Equals(object other)
		{
			if (!(other is Vector4))
			{
				return false;
			}
			Vector4 vector = (Vector4)other;
			return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z) && w.Equals(vector.w);
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2 ^ w.GetHashCode() >> 1;
		}
	}
}
