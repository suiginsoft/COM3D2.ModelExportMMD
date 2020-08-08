using System.Collections.Generic;

namespace PmxLib
{
	internal class PmxIDObject : IPmxID
	{
		protected static uID _id;

		public uint UID
		{
			get;
			protected set;
		}

		public uint CID
		{
			get;
			protected set;
		}

		static PmxIDObject()
		{
			_id = new uID();
		}

		public PmxIDObject()
		{
			UID = _id.Next();
			CID = UID;
		}

		public void FromID(PmxIDObject src)
		{
			CID = src.CID;
		}

		public void ForcedIDSet(uint uid, uint cid)
		{
			UID = uid;
			CID = cid;
		}

		public static void NormalizeCID<T>(List<T> list) where T : PmxIDObject
		{
			Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
			for (int i = 0; i < list.Count; i++)
			{
				uint cID = list[i].CID;
				if (dictionary.ContainsKey(cID))
				{
					list[i].ForcedIDSet(list[i].UID, list[i].UID);
				}
				else
				{
					dictionary.Add(cID, i);
				}
			}
		}
	}
}
