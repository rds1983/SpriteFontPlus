using System.Runtime.InteropServices;

namespace FontStashSharp
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct FontAtlasNode
	{
		public short x;
		public short y;
		public short width;
	}
}
