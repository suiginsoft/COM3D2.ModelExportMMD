using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PmxLib
{
	public class Vpd
	{
		public class PoseData
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
				this.BoneName = name;
				this.Rotation = r;
				this.Translation = t;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("{" + this.BoneName);
				string format = "0.000000";
				StringBuilder stringBuilder2 = stringBuilder;
				string[] array = new string[7]
				{
					"  ",
					null,
					null,
					null,
					null,
					null,
					null
				};
				string[] array2 = array;
				float num = this.Translation.X;
				array2[1] = num.ToString(format);
				array[2] = ",";
				string[] array3 = array;
				num = this.Translation.Y;
				array3[3] = num.ToString(format);
				array[4] = ",";
				string[] array4 = array;
				num = this.Translation.Z;
				array4[5] = num.ToString(format);
				array[6] = ";";
				stringBuilder2.AppendLine(string.Concat(array));
				StringBuilder stringBuilder3 = stringBuilder;
				array = new string[9]
				{
					"  ",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null
				};
				string[] array5 = array;
				num = this.Rotation.X;
				array5[1] = num.ToString(format);
				array[2] = ",";
				string[] array6 = array;
				num = this.Rotation.Y;
				array6[3] = num.ToString(format);
				array[4] = ",";
				string[] array7 = array;
				num = this.Rotation.Z;
				array7[5] = num.ToString(format);
				array[6] = ",";
				string[] array8 = array;
				num = this.Rotation.W;
				array8[7] = num.ToString(format);
				array[8] = ";";
				stringBuilder3.AppendLine(string.Concat(array));
				stringBuilder.AppendLine("}");
				return stringBuilder.ToString();
			}
		}

		public class MorphData
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
				this.MorphName = name;
				this.Value = val;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("{" + this.MorphName);
				stringBuilder.AppendLine("  " + this.Value.ToString() + ";");
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
			this.PoseList = new List<PoseData>();
			this.MorphList = new List<MorphData>();
			this.Extend = true;
		}

		public static bool IsVpdText(string text)
		{
			Regex regex = new Regex(Vpd.HeadGetReg, RegexOptions.IgnoreCase);
			return regex.IsMatch(text);
		}

		public bool FromText(string text)
		{
			bool result = false;
			try
			{
				if (!Vpd.IsVpdText(text))
				{
					return result;
				}
				Regex regex = new Regex(Vpd.InfoGetReg, RegexOptions.IgnoreCase);
				Match match = regex.Match(text);
				if (match.Success)
				{
					string text2 = match.Groups["name"].Value;
					if (text2.ToLower().Contains(Vpd.NameExt))
					{
						text2 = text2.Replace(Vpd.NameExt, "");
					}
					this.ModelName = text2;
				}
				this.PoseList.Clear();
				Regex regex2 = new Regex(Vpd.BoneGetReg, RegexOptions.IgnoreCase);
				match = regex2.Match(text);
				while (match.Success)
				{
					Vector3 t = new Vector3(0f, 0f, 0f);
					Quaternion identity = Quaternion.Identity;
					string value = match.Groups["name"].Value;
					float x = default(float);
					float.TryParse(match.Groups["trans_x"].Value, out x);
					float y = default(float);
					float.TryParse(match.Groups["trans_y"].Value, out y);
					float z = default(float);
					float.TryParse(match.Groups["trans_z"].Value, out z);
					t.x = x;
					t.y = y;
					t.z = z;
					float.TryParse(match.Groups["rot_x"].Value, out x);
					float.TryParse(match.Groups["rot_y"].Value, out y);
					float.TryParse(match.Groups["rot_z"].Value, out z);
					float w = default(float);
					float.TryParse(match.Groups["rot_w"].Value, out w);
					identity.x = x;
					identity.y = y;
					identity.z = z;
					identity.w = w;
					this.PoseList.Add(new PoseData(value, identity, t));
					match = match.NextMatch();
				}
				this.MorphList.Clear();
				regex2 = new Regex(Vpd.MorphGetReg, RegexOptions.IgnoreCase);
				match = regex2.Match(text);
				while (match.Success)
				{
					float val = 0f;
					string value2 = match.Groups["name"].Value;
					float.TryParse(match.Groups["val"].Value, out val);
					this.MorphList.Add(new MorphData(value2, val));
					match = match.NextMatch();
				}
				result = true;
			}
			catch (Exception)
			{
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Vpd.VpdHeader);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(this.ModelName + Vpd.NameExt + ";");
			stringBuilder.AppendLine(this.PoseList.Count.ToString() + ";");
			stringBuilder.AppendLine();
			for (int i = 0; i < this.PoseList.Count; i++)
			{
				stringBuilder.AppendLine("Bone" + i.ToString() + this.PoseList[i].ToString());
			}
			if (this.Extend)
			{
				for (int j = 0; j < this.MorphList.Count; j++)
				{
					stringBuilder.AppendLine("Morph" + j.ToString() + this.MorphList[j].ToString());
				}
			}
			return stringBuilder.ToString();
		}
	}
}
