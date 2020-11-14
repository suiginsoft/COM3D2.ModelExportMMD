using PmxLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.ModelExportMMD
{
    public class PmxExporter : IExporter
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
            public float[] rimColor;
            public float rimPower;
            public float rimShift;
            public string hiTex;
            public float hiRate;
            public float hiPow;
            public float[] outlineColor;
            public float outlineWidth;
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
        private readonly Dictionary<string, int> vertexIndexMap = new Dictionary<string, int>();
        private readonly Dictionary<string, PmxMorph> morphMap = new Dictionary<string, PmxMorph>();
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
            if (boneCount > 1)
            {
                weights[1].Bone = bonesMap[bones[unityWeight.boneIndex1].name];
                weights[1].Value = unityWeight.weight1;
            }
            if (boneCount > 2)
            {
                weights[2].Bone = bonesMap[bones[unityWeight.boneIndex2].name];
                weights[2].Value = unityWeight.weight2;
            }
            if (boneCount > 3)
            {
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
            vertexIndexMap.Add(gameObject.transform.parent.gameObject.name, pmxFile.VertexList.Count);
            if (SavePostion)
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
            vertexIndexMap.Clear();
            morphMap.Clear();

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
                            bonesMap[bone.name] = boneList.Count;
                            boneList.Add(bone);
                            boneParent.Add(-1);
                            bindposeList.Add(skinnedMesh.sharedMesh.bindposes[i]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            Debug.Log($"Mapping bone parents");

            for (int i = 0; i < boneList.Count; i++)
            {
                Transform bone = boneList[i];
                if (bone.parent == null || string.IsNullOrEmpty(bone.parent.name) || bone.parent.name.StartsWith("_SM_"))
                {
                    Debug.Log($"Bone {bone.name} has no parent");
                    continue;
                }
                if (bonesMap.ContainsKey(bone.parent.name))
                {
                    int k = bonesMap[bone.parent.name];
                    if (boneParent[i] == -1)
                    {
                        Debug.Log($"Bone {bone.name} parented to {bone.parent.name}({k})");
                        boneParent[i] = k;
                    }
                    else if (boneParent[i] != k)
                    {
                        Debug.Log($"Warning: bone {bone.name} was parented to {boneList[boneParent[i]].name} but was also found parented to {bone.parent.name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Bone {bone.name} parented to {bone.parent.name} but bone parent index not found");
                }
            }

            Debug.Log($"Bone Count: {boneList.Count} Bindpose Count: {bindposeList.Count}");
        }

        private void CreateBoneList()
        {
            List<PmxBone> pmxBoneList = pmxFile.BoneList;
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
                pmxBone.To_Offset = ToPmxVec3(bone.rotation * UnityEngine.Vector3.left * scaleFactor / 16f);
                if (bone.name.EndsWith("_SCL_") || bone.name.Contains("twist"))
                {
                    pmxBone.SetFlag(PmxBone.BoneFlags.Visible, false);
                }
                pmxBoneList.Add(pmxBone);
            }

            for (int i = 0; i < pmxBoneList.Count; i++)
            {
                PmxBone pmxBone = pmxBoneList[i];
                int children = 0;
                int lastChildIndex = -1;
                for (int j = 0; j < pmxBoneList.Count; j++)
                {
                    PmxBone pmxOtherBone = pmxBoneList[j];
                    if (pmxOtherBone.Parent == i && pmxOtherBone.GetFlag(PmxBone.BoneFlags.Visible))
                    {
                        children++;
                        lastChildIndex = j;
                    }
                }
                if (children == 1)
                {
                    Debug.Log($"Pointing Bone {pmxBone.NameE} to {pmxBoneList[lastChildIndex].NameE}");
                    pmxBone.SetFlag(PmxBone.BoneFlags.ToBone, true);
                    pmxBone.To_Bone = lastChildIndex;
                }
            }
        }

        private void SetMaterialInfoProperty(out float value, Material material, string property)
        {
            if (material.HasProperty(property))
            {
                value = material.GetFloat(property);
            }
            else
            {
                value = 0;
            }
        }

        private void SetMaterialInfoProperty(out float[] value, Material material, string property)
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
                value = null;
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
            /*
            _Color [color]
            _MainTex
            _MultiColTex "Multi Color Table (RGBA)"
            _UseMulticolTex

            _ShadowColor [color]
            _ShadowTex
            _ShadowRateToon [tex]
            _RimColor
            _RimPower
            _RimShift
            _HiTex "Hilight (RGB)"
            _HiRate "Hilight rate"
            _HiPow

            _Ramp "Toon Ramp (RGB)"

            _OutlineColor [color]
            _OutlineTex
            _OutlineToonRamp

            _OutlineWidth [float]

            _EdgeLength "Edge length"
            _Phong "Phong Strengh"



             */
            /* Uncomment to dump list of material textures
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
            info.toonRamp = textureBuilder.Export(ExportFolder, material, "_ToonRamp");
            pmxMaterial.Toon = info.toonRamp ?? "";
            SetMaterialInfoProperty(out info.rimColor, material, "_RimColor");
            SetMaterialInfoProperty(out info.rimPower, material, "_RimPower");
            SetMaterialInfoProperty(out info.rimShift, material, "_RimShift");
            info.hiTex = textureBuilder.Export(ExportFolder, material, "_HiTex");
            SetMaterialInfoProperty(out info.hiRate, material, "_HiRate");
            SetMaterialInfoProperty(out info.hiPow, material, "_HiPow");
            SetMaterialInfoProperty(out info.outlineColor, material, "_OutlineColor");
            SetMaterialInfoProperty(out info.outlineWidth, material, "_OutlineWidth");

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
            pmxFile.MaterialList.Add(pmxMaterial);
        }

        private PmxMorph GetOrCreateMorph(string name)
        {
            if (morphMap.ContainsKey(name))
            {
                return morphMap[name];
            }
            PmxMorph pmxMorph = new PmxMorph();
            pmxMorph.Name = name;
            pmxMorph.NameE = name;
            pmxMorph.Panel = 1;
            pmxMorph.Kind = PmxMorph.OffsetKind.Vertex;
            pmxFile.MorphList.Add(pmxMorph);
            morphMap.Add(name, pmxMorph);
            return pmxMorph;
        }

        private void CreateMorphs(TBodySkin skin)
        {
            if (skin == null || skin.morph == null || skin.morph.BlendDatas.Count <= 0)
            {
                return;
            }
            if (!vertexIndexMap.ContainsKey(skin.obj.name))
            {
                Debug.Log($"Morph: {skin.obj.name} -> Missing vertex base!");
                return;
            }
            int vertexBase = vertexIndexMap[skin.obj.name];
            Debug.Log($"Morph: {skin.obj.name} -> {skin.morph.BlendDatas.Count} ({vertexBase})");
            for (int j = 0; j < skin.morph.BlendDatas.Count; j++)
            {
                BlendData blendData = skin.morph.BlendDatas[j];
                if (blendData != null)
                {
                    PmxMorph pmxMorph = GetOrCreateMorph(blendData.name);
                    for (int k = 0; k < blendData.v_index.Length; k++)
                    {
                        PmxVertexMorph pmxVertexMorph = new PmxVertexMorph(blendData.v_index[k] + vertexBase, new PmxLib.Vector3(-blendData.vert[k].x, blendData.vert[k].z, blendData.vert[k].y));
                        pmxVertexMorph.Offset *= scaleFactor;
                        pmxMorph.OffsetList.Add(pmxVertexMorph);
                    }
                }
            }
        }

        private void CreateMorphs()
        {
            Maid maid = GameMain.Instance.CharacterMgr.GetMaid(0);
            CreateMorphs(maid.body0.Face);
            for (int i = 0; i < maid.body0.goSlot.Count; i++)
            {
                TBodySkin skin = maid.body0.goSlot[i];
                if (skin != maid.body0.Face)
                {
                    CreateMorphs(skin);
                }
            }
        }

        private void Save()
        {
            PmxElementFormat pmxElementFormat = new PmxElementFormat(2.1f);
            pmxElementFormat.VertexSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.VertexList.Count);
            int val = int.MinValue;
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

            CreateMorphs();

            Save();
        }

        #endregion
    }
}