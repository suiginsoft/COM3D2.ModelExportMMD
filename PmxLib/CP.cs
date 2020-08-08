using System;
using System.Collections.Generic;
using System.Linq;

namespace PmxLib
{
	internal static class CP
	{
		public static void Swap<T>(ref T v0, ref T v1) where T : struct
		{
			T val = v0;
			v0 = v1;
			v1 = val;
		}

		public static void Swap<T>(IList<T> list, int ix1, int ix2)
		{
			T value = list[ix1];
			list[ix1] = list[ix2];
			list[ix2] = value;
		}

		public static void _SafeAdd<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue val)
		{
			if (!dic.ContainsKey(key))
			{
				dic.Add(key, val);
			}
		}

		public static bool _SafeAddR<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue val)
		{
			bool result = false;
			if (!dic.ContainsKey(key))
			{
				dic.Add(key, val);
				result = true;
			}
			return result;
		}

		public static List<T> CloneList<T>(List<T> list) where T : ICloneable
		{
			List<T> list2 = new List<T>();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				list2.Add((T)list[i].Clone());
			}
			return list2;
		}

		public static List<T> CloneList_ValueType<T>(List<T> list) where T : struct
		{
			return new List<T>(list.ToArray());
		}

		public static T[] CloneArray<T>(T[] src) where T : ICloneable
		{
			T[] array = new T[src.Length];
			for (int i = 0; i < src.Length; i++)
			{
				array[i] = (T)src[i].Clone();
			}
			return array;
		}

		public static T[] CloneArray_ValueType<T>(T[] src) where T : struct
		{
			T[] array = new T[src.Length];
			Array.Copy(src, array, src.Length);
			return array;
		}

		public static Dictionary<Tv, int> ArrayToTable<Tl, Tv>(Tl[] arr, Func<int, Tv> objProc = null)
		{
			Dictionary<Tv, int> dictionary = new Dictionary<Tv, int>(arr.Length);
			for (int i = 0; i < arr.Length; i++)
			{
				Tv val = objProc(i);
				if (val != null && !dictionary.ContainsKey(val))
				{
					dictionary.Add(val, i);
				}
			}
			return dictionary;
		}

		public static Dictionary<T, int> ArrayToTable<T>(T[] arr)
		{
			Dictionary<T, int> dictionary = new Dictionary<T, int>(arr.Length);
			for (int i = 0; i < arr.Length; i++)
			{
				T val = arr[i];
				if (val != null && !dictionary.ContainsKey(val))
				{
					dictionary.Add(val, i);
				}
			}
			return dictionary;
		}

		public static Dictionary<Tv, int> ListToTable<Tl, Tv>(List<Tl> list, Func<int, Tv> objProc)
		{
			Dictionary<Tv, int> dictionary = new Dictionary<Tv, int>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Tv val = objProc(i);
				if (val != null && !dictionary.ContainsKey(val))
				{
					dictionary.Add(val, i);
				}
			}
			return dictionary;
		}

		public static Dictionary<T, int> ListToTable<T>(List<T> list)
		{
			Dictionary<T, int> dictionary = new Dictionary<T, int>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				T val = list[i];
				if (val != null && !dictionary.ContainsKey(val))
				{
					dictionary.Add(val, i);
				}
			}
			return dictionary;
		}

		public static bool InRange<T>(T[] arr, int index)
		{
			if (0 <= index)
			{
				return index < arr.Length;
			}
			return false;
		}

		public static bool InRange<T>(List<T> list, int index)
		{
			if (0 <= index)
			{
				return index < list.Count;
			}
			return false;
		}

		public static bool InRange<T>(IList<T> list, int index)
		{
			if (0 <= index)
			{
				return index < list.Count;
			}
			return false;
		}

		public static bool InRange(int min, int max, int val)
		{
			if (min <= val)
			{
				return val <= max;
			}
			return false;
		}

		public static T SafeGet<T>(T[] arr, int index) where T : class
		{
			if (arr != null && InRange(arr, index))
			{
				return arr[index];
			}
			return null;
		}

		public static T SafeGet<T>(IList<T> arr, int index) where T : class
		{
			if (arr != null && InRange(arr, index))
			{
				return arr[index];
			}
			return null;
		}

		public static T SafeGetV<T>(T[] arr, int index) where T : struct
		{
			if (arr != null && InRange(arr, index))
			{
				return arr[index];
			}
			return default(T);
		}

		public static T SafeGetV<T>(T[] arr, int index, out bool flag) where T : struct
		{
			flag = false;
			if (arr != null && InRange(arr, index))
			{
				flag = true;
				return arr[index];
			}
			return default(T);
		}

		public static T SafeGetV<T>(IList<T> arr, int index) where T : struct
		{
			if (arr != null && InRange(arr, index))
			{
				return arr[index];
			}
			return default(T);
		}

		public static T SafeGetV<T>(IList<T> arr, int index, out bool flag) where T : struct
		{
			flag = false;
			if (arr != null && InRange(arr, index))
			{
				flag = true;
				return arr[index];
			}
			return default(T);
		}

		public static int[] SortIndexForRemove(int[] ix)
		{
			List<int> list = new List<int>(ix);
			list.Sort((int l, int r) => r - l);
			return list.ToArray();
		}

		public static void SSort<T>(List<T> list, Comparison<T> comp)
		{
			List<KeyValuePair<int, T>> list2 = new List<KeyValuePair<int, T>>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				list2.Add(new KeyValuePair<int, T>(i, list[i]));
			}
			list2.Sort(delegate(KeyValuePair<int, T> x, KeyValuePair<int, T> y)
			{
				int num = comp(x.Value, y.Value);
				if (num == 0)
				{
					num = x.Key.CompareTo(y.Key);
				}
				return num;
			});
			for (int j = 0; j < list.Count; j++)
			{
				list[j] = list2[j].Value;
			}
		}

		public static int[] ComposeIndices(int[] ix1, int[] ix2)
		{
			List<int> list = new List<int>(ix1);
			Dictionary<int, int> dictionary = new Dictionary<int, int>(ix1.Length);
			for (int i = 0; i < ix1.Length; i++)
			{
				dictionary.Add(ix1[i], 0);
			}
			for (int j = 0; j < ix2.Length; j++)
			{
				if (!dictionary.ContainsKey(ix2[j]))
				{
					dictionary.Add(ix2[j], 0);
					list.Add(ix2[j]);
				}
			}
			return list.ToArray();
		}

		public static int[] RemoveIndices(int[] ix1, int[] ix2)
		{
			List<int> list = new List<int>(ix1.Length);
			Dictionary<int, int> dictionary = new Dictionary<int, int>(ix2.Length);
			for (int i = 0; i < ix2.Length; i++)
			{
				dictionary.Add(ix2[i], 0);
			}
			for (int j = 0; j < ix1.Length; j++)
			{
				if (!dictionary.ContainsKey(ix1[j]))
				{
					dictionary.Add(ix1[j], 0);
					list.Add(ix1[j]);
				}
			}
			return list.ToArray();
		}

		public static bool IsSameIndices(int[] arr1, int[] arr2)
		{
			if (arr1.Length != arr2.Length)
			{
				return false;
			}
			for (int i = 0; i < arr1.Length; i++)
			{
				if (arr1[i] != arr2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static IEnumerable<T> RotateArray<T>(T[] arr, int st, bool right = true)
		{
			if (right)
			{
				for (int l = st; l < arr.Length; l++)
				{
					yield return arr[l];
				}
				for (int k = 0; k < st; k++)
				{
					yield return arr[k];
				}
				yield break;
			}
			for (int j = st; j >= 0; j--)
			{
				yield return arr[j];
			}
			for (int i = arr.Length - 1; i > st; i--)
			{
				yield return arr[i];
			}
		}

		public static IEnumerable<int> RotateArrayIndex<T>(T[] arr, int st, bool right = true)
		{
			if (right)
			{
				for (int l = st; l < arr.Length; l++)
				{
					yield return l;
				}
				for (int k = 0; k < st; k++)
				{
					yield return k;
				}
				yield break;
			}
			for (int j = st; j >= 0; j--)
			{
				yield return j;
			}
			for (int i = arr.Length - 1; i > st; i--)
			{
				yield return i;
			}
		}

		public static IEnumerable<T> RotateList<T>(IList<T> list, int st, bool right = true)
		{
			if (right)
			{
				for (int l = st; l < list.Count; l++)
				{
					yield return list[l];
				}
				for (int k = 0; k < st; k++)
				{
					yield return list[k];
				}
				yield break;
			}
			for (int j = st; j >= 0; j--)
			{
				yield return list[j];
			}
			for (int i = list.Count - 1; i > st; i--)
			{
				yield return list[i];
			}
		}

		public static IEnumerable<int> RotateListIndex<T>(IList<T> list, int st, bool right = true)
		{
			if (right)
			{
				for (int l = st; l < list.Count; l++)
				{
					yield return l;
				}
				for (int k = 0; k < st; k++)
				{
					yield return k;
				}
				yield break;
			}
			for (int j = st; j >= 0; j--)
			{
				yield return j;
			}
			for (int i = list.Count - 1; i > st; i--)
			{
				yield return i;
			}
		}

		public static T[] NormalizeArray<T>(T[] arr)
		{
			T[] array = arr.Distinct().ToArray();
			Array.Sort(array);
			return array;
		}

		public static T[] NormalizeArray<T>(IEnumerable<T> arr)
		{
			T[] array = arr.Distinct().ToArray();
			Array.Sort(array);
			return array;
		}
	}
}
