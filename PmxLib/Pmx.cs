using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PmxLib
{
	public class Pmx : IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public const string RootNodeName = "Root";

		public const string ExpNodeName = "表情";

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.Pmx;
			}
		}

		public PmxHeader Header
		{
			get;
			set;
		}

		public PmxModelInfo ModelInfo
		{
			get;
			set;
		}

		public List<PmxVertex> VertexList
		{
			get;
			set;
		}

		public List<int> FaceList
		{
			get;
			set;
		}

		public List<PmxMaterial> MaterialList
		{
			get;
			set;
		}

		public List<PmxBone> BoneList
		{
			get;
			set;
		}

		public List<PmxMorph> MorphList
		{
			get;
			set;
		}

		public List<PmxNode> NodeList
		{
			get;
			set;
		}

		public List<PmxBody> BodyList
		{
			get;
			set;
		}

		public List<PmxJoint> JointList
		{
			get;
			set;
		}

		public List<PmxSoftBody> SoftBodyList
		{
			get;
			set;
		}

		public PmxNode RootNode
		{
			get;
			set;
		}

		public PmxNode ExpNode
		{
			get;
			set;
		}

		public string FilePath
		{
			get;
			set;
		}

		public static PmxSaveVersion SaveVersion
		{
			get;
			set;
		}

		public static bool AutoSelect_UVACount
		{
			get;
			set;
		}

		public Pmx()
		{
			if (!PmxLibClass.IsLocked())
			{
				this.Header = new PmxHeader(2.1f);
				this.ModelInfo = new PmxModelInfo();
				this.VertexList = new List<PmxVertex>();
				this.FaceList = new List<int>();
				this.MaterialList = new List<PmxMaterial>();
				this.BoneList = new List<PmxBone>();
				this.MorphList = new List<PmxMorph>();
				this.NodeList = new List<PmxNode>();
				this.BodyList = new List<PmxBody>();
				this.JointList = new List<PmxJoint>();
				this.SoftBodyList = new List<PmxSoftBody>();
				this.RootNode = new PmxNode();
				this.ExpNode = new PmxNode();
				this.InitializeSystemNode();
				this.FilePath = "";
			}
		}

		static Pmx()
		{
			Pmx.SaveVersion = PmxSaveVersion.AutoSelect;
			Pmx.AutoSelect_UVACount = true;
		}

		public Pmx(Pmx pmx)
			: this()
		{
			this.FromPmx(pmx);
		}

		public Pmx(string path)
			: this()
		{
			this.FromFile(path);
		}

		public virtual void Clear()
		{
			this.Header.ElementFormat.Ver = 2.1f;
			this.Header.ElementFormat.UVACount = 0;
			this.ModelInfo.Clear();
			this.VertexList.Clear();
			this.FaceList.Clear();
			this.MaterialList.Clear();
			this.BoneList.Clear();
			this.MorphList.Clear();
			this.BodyList.Clear();
			this.JointList.Clear();
			this.SoftBodyList.Clear();
			this.InitializeSystemNode();
			this.FilePath = "";
		}

		public void Initialize()
		{
			this.Clear();
			this.InitializeBone();
		}

		public void InitializeBone()
		{
			this.BoneList.Clear();
			PmxBone pmxBone = new PmxBone();
			pmxBone.Name = "センタ\u30fc";
			pmxBone.NameE = "center";
			pmxBone.Parent = -1;
			pmxBone.SetFlag(PmxBone.BoneFlags.Translation, true);
			this.BoneList.Add(pmxBone);
		}

		public void InitializeSystemNode()
		{
			this.RootNode.Name = "Root";
			this.RootNode.NameE = "Root";
			this.RootNode.SystemNode = true;
			this.RootNode.ElementList.Clear();
			this.RootNode.ElementList.Add(new PmxNode.NodeElement
			{
				ElementType = PmxNode.ElementType.Bone,
				Index = 0
			});
			this.ExpNode.Name = "表情";
			this.ExpNode.NameE = "Exp";
			this.ExpNode.SystemNode = true;
			this.ExpNode.ElementList.Clear();
			this.NodeList.Clear();
			this.NodeList.Add(this.RootNode);
			this.NodeList.Add(this.ExpNode);
		}

		public void UpdateSystemNode()
		{
			for (int i = 0; i < this.NodeList.Count; i++)
			{
				if (this.NodeList[i].SystemNode)
				{
					if (this.NodeList[i].Name == "Root")
					{
						this.RootNode = this.NodeList[i];
					}
					else if (this.NodeList[i].Name == "表情")
					{
						this.ExpNode = this.NodeList[i];
					}
				}
			}
		}

		public bool FromFile(string path)
		{
			bool result = false;
			using (FileStream s = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				try
				{
					this.FromStreamEx(s, null);
					result = true;
				}
				catch (Exception value)
				{
					Console.WriteLine(value);
				}
			}
			this.FilePath = path;
			return result;
		}

		public bool ToFile(string path)
		{
			bool result = false;
			using (FileStream s = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				try
				{
					this.NormalizeVersion();
					if (Pmx.AutoSelect_UVACount)
					{
						this.NormalizeUVACount();
					}
					this.ToStreamEx(s, null);
					result = true;
				}
				catch (Exception value)
				{
					Console.WriteLine(value);
					throw new Exception("保存中にエラ\u30fcが発生しました.");
				}
			}
			this.FilePath = path;
			return result;
		}

		public void FromPmx(Pmx pmx)
		{
			this.Clear();
			this.FilePath = pmx.FilePath;
			this.Header = pmx.Header.Clone();
			this.ModelInfo = pmx.ModelInfo.Clone();
			int count = pmx.VertexList.Count;
			this.VertexList.Capacity = count;
			for (int i = 0; i < count; i++)
			{
				this.VertexList.Add(pmx.VertexList[i].Clone());
			}
			count = pmx.FaceList.Count;
			this.FaceList.Capacity = count;
			for (int j = 0; j < count; j++)
			{
				this.FaceList.Add(pmx.FaceList[j]);
			}
			count = pmx.MaterialList.Count;
			this.MaterialList.Capacity = count;
			for (int k = 0; k < count; k++)
			{
				this.MaterialList.Add(pmx.MaterialList[k].Clone());
			}
			count = pmx.BoneList.Count;
			this.BoneList.Capacity = count;
			for (int l = 0; l < count; l++)
			{
				this.BoneList.Add(pmx.BoneList[l].Clone());
			}
			count = pmx.MorphList.Count;
			this.MorphList.Capacity = count;
			for (int m = 0; m < count; m++)
			{
				this.MorphList.Add(pmx.MorphList[m].Clone());
			}
			count = pmx.NodeList.Count;
			this.NodeList.Clear();
			this.NodeList.Capacity = count;
			for (int n = 0; n < count; n++)
			{
				this.NodeList.Add(pmx.NodeList[n].Clone());
				if (this.NodeList[n].SystemNode)
				{
					if (this.NodeList[n].Name == "Root")
					{
						this.RootNode = this.NodeList[n];
					}
					else if (this.NodeList[n].Name == "表情")
					{
						this.ExpNode = this.NodeList[n];
					}
				}
			}
			count = pmx.BodyList.Count;
			this.BodyList.Capacity = count;
			for (int num = 0; num < count; num++)
			{
				this.BodyList.Add(pmx.BodyList[num].Clone());
			}
			count = pmx.JointList.Count;
			this.JointList.Capacity = count;
			for (int num2 = 0; num2 < count; num2++)
			{
				this.JointList.Add(pmx.JointList[num2].Clone());
			}
			count = pmx.SoftBodyList.Count;
			this.SoftBodyList.Capacity = count;
			for (int num3 = 0; num3 < count; num3++)
			{
				this.SoftBodyList.Add(pmx.SoftBodyList[num3].Clone());
			}
		}

		public virtual void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxHeader pmxHeader = new PmxHeader(2.1f);
			pmxHeader.FromStreamEx(s, null);
			this.Header.FromHeader(pmxHeader);
			this.ModelInfo.FromStreamEx(s, pmxHeader.ElementFormat);
			int num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.VertexList.Clear();
			this.VertexList.Capacity = num;
			for (int i = 0; i < num; i++)
			{
				PmxVertex pmxVertex = new PmxVertex();
				pmxVertex.FromStreamEx(s, pmxHeader.ElementFormat);
				this.VertexList.Add(pmxVertex);
			}
			num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.FaceList.Clear();
			this.FaceList.Capacity = num;
			for (int j = 0; j < num; j++)
			{
				int item = PmxStreamHelper.ReadElement_Int32(s, pmxHeader.ElementFormat.VertexSize, false);
				this.FaceList.Add(item);
			}
			PmxTextureTable pmxTextureTable = new PmxTextureTable();
			pmxTextureTable.FromStreamEx(s, pmxHeader.ElementFormat);
			num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.MaterialList.Clear();
			this.MaterialList.Capacity = num;
			for (int k = 0; k < num; k++)
			{
				PmxMaterial pmxMaterial = new PmxMaterial();
				pmxMaterial.FromStreamEx_TexTable(s, pmxTextureTable, pmxHeader.ElementFormat);
				this.MaterialList.Add(pmxMaterial);
			}
			num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.BoneList.Clear();
			this.BoneList.Capacity = num;
			for (int l = 0; l < num; l++)
			{
				PmxBone pmxBone = new PmxBone();
				pmxBone.FromStreamEx(s, pmxHeader.ElementFormat);
				this.BoneList.Add(pmxBone);
			}
			num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.MorphList.Clear();
			this.MorphList.Capacity = num;
			for (int m = 0; m < num; m++)
			{
				PmxMorph pmxMorph = new PmxMorph();
				pmxMorph.FromStreamEx(s, pmxHeader.ElementFormat);
				this.MorphList.Add(pmxMorph);
			}
			num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.NodeList.Clear();
			this.NodeList.Capacity = num;
			for (int n = 0; n < num; n++)
			{
				PmxNode pmxNode = new PmxNode();
				pmxNode.FromStreamEx(s, pmxHeader.ElementFormat);
				this.NodeList.Add(pmxNode);
				if (this.NodeList[n].SystemNode)
				{
					if (this.NodeList[n].Name == "Root")
					{
						this.RootNode = this.NodeList[n];
					}
					else if (this.NodeList[n].Name == "表情")
					{
						this.ExpNode = this.NodeList[n];
					}
				}
			}
			num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.BodyList.Clear();
			this.BodyList.Capacity = num;
			for (int num2 = 0; num2 < num; num2++)
			{
				PmxBody pmxBody = new PmxBody();
				pmxBody.FromStreamEx(s, pmxHeader.ElementFormat);
				this.BodyList.Add(pmxBody);
			}
			num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.JointList.Clear();
			this.JointList.Capacity = num;
			for (int num3 = 0; num3 < num; num3++)
			{
				PmxJoint pmxJoint = new PmxJoint();
				pmxJoint.FromStreamEx(s, pmxHeader.ElementFormat);
				this.JointList.Add(pmxJoint);
			}
			if (pmxHeader.Ver >= 2.1f)
			{
				num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
				this.SoftBodyList.Clear();
				this.SoftBodyList.Capacity = num;
				for (int num4 = 0; num4 < num; num4++)
				{
					PmxSoftBody pmxSoftBody = new PmxSoftBody();
					pmxSoftBody.FromStreamEx(s, pmxHeader.ElementFormat);
					this.SoftBodyList.Add(pmxSoftBody);
				}
			}
		}

		public void UpdateElementFormatSize(PmxElementFormat f = null, PmxTextureTable tx = null)
		{
			if (f == null)
			{
				f = this.Header.ElementFormat;
			}
			f.VertexSize = PmxElementFormat.GetUnsignedBufSize(this.VertexList.Count);
			f.MaterialSize = PmxElementFormat.GetSignedBufSize(this.MaterialList.Count);
			f.BoneSize = PmxElementFormat.GetSignedBufSize(this.BoneList.Count);
			f.MorphSize = PmxElementFormat.GetSignedBufSize(this.MorphList.Count);
			f.BodySize = PmxElementFormat.GetSignedBufSize(this.BodyList.Count);
			if (tx == null)
			{
				tx = new PmxTextureTable(this.MaterialList);
			}
			f.TexSize = PmxElementFormat.GetSignedBufSize(tx.Count);
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxHeader header = this.Header;
			PmxTextureTable pmxTextureTable = new PmxTextureTable(this.MaterialList);
			this.UpdateElementFormatSize(header.ElementFormat, pmxTextureTable);
			header.ToStreamEx(s, null);
			this.ModelInfo.ToStreamEx(s, header.ElementFormat);
			PmxStreamHelper.WriteElement_Int32(s, this.VertexList.Count, 4, true);
			for (int i = 0; i < this.VertexList.Count; i++)
			{
				this.VertexList[i].ToStreamEx(s, header.ElementFormat);
			}
			PmxStreamHelper.WriteElement_Int32(s, this.FaceList.Count, 4, true);
			for (int j = 0; j < this.FaceList.Count; j++)
			{
				PmxStreamHelper.WriteElement_Int32(s, this.FaceList[j], header.ElementFormat.VertexSize, false);
			}
			pmxTextureTable.ToStreamEx(s, header.ElementFormat);
			PmxStreamHelper.WriteElement_Int32(s, this.MaterialList.Count, 4, true);
			for (int k = 0; k < this.MaterialList.Count; k++)
			{
				this.MaterialList[k].ToStreamEx_TexTable(s, pmxTextureTable, header.ElementFormat);
			}
			PmxStreamHelper.WriteElement_Int32(s, this.BoneList.Count, 4, true);
			for (int l = 0; l < this.BoneList.Count; l++)
			{
				this.BoneList[l].ToStreamEx(s, header.ElementFormat);
			}
			if (header.Ver < 2.1f)
			{
				int num = (from mp in this.MorphList
				where mp.IsImpulse
				select mp).Count();
				PmxStreamHelper.WriteElement_Int32(s, this.MorphList.Count - num, 4, true);
			}
			else
			{
				PmxStreamHelper.WriteElement_Int32(s, this.MorphList.Count, 4, true);
			}
			for (int m = 0; m < this.MorphList.Count; m++)
			{
				this.MorphList[m].ToStreamEx(s, header.ElementFormat);
			}
			PmxStreamHelper.WriteElement_Int32(s, this.NodeList.Count, 4, true);
			for (int n = 0; n < this.NodeList.Count; n++)
			{
				this.NodeList[n].ToStreamEx(s, header.ElementFormat);
			}
			PmxStreamHelper.WriteElement_Int32(s, this.BodyList.Count, 4, true);
			for (int num2 = 0; num2 < this.BodyList.Count; num2++)
			{
				this.BodyList[num2].ToStreamEx(s, header.ElementFormat);
			}
			PmxStreamHelper.WriteElement_Int32(s, this.JointList.Count, 4, true);
			for (int num3 = 0; num3 < this.JointList.Count; num3++)
			{
				this.JointList[num3].ToStreamEx(s, header.ElementFormat);
			}
			if (header.Ver >= 2.1f)
			{
				PmxStreamHelper.WriteElement_Int32(s, this.SoftBodyList.Count, 4, true);
				for (int num4 = 0; num4 < this.SoftBodyList.Count; num4++)
				{
					this.SoftBodyList[num4].ToStreamEx(s, header.ElementFormat);
				}
			}
		}

		public void ClearMaterialNames()
		{
			for (int i = 0; i < this.MaterialList.Count; i++)
			{
				this.MaterialList[i].Name = "材質" + (i + 1).ToString();
			}
		}

		public static void UpdateBoneIKKind(List<PmxBone> boneList)
		{
			for (int i = 0; i < boneList.Count; i++)
			{
				PmxBone pmxBone = boneList[i];
				pmxBone.IKKind = PmxBone.IKKindType.None;
			}
			for (int j = 0; j < boneList.Count; j++)
			{
				PmxBone pmxBone2 = boneList[j];
				if (pmxBone2.GetFlag(PmxBone.BoneFlags.IK))
				{
					pmxBone2.IKKind = PmxBone.IKKindType.IK;
					int target = pmxBone2.IK.Target;
					if (CP.InRange(boneList, target))
					{
						boneList[target].IKKind = PmxBone.IKKindType.Target;
					}
					for (int k = 0; k < pmxBone2.IK.LinkList.Count; k++)
					{
						int bone = pmxBone2.IK.LinkList[k].Bone;
						if (CP.InRange(boneList, bone))
						{
							boneList[bone].IKKind = PmxBone.IKKindType.Link;
						}
					}
				}
			}
		}

		public void UpdateBoneIKKind()
		{
			Pmx.UpdateBoneIKKind(this.BoneList);
		}

		public void NormalizeVertex_SDEF_C0()
		{
			for (int i = 0; i < this.VertexList.Count; i++)
			{
				this.VertexList[i].NormalizeSDEF_C0(this.BoneList);
			}
		}

		public float RequireVersion(out bool isQDEF, out bool isExMorph, out bool isExJoint, out bool isSoftBody)
		{
			Func<bool> func = delegate
			{
				bool result4 = false;
				for (int k = 0; k < this.VertexList.Count; k++)
				{
					PmxVertex pmxVertex = this.VertexList[k];
					if (pmxVertex.Deform == PmxVertex.DeformType.QDEF)
					{
						result4 = true;
						break;
					}
				}
				return result4;
			};
			Func<bool> func2 = delegate
			{
				bool result3 = false;
				for (int j = 0; j < this.MorphList.Count; j++)
				{
					PmxMorph pmxMorph = this.MorphList[j];
					if (pmxMorph.IsFlip || pmxMorph.IsImpulse)
					{
						result3 = true;
						break;
					}
				}
				return result3;
			};
			Func<bool> func3 = delegate
			{
				bool result2 = false;
				for (int i = 0; i < this.JointList.Count; i++)
				{
					PmxJoint pmxJoint = this.JointList[i];
					if (pmxJoint.Kind != 0)
					{
						result2 = true;
						break;
					}
				}
				return result2;
			};
			Func<bool> func4 = () => this.SoftBodyList.Count > 0;
			isQDEF = func();
			isExMorph = func2();
			isExJoint = func3();
			isSoftBody = func4();
			float result = 2f;
			if (isQDEF || isExMorph || isExJoint || isSoftBody)
			{
				result = 2.1f;
			}
			return result;
		}

		private void NormalizeVersion()
		{
			bool flag = default(bool);
			bool flag2 = default(bool);
			bool flag3 = default(bool);
			bool flag4 = default(bool);
			float ver = this.RequireVersion(out flag, out flag2, out flag3, out flag4);
			switch (Pmx.SaveVersion)
			{
			case PmxSaveVersion.AutoSelect:
				this.Header.Ver = ver;
				break;
			case PmxSaveVersion.PMX2_0:
			{
				string text = "";
				if (flag)
				{
					text = text + "頂点ウェイト : QDEF -> BDEF4" + Environment.NewLine;
				}
				if (flag2)
				{
					text = text + "モ\u30fcフ : インパルス->削除／フリップ->グル\u30fcプ" + Environment.NewLine;
				}
				if (flag3)
				{
					text = text + "Joint : 拡張Joint -> 基本Joint(ﾊ\uff9eﾈ付6DOF)" + Environment.NewLine;
				}
				if (flag4)
				{
					text = text + "SoftBody : 削除" + Environment.NewLine;
				}
				this.Header.Ver = 2f;
				if (text.Length > 0)
				{
					text = "PMX2.0での保存では 以下の項目が書き換えられますが よろしいですか?" + Environment.NewLine + Environment.NewLine + text;
				}
				break;
			}
			case PmxSaveVersion.PMX2_1:
				this.Header.Ver = 2.1f;
				break;
			}
		}

		public void NormalizeUVACount()
		{
			if (this.VertexList.Count <= 0)
			{
				this.Header.ElementFormat.UVACount = 0;
			}
			else
			{
				Func<Vector4, bool> func = (Vector4 v) => Math.Abs(v.x) > 1E-12f || Math.Abs(v.y) > 1E-12f || Math.Abs(v.z) > 1E-12f || Math.Abs(v.w) > 1E-12f;
				int num = 0;
				foreach (PmxVertex vertex in this.VertexList)
				{
					for (int i = 0; i < vertex.UVA.Length; i++)
					{
						if (func(vertex.UVA[i]))
						{
							int num2 = i + 1;
							if (num < num2)
							{
								num = num2;
							}
						}
					}
				}
				this.Header.ElementFormat.UVACount = num;
			}
		}

		object ICloneable.Clone()
		{
			return new Pmx(this);
		}

		public virtual Pmx Clone()
		{
			return new Pmx(this);
		}
	}
}
