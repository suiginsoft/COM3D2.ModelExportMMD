using PmxLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace COM3D2.ModelExportMMD
{
    public class PmxExporter : IExporter
    {
        #region Constants

        private const float scaleFactor = 8f;

        #endregion

        #region Fields

        private readonly Pmx pmxFile = new Pmx();
        private readonly List<Transform> boneList = new List<Transform>();
        private readonly List<int> boneParent = new List<int>();
        private readonly List<Matrix4x4> bindposeList = new List<Matrix4x4>();
        private readonly Dictionary<string, int> bonesMap = new Dictionary<string, int>();
        private int vertexCount = 0;

        #endregion

        #region Properties

        public ModelFormat ExportFormat { get { return ModelFormat.Pmx; } }
        public string ExportFolder { get; set; }
        public string ExportName { get; set; }
        public bool SavePostion { get; set; } = true;
        public bool SaveTexture { get; set; } = true;

        #endregion

        #region Constructors

        public PmxExporter()
        {
            this.pmxFile.ModelInfo.ModelName = "妹抖";
            this.pmxFile.ModelInfo.ModelNameE = "maid";
            this.pmxFile.ModelInfo.Comment = "我的妹抖";
            this.pmxFile.ModelInfo.CommentE = "my maid";
        }

        #endregion

        #region Methods

        private PmxVertex.BoneWeight[] ConvertBoneWeight(BoneWeight unityWeight, Transform[] bones, SkinQuality quality)
        {
            int boneCount = (int)quality;
            if (boneCount < 1)
            {
                boneCount = 1;
            }

            var weights = new PmxVertex.BoneWeight[boneCount];
            weights[0].Bone = this.bonesMap[bones[unityWeight.boneIndex0].name];
            weights[0].Value = unityWeight.weight0;

            if (boneCount >= 2)
            {
                weights[1].Bone = this.bonesMap[bones[unityWeight.boneIndex1].name];
                weights[1].Value = unityWeight.weight1;
            }

            if (boneCount >= 4)
            {
                weights[2].Bone = this.bonesMap[bones[unityWeight.boneIndex2].name];
                weights[2].Value = unityWeight.weight2;
                weights[3].Bone = this.bonesMap[bones[unityWeight.boneIndex3].name];
                weights[3].Value = unityWeight.weight3;
            }

            return weights;
        }

        private void AddFaceList(int[] faceList, int count)
        {
            for (int i = 0; i < faceList.Length; i++)
            {
                faceList[i] += count;
                this.pmxFile.FaceList.Add(faceList[i]);
            }
        }

        private static PmxLib.Vector3 ToPmxVec3(UnityEngine.Vector3 v)
        {
            return new PmxLib.Vector3(-v.x, v.y, -v.z);
        }

        private void CreateMeshList(SkinnedMeshRenderer meshRender)
        {
            GameObject gameObject = meshRender.gameObject;
            Mesh mesh = meshRender.sharedMesh;
            BoneWeight[] boneWeights = mesh.boneWeights;
            if (this.SavePostion)
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
                this.CreateMaterial(meshRender.materials[i], triangles.Length);
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
                v *= scaleFactor;
                pmxVertex.Position = ToPmxVec3(v);
                this.pmxFile.VertexList.Add(pmxVertex);
            }
        }

        private UnityEngine.Vector3 TransToParent(UnityEngine.Vector3 v, int index)
        {
            var transform = this.boneList[index];

            if (this.bonesMap.ContainsKey(transform.parent.name))
            {
                int parentIndex = this.bonesMap[transform.parent.name];
                if (parentIndex != -1)
                {
                    Matrix4x4 matrix4x = this.bindposeList[index] * this.boneList[parentIndex].worldToLocalMatrix.inverse;
                    v = matrix4x.MultiplyVector(v);
                    v = this.TransToParent(v, parentIndex);
                }
            }

            return v;
        }

        private void PrepareData(List<SkinnedMeshRenderer> skinnedMeshes)
        {
            this.bonesMap.Clear();
            this.boneList.Clear();
            this.boneParent.Clear();
            this.bindposeList.Clear();

            foreach (var skinnedMesh in skinnedMeshes)
            {
                Debug.Log($"Processing bones of {skinnedMesh.name}");

                for (int i = 0; i < skinnedMesh.bones.Length; i++)
                {
                    for (Transform bone = skinnedMesh.bones[i]; bone != null; bone = bone.parent)
                    {
                        if (!string.IsNullOrEmpty(bone.name) &&
                                !bone.name.Equals(skinnedMesh.name) &&
                                !bone.name.Equals(skinnedMesh.sharedMesh.name) &&
                                !bone.name.StartsWith("_SM_") &&
                                !this.bonesMap.ContainsKey(bone.name))
                        {
                            this.bonesMap[bone.name] = this.boneList.Count;
                            this.boneList.Add(bone);
                            this.boneParent.Add(-1);
                            this.bindposeList.Add(skinnedMesh.sharedMesh.bindposes[i]);
                        }
                    }
                }

                Debug.Log($"Mapping bone parents of {skinnedMesh.name}");

                for (int i = 0; i < skinnedMesh.bones.Length; i++)
                {
                    Transform bone = skinnedMesh.bones[i];
                    if (bone == null || string.IsNullOrEmpty(bone.name))
                        continue;
                    if (!this.bonesMap.TryGetValue(bone.name, out int j))
                        continue;
                    if (bone.parent == null || string.IsNullOrEmpty(bone.parent.name) || bone.parent.name.StartsWith("_SM_"))
                    {
                        Debug.Log($"Bone {bone.name} has no parent");
                        continue;
                    }
                    if (this.bonesMap.ContainsKey(bone.parent.name))
                    {
                        int k = this.bonesMap[bone.parent.name];
                        if (this.boneParent[j] == -1)
                        {
                            Debug.Log($"Bone {bone.name} parented to {bone.parent.name}({k})");
                            this.boneParent[j] = k;
                        }
                        else if (this.boneParent[j] != k)
                        {
                            Debug.Log($"Warning: bone {bone.name} was parented to {this.boneList[this.boneParent[j]].name} but was also found parented to {bone.parent.name}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Bone {bone.name} parented to {bone.parent.name} but bone parent index not found");
                    }
                }
            }

            Debug.Log($"Bone Count: {this.boneList.Count} Bindpose Count: {this.bindposeList.Count}");
        }

        private void CreateBoneList()
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
                UnityEngine.Vector3 vector = bone.position * scaleFactor;
                pmxBone.Position = ToPmxVec3(vector);
                this.pmxFile.BoneList.Add(pmxBone);
            }
        }

        private void CreateMaterial(Material material, int count)
        {
            PmxMaterial pmxMaterial = new PmxMaterial();
            pmxMaterial.Name = material.name;
            pmxMaterial.NameE = material.name;
            pmxMaterial.Flags = (PmxMaterial.MaterialFlags.DrawBoth | PmxMaterial.MaterialFlags.Shadow | PmxMaterial.MaterialFlags.SelfShadowMap | PmxMaterial.MaterialFlags.SelfShadow);
            if (material.HasProperty("_MainTex"))
            {
                string textureName = material.name;
                if (textureName.Contains("Instance"))
                {
                    textureName = material.name + "_(" + material.GetInstanceID() + ")";
                }
                pmxMaterial.Tex = textureName + ".png";
                Texture mainTexture = material.GetTexture("_MainTex");
                if (mainTexture == null)
                {
                    mainTexture = material.mainTexture;
                }
                if (mainTexture != null && this.SaveTexture)
                {
                    Debug.Log($"Generate Material: {material.name} {mainTexture.name}");
                    TextureBuilder.WriteTextureToFile(Path.Combine(this.ExportFolder, pmxMaterial.Tex), mainTexture);
                }
            }
            if (material.HasProperty("_AmbColor"))
            {
                pmxMaterial.Ambient = new PmxLib.Vector3(material.GetColor("_AmbColor"));
            }
            if (material.HasProperty("_Color"))
            {
                pmxMaterial.Diffuse = new PmxLib.Vector4(material.GetColor("_Color"));
            }
            if (material.HasProperty("_Opacity"))
            {
                pmxMaterial.Diffuse.Alpha = material.GetFloat("_Opacity");
            }
            if (material.HasProperty("_SpecularColor"))
            {
                pmxMaterial.Specular = new PmxLib.Vector3(material.GetColor("_SpecularColor"));
            }
            if (material.HasProperty("_Shininess"))
            {
                pmxMaterial.Power = material.GetFloat("_Shininess");
            }
            if (material.HasProperty("_OutlineColor"))
            {
                pmxMaterial.EdgeSize = material.GetFloat("_OutlineWidth");
                pmxMaterial.EdgeColor = new PmxLib.Vector4(material.GetColor("_OutlineColor"));
            }
            pmxMaterial.FaceCount = count;
            this.pmxFile.MaterialList.Add(pmxMaterial);
        }

        private void Save()
        {
            PmxElementFormat pmxElementFormat = new PmxElementFormat(2.1f);
            pmxElementFormat.VertexSize = PmxElementFormat.GetUnsignedBufSize(this.pmxFile.VertexList.Count);
            int val = -2147483648;
            for (int i = 0; i < this.pmxFile.BoneList.Count; i++)
                val = Math.Max(val, Math.Abs(this.pmxFile.BoneList[i].IK.LinkList.Count));
            val = Math.Max(val, this.pmxFile.BoneList.Count);
            pmxElementFormat.BoneSize = PmxElementFormat.GetSignedBufSize(val);
            if (pmxElementFormat.BoneSize < 2)
                pmxElementFormat.BoneSize = 2;
            pmxElementFormat.MorphSize = PmxElementFormat.GetUnsignedBufSize(this.pmxFile.MorphList.Count);
            pmxElementFormat.MaterialSize = PmxElementFormat.GetUnsignedBufSize(this.pmxFile.MaterialList.Count);
            pmxElementFormat.BodySize = PmxElementFormat.GetUnsignedBufSize(this.pmxFile.BodyList.Count);
            this.pmxFile.Header.FromElementFormat(pmxElementFormat);
            this.pmxFile.ToFile(Path.Combine(this.ExportFolder, this.ExportName + ".pmx"));
        }

        public void Export(List<SkinnedMeshRenderer> skinnedMeshes)
        {
            Directory.CreateDirectory(this.ExportFolder);

            PrepareData(skinnedMeshes);
            CreateBoneList();

            foreach (var skinnedMesh in skinnedMeshes)
            {
                CreateMeshList(skinnedMesh);
            }

            Save();
        }

        #endregion
    }
}