using System;
using System.IO;

namespace PmxLib
{
	internal class PmxElementFormat : IPmxStreamIO, ICloneable
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

		public bool WithID
		{
			get;
			set;
		}

		public PmxElementFormat(float ver = 2.1f)
		{
			Ver = ver;
			StringEnc = StringEncType.UTF16;
			UVACount = 0;
			VertexSize = 2;
			TexSize = 1;
			MaterialSize = 1;
			BoneSize = 2;
			MorphSize = 2;
			BodySize = 4;
			WithID = false;
		}

		public PmxElementFormat(PmxElementFormat f)
		{
			FromElementFormat(f);
		}

		public void FromElementFormat(PmxElementFormat f)
		{
			Ver = f.Ver;
			StringEnc = f.StringEnc;
			UVACount = f.UVACount;
			VertexSize = f.VertexSize;
			TexSize = f.TexSize;
			MaterialSize = f.MaterialSize;
			BoneSize = f.BoneSize;
			MorphSize = f.MorphSize;
			BodySize = f.BodySize;
			WithID = f.WithID;
		}

		public void InitializeMax()
		{
			Ver = 2.1f;
			StringEnc = StringEncType.UTF16;
			UVACount = 4;
			VertexSize = 4;
			TexSize = 4;
			MaterialSize = 4;
			BoneSize = 4;
			MorphSize = 4;
			BodySize = 4;
		}

		public static int GetUnsignedBufSize(int count)
		{
			if (count < 256)
				return 1;
			if (count < 65536)
				return 2;
			return 4;
		}

		public static int GetSignedBufSize(int count)
		{
			if (count < 128)
				return 1;
			if (count < 32768)
				return 2;
			return 4;
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			byte[] array = new byte[PmxStreamHelper.ReadElement_Int32(s, 1)];
			s.Read(array, 0, array.Length);
			int num = 0;
			if (Ver <= 1f)
			{
				VertexSize = array[num++];
				BoneSize = array[num++];
				MorphSize = array[num++];
				MaterialSize = array[num++];
				BodySize = array[num++];
			}
			else
			{
				StringEnc = (StringEncType)array[num++];
				UVACount = array[num++];
				VertexSize = array[num++];
				TexSize = array[num++];
				MaterialSize = array[num++];
				BoneSize = array[num++];
				MorphSize = array[num++];
				BodySize = array[num++];
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
			if (Ver <= 1f)
			{
				array[num++] = (byte)VertexSize;
				array[num++] = (byte)BoneSize;
				array[num++] = (byte)MorphSize;
				array[num++] = (byte)MaterialSize;
				array[num++] = (byte)BodySize;
			}
			else
			{
				array[num++] = (byte)StringEnc;
				array[num++] = (byte)UVACount;
				array[num++] = (byte)VertexSize;
				array[num++] = (byte)TexSize;
				array[num++] = (byte)MaterialSize;
				array[num++] = (byte)BoneSize;
				array[num++] = (byte)MorphSize;
				array[num++] = (byte)BodySize;
			}
			PmxStreamHelper.WriteElement_Int32(s, array.Length, 1);
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
