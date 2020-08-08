using System;
using System.Text;

namespace PmxLib
{
	public class PmxBodyPassGroup : ICloneable
	{
		private const int PassGroupCount = 16;

		public bool[] Flags
		{
			get;
			private set;
		}

		public PmxBodyPassGroup()
		{
			this.Flags = new bool[16];
		}

		public PmxBodyPassGroup(PmxBodyPassGroup pg)
			: this()
		{
			for (int i = 0; i < 16; i++)
			{
				this.Flags[i] = pg.Flags[i];
			}
		}

		public ushort ToFlagBits()
		{
			int num = 1;
			int num2 = 0;
			for (int i = 0; i < this.Flags.Length; i++)
			{
				if (!this.Flags[i])
				{
					num2 |= num << i;
				}
			}
			return (ushort)num2;
		}

		public void FromFlagBits(ushort bits)
		{
			ushort num = 1;
			for (int i = 0; i < this.Flags.Length; i++)
			{
				this.Flags[i] = ((bits & num << i) <= 0);
			}
		}

		public void FromFlagBits(bool[] flags)
		{
			int num = Math.Min(this.Flags.Length, flags.Length);
			for (int i = 0; i < num; i++)
			{
				this.Flags[i] = flags[i];
			}
		}

		public string ToText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = this.Flags.Length;
			for (int i = 0; i < num; i++)
			{
				if (this.Flags[i])
				{
					stringBuilder.Append((i + 1).ToString() + " ");
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
				int num = this.Flags.Length;
				for (int i = 0; i < num; i++)
				{
					this.Flags[i] = false;
				}
				int num2 = array.Length;
				for (int j = 0; j < num2; j++)
				{
					int num3 = default(int);
					if (int.TryParse(array[j], out num3))
					{
						num3--;
						if (0 <= num3 && num3 < num)
						{
							this.Flags[num3] = true;
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
