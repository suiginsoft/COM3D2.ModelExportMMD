using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PmxLib
{
	internal class Vpd
	{
		internal class PoseData
		{
			public Quaternion Rotation;

			public Vector3 Translation;

			public string BoneName
			{
				get;
				set;
			}

			public PoseData()
			{
			}

			public PoseData(string name, Quaternion r, Vector3 t)
			{
				BoneName = name;
				Rotation = r;
				Translation = t;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("{" + BoneName);
				string format = "0.000000";
				stringBuilder.AppendLine("  " + Translation.X.ToString(format) + "," + Translation.Y.ToString(format) + "," + Translation.Z.ToString(format) + ";");
				stringBuilder.AppendLine("  " + Rotation.X.ToString(format) + "," + Rotation.Y.ToString(format) + "," + Rotation.Z.ToString(format) + "," + Rotation.W.ToString(format) + ";");
				stringBuilder.AppendLine("}");
				return stringBuilder.ToString();
			}
		}

		internal class MorphData
		{
			public float Value;

			public string MorphName
			{
				get;
				set;
			}

			public MorphData()
			{
			}

			public MorphData(string name, float val)
			{
				MorphName = name;
				Value = val;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("{" + MorphName);
				stringBuilder.AppendLine("  " + Value + ";");
				stringBuilder.AppendLine("}");
				return stringBuilder.ToString();
			}
		}

		public static string VpdHeader = "Vocaloid Pose Data file";

		private static string HeadGetReg = "^[^V]*Vocaloid Pose Data file";

		private static string InfoGetReg = "^[^V]*Vocaloid Pose Data file[^\\n]*\\n[\\n\\s]*(?<name>[^;]+);[\\s]*[^\\n]*\\n+(?<num>[^;]+);";

		private static string BoneGetReg = "\\n+\\s*Bone(?<no>\\d+)\\s*\\{\\s*(?<name>[^\\r\\n]+)[\\r\\n]+\\s*(?<trans_x>[^,]+),(?<trans_y>[^,]+),(?<trans_z>[^;]+);[^\\n]*\\n+(?<rot_x>[^,]+),(?<rot_y>[^,]+),(?<rot_z>[^,]+),(?<rot_w>[^;]+);[^\\n]*\\n+\\s*\\}";

		private static string MorphGetReg = "\\n+\\s*Morph(?<no>\\d+)\\s*\\{\\s*(?<name>[^\\r\\n]+)[\\r\\n]+\\s*(?<val>[^;]+);[^\\n]*\\n+\\s*\\}";

		private static string NameExt = ".osm";

		public string ModelName
		{
			get;
			set;
		}

		public List<PoseData> PoseList
		{
			get;
			private set;
		}

		public List<MorphData> MorphList
		{
			get;
			private set;
		}

		public bool Extend
		{
			get;
			set;
		}

		public Vpd()
		{
			PoseList = new List<PoseData>();
			MorphList = new List<MorphData>();
			Extend = true;
		}

		public static bool IsVpdText(string text)
		{
			return new Regex(HeadGetReg, RegexOptions.IgnoreCase).IsMatch(text);
		}

		public bool FromText(string text)
		{
			bool result = false;
			try
			{
				if (!IsVpdText(text))
				{
					return result;
				}
				Match match = new Regex(InfoGetReg, RegexOptions.IgnoreCase).Match(text);
				string text2 = "";
				if (match.Success)
				{
					text2 = match.Groups["name"].Value;
					if (text2.ToLower().Contains(NameExt))
					{
						text2 = text2.Replace(NameExt, "");
					}
					ModelName = text2;
				}
				PoseList.Clear();
				match = new Regex(BoneGetReg, RegexOptions.IgnoreCase).Match(text);
				while (match.Success)
				{
					string text3 = "";
					Vector3 t = new Vector3(0f, 0f, 0f);
					Quaternion identity = Quaternion.Identity;
					text3 = match.Groups["name"].Value;
					float.TryParse(match.Groups["trans_x"].Value, out t.x);
					float.TryParse(match.Groups["trans_y"].Value, out t.y);
					float.TryParse(match.Groups["trans_z"].Value, out t.z);
					float.TryParse(match.Groups["rot_x"].Value, out identity.x);
					float.TryParse(match.Groups["rot_y"].Value, out identity.y);
					float.TryParse(match.Groups["rot_z"].Value, out identity.z);
					float.TryParse(match.Groups["rot_w"].Value, out identity.w);
					PoseList.Add(new PoseData(text3, identity, t));
					match = match.NextMatch();
				}
				MorphList.Clear();
				match = new Regex(MorphGetReg, RegexOptions.IgnoreCase).Match(text);
				while (match.Success)
				{
					string text4 = "";
					float result2 = 0f;
					text4 = match.Groups["name"].Value;
					float.TryParse(match.Groups["val"].Value, out result2);
					MorphList.Add(new MorphData(text4, result2));
					match = match.NextMatch();
				}
				result = true;
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(VpdHeader);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(ModelName + NameExt + ";");
			stringBuilder.AppendLine(PoseList.Count + ";");
			stringBuilder.AppendLine();
			for (int i = 0; i < PoseList.Count; i++)
			{
				stringBuilder.AppendLine("Bone" + i + PoseList[i].ToString());
			}
			if (Extend)
			{
				for (int j = 0; j < MorphList.Count; j++)
				{
					stringBuilder.AppendLine("Morph" + j + MorphList[j].ToString());
				}
			}
			return stringBuilder.ToString();
		}
	}
}
