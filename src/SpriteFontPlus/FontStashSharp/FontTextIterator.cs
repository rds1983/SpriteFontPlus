namespace FontStashSharp
{
	internal class FontTextIterator
	{
		public float X;
		public float Y;
		public float NextX;
		public float NextY;
		public float Scale;
		public float Spacing;
		public int Codepoint;
		public short iSize;
		public short iBlur;
		public Font Font;
		public int PrevGlyphIndex;
		public StringSegment Str, Next;
		public int BitmapOption;
	}
}
