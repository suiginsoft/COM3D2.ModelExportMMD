namespace PmxLib
{
	public struct Quaternion
	{
		public float x;

		public float y;

		public float z;

		public float w;

		public float X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		public float Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		public float Z
		{
			get
			{
				return this.z;
			}
			set
			{
				this.z = value;
			}
		}

		public float W
		{
			get
			{
				return this.w;
			}
			set
			{
				this.w = value;
			}
		}

		public static Quaternion identity
		{
			get
			{
				Quaternion result = default(Quaternion);
				result.X = 0f;
				result.Y = 0f;
				result.Z = 0f;
				result.W = 1f;
				return result;
			}
		}

		public static Quaternion Identity
		{
			get
			{
				Quaternion result = default(Quaternion);
				result.X = 0f;
				result.Y = 0f;
				result.Z = 0f;
				result.W = 1f;
				return result;
			}
		}

		public Quaternion(Vector3 value, float w)
		{
			this.x = value.X;
			this.y = value.Y;
			this.z = value.Z;
			this.w = w;
		}

		public Quaternion(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public static Quaternion operator *(float scale, Quaternion quaternion)
		{
			Quaternion result = default(Quaternion);
			result.X = (float)((double)quaternion.X * (double)scale);
			result.Y = (float)((double)quaternion.Y * (double)scale);
			result.Z = (float)((double)quaternion.Z * (double)scale);
			result.W = (float)((double)quaternion.W * (double)scale);
			return result;
		}

		public static Quaternion operator *(Quaternion quaternion, float scale)
		{
			Quaternion result = default(Quaternion);
			result.X = (float)((double)quaternion.X * (double)scale);
			result.Y = (float)((double)quaternion.Y * (double)scale);
			result.Z = (float)((double)quaternion.Z * (double)scale);
			result.W = (float)((double)quaternion.W * (double)scale);
			return result;
		}

		public static Quaternion operator *(Quaternion left, Quaternion right)
		{
			Quaternion result = default(Quaternion);
			float num = left.X;
			float num2 = left.Y;
			float num3 = left.Z;
			float num4 = left.W;
			float num5 = right.X;
			float num6 = right.Y;
			float num7 = right.Z;
			float num8 = right.W;
			result.X = (float)((double)num5 * (double)num4 + (double)num8 * (double)num + (double)num6 * (double)num3 - (double)num7 * (double)num2);
			result.Y = (float)((double)num6 * (double)num4 + (double)num8 * (double)num2 + (double)num7 * (double)num - (double)num5 * (double)num3);
			result.Z = (float)((double)num7 * (double)num4 + (double)num8 * (double)num3 + (double)num5 * (double)num2 - (double)num6 * (double)num);
			result.W = (float)((double)num8 * (double)num4 - ((double)num6 * (double)num2 + (double)num5 * (double)num + (double)num7 * (double)num3));
			return result;
		}

		public static Quaternion operator /(Quaternion left, float right)
		{
			Quaternion result = default(Quaternion);
			result.X = (float)((double)left.X / (double)right);
			result.Y = (float)((double)left.Y / (double)right);
			result.Z = (float)((double)left.Z / (double)right);
			result.W = (float)((double)left.W / (double)right);
			return result;
		}

		public static Quaternion operator +(Quaternion left, Quaternion right)
		{
			Quaternion result = default(Quaternion);
			result.X = (float)((double)left.X + (double)right.X);
			result.Y = (float)((double)left.Y + (double)right.Y);
			result.Z = (float)((double)left.Z + (double)right.Z);
			result.W = (float)((double)left.W + (double)right.W);
			return result;
		}

		public static Quaternion operator -(Quaternion quaternion)
		{
			Quaternion result = default(Quaternion);
			result.X = 0f - quaternion.X;
			result.Y = 0f - quaternion.Y;
			result.Z = 0f - quaternion.Z;
			result.W = 0f - quaternion.W;
			return result;
		}

		public static Quaternion operator -(Quaternion left, Quaternion right)
		{
			Quaternion result = default(Quaternion);
			result.X = (float)((double)left.X - (double)right.X);
			result.Y = (float)((double)left.Y - (double)right.Y);
			result.Z = (float)((double)left.Z - (double)right.Z);
			result.W = (float)((double)left.W - (double)right.W);
			return result;
		}

		public static bool operator ==(Quaternion left, Quaternion right)
		{
			return Quaternion.Equals(ref left, ref right);
		}

		public static bool operator !=(Quaternion left, Quaternion right)
		{
			return !Quaternion.Equals(ref left, ref right) && true;
		}

		public override string ToString()
		{
			return string.Format("X:{0} Y:{1} Z:{2} W:{3}", this.X.ToString(), this.Y.ToString(), this.Z.ToString(), this.W.ToString());
		}

		public override int GetHashCode()
		{
			float num = this.X;
			float num2 = this.Y;
			float num3 = this.Z;
			float num4 = this.W;
			int num5 = num3.GetHashCode() + num4.GetHashCode() + num2.GetHashCode();
			return num.GetHashCode() + num5;
		}

		public static bool Equals(ref Quaternion value1, ref Quaternion value2)
		{
			int num = ((double)value1.X == (double)value2.X && (double)value1.Y == (double)value2.Y && (double)value1.Z == (double)value2.Z && (double)value1.W == (double)value2.W) ? 1 : 0;
			return (byte)num != 0;
		}

		public bool Equals(Quaternion other)
		{
			int num = ((double)this.X == (double)other.X && (double)this.Y == (double)other.Y && (double)this.Z == (double)other.Z && (double)this.W == (double)other.W) ? 1 : 0;
			return (byte)num != 0;
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj.GetType() == ((object)this).GetType() && this.Equals((Quaternion)obj);
		}
	}
}
