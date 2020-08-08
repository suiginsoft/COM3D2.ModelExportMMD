using System.Collections.Generic;

namespace PmxLib
{
	internal class IDObject<T>
	{
		private Dictionary<uint, T> m_table;

		private readonly uint m_limit;

		private uint m_lastID;

		public int Count
		{
			get
			{
				return this.m_table.Keys.Count;
			}
		}

		public T this[uint i]
		{
			get
			{
				return this.Get(i);
			}
		}

		public IEnumerable<uint> IDs
		{
			get
			{
				foreach (uint key in this.m_table.Keys)
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

		public IDObject(uint limit = 4294967295u)
		{
			this.m_limit = limit;
			this.Clear();
		}

		public void Clear()
		{
			if (this.m_table != null)
			{
				this.m_table.Clear();
				this.m_table = null;
			}
			this.m_table = new Dictionary<uint, T>();
			this.m_table.Add(0u, default(T));
			this.m_lastID = 0u;
			this.IsIDOverflow = false;
		}

		public uint NewObject(T obj)
		{
			uint num;
			if (this.IsIDOverflow)
			{
				num = this.SearchNextID(this.m_lastID + 1);
				if (num == 0)
				{
					num = this.SearchNextID(1u);
					if (num == 0)
					{
						this.m_lastID = 0u;
						throw new IDOverflowException();
					}
				}
			}
			else
			{
				num = (this.m_lastID += 1u);
				if (num >= this.m_limit)
				{
					this.IsIDOverflow = true;
					this.m_lastID = 0u;
					return this.NewObject(obj);
				}
			}
			if (num != 0)
			{
				this.m_table.Add(num, obj);
				this.m_lastID = num;
			}
			return num;
		}

		private uint SearchNextID(uint st)
		{
			for (uint num = st; num < this.m_limit; num++)
			{
				if (!this.m_table.ContainsKey(num))
				{
					return num;
				}
			}
			return 0u;
		}

		public bool ContainsID(uint id)
		{
			return id != 0 && this.m_table.ContainsKey(id);
		}

		public T Get(uint id)
		{
			if (id == 0)
			{
				return default(T);
			}
			if (this.m_table.ContainsKey(id))
			{
				return this.m_table[id];
			}
			return default(T);
		}

		public void Remove(uint id)
		{
			if (id != 0 && this.m_table.ContainsKey(id))
			{
				this.m_table.Remove(id);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (uint iD in this.IDs)
			{
				yield return this.m_table[iD];
			}
		}
	}
}
