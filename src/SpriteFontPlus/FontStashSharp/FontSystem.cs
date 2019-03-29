using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using StbTrueTypeSharp;
using static StbTrueTypeSharp.StbTrueType;

namespace FontStashSharp
{
	internal unsafe class FontSystem
	{
		public const int FONS_ATLAS_FULL = 1;

		private FontSystemParams _params_ = new FontSystemParams();
		private FontAtlas _atlas;
		private Color[] _colors = new Color[1024];
		private int[] _dirtyRect = new int[4];
		private readonly List<Font> _fonts = new List<Font>();
		private float _ith;
		private float _itw;
		private int _vertsNumber;
		private Rectangle[] _textureCoords = new Rectangle[1024 * 2];
		private byte[] _texData;
		private Color[] _colorData;
		private Rectangle[] _verts = new Rectangle[1024 * 2];

		public int FontId;
		public Alignment Alignment;
		public float Size;
		public Color Color;
		public float BlurValue;
		public float Spacing;
		public Vector2 Scale;

		public FontSystem(FontSystemParams p)
		{
			_params_ = p;

			_atlas = new FontAtlas(_params_.Width, _params_.Height, 256);
			_itw = 1.0f / _params_.Width;
			_ith = 1.0f / _params_.Height;
			_texData = new byte[_params_.Width * _params_.Height];
			_colorData = new Color[_params_.Width * _params_.Height];
			Array.Clear(_texData, 0, _texData.Length);
			_dirtyRect[0] = _params_.Width;
			_dirtyRect[1] = _params_.Height;
			_dirtyRect[2] = 0;
			_dirtyRect[3] = 0;
			AddWhiteRect(2, 2);
			ClearState();
		}

		public Texture2D Texture { get; private set; }

		public event EventHandler AtlasFull;

		public void AddWhiteRect(int w, int h)
		{
			var x = 0;
			var y = 0;
			var gx = 0;
			var gy = 0;
			if (!_atlas.AddRect(w, h, ref gx, ref gy))
				return;
			fixed (byte* dst2 = &_texData[gx + gy * _params_.Width])
			{
				var dst = dst2;
				for (y = 0; y < h; y++)
				{
					for (x = 0; x < w; x++) dst[x] = 0xff;
					dst += _params_.Width;
				}
			}

			_dirtyRect[0] = Math.Min(_dirtyRect[0], gx);
			_dirtyRect[1] = Math.Min(_dirtyRect[1], gy);
			_dirtyRect[2] = Math.Max(_dirtyRect[2], gx + w);
			_dirtyRect[3] = Math.Max(_dirtyRect[3], gy + h);
		}

		public void ClearState()
		{
			Size = 12.0f;
			Color = Color.White;
			FontId = 0;
			BlurValue = 0;
			Spacing = 0;
			Alignment = Alignment.Left | Alignment.Baseline;
		}

