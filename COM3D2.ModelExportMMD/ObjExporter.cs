using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace COM3D2.ModelExportMMD
{
    public class ObjExporter : IExporter
    {
        #region Types

        public enum Split
        {
            None,
            ByMesh,
            ByMaterial,
            BySubmesh
        }

        #endregion

        #region Properties

        public ModelFormat ExportFormat { get { return ModelFormat.Obj; } }
        public string ExportFolder { get; set; }
        public string ExportName { get; set; }
        public bool SavePosition { get; set; } = true;
        public bool SaveTexture { get; set; } = true;
        public Split SplitMethod { get; set; } = Split.ByMesh;

        #endregion

        #region Methods

        private string ConstructFace(int index1, int index2, int index3, int vertexOffset)
        {
            return (index1 + vertexOffset) + "/" + (index2 + vertexOffset) + "/" + (index3 + vertexOffset);
        }

        private Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle * (point - pivot) + pivot;
        }

        private Texture2D GetShadowTex(Material material)
        {
            if (material.HasProperty("_ShadowTex"))
            {
                Texture texture = material.GetTexture("_ShadowTex");
                return ((texture.GetType() != typeof(RenderTexture)) ? (texture as Texture2D) : TextureBuilder.ConvertToTexture2D(texture as RenderTexture));
            }
            return null;
        }

        private Texture GetMainTex(Material material)
        {
            if (material.HasProperty("_MainTex"))
            {
                return material.GetTexture("_MainTex");
            }
            return null;
        }

        private string GenerateMaterial(StringBuilder matOutput, List<string> matNameCache, Material material)
        {
            Debug.Log($"Generating material: {material.name}");

            string matRef = material.name;
            if (matRef.Contains("Instance"))
            {
                matRef += "_(" + material.GetInstanceID() + ")";
            }

            if (!matNameCache.Contains(matRef))
            {
                matNameCache.Add(matRef);
                matOutput.AppendLine("newmtl " + matRef);

                if (material.HasProperty("_Color"))
                {
                    Color color = material.color;
                    matOutput.AppendLine("Kd " + color.r + " " + color.g + " " + color.b);
                    float num = Mathf.Lerp(1f, 0f, color.a);
                    matOutput.AppendLine("d " + num);
                }

                if (material.mainTexture != null)
                {
                    Texture mainTex = GetMainTex(material);
                    if (mainTex != null)
                    {
                        if (mainTex.wrapMode == TextureWrapMode.Clamp)
                        {
                            matOutput.AppendLine("-clamp on");
                        }

                        Vector2 textureScale = material.GetTextureScale("_MainTex");
                        matOutput.AppendLine("s " + textureScale.x + " " + textureScale.y);
                        matOutput.AppendLine("map_Kd " + matRef + "d.png");
                        if (SaveTexture)
                        {
                            TextureBuilder.WriteTextureToFile(Path.Combine(ExportFolder, matRef + "d.png"), mainTex);
                        }

                        Texture2D shadowTex = GetShadowTex(material);
                        if (shadowTex != null)
                        {
                            matOutput.AppendLine("map_Ka " + matRef + "a.png");
                            if (SaveTexture)
                            {
                                TextureBuilder.WriteTextureToFile(Path.Combine(ExportFolder, matRef + "a.png"), shadowTex);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No texture found for " + matRef);
                    }
                }
                else
                {
                    Debug.Log("No Texture for " + matRef);
                }

                matOutput.AppendLine();
            }

            return matRef;
        }

        public void Export(List<SkinnedMeshRenderer> meshesList)
        {
            Directory.CreateDirectory(ExportFolder);

            StringBuilder objOutput = new StringBuilder();
            objOutput.AppendLine("mtllib " + ExportName + ".mtl");

            StringBuilder matOutput = new StringBuilder();
            var matNameCache = new List<string>();

            Debug.Log("SkinnedMeshRenderer number :" + meshesList.Count);

            if (SplitMethod == Split.None)
            {
                objOutput.AppendLine("g default");
            }

            int meshVertexCount = 1;

            foreach (var skinnedMesh in meshesList)
            {
                var gameObject = skinnedMesh.gameObject;
                var renderer = gameObject.GetComponent<Renderer>();

                Mesh mesh = null;
                if (SavePosition)
                {
                    mesh = new Mesh();
                    skinnedMesh.BakeMesh(mesh);
                }
                else
                {
                    mesh = skinnedMesh.sharedMesh;
                }

                if (SplitMethod == Split.ByMesh)
                {
                    objOutput.AppendLine("g " + gameObject.name + "[" + gameObject.GetInstanceID() + "]");
                }

                foreach (Vector3 vector in mesh.vertices)
                {
                    Vector3 v = Vector3.Scale(vector, gameObject.transform.lossyScale);
                    v = RotateAroundPoint(v, Vector3.zero, gameObject.transform.rotation);
                    v += gameObject.transform.position;
                    objOutput.AppendLine("v " + v.x * -1f + " " + v.y + " " + v.z);
                }

                foreach (Vector3 vector in mesh.normals)
                {
                    Vector3 v = RotateAroundPoint(vector, Vector3.zero, gameObject.transform.rotation);
                    objOutput.AppendLine("vn " + v.x * -1f + " " + v.y + " " + v.z);
                }

                for (int j = 0; j < mesh.uv.Length; j++)
                {
                    Vector2 vector2 = mesh.uv[j];
                    objOutput.AppendLine("vt " + vector2.x + " " + vector2.y);
                }

                for (int k = 0; k < mesh.subMeshCount; k++)
                {
                    if (SplitMethod == Split.BySubmesh)
                    {
                        objOutput.AppendLine("g " + gameObject.name + "[" + gameObject.GetInstanceID() + ",SM" + k + "]");
                    }

                    if (renderer != null && k < renderer.materials.Length)
                    {
                        string matRef = GenerateMaterial(matOutput, matNameCache, renderer.materials[k]);
                        if (SplitMethod == Split.ByMaterial)
                        {
                            objOutput.AppendLine("g " + matRef);
                        }
                        objOutput.AppendLine("usemtl " + matRef);
                    }

                    int[] triangles = mesh.GetTriangles(k);
                    for (int l = 0; l < triangles.Length; l += 3)
                    {
                        string face1 = ConstructFace(triangles[l + 2], triangles[l + 2], triangles[l + 2], meshVertexCount);
                        string face2 = ConstructFace(triangles[l + 1], triangles[l + 1], triangles[l + 1], meshVertexCount);
                        string face3 = ConstructFace(triangles[l], triangles[l], triangles[l], meshVertexCount);
                        objOutput.AppendLine("f " + face1 + " " + face2 + " " + face3);
                    }
                }

                meshVertexCount += mesh.vertices.Length;
            }

            File.WriteAllText(Path.Combine(ExportFolder, ExportName + ".obj"), objOutput.ToString());
            File.WriteAllText(Path.Combine(ExportFolder, ExportName + ".mtl"), matOutput.ToString());
        }

        #endregion
    }
}