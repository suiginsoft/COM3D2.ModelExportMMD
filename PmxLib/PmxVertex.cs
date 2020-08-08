using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	public class PmxVertex : IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public struct BoneWeight
		{
			public int Bone;

			public float Value;

			public PmxBone RefBone
			{
				get;
				set;
			}

			public static BoneWeight[] Sort(BoneWeight[] w)
			{
				List<BoneWeight> list = new List<BoneWeight>(w);
				BoneWeight.SortList(list);
				return list.ToArray();
			}

			public static void SortList(List<BoneWeight> list)
			{
				CP.SSort(list, delegate(BoneWeight l, BoneWeight r)
				{
					float num = Math.Abs(r.Value) - Math.Abs(l.Value);
					if (num < 0f)
					{
						return -1;
					}
					if (num <= 0f)
					{
						return 0;
					}
					return 1;
				});
			}
		}

		public enum DeformType
		{
			BDEF1,
			BDEF2,
			BDEF4,
			SDEF,
			QDEF
		}

		public const int MaxUVACount = 4;

		public const int MaxWeightBoneCount = 4;

		public Vector3 Position;

		public Vector3 Normal;

		public Vector2 UV;

		public Vector4[] UVA;

		public BoneWeight[] Weight;

		public DeformType Deform;

		public float EdgeScale = 1f;

		public int VertexMorphIndex = -1;

		public int UVMorphIndex = -1;

		public int[] UVAMorphIndex;

		public int SDEFIndex = -1;

		public int QDEFIndex = -1;

		public int SoftBodyPosIndex = -1;

		public int SoftBodyNormalIndex = -1;

		public bool SDEF;

		public Vector3 C0;

		public Vector3 R0;

		public Vector3 R1;

		public Vector3 RW0;

		public Vector3 RW1;

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.Vertex;
			}
		}

		public int NWeight
		{
			get
			{
				return (int)((this.Weight[0].Value + 0.005f) * 100f);
			}
			set
			{
				this.ClearWeightValue();
				this.Weight[0].Value = (float)value * 0.01f;
				this.Weight[1].Value = 1f - this.Weight[0].Value;
				this.UpdateDeformType();
				if (this.Deform == DeformType.BDEF4 || this.Deform == DeformType.QDEF)
				{
					this.Deform = DeformType.BDEF2;
				}
			}
		}

		public PmxVertex()
		{
			this.UVAMorphIndex = new int[4];
			this.Weight = new BoneWeight[4];
			this.UVA = new Vector4[4];
			this.VertexMorphIndex = -1;
			this.UVMorphIndex = -1;
			for (int i = 0; i < this.UVAMorphIndex.Length; i++)
			{
				this.UVAMorphIndex[i] = -1;
			}
			this.SDEFIndex = -1;
			this.QDEFIndex = -1;
			this.SoftBodyPosIndex = -1;
			this.SoftBodyNormalIndex = -1;
			this.ClearWeight();
		}

		public void ClearWeight()
		{
			this.ClearWeightBone();
			this.ClearWeightValue();
			this.Deform = DeformType.BDEF1;
			this.SDEF = false;
		}

		public void ClearWeightBone()
		{
			this.Weight[0].Bone = 0;
			for (int i = 1; i < 4; i++)
			{
				this.Weight[i].Bone = -1;
			}
		}

		public void ClearWeightValue()
		{
			this.Weight[0].Value = 1f;
			for (int i = 1; i < 4; i++)
			{
				this.Weight[i].Value = 0f;
			}
		}

		public void NormalizeWeight(bool bdef4Sum = false)
		{
			for (int i = 0; i < 4; i++)
			{
				if (this.Weight[i].Bone < 0)
				{
					this.Weight[i].Value = 0f;
				}
			}
			if (this.Deform == DeformType.SDEF)
			{
				this.NormalizeWeightOrder_SDEF();
			}
			else
			{
				this.NormalizeWeightOrder();
			}
			this.UpdateDeformType();
			if (this.Deform == DeformType.SDEF)
			{
				this.Weight[2].Bone = -1;
				this.Weight[2].Value = 0f;
				this.Weight[3].Bone = -1;
				this.Weight[3].Value = 0f;
			}
			this.NormalizeWeightSum(bdef4Sum);
			int num = 1;
			switch (this.Deform)
			{
			case DeformType.BDEF2:
			case DeformType.SDEF:
				num = 2;
				break;
			case DeformType.BDEF4:
			case DeformType.QDEF:
				num = 4;
				break;
			}
			for (int j = 0; j < num; j++)
			{
				if (this.Weight[j].Bone < 0)
				{
					this.Weight[j].Bone = 0;
					this.Weight[j].Value = 0f;
				}
			}
			this.UpdateDeformType();
		}

		public void NormalizeWeightOrder()
		{
			BoneWeight[] array = BoneWeight.Sort(this.Weight);
			for (int i = 0; i < array.Length; i++)
			{
				this.Weight[i] = array[i];
			}
		}

		public void NormalizeWeightOrder_BDEF2()
		{
			if (this.Weight[0].Value < this.Weight[1].Value)
			{
				CP.Swap<BoneWeight>(ref this.Weight[0], ref this.Weight[1]);
			}
		}

		public void NormalizeWeightOrder_SDEF()
		{
			if (this.Weight[0].Bone > this.Weight[1].Bone)
			{
				CP.Swap<BoneWeight>(ref this.Weight[0], ref this.Weight[1]);
				CP.Swap<Vector3>(ref this.R0, ref this.R1);
				CP.Swap<Vector3>(ref this.RW0, ref this.RW0);
			}
		}

		public void NormalizeWeightSum(bool bdef4 = false)
		{
			if (bdef4 || (this.Deform != DeformType.BDEF4 && this.Deform != DeformType.QDEF))
			{
				float num = 0f;
				for (int i = 0; i < 4; i++)
				{
					num += this.Weight[i].Value;
				}
				if (num != 0f && num != 1f)
				{
					float num2 = 1f / num;
					for (int j = 0; j < 4; j++)
					{
						BoneWeight[] weight = this.Weight;
						int num3 = j;
						weight[num3].Value = weight[num3].Value * num2;
					}
				}
			}
		}

		public void NormalizeWeightSum_BDEF2()
		{
			float num = this.Weight[0].Value + this.Weight[1].Value;
			if (num != 1f)
			{
				if (num == 0f)
				{
					this.Weight[0].Value = 1f;
					this.Weight[1].Value = 0f;
				}
				else
				{
					float num2 = 1f / num;
					BoneWeight[] weight = this.Weight;
					int num3 = 0;
					weight[num3].Value = weight[num3].Value * num2;
					BoneWeight[] weight2 = this.Weight;
					int num4 = 1;
					weight2[num4].Value = weight2[num4].Value * num2;
				}
			}
		}

		public DeformType GetDeformType()
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				if (this.Weight[i].Value != 0f)
				{
					num++;
				}
			}
			if (this.SDEF && num != 1)
			{
				return DeformType.SDEF;
			}
			if (this.Deform == DeformType.QDEF && num != 1)
			{
				return DeformType.QDEF;
			}
			DeformType result = DeformType.BDEF1;
			switch (num)
			{
			case 0:
			case 1:
				result = DeformType.BDEF1;
				break;
			case 2:
				result = DeformType.BDEF2;
				break;
			case 3:
			case 4:
				result = DeformType.BDEF4;
				break;
			}
			return result;
		}

		public void UpdateDeformType()
		{
			this.Deform = this.GetDeformType();
		}

		public void SetSDEF_RV(Vector3 r0, Vector3 r1)
		{
			this.R0 = r0;
			this.R1 = r1;
			this.CalcSDEF_RW();
		}

		public void CalcSDEF_RW()
		{
			Vector3 b = this.Weight[0].Value * this.R0 + this.Weight[1].Value * this.R1;
			this.RW0 = this.R0 - b;
			this.RW1 = this.R1 - b;
		}

		public bool NormalizeSDEF_C0(List<PmxBone> boneList)
		{
			if (this.Deform != DeformType.SDEF)
			{
				return true;
			}
			int bone = this.Weight[0].Bone;
			int bone2 = this.Weight[1].Bone;
			if (!CP.InRange(boneList, bone))
			{
				return false;
			}
			PmxBone pmxBone = boneList[bone];
			if (CP.InRange(boneList, bone2))
			{
				PmxBone pmxBone2 = boneList[bone2];
				Vector3 position = pmxBone.Position;
				Vector3 position2 = pmxBone2.Position;
				Vector3 vector = position2 - position;
				Vector3 vector2 = vector;
				vector2.Normalize();
				Vector3 position3 = this.Position;
				position3 -= position;
				float d = Vector3.Dot(vector2, position3);
				this.C0 = vector2 * d + position;
				return true;
			}
			return false;
		}

		public bool IsSDEF_EnableBone(List<PmxBone> boneList)
		{
			int bone = this.Weight[0].Bone;
			int bone2 = this.Weight[1].Bone;
			if (!CP.InRange(boneList, bone))
			{
				return false;
			}
			PmxBone pmxBone = boneList[bone];
			if (CP.InRange(boneList, bone2))
			{
				PmxBone pmxBone2 = boneList[bone2];
				return pmxBone.Parent == bone2 || pmxBone2.Parent == bone;
			}
			return false;
		}

		public PmxVertex(PmxVertex vertex)
			: this()
		{
			this.FromPmxVertex(vertex);
		}

		public void FromPmxVertex(PmxVertex vertex)
		{
			this.Position = vertex.Position;
			this.Normal = vertex.Normal;
			this.UV = vertex.UV;
			for (int i = 0; i < 4; i++)
			{
				this.UVA[i] = vertex.UVA[i];
			}
			for (int j = 0; j < 4; j++)
			{
				this.Weight[j] = vertex.Weight[j];
				this.Weight[j].RefBone = null;
			}
			this.EdgeScale = vertex.EdgeScale;
			this.Deform = vertex.Deform;
			this.SDEF = vertex.SDEF;
			this.C0 = vertex.C0;
			this.R0 = vertex.R0;
			this.R1 = vertex.R1;
			this.RW0 = vertex.RW0;
			this.RW1 = vertex.RW1;
			this.VertexMorphIndex = vertex.VertexMorphIndex;
			this.UVMorphIndex = vertex.UVMorphIndex;
			for (int k = 0; k < this.UVAMorphIndex.Length; k++)
			{
				this.UVAMorphIndex[k] = vertex.UVAMorphIndex[k];
			}
			this.SDEFIndex = vertex.SDEFIndex;
			this.QDEFIndex = vertex.QDEFIndex;
			this.SoftBodyPosIndex = vertex.SoftBodyPosIndex;
			this.SoftBodyNormalIndex = vertex.SoftBodyNormalIndex;
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			this.Position = V3_BytesConvert.FromStream(s);
			this.Normal = V3_BytesConvert.FromStream(s);
			this.UV = V2_BytesConvert.FromStream(s);
			for (int i = 0; i < f.UVACount; i++)
			{
				Vector4 vector = V4_BytesConvert.FromStream(s);
				if (0 <= i && i < this.UVA.Length)
				{
					this.UVA[i] = vector;
				}
			}
			this.Deform = (DeformType)s.ReadByte();
			this.SDEF = false;
			switch (this.Deform)
			{
			case DeformType.BDEF1:
				this.Weight[0].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
				this.Weight[0].Value = 1f;
				break;
			case DeformType.BDEF2:
				this.Weight[0].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
				this.Weight[1].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
				this.Weight[0].Value = PmxStreamHelper.ReadElement_Float(s);
				this.Weight[1].Value = 1f - this.Weight[0].Value;
				break;
			case DeformType.BDEF4:
			case DeformType.QDEF:
				this.Weight[0].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
				this.Weight[1].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
				this.Weight[2].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
				this.Weight[3].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
				this.Weight[0].Value = PmxStreamHelper.ReadElement_Float(s);
				this.Weight[1].Value = PmxStreamHelper.ReadElement_Float(s);
				this.Weight[2].Value = PmxStreamHelper.ReadElement_Float(s);
				this.Weight[3].Value = PmxStreamHelper.ReadElement_Float(s);
				break;
			case DeformType.SDEF:
				this.Weight[0].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
				this.Weight[1].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
				this.Weight[0].Value = PmxStreamHelper.ReadElement_Float(s);
				this.Weight[1].Value = 1f - this.Weight[0].Value;
				this.C0 = V3_BytesConvert.FromStream(s);
				this.R0 = V3_BytesConvert.FromStream(s);
				this.R1 = V3_BytesConvert.FromStream(s);
				this.CalcSDEF_RW();
				this.SDEF = true;
				break;
			}
			this.EdgeScale = PmxStreamHelper.ReadElement_Float(s);
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			V3_BytesConvert.ToStream(s, this.Position);
			V3_BytesConvert.ToStream(s, this.Normal);
			V2_BytesConvert.ToStream(s, this.UV);
			for (int i = 0; i < f.UVACount; i++)
			{
				V4_BytesConvert.ToStream(s, this.UVA[i]);
			}
			if (this.Deform == DeformType.QDEF && f.Ver < 2.1f)
			{
				s.WriteByte(2);
			}
			else
			{
				s.WriteByte((byte)this.Deform);
			}
			switch (this.Deform)
			{
			case DeformType.BDEF1:
				PmxStreamHelper.WriteElement_Int32(s, this.Weight[0].Bone, f.BoneSize, true);
				break;
			case DeformType.BDEF2:
				PmxStreamHelper.WriteElement_Int32(s, this.Weight[0].Bone, f.BoneSize, true);
				PmxStreamHelper.WriteElement_Int32(s, this.Weight[1].Bone, f.BoneSize, true);
				PmxStreamHelper.WriteElement_Float(s, this.Weight[0].Value);
				break;
			case DeformType.BDEF4:
			case DeformType.QDEF:
				PmxStreamHelper.WriteElement_Int32(s, this.Weight[0].Bone, f.BoneSize, true);
				PmxStreamHelper.WriteElement_Int32(s, this.Weight[1].Bone, f.BoneSize, true);
				PmxStreamHelper.WriteElement_Int32(s, this.Weight[2].Bone, f.BoneSize, true);
				PmxStreamHelper.WriteElement_Int32(s, this.Weight[3].Bone, f.BoneSize, true);
				PmxStreamHelper.WriteElement_Float(s, this.Weight[0].Value);
				PmxStreamHelper.WriteElement_Float(s, this.Weight[1].Value);
				PmxStreamHelper.WriteElement_Float(s, this.Weight[2].Value);
				PmxStreamHelper.WriteElement_Float(s, this.Weight[3].Value);
				break;
			case DeformType.SDEF:
				PmxStreamHelper.WriteElement_Int32(s, this.Weight[0].Bone, f.BoneSize, true);
				PmxStreamHelper.WriteElement_Int32(s, this.Weight[1].Bone, f.BoneSize, true);
				PmxStreamHelper.WriteElement_Float(s, this.Weight[0].Value);
				V3_BytesConvert.ToStream(s, this.C0);
				V3_BytesConvert.ToStream(s, this.R0);
				V3_BytesConvert.ToStream(s, this.R1);
				break;
			}
			PmxStreamHelper.WriteElement_Float(s, this.EdgeScale);
		}

		object ICloneable.Clone()
		{
			return new PmxVertex(this);
		}

		public PmxVertex Clone()
		{
			return new PmxVertex(this);
		}
	}
}
