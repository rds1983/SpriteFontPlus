using System.Runtime.InteropServices;

namespace FontStashSharp
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct FontGlyph
	{
		public int Codepoint;
		public int Index;
		public int Next;
		public short Size;
		public short Blur;
		public short X0;
		public short Y0;
		public short X1;
		public short Y1;
		public short XAdvance;
		public short XOffset;
		public short YOffset;
	}
}
