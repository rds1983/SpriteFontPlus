using Microsoft.Xna.Framework;

namespace FontStashSharp
{
	class FontGlyph
	{
		public Font Font;
		public FontAtlas Atlas;
		public int Codepoint;
		public int Index;
		public int Size;
		public Rectangle Bounds;
		public int XAdvance;
		public int XOffset;
		public int YOffset;

		public static int PadFromBlur(int blur)
		{
			return blur + 2;
		}

		public bool IsEmpty
		{
			get
			{
				return Bounds.Width == 0 || Bounds.Height == 0;
			}
		}
	}
}
