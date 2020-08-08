using System.Text.RegularExpressions;

namespace PmxLib
{
	internal static class PmxTag
	{
		public const string GROUPNAME = "gp";

		public static MatchCollection MatchsTag(string tag, string text, string groupName = "gp")
		{
			string text2 = "<" + tag + ">";
			string text3 = "</" + tag + ">";
			string pattern = text2 + "(?<" + groupName + ">.*?)" + text3;
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
			return regex.Matches(text);
		}

		public static string[] GetTag(string tag, string text)
		{
			MatchCollection matchCollection = PmxTag.MatchsTag(tag, text, "gp");
			if (matchCollection.Count <= 0)
			{
				return null;
			}
			string[] array = new string[matchCollection.Count];
			for (int i = 0; i < matchCollection.Count; i++)
			{
				Match match = matchCollection[i];
				string value = match.Groups["gp"].Value;
				array[i] = (string.IsNullOrEmpty(value) ? "" : value);
			}
			return array;
		}

		public static string SetTag(string tag, string text)
		{
			string text2 = tag.Trim();
			return "<" + text2 + ">" + text + "</" + text2 + ">";
		}

		public static bool ExistTag(string tag, string text)
		{
			string[] tag2 = PmxTag.GetTag(tag, text);
			return tag2 != null;
		}

		public static string RemoveTag(string tag, string text)
		{
			MatchCollection matchCollection = PmxTag.MatchsTag(tag, text, "gp");
			for (int i = 0; i < matchCollection.Count; i++)
			{
				text = text.Replace(matchCollection[i].Value, "");
			}
			return text;
		}
	}
}
