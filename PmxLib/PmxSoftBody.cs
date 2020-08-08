using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	public class PmxSoftBody : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
	{
		public enum ShapeKind
		{
			TriMesh,
			Rope
		}

		[Flags]
		public enum SoftBodyFlags
		{
			GenerateBendingLinks = 1,
			GenerateClusters = 2,
			RandomizeConstraints = 4
		}

		public struct SoftBodyConfig
		{
			public int AeroModel;

			public float VCF;

			public float DP;

			public float DG;

			public float LF;

			public float PR;

			public float VC;

			public float DF;

			public float MT;

			public float CHR;

			public float KHR;

			public float SHR;

			public float AHR;

			public float SRHR_CL;

			public float SKHR_CL;

			public float SSHR_CL;

			public float SR_SPLT_CL;

			public float SK_SPLT_CL;

			public float SS_SPLT_CL;

			public int V_IT;

			public int P_IT;

			public int D_IT;

			public int C_IT;

			public void Clear()
			{
				this.AeroModel = 0;
				this.VCF = 1f;
				this.DP = 0f;
				this.DG = 0f;
				this.LF = 0f;
				this.PR = 0f;
				this.VC = 0f;
				this.DF = 0.2f;
				this.MT = 0f;
				this.CHR = 1f;
				this.KHR = 0.1f;
				this.SHR = 1f;
				this.AHR = 0.7f;
				this.SRHR_CL = 0.1f;
				this.SKHR_CL = 1f;
				this.SSHR_CL = 0.5f;
				this.SR_SPLT_CL = 0.5f;
				this.SK_SPLT_CL = 0.5f;
				this.SS_SPLT_CL = 0.5f;
				this.V_IT = 0;
				this.P_IT = 1;
				this.D_IT = 0;
				this.C_IT = 4;
			}
		}

		public struct SoftBodyMaterialConfig
		{
			public float LST;

			public float AST;

			public float VST;

			public void Clear()
			{
				this.LST = 1f;
				this.AST = 1f;
				this.VST = 1f;
			}
		}

		public class BodyAnchor : IPmxObjectKey, ICloneable
		{
			public int Body;

			public int Vertex;

			public int NodeIndex;

			public bool IsNear;

			public PmxBody RefBody
			{
				get;
				set;
			}

			public PmxVertex RefVertex
			{
				get;
				set;
			}

			public PmxObject ObjectKey
			{
				get
				{
					return PmxObject.SoftBodyAnchor;
				}
			}

			public BodyAnchor()
			{
				this.NodeIndex = -1;
				this.IsNear = false;
			}

			public BodyAnchor(BodyAnchor ac)
			{
				this.Body = ac.Body;
				this.Vertex = ac.Vertex;
				this.NodeIndex = ac.NodeIndex;
				this.IsNear = ac.IsNear;
			}

			public object Clone()
			{
				return new BodyAnchor(this);
			}
		}

		public class VertexPin : IPmxObjectKey, ICloneable
		{
			public int Vertex;

			public int NodeIndex;

			public PmxVertex RefVertex
			{
				get;
				set;
			}

			public PmxObject ObjectKey
			{
				get
				{
					return PmxObject.SoftBodyPinVertex;
				}
			}

			public VertexPin()
			{
				this.NodeIndex = -1;
			}

			public VertexPin(VertexPin pin)
			{
				this.Vertex = pin.Vertex;
				this.NodeIndex = pin.NodeIndex;
			}

			public object Clone()
			{
				return new VertexPin(this);
			}
		}

		public ShapeKind Shape;

		public int Material;

		public int Group;

		public PmxBodyPassGroup PassGroup;

		public SoftBodyFlags Flags;

		public int BendingLinkDistance;

		public int ClusterCount;

		public float TotalMass;

		public float Margin;

		public SoftBodyConfig Config;

		public SoftBodyMaterialConfig MaterialConfig;

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.SoftBody;
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

		public PmxMaterial RefMaterial
		{
			get;
			set;
		}

		public bool IsGenerateBendingLinks
		{
			get
			{
				return (this.Flags & SoftBodyFlags.GenerateBendingLinks) > (SoftBodyFlags)0;
			}
			set
			{
				if (value)
				{
					this.Flags |= SoftBodyFlags.GenerateBendingLinks;
				}
				else
				{
					this.Flags &= ~SoftBodyFlags.GenerateBendingLinks;
				}
			}
		}

		public bool IsGenerateClusters
		{
			get
			{
				return (this.Flags & SoftBodyFlags.GenerateClusters) > (SoftBodyFlags)0;
			}
			set
			{
				if (value)
				{
					this.Flags |= SoftBodyFlags.GenerateClusters;
				}
				else
				{
					this.Flags &= ~SoftBodyFlags.GenerateClusters;
				}
			}
		}

		public bool IsRandomizeConstraints
		{
			get
			{
				return (this.Flags & SoftBodyFlags.RandomizeConstraints) > (SoftBodyFlags)0;
			}
			set
			{
				if (value)
				{
					this.Flags |= SoftBodyFlags.RandomizeConstraints;
				}
				else
				{
					this.Flags &= ~SoftBodyFlags.RandomizeConstraints;
				}
			}
		}

		public List<BodyAnchor> BodyAnchorList
		{
			get;
			private set;
		}

		public List<VertexPin> VertexPinList
		{
			get;
			private set;
		}

		public int[] VertexIndices
		{
			get;
			set;
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

		public void NormalizeBodyAnchorList()
		{
			if (this.BodyAnchorList.Count > 0)
			{
				List<int> list = new List<int>(this.BodyAnchorList.Count);
				Dictionary<string, int> dictionary = new Dictionary<string, int>(this.BodyAnchorList.Count);
				for (int i = 0; i < this.BodyAnchorList.Count; i++)
				{
					BodyAnchor bodyAnchor = this.BodyAnchorList[i];
					string key = bodyAnchor.Body.ToString() + "_" + bodyAnchor.Vertex.ToString();
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, i);
					}
					else
					{
						list.Add(i);
					}
				}
				if (list.Count > 0)
				{
					int[] array = CP.SortIndexForRemove(list.ToArray());
					for (int j = 0; j < array.Length; j++)
					{
						this.BodyAnchorList.RemoveAt(array[j]);
					}
				}
			}
		}

		public void SetVertexPinFromText(string text)
		{
			this.VertexPinList.Clear();
			string[] array = text.Split(',');
			if (array != null)
			{
				this.VertexPinList.Capacity = array.Length;
				for (int i = 0; i < array.Length; i++)
				{
					int vertex = default(int);
					if (!string.IsNullOrEmpty(array[i]) && int.TryParse(array[i].Trim(), out vertex))
					{
						this.VertexPinList.Add(new VertexPin
						{
							Vertex = vertex
						});
					}
				}
			}
		}

		public void SortVertexPinList()
		{
			if (this.VertexPinList.Count > 0)
			{
				List<int> list = new List<int>(this.VertexPinList.Count);
				for (int i = 0; i < this.VertexPinList.Count; i++)
				{
					list.Add(this.VertexPinList[i].Vertex);
				}
				list.Sort();
				for (int j = 0; j < this.VertexPinList.Count; j++)
				{
					VertexPin vertexPin = this.VertexPinList[j];
					vertexPin.Vertex = list[j];
					vertexPin.NodeIndex = -1;
					vertexPin.RefVertex = null;
				}
			}
		}

		public void NormalizeVertexPinList()
		{
			if (this.VertexPinList.Count > 0)
			{
				this.SortVertexPinList();
				bool[] array = new bool[this.VertexPinList.Count];
				array[0] = false;
				for (int i = 1; i < this.VertexPinList.Count; i++)
				{
					VertexPin vertexPin = this.VertexPinList[i - 1];
					VertexPin vertexPin2 = this.VertexPinList[i];
					if (vertexPin.Vertex == vertexPin2.Vertex)
					{
						array[i] = true;
					}
				}
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				foreach (BodyAnchor bodyAnchor in this.BodyAnchorList)
				{
					dictionary.Add(bodyAnchor.Vertex, 0);
				}
				for (int j = 0; j < this.VertexPinList.Count; j++)
				{
					int vertex = this.VertexPinList[j].Vertex;
					if (dictionary.ContainsKey(vertex))
					{
						array[j] = true;
					}
				}
				for (int num = array.Length - 1; num > 0; num--)
				{
					if (array[num])
					{
						this.VertexPinList.RemoveAt(num);
					}
				}
			}
		}

		public PmxSoftBody()
		{
			this.Name = "";
			this.NameE = "";
			this.Shape = ShapeKind.TriMesh;
			this.Material = -1;
			this.Group = 0;
			this.PassGroup = new PmxBodyPassGroup();
			this.InitializeParameter();
			this.BodyAnchorList = new List<BodyAnchor>();
			this.VertexPinList = new List<VertexPin>();
			this.VertexIndices = new int[0];
		}

		public PmxSoftBody(PmxSoftBody sbody, bool nonStr = false)
		{
			this.FromPmxSoftBody(sbody, nonStr);
		}

		public void InitializeParameter()
		{
			this.ClearGenerate();
			this.TotalMass = 1f;
			this.Margin = 0.05f;
			this.Config.Clear();
			this.MaterialConfig.Clear();
		}

		public void ClearGenerate()
		{
			this.IsGenerateBendingLinks = true;
			this.IsGenerateClusters = false;
			this.IsRandomizeConstraints = true;
			this.BendingLinkDistance = 2;
			this.ClusterCount = 0;
		}

		public void FromPmxSoftBody(PmxSoftBody sbody, bool nonStr = false)
		{
			if (!nonStr)
			{
				this.Name = sbody.Name;
				this.NameE = sbody.NameE;
			}
			this.Shape = sbody.Shape;
			this.Material = sbody.Material;
			this.Group = sbody.Group;
			this.PassGroup = sbody.PassGroup.Clone();
			this.IsGenerateBendingLinks = sbody.IsGenerateBendingLinks;
			this.IsGenerateClusters = sbody.IsGenerateClusters;
			this.IsRandomizeConstraints = sbody.IsRandomizeConstraints;
			this.BendingLinkDistance = sbody.BendingLinkDistance;
			this.ClusterCount = sbody.ClusterCount;
			this.TotalMass = sbody.TotalMass;
			this.Margin = sbody.Margin;
			this.Config = sbody.Config;
			this.MaterialConfig = sbody.MaterialConfig;
			this.BodyAnchorList = CP.CloneList(sbody.BodyAnchorList);
			this.VertexPinList = CP.CloneList(sbody.VertexPinList);
			this.VertexIndices = CP.CloneArray_ValueType(sbody.VertexIndices);
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			this.Name = PmxStreamHelper.ReadString(s, f);
			this.NameE = PmxStreamHelper.ReadString(s, f);
			this.Shape = (ShapeKind)PmxStreamHelper.ReadElement_Int32(s, 1, true);
			this.Material = PmxStreamHelper.ReadElement_Int32(s, f.MaterialSize, true);
			this.Group = PmxStreamHelper.ReadElement_Int32(s, 1, true);
			ushort bits = (ushort)PmxStreamHelper.ReadElement_Int32(s, 2, false);
			this.PassGroup.FromFlagBits(bits);
			this.Flags = (SoftBodyFlags)PmxStreamHelper.ReadElement_Int32(s, 1, true);
			this.BendingLinkDistance = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.ClusterCount = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.TotalMass = PmxStreamHelper.ReadElement_Float(s);
			this.Margin = PmxStreamHelper.ReadElement_Float(s);
			this.Config.AeroModel = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.Config.VCF = PmxStreamHelper.ReadElement_Float(s);
			this.Config.DP = PmxStreamHelper.ReadElement_Float(s);
			this.Config.DG = PmxStreamHelper.ReadElement_Float(s);
			this.Config.LF = PmxStreamHelper.ReadElement_Float(s);
			this.Config.PR = PmxStreamHelper.ReadElement_Float(s);
			this.Config.VC = PmxStreamHelper.ReadElement_Float(s);
			this.Config.DF = PmxStreamHelper.ReadElement_Float(s);
			this.Config.MT = PmxStreamHelper.ReadElement_Float(s);
			this.Config.CHR = PmxStreamHelper.ReadElement_Float(s);
			this.Config.KHR = PmxStreamHelper.ReadElement_Float(s);
			this.Config.SHR = PmxStreamHelper.ReadElement_Float(s);
			this.Config.AHR = PmxStreamHelper.ReadElement_Float(s);
			this.Config.SRHR_CL = PmxStreamHelper.ReadElement_Float(s);
			this.Config.SKHR_CL = PmxStreamHelper.ReadElement_Float(s);
			this.Config.SSHR_CL = PmxStreamHelper.ReadElement_Float(s);
			this.Config.SR_SPLT_CL = PmxStreamHelper.ReadElement_Float(s);
			this.Config.SK_SPLT_CL = PmxStreamHelper.ReadElement_Float(s);
			this.Config.SS_SPLT_CL = PmxStreamHelper.ReadElement_Float(s);
			this.Config.V_IT = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.Config.P_IT = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.Config.D_IT = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.Config.C_IT = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.MaterialConfig.LST = PmxStreamHelper.ReadElement_Float(s);
			this.MaterialConfig.AST = PmxStreamHelper.ReadElement_Float(s);
			this.MaterialConfig.VST = PmxStreamHelper.ReadElement_Float(s);
			int num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.BodyAnchorList.Clear();
			this.BodyAnchorList.Capacity = num;
			for (int i = 0; i < num; i++)
			{
				int body = PmxStreamHelper.ReadElement_Int32(s, f.BodySize, true);
				int vertex = PmxStreamHelper.ReadElement_Int32(s, f.VertexSize, true);
				int num2 = PmxStreamHelper.ReadElement_Int32(s, 1, true);
				this.BodyAnchorList.Add(new BodyAnchor
				{
					Body = body,
					Vertex = vertex,
					IsNear = (num2 != 0)
				});
			}
			num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.VertexPinList.Clear();
			this.VertexPinList.Capacity = num;
			for (int j = 0; j < num; j++)
			{
				int vertex2 = PmxStreamHelper.ReadElement_Int32(s, f.VertexSize, true);
				this.VertexPinList.Add(new VertexPin
				{
					Vertex = vertex2
				});
			}
			this.NormalizeBodyAnchorList();
			this.NormalizeVertexPinList();
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteString(s, this.Name, f);
			PmxStreamHelper.WriteString(s, this.NameE, f);
			PmxStreamHelper.WriteElement_Int32(s, (int)this.Shape, 1, true);
			PmxStreamHelper.WriteElement_Int32(s, this.Material, f.MaterialSize, true);
			PmxStreamHelper.WriteElement_Int32(s, this.Group, 1, true);
			PmxStreamHelper.WriteElement_Int32(s, this.PassGroup.ToFlagBits(), 2, false);
			PmxStreamHelper.WriteElement_Int32(s, (int)this.Flags, 1, false);
			PmxStreamHelper.WriteElement_Int32(s, this.BendingLinkDistance, 4, true);
			PmxStreamHelper.WriteElement_Int32(s, this.ClusterCount, 4, true);
			PmxStreamHelper.WriteElement_Float(s, this.TotalMass);
			PmxStreamHelper.WriteElement_Float(s, this.Margin);
			PmxStreamHelper.WriteElement_Int32(s, this.Config.AeroModel, 4, true);
			PmxStreamHelper.WriteElement_Float(s, this.Config.VCF);
			PmxStreamHelper.WriteElement_Float(s, this.Config.DP);
			PmxStreamHelper.WriteElement_Float(s, this.Config.DG);
			PmxStreamHelper.WriteElement_Float(s, this.Config.LF);
			PmxStreamHelper.WriteElement_Float(s, this.Config.PR);
			PmxStreamHelper.WriteElement_Float(s, this.Config.VC);
			PmxStreamHelper.WriteElement_Float(s, this.Config.DF);
			PmxStreamHelper.WriteElement_Float(s, this.Config.MT);
			PmxStreamHelper.WriteElement_Float(s, this.Config.CHR);
			PmxStreamHelper.WriteElement_Float(s, this.Config.KHR);
			PmxStreamHelper.WriteElement_Float(s, this.Config.SHR);
			PmxStreamHelper.WriteElement_Float(s, this.Config.AHR);
			PmxStreamHelper.WriteElement_Float(s, this.Config.SRHR_CL);
			PmxStreamHelper.WriteElement_Float(s, this.Config.SKHR_CL);
			PmxStreamHelper.WriteElement_Float(s, this.Config.SSHR_CL);
			PmxStreamHelper.WriteElement_Float(s, this.Config.SR_SPLT_CL);
			PmxStreamHelper.WriteElement_Float(s, this.Config.SK_SPLT_CL);
			PmxStreamHelper.WriteElement_Float(s, this.Config.SS_SPLT_CL);
			PmxStreamHelper.WriteElement_Int32(s, this.Config.V_IT, 4, true);
			PmxStreamHelper.WriteElement_Int32(s, this.Config.P_IT, 4, true);
			PmxStreamHelper.WriteElement_Int32(s, this.Config.D_IT, 4, true);
			PmxStreamHelper.WriteElement_Int32(s, this.Config.C_IT, 4, true);
			PmxStreamHelper.WriteElement_Float(s, this.MaterialConfig.LST);
			PmxStreamHelper.WriteElement_Float(s, this.MaterialConfig.AST);
			PmxStreamHelper.WriteElement_Float(s, this.MaterialConfig.VST);
			PmxStreamHelper.WriteElement_Int32(s, this.BodyAnchorList.Count, 4, true);
			for (int i = 0; i < this.BodyAnchorList.Count; i++)
			{
				PmxStreamHelper.WriteElement_Int32(s, this.BodyAnchorList[i].Body, f.BodySize, true);
				PmxStreamHelper.WriteElement_Int32(s, this.BodyAnchorList[i].Vertex, f.VertexSize, false);
				PmxStreamHelper.WriteElement_Int32(s, this.BodyAnchorList[i].IsNear ? 1 : 0, 1, true);
			}
			PmxStreamHelper.WriteElement_Int32(s, this.VertexPinList.Count, 4, true);
			for (int j = 0; j < this.VertexPinList.Count; j++)
			{
				PmxStreamHelper.WriteElement_Int32(s, this.VertexPinList[j].Vertex, f.VertexSize, false);
			}
		}

		object ICloneable.Clone()
		{
			return new PmxSoftBody(this, false);
		}

		public PmxSoftBody Clone()
		{
			return new PmxSoftBody(this, false);
		}
	}
}
