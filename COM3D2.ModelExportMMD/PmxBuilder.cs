using COM3D2.ModelExportMMD.Gui;
using COM3D2.ModelExportMMD.Util;
using PmxLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace COM3D2.ModelExportMMD
{
    public class PmxBuilder
    {
        #region Constants

        private const float ScaleFactor = 8;

        #endregion

        #region Fields

        private readonly Pmx pmxFile;
        private readonly List<Transform> boneList = new List<Transform>();
        private readonly List<int> boneParent = new List<int>();
        private readonly List<Matrix4x4> bindposeList = new List<Matrix4x4>();
        private Dictionary<string, int> bonesMap;
        private int vertexCount = 0;

        #endregion

        #region Constructors

        public PmxBuilder()
        {
            if (!Directory.Exists(ExportWindow.ExportFolder))
            {
                Directory.CreateDirectory(ExportWindow.ExportFolder);
            }
            this.pmxFile = new Pmx();
        }

        #endregion

        #region Methods

        public void CreatePmxHeader()
        {
            PmxElementFormat pmxElementFormat = new PmxElementFormat(1f);
            pmxElementFormat.VertexSize = PmxElementFormat.GetUnsignedBufSize(this.pmxFile.VertexList.Count);
            int val = -2147483648;
            for (int i = 0; i < this.pmxFile.BoneList.Count; i++)
            {
                val = Math.Max(val, Math.Abs(this.pmxFile.BoneList[i].IK.LinkList.Count));
            }
            val = Math.Max(val, this.pmxFile.BoneList.Count);
            pmxElementFormat.BoneSize = PmxElementFormat.GetSignedBufSize(val);
            if (pmxElementFormat.BoneSize < 2)
            {
                pmxElementFormat.BoneSize = 2;
            }
            pmxElementFormat.MorphSize = PmxElementFormat.GetUnsignedBufSize(this.pmxFile.MorphList.Count);
            pmxElementFormat.MaterialSize = PmxElementFormat.GetUnsignedBufSize(this.pmxFile.MaterialList.Count);
            pmxElementFormat.BodySize = PmxElementFormat.GetUnsignedBufSize(this.pmxFile.BodyList.Count);
            PmxHeader pmxHeader = new PmxHeader(2.1f);
            pmxHeader.FromElementFormat(pmxElementFormat);
            this.pmxFile.Header = pmxHeader;
        }

        public void CreateModelInfo()
        {
            PmxModelInfo pmxModelInfo = new PmxModelInfo();
            pmxModelInfo.ModelName = "妹抖";
            pmxModelInfo.ModelNameE = "maid";
            pmxModelInfo.Comment = "我的妹抖";
            pmxModelInfo.CommentE = "my maid";
            this.pmxFile.ModelInfo = pmxModelInfo;
        }

        public PmxVertex.BoneWeight[] ConvertBoneWeight(BoneWeight unityWeight, Transform[] bones, SkinQuality quality)
        {
            int n = (int)quality;
            if (n < 1) n = 1;
            PmxVertex.BoneWeight[] array = new PmxVertex.BoneWeight[n];
            array[0].Bone = bonesMap[bones[unityWeight.boneIndex0].name];
            array[0].Value = unityWeight.weight0;
            if (n >= 2)
            {
                array[1].Bone = bonesMap[bones[unityWeight.boneIndex1].name];
                array[1].Value = unityWeight.weight1;
            }
            if (n >= 4)
            {
                array[2].Bone = bonesMap[bones[unityWeight.boneIndex2].name];
                array[2].Value = unityWeight.weight2;
                array[3].Bone = bonesMap[bones[unityWeight.boneIndex3].name];
                array[3].Value = unityWeight.weight3;
            }
            return array;
        }

        private void AddFaceList(int[] faceList, int count)
        {
            for (int i = 0; i < faceList.Length; i++)
            {
                faceList[i] += count;
                this.pmxFile.FaceList.Add(faceList[i]);
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

        private PmxLib.Vector3 ToPmxVec3(UnityEngine.Vector3 v)
        {
            return new PmxLib.Vector3(-v.x, v.y, -v.z);
        }

        public void CreateMeshList(SkinnedMeshRenderer meshRender)
        {
            GameObject gameObject = meshRender.gameObject;
            Mesh mesh = meshRender.sharedMesh;
            BoneWeight[] boneWeights = mesh.boneWeights;
            if (ExportWindow.SavePostion)
            {
                Mesh mesh2 = new Mesh();
                meshRender.BakeMesh(mesh2);
                mesh = mesh2;
            }
            UnityEngine.Vector2[] uv = mesh.uv;
            UnityEngine.Vector2[] uv2 = mesh.uv2;
            UnityEngine.Vector3[] normals = mesh.normals;
            UnityEngine.Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] triangles = mesh.GetTriangles(i);
                this.AddFaceList(triangles, this.vertexCount);
                this.CreateMaterial(meshRender.sharedMaterials[i], triangles.Length);
            }
            this.vertexCount += mesh.vertexCount;
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                PmxVertex pmxVertex = new PmxVertex();
                pmxVertex.UV = new PmxLib.Vector2(uv[i].x, -uv[i].y);
                pmxVertex.Weight = this.ConvertBoneWeight(boneWeights[i], meshRender.bones, meshRender.quality);
                Transform t = gameObject.transform;
                UnityEngine.Vector3 n = normals[i];
                n = t.TransformDirection(n);
                pmxVertex.Normal = ToPmxVec3(n);
                UnityEngine.Vector3 v = vertices[i];
                v = t.TransformPoint(v);
                v *= ScaleFactor;
                pmxVertex.Position = ToPmxVec3(v);
                this.pmxFile.VertexList.Add(pmxVertex);
            }
        }

        private UnityEngine.Vector3 TransToParent(UnityEngine.Vector3 v, int index)
        {
            Transform transform = this.boneList[index];
            int num = -1;
            if (this.bonesMap.ContainsKey(transform.parent.name))
            {
                num = this.bonesMap[transform.parent.name];
            }
            if (num != -1)
            {
                Matrix4x4 matrix4x = default(Matrix4x4);
                matrix4x = this.bindposeList[index] * this.boneList[num].worldToLocalMatrix.inverse;
                v = matrix4x.MultiplyVector(v);
                v = this.TransToParent(v, num);
            }
            return v;
        }

        private UnityEngine.Vector3 CalcPostion(UnityEngine.Vector3 v, BoneWeight boneWeight, Transform[] bones)
        {
            Transform key = bones[boneWeight.boneIndex0];
            if (this.bonesMap.ContainsKey(key.name))
            {
                int index = this.bonesMap[key.name];
                v = this.TransToParent(v, index);
            }
            return v;
        }

        public void PrepareData(List<SkinnedMeshRenderer> skinnedMeshList)
        {
            this.bonesMap = new Dictionary<string, int>();
            foreach (SkinnedMeshRenderer skinnedMesh in skinnedMeshList)
            {
                Debug.Log("Processing bones of " + skinnedMesh.name);
                for (int i = 0; i < skinnedMesh.bones.Length; i++)
                {
                    for (Transform bone = skinnedMesh.bones[i]; bone != null; bone = bone.parent)
                    {
                        if (bone.name.Equals(skinnedMesh.name)) break;
                        if (bone.name.Equals(skinnedMesh.sharedMesh.name)) break;
                        if (bone.name.StartsWith("_SM_")) break;
                        if (!this.bonesMap.ContainsKey(bone.name))
                        {
                            this.bonesMap[bone.name] = this.boneList.Count;
                            this.boneList.Add(bone);
                            this.boneParent.Add(-1);
                            this.bindposeList.Add(skinnedMesh.sharedMesh.bindposes[i]);
                        }
                    }
                }
                for (int i = 0; i < skinnedMesh.bones.Length; i++)
                {
                    Transform bone = skinnedMesh.bones[i];
                    if (bone.parent == null)
                    {
                        Debug.Log("Bone " + bone.name + " has no parent");
                        continue;
                    }
                    if (bone.parent.name.StartsWith("_SM_")) continue;
                    int j = this.bonesMap[bone.name];
                    if (this.bonesMap.ContainsKey(bone.parent.name))
                    {
                        int k = this.bonesMap[bone.parent.name];
                        if (this.boneParent[j] == -1)
                        {
                            Debug.Log("Bone " + bone.name + " parented to " + bone.parent.name + "(" + k + ")");
                            this.boneParent[j] = k;
                        }
                        else if (this.boneParent[j] != k)
                        {
                            Debug.Log("Warning: bone " + bone.name + " was parented to " + this.boneList[this.boneParent[j]].name + " but was also found parented to " + bone.parent.name);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Bone " + bone.name + " parented to " + bone.parent.name + "(INDEX NOT FOUND)");
                    }
                }
            }
            Debug.Log("boneList:" + this.boneList.Count);
            Debug.Log("bindposeList:" + this.bindposeList.Count);
        }

        public void CreateBoneList()
        {
            for (int i = 0; i < this.boneList.Count; i++)
            {
                Transform bone = this.boneList[i];
                PmxBone pmxBone = new PmxBone();
                pmxBone.Name = bone.name;
                pmxBone.NameE = bone.name;
                if (this.boneParent[i] >= 0)
                {
                    pmxBone.Parent = this.boneParent[i];
                }
                UnityEngine.Vector3 vector = bone.position * ScaleFactor;
                pmxBone.Position = ToPmxVec3(vector);
                this.pmxFile.BoneList.Add(pmxBone);
            }
        }

        public PmxBody CreateColliderBody(Collider rigidbody)
        {
            PmxBody pmxBody = new PmxBody();
            pmxBody.Name = rigidbody.name;
            pmxBody.NameE = rigidbody.name;
            pmxBody.Position.X = rigidbody.transform.position.x;
            pmxBody.Position.Y = rigidbody.transform.position.y;
            pmxBody.Position.Z = rigidbody.transform.position.z;
            pmxBody.Rotation.X = rigidbody.transform.rotation.eulerAngles.x;
            pmxBody.Rotation.Y = rigidbody.transform.rotation.eulerAngles.y;
            pmxBody.Rotation.Z = rigidbody.transform.rotation.eulerAngles.z;
            return pmxBody;
        }

        public void CreateMaterial(Material material, int count)
        {
            PmxMaterial pmxMaterial = new PmxMaterial();
            pmxMaterial.Name = material.name;
            pmxMaterial.NameE = material.name;
            pmxMaterial.Flags = (PmxMaterial.MaterialFlags.DrawBoth | PmxMaterial.MaterialFlags.Shadow | PmxMaterial.MaterialFlags.SelfShadowMap | PmxMaterial.MaterialFlags.SelfShadow);
            if (material.mainTexture != null)
            {
                string text = material.name;
                Debug.Log("Generate Material : " + text);
                if (text.Contains("Instance"))
                {
                    object obj = text;
                    text = obj + "_(" + material.GetInstanceID() + ")";
                }
                pmxMaterial.Tex = text + ".png";
                Texture mainTexture = material.mainTexture;
                if (ExportWindow.SaveTexture)
                {
                    TextureWriter.WriteTexture2D(ExportWindow.ExportFolder + "/" + pmxMaterial.Tex, mainTexture);
                }
            }
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
            }
            if (material.HasProperty("_OutlineColor"))
            {
                pmxMaterial.EdgeSize = material.GetFloat("_OutlineWidth");
                pmxMaterial.EdgeColor = material.GetColor("_OutlineColor");
            }
            pmxMaterial.FaceCount = count;
            this.pmxFile.MaterialList.Add(pmxMaterial);
        }

        public void Save(string filename)
        {
            this.pmxFile.ToFile(filename);
        }

        #endregion
    }
}