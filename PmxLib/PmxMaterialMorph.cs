using System;
using System.IO;

namespace PmxLib
{
	public class PmxMaterialMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
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
				this.Diffuse = new Vector4(v, v, v, v);
				this.Specular = new Vector4(v, v, v, v);
				this.Ambient = new Vector3(v, v, v);
				this.EdgeSize = v;
				this.EdgeColor = new Vector4(v, v, v, v);
				this.Tex = new Vector4(v, v, v, v);
				this.Sphere = new Vector4(v, v, v, v);
				this.Toon = new Vector4(v, v, v, v);
			}

			public void Clear(OpType op)
			{
				switch (op)
				{
				case OpType.Mul:
					this.Set(1f);
					this.Diffuse = new Vector4(1f, 1f, 1f, 1f);
					this.Specular = new Vector4(1f, 1f, 1f, 1f);
					this.Ambient = new Vector3(1f, 1f, 1f);
					this.EdgeSize = 1f;
					this.EdgeColor = new Vector4(1f, 1f, 1f, 1f);
					this.Tex = new Vector4(1f, 1f, 1f, 1f);
					this.Sphere = new Vector4(1f, 1f, 1f, 1f);
					this.Toon = new Vector4(1f, 1f, 1f, 1f);
					break;
				case OpType.Add:
					this.Diffuse = Vector4.zero;
					this.Specular = Vector4.zero;
					this.Ambient = Vector3.zero;
					this.EdgeSize = 0f;
					this.EdgeColor = Vector4.zero;
					this.Tex = Vector4.zero;
					this.Sphere = Vector4.zero;
					this.Toon = Vector4.zero;
					break;
				}
			}

			private Vector4 mul_v4(Vector4 v0, Vector4 v1)
			{
				return new Vector4(v0.x * v1.x, v0.y * v1.y, v0.z * v1.z, v0.w * v1.w);
			}

			private Vector3 mul_v3(Vector3 v0, Vector3 v1)
			{
				return new Vector3(v0.x * v1.x, v0.y * v1.y, v0.z * v1.z);
			}

			public void Mul(MorphData d)
			{
				this.Diffuse = this.mul_v4(this.Diffuse, d.Diffuse);
				this.Specular = this.mul_v4(this.Specular, d.Specular);
				this.Ambient = this.mul_v3(this.Ambient, d.Ambient);
				this.EdgeSize *= d.EdgeSize;
				this.EdgeColor = this.mul_v4(this.EdgeColor, d.EdgeColor);
				this.Tex = this.mul_v4(this.Tex, d.Tex);
				this.Sphere = this.mul_v4(this.Sphere, d.Sphere);
				this.Toon = this.mul_v4(this.Toon, d.Toon);
			}

			public void Mul(float v)
			{
				this.Diffuse *= v;
				this.Specular *= v;
				this.Ambient *= v;
				this.EdgeSize *= v;
				this.EdgeColor *= v;
				this.Tex *= v;
				this.Sphere *= v;
				this.Toon *= v;
			}

			public void Add(MorphData d)
			{
				this.Diffuse += d.Diffuse;
				this.Specular += d.Specular;
				this.Ambient += d.Ambient;
				this.EdgeSize += d.EdgeSize;
				this.EdgeColor += d.EdgeColor;
				this.Tex += d.Tex;
				this.Sphere += d.Sphere;
				this.Toon += d.Toon;
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

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.MaterialMorph;
			}
		}

		public override int BaseIndex
		{
			get
			{
				return this.Index;
			}
			set
			{
				this.Index = value;
			}
		}

		public PmxMaterial RefMaterial
		{
			get;
			set;
		}

		public void ClearData()
		{
			this.Data.Clear(this.Op);
		}

		public PmxMaterialMorph()
		{
			this.Op = OpType.Mul;
			this.ClearData();
		}

		public PmxMaterialMorph(int index, MorphData d)
			: this()
		{
			this.Index = index;
			this.Data = d;
		}

		public PmxMaterialMorph(PmxMaterialMorph sv)
			: this()
		{
			this.FromPmxMaterialMorph(sv);
		}

		public void FromPmxMaterialMorph(PmxMaterialMorph sv)
		{
			this.Index = sv.Index;
			this.Op = sv.Op;
			this.Data = sv.Data;
		}

		public override void FromStreamEx(Stream s, PmxElementFormat size = null)
		{
			this.Index = PmxStreamHelper.ReadElement_Int32(s, size.MaterialSize, true);
			this.Op = (OpType)s.ReadByte();
			this.Data.Diffuse = V4_BytesConvert.FromStream(s);
			this.Data.Specular = V4_BytesConvert.FromStream(s);
			this.Data.Ambient = V3_BytesConvert.FromStream(s);
			this.Data.EdgeColor = V4_BytesConvert.FromStream(s);
			this.Data.EdgeSize = PmxStreamHelper.ReadElement_Float(s);
			this.Data.Tex = V4_BytesConvert.FromStream(s);
			this.Data.Sphere = V4_BytesConvert.FromStream(s);
			this.Data.Toon = V4_BytesConvert.FromStream(s);
		}

		public override void ToStreamEx(Stream s, PmxElementFormat size = null)
		{
			PmxStreamHelper.WriteElement_Int32(s, this.Index, size.MaterialSize, true);
			s.WriteByte((byte)this.Op);
			V4_BytesConvert.ToStream(s, this.Data.Diffuse);
			V4_BytesConvert.ToStream(s, this.Data.Specular);
			V3_BytesConvert.ToStream(s, this.Data.Ambient);
			V4_BytesConvert.ToStream(s, this.Data.EdgeColor);
			PmxStreamHelper.WriteElement_Float(s, this.Data.EdgeSize);
			V4_BytesConvert.ToStream(s, this.Data.Tex);
			V4_BytesConvert.ToStream(s, this.Data.Sphere);
			V4_BytesConvert.ToStream(s, this.Data.Toon);
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
