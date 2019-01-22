namespace FontStashSharp
{
	internal class FontTextIterator
	{
		public float x;
		public float y;
		public float nextx;
		public float nexty;
		public float scale;
		public float spacing;
		public uint codepoint;
		public short isize;
		public short iblur;
		public Font font;
		public int prevGlyphIndex;
		public StringSegment str, next;
		public int bitmapOption;
	}
}
