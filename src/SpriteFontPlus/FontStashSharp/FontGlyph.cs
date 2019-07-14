namespace FontStashSharp
{
	internal class FontGlyph
	{
		public int Codepoint;
		public int Index;
		public int Size;
		public int Blur;
		public int AtlasIndex;
		public int X0;
		public int Y0;
		public int X1;
		public int Y1;
		public int XAdvance;
		public int XOffset;
		public int YOffset;

		public int Pad
		{
			get
			{
				return PadFromBlur(Blur);
			}
		}

		public static int PadFromBlur(int blur)
		{
			return blur + 2;
		}
	}
}
