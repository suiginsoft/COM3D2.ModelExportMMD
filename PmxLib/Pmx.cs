using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PmxLib
{
	internal class Pmx : IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public const string RootNodeName = "Root";

		public const string ExpNodeName = "表情";

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.Pmx;

		public PmxHeader Header
		{
			get;
			private set;
		}

		public PmxModelInfo ModelInfo
		{
			get;
			private set;
		}

		public List<PmxVertex> VertexList
		{
			get;
			private set;
		}

		public List<int> FaceList
		{
			get;
			private set;
		}

		public List<PmxMaterial> MaterialList
		{
			get;
			private set;
		}

		public List<PmxBone> BoneList
		{
			get;
			private set;
		}

		public List<PmxMorph> MorphList
		{
			get;
			private set;
		}

		public List<PmxNode> NodeList
		{
			get;
			private set;
		}

		public List<PmxBody> BodyList
		{
			get;
			private set;
		}

		public List<PmxJoint> JointList
		{
			get;
			private set;
		}

		public List<PmxSoftBody> SoftBodyList
		{
			get;
			private set;
		}

		public PmxNode RootNode
		{
			get;
			private set;
		}

		public PmxNode ExpNode
		{
			get;
			private set;
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

		public int LoadErrCount
		{
			get;
			private set;
		}

		public Pmx()
		{
			Header = new PmxHeader();
			ModelInfo = new PmxModelInfo();
			VertexList = new List<PmxVertex>();
			FaceList = new List<int>();
			MaterialList = new List<PmxMaterial>();
			BoneList = new List<PmxBone>();
			MorphList = new List<PmxMorph>();
			NodeList = new List<PmxNode>();
			BodyList = new List<PmxBody>();
			JointList = new List<PmxJoint>();
			SoftBodyList = new List<PmxSoftBody>();
			RootNode = new PmxNode();
			ExpNode = new PmxNode();
			InitializeSystemNode();
			FilePath = "";
			LoadErrCount = 0;
		}

		static Pmx()
		{
			SaveVersion = PmxSaveVersion.AutoSelect;
			AutoSelect_UVACount = true;
		}

		public Pmx(Pmx pmx)
			: this()
		{
			FromPmx(pmx);
		}

		public Pmx(string path)
			: this()
		{
			FromFile(path);
		}

		public virtual void Clear()
		{
			Header.ElementFormat.Ver = 2.1f;
			Header.ElementFormat.UVACount = 0;
			ModelInfo.Clear();
			VertexList.Clear();
			FaceList.Clear();
			MaterialList.Clear();
			BoneList.Clear();
			MorphList.Clear();
			BodyList.Clear();
			JointList.Clear();
			SoftBodyList.Clear();
			InitializeSystemNode();
			FilePath = "";
			LoadErrCount = 0;
		}

		public void Initialize()
		{
			Clear();
			InitializeBone();
		}

		public void InitializeBone()
		{
			BoneList.Clear();
			PmxBone pmxBone = new PmxBone();
			pmxBone.Name = "センター";
			pmxBone.NameE = "center";
			pmxBone.Parent = -1;
			pmxBone.SetFlag(PmxBone.BoneFlags.Translation, val: true);
			BoneList.Add(pmxBone);
		}

		public void InitializeSystemNode()
		{
			RootNode.Name = "Root";
			RootNode.NameE = "Root";
			RootNode.SystemNode = true;
			RootNode.ElementList.Clear();
			RootNode.ElementList.Add(new PmxNode.NodeElement
			{
				ElementType = PmxNode.ElementType.Bone,
				Index = 0
			});
			ExpNode.Name = "表情";
			ExpNode.NameE = "Exp";
			ExpNode.SystemNode = true;
			ExpNode.ElementList.Clear();
			NodeList.Clear();
			NodeList.Add(RootNode);
			NodeList.Add(ExpNode);
		}

		public void UpdateSystemNode()
		{
			for (int i = 0; i < NodeList.Count; i++)
			{
				if (NodeList[i].SystemNode)
				{
					if (NodeList[i].Name == "Root")
					{
						RootNode = NodeList[i];
					}
					else if (NodeList[i].Name == "表情")
					{
						ExpNode = NodeList[i];
					}
				}
			}
		}

		public bool FromFile(string path)
		{
			try
			{
				using (FileStream s = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					FromStreamEx(s);
					FilePath = path;
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		public bool ToFile(string path)
		{
			try
			{
				using (FileStream s = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					NormalizeVersion();
					if (AutoSelect_UVACount)
						NormalizeUVACount();
					ToStreamEx(s);
					FilePath = path;
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		public void FromPmx(Pmx pmx)
		{
			Clear();
			FilePath = pmx.FilePath;
			LoadErrCount = pmx.LoadErrCount;
			Header = pmx.Header.Clone();
			ModelInfo = pmx.ModelInfo.Clone();
			int count = pmx.VertexList.Count;
			VertexList.Capacity = count;
			for (int i = 0; i < count; i++)
			{
				VertexList.Add(pmx.VertexList[i].Clone());
			}
			count = pmx.FaceList.Count;
			FaceList.Capacity = count;
			for (int j = 0; j < count; j++)
			{
				FaceList.Add(pmx.FaceList[j]);
			}
			count = pmx.MaterialList.Count;
			MaterialList.Capacity = count;
			for (int k = 0; k < count; k++)
			{
				MaterialList.Add(pmx.MaterialList[k].Clone());
			}
			count = pmx.BoneList.Count;
			BoneList.Capacity = count;
			for (int l = 0; l < count; l++)
			{
				BoneList.Add(pmx.BoneList[l].Clone());
			}
			count = pmx.MorphList.Count;
			MorphList.Capacity = count;
			for (int m = 0; m < count; m++)
			{
				MorphList.Add(pmx.MorphList[m].Clone());
			}
			count = pmx.NodeList.Count;
			NodeList.Clear();
			NodeList.Capacity = count;
			for (int n = 0; n < count; n++)
			{
				NodeList.Add(pmx.NodeList[n].Clone());
				if (NodeList[n].SystemNode)
				{
					if (NodeList[n].Name == "Root")
					{
						RootNode = NodeList[n];
					}
					else if (NodeList[n].Name == "表情")
					{
						ExpNode = NodeList[n];
					}
				}
			}
			count = pmx.BodyList.Count;
			BodyList.Capacity = count;
			for (int num = 0; num < count; num++)
			{
				BodyList.Add(pmx.BodyList[num].Clone());
			}
			count = pmx.JointList.Count;
			JointList.Capacity = count;
			for (int num2 = 0; num2 < count; num2++)
			{
				JointList.Add(pmx.JointList[num2].Clone());
			}
			count = pmx.SoftBodyList.Count;
			SoftBodyList.Capacity = count;
			for (int num3 = 0; num3 < count; num3++)
			{
				SoftBodyList.Add(pmx.SoftBodyList[num3].Clone());
			}
		}

		public virtual void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			bool id = false;
			if (f != null && f.WithID)
			{
				id = true;
			}
			FromStream(s, id);
		}

		public virtual void FromStream(Stream s, bool id)
		{
			Action<Action> action = delegate(Action a)
			{
				try
				{
					a();
				}
				catch (Exception)
				{
					LoadErrCount++;
				}
			};
			PmxHeader head = new PmxHeader();
			head.FromStreamEx(s);
			Header.FromHeader(head);
			head.ElementFormat.WithID = id;
			action(delegate
			{
				ModelInfo.FromStreamEx(s, head.ElementFormat);
			});
			int count = 0;
			action(delegate
			{
				count = PmxStreamHelper.ReadElement_Int32(s);
			});
			VertexList.Clear();
			VertexList.Capacity = count;
			for (int k = 0; k < count; k++)
			{
				PmxVertex v = new PmxVertex();
				action(delegate
				{
					v.FromStreamEx(s, head.ElementFormat);
				});
				VertexList.Add(v);
			}
			action(delegate
			{
				count = PmxStreamHelper.ReadElement_Int32(s);
			});
			FaceList.Clear();
			FaceList.Capacity = count;
			for (int l = 0; l < count; l++)
			{
				int ix = 0;
				action(delegate
				{
					ix = PmxStreamHelper.ReadElement_Int32(s, head.ElementFormat.VertexSize, signed: false);
				});
				FaceList.Add(ix);
			}
			PmxTextureTable tx = new PmxTextureTable();
			action(delegate
			{
				tx.FromStreamEx(s, head.ElementFormat);
			});
			action(delegate
			{
				count = PmxStreamHelper.ReadElement_Int32(s);
			});
			MaterialList.Clear();
			MaterialList.Capacity = count;
			for (int m = 0; m < count; m++)
			{
				PmxMaterial j = new PmxMaterial();
				action(delegate
				{
					j.FromStreamEx_TexTable(s, tx, head.ElementFormat);
				});
				MaterialList.Add(j);
			}
			action(delegate
			{
				count = PmxStreamHelper.ReadElement_Int32(s);
			});
			BoneList.Clear();
			BoneList.Capacity = count;
			for (int n = 0; n < count; n++)
			{
				PmxBone b = new PmxBone();
				action(delegate
				{
					b.FromStreamEx(s, head.ElementFormat);
				});
				BoneList.Add(b);
			}
			action(delegate
			{
				count = PmxStreamHelper.ReadElement_Int32(s);
			});
			MorphList.Clear();
			MorphList.Capacity = count;
			for (int num = 0; num < count; num++)
			{
				PmxMorph morph = new PmxMorph();
				action(delegate
				{
					morph.FromStreamEx(s, head.ElementFormat);
				});
				MorphList.Add(morph);
			}
			action(delegate
			{
				count = PmxStreamHelper.ReadElement_Int32(s);
			});
			NodeList.Clear();
			NodeList.Capacity = count;
			for (int num2 = 0; num2 < count; num2++)
			{
				PmxNode node = new PmxNode();
				action(delegate
				{
					node.FromStreamEx(s, head.ElementFormat);
				});
				NodeList.Add(node);
				if (NodeList[num2].SystemNode)
				{
					if (NodeList[num2].Name == "Root")
					{
						RootNode = NodeList[num2];
					}
					else if (NodeList[num2].Name == "表情")
					{
						ExpNode = NodeList[num2];
					}
				}
			}
			action(delegate
			{
				count = PmxStreamHelper.ReadElement_Int32(s);
			});
			BodyList.Clear();
			BodyList.Capacity = count;
			for (int num3 = 0; num3 < count; num3++)
			{
				PmxBody b2 = new PmxBody();
				action(delegate
				{
					b2.FromStreamEx(s, head.ElementFormat);
				});
				BodyList.Add(b2);
			}
			action(delegate
			{
				count = PmxStreamHelper.ReadElement_Int32(s);
			});
			JointList.Clear();
			JointList.Capacity = count;
			for (int num4 = 0; num4 < count; num4++)
			{
				PmxJoint i = new PmxJoint();
				action(delegate
				{
					i.FromStreamEx(s, head.ElementFormat);
				});
				JointList.Add(i);
			}
			if (head.Ver >= 2.1f)
			{
				action(delegate
				{
					count = PmxStreamHelper.ReadElement_Int32(s);
				});
				SoftBodyList.Clear();
				SoftBodyList.Capacity = count;
				for (int num5 = 0; num5 < count; num5++)
				{
					PmxSoftBody b3 = new PmxSoftBody();
					action(delegate
					{
						b3.FromStreamEx(s, head.ElementFormat);
					});
					SoftBodyList.Add(b3);
				}
			}
			if (id)
			{
				action(delegate
				{
					FilePath = PmxStreamHelper.ReadString(s, head.ElementFormat);
				});
			}
			head.ElementFormat.WithID = false;
		}

		public void UpdateElementFormatSize(PmxElementFormat f = null, PmxTextureTable tx = null)
		{
			if (f == null)
			{
				f = Header.ElementFormat;
			}
			f.VertexSize = PmxElementFormat.GetUnsignedBufSize(VertexList.Count);
			f.MaterialSize = PmxElementFormat.GetSignedBufSize(MaterialList.Count);
			f.BoneSize = PmxElementFormat.GetSignedBufSize(BoneList.Count);
			f.MorphSize = PmxElementFormat.GetSignedBufSize(MorphList.Count);
			f.BodySize = PmxElementFormat.GetSignedBufSize(BodyList.Count);
			if (tx == null)
			{
				tx = new PmxTextureTable(MaterialList);
			}
			f.TexSize = PmxElementFormat.GetSignedBufSize(tx.Count);
		}

		public virtual void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			bool id = false;
			if (f != null && f.WithID)
			{
				id = true;
			}
			ToStream(s, id);
		}

		public virtual void ToStream(Stream s, bool id)
		{
			PmxHeader header = Header;
			header.ElementFormat.WithID = id;
			PmxTextureTable pmxTextureTable = new PmxTextureTable(MaterialList);
			UpdateElementFormatSize(header.ElementFormat, pmxTextureTable);
			header.ToStreamEx(s);
			ModelInfo.ToStreamEx(s, header.ElementFormat);
			PmxStreamHelper.WriteElement_Int32(s, VertexList.Count);
			for (int i = 0; i < VertexList.Count; i++)
			{
				VertexList[i].ToStreamEx(s, header.ElementFormat);
			}
			PmxStreamHelper.WriteElement_Int32(s, FaceList.Count);
			for (int j = 0; j < FaceList.Count; j++)
			{
				PmxStreamHelper.WriteElement_Int32(s, FaceList[j], header.ElementFormat.VertexSize, signed: false);
			}
			pmxTextureTable.ToStreamEx(s, header.ElementFormat);
			PmxStreamHelper.WriteElement_Int32(s, MaterialList.Count);
			for (int k = 0; k < MaterialList.Count; k++)
			{
				MaterialList[k].ToStreamEx_TexTable(s, pmxTextureTable, header.ElementFormat);
			}
			PmxStreamHelper.WriteElement_Int32(s, BoneList.Count);
			for (int l = 0; l < BoneList.Count; l++)
			{
				BoneList[l].ToStreamEx(s, header.ElementFormat);
			}
			if (header.Ver < 2.1f)
			{
				int num = MorphList.Where((PmxMorph mp) => mp.IsImpulse).Count();
				PmxStreamHelper.WriteElement_Int32(s, MorphList.Count - num);
			}
			else
			{
				PmxStreamHelper.WriteElement_Int32(s, MorphList.Count);
			}
			for (int m = 0; m < MorphList.Count; m++)
			{
				MorphList[m].ToStreamEx(s, header.ElementFormat);
			}
			PmxStreamHelper.WriteElement_Int32(s, NodeList.Count);
			for (int n = 0; n < NodeList.Count; n++)
			{
				NodeList[n].ToStreamEx(s, header.ElementFormat);
			}
			PmxStreamHelper.WriteElement_Int32(s, BodyList.Count);
			for (int num2 = 0; num2 < BodyList.Count; num2++)
			{
				BodyList[num2].ToStreamEx(s, header.ElementFormat);
			}
			PmxStreamHelper.WriteElement_Int32(s, JointList.Count);
			for (int num3 = 0; num3 < JointList.Count; num3++)
			{
				JointList[num3].ToStreamEx(s, header.ElementFormat);
			}
			if (header.Ver >= 2.1f)
			{
				PmxStreamHelper.WriteElement_Int32(s, SoftBodyList.Count);
				for (int num4 = 0; num4 < SoftBodyList.Count; num4++)
				{
					SoftBodyList[num4].ToStreamEx(s, header.ElementFormat);
				}
			}
			if (id)
			{
				PmxStreamHelper.WriteString(s, FilePath, header.ElementFormat);
			}
			header.ElementFormat.WithID = false;
		}

		public void ClearMaterialNames()
		{
			for (int i = 0; i < MaterialList.Count; i++)
			{
				MaterialList[i].Name = "材質" + (i + 1);
			}
		}

		public static void UpdateBoneIKKind(List<PmxBone> boneList)
		{
			for (int i = 0; i < boneList.Count; i++)
			{
				boneList[i].IKKind = PmxBone.IKKindType.None;
			}
			for (int j = 0; j < boneList.Count; j++)
			{
				PmxBone pmxBone = boneList[j];
				if (!pmxBone.GetFlag(PmxBone.BoneFlags.IK))
				{
					continue;
				}
				pmxBone.IKKind = PmxBone.IKKindType.IK;
				int target = pmxBone.IK.Target;
				if (CP.InRange(boneList, target))
				{
					boneList[target].IKKind = PmxBone.IKKindType.Target;
				}
				for (int k = 0; k < pmxBone.IK.LinkList.Count; k++)
				{
					int bone = pmxBone.IK.LinkList[k].Bone;
					if (CP.InRange(boneList, bone))
					{
						boneList[bone].IKKind = PmxBone.IKKindType.Link;
					}
				}
			}
		}

		public void UpdateBoneIKKind()
		{
			UpdateBoneIKKind(BoneList);
		}

		public void NormalizeVertex_SDEF_C0()
		{
			for (int i = 0; i < VertexList.Count; i++)
			{
				VertexList[i].NormalizeSDEF_C0(BoneList);
			}
		}

		public float RequireVersion(out bool isQDEF, out bool isExMorph, out bool isExJoint, out bool isSoftBody)
		{
			Func<bool> func = delegate
			{
				bool result4 = false;
				for (int k = 0; k < VertexList.Count; k++)
				{
					if (VertexList[k].Deform == PmxVertex.DeformType.QDEF)
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
				for (int j = 0; j < MorphList.Count; j++)
				{
					PmxMorph pmxMorph = MorphList[j];
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
				for (int i = 0; i < JointList.Count; i++)
				{
					if (JointList[i].Kind != 0)
					{
						result2 = true;
						break;
					}
				}
				return result2;
			};
			Func<bool> func4 = () => SoftBodyList.Count > 0;
			isQDEF = func();
			isExMorph = func2();
			isExJoint = func3();
			isSoftBody = func4();
			float result = 2f;
			if (isQDEF | isExMorph | isExJoint | isSoftBody)
			{
				result = 2.1f;
			}
			return result;
		}

		private void NormalizeVersion()
		{
			bool isQDEF;
			bool isExMorph;
			bool isExJoint;
			bool isSoftBody;
			float ver = RequireVersion(out isQDEF, out isExMorph, out isExJoint, out isSoftBody);
			switch (SaveVersion)
			{
				case PmxSaveVersion.AutoSelect:
					Header.Ver = ver;
					break;
				case PmxSaveVersion.PMX2_0:
					Header.Ver = 2f;
					if (isQDEF || isExMorph || isExJoint || isSoftBody)
						Header.Ver = 2.1f;
					break;
				case PmxSaveVersion.PMX2_1:
					Header.Ver = 2.1f;
					break;
			}
		}

		public void NormalizeUVACount()
		{
			if (VertexList.Count <= 0)
			{
				Header.ElementFormat.UVACount = 0;
				return;
			}
			Func<Vector4, bool> func = (Vector4 v) => Math.Abs(v.X) > 1E-12f || Math.Abs(v.Y) > 1E-12f || Math.Abs(v.Z) > 1E-12f || Math.Abs(v.W) > 1E-12f;
			int num = 0;
			foreach (PmxVertex vertex in VertexList)
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
			Header.ElementFormat.UVACount = num;
		}

		public void NormalizeCID()
		{
			PmxIDObject.NormalizeCID(VertexList);
			PmxIDObject.NormalizeCID(MaterialList);
			PmxIDObject.NormalizeCID(BoneList);
			PmxIDObject.NormalizeCID(MorphList);
			PmxIDObject.NormalizeCID(NodeList);
			PmxIDObject.NormalizeCID(BodyList);
			PmxIDObject.NormalizeCID(JointList);
			PmxIDObject.NormalizeCID(SoftBodyList);
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
