using System.Runtime.InteropServices;

namespace FontStashSharp
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct GlyphPosition
	{
		public StringSegment str;
		public float x;
		public float minx;
		public float maxx;
	}
}
