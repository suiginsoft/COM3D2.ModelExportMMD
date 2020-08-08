using System;

namespace PmxLib
{
	internal class PmxMaterialAttribute : ICloneable
	{
		public enum UVTarget
		{
			UV,
			UVA1xy,
			UVA1zw,
			UVA2xy,
			UVA2zw,
			UVA3xy,
			UVA3zw,
			UVA4xy,
			UVA4zw
		}

		public const string TAG_BumpMap = "BumpMap";

		public const string TAG_NormalMap = "NormalMap";

		public const string TAG_CubeMap = "CubeMap";

		public const string TAG_BumpMapUV = "BumpMapUV";

		public const string TAG_NormalMapUV = "NormalMapUV";

		public const string TAG_CubeMapUV = "CubeMapUV";

		public const string UVTarget_UV = "uv";

		public const string UVTarget_UVA1xy = "uva1xy";

		public const string UVTarget_UVA1zw = "uva1zw";

		public const string UVTarget_UVA2xy = "uva2xy";

		public const string UVTarget_UVA2zw = "uva2zw";

		public const string UVTarget_UVA3xy = "uva3xy";

		public const string UVTarget_UVA3zw = "uva3zw";

		public const string UVTarget_UVA4xy = "uva4xy";

		public const string UVTarget_UVA4zw = "uva4zw";

		public string BumpMapTexture
		{
			get;
			private set;
		}

		public UVTarget BumpMapUV
		{
			get;
			private set;
		}

		public string NormalMapTexture
		{
			get;
			private set;
		}

		public UVTarget NormalMapUV
		{
			get;
			private set;
		}

		public string CubeMapTexture
		{
			get;
			private set;
		}

		public UVTarget CubeMapUV
		{
			get;
			private set;
		}

		public PmxMaterialAttribute()
		{
			Clear();
		}

		public PmxMaterialAttribute(string text)
			: this()
		{
			SetFromText(text);
		}

		public PmxMaterialAttribute(PmxMaterialAttribute att)
		{
			BumpMapTexture = att.BumpMapTexture;
			NormalMapTexture = att.NormalMapTexture;
			CubeMapTexture = att.CubeMapTexture;
			BumpMapUV = att.BumpMapUV;
			NormalMapUV = att.NormalMapUV;
			CubeMapUV = att.CubeMapUV;
		}

		public void Clear()
		{
			BumpMapTexture = null;
			NormalMapTexture = null;
			CubeMapTexture = null;
			BumpMapUV = UVTarget.UV;
			NormalMapUV = UVTarget.UV;
			CubeMapUV = UVTarget.UV;
		}

		private static UVTarget TextToUVTarget(string text)
		{
			UVTarget result = UVTarget.UV;
			switch (text.ToLower())
			{
			case "uva1xy":
				result = UVTarget.UVA1xy;
				break;
			case "uva1zw":
				result = UVTarget.UVA1zw;
				break;
			case "uva2xy":
				result = UVTarget.UVA2xy;
				break;
			case "uva2zw":
				result = UVTarget.UVA2zw;
				break;
			case "uva3xy":
				result = UVTarget.UVA3xy;
				break;
			case "uva3zw":
				result = UVTarget.UVA3zw;
				break;
			case "uva4xy":
				result = UVTarget.UVA4xy;
				break;
			case "uva4zw":
				result = UVTarget.UVA4zw;
				break;
			}
			return result;
		}

		public void SetFromText(string text)
		{
			Clear();
			if (!string.IsNullOrEmpty(text))
			{
				string[] tag = PmxTag.GetTag("BumpMap", text);
				if (tag != null && tag.Length != 0)
				{
					BumpMapTexture = tag[0];
				}
				string[] tag2 = PmxTag.GetTag("BumpMapUV", text);
				if (tag2 != null && tag2.Length != 0)
				{
					BumpMapUV = TextToUVTarget(tag2[0]);
				}
				string[] tag3 = PmxTag.GetTag("NormalMap", text);
				if (tag3 != null && tag3.Length != 0)
				{
					NormalMapTexture = tag3[0];
				}
				string[] tag4 = PmxTag.GetTag("NormalMapUV", text);
				if (tag4 != null && tag4.Length != 0)
				{
					NormalMapUV = TextToUVTarget(tag4[0]);
				}
				string[] tag5 = PmxTag.GetTag("CubeMap", text);
				if (tag5 != null && tag5.Length != 0)
				{
					CubeMapTexture = tag5[0];
				}
				string[] tag6 = PmxTag.GetTag("CubeMapUV", text);
				if (tag6 != null && tag6.Length != 0)
				{
					CubeMapUV = TextToUVTarget(tag6[0]);
				}
			}
		}

		object ICloneable.Clone()
		{
			return new PmxMaterialAttribute(this);
		}

		public PmxMaterialAttribute Clone()
		{
			return new PmxMaterialAttribute(this);
		}
	}
}
