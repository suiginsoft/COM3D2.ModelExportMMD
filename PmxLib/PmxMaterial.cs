using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	internal class PmxMaterial : PmxIDObject, IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
	{
		[Flags]
		public enum MaterialFlags
		{
			None = 0x0,
			DrawBoth = 0x1,
			Shadow = 0x2,
			SelfShadowMap = 0x4,
			SelfShadow = 0x8,
			Edge = 0x10,
			VertexColor = 0x20,
			PointDraw = 0x40,
			LineDraw = 0x80
		}

		public enum ExDrawMode
		{
			F1,
			F2,
			F3
		}

		public enum SphereModeType
		{
			None,
			Mul,
			Add,
			SubTex
		}

		public Vector4 Diffuse;

		public Vector3 Specular;

		public float Power;

		public Vector3 Ambient;

		public ExDrawMode ExDraw;

		public MaterialFlags Flags;

		public Vector4 EdgeColor;

		public float EdgeSize;

		public int FaceCount;

		public SphereModeType SphereMode;

		public PmxMaterialMorph.MorphData OffsetMul;

		public PmxMaterialMorph.MorphData OffsetAdd;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.Material;

		public string Name
		{
			get;
			set;
		}

		public string NameE
		{
			get;
			set;
		}

		public string Tex
		{
			get;
			set;
		}

		public string Sphere
		{
			get;
			set;
		}

		public string Toon
		{
			get;
			set;
		}

		public string Memo
		{
			get;
			set;
		}

		public PmxMaterialAttribute Attribute
		{
			get;
			private set;
		}

		public string NXName
		{
			get
			{
				return Name;
			}
			set
			{
				Name = value;
			}
		}

		public void ClearFlags()
		{
			Flags = MaterialFlags.None;
		}

		public bool GetFlag(MaterialFlags f)
		{
			return (f & Flags) == f;
		}

		public void SetFlag(MaterialFlags f, bool val)
		{
			if (val)
			{
				Flags |= f;
			}
			else
			{
				Flags &= ~f;
			}
		}

		public void ClearOffset()
		{
			OffsetMul.Clear(PmxMaterialMorph.OpType.Mul);
			OffsetAdd.Clear(PmxMaterialMorph.OpType.Add);
		}

		public void UpdateAttributeFromMemo()
		{
			Attribute.SetFromText(Memo);
		}

		public PmxMaterial()
		{
			Name = "";
			NameE = "";
			Diffuse = new Vector4(0f, 0f, 0f, 1f);
			Specular = new Vector3(0f, 0f, 0f);
			Power = 0f;
			Ambient = new Vector3(0f, 0f, 0f);
			ClearFlags();
			EdgeColor = new Vector4(0f, 0f, 0f, 1f);
			EdgeSize = 1f;
			Tex = "";
			Sphere = "";
			SphereMode = SphereModeType.Mul;
			Toon = "";
			Memo = "";
			OffsetMul = default(PmxMaterialMorph.MorphData);
			OffsetAdd = default(PmxMaterialMorph.MorphData);
			ClearOffset();
			ExDraw = ExDrawMode.F3;
			Attribute = new PmxMaterialAttribute();
		}

		public PmxMaterial(PmxMaterial m, bool nonStr = false)
			: this()
		{
			FromPmxMaterial(m, nonStr);
		}

		public void FromPmxMaterial(PmxMaterial m, bool nonStr = false)
		{
			Diffuse = m.Diffuse;
			Specular = m.Specular;
			Power = m.Power;
			Ambient = m.Ambient;
			Flags = m.Flags;
			EdgeColor = m.EdgeColor;
			EdgeSize = m.EdgeSize;
			SphereMode = m.SphereMode;
			FaceCount = m.FaceCount;
			ExDraw = m.ExDraw;
			if (!nonStr)
			{
				Name = m.Name;
				NameE = m.NameE;
				Tex = m.Tex;
				Sphere = m.Sphere;
				Toon = m.Toon;
				Memo = m.Memo;
			}
			Attribute = m.Attribute;
			FromID(m);
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			Name = PmxStreamHelper.ReadString(s, f);
			NameE = PmxStreamHelper.ReadString(s, f);
			Diffuse = V4_BytesConvert.FromStream(s);
			Specular = V3_BytesConvert.FromStream(s);
			Power = PmxStreamHelper.ReadElement_Float(s);
			Ambient = V3_BytesConvert.FromStream(s);
			Flags = (MaterialFlags)s.ReadByte();
			EdgeColor = V4_BytesConvert.FromStream(s);
			EdgeSize = PmxStreamHelper.ReadElement_Float(s);
			Tex = PmxStreamHelper.ReadString(s, f);
			Sphere = PmxStreamHelper.ReadString(s, f);
			SphereMode = (SphereModeType)s.ReadByte();
			Toon = PmxStreamHelper.ReadString(s, f);
			Memo = PmxStreamHelper.ReadString(s, f);
			FaceCount = PmxStreamHelper.ReadElement_Int32(s);
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteString(s, Name, f);
			PmxStreamHelper.WriteString(s, NameE, f);
			V4_BytesConvert.ToStream(s, Diffuse);
			V3_BytesConvert.ToStream(s, Specular);
			PmxStreamHelper.WriteElement_Float(s, Power);
			V3_BytesConvert.ToStream(s, Ambient);
			s.WriteByte((byte)Flags);
			V4_BytesConvert.ToStream(s, EdgeColor);
			PmxStreamHelper.WriteElement_Float(s, EdgeSize);
			PmxStreamHelper.WriteString(s, Tex, f);
			PmxStreamHelper.WriteString(s, Sphere, f);
			s.WriteByte((byte)SphereMode);
			PmxStreamHelper.WriteString(s, Toon, f);
			PmxStreamHelper.WriteString(s, Memo, f);
			PmxStreamHelper.WriteElement_Int32(s, FaceCount);
		}

		public void FromStreamEx_TexTable(Stream s, PmxTextureTable tx, PmxElementFormat f = null)
		{
			Name = PmxStreamHelper.ReadString(s, f);
			NameE = PmxStreamHelper.ReadString(s, f);
			Diffuse = V4_BytesConvert.FromStream(s);
			Specular = V3_BytesConvert.FromStream(s);
			Power = PmxStreamHelper.ReadElement_Float(s);
			Ambient = V3_BytesConvert.FromStream(s);
			Flags = (MaterialFlags)s.ReadByte();
			EdgeColor = V4_BytesConvert.FromStream(s);
			EdgeSize = PmxStreamHelper.ReadElement_Float(s);
			Tex = tx.GetName(PmxStreamHelper.ReadElement_Int32(s, f.TexSize));
			Sphere = tx.GetName(PmxStreamHelper.ReadElement_Int32(s, f.TexSize));
			SphereMode = (SphereModeType)s.ReadByte();
			if (s.ReadByte() == 0)
			{
				Toon = tx.GetName(PmxStreamHelper.ReadElement_Int32(s, f.TexSize));
			}
			else
			{
				int n = s.ReadByte();
				Toon = SystemToon.GetToonName(n);
			}
			Memo = PmxStreamHelper.ReadString(s, f);
			UpdateAttributeFromMemo();
			FaceCount = PmxStreamHelper.ReadElement_Int32(s);
			if (f.WithID)
			{
				base.UID = PmxStreamHelper.ReadElement_UInt(s);
				base.CID = PmxStreamHelper.ReadElement_UInt(s);
			}
		}

		public void ToStreamEx_TexTable(Stream s, PmxTextureTable tx, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteString(s, Name, f);
			PmxStreamHelper.WriteString(s, NameE, f);
			V4_BytesConvert.ToStream(s, Diffuse);
			V3_BytesConvert.ToStream(s, Specular);
			PmxStreamHelper.WriteElement_Float(s, Power);
			V3_BytesConvert.ToStream(s, Ambient);
			s.WriteByte((byte)Flags);
			V4_BytesConvert.ToStream(s, EdgeColor);
			PmxStreamHelper.WriteElement_Float(s, EdgeSize);
			PmxStreamHelper.WriteElement_Int32(s, tx.GetIndex(Tex), f.TexSize);
			PmxStreamHelper.WriteElement_Int32(s, tx.GetIndex(Sphere), f.TexSize);
			s.WriteByte((byte)SphereMode);
			int toonIndex = SystemToon.GetToonIndex(Toon);
			if (toonIndex < 0)
			{
				s.WriteByte(0);
				PmxStreamHelper.WriteElement_Int32(s, tx.GetIndex(Toon), f.TexSize);
			}
			else
			{
				s.WriteByte(1);
				s.WriteByte((byte)toonIndex);
			}
			PmxStreamHelper.WriteString(s, Memo, f);
			PmxStreamHelper.WriteElement_Int32(s, FaceCount);
			if (f.WithID)
			{
				PmxStreamHelper.WriteElement_UInt(s, base.UID);
				PmxStreamHelper.WriteElement_UInt(s, base.CID);
			}
		}

		object ICloneable.Clone()
		{
			return new PmxMaterial(this);
		}

		public PmxMaterial Clone()
		{
			return new PmxMaterial(this);
		}
	}
}
