using StbTrueTypeSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpriteFontPlus
{
	public static class TtfFontBaker
	{
		public static TtfFontBakerResult Bake(Stream ttfStream, float fontPixelHeight,
			int bitmapWidth, int bitmapHeight,
			IEnumerable<CharacterRange> characterRanges)
		{
			return Bake(ttfStream.ToByteArray(), fontPixelHeight, bitmapWidth, bitmapHeight, characterRanges);
		}

		public unsafe static TtfFontBakerResult Bake(byte[] ttf, float fontPixelHeight,
			int bitmapWidth, int bitmapHeight,
			IEnumerable<CharacterRange> characterRanges)
		{
			if (ttf == null || ttf.Length == 0)
			{
				throw new ArgumentNullException(nameof(ttf));
			}

			if (fontPixelHeight <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(fontPixelHeight));
			}

			if (bitmapWidth <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(bitmapWidth));
			}

			if (bitmapHeight <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(bitmapHeight));
			}

			if (characterRanges == null)
			{
				throw new ArgumentNullException(nameof(characterRanges));
			}

			if (!characterRanges.Any())
			{
				throw new ArgumentException("characterRanges must have a least one value.");
			}

			byte[] pixels;
			var glyphs = new Dictionary<int, GlyphInfo>();
			StbTrueType.stbtt_fontinfo fontInfo = new StbTrueType.stbtt_fontinfo();
			fixed (byte* ttfPtr = ttf)
			{
				if (StbTrueType.stbtt_InitFont(fontInfo, ttfPtr, 0) == 0)
				{
					throw new Exception("Failed to init font.");
				}
			}

			float scaleFactor = StbTrueType.stbtt_ScaleForPixelHeight(fontInfo, fontPixelHeight);

			int ascent, descent, lineGap;
			StbTrueType.stbtt_GetFontVMetrics(fontInfo, &ascent, &descent, &lineGap);

			pixels = new byte[bitmapWidth * bitmapHeight];
			StbTrueType.stbtt_pack_context pc = new StbTrueType.stbtt_pack_context();

			fixed (byte* ttfPtr = ttf)
			fixed (byte* pixelsPtr = pixels)
			{
				StbTrueType.stbtt_PackBegin(pc, pixelsPtr, bitmapWidth,
					bitmapHeight, bitmapWidth, 1, null);

				foreach (var range in characterRanges)
				{
					if (range.Start > range.End)
					{
						continue;
					}

					var cd = new StbTrueType.stbtt_packedchar[range.End - range.Start + 1];
					for (var i = 0; i < cd.Length; ++i)
					{
						cd[i] = new StbTrueType.stbtt_packedchar();
					}

					fixed (StbTrueType.stbtt_packedchar* cdPtr = cd)
					{
						StbTrueType.stbtt_PackFontRange(pc, ttfPtr, 0, fontPixelHeight,
							range.Start,
							range.End - range.Start + 1,
							cdPtr);
					}

					for (var i = 0; i < cd.Length; ++i)
					{
						var yOff = cd[i].yoff;
						yOff += ascent * scaleFactor;

						var glyphInfo = new GlyphInfo
						{
							X = cd[i].x0,
							Y = cd[i].y0,
							Width = cd[i].x1 - cd[i].x0,
							Height = cd[i].y1 - cd[i].y0,
							XOffset = (int)cd[i].xoff,
							YOffset = (int)Math.Round(yOff),
							XAdvance = (int)Math.Round(cd[i].xadvance)
						};

						glyphs[(char)(i + range.Start)] = glyphInfo;
					}
				}
			}


			return new TtfFontBakerResult(glyphs, fontPixelHeight, pixels, bitmapWidth, bitmapHeight);
		}
	}
}