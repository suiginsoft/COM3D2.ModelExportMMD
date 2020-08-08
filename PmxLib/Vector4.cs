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
			get { return this.x; }
			set { this.x = value; }
		}

		public float Y
		{
			get { return this.y; }
			set { this.y = value; }
		}

		public float Z
		{
			get { return this.z; }
			set { this.z = value; }
		}

		public float W
		{
			get { return this.w; }
			set { this.w = value; }
		}

		public float Red
		{
			get { return this.x; }
			set { this.x = value; }
		}

		public float Green
		{
			get { return this.y; }
			set { this.y = value; }
		}

		public float Blue
		{
			get { return this.z; }
			set { this.z = value; }
		}

		public float Alpha
		{
			get { return this.w; }
			set { this.w = value; }
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
			this.w = 0f;
		}

		public Vector4(float x, float y)
		{
			this.x = x;
			this.y = y;
			this.z = 0f;
			this.w = 0f;
		}

		public Vector4(System.Drawing.Color c)
		{
			this.x = (float)((double)c.R / 255.0);
			this.y = (float)((double)c.G / 255.0);
			this.z = (float)((double)c.B / 255.0);
			this.w = (float)((double)c.A / 255.0);
		}

		public Vector4(UnityEngine.Color c)
		{
			this.x = c.r;
			this.y = c.g;
			this.z = c.b;
			this.w = c.a;
		}

		public Vector4(UnityEngine.Vector4 v)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = v.w;
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
			return this.x.Equals(vector.x) && this.y.Equals(vector.y) && this.z.Equals(vector.z) && this.w.Equals(vector.w);
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
		}
	}
}
