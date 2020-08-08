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
			return new Regex(text2 + "(?<" + groupName + ">.*?)" + text3, RegexOptions.IgnoreCase).Matches(text);
		}

		public static string[] GetTag(string tag, string text)
		{
			MatchCollection matchCollection = MatchsTag(tag, text);
			if (matchCollection.Count <= 0)
			{
				return null;
			}
			string[] array = new string[matchCollection.Count];
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string value = matchCollection[i].Groups["gp"].Value;
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
			return GetTag(tag, text) != null;
		}

		public static string RemoveTag(string tag, string text)
		{
			MatchCollection matchCollection = MatchsTag(tag, text);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				text = text.Replace(matchCollection[i].Value, "");
			}
			return text;
		}
	}
}
