using System;
using System.IO;
using UnityEngine;

namespace COM3D2.ModelExportMMD.Util
{
	public class TextureWriter
	{
		public static Texture2D Render2Texture2D(RenderTexture renderTexture)
		{
			int width = renderTexture.width;
			int height = renderTexture.height;
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
			texture2D.Apply();
			return texture2D;
		}

		public static void WriteTexture2D(string path, Texture tex)
		{
			try
			{
				Texture2D texture2D = null;
				texture2D = ((!(tex is RenderTexture)) ? (tex as Texture2D) : TextureWriter.Render2Texture2D(tex as RenderTexture));
				Texture2D texture2D2 = new Texture2D(texture2D.width, texture2D.height, TextureFormat.ARGB32, false);
				Color[] pixels = texture2D.GetPixels();
				texture2D2.SetPixels(pixels);
				byte[] bytes = texture2D2.EncodeToPNG();
				File.WriteAllBytes(path, bytes);
				Debug.Log("Writing Texture :" + path);
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}
	}
}
