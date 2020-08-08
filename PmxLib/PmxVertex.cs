using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	internal class PmxVertex : PmxIDObject, IPmxObjectKey, IPmxStreamIO, ICloneable
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
				SortList(list);
				return list.ToArray();
			}

			public static void SortList(List<BoneWeight> list)
			{
				CP.SSort(list, delegate(BoneWeight l, BoneWeight r)
				{
					float num = Math.Abs(r.Value) - Math.Abs(l.Value);
					return (!(num < 0f)) ? ((num > 0f) ? 1 : 0) : (-1);
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

		public Vector3 Position;

		public Vector3 Normal;

		public Vector2 UV;

		public const int MaxUVACount = 4;

		public Vector4[] UVA = new Vector4[4];

		public const int MaxWeightBoneCount = 4;

		public BoneWeight[] Weight = new BoneWeight[4];

		public DeformType Deform;

		public float EdgeScale = 1f;

		public int VertexMorphIndex = -1;

		public int UVMorphIndex = -1;

		public int[] UVAMorphIndex = new int[4];

		public int SDEFIndex = -1;

		public int QDEFIndex = -1;

		public int SoftBodyPosIndex = -1;

		public int SoftBodyNormalIndex = -1;

		public bool SDEF;

		public Vector3 C0;

		public Vector3 R0;

		public Vector3 R1;

		public Vector3 RW;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.Vertex;

		public int NWeight
		{
			get
			{
				return (int)((Weight[0].Value + 0.005f) * 100f);
			}
			set
			{
				ClearWeightValue();
				Weight[0].Value = (float)value * 0.01f;
				Weight[1].Value = 1f - Weight[0].Value;
				UpdateDeformType();
				if (Deform == DeformType.BDEF4 || Deform == DeformType.QDEF)
				{
					Deform = DeformType.BDEF2;
				}
			}
		}

		public IEnumerable<BoneWeight> EnumWeights(bool normalize = false)
		{
			if (normalize)
			{
				NormalizeWeight();
			}
			switch (Deform)
			{
			case DeformType.BDEF1:
				yield return Weight[0];
				break;
			case DeformType.BDEF2:
			case DeformType.SDEF:
				if (Weight[0].Value != 0f)
				{
					yield return Weight[0];
				}
				if (Weight[1].Value != 0f)
				{
					yield return Weight[1];
				}
				break;
			case DeformType.BDEF4:
			case DeformType.QDEF:
				if (Weight[0].Value != 0f)
				{
					yield return Weight[0];
				}
				if (Weight[1].Value != 0f)
				{
					yield return Weight[1];
				}
				if (Weight[2].Value != 0f)
				{
					yield return Weight[2];
				}
				if (Weight[3].Value != 0f)
				{
					yield return Weight[3];
				}
				break;
			}
		}

		public void ClearWeight()
		{
			ClearWeightBone();
			ClearWeightValue();
			Deform = DeformType.BDEF1;
			SDEF = false;
		}

		public void ClearWeightBone()
		{
			Weight[0].Bone = 0;
			for (int i = 1; i < 4; i++)
			{
				Weight[i].Bone = -1;
			}
		}

		public void ClearWeightValue()
		{
			Weight[0].Value = 1f;
			for (int i = 1; i < 4; i++)
			{
				Weight[i].Value = 0f;
			}
		}

		public void NormalizeWeight(bool bdef4Sum = false)
		{
			for (int i = 0; i < 4; i++)
			{
				if (Weight[i].Bone < 0)
				{
					Weight[i].Value = 0f;
				}
			}
			if (Deform == DeformType.SDEF)
			{
				NormalizeWeightOrder_SDEF();
			}
			else
			{
				NormalizeWeightOrder();
			}
			UpdateDeformType();
			if (Deform == DeformType.SDEF)
			{
				Weight[2].Bone = -1;
				Weight[2].Value = 0f;
				Weight[3].Bone = -1;
				Weight[3].Value = 0f;
			}
			NormalizeWeightSum(bdef4Sum);
			int num = 1;
			switch (Deform)
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
				if (Weight[j].Bone < 0)
				{
					Weight[j].Bone = 0;
					Weight[j].Value = 0f;
				}
			}
			UpdateDeformType();
		}

		public void NormalizeWeightOrder()
		{
			BoneWeight[] array = BoneWeight.Sort(Weight);
			for (int i = 0; i < array.Length; i++)
			{
				Weight[i] = array[i];
			}
		}

		public void NormalizeWeightOrder_BDEF2()
		{
			if (Weight[0].Value < Weight[1].Value)
			{
				CP.Swap(ref Weight[0], ref Weight[1]);
			}
		}

		public void NormalizeWeightOrder_SDEF()
		{
			if (Weight[0].Bone > Weight[1].Bone)
			{
				CP.Swap(ref Weight[0], ref Weight[1]);
				CP.Swap(ref R0, ref R1);
			}
		}

		public void NormalizeWeightSum(bool bdef4 = false)
		{
			if (!bdef4 && (Deform == DeformType.BDEF4 || Deform == DeformType.QDEF))
			{
				return;
			}
			float num = 0f;
			for (int i = 0; i < 4; i++)
			{
				num += Weight[i].Value;
			}
			if (num != 0f && num != 1f)
			{
				float num2 = 1f / num;
				for (int j = 0; j < 4; j++)
				{
					Weight[j].Value *= num2;
				}
			}
		}

		public void NormalizeWeightSum_BDEF2()
		{
			float num = Weight[0].Value + Weight[1].Value;
			if (num != 1f)
			{
				if (num == 0f)
				{
					Weight[0].Value = 1f;
					Weight[1].Value = 0f;
				}
				else
				{
					float num2 = 1f / num;
					Weight[0].Value *= num2;
					Weight[1].Value *= num2;
				}
			}
		}

		public DeformType GetDeformType()
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				if (Weight[i].Value != 0f)
				{
					num++;
				}
			}
			if (SDEF && num != 1)
			{
				return DeformType.SDEF;
			}
			if (Deform == DeformType.QDEF && num != 1)
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
			Deform = GetDeformType();
		}

		public void SetSDEF_RV(Vector3 r0, Vector3 r1)
		{
			R0 = r0;
			R1 = r1;
			CalcSDEF_RW();
		}

		public void CalcSDEF_RW()
		{
			RW = 0.5f * Weight[0].Value * Weight[1].Value * (R0 - R1);
		}

		public bool NormalizeSDEF_C0(List<PmxBone> boneList)
		{
			if (Deform != DeformType.SDEF)
			{
				return true;
			}
			int bone = Weight[0].Bone;
			int bone2 = Weight[1].Bone;
			PmxBone pmxBone = null;
			PmxBone pmxBone2 = null;
			if (CP.InRange(boneList, bone))
			{
				pmxBone = boneList[bone];
				if (CP.InRange(boneList, bone2))
				{
					pmxBone2 = boneList[bone2];
					Vector3 position = pmxBone.Position;
					Vector3 vector = pmxBone2.Position - position;
					vector.Normalize();
					Vector3 position2 = Position;
					position2 -= position;
					float scale = Vector3.Dot(vector, position2);
					C0 = vector * scale + position;
					return true;
				}
				return false;
			}
			return false;
		}

		public PmxVertex()
		{
			VertexMorphIndex = -1;
			UVMorphIndex = -1;
			for (int i = 0; i < UVAMorphIndex.Length; i++)
			{
				UVAMorphIndex[i] = -1;
			}
			SDEFIndex = -1;
			QDEFIndex = -1;
			SoftBodyPosIndex = -1;
			SoftBodyNormalIndex = -1;
			ClearWeight();
		}

		public PmxVertex(PmxVertex vertex)
			: this()
		{
			FromPmxVertex(vertex);
		}

		public void FromPmxVertex(PmxVertex vertex)
		{
			Position = vertex.Position;
			Normal = vertex.Normal;
			UV = vertex.UV;
			for (int i = 0; i < 4; i++)
			{
				UVA[i] = vertex.UVA[i];
			}
			for (int j = 0; j < 4; j++)
			{
				Weight[j] = vertex.Weight[j];
			}
			EdgeScale = vertex.EdgeScale;
			Deform = vertex.Deform;
			SDEF = vertex.SDEF;
			C0 = vertex.C0;
			R0 = vertex.R0;
			R1 = vertex.R1;
			RW = vertex.RW;
			VertexMorphIndex = vertex.VertexMorphIndex;
			UVMorphIndex = vertex.UVMorphIndex;
			for (int k = 0; k < UVAMorphIndex.Length; k++)
			{
				UVAMorphIndex[k] = vertex.UVAMorphIndex[k];
			}
			SDEFIndex = vertex.SDEFIndex;
			QDEFIndex = vertex.QDEFIndex;
			SoftBodyPosIndex = vertex.SoftBodyPosIndex;
			SoftBodyNormalIndex = vertex.SoftBodyNormalIndex;
			FromID(vertex);
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			Position = V3_BytesConvert.FromStream(s);
			Normal = V3_BytesConvert.FromStream(s);
			UV = V2_BytesConvert.FromStream(s);
			for (int i = 0; i < f.UVACount; i++)
			{
				Vector4 vector = V4_BytesConvert.FromStream(s);
				if (0 <= i && i < UVA.Length)
				{
					UVA[i] = vector;
				}
			}
			Deform = (DeformType)s.ReadByte();
			SDEF = false;
			switch (Deform)
			{
			case DeformType.BDEF1:
				Weight[0].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
				Weight[0].Value = 1f;
				break;
			case DeformType.BDEF2:
				Weight[0].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
				Weight[1].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
				Weight[0].Value = PmxStreamHelper.ReadElement_Float(s);
				Weight[1].Value = 1f - Weight[0].Value;
				break;
			case DeformType.BDEF4:
			case DeformType.QDEF:
				Weight[0].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
				Weight[1].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
				Weight[2].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
				Weight[3].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
				Weight[0].Value = PmxStreamHelper.ReadElement_Float(s);
				Weight[1].Value = PmxStreamHelper.ReadElement_Float(s);
				Weight[2].Value = PmxStreamHelper.ReadElement_Float(s);
				Weight[3].Value = PmxStreamHelper.ReadElement_Float(s);
				break;
			case DeformType.SDEF:
				Weight[0].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
				Weight[1].Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
				Weight[0].Value = PmxStreamHelper.ReadElement_Float(s);
				Weight[1].Value = 1f - Weight[0].Value;
				C0 = V3_BytesConvert.FromStream(s);
				R0 = V3_BytesConvert.FromStream(s);
				R1 = V3_BytesConvert.FromStream(s);
				CalcSDEF_RW();
				SDEF = true;
				break;
			}
			EdgeScale = PmxStreamHelper.ReadElement_Float(s);
			if (f.WithID)
			{
				base.UID = PmxStreamHelper.ReadElement_UInt(s);
				base.CID = PmxStreamHelper.ReadElement_UInt(s);
			}
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			V3_BytesConvert.ToStream(s, Position);
			V3_BytesConvert.ToStream(s, Normal);
			V2_BytesConvert.ToStream(s, UV);
			for (int i = 0; i < f.UVACount; i++)
			{
				V4_BytesConvert.ToStream(s, UVA[i]);
			}
			if (Deform == DeformType.QDEF && f.Ver < 2.1f)
			{
				s.WriteByte(2);
			}
			else
			{
				s.WriteByte((byte)Deform);
			}
			switch (Deform)
			{
			case DeformType.BDEF1:
				PmxStreamHelper.WriteElement_Int32(s, Weight[0].Bone, f.BoneSize);
				break;
			case DeformType.BDEF2:
				PmxStreamHelper.WriteElement_Int32(s, Weight[0].Bone, f.BoneSize);
				PmxStreamHelper.WriteElement_Int32(s, Weight[1].Bone, f.BoneSize);
				PmxStreamHelper.WriteElement_Float(s, Weight[0].Value);
				break;
			case DeformType.BDEF4:
			case DeformType.QDEF:
				PmxStreamHelper.WriteElement_Int32(s, Weight[0].Bone, f.BoneSize);
				PmxStreamHelper.WriteElement_Int32(s, Weight[1].Bone, f.BoneSize);
				PmxStreamHelper.WriteElement_Int32(s, Weight[2].Bone, f.BoneSize);
				PmxStreamHelper.WriteElement_Int32(s, Weight[3].Bone, f.BoneSize);
				PmxStreamHelper.WriteElement_Float(s, Weight[0].Value);
				PmxStreamHelper.WriteElement_Float(s, Weight[1].Value);
				PmxStreamHelper.WriteElement_Float(s, Weight[2].Value);
				PmxStreamHelper.WriteElement_Float(s, Weight[3].Value);
				break;
			case DeformType.SDEF:
				PmxStreamHelper.WriteElement_Int32(s, Weight[0].Bone, f.BoneSize);
				PmxStreamHelper.WriteElement_Int32(s, Weight[1].Bone, f.BoneSize);
				PmxStreamHelper.WriteElement_Float(s, Weight[0].Value);
				V3_BytesConvert.ToStream(s, C0);
				V3_BytesConvert.ToStream(s, R0);
				V3_BytesConvert.ToStream(s, R1);
				break;
			}
			PmxStreamHelper.WriteElement_Float(s, EdgeScale);
			if (f.WithID)
			{
				PmxStreamHelper.WriteElement_UInt(s, base.UID);
				PmxStreamHelper.WriteElement_UInt(s, base.CID);
			}
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
