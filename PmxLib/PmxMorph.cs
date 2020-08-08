using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	public class PmxMorph : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
	{
		public enum OffsetKind
		{
			Group,
			Vertex,
			Bone,
			UV,
			UVA1,
			UVA2,
			UVA3,
			UVA4,
			Material,
			Flip,
			Impulse
		}

		public int Panel;

		public OffsetKind Kind;

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.Morph;
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

		public List<PmxBaseMorph> OffsetList
		{
			get;
			private set;
		}

		public bool IsUV
		{
			get
			{
				return this.Kind == OffsetKind.UV || this.Kind == OffsetKind.UVA1 || this.Kind == OffsetKind.UVA2 || this.Kind == OffsetKind.UVA3 || this.Kind == OffsetKind.UVA4;
			}
		}

		public bool IsVertex
		{
			get
			{
				return this.Kind == OffsetKind.Vertex;
			}
		}

		public bool IsBone
		{
			get
			{
				return this.Kind == OffsetKind.Bone;
			}
		}

		public bool IsMaterial
		{
			get
			{
				return this.Kind == OffsetKind.Material;
			}
		}

		public bool IsFlip
		{
			get
			{
				return this.Kind == OffsetKind.Flip;
			}
		}

		public bool IsImpulse
		{
			get
			{
				return this.Kind == OffsetKind.Impulse;
			}
		}

		public bool IsGroup
		{
			get
			{
				return this.Kind == OffsetKind.Group;
			}
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

		public static string KindText(OffsetKind kind)
		{
			string result = "-";
			switch (kind)
			{
			case OffsetKind.Group:
				result = "グル\u30fcプ";
				break;
			case OffsetKind.Vertex:
				result = "頂点";
				break;
			case OffsetKind.Bone:
				result = "ボ\u30fcン";
				break;
			case OffsetKind.UV:
				result = "UV";
				break;
			case OffsetKind.UVA1:
				result = "追加UV1";
				break;
			case OffsetKind.UVA2:
				result = "追加UV2";
				break;
			case OffsetKind.UVA3:
				result = "追加UV3";
				break;
			case OffsetKind.UVA4:
				result = "追加UV4";
				break;
			case OffsetKind.Material:
				result = "材質";
				break;
			case OffsetKind.Flip:
				result = "フリップ";
				break;
			case OffsetKind.Impulse:
				result = "インパルス";
				break;
			}
			return result;
		}

		public PmxMorph()
		{
			this.Name = "";
			this.NameE = "";
			this.Panel = 4;
			this.Kind = OffsetKind.Vertex;
			this.OffsetList = new List<PmxBaseMorph>();
		}

		public PmxMorph(PmxMorph m, bool nonStr = false)
		{
			this.FromPmxMorph(m, nonStr);
		}

		public void FromPmxMorph(PmxMorph m, bool nonStr = false)
		{
			if (!nonStr)
			{
				this.Name = m.Name;
				this.NameE = m.NameE;
			}
			this.Panel = m.Panel;
			this.Kind = m.Kind;
			int count = m.OffsetList.Count;
			this.OffsetList = new List<PmxBaseMorph>(count);
			for (int i = 0; i < count; i++)
			{
				this.OffsetList.Add(m.OffsetList[i].Clone());
			}
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			this.Name = PmxStreamHelper.ReadString(s, f);
			this.NameE = PmxStreamHelper.ReadString(s, f);
			this.Panel = PmxStreamHelper.ReadElement_Int32(s, 1, true);
			this.Kind = (OffsetKind)PmxStreamHelper.ReadElement_Int32(s, 1, true);
			int num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.OffsetList.Clear();
			this.OffsetList.Capacity = num;
			for (int i = 0; i < num; i++)
			{
				switch (this.Kind)
				{
				case OffsetKind.Group:
				case OffsetKind.Flip:
				{
					PmxGroupMorph pmxGroupMorph = new PmxGroupMorph();
					pmxGroupMorph.FromStreamEx(s, f);
					this.OffsetList.Add(pmxGroupMorph);
					break;
				}
				case OffsetKind.Vertex:
				{
					PmxVertexMorph pmxVertexMorph = new PmxVertexMorph();
					pmxVertexMorph.FromStreamEx(s, f);
					this.OffsetList.Add(pmxVertexMorph);
					break;
				}
				case OffsetKind.Bone:
				{
					PmxBoneMorph pmxBoneMorph = new PmxBoneMorph();
					pmxBoneMorph.FromStreamEx(s, f);
					this.OffsetList.Add(pmxBoneMorph);
					break;
				}
				case OffsetKind.UV:
				case OffsetKind.UVA1:
				case OffsetKind.UVA2:
				case OffsetKind.UVA3:
				case OffsetKind.UVA4:
				{
					PmxUVMorph pmxUVMorph = new PmxUVMorph();
					pmxUVMorph.FromStreamEx(s, f);
					this.OffsetList.Add(pmxUVMorph);
					break;
				}
				case OffsetKind.Material:
				{
					PmxMaterialMorph pmxMaterialMorph = new PmxMaterialMorph();
					pmxMaterialMorph.FromStreamEx(s, f);
					this.OffsetList.Add(pmxMaterialMorph);
					break;
				}
				case OffsetKind.Impulse:
				{
					PmxImpulseMorph pmxImpulseMorph = new PmxImpulseMorph();
					pmxImpulseMorph.FromStreamEx(s, f);
					this.OffsetList.Add(pmxImpulseMorph);
					break;
				}
				}
			}
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			if (!this.IsImpulse || !(f.Ver < 2.1f))
			{
				PmxStreamHelper.WriteString(s, this.Name, f);
				PmxStreamHelper.WriteString(s, this.NameE, f);
				PmxStreamHelper.WriteElement_Int32(s, this.Panel, 1, true);
				if (this.IsFlip && f.Ver < 2.1f)
				{
					PmxStreamHelper.WriteElement_Int32(s, 0, 1, true);
				}
				else
				{
					PmxStreamHelper.WriteElement_Int32(s, (int)this.Kind, 1, true);
				}
				PmxStreamHelper.WriteElement_Int32(s, this.OffsetList.Count, 4, true);
				for (int i = 0; i < this.OffsetList.Count; i++)
				{
					switch (this.Kind)
					{
					case OffsetKind.Group:
					case OffsetKind.Flip:
						(this.OffsetList[i] as PmxGroupMorph).ToStreamEx(s, f);
						break;
					case OffsetKind.Vertex:
						(this.OffsetList[i] as PmxVertexMorph).ToStreamEx(s, f);
						break;
					case OffsetKind.Bone:
						(this.OffsetList[i] as PmxBoneMorph).ToStreamEx(s, f);
						break;
					case OffsetKind.UV:
					case OffsetKind.UVA1:
					case OffsetKind.UVA2:
					case OffsetKind.UVA3:
					case OffsetKind.UVA4:
						(this.OffsetList[i] as PmxUVMorph).ToStreamEx(s, f);
						break;
					case OffsetKind.Material:
						(this.OffsetList[i] as PmxMaterialMorph).ToStreamEx(s, f);
						break;
					case OffsetKind.Impulse:
						(this.OffsetList[i] as PmxImpulseMorph).ToStreamEx(s, f);
						break;
					}
				}
			}
		}

		object ICloneable.Clone()
		{
			return new PmxMorph(this, false);
		}

		public PmxMorph Clone()
		{
			return new PmxMorph(this, false);
		}
	}
}
