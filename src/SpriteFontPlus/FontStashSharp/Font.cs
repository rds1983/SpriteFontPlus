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
		private byte[] _data;

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

		public unsafe void BuildGlyphBitmap(int glyph, float size, float scale, int* advance, int* lsb, int* x0, int* y0, int* x1, int* y1)
		{
			stbtt_GetGlyphHMetrics(_font, glyph, advance, lsb);
			stbtt_GetGlyphBitmapBox(_font, glyph, scale, scale, x0, y0, x1, y1);
		}

		public unsafe void RenderGlyphBitmap(byte* output, int outWidth, int outHeight, int outStride, int glyph)
		{
			stbtt_MakeGlyphBitmap(_font, output, outWidth, outHeight, outStride, Scale, Scale, glyph);
		}

		public static unsafe Font FromMemory(byte[] data)
		{
			var font = new Font
			{
				_data = data
			};

			fixed (byte* ptr = data)
			{
				if (stbtt_InitFont(font._font, ptr, 0) == 0)
					throw new Exception("stbtt_InitFont failed");
			}

			int ascent, descent, lineGap;
			stbtt_GetFontVMetrics(font._font , &ascent, &descent, &lineGap);

			var fh = ascent - descent;
			font.AscentBase = ascent / (float)fh;
			font.DescentBase = descent / (float)fh;
			font.LineHeightBase = (fh + lineGap) / (float)fh;

			return font;
		}
	}
}
