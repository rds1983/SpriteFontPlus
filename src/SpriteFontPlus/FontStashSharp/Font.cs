using System;
using System.Collections.Generic;
using static StbTrueTypeSharp.StbTrueType;

namespace FontStashSharp
{
	internal class Font
	{
		private float AscentBase, DescentBase, LineHeightBase;

		public stbtt_fontinfo _font = new stbtt_fontinfo();
		public string Name;
		public byte[] Data;
		private readonly Dictionary<ulong, int> _kernings = new Dictionary<ulong, int>();

		public Dictionary<ulong, FontGlyph> Glyphs { get; } = new Dictionary<ulong, FontGlyph>();

		private ulong BuildKey(int codePoint, int size, int blur)
		{
			ulong result = (uint)codePoint << 32;
			result |= (ulong)size << 16;
			result |= (ushort)blur;

			return result;
		}

		public bool TryGetGlyph(int codePoint, int size, int blur, out FontGlyph glyph)
		{
			var key = BuildKey(codePoint, size, blur);

			return Glyphs.TryGetValue(key, out glyph);
		}

		public void SetGlyph(int codePoint, int size, int blur, FontGlyph glyph)
		{
			var key = BuildKey(codePoint, size, blur);
			Glyphs[key] = glyph;
		}

		public int GetKerning(int glyphIndex1, int glyphIndex2)
		{
			ulong key = (uint)glyphIndex1 << 32;
			key |= (uint)glyphIndex2;

			int result;
			if (_kernings.TryGetValue(key, out result))
			{
				return result;
			}
			result = stbtt_GetGlyphKernAdvance(_font, glyphIndex1, glyphIndex2);
			_kernings[key] = result;

			return result;
		}

		public float GetAscent(float size)
		{
			return AscentBase * size;
		}

		public float GetDescent(float size)
		{
			return DescentBase * size;
		}

		public float GetLineHeight(float size)
		{
			return LineHeightBase * size;
		}

		public static unsafe Font FromMemory(string name, byte[] data)
		{
			var font = new Font
			{
				Name = name,
				Data = data
			};
			fixed (byte* ptr = data)
			{
				if (stbtt_InitFont(font._font, ptr, 0) == 0)
					throw new Exception(string.Format("stbtt_InitFont failed. name={0}", name));
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
