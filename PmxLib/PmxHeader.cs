using System;
using System.IO;
using System.Text;

namespace PmxLib
{
	public class PmxHeader : IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public const float LastVer = 2.1f;

		public static string PmxKey_v1 = "Pmx ";

		public static string PmxKey = "PMX ";

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.Header;
			}
		}

		public float Ver
		{
			get
			{
				return this.ElementFormat.Ver;
			}
			set
			{
				this.ElementFormat.Ver = value;
			}
		}

		public PmxElementFormat ElementFormat
		{
			get;
			private set;
		}

		public PmxHeader(float ver = 2.1f)
		{
			this.ElementFormat = new PmxElementFormat(ver);
		}

		public PmxHeader(PmxHeader h)
		{
			this.FromHeader(h);
		}

		public void FromHeader(PmxHeader h)
		{
			this.ElementFormat = h.ElementFormat.Clone();
		}

		public void FromElementFormat(PmxElementFormat f)
		{
			this.ElementFormat = f;
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			byte[] array = new byte[4];
			s.Read(array, 0, array.Length);
			string @string = Encoding.ASCII.GetString(array);
			if (@string.Equals(PmxHeader.PmxKey_v1))
			{
				this.Ver = 1f;
				array = new byte[4];
				s.Read(array, 0, array.Length);
			}
			else
			{
				if (!@string.Equals(PmxHeader.PmxKey))
				{
					throw new Exception("ファイル形式が異なります.");
				}
				array = new byte[4];
				s.Read(array, 0, array.Length);
				this.Ver = BitConverter.ToSingle(array, 0);
			}
			if (this.Ver > 2.1f)
			{
				throw new Exception("未対応のverです.");
			}
			this.ElementFormat = new PmxElementFormat(this.Ver);
			this.ElementFormat.FromStreamEx(s, null);
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			if (f == null)
			{
				f = this.ElementFormat;
			}
			byte[] array = new byte[4];
			array = ((!(f.Ver <= 1f)) ? Encoding.ASCII.GetBytes(PmxHeader.PmxKey) : Encoding.ASCII.GetBytes(PmxHeader.PmxKey_v1));
			s.Write(array, 0, array.Length);
			array = BitConverter.GetBytes(this.Ver);
			s.Write(array, 0, array.Length);
			f.ToStreamEx(s, null);
		}

		object ICloneable.Clone()
		{
			return new PmxHeader(this);
		}

		public PmxHeader Clone()
		{
			return new PmxHeader(this);
		}
	}
}
