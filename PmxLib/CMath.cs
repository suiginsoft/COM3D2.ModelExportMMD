using System;
using System.Drawing;

namespace PmxLib
{
	internal static class CMath
	{
		public static void MinMax<T>(T v1, T v2, out T min, out T max) where T : IComparable
		{
			if (v1.CompareTo(v2) <= 0)
			{
				min = v1;
				max = v2;
			}
			else
			{
				min = v2;
				max = v1;
			}
		}

		public static float CrossVector2(ref Vector2 p, ref Vector2 q)
		{
			return p.X * q.Y - p.Y * q.X;
		}

		public static void NormalizeValue(ref Vector2 val)
		{
			if (float.IsNaN(val.X))
			{
				val.X = 0f;
			}
			if (float.IsNaN(val.Y))
			{
				val.Y = 0f;
			}
		}

		public static void NormalizeValue(ref Vector3 val)
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
		}

		public static void NormalizeValue(ref Vector4 val)
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
		}

		public static Vector3 SwapMin(ref Vector3 min, ref Vector3 p)
		{
			return new Vector3(Math.Min(min.X, p.X), Math.Min(min.Y, p.Y), Math.Min(min.Z, p.Z));
		}

		public static Vector3 SwapMax(ref Vector3 max, ref Vector3 p)
		{
			return new Vector3(Math.Max(max.X, p.X), Math.Max(max.Y, p.Y), Math.Max(max.Z, p.Z));
		}

		public static bool GetIntersectPoint(ref Vector2 p0, ref Vector2 p1, ref Vector2 q0, ref Vector2 q1, out Vector2 rate, out Vector2 pos)
		{
			Vector2 p2 = p1 - p0;
			Vector2 q2 = q1 - q0;
			float num = CrossVector2(ref p2, ref q2);
			if (num == 0f)
			{
				rate = Vector2.Zero;
				pos = Vector2.Zero;
				return false;
			}
			Vector2 p3 = q0 - p0;
			float num2 = CrossVector2(ref p3, ref p2);
			float num3 = CrossVector2(ref p3, ref q2);
			float num4 = num2 / num;
			float num5 = num3 / num;
			if (num5 + 1E-05f < 0f || num5 - 1E-05f > 1f || num4 + 1E-05f < 0f || num4 - 1E-05f > 1f)
			{
				rate = Vector2.Zero;
				pos = Vector2.Zero;
				return false;
			}
			rate = new Vector2(num5, num4);
			pos = p0 + p2 * num5;
			return true;
		}

		public static Vector3 GetFaceNormal(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2)
		{
			Vector3 left = p1 - p0;
			Vector3 right = p2 - p0;
			Vector3 result = Vector3.Cross(left, right);
			result.Normalize();
			return result;
		}

		public static Vector3 GetFaceOrigin(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2)
		{
			return (p0 + p1 + p2) / 3f;
		}

		public static Vector3 GetTriangleIntersect(ref Vector3 org, ref Vector3 dir, ref Vector3 v0, ref Vector3 v1, ref Vector3 v2)
		{
			Vector3 result = new Vector3(-1f, 0f, 0f);
			Vector3 right = v1 - v0;
			Vector3 right2 = v2 - v0;
			Vector3 result2 = Vector3.Cross(dir, right2);
			float num = Vector3.Dot(right, result2);
			Vector3 result3;
			float num2;
			float num3;
			if (num > 0.001f)
			{
				Vector3 left = org - v0;
				num2 = Vector3.Dot(left, result2);
				if (num2 < 0f || num2 > num)
				{
					return result;
				}
				result3 = Vector3.Cross(left, right);
				num3 = Vector3.Dot(dir, result3);
				if (num3 < 0f || num2 + num3 > num)
				{
					return result;
				}
			}
			else
			{
				if (!(num < -0.001f))
				{
					return result;
				}
				Vector3 left = org - v0;
				num2 = Vector3.Dot(left, result2);
				if ((double)num2 > 0.0 || num2 < num)
				{
					return result;
				}
				result3 = Vector3.Cross(left, right);
				num3 = Vector3.Dot(dir, result3);
				if ((double)num3 > 0.0 || num2 + num3 < num)
				{
					return result;
				}
			}
			float num4 = 1f / num;
			float num5 = Vector3.Dot(right2, result3);
			num5 *= num4;
			num2 *= num4;
			num3 *= num4;
			result.X = num5;
			result.Y = num2;
			result.Z = num3;
			return result;
		}

		public static Vector3 GetLineCrossPoint(ref Vector3 p, ref Vector3 from, ref Vector3 dir, out float d)
		{
			Vector3 right = p - from;
			d = Vector3.Dot(dir, right);
			return dir * d + from;
		}

		public static Vector3 GetLineCrossPoint(ref Vector3 p, ref Vector3 from, ref Vector3 dir)
		{
			float d;
			return GetLineCrossPoint(ref p, ref from, ref dir, out d);
		}

		public static int GetNearAxis(Vector3 axis)
		{
			axis.Normalize();
			float num = Math.Abs(Vector3.Dot(axis, Vector3.UnitX));
			float num2 = Math.Abs(Vector3.Dot(axis, Vector3.UnitY));
			float num3 = Math.Abs(Vector3.Dot(axis, Vector3.UnitZ));
			int result = 2;
			if (num3 <= num && num2 <= num)
			{
				result = 0;
			}
			else if (num3 <= num2)
			{
				result = 1;
			}
			return result;
		}

		public static void CreateViewportMatrix(int width, int height, out Matrix m)
		{
			m = Matrix.Identity;
			float num = (float)width * 0.5f;
			float num2 = (float)height * 0.5f;
			m.M11 = num;
			m.M22 = 0f - num2;
			m.M41 = num;
			m.M42 = num2;
		}

		public static bool InBoxPosition(ref Vector3 pos, ref Rectangle rect)
		{
			int x = (int)(pos.X + 0.5f);
			int y = (int)(pos.Y + 0.5f);
			return rect.Contains(x, y);
		}

		public static bool InArcPosition(ref Vector3 pos, float cx, float cy, float r2, out float d2)
		{
			float num = pos.X - cx;
			float num2 = pos.Y - cy;
			d2 = num * num + num2 * num2;
			return d2 <= r2;
		}

		public static bool InArcPosition(ref Vector3 pos, float cx, float cy, float r2)
		{
			float d;
			return InArcPosition(ref pos, cx, cy, r2, out d);
		}

		public static void NormalizeRotateXYZ(ref Vector3 v)
		{
			if (v.X < -(float)Math.PI)
			{
				v.X += (float)Math.PI * 2f;
			}
			else if (v.X > (float)Math.PI)
			{
				v.X -= (float)Math.PI * 2f;
			}
			if (v.Y < -(float)Math.PI)
			{
				v.Y += (float)Math.PI * 2f;
			}
			else if (v.Y > (float)Math.PI)
			{
				v.Y -= (float)Math.PI * 2f;
			}
			if (v.Z < -(float)Math.PI)
			{
				v.Z += (float)Math.PI * 2f;
			}
			else if (v.Z > (float)Math.PI)
			{
				v.Z -= (float)Math.PI * 2f;
			}
		}

		public static void MatrixToEuler_ZXY0(ref Matrix m, out Vector3 p)
		{
			p = Vector3.Zero;
			if (m.M32 == 1f)
			{
				p.X = (float)Math.PI / 2f;
				p.Z = (float)Math.Atan2(m.M21, m.M11);
			}
			else if (m.M32 == -1f)
			{
				p.X = -(float)Math.PI / 2f;
				p.Z = (float)Math.Atan2(m.M21, m.M11);
			}
			else
			{
				p.X = 0f - (float)Math.Asin(m.M32);
				p.Y = 0f - (float)Math.Atan2(0f - m.M31, m.M33);
				p.Z = 0f - (float)Math.Atan2(0f - m.M12, m.M22);
			}
		}

		public static void MatrixToEuler_ZXY(ref Matrix m, out Vector3 p)
		{
			p = Vector3.Zero;
			p.X = 0f - (float)Math.Asin(m.M32);
			if (p.X == (float)Math.PI / 2f || p.X == -(float)Math.PI / 2f)
			{
				p.Y = (float)Math.Atan2(0f - m.M13, m.M11);
				return;
			}
			p.Y = (float)Math.Atan2(m.M31, m.M33);
			p.Z = (float)Math.Asin(m.M12 / (float)Math.Cos(p.X));
			if (m.M22 < 0f)
			{
				p.Z = (float)Math.PI - p.Z;
			}
		}

		public static void MatrixToEuler_XYZ(ref Matrix m, out Vector3 p)
		{
			p = Vector3.Zero;
			p.Y = 0f - (float)Math.Asin(m.M13);
			if (p.Y == (float)Math.PI / 2f || p.Y == -(float)Math.PI / 2f)
			{
				p.Z = (float)Math.Atan2(0f - m.M21, m.M22);
				return;
			}
			p.Z = (float)Math.Atan2(m.M12, m.M11);
			p.X = (float)Math.Asin(m.M23 / (float)Math.Cos(p.Y));
			if (m.M33 < 0f)
			{
				p.X = (float)Math.PI - p.X;
			}
		}

		public static void MatrixToEuler_YZX(ref Matrix m, out Vector3 p)
		{
			p = Vector3.Zero;
			p.Z = 0f - (float)Math.Asin(m.M21);
			if (p.Z == (float)Math.PI / 2f || p.Z == -(float)Math.PI / 2f)
			{
				p.X = (float)Math.Atan2(0f - m.M32, m.M33);
				return;
			}
			p.X = (float)Math.Atan2(m.M23, m.M22);
			p.Y = (float)Math.Asin(m.M31 / (float)Math.Cos(p.Z));
			if (m.M11 < 0f)
			{
				p.Y = (float)Math.PI - p.Y;
			}
		}

		public static void MatrixToEuler_ZXY_Lim2(ref Matrix m, out Vector3 p)
		{
			p = Vector3.Zero;
			p.X = 0f - (float)Math.Asin(m.M32);
			if (p.X == (float)Math.PI / 2f || p.X == -(float)Math.PI / 2f)
			{
				p.Y = (float)Math.Atan2(0f - m.M13, m.M11);
				return;
			}
			if (1.53588974f < p.X)
			{
				p.X = 1.53588974f;
			}
			else if (p.X < -1.53588974f)
			{
				p.X = -1.53588974f;
			}
			p.Y = (float)Math.Atan2(m.M31, m.M33);
			p.Z = (float)Math.Asin(m.M12 / (float)Math.Cos(p.X));
			if (m.M22 < 0f)
			{
				p.Z = (float)Math.PI - p.Z;
			}
		}

		public static void MatrixToEuler_XYZ_Lim2(ref Matrix m, out Vector3 p)
		{
			p = Vector3.Zero;
			p.Y = 0f - (float)Math.Asin(m.M13);
			if (p.Y == (float)Math.PI / 2f || p.Y == -(float)Math.PI / 2f)
			{
				p.Z = (float)Math.Atan2(0f - m.M21, m.M22);
				return;
			}
			if (1.53588974f < p.Y)
			{
				p.Y = 1.53588974f;
			}
			else if (p.Y < -1.53588974f)
			{
				p.Y = -1.53588974f;
			}
			p.Z = (float)Math.Atan2(m.M12, m.M11);
			p.X = (float)Math.Asin(m.M23 / (float)Math.Cos(p.Y));
			if (m.M33 < 0f)
			{
				p.X = (float)Math.PI - p.X;
			}
		}

		public static void MatrixToEuler_YZX_Lim2(ref Matrix m, out Vector3 p)
		{
			p = Vector3.Zero;
			p.Z = 0f - (float)Math.Asin(m.M21);
			if (p.Z == (float)Math.PI / 2f || p.Z == -(float)Math.PI / 2f)
			{
				p.X = (float)Math.Atan2(0f - m.M32, m.M33);
				return;
			}
			if (1.53588974f < p.Z)
			{
				p.Z = 1.53588974f;
			}
			else if (p.Z < -1.53588974f)
			{
				p.Z = -1.53588974f;
			}
			p.X = (float)Math.Atan2(m.M23, m.M22);
			p.Y = (float)Math.Asin(m.M31 / (float)Math.Cos(p.Z));
			if (m.M11 < 0f)
			{
				p.Y = (float)Math.PI - p.Y;
			}
		}

		public static bool NormalizeOrder(ref int n0, ref int n1, ref int n2)
		{
			bool result = true;
			if (n0 <= n1 && n0 <= n2)
			{
				if (n2 < n1)
				{
					int num = n1;
					n1 = n2;
					n2 = num;
				}
				else
				{
					result = false;
				}
			}
			else if (n1 <= n2)
			{
				int num2 = n0;
				int num3 = n1;
				int num4 = n2;
				if (n0 < n2)
				{
					n0 = num3;
					n1 = num2;
					n2 = num4;
				}
				else
				{
					n0 = num3;
					n1 = num4;
					n2 = num2;
				}
			}
			else
			{
				int num5 = n0;
				int num6 = n1;
				int num7 = n2;
				if (n0 < n1)
				{
					n0 = num7;
					n1 = num5;
					n2 = num6;
				}
				else
				{
					n0 = num7;
					n1 = num6;
					n2 = num5;
				}
			}
			return result;
		}
	}
}
