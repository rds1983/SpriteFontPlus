using System.Runtime.InteropServices;

namespace FontStashSharp
{
	[StructLayout(LayoutKind.Sequential)]
	struct FontAtlasNode
	{
		public int X;
		public int Y;
		public int Width;
	}
}
