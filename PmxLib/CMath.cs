using System;

namespace PmxLib
{
	internal static class CMath
	{
		public static float CrossVector2(Vector2 p, Vector2 q)
		{
			return p.X * q.Y - p.Y * q.X;
		}

		public static Vector2 NormalizeValue(Vector2 val)
		{
			if (float.IsNaN(val.X))
			{
				val.X = 0f;
			}
			if (float.IsNaN(val.Y))
			{
				val.Y = 0f;
			}
			return val;
		}

		public static Vector3 NormalizeValue(Vector3 val)
		{
			if (float.IsNaN(val.X))
			{
				val.X = 0f;
			}
			if (float.IsNaN(val.Y))
			{
				val.Y = 0f;
			}
			if (float.IsNaN(val.Z))
			{
				val.Z = 0f;
			}
			return val;
		}

		public static Vector4 NormalizeValue(Vector4 val)
		{
			if (float.IsNaN(val.X))
			{
				val.X = 0f;
			}
			if (float.IsNaN(val.Y))
			{
				val.Y = 0f;
			}
			if (float.IsNaN(val.Z))
			{
				val.Z = 0f;
			}
			if (float.IsNaN(val.W))
			{
				val.W = 0f;
			}
			return val;
		}

		public static bool GetIntersectPoint(Vector2 p0, Vector2 p1, Vector2 q0, Vector2 q1, ref Vector2 rate, ref Vector2 pos)
		{
			Vector2 vector = p1 - p0;
			Vector2 q2 = q1 - q0;
			float num = CMath.CrossVector2(vector, q2);
			if (num == 0f)
			{
				return false;
			}
			Vector2 p2 = q0 - p0;
			float num2 = CMath.CrossVector2(p2, vector);
			float num3 = CMath.CrossVector2(p2, q2);
			float num4 = num2 / num;
			float num5 = num3 / num;
			if (num5 + 1E-05f < 0f || num5 - 1E-05f > 1f || num4 + 1E-05f < 0f || num4 - 1E-05f > 1f)
			{
				return false;
			}
			rate = new Vector2(num5, num4);
			pos = p0 + vector * num5;
			return true;
		}

		public static Vector3 GetFaceNormal(Vector3 p0, Vector3 p1, Vector3 p2)
		{
			Vector3 lhs = p1 - p0;
			Vector3 rhs = p2 - p0;
			Vector3 result = Vector3.Cross(lhs, rhs);
			result.Normalize();
			return result;
		}

		public static Vector3 GetFaceOrigin(Vector3 p0, Vector3 p1, Vector3 p2)
		{
			return (p0 + p1 + p2) / 3f;
		}

		public static Vector3 GetTriangleIntersect(Vector3 org, Vector3 dir, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			Vector3 result = new Vector3(-1f, 0f, 0f);
			Vector3 vector = v1 - v0;
			Vector3 vector2 = v2 - v0;
			Vector3 rhs = Vector3.Cross(dir, vector2);
			float num = Vector3.Dot(vector, rhs);
			Vector3 rhs2;
			float num2;
			float num3;
			if (num > 0.001f)
			{
				Vector3 lhs = org - v0;
				num2 = Vector3.Dot(lhs, rhs);
				if (num2 < 0f || num2 > num)
				{
					return result;
				}
				rhs2 = Vector3.Cross(lhs, vector);
				num3 = Vector3.Dot(dir, rhs2);
				if (num3 < 0f || num2 + num3 > num)
				{
					return result;
				}
			}
			else
			{
				if (num >= -0.001f)
				{
					return result;
				}
				Vector3 lhs = org - v0;
				num2 = Vector3.Dot(lhs, rhs);
				if ((double)num2 > 0.0 || num2 < num)
				{
					return result;
				}
				rhs2 = Vector3.Cross(lhs, vector);
				num3 = Vector3.Dot(dir, rhs2);
				if ((double)num3 > 0.0 || num2 + num3 < num)
				{
					return result;
				}
			}
			float num4 = 1f / num;
			float num5 = Vector3.Dot(vector2, rhs2);
			num5 *= num4;
			num2 *= num4;
			num3 *= num4;
			result.X = num5;
			result.Y = num2;
			result.Z = num3;
			return result;
		}

