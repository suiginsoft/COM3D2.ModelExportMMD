using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace COM3D2.ModelExportMMD
{
    public class TextureBuilder
    {

        private HashSet<string> exportedFileNames = new HashSet<string>();

        #region Methods

        public static int nextPowerOfTwo(int x)
        {
            if (x <= 0) return 0;
            if ((x & (x - 1)) == 0) return x;
            if (x >= 0x40000000) return x;
            int result = 1;
            while (result < x) result *= 2;
            return result;
        }

        public static Texture2D makePowerOfTwo(Texture2D tex)
        {
            return tex;
            /*
            // doesn't work right now
            int w = tex.width;
            int h = tex.height;
            int w2 = nextPowerOfTwo(w);
            int h2 = nextPowerOfTwo(h);
            if (w == w2 && h == h2)
            {
                return tex;
            }
            Texture2D copy = new Texture2D(w, h, tex.format, false);
            copy.SetPixels32(tex.GetPixels32());
            copy.Resize(w2, h2);
            return copy;
            */
        }

        public static Texture2D ConvertToTexture2D(RenderTexture renderTexture)
        {
            var priorRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;

            try
            {
                Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
                texture2D.Apply();
                texture2D.ReadPixels(new Rect(0f, 0f, texture2D.width, texture2D.height), 0, 0);
                return texture2D;
            }
            finally
            {
                RenderTexture.active = priorRenderTexture;
            }
        }

        public static void WriteTextureToFile(string path, Texture tex, bool keepAlpha)
        {
            try
            {
                Texture2D texture2D = null;
                if (tex is RenderTexture) {
                    RenderTexture renderTexture = tex as RenderTexture;
                    texture2D = ConvertToTexture2D(renderTexture);
                } else {
                    texture2D = tex as Texture2D;
                }
                if (!keepAlpha)
                {
                    // In some cases textures are rendered with all transparent pixels
                    // That doesn't affect the game's shader because it ignores alpha, but it's troublesome for using the texture in other applications
                    // Remove the alpha if the shader isn't a transparent shader
                    Color[] pixels = texture2D.GetPixels();
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        pixels[i].a = 1;
                    }
                    texture2D.SetPixels(pixels);
                }
                byte[] bytes = texture2D.EncodeToPNG();
                File.WriteAllBytes(path, bytes);
                Debug.Log($"Texture written to file: {path}");
            }
            catch (Exception error)
            {
                Debug.Log($"Error writing texture to file: {error.Message}\n\nStack trace:\n{error.StackTrace}");
            }
        }

        public static void WriteTextureToFile(string path, Texture tex)
        {
            WriteTextureToFile(path, tex, true);
        }

        /// <summary>
        /// Export texture to file in folder if not already exported and return filename without folder path
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="materialName"></param>
        /// <param name="tex"></param>
        /// <returns>File name without folder</returns>
        public string Export(string folderPath, Material material, string propertyName, Texture tex)
        {
            string fileName;
            if (string.IsNullOrEmpty(tex.name) || tex.name.Contains(":") /* for rt: textures */)
            {
                fileName = material.name.Replace("Instance", material.GetInstanceID().ToString()) + propertyName + ".png";
            }
            else
            {
                fileName = tex.name + ".png";
            }
            if (exportedFileNames.Add(fileName))
            {
                WriteTextureToFile(Path.Combine(folderPath, fileName), tex, material.shader.renderQueue >= 2450);
            }
            return fileName;
        }

        public string Export(string folderPath, Material material, string propertyName)
        {
            Texture tex = material.GetTexture(propertyName);
            if (tex == null)
            {
                // return empty string NOT null!
                // PmxMaterial breaks if you set null.
                return "";
            }
            else
            {
                return Export(folderPath, material, propertyName, tex);
            }
        }
        #endregion
    }
}
