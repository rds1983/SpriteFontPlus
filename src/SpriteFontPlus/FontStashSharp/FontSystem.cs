using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace FontStashSharp
{
	internal unsafe class FontSystem
	{
		private readonly List<Font> _fonts = new List<Font>();
		private float _ith;
		private float _itw;
		private readonly List<RenderItem> _renderItems = new List<RenderItem>();
		private FontAtlas _currentAtlas;
		private Point _size;

		public int FontId;
		public float Size;
		public Color Color;
		public float BlurValue;
		public float Spacing;
		public Vector2 Scale;
		public bool UseKernings = true;
		public bool TryFallback = false;

		public FontAtlas CurrentAtlas
		{
			get
			{
				if (_currentAtlas == null)
				{
					_currentAtlas = new FontAtlas(_size.X, _size.Y, 256, Atlases.Count);
					Atlases.Add(_currentAtlas);
				}

				return _currentAtlas;
			}
		}

		private Font CurrentFont
		{
			get
			{
				return _fonts[FontId];
			}
		}

		public List<FontAtlas> Atlases { get; } = new List<FontAtlas>();

		public event EventHandler CurrentAtlasFull;

		public FontSystem(int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width));
			}

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height));
			}

			_size = new Point(width, height);

			_itw = 1.0f / _size.X;
			_ith = 1.0f / _size.Y;
			ClearState();
		}

		public void ClearState()
		{
			Size = 12.0f;
			Color = Color.White;
			FontId = 0;
			BlurValue = 0;
			Spacing = 0;
		}

		public int AddFontMem(string name, byte[] data)
		{
			var font = Font.FromMemory(name, data);

			_fonts.Add(font);
			return _fonts.Count - 1;
		}

		public int? GetFontByName(string name)
		{
			var i = 0;
			for (i = 0; i < _fonts.Count; i++)
				if (_fonts[i].Name == name)
					return i;
			return null;
		}

		public float DrawText(SpriteBatch batch, float x, float y, StringSegment str, float depth)
		{
			if (str.IsNullOrEmpty) return 0.0f;

			FontGlyph glyph = null;
			var q = new FontGlyphSquad();
			var prevGlyphIndex = -1;
			var isize = (int)(Size * 10.0f);
			var iblur = (int)BlurValue;
			float scale = 0;
			Font font;
			if (FontId < 0 || FontId >= _fonts.Count)
				return x;
			font = _fonts[FontId];
			if (font.Data == null)
				return x;
			scale = font._font.fons__tt_getPixelHeightScale(isize / 10.0f);

			float originX = 0.0f;
			float originY = 0.0f;

			var ascent = CurrentFont.GetAscent(Size);
			var lineHeight = CurrentFont.GetLineHeight(Size);
			originY += ascent;
			for (int i = 0; i < str.Length; i += char.IsSurrogatePair(str.String, i + str.Location) ? 2 : 1)
			{
				var codepoint = char.ConvertToUtf32(str.String, i + str.Location);

				if (codepoint == '\n')
				{
					originX = 0.0f;
					originY += lineHeight;
					prevGlyphIndex = -1;
					continue;
				}

				glyph = GetGlyph(font, codepoint, isize, iblur, true);
				if (glyph == null && TryFallback)
				{
					for (int j = 0; j < _fonts.Count; j++)
					{
						if (FontId == j) continue;

						Font f = _fonts[j];

						glyph = GetGlyph(f, codepoint, isize, iblur, true);

						if (glyph != null) break;
					}
				}
				if (glyph != null)
				{
					GetQuad(font, prevGlyphIndex, glyph, scale, Spacing, ref originX, ref originY, &q);

					q.X0 = (int)(q.X0 * Scale.X);
					q.X1 = (int)(q.X1 * Scale.X);
					q.Y0 = (int)(q.Y0 * Scale.Y);
					q.Y1 = (int)(q.Y1 * Scale.Y);

					var renderItem = new RenderItem
					{
						Atlas = Atlases[glyph.AtlasIndex],
						_verts = new Rectangle((int)(x + q.X0),
								(int)(y + q.Y0),
								(int)(q.X1 - q.X0),
								(int)(q.Y1 - q.Y0)),
						_textureCoords = new Rectangle((int)(q.S0 * _size.X),
								(int)(q.T0 * _size.Y),
								(int)((q.S1 - q.S0) * _size.X),
								(int)((q.T1 - q.T0) * _size.Y)),
						_colors = Color
					};

					_renderItems.Add(renderItem);
				}

				prevGlyphIndex = glyph != null ? glyph.Index : -1;
			}

			Flush(batch, depth);
			return x;
		}

		public float TextBounds(float x, float y, StringSegment str, ref Bounds bounds)
		{
			var q = new FontGlyphSquad();
			FontGlyph glyph = null;
			var prevGlyphIndex = -1;
			var isize = (int)(Size * 10.0f);
			var iblur = (int)BlurValue;
			float scale = 0;
			Font font;
			float startx = 0;
			float advance = 0;
			float minx = 0;
			float miny = 0;
			float maxx = 0;
			float maxy = 0;
			if (FontId < 0 || FontId >= _fonts.Count)
				return 0;
			font = _fonts[FontId];
			if (font.Data == null)
				return 0;
			scale = font._font.fons__tt_getPixelHeightScale(isize / 10.0f);

			var ascent = CurrentFont.GetAscent(Size);
			var lineHeight = CurrentFont.GetLineHeight(Size);

			y += ascent;
			minx = maxx = x;
			miny = maxy = y;
			startx = x;
			for (int i = 0; i < str.Length; i += char.IsSurrogatePair(str.String, i + str.Location) ? 2 : 1)
			{
				var codepoint = char.ConvertToUtf32(str.String, i + str.Location);

				if (codepoint == '\n')
				{
					x = startx;
					y += lineHeight;
					prevGlyphIndex = -1;
					continue;
				}

				glyph = GetGlyph(font, codepoint, isize, iblur, false);
				if (glyph == null && TryFallback)
				{
					for (int j = 0; j < _fonts.Count; j++)
					{
						if (FontId == j) continue;

						Font f = _fonts[j];

						glyph = GetGlyph(f, codepoint, isize, iblur, false);

						if (glyph != null) break;
					}
				}
				if (glyph != null)
				{
					GetQuad(font, prevGlyphIndex, glyph, scale, Spacing, ref x, ref y, &q);
					if (q.X0 < minx)
						minx = q.X0;
					if (x > maxx)
						maxx = x;
					if (q.Y0 < miny)
						miny = q.Y0;
					if (q.Y1 > maxy)
						maxy = q.Y1;
				}

				prevGlyphIndex = glyph != null ? glyph.Index : -1;
			}

			advance = x - startx;

			bounds.X = minx;
			bounds.Y = miny;
			bounds.X2 = maxx;
			bounds.Y2 = maxy;

			return advance;
		}

		public void Reset(int width, int height)
		{
			Atlases.Clear();

			for (var i = 0; i < _fonts.Count; i++)
			{
				var font = _fonts[i];
				font.Glyphs.Clear();
			}

			if (width == _size.X && height == _size.Y)
				return;

			_size = new Point(width, height);
			_itw = 1.0f / _size.X;
			_ith = 1.0f / _size.Y;
		}

		public void Reset()
		{
			Reset(_size.X, _size.Y);
		}

		private FontGlyph GetGlyph(Font font, int codepoint, int isize, int iblur, bool isBitmapRequired)
		{
			var g = 0;
			var advance = 0;
			var lsb = 0;
			var x0 = 0;
			var y0 = 0;
			var x1 = 0;
			var y1 = 0;
			var gw = 0;
			var gh = 0;
			var gx = 0;
			var gy = 0;
			float scale = 0;
			FontGlyph glyph = null;
			var size = isize / 10.0f;
			if (isize < 2)
				return null;
			if (iblur > 20)
				iblur = 20;

			if (font.TryGetGlyph(codepoint, isize, iblur, out glyph))
			{
				if (!isBitmapRequired || glyph.X0 >= 0 && glyph.Y0 >= 0)
					return glyph;

			}
			g = font._font.fons__tt_getGlyphIndex(codepoint);
			if (g == 0)
			{
				return null;
			}

			scale = font._font.fons__tt_getPixelHeightScale(size);
			font._font.fons__tt_buildGlyphBitmap(g, size, scale, &advance, &lsb, &x0, &y0, &x1, &y1);

			var pad = FontGlyph.PadFromBlur(iblur);
			gw = x1 - x0 + pad * 2;
			gh = y1 - y0 + pad * 2;

			var currentAtlas = CurrentAtlas;
			if (isBitmapRequired)
			{
				if (!currentAtlas.AddRect(gw, gh, ref gx, ref gy))
				{
					var ev = CurrentAtlasFull;
					if (ev != null)
					{
						ev(this, EventArgs.Empty);
					}

					// This code will force creation of new atlas
					_currentAtlas = null;
					currentAtlas = CurrentAtlas;

					// Try to add again
					if (!currentAtlas.AddRect(gw, gh, ref gx, ref gy))
					{
						throw new Exception(string.Format("Could not add rect to the newly created atlas. gw={0}, gh={1}", gw, gh));
					}
				}
			}
			else
			{
				gx = -1;
				gy = -1;
			}

			if (glyph == null)
			{
				glyph = new FontGlyph
				{
					Codepoint = codepoint,
					Size = isize,
					Blur = iblur
				};

				font.SetGlyph(codepoint, isize, iblur, glyph);
			}

			glyph.Index = g;
			glyph.AtlasIndex = currentAtlas.Index;
			glyph.X0 = gx;
			glyph.Y0 = gy;
			glyph.X1 = glyph.X0 + gw;
			glyph.Y1 = glyph.Y0 + gh;
			glyph.XAdvance = (int)(scale * advance * 10.0f);
			glyph.XOffset = x0 - pad;
			glyph.YOffset = y0 - pad;
			if (!isBitmapRequired) return glyph;

			currentAtlas.RenderGlyph(font, glyph, gw, gh, scale);

			return glyph;
		}

		private void GetQuad(Font font, int prevGlyphIndex, FontGlyph glyph, float scale,
			float spacing, ref float x, ref float y, FontGlyphSquad* q)
		{
			if (prevGlyphIndex != -1)
			{
				float adv = 0;
				if (UseKernings)
				{
					adv = font.GetKerning(prevGlyphIndex, glyph.Index) * scale;
				}
				x += (int)(adv + spacing + 0.5f);
			}

			float rx = 0;
			float ry = 0;

			rx = x + glyph.XOffset;
			ry = y + glyph.YOffset;
			q->X0 = rx;
			q->Y0 = ry;
			q->X1 = rx + (glyph.X1 - glyph.X0);
			q->Y1 = ry + (glyph.Y1 - glyph.Y0);
			q->S0 = glyph.X0 * _itw;
			q->T0 = glyph.Y0 * _ith;
			q->S1 = glyph.X1 * _itw;
			q->T1 = glyph.Y1 * _ith;

			x += (int)(glyph.XAdvance / 10.0f + 0.5f);
		}

		private void Flush(SpriteBatch batch, float depth)
		{
			foreach (var atlas in Atlases)
			{
				atlas.Flush(batch.GraphicsDevice);
			}

			for (var i = 0; i < _renderItems.Count; ++i)
			{
				var renderItem = _renderItems[i];
				batch.Draw(renderItem.Atlas.Texture,
					renderItem._verts,
					renderItem._textureCoords,
					renderItem._colors,
					0f,
					Vector2.Zero,
					SpriteEffects.None,
					depth);
			}

			_renderItems.Clear();
		}
	}
}