using System;
using System.IO;

namespace PmxLib
{
	internal class PmxMaterialMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public enum OpType
		{
			Mul,
			Add
		}

		public struct MorphData
		{
			public Vector4 Diffuse;

			public Vector4 Specular;

			public Vector3 Ambient;

			public float EdgeSize;

			public Vector4 EdgeColor;

			public Vector4 Tex;

			public Vector4 Sphere;

			public Vector4 Toon;

			public void Set(float v)
			{
				Diffuse = new Vector4(v, v, v, v);
				Specular = new Vector4(v, v, v, v);
				Ambient = new Vector3(v, v, v);
				EdgeSize = v;
				EdgeColor = new Vector4(v, v, v, v);
				Tex = new Vector4(v, v, v, v);
				Sphere = new Vector4(v, v, v, v);
				Toon = new Vector4(v, v, v, v);
			}

			public void Clear(OpType op)
			{
				switch (op)
				{
				case OpType.Mul:
					Set(1f);
					Diffuse = new Vector4(1f, 1f, 1f, 1f);
					Specular = new Vector4(1f, 1f, 1f, 1f);
					Ambient = new Vector3(1f, 1f, 1f);
					EdgeSize = 1f;
					EdgeColor = new Vector4(1f, 1f, 1f, 1f);
					Tex = new Vector4(1f, 1f, 1f, 1f);
					Sphere = new Vector4(1f, 1f, 1f, 1f);
					Toon = new Vector4(1f, 1f, 1f, 1f);
					break;
				case OpType.Add:
					Diffuse = Vector4.Zero;
					Specular = Vector4.Zero;
					Ambient = Vector3.Zero;
					EdgeSize = 0f;
					EdgeColor = Vector4.Zero;
					Tex = Vector4.Zero;
					Sphere = Vector4.Zero;
					Toon = Vector4.Zero;
					break;
				}
			}

			private Vector4 mul_v4(Vector4 v0, Vector4 v1)
			{
				return new Vector4(v0.X * v1.X, v0.Y * v1.Y, v0.Z * v1.Z, v0.W * v1.W);
			}

			private Vector3 mul_v3(Vector3 v0, Vector3 v1)
			{
				return new Vector3(v0.X * v1.X, v0.Y * v1.Y, v0.Z * v1.Z);
			}

			public void Mul(MorphData d)
			{
				Diffuse = mul_v4(Diffuse, d.Diffuse);
				Specular = mul_v4(Specular, d.Specular);
				Ambient = mul_v3(Ambient, d.Ambient);
				EdgeSize *= d.EdgeSize;
				EdgeColor = mul_v4(EdgeColor, d.EdgeColor);
				Tex = mul_v4(Tex, d.Tex);
				Sphere = mul_v4(Sphere, d.Sphere);
				Toon = mul_v4(Toon, d.Toon);
			}

			public void Mul(float v)
			{
				Diffuse *= v;
				Specular *= v;
				Ambient *= v;
				EdgeSize *= v;
				EdgeColor *= v;
				Tex *= v;
				Sphere *= v;
				Toon *= v;
			}

			public void Add(MorphData d)
			{
				Diffuse += d.Diffuse;
				Specular += d.Specular;
				Ambient += d.Ambient;
				EdgeSize += d.EdgeSize;
				EdgeColor += d.EdgeColor;
				Tex += d.Tex;
				Sphere += d.Sphere;
				Toon += d.Toon;
			}

			public static MorphData Inter(MorphData a, MorphData b, float val)
			{
				if (val == 0f)
				{
					return a;
				}
				if (val == 1f)
				{
					return b;
				}
				MorphData result = a;
				result.Mul(1f - val);
				MorphData d = b;
				d.Mul(val);
				result.Add(d);
				return result;
			}
		}

		public int Index;

		public static PmxMaterial RefAllMaterial = new PmxMaterial();

		public OpType Op;

		public MorphData Data;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.MaterialMorph;

		public override int BaseIndex
		{
			get
			{
				return Index;
			}
			set
			{
				Index = value;
			}
		}

		public PmxMaterial RefMaterial
		{
			get;
			set;
		}

		public void ClearData()
		{
			Data.Clear(Op);
		}

		public PmxMaterialMorph()
		{
			Op = OpType.Mul;
			ClearData();
		}

		public PmxMaterialMorph(int index, MorphData d)
			: this()
		{
			Index = index;
			Data = d;
		}

		public PmxMaterialMorph(int index, MorphData d, OpType op)
			: this()
		{
			Index = index;
			Data = d;
			Op = op;
		}

		public PmxMaterialMorph(PmxMaterialMorph sv)
			: this()
		{
			FromPmxMaterialMorph(sv);
		}

		public void FromPmxMaterialMorph(PmxMaterialMorph sv)
		{
			FromPmxBaseMorph(sv);
			Op = sv.Op;
			Data = sv.Data;
		}

		public override void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.FromStreamEx(s, f);
			Index = PmxStreamHelper.ReadElement_Int32(s, f.MaterialSize);
			Op = (OpType)s.ReadByte();
			Data.Diffuse = V4_BytesConvert.FromStream(s);
			Data.Specular = V4_BytesConvert.FromStream(s);
			Data.Ambient = V3_BytesConvert.FromStream(s);
			Data.EdgeColor = V4_BytesConvert.FromStream(s);
			Data.EdgeSize = PmxStreamHelper.ReadElement_Float(s);
			Data.Tex = V4_BytesConvert.FromStream(s);
			Data.Sphere = V4_BytesConvert.FromStream(s);
			Data.Toon = V4_BytesConvert.FromStream(s);
		}

		public override void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			base.ToStreamEx(s, f);
			PmxStreamHelper.WriteElement_Int32(s, Index, f.MaterialSize);
			s.WriteByte((byte)Op);
			V4_BytesConvert.ToStream(s, Data.Diffuse);
			V4_BytesConvert.ToStream(s, Data.Specular);
			V3_BytesConvert.ToStream(s, Data.Ambient);
			V4_BytesConvert.ToStream(s, Data.EdgeColor);
			PmxStreamHelper.WriteElement_Float(s, Data.EdgeSize);
			V4_BytesConvert.ToStream(s, Data.Tex);
			V4_BytesConvert.ToStream(s, Data.Sphere);
			V4_BytesConvert.ToStream(s, Data.Toon);
		}

		object ICloneable.Clone()
		{
			return new PmxMaterialMorph(this);
		}

		public override PmxBaseMorph Clone()
		{
			return new PmxMaterialMorph(this);
		}
	}
}
