using System.Collections.Generic;

namespace PmxLib
{
	internal class IDObject<T>
	{
		private Dictionary<uint, T> m_table;

		private readonly uint m_limit;

		private uint m_lastID;

		public int Count => m_table.Keys.Count;

		public T this[uint i] => Get(i);

		public IEnumerable<uint> IDs
		{
			get
			{
				foreach (uint key in m_table.Keys)
				{
					if (key != 0)
					{
						yield return key;
					}
				}
			}
		}

		public bool IsIDOverflow
		{
			get;
			private set;
		}

		public IDObject(uint limit = uint.MaxValue)
		{
			m_limit = limit;
			Clear();
		}

		public void Clear()
		{
			if (m_table != null)
			{
				m_table.Clear();
				m_table = null;
			}
			m_table = new Dictionary<uint, T>();
			m_table.Add(0u, default(T));
			m_lastID = 0u;
			IsIDOverflow = false;
		}

		public uint NewObject(T obj)
		{
			uint num = 0u;
			if (IsIDOverflow)
			{
				num = SearchNextID(m_lastID + 1);
				if (num == 0)
				{
					num = SearchNextID(1u);
					if (num == 0)
					{
						m_lastID = 0u;
						throw new IDOverflowException();
					}
				}
			}
			else
			{
				num = ++m_lastID;
				if (num >= m_limit)
				{
					IsIDOverflow = true;
					m_lastID = 0u;
					return NewObject(obj);
				}
			}
			if (num != 0)
			{
				m_table.Add(num, obj);
				m_lastID = num;
			}
			return num;
		}

		private uint SearchNextID(uint st)
		{
			for (uint num = st; num < m_limit; num++)
			{
				if (!m_table.ContainsKey(num))
				{
					return num;
				}
			}
			return 0u;
		}

		public bool ContainsID(uint id)
		{
			if (id == 0)
			{
				return false;
			}
			if (m_table.ContainsKey(id))
			{
				return true;
			}
			return false;
		}

		public T Get(uint id)
		{
			if (id == 0)
			{
				return default(T);
			}
			if (m_table.ContainsKey(id))
			{
				return m_table[id];
			}
			return default(T);
		}

		public void Remove(uint id)
		{
			if (id != 0 && m_table.ContainsKey(id))
			{
				m_table.Remove(id);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (uint iD in IDs)
			{
				yield return m_table[iD];
			}
		}
	}
}
