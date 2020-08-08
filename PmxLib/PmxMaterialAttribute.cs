using System;

namespace PmxLib
{
	public class PmxMaterialAttribute : ICloneable
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
			this.Clear();
		}

		public PmxMaterialAttribute(string text)
			: this()
		{
			this.SetFromText(text);
		}

		public PmxMaterialAttribute(PmxMaterialAttribute att)
		{
			this.BumpMapTexture = att.BumpMapTexture;
			this.NormalMapTexture = att.NormalMapTexture;
			this.CubeMapTexture = att.CubeMapTexture;
			this.BumpMapUV = att.BumpMapUV;
			this.NormalMapUV = att.NormalMapUV;
			this.CubeMapUV = att.CubeMapUV;
		}

		public void Clear()
		{
			this.BumpMapTexture = null;
			this.NormalMapTexture = null;
			this.CubeMapTexture = null;
			this.BumpMapUV = UVTarget.UV;
			this.NormalMapUV = UVTarget.UV;
			this.CubeMapUV = UVTarget.UV;
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
			this.Clear();
			if (!string.IsNullOrEmpty(text))
			{
				string[] tag = PmxTag.GetTag("BumpMap", text);
				if (tag != null && tag.Length > 0)
				{
					this.BumpMapTexture = tag[0];
				}
				string[] tag2 = PmxTag.GetTag("BumpMapUV", text);
				if (tag2 != null && tag2.Length > 0)
				{
					this.BumpMapUV = PmxMaterialAttribute.TextToUVTarget(tag2[0]);
				}
				string[] tag3 = PmxTag.GetTag("NormalMap", text);
				if (tag3 != null && tag3.Length > 0)
				{
					this.NormalMapTexture = tag3[0];
				}
				string[] tag4 = PmxTag.GetTag("NormalMapUV", text);
				if (tag4 != null && tag4.Length > 0)
				{
					this.NormalMapUV = PmxMaterialAttribute.TextToUVTarget(tag4[0]);
				}
				string[] tag5 = PmxTag.GetTag("CubeMap", text);
				if (tag5 != null && tag5.Length > 0)
				{
					this.CubeMapTexture = tag5[0];
				}
				string[] tag6 = PmxTag.GetTag("CubeMapUV", text);
				if (tag6 != null && tag6.Length > 0)
				{
					this.CubeMapUV = PmxMaterialAttribute.TextToUVTarget(tag6[0]);
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