		public static Vector3 GetLineCrossPoint(Vector3 p, Vector3 from, Vector3 dir, out float d)
		{
			Vector3 rhs = p - from;
			d = Vector3.Dot(dir, rhs);
			return dir * d + from;
		}

		public static Vector3 GetLineCrossPoint(Vector3 p, Vector3 from, Vector3 dir)
		{
			float num = default(float);
			return CMath.GetLineCrossPoint(p, from, dir, out num);
		}

		public static Matrix CreateViewportMatrix(int width, int height)
		{
			Matrix identity = Matrix.Identity;
			float num = (float)width * 0.5f;
			float num2 = (float)height * 0.5f;
			identity.M11 = num;
			identity.M22 = 0f - num2;
			identity.M41 = num;
			identity.M42 = num2;
			return identity;
		}

		public static bool InArcPosition(Vector3 pos, float cx, float cy, float r2, out float d2)
		{
			float num = pos.X - cx;
			float num2 = pos.Y - cy;
			d2 = num * num + num2 * num2;
			return d2 <= r2;
		}

		public static bool InArcPosition(Vector3 pos, float cx, float cy, float r2)
		{
			float num = default(float);
			return CMath.InArcPosition(pos, cx, cy, r2, out num);
		}

		public static void NormalizeRotateXYZ(ref Vector3 v)
		{
			if (v.X < -3.14159274f)
			{
				v.X += 6.28318548f;
			}
			else if (v.X > 3.14159274f)
			{
				v.X -= 6.28318548f;
			}
			if (v.Y < -3.14159274f)
			{
				v.Y += 6.28318548f;
			}
			else if (v.Y > 3.14159274f)
			{
				v.Y -= 6.28318548f;
			}
			if (v.Z < -3.14159274f)
			{
				v.Z += 6.28318548f;
			}
			else if (v.Z > 3.14159274f)
			{
				v.Z -= 6.28318548f;
			}
		}

		public static Vector3 MatrixToEuler_ZXY0(ref Matrix m)
		{
			Vector3 zero = Vector3.Zero;
			if (m.M32 == 1f)
			{
				zero.X = 1.57079637f;
				zero.Z = (float)Math.Atan2((double)m.M21, (double)m.M11);
			}
			else if (m.M32 == -1f)
			{
				zero.X = -1.57079637f;
				zero.Z = (float)Math.Atan2((double)m.M21, (double)m.M11);
			}
			else
			{
				zero.X = 0f - (float)Math.Asin((double)m.M32);
				zero.Y = 0f - (float)Math.Atan2(0.0 - (double)m.M31, (double)m.M33);
				zero.Z = 0f - (float)Math.Atan2(0.0 - (double)m.M12, (double)m.M22);
			}
			return zero;
		}

		public static Vector3 MatrixToEuler_ZXY(ref Matrix m)
		{
			Vector3 zero = Vector3.Zero;
			zero.X = 0f - (float)Math.Asin((double)m.M32);
			if (zero.X == 1.57079637f || zero.X == -1.57079637f)
			{
				zero.Y = (float)Math.Atan2(0.0 - (double)m.M13, (double)m.M11);
			}
			else
			{
				zero.Y = (float)Math.Atan2((double)m.M31, (double)m.M33);
				zero.Z = (float)Math.Asin((double)(m.M12 / (float)Math.Cos((double)zero.X)));
				if (m.M22 < 0f)
				{
					zero.Z = 3.14159274f - zero.Z;
				}
			}
			return zero;
		}

