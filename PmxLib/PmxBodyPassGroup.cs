using System;
using System.Text;

namespace PmxLib
{
	internal class PmxBodyPassGroup : ICloneable
	{
		private const int PassGroupCount = 16;

		public bool[] Flags
		{
			get;
			private set;
		}

		public PmxBodyPassGroup()
		{
			Flags = new bool[16];
		}

		public PmxBodyPassGroup(PmxBodyPassGroup pg)
			: this()
		{
			for (int i = 0; i < 16; i++)
			{
				Flags[i] = pg.Flags[i];
			}
		}

		public ushort ToFlagBits()
		{
			int num = 1;
			int num2 = 0;
			for (int i = 0; i < Flags.Length; i++)
			{
				if (!Flags[i])
				{
					num2 |= num << i;
				}
			}
			return (ushort)num2;
		}

		public void FromFlagBits(ushort bits)
		{
			ushort num = 1;
			for (int i = 0; i < Flags.Length; i++)
			{
				Flags[i] = ((bits & (num << i)) <= 0);
			}
		}

		public void FromFlagBits(bool[] flags)
		{
			int num = Math.Min(Flags.Length, flags.Length);
			for (int i = 0; i < num; i++)
			{
				Flags[i] = flags[i];
			}
		}

		public string ToText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = Flags.Length;
			for (int i = 0; i < num; i++)
			{
				if (Flags[i])
				{
					stringBuilder.Append(i + 1 + " ");
				}
			}
			return stringBuilder.ToString();
		}

		public void FromText(string text)
		{
			try
			{
				string[] array = text.Split(new char[1]
				{
					' '
				}, StringSplitOptions.RemoveEmptyEntries);
				int num = Flags.Length;
				for (int i = 0; i < num; i++)
				{
					Flags[i] = false;
				}
				int num2 = array.Length;
				for (int j = 0; j < num2; j++)
				{
					if (int.TryParse(array[j], out int result))
					{
						result--;
						if (0 <= result && result < num)
						{
							Flags[result] = true;
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		object ICloneable.Clone()
		{
			return new PmxBodyPassGroup(this);
		}

		public PmxBodyPassGroup Clone()
		{
			return new PmxBodyPassGroup(this);
		}
	}
}
