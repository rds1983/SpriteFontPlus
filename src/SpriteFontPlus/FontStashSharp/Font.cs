using System.Collections.Generic;
using static StbTrueTypeSharp.StbTrueType;

namespace FontStashSharp
{
	internal class Font
	{
		private readonly Dictionary<ulong, FontGlyph> _glyphs = new Dictionary<ulong, FontGlyph>();

		public stbtt_fontinfo _font = new stbtt_fontinfo();
		public string Name;
		public byte[] Data;
		public float Ascent;
		public float Ascender;
		public float Descender;
		public float LineHeight;
		private readonly Dictionary<ulong, int> _kernings = new Dictionary<ulong, int>();

		public Dictionary<ulong, FontGlyph> Glyphs
		{
			get
			{
				return _glyphs;
			}
		}

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

			return _glyphs.TryGetValue(key, out glyph);
		}

		public void SetGlyph(int codePoint, int size, int blur, FontGlyph glyph)
		{
			var key = BuildKey(codePoint, size, blur);
			_glyphs[key] = glyph;
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
	}
}
