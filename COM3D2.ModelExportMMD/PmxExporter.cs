using PmxLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.ModelExportMMD
{
    public class PmxExporterOld : IExporter
    {
        #region Types
        private class MaterialInfo
        {
            public string shader;
            public float[] color;
            public string mainTex;
            public string shadowTex;
            public float[] shadowColor;
            public string shadowRateToon;
            public string toonRamp;
            public float shininess;
            public float[] rimColor;
            public float rimPower;
            public float rimShift;
            public string hiTex;
            public float hiRate;
            public float hiPow;
            public float[] outlineColor;
            public string outlineTex;
            public string outlineToonRamp;
            public float outlineWidth;
            public string outlineWidthTex;
            public float zTest;
            public float zTest2;
            public float zTest2Alpha;
        };
        #endregion

        #region Constants

        private const float scaleFactor = 8f;

        #endregion

        #region Fields

        private readonly Pmx pmxFile = new Pmx();
        private readonly List<Transform> boneList = new List<Transform>();
        private readonly List<int> boneParent = new List<int>();
        private readonly List<Matrix4x4> bindposeList = new List<Matrix4x4>();
        private readonly Dictionary<string, int> bonesMap = new Dictionary<string, int>();
        private readonly TextureBuilder textureBuilder = new TextureBuilder();
        private readonly Dictionary<string, MaterialInfo> materialInfo = new Dictionary<string, MaterialInfo>();
        private int vertexCount = 0;

        #endregion

        #region Properties

        public ModelFormat ExportFormat { get { return ModelFormat.Pmx; } }
        public string ExportFolder { get; set; }
        public string ExportName { get; set; }
        public bool SavePosition { get; set; } = true;
        public bool SaveTexture { get; set; } = true;

        #endregion

        #region Constructors

        public PmxExporterOld()
        {
            pmxFile.ModelInfo.ModelName = "妹抖";
            pmxFile.ModelInfo.ModelNameE = "maid";
            pmxFile.ModelInfo.Comment = "我的妹抖";
            pmxFile.ModelInfo.CommentE = "my maid";
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
            weights[0].Bone = bonesMap[bones[unityWeight.boneIndex0].name];
            weights[0].Value = unityWeight.weight0;
            if (quality >= SkinQuality.Bone2)
            {
                weights[1].Bone = bonesMap[bones[unityWeight.boneIndex1].name];
                weights[1].Value = unityWeight.weight1;
            }
            if (quality >= SkinQuality.Bone4)
            {
                weights[2].Bone = bonesMap[bones[unityWeight.boneIndex2].name];
                weights[2].Value = unityWeight.weight2;
                weights[3].Bone = bonesMap[bones[unityWeight.boneIndex3].name];
                weights[3].Value = unityWeight.weight3;
            }

            return weights;
        }

        private void AddFaceList(int[] faceList, int count)
        {
            for (int i = 0; i < faceList.Length; i++)
            {
                faceList[i] += count;
                pmxFile.FaceList.Add(faceList[i]);
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
            if (SavePosition)
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
                AddFaceList(triangles, vertexCount);
                CreateMaterial(meshRender.materials[i], triangles.Length);
            }
            vertexCount += mesh.vertexCount;
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                PmxVertex pmxVertex = new PmxVertex();
                pmxVertex.UV = new PmxLib.Vector2(uv[i].x, -uv[i].y);
                pmxVertex.Weight = ConvertBoneWeight(boneWeights[i], meshRender.bones, meshRender.quality);
                Transform t = gameObject.transform;
                UnityEngine.Vector3 n = normals[i];
                n = t.TransformDirection(n);
                pmxVertex.Normal = ToPmxVec3(n);
                UnityEngine.Vector3 v = vertices[i];
                v = t.TransformPoint(v);
                v *= scaleFactor;
                pmxVertex.Position = ToPmxVec3(v);
                pmxFile.VertexList.Add(pmxVertex);
            }
        }

        private UnityEngine.Vector3 TransToParent(UnityEngine.Vector3 v, int index)
        {
            var transform = boneList[index];

            if (bonesMap.ContainsKey(transform.parent.name))
            {
                int parentIndex = bonesMap[transform.parent.name];
                if (parentIndex != -1)
                {
                    Matrix4x4 matrix4x = bindposeList[index] * boneList[parentIndex].worldToLocalMatrix.inverse;
                    v = matrix4x.MultiplyVector(v);
                    v = TransToParent(v, parentIndex);
                }
            }

            return v;
        }

        private void PrepareData(List<SkinnedMeshRenderer> skinnedMeshes)
        {
            bonesMap.Clear();
            boneList.Clear();
            boneParent.Clear();
            bindposeList.Clear();

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
                                !bonesMap.ContainsKey(bone.name))
                        {
                            boneList.Add(bone);
                            boneParent.Add(-1);
                            bindposeList.Add(skinnedMesh.sharedMesh.bindposes[i]);
                        }
                    }
                }

                Debug.Log($"Mapping bone parents of {skinnedMesh.name}");

                for (int i = 0; i < skinnedMesh.bones.Length; i++)
                {
                    Transform bone = skinnedMesh.bones[i];
                    if (bone == null || string.IsNullOrEmpty(bone.name))
                        continue;
                    if (!bonesMap.TryGetValue(bone.name, out int j))
                        continue;
                    if (bone.parent == null || string.IsNullOrEmpty(bone.parent.name) || bone.parent.name.StartsWith("_SM_"))
                    {
                        Debug.Log($"Bone {bone.name} has no parent");
                        continue;
                    }
                    if (bonesMap.ContainsKey(bone.parent.name))
                    {
                        int k = bonesMap[bone.parent.name];
                        if (boneParent[j] == -1)
                        {
                            Debug.Log($"Bone {bone.name} parented to {bone.parent.name}({k})");
                            boneParent[j] = k;
                        }
                        else if (boneParent[j] != k)
                        {
                            Debug.Log($"Warning: bone {bone.name} was parented to {boneList[boneParent[j]].name} but was also found parented to {bone.parent.name}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Bone {bone.name} parented to {bone.parent.name} but bone parent index not found");
                    }
                }
            }

            Debug.Log($"Bone Count: {boneList.Count} Bindpose Count: {bindposeList.Count}");
        }

        private void CreateBoneList()
        {
            for (int i = 0; i < boneList.Count; i++)
            {
                Transform bone = boneList[i];
                PmxBone pmxBone = new PmxBone();
                pmxBone.Name = bone.name;
                pmxBone.NameE = bone.name;
                if (boneParent[i] >= 0)
                {
                    pmxBone.Parent = boneParent[i];
                }
                UnityEngine.Vector3 vector = bone.position * scaleFactor;
                pmxBone.Position = ToPmxVec3(vector);
                pmxFile.BoneList.Add(pmxBone);
            }
        }

        private void SetMaterialInfoProperty(out float value, Material material, string property, float defaultValue=-1)
        {
            if (material.HasProperty(property))
            {
                value = material.GetFloat(property);
            }
            else
            {
                value = defaultValue;
            }
        }

        private void SetMaterialInfoProperty(out float[] value, Material material, string property, float[] defaultValue=null)
        {
            if (material.HasProperty(property))
            {
                UnityEngine.Vector4 v;
                v = material.GetVector(property);
                value = new float[4];
                value[0] = v.x;
                value[1] = v.y;
                value[2] = v.z;
                value[3] = v.w;
            }
            else
            {
                value = defaultValue;
            }
        }

        private void CreateMaterial(Material material, int count)
        {
            PmxMaterial pmxMaterial = new PmxMaterial();
            pmxMaterial.Name = material.name;
            pmxMaterial.NameE = material.name;
            pmxMaterial.Flags = (PmxMaterial.MaterialFlags.DrawBoth | PmxMaterial.MaterialFlags.Shadow | PmxMaterial.MaterialFlags.SelfShadowMap | PmxMaterial.MaterialFlags.SelfShadow);
            MaterialInfo info = new MaterialInfo();
            materialInfo[material.name] = info;
            info.shader = material.shader.name;
            /* Uncomment to dump list of material textures (put one / at the start of this line)
            if (true)//material.name.StartsWith("Face011_GP_Skin"))
            {
                Debug.Log($"Material {material.name} uses shader {material.shader.name} ({material.shader.GetInstanceID()}) and textures:");
                for (int i = 0; i < 99999; i++)
                {
                    Texture t = material.GetTexture(i);
                    if (t == null)
                    {
                        continue;
                    }
                    Debug.Log($"  ({i,5}) {t.name} [{t.GetType().Name}]");
                }
            }
            // */
            if (material.HasProperty("_MainTex"))
            {
                Texture mainTexture = material.GetTexture("_MainTex");
                if (mainTexture == null)
                {
                    mainTexture = material.mainTexture;
                }
                if (mainTexture != null && SaveTexture)
                {
                    Debug.Log($"Generate Material: {material.name} {mainTexture.name}");
                    pmxMaterial.Tex = textureBuilder.Export(ExportFolder, material, "_MainTex", mainTexture);
                }
                info.mainTex = pmxMaterial.Tex;
            }
            SetMaterialInfoProperty(out info.color, material, "_Color");
            info.shadowTex = textureBuilder.Export(ExportFolder, material, "_ShadowTex");
            SetMaterialInfoProperty(out info.shadowColor, material, "_ShadowColor");
            info.shadowRateToon= textureBuilder.Export(ExportFolder, material, "_ShadowRateToon");
            pmxMaterial.Toon = textureBuilder.Export(ExportFolder, material, "_ToonRamp");
            info.toonRamp = pmxMaterial.Toon;
            SetMaterialInfoProperty(out info.shininess, material, "_Shininess");
            SetMaterialInfoProperty(out info.rimColor, material, "_RimColor");
            SetMaterialInfoProperty(out info.rimPower, material, "_RimPower");
            SetMaterialInfoProperty(out info.rimShift, material, "_RimShift");
            info.hiTex = textureBuilder.Export(ExportFolder, material, "_HiTex");
            SetMaterialInfoProperty(out info.hiRate, material, "_HiRate");
            SetMaterialInfoProperty(out info.hiPow, material, "_HiPow");
            SetMaterialInfoProperty(out info.outlineColor, material, "_OutlineColor");
            info.outlineTex = textureBuilder.Export(ExportFolder, material, "_OutlineTex");
            info.outlineToonRamp = textureBuilder.Export(ExportFolder, material, "_OutlineToonRamp");
            SetMaterialInfoProperty(out info.outlineWidth, material, "_OutlineWidth");
            info.outlineWidthTex = textureBuilder.Export(ExportFolder, material, "_OutlineWidthTex");
            SetMaterialInfoProperty(out info.zTest, material, "_ZTest");
            SetMaterialInfoProperty(out info.zTest2, material, "_ZTest2");
            SetMaterialInfoProperty(out info.zTest2Alpha, material, "_ZTest2Alpha");
            if (material.HasProperty("_Color"))
            {
                pmxMaterial.Diffuse = new PmxLib.Vector4(material.GetColor("_Color"));
            }
            if (material.HasProperty("_Opacity"))
            {
                pmxMaterial.Diffuse.Alpha = material.GetFloat("_Opacity");
            }
            pmxMaterial.FaceCount = count;
            pmxFile.MaterialList.Add(pmxMaterial);
        }

        private void Save()
        {
            PmxElementFormat pmxElementFormat = new PmxElementFormat(1f);
            pmxElementFormat.VertexSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.VertexList.Count);
            int val = -2147483648;
            for (int i = 0; i < pmxFile.BoneList.Count; i++)
                val = Math.Max(val, Math.Abs(pmxFile.BoneList[i].IK.LinkList.Count));
            val = Math.Max(val, pmxFile.BoneList.Count);
            pmxElementFormat.BoneSize = PmxElementFormat.GetSignedBufSize(val);
            if (pmxElementFormat.BoneSize < 2)
                pmxElementFormat.BoneSize = 2;
            pmxElementFormat.MorphSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.MorphList.Count);
            pmxElementFormat.MaterialSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.MaterialList.Count);
            pmxElementFormat.BodySize = PmxElementFormat.GetUnsignedBufSize(pmxFile.BodyList.Count);
            pmxFile.Header.FromElementFormat(pmxElementFormat);
            pmxFile.ToFile(Path.Combine(ExportFolder, ExportName + ".pmx"));
            StreamWriter writer = new StreamWriter(Path.Combine(ExportFolder, ExportName + ".json"));
            string jsonInfo = JsonConvert.SerializeObject(materialInfo, Formatting.Indented);
            writer.Write(jsonInfo);
            writer.Close();
        }

        public void Export(List<SkinnedMeshRenderer> skinnedMeshes)
        {
            Directory.CreateDirectory(ExportFolder);

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