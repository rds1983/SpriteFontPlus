using System;
using System.Runtime.InteropServices;
using static StbTrueTypeSharp.StbTrueType;

namespace FontStashSharp
{
	internal unsafe class Font: IDisposable
	{
		private GCHandle? dataPtr = null;
		private float AscentBase, DescentBase, LineHeightBase;
		readonly Int32Map<int> _kernings = new Int32Map<int>();

		public float Ascent { get; private set; }
		public float Descent { get; private set; }
		public float LineHeight { get; private set; }
		public float Scale { get; private set; }

		public stbtt_fontinfo _font = new stbtt_fontinfo();

		public Font(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			dataPtr = GCHandle.Alloc(data, GCHandleType.Pinned);
		}

		~Font()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (dataPtr != null)
			{
				dataPtr.Value.Free();
				dataPtr = null;
			}
		}

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

		public void BuildGlyphBitmap(int glyph, float scale, ref int advance, ref int lsb, ref int x0, ref int y0, ref int x1, ref int y1)
		{
			int advanceTemp, lsbTemp;
			stbtt_GetGlyphHMetrics(_font, glyph, &advanceTemp, &lsbTemp);
			advance = advanceTemp;
			lsb = lsbTemp;

			int x0Temp, y0Temp, x1Temp, y1Temp;
			stbtt_GetGlyphBitmapBox(_font, glyph, scale, scale, &x0Temp, &y0Temp, &x1Temp, &y1Temp);
			x0 = x0Temp;
			y0 = y0Temp;
			x1 = x1Temp;
			y1 = y1Temp;
		}

		public void RenderGlyphBitmap(byte *output, int outWidth, int outHeight, int outStride, int glyph)
		{
			stbtt_MakeGlyphBitmap(_font, output, outWidth, outHeight, outStride, Scale, Scale, glyph);
		}

		public int GetGlyphKernAdvance(int glyph1, int glyph2)
		{
			var key = ((glyph1 << 16) | (glyph1 >> 16)) ^ glyph2;
			int result;
			if (_kernings.TryGetValue(key, out result))
			{
				return result;
			}
			result = stbtt_GetGlyphKernAdvance(_font, glyph1, glyph2);
			_kernings[key] = result;
			return result;
		}

		public static Font FromMemory(byte[] data)
		{
			var font = new Font(data);

			byte* dataPtr = (byte *)font.dataPtr.Value.AddrOfPinnedObject();
			if (stbtt_InitFont(font._font, dataPtr, 0) == 0)
				throw new Exception("stbtt_InitFont failed");

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
