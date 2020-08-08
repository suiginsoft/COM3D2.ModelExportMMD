using System;
using System.IO;

namespace PmxLib
{
	public class PmxElementFormat : IPmxStreamIO, ICloneable
	{
		public enum StringEncType
		{
			UTF16,
			UTF8
		}

		private const int SizeBufLength = 8;

		public const int MaxUVACount = 4;

		public float Ver
		{
			get;
			set;
		}

		public StringEncType StringEnc
		{
			get;
			set;
		}

		public int UVACount
		{
			get;
			set;
		}

		public int VertexSize
		{
			get;
			set;
		}

		public int TexSize
		{
			get;
			set;
		}

		public int MaterialSize
		{
			get;
			set;
		}

		public int BoneSize
		{
			get;
			set;
		}

		public int MorphSize
		{
			get;
			set;
		}

		public int BodySize
		{
			get;
			set;
		}

		public PmxElementFormat(float ver = 2.1f)
		{
			this.Ver = ver;
			this.StringEnc = StringEncType.UTF16;
			this.UVACount = 0;
			this.VertexSize = 2;
			this.TexSize = 1;
			this.MaterialSize = 1;
			this.BoneSize = 2;
			this.MorphSize = 2;
			this.BodySize = 4;
		}

		public PmxElementFormat(PmxElementFormat f)
		{
			this.FromElementFormat(f);
		}

		public void FromElementFormat(PmxElementFormat f)
		{
			this.Ver = f.Ver;
			this.StringEnc = f.StringEnc;
			this.UVACount = f.UVACount;
			this.VertexSize = f.VertexSize;
			this.TexSize = f.TexSize;
			this.MaterialSize = f.MaterialSize;
			this.BoneSize = f.BoneSize;
			this.MorphSize = f.MorphSize;
			this.BodySize = f.BodySize;
		}

		public static int GetUnsignedBufSize(int count)
		{
			if (count < 256)
			{
				return 1;
			}
			if (count < 65536)
			{
				return 2;
			}
			return 4;
		}

		public static int GetSignedBufSize(int count)
		{
			if (count < 128)
			{
				return 1;
			}
			if (count < 32768)
			{
				return 2;
			}
			return 4;
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			int num = PmxStreamHelper.ReadElement_Int32(s, 1, true);
			byte[] array = new byte[num];
			s.Read(array, 0, array.Length);
			int num2 = 0;
			if (this.Ver <= 1f)
			{
				this.VertexSize = array[num2++];
				this.BoneSize = array[num2++];
				this.MorphSize = array[num2++];
				this.MaterialSize = array[num2++];
				this.BodySize = array[num2++];
			}
			else
			{
				this.StringEnc = (StringEncType)array[num2++];
				this.UVACount = array[num2++];
				this.VertexSize = array[num2++];
				this.TexSize = array[num2++];
				this.MaterialSize = array[num2++];
				this.BoneSize = array[num2++];
				this.MorphSize = array[num2++];
				this.BodySize = array[num2++];
			}
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			byte[] array = new byte[8];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 0;
			}
			int num = 0;
			if (this.Ver <= 1f)
			{
				array[num++] = (byte)this.VertexSize;
				array[num++] = (byte)this.BoneSize;
				array[num++] = (byte)this.MorphSize;
				array[num++] = (byte)this.MaterialSize;
				array[num++] = (byte)this.BodySize;
			}
			else
			{
				array[num++] = (byte)this.StringEnc;
				array[num++] = (byte)this.UVACount;
				array[num++] = (byte)this.VertexSize;
				array[num++] = (byte)this.TexSize;
				array[num++] = (byte)this.MaterialSize;
				array[num++] = (byte)this.BoneSize;
				array[num++] = (byte)this.MorphSize;
				array[num++] = (byte)this.BodySize;
			}
			PmxStreamHelper.WriteElement_Int32(s, array.Length, 1, true);
			s.Write(array, 0, array.Length);
		}

		object ICloneable.Clone()
		{
			return new PmxElementFormat(this);
		}

		public PmxElementFormat Clone()
		{
			return new PmxElementFormat(this);
		}
	}
}
