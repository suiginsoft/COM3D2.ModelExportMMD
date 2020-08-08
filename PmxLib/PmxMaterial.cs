using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PmxLib
{
	public class PmxMaterial : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
	{
		[Flags]
		public enum MaterialFlags
		{
			None = 0,
			DrawBoth = 1,
			Shadow = 2,
			SelfShadowMap = 4,
			SelfShadow = 8,
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

		public Color Diffuse;

		public Color Specular;

		public float Power;

		public Color Ambient;

		public ExDrawMode ExDraw;

		public MaterialFlags Flags;

		public Color EdgeColor;

		public float EdgeSize;

		public int FaceCount;

		public SphereModeType SphereMode;

		public PmxMaterialMorph.MorphData OffsetMul;

		public PmxMaterialMorph.MorphData OffsetAdd;

		public List<PmxFace> FaceList;

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.Material;
			}
		}

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
				return this.Name;
			}
			set
			{
				this.Name = value;
			}
		}

		public void ClearFlags()
		{
			this.Flags = MaterialFlags.None;
		}

		public bool GetFlag(MaterialFlags f)
		{
			return (f & this.Flags) == f;
		}

		public void SetFlag(MaterialFlags f, bool val)
		{
			if (val)
			{
				this.Flags |= f;
			}
			else
			{
				this.Flags &= ~f;
			}
		}

		public void ClearOffset()
		{
			this.OffsetMul.Clear(PmxMaterialMorph.OpType.Mul);
			this.OffsetAdd.Clear(PmxMaterialMorph.OpType.Add);
		}

		public void UpdateAttributeFromMemo()
		{
			this.Attribute.SetFromText(this.Memo);
		}

		public PmxMaterial()
		{
			this.Name = "";
			this.NameE = "";
			this.Diffuse = new Color(0f, 0f, 0f);
			this.Specular = new Color(0f, 0f, 0f);
			this.Power = 0f;
			this.Ambient = new Color(0f, 0f, 0f);
			this.ClearFlags();
			this.EdgeColor = new Color(0f, 0f, 0f);
			this.EdgeSize = 1f;
			this.Tex = "";
			this.Sphere = "";
			this.SphereMode = SphereModeType.Mul;
			this.Toon = "";
			this.Memo = "";
			this.OffsetMul = default(PmxMaterialMorph.MorphData);
			this.OffsetAdd = default(PmxMaterialMorph.MorphData);
			this.ClearOffset();
			this.ExDraw = ExDrawMode.F3;
			this.Attribute = new PmxMaterialAttribute();
		}

		public PmxMaterial(PmxMaterial m, bool nonStr = false)
			: this()
		{
			this.FromPmxMaterial(m, nonStr);
		}

		public void FromPmxMaterial(PmxMaterial m, bool nonStr = false)
		{
			this.Diffuse = m.Diffuse;
			this.Specular = m.Specular;
			this.Power = m.Power;
			this.Ambient = m.Ambient;
			this.Flags = m.Flags;
			this.EdgeColor = m.EdgeColor;
			this.EdgeSize = m.EdgeSize;
			this.SphereMode = m.SphereMode;
			this.FaceCount = m.FaceCount;
			this.ExDraw = m.ExDraw;
			if (!nonStr)
			{
				this.Name = m.Name;
				this.NameE = m.NameE;
				this.Tex = m.Tex;
				this.Sphere = m.Sphere;
				this.Toon = m.Toon;
				this.Memo = m.Memo;
			}
			this.Attribute = m.Attribute;
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			this.Name = PmxStreamHelper.ReadString(s, f);
			this.NameE = PmxStreamHelper.ReadString(s, f);
			this.Diffuse = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
			this.Specular = V3_BytesConvert.Vector3ToColor(V3_BytesConvert.FromStream(s));
			this.Power = PmxStreamHelper.ReadElement_Float(s);
			this.Ambient = V3_BytesConvert.Vector3ToColor(V3_BytesConvert.FromStream(s));
			this.Flags = (MaterialFlags)s.ReadByte();
			this.EdgeColor = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
			this.EdgeSize = PmxStreamHelper.ReadElement_Float(s);
			this.Tex = PmxStreamHelper.ReadString(s, f);
			this.Sphere = PmxStreamHelper.ReadString(s, f);
			this.SphereMode = (SphereModeType)s.ReadByte();
			this.Toon = PmxStreamHelper.ReadString(s, f);
			this.Memo = PmxStreamHelper.ReadString(s, f);
			this.FaceCount = PmxStreamHelper.ReadElement_Int32(s, 4, true);
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteString(s, this.Name, f);
			PmxStreamHelper.WriteString(s, this.NameE, f);
			V4_BytesConvert.ToStream(s, V4_BytesConvert.ColorToVector4(this.Diffuse));
			V3_BytesConvert.ToStream(s, V3_BytesConvert.ColorToVector3(this.Specular));
			PmxStreamHelper.WriteElement_Float(s, this.Power);
			V3_BytesConvert.ToStream(s, V3_BytesConvert.ColorToVector3(this.Ambient));
			s.WriteByte((byte)this.Flags);
			V4_BytesConvert.ToStream(s, V4_BytesConvert.ColorToVector4(this.EdgeColor));
			PmxStreamHelper.WriteElement_Float(s, this.EdgeSize);
			PmxStreamHelper.WriteString(s, this.Tex, f);
			PmxStreamHelper.WriteString(s, this.Sphere, f);
			s.WriteByte((byte)this.SphereMode);
			PmxStreamHelper.WriteString(s, this.Toon, f);
			PmxStreamHelper.WriteString(s, this.Memo, f);
			PmxStreamHelper.WriteElement_Int32(s, this.FaceCount, 4, true);
		}

		public void FromStreamEx_TexTable(Stream s, PmxTextureTable tx, PmxElementFormat f = null)
		{
			this.Name = PmxStreamHelper.ReadString(s, f);
			this.NameE = PmxStreamHelper.ReadString(s, f);
			this.Diffuse = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
			this.Specular = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
			this.Power = PmxStreamHelper.ReadElement_Float(s);
			this.Ambient = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
			this.Flags = (MaterialFlags)s.ReadByte();
			this.EdgeColor = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
			this.EdgeSize = PmxStreamHelper.ReadElement_Float(s);
			this.Tex = tx.GetName(PmxStreamHelper.ReadElement_Int32(s, f.TexSize, true));
			this.Sphere = tx.GetName(PmxStreamHelper.ReadElement_Int32(s, f.TexSize, true));
			this.SphereMode = (SphereModeType)s.ReadByte();
			if (s.ReadByte() == 0)
			{
				this.Toon = tx.GetName(PmxStreamHelper.ReadElement_Int32(s, f.TexSize, true));
			}
			else
			{
				int n = s.ReadByte();
				this.Toon = SystemToon.GetToonName(n);
			}
			this.Memo = PmxStreamHelper.ReadString(s, f);
			this.UpdateAttributeFromMemo();
			this.FaceCount = PmxStreamHelper.ReadElement_Int32(s, 4, true);
		}

		public void ToStreamEx_TexTable(Stream s, PmxTextureTable tx, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteString(s, this.Name, f);
			PmxStreamHelper.WriteString(s, this.NameE, f);
			V4_BytesConvert.ToStream(s, V4_BytesConvert.ColorToVector4(this.Diffuse));
			V3_BytesConvert.ToStream(s, V3_BytesConvert.ColorToVector3(this.Specular));
			PmxStreamHelper.WriteElement_Float(s, this.Power);
			V3_BytesConvert.ToStream(s, V3_BytesConvert.ColorToVector3(this.Ambient));
			s.WriteByte((byte)this.Flags);
			V4_BytesConvert.ToStream(s, V4_BytesConvert.ColorToVector4(this.EdgeColor));
			PmxStreamHelper.WriteElement_Float(s, this.EdgeSize);
			PmxStreamHelper.WriteElement_Int32(s, tx.GetIndex(this.Tex), f.TexSize, true);
			PmxStreamHelper.WriteElement_Int32(s, tx.GetIndex(this.Sphere), f.TexSize, true);
			s.WriteByte((byte)this.SphereMode);
			int toonIndex = SystemToon.GetToonIndex(this.Toon);
			if (toonIndex < 0)
			{
				s.WriteByte(0);
				PmxStreamHelper.WriteElement_Int32(s, tx.GetIndex(this.Toon), f.TexSize, true);
			}
			else
			{
				s.WriteByte(1);
				s.WriteByte((byte)toonIndex);
			}
			PmxStreamHelper.WriteString(s, this.Memo, f);
			PmxStreamHelper.WriteElement_Int32(s, this.FaceCount, 4, true);
		}

		object ICloneable.Clone()
		{
			return new PmxMaterial(this, false);
		}

		public PmxMaterial Clone()
		{
			return new PmxMaterial(this, false);
		}
	}
}
