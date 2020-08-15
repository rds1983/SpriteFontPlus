using Microsoft.Xna.Framework;

namespace FontStashSharp
{
	class FontGlyph
	{
		private readonly Int32Map<int> _kernings = new Int32Map<int>();

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

		public int GetKerning(FontGlyph nextGlyph)
		{
			int result;
			if (_kernings.TryGetValue(nextGlyph.Index, out result))
			{
				return result;
			}
			result = StbTrueTypeSharp.StbTrueType.stbtt_GetGlyphKernAdvance(Font._font, Index, nextGlyph.Index);
			_kernings[nextGlyph.Index] = result;

			return result;
		}
	}
}
