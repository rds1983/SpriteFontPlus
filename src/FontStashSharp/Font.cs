using StbTrueTypeSharp;
using System;
using static StbTrueTypeSharp.StbTrueType;

namespace FontStashSharp
{
	internal class Font
	{
		private float AscentBase, DescentBase, LineHeightBase;
		
		public float Ascent { get; private set; }
		public float Descent { get; private set; }
		public float LineHeight { get; private set; }
		public float Scale { get; private set; }

		public stbtt_fontinfo _font = new stbtt_fontinfo();

		public void Recalculate(float size)
		{
			Ascent = AscentBase * size;
			Descent = DescentBase * size;
			LineHeight = LineHeightBase * size;
			Scale = stbtt_ScaleForPixelHeight(_font, size);
		}

		public int GetGlyphIndex(int codepoint)
		{
			return stbtt_FindGlyphIndex(_font, codepoint);
		}

		public void BuildGlyphBitmap(int glyph, float size, float scale, ref int advance, ref int lsb, ref int x0, ref int y0, ref int x1, ref int y1)
		{
			stbtt_GetGlyphHMetrics(_font, glyph, ref advance, ref lsb);
			stbtt_GetGlyphBitmapBox(_font, glyph, scale, scale, ref x0, ref y0, ref x1, ref y1);
		}

		public void RenderGlyphBitmap(FakePtr<byte> output, int outWidth, int outHeight, int outStride, int glyph)
		{
			stbtt_MakeGlyphBitmap(_font, output, outWidth, outHeight, outStride, Scale, Scale, glyph);
		}

		public static Font FromMemory(byte[] data)
		{
			var font = new Font();

			if (stbtt_InitFont(font._font, data, 0) == 0)
					throw new Exception("stbtt_InitFont failed");

			int ascent, descent, lineGap;
			stbtt_GetFontVMetrics(font._font , out ascent, out descent, out lineGap);

			var fh = ascent - descent;
			font.AscentBase = ascent / (float)fh;
			font.DescentBase = descent / (float)fh;
			font.LineHeightBase = (fh + lineGap) / (float)fh;

			return font;
		}
	}
}