		public int AddFontMem(string name, byte[] data)
		{
			var i = 0;
			var ascent = 0;
			var descent = 0;
			var fh = 0;
			var lineGap = 0;
			var font = new Font
			{
				Name = name,
				Data = data
			};
			fixed (byte* ptr = data)
			{
				if (LoadFont(font._font, ptr, data.Length) == 0)
					return -1;
			}

			font._font.fons__tt_getFontVMetrics(&ascent, &descent, &lineGap);
			fh = ascent - descent;
			font.Ascent = ascent;
			font.Ascender = ascent / (float)fh;
			font.Descender = descent / (float)fh;
			font.LineHeight = (fh + lineGap) / (float)fh;

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
			float width = 0;
			if (FontId < 0 || FontId >= _fonts.Count)
				return x;
			font = _fonts[FontId];
			if (font.Data == null)
				return x;
			scale = font._font.fons__tt_getPixelHeightScale(isize / 10.0f);

			if ((Alignment & Alignment.Left) != 0)
			{
			}
			else if ((Alignment & Alignment.Right) != 0)
			{
				var bounds = new Bounds();
				width = TextBounds(x, y, str, ref bounds);
				x -= width;
			}
			else if ((Alignment & Alignment.Center) != 0)
			{
				var bounds = new Bounds();
				width = TextBounds(x, y, str, ref bounds);
				x -= width * 0.5f;
			}

			float originX = 0.0f;
			float originY = 0.0f;

			originY += GetVertAlign(font, Alignment, isize);
			for (int i = 0; i < str.Length; i += char.IsSurrogatePair(str.String, i + str.Location) ? 2 : 1)
			{
				var codepoint = char.ConvertToUtf32(str.String, i + str.Location);
				glyph = GetGlyph(font, codepoint, isize, iblur, true);
				if (glyph != null)
				{
					GetQuad(font, prevGlyphIndex, glyph, scale, Spacing, ref originX, ref originY, &q);
					if (_vertsNumber + 6 > 1024)
					{
						Flush(batch, depth);
					}

					q.X0 = (int)(q.X0 * Scale.X);
					q.X1 = (int)(q.X1 * Scale.X);
					q.Y0 = (int)(q.Y0 * Scale.Y);
					q.Y1 = (int)(q.Y1 * Scale.Y);

					AddVertex(new Rectangle((int)(x + q.X0),
								(int)(y + q.Y0),
								(int)(q.X1 - q.X0),
								(int)(q.Y1 - q.Y0)),
							new Rectangle((int)(q.S0 * _params_.Width),
								(int)(q.T0 * _params_.Height),
								(int)((q.S1 - q.S0) * _params_.Width),
								(int)((q.T1 - q.T0) * _params_.Height)),
							Color);
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
			y += GetVertAlign(font, Alignment, isize);
			minx = maxx = x;
			miny = maxy = y;
			startx = x;
			for (int i = 0; i < str.Length; i += char.IsSurrogatePair(str.String, i + str.Location) ? 2 : 1)
			{
				var codepoint = char.ConvertToUtf32(str.String, i + str.Location);
				glyph = GetGlyph(font, codepoint, isize, iblur, false);
				if (glyph != null)
				{
					GetQuad(font, prevGlyphIndex, glyph, scale, Spacing, ref x, ref y, &q);
					if (q.X0 < minx)
						minx = q.X0;
					if (x > maxx)
						maxx = x;
					if (_params_.IsAlignmentTopLeft)
					{
						if (q.Y0 < miny)
							miny = q.Y0;
						if (q.Y1 > maxy)
							maxy = q.Y1;
					}
					else
					{
						if (q.Y1 < miny)
							miny = q.Y1;
						if (q.Y0 > maxy)
							maxy = q.Y0;
					}
				}

				prevGlyphIndex = glyph != null ? glyph.Index : -1;
			}

			advance = x - startx;
			if ((Alignment & Alignment.Left) != 0)
			{
			}
			else if ((Alignment & Alignment.Right) != 0)
			{
				minx -= advance;
				maxx -= advance;
			}
			else if ((Alignment & Alignment.Center) != 0)
			{
				minx -= advance * 0.5f;
				maxx -= advance * 0.5f;
			}

			bounds.X = minx;
			bounds.Y = miny;
			bounds.X2 = maxx;
			bounds.Y2 = maxy;

			return advance;
		}

		public void VertMetrics(out float ascender, out float descender, out float lineh)
		{
			ascender = descender = lineh = 0;
			Font font;
			int isize = 0;
			if (FontId < 0 || FontId >= _fonts.Count)
				return;
			font = _fonts[FontId];
			isize = (int)(Size * 10.0f);
			if (font.Data == null)
				return;

			ascender = font.Ascender * isize / 10.0f;
			descender = font.Descender * isize / 10.0f;
			lineh = font.LineHeight * isize / 10.0f;
		}

		public void LineBounds(float y, ref float miny, ref float maxy)
		{
			Font font;
			int isize = 0;
			if (FontId < 0 || FontId >= _fonts.Count)
				return;
			font = _fonts[FontId];
			isize = (int)(Size * 10.0f);
			if (font.Data == null)
				return;
			y += GetVertAlign(font, Alignment, isize);
			if (_params_.IsAlignmentTopLeft)
			{
				miny = y - font.Ascender * isize / 10.0f;
				maxy = miny + font.LineHeight * isize / 10.0f;
			}
			else
			{
				maxy = y + font.Descender * isize / 10.0f;
				miny = maxy - font.LineHeight * isize / 10.0f;
			}
		}

		public void ExpandAtlas(int width, int height)
		{
			var i = 0;
			var maxy = 0;
			width = Math.Max(width, _params_.Width);
			height = Math.Max(height, _params_.Height);
			if (width == _params_.Width && height == _params_.Height)
				return;

			Texture = null;

			var data = new byte[width * height];
			for (i = 0; i < _params_.Height; i++)
			{
				fixed (byte* dst = &data[i * width])
				{
					fixed (byte* src = &_texData[i * _params_.Width])
					{
						CRuntime.memcpy(dst, src, (ulong)_params_.Width);
						if (width > _params_.Width)
							CRuntime.memset(dst + _params_.Width, 0, (ulong)(width - _params_.Width));
					}
				}
			}

			if (height > _params_.Height)
				Array.Clear(data, _params_.Height * width, (height - _params_.Height) * width);

			_texData = data;

			_colorData = new Color[width * height];
			for(i = 0; i < width * height; ++i)
			{
				_colorData[i].R = _texData[i];
				_colorData[i].G = _texData[i];
				_colorData[i].B = _texData[i];
				_colorData[i].A = _texData[i];
			}

			_atlas.Expand(width, height);
			for (i = 0; i < _atlas.NodesNumber; i++) maxy = Math.Max(maxy, _atlas.Nodes[i].Y);
			_dirtyRect[0] = 0;
			_dirtyRect[1] = 0;
			_dirtyRect[2] = _params_.Width;
			_dirtyRect[3] = maxy;
			_params_.Width = width;
			_params_.Height = height;
			_itw = 1.0f / _params_.Width;
			_ith = 1.0f / _params_.Height;
		}

		public void ResetAtlas(int width, int height)
		{
			_atlas.Reset(width, height);
			_dirtyRect[0] = width;
			_dirtyRect[1] = height;
			_dirtyRect[2] = 0;
			_dirtyRect[3] = 0;

			for (var i = 0; i < _fonts.Count; i++)
			{
				var font = _fonts[i];
				font.Glyphs.Clear();
			}

			if (width == _params_.Width && height == _params_.Height)
				return;

			_texData = new byte[width * height];
			Array.Clear(_texData, 0, _texData.Length);
			_colorData = new Color[width * height];

			Texture = null;
			_params_.Width = width;
			_params_.Height = height;
			_itw = 1.0f / _params_.Width;
			_ith = 1.0f / _params_.Height;
			AddWhiteRect(2, 2);
		}

		public void ResetAtlas()
		{
			ResetAtlas(_params_.Width, _params_.Height);
		}

		private int LoadFont(stbtt_fontinfo font, byte* data, int dataSize)
		{
			return stbtt_InitFont(font, data, 0);
		}

		private void Blur(byte* dst, int w, int h, int dstStride, int blur)
		{
			var alpha = 0;
			float sigma = 0;
			if (blur < 1)
				return;
			sigma = blur * 0.57735f;
			alpha = (int)((1 << 16) * (1.0f - Math.Exp(-2.3f / (sigma + 1.0f))));
			BlurRows(dst, w, h, dstStride, alpha);
			BlurCols(dst, w, h, dstStride, alpha);
			BlurRows(dst, w, h, dstStride, alpha);
			BlurCols(dst, w, h, dstStride, alpha);
		}

		private FontGlyph GetGlyph(Font font, int codepoint, int isize, int iblur, bool isBitmapRequired)
		{
			var i = 0;
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
			var x = 0;
			var y = 0;
			float scale = 0;
			FontGlyph glyph = null;
			int h = 0;
			var size = isize / 10.0f;
			var pad = 0;
			var renderFont = font;
			if (isize < 2)
				return null;
			if (iblur > 20)
				iblur = 20;
			pad = iblur + 2;

			if (font.TryGetGlyph(codepoint, isize, iblur, out glyph))
			{
				if (!isBitmapRequired || glyph.X0 >= 0 && glyph.Y0 >= 0)
					return glyph;

			}
			g = font._font.fons__tt_getGlyphIndex(codepoint);
			if (g == 0)
			{
				throw new Exception(string.Format("Could not find glyph for codepoint {0}", codepoint));
			}

			scale = renderFont._font.fons__tt_getPixelHeightScale(size);
			renderFont._font.fons__tt_buildGlyphBitmap(g, size, scale, &advance, &lsb, &x0, &y0, &x1, &y1);
			gw = x1 - x0 + pad * 2;
			gh = y1 - y0 + pad * 2;
			if (isBitmapRequired)
			{
				if (!_atlas.AddRect(gw, gh, ref gx, ref gy))
				{
					var ev = AtlasFull;
					if (ev != null)
					{
						ev(this, EventArgs.Empty);
					}

					// Try again
					if (!_atlas.AddRect(gw, gh, ref gx, ref gy))
					{
						throw new Exception("FONS_ATLAS_FULL");
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
			glyph.X0 = gx;
			glyph.Y0 = gy;
			glyph.X1 = glyph.X0 + gw;
			glyph.Y1 = glyph.Y0 + gh;
			glyph.XAdvance = (int)(scale * advance * 10.0f);
			glyph.XOffset = x0 - pad;
			glyph.YOffset = y0 - pad;
			if (!isBitmapRequired) return glyph;

			fixed (byte* dst = &_texData[glyph.X0 + pad + (glyph.Y0 + pad) * _params_.Width])
			{
				renderFont._font.fons__tt_renderGlyphBitmap(dst, gw - pad * 2, gh - pad * 2, _params_.Width, scale,
					scale, g);
			}

			fixed (byte* dst = &_texData[glyph.X0 + glyph.Y0 * _params_.Width])
			{
				for (y = 0; y < gh; y++)
				{
					dst[y * _params_.Width] = 0;
					dst[gw - 1 + y * _params_.Width] = 0;
				}

				for (x = 0; x < gw; x++)
				{
					dst[x] = 0;
					dst[x + (gh - 1) * _params_.Width] = 0;
				}
			}

			if (iblur > 0)
			{
				fixed (byte* bdst = &_texData[glyph.X0 + glyph.Y0 * _params_.Width])
				{
					Blur(bdst, gw, gh, _params_.Width, iblur);
				}
			}

			_dirtyRect[0] = Math.Min(_dirtyRect[0], glyph.X0);
			_dirtyRect[1] = Math.Min(_dirtyRect[1], glyph.Y0);
			_dirtyRect[2] = Math.Max(_dirtyRect[2], glyph.X1);
			_dirtyRect[3] = Math.Max(_dirtyRect[3], glyph.Y1);
			return glyph;
		}

		private void GetQuad(Font font, int prevGlyphIndex, FontGlyph glyph, float scale,
			float spacing, ref float x, ref float y, FontGlyphSquad* q)
		{

			if (prevGlyphIndex != -1)
			{
				var adv = font._font.fons__tt_getGlyphKernAdvance(prevGlyphIndex, glyph.Index) * scale;
				x += (int)(adv + spacing + 0.5f);
			}

			float rx = 0;
			float ry = 0;

			if (_params_.IsAlignmentTopLeft)
			{
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
			}
			else
			{
				rx = x + glyph.XOffset;
				ry = y - glyph.YOffset;
				q->X0 = rx;
				q->Y0 = ry;
				q->X1 = rx + (glyph.X1 - glyph.X0);
				q->Y1 = ry - (glyph.Y1 + glyph.Y0);
				q->S0 = glyph.X0 * _itw;
				q->T0 = glyph.Y0 * _ith;
				q->S1 = glyph.X1 * _itw;
				q->T1 = glyph.Y1 * _ith;
			}

			x += (int)(glyph.XAdvance / 10.0f + 0.5f);
		}

		private void Flush(SpriteBatch batch, float depth)
		{
			if (Texture == null) Texture = new Texture2D(batch.GraphicsDevice, _params_.Width, _params_.Height);

			if (_dirtyRect[0] < _dirtyRect[2] && _dirtyRect[1] < _dirtyRect[3])
			{
				if (_texData != null)
				{
					var x = _dirtyRect[0];
					var y = _dirtyRect[1];
					var w = _dirtyRect[2] - x;
					var h = _dirtyRect[3] - y;
					var sz = w * h;
					for (var xx = x; xx < x + w; ++xx)
					{
						for (var yy = y; yy < y + h; ++yy)
						{
							var destPos = yy * _params_.Width + xx;

							var c = _texData[destPos];
							_colorData[destPos].R = c;
							_colorData[destPos].G = c;
							_colorData[destPos].B = c;
							_colorData[destPos].A = c;
						}
					}

					Texture.SetData(_colorData);
				}

				_dirtyRect[0] = _params_.Width;
				_dirtyRect[1] = _params_.Height;
				_dirtyRect[2] = 0;
				_dirtyRect[3] = 0;
			}

			if (_vertsNumber > 0)
			{
				for (var i = 0; i < _vertsNumber; ++i)
				{
					batch.Draw(Texture, _verts[i], _textureCoords[i], _colors[i], 0f, Vector2.Zero, SpriteEffects.None, depth);
				}

				_vertsNumber = 0;
			}
		}

		private void AddVertex(Rectangle destRect, Rectangle srcRect, Color c)
		{
			_verts[_vertsNumber] = destRect;
			_textureCoords[_vertsNumber] = srcRect;
			_colors[_vertsNumber] = c;
			_vertsNumber++;
		}

		private float GetVertAlign(Font font, Alignment align, int isize)
		{
			float result = 0.0f; ;
			if (_params_.IsAlignmentTopLeft)
			{
				if ((align & Alignment.Top) != 0)
				{
					result = font.Ascender * isize / 10.0f;
				}
				else if ((align & Alignment.Middle) != 0)
				{
					result = (font.Ascender + font.Descender) / 2.0f * isize / 10.0f;
				}
				else if ((align & Alignment.Baseline) != 0)
				{
				}
				else if ((align & Alignment.Bottom) != 0)
				{
					result = font.Descender * isize / 10.0f;
				}
			}
			else
			{
				if ((align & Alignment.Top) != 0)
				{
					result = -font.Ascender * isize / 10.0f;
				}
				else
				if ((align & Alignment.Middle) != 0)
				{
					result = -(font.Ascender + font.Descender) / 2.0f * isize / 10.0f;
				}
				else
				if ((align & Alignment.Baseline) != 0)
				{
				}
				else if ((align & Alignment.Bottom) != 0)
				{
					result = -font.Descender * isize / 10.0f;
				}
			}

			return result;
		}

		private static void BlurCols(byte* dst, int w, int h, int dstStride, int alpha)
		{
			var x = 0;
			var y = 0;
			for (y = 0; y < h; y++)
			{
				var z = 0;
				for (x = 1; x < w; x++)
				{
					z += (alpha * ((dst[x] << 7) - z)) >> 16;
					dst[x] = (byte)(z >> 7);
				}

				dst[w - 1] = 0;
				z = 0;
				for (x = w - 2; x >= 0; x--)
				{
					z += (alpha * ((dst[x] << 7) - z)) >> 16;
					dst[x] = (byte)(z >> 7);
				}

				dst[0] = 0;
				dst += dstStride;
			}
		}

		private static void BlurRows(byte* dst, int w, int h, int dstStride, int alpha)
		{
			var x = 0;
			var y = 0;
			for (x = 0; x < w; x++)
			{
				var z = 0;
				for (y = dstStride; y < h * dstStride; y += dstStride)
				{
					z += (alpha * ((dst[y] << 7) - z)) >> 16;
					dst[y] = (byte)(z >> 7);
				}

				dst[(h - 1) * dstStride] = 0;
				z = 0;
				for (y = (h - 2) * dstStride; y >= 0; y -= dstStride)
				{
					z += (alpha * ((dst[y] << 7) - z)) >> 16;
					dst[y] = (byte)(z >> 7);
				}

				dst[0] = 0;
				dst++;
			}
		}
	}
}
