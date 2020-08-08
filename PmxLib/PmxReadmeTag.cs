using System;
using System.IO;

namespace PmxLib
{
	internal static class PmxReadmeTag
	{
		public const string TAG_README = "readme";

		public static string[] GetReadme(Pmx pmx, string root = "")
		{
			string[] tag = PmxTag.GetTag("readme", pmx.ModelInfo.CommentE);
			if (tag != null && !string.IsNullOrEmpty(root))
			{
				for (int i = 0; i < tag.Length; i++)
				{
					if (!Path.IsPathRooted(tag[i]))
					{
						tag[i] = root + "\\" + tag[i];
					}
				}
			}
			return tag;
		}

		public static void SetReadme(Pmx pmx, string path)
		{
			pmx.ModelInfo.CommentE += Environment.NewLine;
			PmxTag.SetTag("readme", pmx.ModelInfo.CommentE);
		}

		public static bool ExistReadme(Pmx pmx)
		{
			return PmxTag.ExistTag("readme", pmx.ModelInfo.CommentE);
		}

		public static void RemoveReadme(Pmx pmx)
		{
			pmx.ModelInfo.CommentE = PmxTag.RemoveTag("readme", pmx.ModelInfo.CommentE);
			pmx.ModelInfo.CommentE = pmx.ModelInfo.CommentE.Trim();
		}
	}
}