		public static Vector3 MatrixToEuler_XYZ(ref Matrix m)
		{
			Vector3 zero = Vector3.Zero;
			zero.Y = 0f - (float)Math.Asin((double)m.M13);
			if (zero.Y == 1.57079637f || zero.Y == -1.57079637f)
			{
				zero.Z = (float)Math.Atan2(0.0 - (double)m.M21, (double)m.M22);
			}
			else
			{
				zero.Z = (float)Math.Atan2((double)m.M12, (double)m.M11);
				zero.X = (float)Math.Asin((double)(m.M23 / (float)Math.Cos((double)zero.Y)));
				if (m.M33 < 0f)
				{
					zero.X = 3.14159274f - zero.X;
				}
			}
			return zero;
		}

		public static Vector3 MatrixToEuler_YZX(ref Matrix m)
		{
			Vector3 zero = Vector3.Zero;
			zero.Z = 0f - (float)Math.Asin((double)m.M21);
			if (zero.Z == 1.57079637f || zero.Z == -1.57079637f)
			{
				zero.X = (float)Math.Atan2(0.0 - (double)m.M32, (double)m.M33);
			}
			else
			{
				zero.X = (float)Math.Atan2((double)m.M23, (double)m.M22);
				zero.Y = (float)Math.Asin((double)(m.M31 / (float)Math.Cos((double)zero.Z)));
				if (m.M11 < 0f)
				{
					zero.Y = 3.14159274f - zero.Y;
				}
			}
			return zero;
		}

		public static Vector3 MatrixToEuler_ZXY_Lim2(ref Matrix m)
		{
			Vector3 zero = Vector3.Zero;
			zero.X = 0f - (float)Math.Asin((double)m.M32);
			if (zero.X == 1.57079637f || zero.X == -1.57079637f)
			{
				zero.Y = (float)Math.Atan2(0.0 - (double)m.M13, (double)m.M11);
			}
			else
			{
				if (1.53588974f < zero.X)
				{
					zero.X = 1.53588974f;
				}
				else if (zero.X < -1.53588974f)
				{
					zero.X = -1.53588974f;
				}
				zero.Y = (float)Math.Atan2((double)m.M31, (double)m.M33);
				zero.Z = (float)Math.Asin((double)(m.M12 / (float)Math.Cos((double)zero.X)));
				if (m.M22 < 0f)
				{
					zero.Z = 3.14159274f - zero.Z;
				}
			}
			return zero;
		}

		public static Vector3 MatrixToEuler_XYZ_Lim2(ref Matrix m)
		{
			Vector3 zero = Vector3.Zero;
			zero.Y = 0f - (float)Math.Asin((double)m.M13);
			if (zero.Y == 1.57079637f || zero.Y == -1.57079637f)
			{
				zero.Z = (float)Math.Atan2(0.0 - (double)m.M21, (double)m.M22);
			}
			else
			{
				if (1.53588974f < zero.Y)
				{
					zero.Y = 1.53588974f;
				}
				else if (zero.Y < -1.53588974f)
				{
					zero.Y = -1.53588974f;
				}
				zero.Z = (float)Math.Atan2((double)m.M12, (double)m.M11);
				zero.X = (float)Math.Asin((double)(m.M23 / (float)Math.Cos((double)zero.Y)));
				if (m.M33 < 0f)
				{
					zero.X = 3.14159274f - zero.X;
				}
			}
			return zero;
		}

		public static Vector3 MatrixToEuler_YZX_Lim2(ref Matrix m)
		{
			Vector3 zero = Vector3.Zero;
			zero.Z = 0f - (float)Math.Asin((double)m.M21);
			if (zero.Z == 1.57079637f || zero.Z == -1.57079637f)
			{
				zero.X = (float)Math.Atan2(0.0 - (double)m.M32, (double)m.M33);
			}
			else
			{
				if (1.53588974f < zero.Z)
				{
					zero.Z = 1.53588974f;
				}
				else if (zero.Z < -1.53588974f)
				{
					zero.Z = -1.53588974f;
				}
				zero.X = (float)Math.Atan2((double)m.M23, (double)m.M22);
				zero.Y = (float)Math.Asin((double)(m.M31 / (float)Math.Cos((double)zero.Z)));
				if (m.M11 < 0f)
				{
					zero.Y = 3.14159274f - zero.Y;
				}
			}
			return zero;
		}
	}
}
