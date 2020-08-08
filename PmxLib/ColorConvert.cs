using System;
using System.Drawing;

namespace PmxLib
{
	internal static class ColorConvert
	{
		public static void RGBtoHSV(Color c, out int h, out int s, out int v)
		{
			int r = c.R;
			int g = c.G;
			int b = c.B;
			int num = Math.Max(r, Math.Max(g, b));
			int num2 = Math.Min(r, Math.Min(g, b));
			v = 100 * num / 255;
			int num3 = num - num2;
			if (num == 0 || num3 == 0)
			{
				s = 0;
				h = 0;
				return;
			}
			s = 100 * (num3 * 255 / num) / 255;
			if (r == num)
			{
				h = 60 * (g - b) / num3;
			}
			else if (g == num)
			{
				h = 120 + 60 * (b - r) / num3;
			}
			else
			{
				h = 240 + 60 * (r - g) / num3;
			}
			if (h < 0)
			{
				h += 360;
			}
		}

		public static Color HSVtoRGB(int h, int s, int v)
		{
			int num = 255 * v / 100;
			if (s == 0)
			{
				return Color.FromArgb(num, num, num);
			}
			int num2 = h / 60 % 6;
			int num3 = (h - num2 * 60) % 60;
			int num4 = 255 * v * (100 - s) / 10000;
			int num5 = 255 * v * (6000 - s * num3) / 600000;
			int num6 = 255 * v * (6000 - s * (60 - num3)) / 600000;
			int red;
			int green;
			int blue;
			switch (num2)
			{
			case 0:
				red = num;
				green = num6;
				blue = num4;
				break;
			case 1:
				red = num5;
				green = num;
				blue = num4;
				break;
			case 2:
				red = num4;
				green = num;
				blue = num6;
				break;
			case 3:
				red = num4;
				green = num5;
				blue = num;
				break;
			case 4:
				red = num6;
				green = num4;
				blue = num;
				break;
			case 5:
				red = num;
				green = num4;
				blue = num5;
				break;
			default:
				red = 0;
				green = 0;
				blue = 0;
				break;
			}
			return Color.FromArgb(red, green, blue);
		}

		public static Color V4toColor(Vector4 c)
		{
			return Color.FromArgb((int)(c.W * 255f), (int)(c.X * 255f), (int)(c.Y * 255f), (int)(c.Z * 255f));
		}

		public static Color V3toColor(Vector3 c)
		{
			return Color.FromArgb((int)(c.X * 255f), (int)(c.Y * 255f), (int)(c.Z * 255f));
		}

		public static void ToFloatValue(Color c, out float r, out float g, out float b, out float a)
		{
			r = (float)(int)c.R / 255f;
			g = (float)(int)c.G / 255f;
			b = (float)(int)c.B / 255f;
			a = (float)(int)c.A / 255f;
		}

		public static void ToFloatValue(Color c, out float r, out float g, out float b)
		{
			r = (float)(int)c.R / 255f;
			g = (float)(int)c.G / 255f;
			b = (float)(int)c.B / 255f;
		}
	}
}
