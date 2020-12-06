using PmxLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace COM3D.ModelExportMMD
{
	public class PmxBuilder
	{
		public float scale2 = 0.001f;

		public int counti = 0;

		public int counti2 = 0;

		private Pmx pmxFile;

		private int scale = 11;

		private int vertexCount = 0;

		private Dictionary<Transform, int> bonesMap;

		private List<Transform> boneList = new List<Transform>();

		private List<Matrix4x4> bindposeList = new List<Matrix4x4>();

		public List<Bones> bones2 = new List<Bones>();

		public Mesh mesh3;

		public PmxBuilder()
		{
            /*
			if (!Directory.Exists(ExportWindow.ExportFolder))
			{
				Directory.CreateDirectory(ExportWindow.ExportFolder);
			}
            */
			pmxFile = new Pmx();
		}

		public void CreatePmxHeader()
		{
			PmxElementFormat pmxElementFormat = new PmxElementFormat(1f);
			pmxElementFormat.VertexSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.VertexList.Count);
			int val = -2147483648;
			for (int i = 0; i < pmxFile.BoneList.Count; i++)
			{
				val = Math.Max(val, Math.Abs(pmxFile.BoneList[i].IK.LinkList.Count));
			}
			val = Math.Max(val, pmxFile.BoneList.Count);
			pmxElementFormat.BoneSize = PmxElementFormat.GetSignedBufSize(val);
			if (pmxElementFormat.BoneSize < 2)
			{
				pmxElementFormat.BoneSize = 2;
			}
			pmxElementFormat.MorphSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.MorphList.Count);
			pmxElementFormat.MaterialSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.MaterialList.Count);
			pmxElementFormat.BodySize = PmxElementFormat.GetUnsignedBufSize(pmxFile.BodyList.Count);
			PmxHeader pmxHeader = new PmxHeader(2.1f);
			pmxHeader.FromElementFormat(pmxElementFormat);
            pmxFile.Header.FromHeader(pmxHeader);
			//this.pmxFile.Header = pmxHeader;
		}

		public void CreateModelInfo()
		{
			PmxModelInfo pmxModelInfo = new PmxModelInfo();
			pmxModelInfo.ModelName = "妹抖";
			pmxModelInfo.ModelNameE = "maid";
			pmxModelInfo.Comment = "我的妹抖";
			pmxModelInfo.Comment = "my maid";
            pmxFile.ModelInfo.FromModelInfo(pmxModelInfo);
		}

		private PmxVertex.BoneWeight[] ConvertBoneWeight(BoneWeight unityWeight)
		{
			PmxVertex.BoneWeight[] array = new PmxVertex.BoneWeight[4];
			array[0].Bone = unityWeight.boneIndex0;
			array[0].Value = unityWeight.weight0;
			array[1].Bone = unityWeight.boneIndex1;
			array[1].Value = unityWeight.weight1;
			array[2].Bone = unityWeight.boneIndex2;
			array[2].Value = unityWeight.weight2;
			array[3].Bone = unityWeight.boneIndex3;
			array[3].Value = unityWeight.weight3;
			return array;
		}

		private void AddFaceList(int[] faceList, int count)
		{
			for (int i = 0; i < faceList.Length; i++)
			{
				faceList[i] += count;
				pmxFile.FaceList.Add(faceList[i]);
			}
		}

		private UnityEngine.Vector3 MultiplyVec3s(UnityEngine.Vector3 v1, UnityEngine.Vector3 v2)
		{
			return new UnityEngine.Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
		}

		private UnityEngine.Vector3 RotateAroundPoint(UnityEngine.Vector3 point, UnityEngine.Vector3 pivot, UnityEngine.Quaternion angle)
		{
			return angle * (point - pivot) + pivot;
		}

		public void CreateMeshList(SkinnedMeshRenderer meshRender)
		{
			if (meshRender.name.Contains("Face"))
			{
				counti = pmxFile.VertexList.Count;
			}
			if (meshRender.name.Contains("body"))
			{
				counti2 = pmxFile.VertexList.Count;
			}
			GameObject gameObject = meshRender.gameObject;
			Mesh mesh = meshRender.sharedMesh;
			BoneWeight[] boneWeights = mesh.boneWeights;
			if (true)//ExportWindow.SavePostion)
			{
				Mesh mesh2 = new Mesh();
				meshRender.BakeMesh(mesh2);
				mesh = mesh2;
			}
			mesh3 = mesh;
			UnityEngine.Vector2[] uv = mesh.uv;
			UnityEngine.Vector2[] uv2 = mesh.uv2;
			UnityEngine.Vector3[] normals = mesh.normals;
			UnityEngine.Vector3[] vertices = mesh.vertices;
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				if (meshRender.name == "moza")
				{
					break;
				}
				int[] triangles = mesh.GetTriangles(i);
				AddFaceList(triangles, vertexCount);
				CreateMaterial(meshRender.sharedMaterials[i], triangles.Length);
			}
			vertexCount += mesh.vertexCount;
			for (int i = 0; i < mesh.vertexCount; i++)
			{
				PmxVertex pmxVertex = new PmxVertex();
				pmxVertex.UV = new PmxLib.Vector2(uv[i].x, 0f - uv[i].y);
				pmxVertex.Weight = ConvertBoneWeight(boneWeights[i]);
				UnityEngine.Vector3 point = normals[i];
				point = RotateAroundPoint(point, UnityEngine.Vector3.zero, gameObject.transform.rotation);
				pmxVertex.Normal = new PmxLib.Vector3(0f - point.x, point.y, 0f - point.z);
				UnityEngine.Vector3 point2 = vertices[i];
				point2 = RotateAroundPoint(point2, UnityEngine.Vector3.zero, gameObject.transform.rotation);
				point2 += gameObject.transform.position;
				point2 *= (float)scale;
				pmxVertex.Position = new PmxLib.Vector3(0f - point2.x, point2.y, 0f - point2.z);
				pmxVertex.Deform = PmxVertex.DeformType.BDEF4;
				pmxFile.VertexList.Add(pmxVertex);
			}
		}

		private UnityEngine.Vector3 TransToParent(UnityEngine.Vector3 v, int index)
		{
			Transform transform = boneList[index];
			int num = -1;
			if (bonesMap.ContainsKey(transform.parent))
			{
				num = bonesMap[transform.parent];
			}
			if (num != -1)
			{
				Matrix4x4 matrix4x = default(Matrix4x4);
				matrix4x = bindposeList[index] * boneList[num].worldToLocalMatrix.inverse;
				v = matrix4x.MultiplyVector(v);
				v = TransToParent(v, num);
			}
			return v;
		}

		private UnityEngine.Vector3 CalcPostion(UnityEngine.Vector3 v, BoneWeight boneWeight, Transform[] bones)
		{
			Transform key = bones[boneWeight.boneIndex0];
			if (bonesMap.ContainsKey(key))
			{
				int index = bonesMap[key];
				v = TransToParent(v, index);
			}
			return v;
		}

		public void PrepareData(List<SkinnedMeshRenderer> skinnedMeshList)
		{
			for (int i = 0; i < skinnedMeshList.Count; i++)
			{
				bones2.Add(new Bones());
			}
			for (int num = skinnedMeshList.Count - 1; num >= 0; num--)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = skinnedMeshList[num];
				for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
				{
					bool flag = false;
					Transform transform = skinnedMeshRenderer.bones[i];
					if ((UnityEngine.Object)transform == (UnityEngine.Object)null)
					{
						bones2[num].names.Add(null);
					}
					else
					{
						int num2 = 0;
						while (!flag && num2 < boneList.Count)
						{
							if (boneList[num2].name == transform.name)
							{
								if (transform.name == "Bone_center")
								{
									Transform transform2 = skinnedMeshRenderer.bones[i];
									Transform transform3 = transform2;
									transform3.name += num;
									break;
								}
								flag = true;
							}
							num2++;
						}
						bones2[num].names.Add(skinnedMeshRenderer.bones[i].name);
						if (!flag)
						{
							boneList.Add(skinnedMeshRenderer.bones[i]);
							bindposeList.Add(skinnedMeshRenderer.sharedMesh.bindposes[i]);
						}
					}
				}
			}
			bonesMap = new Dictionary<Transform, int>();
			for (int i = 0; i < boneList.Count; i++)
			{
				bonesMap[boneList[i]] = i;
			}
			for (int i = 0; i < skinnedMeshList.Count; i++)
			{
				if (skinnedMeshList[i].name == "moza")
				{
					bones2.Remove(bones2[i]);
				}
			}
		}

		public void CreateBoneList()
		{
			foreach (Transform bone in boneList)
			{
				PmxBone pmxBone = new PmxBone();
				pmxBone.Name = bone.name;
				pmxBone.NameE = bone.name;
				if (bonesMap.ContainsKey(bone.parent))
				{
					pmxBone.Parent = bonesMap[bone.parent];
				}
				UnityEngine.Vector3 vector = bone.transform.position * (float)scale;
				pmxBone.Position = new PmxLib.Vector3(0f - vector.x, vector.y, 0f - vector.z);
				pmxFile.BoneList.Add(pmxBone);
			}
		}

		private PmxBody CreateColliderBody(Collider rigidbody)
		{
			PmxBody pmxBody = new PmxBody();
			pmxBody.Name = rigidbody.name;
			pmxBody.NameE = rigidbody.name;
			pmxBody.Position.X = rigidbody.transform.position.x;
			pmxBody.Position.Y = rigidbody.transform.position.y;
			pmxBody.Position.Z = rigidbody.transform.position.z;
			PmxBody pmxBody2 = pmxBody;
			UnityEngine.Quaternion rotation = rigidbody.transform.rotation;
			pmxBody2.Rotation.X = rotation.eulerAngles.x;
			PmxBody pmxBody3 = pmxBody;
			rotation = rigidbody.transform.rotation;
			pmxBody3.Rotation.Y = rotation.eulerAngles.y;
			PmxBody pmxBody4 = pmxBody;
			rotation = rigidbody.transform.rotation;
			pmxBody4.Rotation.Z = rotation.eulerAngles.z;
			return pmxBody;
		}

		public void CreateMaterial(Material material, int count)
		{
			PmxMaterial pmxMaterial = new PmxMaterial();
			pmxMaterial.Name = material.name;
			pmxMaterial.NameE = material.name;
			pmxMaterial.Flags = (PmxMaterial.MaterialFlags.DrawBoth | PmxMaterial.MaterialFlags.Shadow | PmxMaterial.MaterialFlags.SelfShadowMap | PmxMaterial.MaterialFlags.SelfShadow);
			if ((UnityEngine.Object)material.mainTexture != (UnityEngine.Object)null)
			{
				string text = material.name;
				if (text.Contains("Instance"))
				{
					object obj = text;
					text = obj + "_(" + material.GetInstanceID() + ")";
				}
				pmxMaterial.Tex = text + ".png";
				Texture mainTexture = material.mainTexture;
				if (true)//ExportWindow.SaveTexture)
				{
					//TextureWriter.WriteTexture2D(ExportWindow.ExportFolder + "/" + pmxMaterial.Tex, mainTexture);
				}
			}
            /*
			if (material.HasProperty("_Color"))
			{
				pmxMaterial.Diffuse = material.GetColor("_Color");
			}
			if (material.HasProperty("_AmbColor"))
			{
				pmxMaterial.Ambient = material.GetColor("_AmbColor");
			}
			if (material.HasProperty("_Opacity"))
			{
				pmxMaterial.Diffuse.a = material.GetFloat("_Opacity");
			}
			if (material.HasProperty("_SpecularColor"))
			{
				pmxMaterial.Specular = material.GetColor("_SpecularColor");
			}
			if (material.HasProperty("_Shininess"))
			{
				goto IL_0194;
			}
			goto IL_0194;
			IL_0194:
			if (material.HasProperty("_OutlineColor"))
			{
				pmxMaterial.EdgeSize = material.GetFloat("_OutlineWidth");
				pmxMaterial.EdgeColor = material.GetColor("_OutlineColor");
			}
            */
			pmxMaterial.FaceCount = count;
			pmxFile.MaterialList.Add(pmxMaterial);
		}

		public void Save()
		{
            pmxFile.ToFile("output");// ExportWindow.ExportFolder + "/" + ExportWindow.ExportName + ".pmx");
		}

		public void setParent(Transform[] tlist)
		{
			for (int i = 0; i < pmxFile.BoneList.Count; i++)
			{
				pmxFile.BoneList[i].Name = pmxFile.BoneList[i].Name.Replace("_SCL_", "");
			}
			for (int i = 0; i < bones2.Count; i++)
			{
				for (int j = 0; j < bones2[i].names.Count; j++)
				{
					if (bones2[i].names[j] != null)
					{
						bones2[i].names[j] = bones2[i].names[j].Replace("_SCL_", "");
					}
				}
			}
			for (int i = 0; i < pmxFile.BoneList.Count; i++)
			{
				for (int j = 0; j < tlist.Length; j++)
				{
					if (pmxFile.BoneList[i].Name == tlist[j].name && ((UnityEngine.Object)tlist[j].parent != (UnityEngine.Object)null || FindBone(tlist[j].parent.name) != null))
					{
						pmxFile.BoneList[i].Parent = FindBoneIndex(tlist[j].parent.name);
					}
				}
			}
		}

		public void Sbp()
		{
			PmxBone pmxBone = FindBone("Bone_Face");
			if (pmxBone != null)
			{
				pmxBone.Parent = FindBoneIndex("Bip01 Neck");
			}
			pmxBone = FindBone("Hair_F");
			if (pmxBone != null)
			{
				pmxBone.Parent = FindBoneIndex("Bone_Face");
			}
			pmxBone = FindBone("Hair_R");
			if (pmxBone != null)
			{
				pmxBone.Parent = FindBoneIndex("Bone_Face");
			}
			if (FindBone("Skirt") == null)
			{
				for (int i = 0; i < pmxFile.BoneList.Count; i++)
				{
					if (pmxFile.BoneList[i].Name.Contains("Skirt") && pmxFile.BoneList[i].Name[8] == 'A')
					{
						pmxFile.BoneList[i].Parent = FindBoneIndex("Bip01 Pelvis");
					}
				}
			}
		}

		public void CreateMorph()
		{
			Maid maid = GameMain.Instance.CharacterMgr.GetMaid(0);
			for (int i = 0; i < maid.body0.Face.morph.BlendDatas.Count; i++)
			{
				if (maid.body0.Face.morph.BlendDatas[i] != null)
				{
					PmxMorph pmxMorph = new PmxMorph();
					pmxMorph.Name = maid.body0.Face.morph.BlendDatas[i].name;
					pmxMorph.NameE = maid.body0.Face.morph.BlendDatas[i].name;
					pmxMorph.Panel = 1;
					pmxMorph.Kind = PmxMorph.OffsetKind.Vertex;
					for (int j = 0; j < maid.body0.Face.morph.BlendDatas[i].v_index.Length; j++)
					{
						PmxVertexMorph pmxVertexMorph = new PmxVertexMorph(maid.body0.Face.morph.BlendDatas[i].v_index[j] + counti, new PmxLib.Vector3(0f - maid.body0.Face.morph.BlendDatas[i].vert[j].x, maid.body0.Face.morph.BlendDatas[i].vert[j].z, maid.body0.Face.morph.BlendDatas[i].vert[j].y));
						PmxVertexMorph pmxVertexMorph2 = pmxVertexMorph;
						PmxVertexMorph pmxVertexMorph3 = pmxVertexMorph2;
						pmxVertexMorph3.Offset *= (float)scale;
						if (pmxMorph.Name.Contains("eye"))
						{
							pmxVertexMorph.Offset.x *= 1f + (float)(maid.GetProp(MPN.EyeSclX).value - maid.GetProp(MPN.EyeSclX).value_Default) * scale2;
							pmxVertexMorph.Offset.y *= 1f + (float)(maid.GetProp(MPN.EyeSclY).value - maid.GetProp(MPN.EyeSclY).value_Default) * scale2;
							pmxVertexMorph.Offset.z *= 1f + (float)(maid.GetProp(MPN.EyeSclX).value - maid.GetProp(MPN.EyeSclX).value_Default) * scale2;
						}
						pmxVertexMorph.Offset.x *= 1f + (float)(maid.GetProp(MPN.EarScl).value - maid.GetProp(MPN.EarScl).value_Default) * scale2;
						pmxVertexMorph.Offset.y *= 1f + (float)(maid.GetProp(MPN.NosePos).value - maid.GetProp(MPN.NosePos).value_Default) * scale2;
						pmxVertexMorph.Offset.z *= 1f + (float)(maid.GetProp(MPN.EarScl).value - maid.GetProp(MPN.EarScl).value_Default) * scale2;
						pmxMorph.OffsetList.Add(pmxVertexMorph);
					}
					pmxFile.MorphList.Add(pmxMorph);
				}
			}
		}

		public void CreateMorphKupa()
		{
			Maid maid = GameMain.Instance.CharacterMgr.GetMaid(0);
			for (int i = 0; i < maid.body0.goSlot.Count; i++)
			{
				if (maid.body0.goSlot[i] != null && maid.body0.goSlot[i].morph != null)
				{
					for (int j = 0; j < maid.body0.goSlot[i].morph.BlendDatas.Count; j++)
					{
						if (maid.body0.goSlot[i].morph.BlendDatas[j] != null && (!(maid.body0.goSlot[i].morph.BlendDatas[j].name != "analkupa") || !(maid.body0.goSlot[i].morph.BlendDatas[j].name != "kupa") || !(maid.body0.goSlot[i].morph.BlendDatas[j].name != "labiakupa") || !(maid.body0.goSlot[i].morph.BlendDatas[j].name != "vaginakupa") || !(maid.body0.goSlot[i].morph.BlendDatas[j].name != "nyodokupa") || !(maid.body0.goSlot[i].morph.BlendDatas[j].name != "suji") || !(maid.body0.goSlot[i].morph.BlendDatas[j].name != "clitoris") || !(maid.body0.goSlot[i].morph.BlendDatas[j].name != "ap_vagina_y") || !(maid.body0.goSlot[i].morph.BlendDatas[j].name != "ap_nyodo_y") || !(maid.body0.goSlot[i].morph.BlendDatas[j].name != "ap_anal_y")))
						{
							PmxMorph pmxMorph = new PmxMorph();
							pmxMorph.Name = maid.body0.goSlot[i].morph.BlendDatas[j].name;
							pmxMorph.NameE = maid.body0.goSlot[i].morph.BlendDatas[j].name;
							pmxMorph.Panel = 1;
							pmxMorph.Kind = PmxMorph.OffsetKind.Vertex;
							for (int k = 0; k < maid.body0.goSlot[i].morph.BlendDatas[j].v_index.Length; k++)
							{
								PmxVertexMorph pmxVertexMorph = new PmxVertexMorph(maid.body0.goSlot[i].morph.BlendDatas[j].v_index[k] + counti2, new PmxLib.Vector3(0f - maid.body0.goSlot[i].morph.BlendDatas[j].vert[k].x, maid.body0.goSlot[i].morph.BlendDatas[j].vert[k].z, maid.body0.goSlot[i].morph.BlendDatas[j].vert[k].y));
								PmxVertexMorph pmxVertexMorph2 = pmxVertexMorph;
								PmxVertexMorph pmxVertexMorph3 = pmxVertexMorph2;
								pmxVertexMorph3.Offset *= (float)scale;
								pmxMorph.OffsetList.Add(pmxVertexMorph);
							}
							pmxFile.MorphList.Add(pmxMorph);
						}
					}
				}
			}
		}

		public void ChangeBoneNames()
		{
			if (FindBone("Bip01") != null)
			{
				ChangeName(FindBone("Bip01 Pelvis"), "下半身");
				ChangeName(FindBone("Bip01 L Thigh"), "左足");
				ChangeName(FindBone("Bip01 L Calf"), "左ひざ");
				ChangeName(FindBone("Bip01 L Foot"), "左足首");
				ChangeName(FindBone("Bip01 R Thigh"), "右足");
				ChangeName(FindBone("Bip01 R Calf"), "右ひざ");
				ChangeName(FindBone("Bip01 R Foot"), "右足首");
				ChangeName(FindBone("Bip01 Spine"), "上半身");
				ChangeName(FindBone("Bip01 Spine1"), "上半身2");
				ChangeName(FindBone("Bip01 L Clavicle"), "左肩");
				ChangeName(FindBone("Bip01 L UpperArm"), "左腕捩");
				ChangeName(FindBone("Uppertwist_L"), "左腕");
				ChangeName(FindBone("Bip01 L Forearm"), "左ひじ");
				ChangeName(FindBone("Bip01 L Hand"), "左手首");
				ChangeName(FindBone("Bip01 L Finger0"), "左親指０");
				ChangeName(FindBone("Bip01 L Finger01"), "左親指１");
				ChangeName(FindBone("Bip01 L Finger02"), "左親指２");
				ChangeName(FindBone("Bip01 L Finger1"), "左人指１");
				ChangeName(FindBone("Bip01 L Finger11"), "左人指２");
				ChangeName(FindBone("Bip01 L Finger12"), "左人指３");
				ChangeName(FindBone("Bip01 L Finger2"), "左中指１");
				ChangeName(FindBone("Bip01 L Finger21"), "左中指２");
				ChangeName(FindBone("Bip01 L Finger22"), "左中指３");
				ChangeName(FindBone("Bip01 L Finger3"), "左薬指１");
				ChangeName(FindBone("Bip01 L Finger31"), "左薬指２");
				ChangeName(FindBone("Bip01 L Finger32"), "左薬指３");
				ChangeName(FindBone("Bip01 L Finger4"), "左小指１");
				ChangeName(FindBone("Bip01 L Finger41"), "左小指２");
				ChangeName(FindBone("Bip01 L Finger42"), "左小指３");
				ChangeName(FindBone("Bip01 R Clavicle"), "右肩");
				ChangeName(FindBone("Bip01 R UpperArm"), "右腕捩");
				ChangeName(FindBone("Uppertwist_R"), "右腕");
				ChangeName(FindBone("Bip01 R Forearm"), "右ひじ");
				ChangeName(FindBone("Bip01 R Hand"), "右手首");
				ChangeName(FindBone("Bip01 R Finger0"), "右親指０");
				ChangeName(FindBone("Bip01 R Finger01"), "右親指１");
				ChangeName(FindBone("Bip01 R Finger02"), "右親指２");
				ChangeName(FindBone("Bip01 R Finger1"), "右人指１");
				ChangeName(FindBone("Bip01 R Finger11"), "右人指２");
				ChangeName(FindBone("Bip01 R Finger12"), "右人指３");
				ChangeName(FindBone("Bip01 R Finger2"), "右中指１");
				ChangeName(FindBone("Bip01 R Finger21"), "右中指２");
				ChangeName(FindBone("Bip01 R Finger22"), "右中指３");
				ChangeName(FindBone("Bip01 R Finger3"), "右薬指１");
				ChangeName(FindBone("Bip01 R Finger31"), "右薬指２");
				ChangeName(FindBone("Bip01 R Finger32"), "右薬指３");
				ChangeName(FindBone("Bip01 R Finger4"), "右小指１");
				ChangeName(FindBone("Bip01 R Finger41"), "右小指２");
				ChangeName(FindBone("Bip01 R Finger42"), "右小指３");
				ChangeName(FindBone("Bip01 Neck"), "首");
				ChangeName(FindBone("Bip01 Head"), "頭");
			}
			if (FindBone("Bone_Face") != null)
			{
				ChangeName(FindBone("Eye_L"), "左目");
				ChangeName(FindBone("Eye_R"), "右目");
			}
		}

		private PmxBone FindBone(string name)
		{
			for (int i = 0; i < pmxFile.BoneList.Count; i++)
			{
				if (pmxFile.BoneList[i].Name.Equals(name))
				{
					return pmxFile.BoneList[i];
				}
			}
			return null;
		}

		private void ChangeName(PmxBone b, string name)
		{
			if (b != null)
			{
				b.Name = name;
			}
		}

		private int FindBoneIndex(string name)
		{
			for (int i = 0; i < pmxFile.BoneList.Count; i++)
			{
				if (pmxFile.BoneList[i].Name == name)
				{
					return i;
				}
			}
			return -1;
		}

		public void AddBone()
		{
			PmxBone pmxBone = new PmxBone();
			pmxBone.Name = "全ての親";
			pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Visible);
			InsertBone(0, pmxBone);
			pmxBone = new PmxBone();
			pmxBone.Name = "センタ\u30fc";
			pmxBone.Position = new PmxLib.Vector3(0f, FindBone("Bip01").Position.Y * 0.75f, 0f);
			pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Visible);
			InsertBone(1, pmxBone);
			if (FindBone("Bip01") != null)
			{
				pmxBone = new PmxBone();
				pmxBone.Name = "左つま先";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("左足首")) + 1, pmxBone);
				pmxBone = new PmxBone();
				pmxBone.Name = "左足ＩＫ";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("左つま先")) + 1, pmxBone);
				pmxBone = new PmxBone();
				pmxBone.Name = "左つま先ＩＫ";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("左足ＩＫ")) + 1, pmxBone);
				pmxBone = new PmxBone();
				pmxBone.Name = "左手捩";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("左ひじ")) + 1, pmxBone);
				pmxBone = new PmxBone();
				pmxBone.Name = "右つま先";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("右足首")) + 1, pmxBone);
				pmxBone = new PmxBone();
				pmxBone.Name = "右足ＩＫ";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("右つま先")) + 1, pmxBone);
				pmxBone = new PmxBone();
				pmxBone.Name = "右つま先ＩＫ";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("右足ＩＫ")) + 1, pmxBone);
				pmxBone = new PmxBone();
				pmxBone.Name = "右手捩";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("右ひじ")) + 1, pmxBone);
				pmxBone = new PmxBone();
				pmxBone.Name = "胸親";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("Mune_L")), pmxBone);
				pmxBone = new PmxBone();
				pmxBone.Name = "左AH2";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("Mune_L_sub")) + 1, pmxBone);
				pmxBone = new PmxBone();
				pmxBone.Name = "右AH2";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("Mune_R_sub")) + 1, pmxBone);
			}
			SortBone2(FindBoneIndex("左腕"), FindBoneIndex("左肩") + 1);
			SortBone2(FindBoneIndex("右腕"), FindBoneIndex("右肩") + 1);
			if (FindBone("Bone_Face") != null)
			{
				pmxBone = new PmxBone();
				pmxBone.Name = "両目";
				InsertBone(pmxFile.BoneList.IndexOf(FindBone("Bone_Face")) + 1, pmxBone);
			}
			if (FindBone("Bip01") != null)
			{
				pmxBone = FindBone("左つま先");
				pmxBone.Position = FindBone("Bip01 L Toe11").Position;
				pmxBone.To_Bone = FindBone("Bip01 L Toe11").To_Bone;
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone.Parent = FindBoneIndex("左足首");
				pmxBone = FindBone("左足ＩＫ");
				pmxBone.Position = FindBone("左足首").Position;
				pmxBone.To_Offset = new PmxLib.Vector3(0f, -1.3f, 0f);
				pmxBone.Parent = FindBoneIndex("全ての親");
				pmxBone.Flags = (PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.IK);
				pmxBone.IK.Angle = 2f;
				pmxBone.IK.LoopCount = 40;
				pmxBone.IK.Target = FindBoneIndex("左足首");
				PmxIK.IKLink iKLink = new PmxIK.IKLink();
				iKLink.Bone = FindBoneIndex("左ひざ");
				iKLink.IsLimit = true;
				iKLink.High = new PmxLib.Vector3(-0.008726646f, 0f, 0f);
				iKLink.Low = new PmxLib.Vector3(-3.14159274f, 0f, 0f);
				pmxBone.IK.LinkList.Add(iKLink);
				PmxIK.IKLink iKLink2 = new PmxIK.IKLink();
				iKLink2.Bone = FindBoneIndex("左足");
				pmxBone.IK.LinkList.Add(iKLink2);
				pmxBone = FindBone("左つま先ＩＫ");
				pmxBone.Position = FindBone("左つま先").Position;
				pmxBone.To_Offset = new PmxLib.Vector3(0f, -1.3081f, 0f);
				pmxBone.Parent = FindBoneIndex("左足ＩＫ");
				pmxBone.Flags = (PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.IK);
				pmxBone.IK.Angle = 4f;
				pmxBone.IK.LoopCount = 3;
				pmxBone.IK.Target = FindBoneIndex("左つま先");
				iKLink = new PmxIK.IKLink();
				iKLink.Bone = FindBoneIndex("左足首");
				pmxBone.IK.LinkList.Add(iKLink);
				pmxBone = FindBone("左手捩");
				pmxBone.Position = default(PmxLib.Vector3);
				pmxBone.Position.X = (FindBone("Foretwist1_L").Position.X + FindBone("左手首").Position.X) / 2f;
				pmxBone.Position.Y = (FindBone("Foretwist1_L").Position.Y + FindBone("左手首").Position.Y) / 2f;
				pmxBone.Position.Z = (FindBone("Foretwist1_L").Position.Z + FindBone("左手首").Position.Z) / 2f;
				pmxBone.To_Bone = FindBoneIndex("左手首");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone.Parent = FindBoneIndex("左ひじ");
				pmxBone = FindBone("右つま先");
				pmxBone.Position = FindBone("Bip01 R Toe11").Position;
				pmxBone.To_Bone = FindBone("Bip01 R Toe11").To_Bone;
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone.Parent = FindBoneIndex("右足首");
				pmxBone = FindBone("右足ＩＫ");
				pmxBone.Position = FindBone("右足首").Position;
				pmxBone.To_Offset = new PmxLib.Vector3(0f, -1.3f, 0f);
				pmxBone.Parent = FindBoneIndex("全ての親");
				pmxBone.Flags = (PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.IK);
				pmxBone.IK.Angle = 2f;
				pmxBone.IK.LoopCount = 40;
				pmxBone.IK.Target = FindBoneIndex("右足首");
				iKLink = new PmxIK.IKLink();
				iKLink.Bone = FindBoneIndex("右ひざ");
				iKLink.IsLimit = true;
				iKLink.High = new PmxLib.Vector3(-0.008726646f, 0f, 0f);
				iKLink.Low = new PmxLib.Vector3(-3.14159274f, 0f, 0f);
				pmxBone.IK.LinkList.Add(iKLink);
				iKLink2 = new PmxIK.IKLink();
				iKLink2.Bone = FindBoneIndex("右足");
				pmxBone.IK.LinkList.Add(iKLink2);
				pmxBone = FindBone("右つま先ＩＫ");
				pmxBone.Position = FindBone("右つま先").Position;
				pmxBone.To_Offset = new PmxLib.Vector3(0f, -1.3081f, 0f);
				pmxBone.Parent = FindBoneIndex("右足ＩＫ");
				pmxBone.Flags = (PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.IK);
				pmxBone.IK.Angle = 4f;
				pmxBone.IK.LoopCount = 3;
				pmxBone.IK.Target = FindBoneIndex("右つま先");
				iKLink = new PmxIK.IKLink();
				iKLink.Bone = FindBoneIndex("右足首");
				pmxBone.IK.LinkList.Add(iKLink);
				pmxBone = FindBone("右手捩");
				pmxBone.Position = default(PmxLib.Vector3);
				pmxBone.Position.X = (FindBone("Foretwist1_R").Position.X + FindBone("右手首").Position.X) / 2f;
				pmxBone.Position.Y = (FindBone("Foretwist1_R").Position.Y + FindBone("右手首").Position.Y) / 2f;
				pmxBone.Position.Z = (FindBone("Foretwist1_R").Position.Z + FindBone("右手首").Position.Z) / 2f;
				pmxBone.To_Bone = FindBoneIndex("右手首");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone.Parent = FindBoneIndex("右ひじ");
			}
			if (FindBone("Bone_Face") != null)
			{
				pmxBone = FindBone("両目");
				pmxBone.Position = new PmxLib.Vector3(0f, FindBone("頭").Position.Y + 3f, 0f);
				pmxBone.To_Offset = new PmxLib.Vector3(0f, 0f, -1f);
				pmxBone.Parent = FindBoneIndex("頭");
			}
		}

		public void ChangeBoneInfo()
		{
			PmxBone pmxBone;
			if (FindBone("Bip01") != null)
			{
				pmxBone = FindBone("下半身");
				pmxBone.To_Bone = FindBoneIndex("Bip01");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone.Position = FindBone("上半身").Position;
				pmxBone = FindBone("Hip_L");
				pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
				pmxBone.AddRatio = 0.5f;
				pmxBone.AddParent = FindBoneIndex("左足");
				pmxBone = FindBone("Hip_R");
				pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
				pmxBone.AddRatio = 0.5f;
				pmxBone.AddParent = FindBoneIndex("右足");
				pmxBone = FindBone("左肩");
				pmxBone.To_Bone = FindBoneIndex("左腕");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("左腕");
				pmxBone.To_Bone = FindBoneIndex("左ひじ");
				pmxBone.Parent = FindBoneIndex("左肩");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("左腕捩");
				pmxBone.To_Bone = FindBoneIndex("左ひじ");
				pmxBone.Parent = FindBoneIndex("左腕");
				PmxLib.Vector3 vector = default(PmxLib.Vector3);
				vector = FindBone("左腕").Position - FindBone("左ひじ").Position;
				pmxBone.Position = FindBone("左腕").Position - vector * 0.8f;
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("左ひじ");
				pmxBone.Parent = FindBoneIndex("左腕捩");
				pmxBone.To_Bone = FindBoneIndex("左手首");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("左手首");
				pmxBone.To_Bone = FindBoneIndex("左中指１");
				pmxBone.Parent = FindBoneIndex("左手捩");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("右肩");
				pmxBone.To_Bone = FindBoneIndex("右腕");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("右腕");
				pmxBone.To_Bone = FindBoneIndex("右ひじ");
				pmxBone.Parent = FindBoneIndex("右肩");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("右腕捩");
				pmxBone.Parent = FindBoneIndex("右腕");
				pmxBone.To_Bone = FindBoneIndex("右ひじ");
				vector = default(PmxLib.Vector3);
				vector = FindBone("右腕").Position - FindBone("右ひじ").Position;
				pmxBone.Position = FindBone("右腕").Position - vector * 0.8f;
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("右ひじ");
				pmxBone.Parent = FindBoneIndex("右腕捩");
				pmxBone.To_Bone = FindBoneIndex("右手首");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("右手首");
				pmxBone.To_Bone = FindBoneIndex("右中指１");
				pmxBone.Parent = FindBoneIndex("右手捩");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("左足");
				pmxBone.To_Bone = FindBoneIndex("左ひざ");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("右足");
				pmxBone.To_Bone = FindBoneIndex("右ひざ");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("Bip01 Spine1a");
				pmxBone.To_Bone = FindBoneIndex("首");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("Bip01");
				pmxBone.Parent = FindBoneIndex("センタ\u30fc");
				pmxBone = FindBone("Foretwist1_L");
				pmxBone.To_Bone = FindBoneIndex("左手首");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
				pmxBone.AddRatio = 0.5f;
				pmxBone.AddParent = FindBoneIndex("左手捩");
				pmxBone = FindBone("Foretwist_L");
				pmxBone.To_Bone = FindBoneIndex("左手首");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
				pmxBone.AddRatio = 0.8f;
				pmxBone.AddParent = FindBoneIndex("左手捩");
				pmxBone = FindBone("Foretwist1_R");
				pmxBone.To_Bone = FindBoneIndex("右手首");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
				pmxBone.AddRatio = 0.5f;
				pmxBone.AddParent = FindBoneIndex("右手捩");
				pmxBone = FindBone("Foretwist_R");
				pmxBone.To_Bone = FindBoneIndex("右手首");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
				pmxBone.AddRatio = 0.8f;
				pmxBone.AddParent = FindBoneIndex("右手捩");
				pmxBone = FindBone("Uppertwist1_L");
				pmxBone.Parent = FindBoneIndex("左腕");
				pmxBone.To_Bone = FindBoneIndex("左ひじ");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
				pmxBone.AddRatio = 0.5f;
				pmxBone.AddParent = FindBoneIndex("左腕捩");
				pmxBone = FindBone("Uppertwist1_R");
				pmxBone.Parent = FindBoneIndex("右腕");
				pmxBone.To_Bone = FindBoneIndex("右ひじ");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
				pmxBone.AddRatio = 0.5f;
				pmxBone.AddParent = FindBoneIndex("右腕捩");
				pmxBone = FindBone("Kata_L");
				pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
				pmxBone.AddRatio = 0.6f;
				pmxBone.AddParent = FindBoneIndex("左腕");
				pmxBone = FindBone("Kata_R");
				pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
				pmxBone.AddRatio = 0.6f;
				pmxBone.AddParent = FindBoneIndex("右腕");
				pmxBone = FindBone("右ひざ");
				pmxBone.To_Bone = FindBoneIndex("右足首");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("左ひざ");
				pmxBone.To_Bone = FindBoneIndex("左足首");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("右足首");
				pmxBone.To_Bone = FindBoneIndex("右つま先");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("左足首");
				pmxBone.To_Bone = FindBoneIndex("左つま先");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("首");
				pmxBone.To_Bone = FindBoneIndex("Bone_Face");
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("Mune_L_sub");
				pmxBone.Name = "左AH1";
				pmxBone.To_Bone = FindBoneIndex("左AH2");
				pmxBone.Position = new PmxLib.Vector3(FindBone("Mune_L").Position);
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("Mune_L");
				pmxBone.Name = "左胸操作";
				pmxBone.Parent = FindBoneIndex("胸親");
				pmxBone.Position = FindBone("左AH1").Position + new PmxLib.Vector3(0f, 0f, -0.213f);
				pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("左AH2");
				pmxBone.Parent = FindBoneIndex("左AH1");
				pmxBone.Position = FindBone("左AH1").Position + new PmxLib.Vector3(0f, 0f, -1.515f);
				pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("Mune_R_sub");
				pmxBone.Name = "右AH1";
				pmxBone.To_Bone = FindBoneIndex("右AH2");
				pmxBone.Position = new PmxLib.Vector3(FindBone("Mune_R").Position);
				pmxBone.Flags = (PmxBone.BoneFlags.ToBone | PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("Mune_R");
				pmxBone.Name = "右胸操作";
				pmxBone.Parent = FindBoneIndex("胸親");
				pmxBone.Position = FindBone("右AH1").Position + new PmxLib.Vector3(0f, 0f, -0.213f);
				pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("右AH2");
				pmxBone.Parent = FindBoneIndex("右AH1");
				pmxBone.Position = FindBone("右AH1").Position + new PmxLib.Vector3(0f, 0f, -1.515f);
				pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("胸親");
				pmxBone.Parent = FindBoneIndex("上半身2");
				pmxBone.Position = new PmxLib.Vector3(FindBone("右胸操作").Position);
				pmxBone.Position.x = 0f;
				pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Translation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable);
				pmxBone = FindBone("Bone_Face");
				if (pmxBone != null)
				{
					pmxBone.Parent = FindBoneIndex("頭");
				}
			}
			if (FindBone("Bone_Face") != null)
			{
				pmxBone = FindBone("左目");
				if (pmxBone != null)
				{
					pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
					pmxBone.AddRatio = 0.5f;
					pmxBone.AddParent = FindBoneIndex("両目");
				}
				pmxBone = FindBone("右目");
				if (pmxBone != null)
				{
					pmxBone.Flags = (PmxBone.BoneFlags.Rotation | PmxBone.BoneFlags.Visible | PmxBone.BoneFlags.Enable | PmxBone.BoneFlags.AddRotation);
					pmxBone.AddRatio = 0.5f;
					pmxBone.AddParent = FindBoneIndex("両目");
				}
				pmxBone = FindBone("Hair_R");
				if (pmxBone != null)
				{
					pmxBone.Parent = FindBoneIndex("Bone_Face");
				}
				pmxBone = FindBone("Hair_F");
				if (pmxBone != null)
				{
					pmxBone.Parent = FindBoneIndex("Bone_Face");
				}
			}
			pmxBone = FindBone("センタ\u30fc");
			pmxBone.Parent = FindBoneIndex("全ての親");
		}

		public void ChangeMorphName()
		{
			if (FindBone("Bone_Face") != null)
			{
				PmxMorph pmxMorph = FindMorph("moutha");
				pmxMorph.Name = "あa";
				pmxMorph.Panel = 3;
				pmxMorph = FindMorph("mouthc");
				pmxMorph.Name = "うu";
				pmxMorph.Panel = 3;
				pmxMorph = FindMorph("mouthi");
				pmxMorph.Name = "いi";
				pmxMorph.Panel = 3;
				pmxMorph = FindMorph("mouths");
				pmxMorph.Name = "ワ";
				pmxMorph.Panel = 3;
				pmxMorph = FindMorph("mouthup");
				pmxMorph.Name = "にやり";
				pmxMorph.Panel = 3;
				pmxMorph = FindMorph("eyeclose");
				pmxMorph.Name = "まばたき";
				pmxMorph.Panel = 2;
				pmxMorph = FindMorph("eyeclose2");
				pmxMorph.Name = "笑い";
				pmxMorph.Panel = 2;
				pmxMorph = FindMorph("eyeclose5");
				pmxMorph.Name = "ウィンク";
				pmxMorph.Panel = 2;
				pmxMorph = FindMorph("eyeclose6");
				pmxMorph.Name = "ウィンク2";
				pmxMorph.Panel = 2;
				pmxMorph = FindMorph("eyebig");
				pmxMorph.Name = "びっくり";
				pmxMorph.Panel = 2;
				pmxMorph = FindMorph("eyeclose3");
				pmxMorph.Name = "ジト目";
				pmxMorph.Panel = 2;
				pmxMorph = FindMorph("mayuha");
				pmxMorph.Name = "困る";
				pmxMorph.Panel = 1;
				pmxMorph = FindMorph("mayuup");
				pmxMorph.Name = "にこり";
				pmxMorph.Panel = 1;
				pmxMorph = FindMorph("mayuw");
				pmxMorph.Name = "怒り";
				pmxMorph.Panel = 1;
				pmxMorph = FindMorph("mayuv");
				pmxMorph.Name = "真面目";
				pmxMorph.Panel = 1;
				pmxMorph = FindMorph("hohos");
				pmxMorph.Name = "照れ";
				pmxMorph.Panel = 4;
				pmxMorph = FindMorph("hoho");
				pmxMorph.Name = "照れ2";
				pmxMorph.Panel = 4;
				pmxMorph = FindMorph("namida");
				pmxMorph.Name = "涙";
				pmxMorph.Panel = 4;
			}
		}

		private PmxMorph FindMorph(string name)
		{
			for (int i = 0; i < pmxFile.MorphList.Count; i++)
			{
				if (pmxFile.MorphList[i].Name.Equals(name))
				{
					return pmxFile.MorphList[i];
				}
			}
			return null;
		}

		private int FindMorphIndex(string name)
		{
			for (int i = 0; i < pmxFile.MorphList.Count; i++)
			{
				if (pmxFile.MorphList[i].Name.Equals(name))
				{
					return i;
				}
			}
			return -1;
		}

		public void AddMorph()
		{
			PmxMorph pmxMorph = new PmxMorph();
			pmxMorph.Kind = PmxMorph.OffsetKind.Group;
			pmxMorph.Name = "あ";
			pmxMorph.Panel = 3;
			PmxGroupMorph pmxGroupMorph = new PmxGroupMorph();
			pmxGroupMorph.Index = FindMorphIndex("あa");
			pmxGroupMorph.Ratio = 0.6f;
			pmxMorph.OffsetList.Add(pmxGroupMorph);
			pmxFile.MorphList.Add(pmxMorph);
			pmxMorph = new PmxMorph();
			pmxMorph.Kind = PmxMorph.OffsetKind.Group;
			pmxMorph.Name = "い";
			pmxMorph.Panel = 3;
			pmxGroupMorph = new PmxGroupMorph();
			pmxGroupMorph.Index = FindMorphIndex("いi");
			pmxGroupMorph.Ratio = 0.5f;
			pmxMorph.OffsetList.Add(pmxGroupMorph);
			pmxFile.MorphList.Add(pmxMorph);
			pmxMorph = new PmxMorph();
			pmxMorph.Kind = PmxMorph.OffsetKind.Group;
			pmxMorph.Name = "う";
			pmxMorph.Panel = 3;
			pmxGroupMorph = new PmxGroupMorph();
			pmxGroupMorph.Index = FindMorphIndex("うu");
			pmxGroupMorph.Ratio = 0.6f;
			pmxMorph.OffsetList.Add(pmxGroupMorph);
			pmxGroupMorph = new PmxGroupMorph();
			pmxGroupMorph.Index = FindMorphIndex("あa");
			pmxGroupMorph.Ratio = 0.4f;
			pmxMorph.OffsetList.Add(pmxGroupMorph);
			pmxFile.MorphList.Add(pmxMorph);
			pmxMorph = new PmxMorph();
			pmxMorph.Kind = PmxMorph.OffsetKind.Group;
			pmxMorph.Name = "え";
			pmxMorph.Panel = 3;
			pmxGroupMorph = new PmxGroupMorph();
			pmxGroupMorph.Index = FindMorphIndex("いi");
			pmxGroupMorph.Ratio = 0.4f;
			pmxMorph.OffsetList.Add(pmxGroupMorph);
			pmxGroupMorph = new PmxGroupMorph();
			pmxGroupMorph.Index = FindMorphIndex("あa");
			pmxGroupMorph.Ratio = 0.4f;
			pmxMorph.OffsetList.Add(pmxGroupMorph);
			pmxFile.MorphList.Add(pmxMorph);
			pmxMorph = new PmxMorph();
			pmxMorph.Kind = PmxMorph.OffsetKind.Group;
			pmxMorph.Name = "お";
			pmxMorph.Panel = 3;
			pmxGroupMorph = new PmxGroupMorph();
			pmxGroupMorph.Index = FindMorphIndex("うu");
			pmxGroupMorph.Ratio = 0.6f;
			pmxMorph.OffsetList.Add(pmxGroupMorph);
			pmxGroupMorph = new PmxGroupMorph();
			pmxGroupMorph.Index = FindMorphIndex("あa");
			pmxGroupMorph.Ratio = 0.4f;
			pmxMorph.OffsetList.Add(pmxGroupMorph);
			pmxFile.MorphList.Add(pmxMorph);
		}

		public void BoneParentSort()
		{
			bool flag = true;
			while (flag)
			{
				flag = false;
				for (int i = 0; i < pmxFile.BoneList.Count; i++)
				{
					if (i < pmxFile.BoneList[i].Parent)
					{
						SortBone(i, pmxFile.BoneList[i].Parent);
						flag = true;
					}
				}
			}
		}

		private void SortBone(int boneindex, int parentindex)
		{
			for (int i = 0; i < pmxFile.VertexList.Count; i++)
			{
				for (int j = 0; j < pmxFile.VertexList[i].Weight.Length; j++)
				{
					if (parentindex >= pmxFile.VertexList[i].Weight[j].Bone && pmxFile.VertexList[i].Weight[j].Bone > boneindex)
					{
						pmxFile.VertexList[i].Weight[j].Bone--;
					}
					else if (pmxFile.VertexList[i].Weight[j].Bone == boneindex)
					{
						pmxFile.VertexList[i].Weight[j].Bone = parentindex;
					}
				}
			}
			for (int i = 0; i < pmxFile.BoneList.Count; i++)
			{
				if (parentindex >= pmxFile.BoneList[i].Parent && pmxFile.BoneList[i].Parent > boneindex)
				{
					pmxFile.BoneList[i].Parent--;
				}
				else if (pmxFile.BoneList[i].Parent == boneindex)
				{
					pmxFile.BoneList[i].Parent = parentindex;
				}
				if (parentindex >= pmxFile.BoneList[i].To_Bone && pmxFile.BoneList[i].To_Bone > boneindex)
				{
					pmxFile.BoneList[i].To_Bone--;
				}
				else if (pmxFile.BoneList[i].To_Bone == boneindex)
				{
					pmxFile.BoneList[i].To_Bone = parentindex;
				}
			}
			PmxBone item = pmxFile.BoneList[boneindex];
			pmxFile.BoneList.RemoveAt(boneindex);
			pmxFile.BoneList.Insert(parentindex, item);
		}

		private void SortBone2(int boneindex, int parentindex)
		{
			for (int i = 0; i < pmxFile.VertexList.Count; i++)
			{
				for (int j = 0; j < pmxFile.VertexList[i].Weight.Length; j++)
				{
					if (parentindex <= pmxFile.VertexList[i].Weight[j].Bone && pmxFile.VertexList[i].Weight[j].Bone < boneindex)
					{
						pmxFile.VertexList[i].Weight[j].Bone++;
					}
					else if (pmxFile.VertexList[i].Weight[j].Bone == boneindex)
					{
						pmxFile.VertexList[i].Weight[j].Bone = parentindex;
					}
				}
			}
			for (int i = 0; i < pmxFile.BoneList.Count; i++)
			{
				if (parentindex <= pmxFile.BoneList[i].Parent && pmxFile.BoneList[i].Parent < boneindex)
				{
					pmxFile.BoneList[i].Parent++;
				}
				else if (pmxFile.BoneList[i].Parent == boneindex)
				{
					pmxFile.BoneList[i].Parent = parentindex;
				}
				if (parentindex <= pmxFile.BoneList[i].To_Bone && pmxFile.BoneList[i].To_Bone < boneindex)
				{
					pmxFile.BoneList[i].To_Bone++;
				}
				else if (pmxFile.BoneList[i].To_Bone == boneindex)
				{
					pmxFile.BoneList[i].To_Bone = parentindex;
				}
			}
			PmxBone item = pmxFile.BoneList[boneindex];
			pmxFile.BoneList.RemoveAt(boneindex);
			pmxFile.BoneList.Insert(parentindex, item);
		}

		private void InsertBone(int index, PmxBone b)
		{
			for (int i = 0; i < pmxFile.VertexList.Count; i++)
			{
				for (int j = 0; j < pmxFile.VertexList[i].Weight.Length; j++)
				{
					if (pmxFile.VertexList[i].Weight[j].Bone >= index)
					{
						pmxFile.VertexList[i].Weight[j].Bone++;
					}
				}
			}
			for (int i = 0; i < pmxFile.BoneList.Count; i++)
			{
				if (pmxFile.BoneList[i].Parent >= index)
				{
					pmxFile.BoneList[i].Parent++;
				}
				if (pmxFile.BoneList[i].To_Bone >= index)
				{
					pmxFile.BoneList[i].To_Bone++;
				}
			}
			pmxFile.BoneList.Insert(index, b);
		}

		public void AddBody()
		{
			if (FindBone("Bip01") != null)
			{
				PmxBone pmxBone = FindBone("下半身");
				PmxBody pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.8f, 1f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("右足");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.7f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f * 1.5f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("右ひざ");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.4f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("左足");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.7f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f * 1.5f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("左ひざ");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.4f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("上半身");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + FindBone("上半身2").Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.8f, 0.7f, 0f);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("上半身2");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + FindBone("首").Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(1f, 0.7f, 0f);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("右肩");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.64f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("右腕");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.45f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("右ひじ");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.3f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("右手首");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.4f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("左肩");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.64f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("左腕");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.45f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("左ひじ");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.3f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("左手首");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.4f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("首");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.BoxSize = new PmxLib.Vector3(0.45f, getDistance(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position) / 2f, 0f);
				pmxBody.Rotation = getDirection(pmxFile.BoneList[pmxBone.To_Bone].Position, pmxBone.Position);
				pmxFile.BodyList.Add(pmxBody);
				pmxBone = FindBone("頭");
				pmxBody = new PmxBody();
				pmxBody.Name = pmxBone.Name;
				pmxBody.Bone = FindBoneIndex(pmxBone.Name);
				if (pmxBone.To_Bone != -1)
				{
					pmxBody.Position = (pmxBone.Position + pmxFile.BoneList[pmxBone.To_Bone].Position) / 2f;
				}
				else
				{
					pmxBody.Position = pmxBone.Position + pmxBone.To_Offset / 2f;
				}
				pmxBody.BoxType = PmxBody.BoxKind.Sphere;
				pmxBody.BoxSize = new PmxLib.Vector3(0.8f, 0.4f, 0f);
				pmxFile.BodyList.Add(pmxBody);
			}
		}

		private float ToRadians(double degrees)
		{
			return (float)(degrees * 3.1415926535897931 / 180.0);
		}

		private PmxLib.Vector3 getDirection(PmxLib.Vector3 first, PmxLib.Vector3 last)
		{
			PmxLib.Vector3 result = default(PmxLib.Vector3);
			float num = (float)Math.Sqrt((double)((last.X - first.X) * (last.X - first.X) + (last.Z - first.Z) * (last.Z - first.Z)));
			result.X = (float)(Math.Atan2((double)(last.Y - first.Y), (double)num) + 1.5707963267948966);
			result.Y = 0f - (float)(Math.Atan2((double)(last.Z - first.Z), (double)(last.X - first.X)) + 1.5707963267948966);
			return result;
		}

		private float getDistance(PmxLib.Vector3 one, PmxLib.Vector3 two)
		{
			return (float)Math.Sqrt((double)((one.X - two.X) * (one.X - two.X) + (one.Y - two.Y) * (one.Y - two.Y) + (one.Z - two.Z) * (one.Z - two.Z)));
		}

		public void SortMaterial()
		{
			int num = 0;
			for (int i = 0; i < pmxFile.MaterialList.Count; i++)
			{
				PmxMaterial pmxMaterial = pmxFile.MaterialList[i];
				pmxFile.MaterialList.RemoveAt(i);
				pmxFile.MaterialList.Insert(0, pmxMaterial);
				for (int num2 = num + pmxMaterial.FaceCount * 3 - 1; num2 >= num; num2--)
				{
					int item = pmxFile.FaceList[num2];
					pmxFile.FaceList.RemoveAt(num2);
					pmxFile.FaceList.Insert(num, item);
				}
				num += pmxMaterial.FaceCount * 3;
			}
		}

		public void AddPhysics()
		{
			List<PmxBone> list = new List<PmxBone>();
			List<PmxBone> list2 = new List<PmxBone>();
			for (int i = 0; i < pmxFile.BoneList.Count; i++)
			{
				if (pmxFile.BoneList[i].Name.Contains("yure") && pmxFile.BoneList[i].Name.Contains("Skirt"))
				{
					list.Add(pmxFile.BoneList[i]);
				}
				if (pmxFile.BoneList[i].Name.Contains("yure") && pmxFile.BoneList[i].Name.Contains("hair"))
				{
					list2.Add(pmxFile.BoneList[i]);
				}
			}
			for (int i = 0; i < list2.Count; i++)
			{
				if (list2[i].Parent != -1)
				{
					PmxBone pmxBone = pmxFile.BoneList[list2[i].Parent];
					PmxLib.Vector3 vector = default(PmxLib.Vector3);
					int num = FindChildIndex(list2[i].Name);
					vector = ((num == -1) ? (new PmxLib.Vector3(0f, -0.5f, 0f) + list2[i].Position) : pmxFile.BoneList[num].Position);
					PmxBody pmxBody = new PmxBody();
					pmxBody.Name = list2[i].Name;
					pmxBody.Bone = FindBoneIndex(list2[i].Name);
					pmxBody.Position = (list2[i].Position + vector) / 2f;
					pmxBody.BoxType = PmxBody.BoxKind.Capsule;
					pmxBody.Group = 2;
					pmxBody.Mass = 1f;
					pmxBody.PositionDamping = 0.9f;
					pmxBody.RotationDamping = 0.99f;
					PmxBodyPassGroup pmxBodyPassGroup = new PmxBodyPassGroup();
					pmxBodyPassGroup.Flags[2] = true;
					pmxBody.PassGroup = pmxBodyPassGroup;
					pmxBody.Mode = PmxBody.ModeType.Dynamic;
					if (pmxBone.Name == "Hair_F" || pmxBone.Name == "Hair_R")
					{
						pmxBody.Mode = PmxBody.ModeType.Static;
					}
					pmxBody.BoxSize = new PmxLib.Vector3(0.2f, getDistance(list2[i].Position, vector) / 2f, 0f);
					pmxBody.Rotation = getDirection(list2[i].Position, vector);
					pmxFile.BodyList.Add(pmxBody);
					if (FindBodyIndex(pmxBone.Name) == -1)
					{
						pmxBody.Mode = PmxBody.ModeType.Static;
					}
					else if (!(pmxBone.Name == "Hair_F") && !(pmxBone.Name == "Hair_R"))
					{
						PmxJoint pmxJoint = new PmxJoint();
						pmxJoint.Name = list2[i].Name;
						pmxJoint.Position = list2[i].Position;
						pmxJoint.Rotation = pmxBody.Rotation;
						pmxJoint.BodyA = FindBodyIndex(pmxBone.Name);
						pmxJoint.BodyB = FindBodyIndex(pmxBody.Name);
						pmxJoint.Limit_AngleLow = new PmxLib.Vector3(-0.17453292f, -0.08726646f, -0.17453292f);
						pmxJoint.Limit_AngleHigh = new PmxLib.Vector3(0.17453292f, 0.08726646f, 0.17453292f);
						pmxFile.JointList.Add(pmxJoint);
					}
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				PmxBone pmxBone = pmxFile.BoneList[list[i].Parent];
				PmxLib.Vector3 vector = default(PmxLib.Vector3);
				int num = FindChildIndex(list[i].Name);
				vector = ((num == -1) ? (new PmxLib.Vector3(0f, -0.5f, 0f) + list[i].Position) : pmxFile.BoneList[num].Position);
				PmxBody pmxBody = new PmxBody();
				pmxBody.Name = list[i].Name;
				pmxBody.Bone = FindBoneIndex(list[i].Name);
				pmxBody.Position = (list[i].Position + vector) / 2f;
				pmxBody.BoxType = PmxBody.BoxKind.Capsule;
				pmxBody.Group = 3;
				pmxBody.Mass = 1f;
				pmxBody.PositionDamping = 0.99f;
				pmxBody.RotationDamping = 0.99f;
				PmxBodyPassGroup pmxBodyPassGroup = new PmxBodyPassGroup();
				pmxBodyPassGroup.Flags[3] = true;
				pmxBody.PassGroup = pmxBodyPassGroup;
				pmxBody.Mode = PmxBody.ModeType.Dynamic;
				if (pmxBone.Name == "Skirt" || pmxBone.Name == "下半身")
				{
					pmxBody.Mode = PmxBody.ModeType.Static;
				}
				pmxBody.BoxSize = new PmxLib.Vector3(0.2f, getDistance(list[i].Position, vector) / 2f, 0f);
				pmxBody.Rotation = getDirection(list[i].Position, vector);
				pmxFile.BodyList.Add(pmxBody);
				if (!(pmxBone.Name == "Skirt") && !(pmxBone.Name == "下半身"))
				{
					PmxJoint pmxJoint = new PmxJoint();
					pmxJoint.Name = list[i].Name;
					pmxJoint.Position = list[i].Position;
					pmxJoint.Rotation = pmxBody.Rotation;
					pmxJoint.BodyA = FindBodyIndex(pmxBone.Name);
					pmxJoint.BodyB = FindBodyIndex(pmxBody.Name);
					pmxJoint.Limit_AngleLow = new PmxLib.Vector3(-0.5235988f, -0.2617994f, -0.5235988f);
					pmxJoint.Limit_AngleHigh = new PmxLib.Vector3(0.5235988f, 0.2617994f, 0.5235988f);
					pmxFile.JointList.Add(pmxJoint);
				}
			}
			int num2 = 0;
			for (int i = 0; i < list.Count; i++)
			{
				int num3 = int.Parse(string.Concat(list[i].Name[5], list[i].Name[6]));
				if (num2 < num3)
				{
					num2 = num3;
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				int num4 = int.Parse(string.Concat(list[i].Name[5], list[i].Name[6]));
				char c = list[i].Name[8];
				PmxBone pmxBone2 = null;
				int num5 = 1;
				if (num4 != num2)
				{
					num5 = num4 + 1;
				}
				for (int num3 = 0; num3 < list.Count; num3++)
				{
					if (int.Parse(string.Concat(list[num3].Name[5], list[num3].Name[6])) == num5 && list[num3].Name[8] == c)
					{
						pmxBone2 = list[num3];
					}
				}
				if (pmxBone2 != null)
				{
					PmxJoint pmxJoint = new PmxJoint();
					pmxJoint.Name = list[i].Name + "[side]";
					pmxJoint.Position = list[i].Position;
					pmxJoint.Rotation = pmxFile.BodyList[FindBodyIndex(list[i].Name)].Rotation;
					pmxJoint.BodyA = FindBodyIndex(list[i].Name);
					pmxJoint.BodyB = FindBodyIndex(pmxBone2.Name);
					pmxJoint.Limit_MoveLow = new PmxLib.Vector3(0f, 0f, 0f);
					pmxJoint.Limit_MoveHigh = new PmxLib.Vector3(0f, 0f, 0f);
					pmxJoint.Limit_AngleLow = new PmxLib.Vector3(-0.5235988f, -0.2617994f, -0.5235988f);
					pmxJoint.Limit_AngleHigh = new PmxLib.Vector3(0.5235988f, 0.2617994f, 0.5235988f);
					pmxFile.JointList.Add(pmxJoint);
				}
			}
		}

		private int FindChildIndex(string name)
		{
			for (int i = 0; i < pmxFile.BoneList.Count; i++)
			{
				if (pmxFile.BoneList[i].Parent != -1 && pmxFile.BoneList[pmxFile.BoneList[i].Parent].Name == name)
				{
					return i;
				}
			}
			return -1;
		}

		private int FindBodyIndex(string name)
		{
			for (int i = 0; i < pmxFile.BodyList.Count; i++)
			{
				if (pmxFile.BodyList[i].Name == name)
				{
					return i;
				}
			}
			return -1;
		}

		private void phymune()
		{
		}
	}
}
