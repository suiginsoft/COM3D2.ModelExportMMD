using COM3D2.ModelExportMMD.Gui;
using COM3D2.ModelExportMMD.Util;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace COM3D2.ModelExportMMD
{
    public class ObjBuilder
    {
        #region Types

        public enum SplitType
        {
            By_Mesh,
            By_Material,
            By_Submesh,
            None
        }

        #endregion

        #region Fields

        private string exportFolder;
        private string exportName;
        private List<string> matNameCache;

        #endregion

        #region Methods

        private string ConstructFaceString(int i1, int i2, int i3)
        {
            return i1 + "/" + i2 + "/" + i3;
        }

        private Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle * (point - pivot) + pivot;
        }

        private Vector3 MultiplyVec3s(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        private Texture2D GetShadowTex(Material material)
        {
            Texture2D result = null;
            if (material.HasProperty("_ShadowTex"))
            {
                Texture texture = material.GetTexture("_ShadowTex");
                result = ((texture.GetType() != typeof(RenderTexture)) ? (texture as Texture2D) : TextureWriter.Render2Texture2D(texture as RenderTexture));
            }
            return result;
        }

        private Texture GetMainTex(Material material)
        {
            Texture result = null;
            if (material.HasProperty("_MainTex"))
            {
                result = material.GetTexture("_MainTex");
            }
            return result;
        }

        private string GenerateMaterial(ref StringBuilder sbMaterial, Material material)
        {
            string text = material.name;
            Debug.Log("Generate Material : " + text);
            if (text.Contains("Instance"))
            {
                object obj = text;
                text = obj + "_(" + material.GetInstanceID() + ")";
            }
            if (!this.matNameCache.Contains(text))
            {
                this.matNameCache.Add(text);
                sbMaterial.AppendLine("newmtl " + text);
                if (material.HasProperty("_Color"))
                {
                    Color color = material.color;
                    sbMaterial.AppendLine("Kd " + color.r + " " + color.g + " " + color.b);
                    float num = Mathf.Lerp(1f, 0f, color.a);
                    sbMaterial.AppendLine("d " + num);
                }
                if (material.mainTexture != null)
                {
                    Texture mainTex = this.GetMainTex(material);
                    if (mainTex != null)
                    {
                        Vector2 textureScale = material.GetTextureScale("_MainTex");
                        if (mainTex.wrapMode == TextureWrapMode.Clamp)
                        {
                            sbMaterial.AppendLine("-clamp on");
                        }
                        sbMaterial.AppendLine("s " + textureScale.x + " " + textureScale.y);
                        sbMaterial.AppendLine("map_Kd " + text + "d.png");
                        if (ModelExportWindow.SaveTexture)
                        {
                            TextureWriter.WriteTexture2D(this.exportFolder + "/" + text + "d.png", mainTex);
                        }
                        Texture2D shadowTex = this.GetShadowTex(material);
                        if (shadowTex != null)
                        {
                            sbMaterial.AppendLine("map_Ka " + text + "a.png");
                            if (ModelExportWindow.SaveTexture)
                            {
                                TextureWriter.WriteTexture2D(this.exportFolder + "/" + text + "a.png", shadowTex);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No Found Texture " + text);
                    }
                }
                else
                {
                    Debug.Log("No Texture " + text);
                }
                sbMaterial.AppendLine();
            }
            return text;
        }

        private void PrepareFileHeader(StringBuilder sb)
        {
            sb.AppendLine("mtllib " + this.exportName + ".mtl");
        }

        public void Export(List<SkinnedMeshRenderer> meshesList, string path)
        {
            exportFolder = Path.GetDirectoryName(path);
            exportName = Path.GetFileNameWithoutExtension(path);
            this.matNameCache = new List<string>();
            SplitType splitType = SplitType.By_Mesh;
            if (!Directory.Exists(this.exportFolder))
            {
                Directory.CreateDirectory(this.exportFolder);
            }
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            this.PrepareFileHeader(stringBuilder);
            Debug.Log("SkinnedMeshRenderer number :" + meshesList.Count);
            int num = 1;
            if (splitType == SplitType.None)
            {
                stringBuilder.AppendLine("g default");
            }
            for (int i = 0; i < meshesList.Count; i++)
            {
                int num2 = 0;
                SkinnedMeshRenderer skinnedMeshRenderer = meshesList[i];
                Mesh mesh = null;
                GameObject gameObject = meshesList[i].gameObject;
                if (ModelExportWindow.SavePostion)
                {
                    Mesh mesh2 = new Mesh();
                    meshesList[i].BakeMesh(mesh2);
                    mesh = mesh2;
                }
                else
                {
                    mesh = meshesList[i].sharedMesh;
                }
                if (splitType == SplitType.By_Mesh)
                {
                    stringBuilder.AppendLine("g " + gameObject.name + "[" + gameObject.GetInstanceID() + "]");
                }
                Vector3[] vertices = mesh.vertices;
                foreach (Vector3 vector in vertices)
                {
                    Vector3 v = vector;
                    v = this.MultiplyVec3s(v, gameObject.transform.lossyScale);
                    v = this.RotateAroundPoint(v, Vector3.zero, gameObject.transform.rotation);
                    v += gameObject.transform.position;
                    stringBuilder.AppendLine("v " + v.x * -1f + " " + v.y + " " + v.z);
                    num2++;
                }
                vertices = mesh.normals;
                foreach (Vector3 vector in vertices)
                {
                    Vector3 v = vector;
                    v = this.RotateAroundPoint(vector, Vector3.zero, gameObject.transform.rotation);
                    stringBuilder.AppendLine("vn " + v.x * -1f + " " + v.y + " " + v.z);
                }
                Vector2[] uv = mesh.uv;
                for (int j = 0; j < uv.Length; j++)
                {
                    Vector2 vector2 = uv[j];
                    stringBuilder.AppendLine("vt " + vector2.x + " " + vector2.y);
                }
                bool flag = false;
                Renderer component = gameObject.GetComponent<Renderer>();
                if (component != null)
                {
                    flag = true;
                }
                for (int k = 0; k < mesh.subMeshCount; k++)
                {
                    if (splitType == SplitType.By_Submesh)
                    {
                        stringBuilder.AppendLine("g " + gameObject.name + "[" + gameObject.GetInstanceID() + ",SM" + k + "]");
                    }
                    if (flag && k <= component.sharedMaterials.Length - 1)
                    {
                        Material material = component.materials[k];
                        string str = this.GenerateMaterial(ref stringBuilder2, material);
                        if (splitType == SplitType.By_Material)
                        {
                            stringBuilder.AppendLine("g " + str);
                        }
                        stringBuilder.AppendLine("usemtl " + str);
                    }
                    int[] triangles = mesh.GetTriangles(k);
                    for (int l = 0; l < triangles.Length; l += 3)
                    {
                        string text = this.ConstructFaceString(triangles[l + 2] + num, triangles[l + 2] + num, triangles[l + 2] + num);
                        string text2 = this.ConstructFaceString(triangles[l + 1] + num, triangles[l + 1] + num, triangles[l + 1] + num);
                        string text3 = this.ConstructFaceString(triangles[l] + num, triangles[l] + num, triangles[l] + num);
                        stringBuilder.AppendLine("f " + text + " " + text2 + " " + text3);
                    }
                }
                num += num2;
            }
            File.WriteAllText(this.exportFolder + "\\" + this.exportName + ".obj", stringBuilder.ToString());
            File.WriteAllText(this.exportFolder + "\\" + this.exportName + ".mtl", stringBuilder2.ToString());
        }

        #endregion
    }
}