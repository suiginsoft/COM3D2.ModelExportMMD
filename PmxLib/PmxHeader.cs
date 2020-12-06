using System;
using System.IO;
using System.Text;

namespace PmxLib
{
	internal class PmxHeader : IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public class LoadException : Exception
		{
			public LoadException()
			{
			}

			public LoadException(string message)
				: base(message)
			{
			}
		}

		public static string PmxKey_v1 = "Pmx ";

		public static string PmxKey = "PMX ";

		public static string BadPmxKey = "PMX";

		public const float LastVer = 2.1f;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.Header;

		public float Ver
		{
			get
			{
				return ElementFormat.Ver;
			}
			set
			{
				ElementFormat.Ver = value;
			}
		}

		public PmxElementFormat ElementFormat
		{
			get;
			private set;
		}

		public bool BadKey
		{
			get;
			private set;
		}

		public PmxHeader()
		{
			ElementFormat = new PmxElementFormat();
		}

		public PmxHeader(float ver)
		{
			ElementFormat = new PmxElementFormat(ver);
		}

		public PmxHeader(PmxHeader h)
		{
			FromHeader(h);
		}

		public void FromHeader(PmxHeader h)
		{
			ElementFormat = h.ElementFormat.Clone();
			BadKey = h.BadKey;
		}

		public void FromElementFormat(PmxElementFormat f)
		{
			ElementFormat = f;
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			BadKey = false;
			byte[] array = new byte[4];
			s.Read(array, 0, array.Length);
			string @string = Encoding.ASCII.GetString(array);
			if (@string.Equals(PmxKey_v1))
			{
				Ver = 1f;
				array = new byte[4];
				s.Read(array, 0, array.Length);
			}
			else if (@string.Equals(PmxKey))
			{
				array = new byte[4];
				s.Read(array, 0, array.Length);
				Ver = BitConverter.ToSingle(array, 0);
			}
			else
			{
				if (!@string.Substring(0, 3).Equals(BadPmxKey))
				{
					throw new LoadException("ファイル形式が異なります.");
				}
				array = new byte[4];
				s.Read(array, 0, array.Length);
				Ver = BitConverter.ToSingle(array, 0);
				BadKey = true;
			}
			if (Ver > 2.1f)
			{
				throw new LoadException("未対応のverです.");
			}
			ElementFormat = new PmxElementFormat(Ver);
			ElementFormat.FromStreamEx(s);
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			if (f == null)
			{
				f = ElementFormat;
			}
			byte[] array = new byte[4];
			array = f.Ver > 1f ? Encoding.ASCII.GetBytes(PmxKey) : Encoding.ASCII.GetBytes(PmxKey_v1);
			s.Write(array, 0, array.Length);
			array = BitConverter.GetBytes(Ver);
			s.Write(array, 0, array.Length);
			f.ToStreamEx(s);
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
