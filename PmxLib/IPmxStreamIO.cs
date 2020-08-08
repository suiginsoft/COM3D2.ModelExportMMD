using System.IO;

namespace PmxLib
{
	internal interface IPmxStreamIO
	{
		void FromStreamEx(Stream s, PmxElementFormat f = null);

		void ToStreamEx(Stream s, PmxElementFormat f = null);
	}
}
