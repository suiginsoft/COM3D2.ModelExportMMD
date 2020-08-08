namespace PmxLib
{
	internal class uID
	{
		private uint m_next;

		public uint Next()
		{
			m_next++;
			if (m_next >= uint.MaxValue)
			{
				return 0u;
			}
			return m_next;
		}
	}
}
