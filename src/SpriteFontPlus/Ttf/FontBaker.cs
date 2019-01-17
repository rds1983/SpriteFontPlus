using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using StbSharp;

namespace SpriteFontPlus.Ttf
{
	public unsafe class FontBaker
	{
		private StbTrueType.stbtt_pack_context pc;
		private GCHandle _handle;

		private readonly Dictionary<char, GlyphInfo> result =
			new Dictionary<char, GlyphInfo>();

		private byte[] _pixels;

		public byte[] Pixels
		{
			get { return _pixels; }
		}

		private FontBaker(int pw, int ph)
		{
			if (pw <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(pw));
			}

			if (ph <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(ph));
			}
			
			_pixels = new byte[pw * ph];
			_handle = GCHandle.Alloc(_pixels, GCHandleType.Pinned);
			fixed (StbTrueType.stbtt_pack_context* pcPtr = &pc)
			{
				StbTrueType.stbtt_PackBegin(pcPtr, (byte*) _handle.AddrOfPinnedObject().ToPointer(), pw, ph, pw, 1, null);
			}
		}

		private void Add(byte[] ttf, float pixel_height, IEnumerable<CharacterRange> ranges)
		{
			fixed (StbTrueType.stbtt_pack_context* pcPtr = &pc)
			{
				fixed (byte* ttfPtr = ttf)
				{
					foreach (var range in ranges)
					{
						if (range.Start > range.End)
						{
							continue;
						}

						var cd = new GlyphInfo[range.End - range.Start + 1];
						fixed (GlyphInfo* chardataPtr = cd)
						{
							StbTrueType.stbtt_PackFontRange(pcPtr, ttfPtr, 0, pixel_height, range.Start,
								range.End - range.Start + 1, chardataPtr);
						}

						for (var i = 0; i < cd.Length; ++i)
						{
							result[(char) (i + range.Start)] = cd[i];
						}
					}
				}
			}
		}

		private Dictionary<char, GlyphInfo> End()
		{
			fixed (StbTrueType.stbtt_pack_context* pcPtr = &pc)
			{
				StbTrueType.stbtt_PackEnd(pcPtr);
			}
			
			_handle.Free();

			return result;
		}

		public static FontBakerResult Bake(int bitmapWidth, int bitmapHeight, IEnumerable<FontBakerEntry> entries)
		{
			if (entries == null)
			{
				throw new ArgumentNullException(nameof(entries));
			}

			if (!entries.Any())
			{
				throw new ArgumentException("entries must contain at least one entry.");
			}
			
			var baker = new FontBaker(bitmapWidth, bitmapHeight);

			foreach (var entry in entries)
			{
				baker.Add(entry.Ttf, entry.PixelHeight, entry.CharacterRanges);
			}

			var glyphs = baker.End();

			return new FontBakerResult(glyphs, baker._pixels, bitmapWidth, bitmapHeight);
		}
	}
}