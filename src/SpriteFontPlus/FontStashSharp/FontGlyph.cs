using System.Runtime.InteropServices;

namespace FontStashSharp
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct FontGlyph
	{
		public uint codepoint;
		public int index;
		public int next;
		public short size;
		public short blur;
		public short x0;
		public short y0;
		public short x1;
		public short y1;
		public short xadv;
		public short xoff;
		public short yoff;
	}
}
