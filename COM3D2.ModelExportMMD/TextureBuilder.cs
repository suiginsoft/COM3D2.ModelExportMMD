using System;
using System.IO;
using UnityEngine;

namespace COM3D2.ModelExportMMD
{
    public static class TextureBuilder
    {
        #region Methods

        public static Texture2D ConvertToTexture2D(RenderTexture renderTexture)
        {
            var priorRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;

            try
            {
                Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
                texture2D.Apply();
                texture2D.ReadPixels(new Rect(0f, 0f, texture2D.width, texture2D.height), 0, 0);
                // In some cases textures are rendered with all transparent pixels
                // That doesn't affect the game's shader because it ignores alpha, but it's troublesome for using the texture in other applications
                // Remove the alpha if over 90% of pixels are transparent
                Color[] pixels = texture2D.GetPixels();
                int numTransparent = 0;
                for (int i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i].a < 0.01)
                    {
                        numTransparent++;
                    }
                }
                if (numTransparent > (9 * pixels.Length) / 10)
                {
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        pixels[i].a = 1;
                    }
                    texture2D.SetPixels(pixels);
                }
                return texture2D;
            }
            finally
            {
                RenderTexture.active = priorRenderTexture;
            }
        }

        public static void WriteTextureToFile(string path, Texture tex)
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
                byte[] bytes = texture2D.EncodeToPNG();
                File.WriteAllBytes(path, bytes);
                Debug.Log($"Texture written to file: {path}");
            }
            catch (Exception error)
            {
                Debug.Log($"Error writing texture to file: {error.Message}\n\nStack trace:\n{error.StackTrace}");
            }
        }

        #endregion
    }
}
