using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using System;

namespace FontStashSharp
{
	internal unsafe class FontAtlas
	{
		private byte[] _texData;
		private Color[] _colorData;
		private int[] _dirtyRect = new int[4];

		public int Width
		{
			get; private set;
		}

		public int Height
		{
			get; private set;
		}

		public int NodesNumber
		{
			get; private set;
		}

		public FontAtlasNode[] Nodes
		{
			get; private set;
		}

		public Texture2D Texture
		{
			get; set;
		}

		public int Index
		{
			get; private set;
		}

		public FontAtlas(int w, int h, int count, int index)
		{
			Width = w;
			Height = h;
			Nodes = new FontAtlasNode[count];
			Index = index;
			count = 0;
			Nodes[0].X = 0;
			Nodes[0].Y = 0;
			Nodes[0].Width = w;
			NodesNumber++;

			_texData = new byte[w * h];
			_colorData = new Color[w * h];
			Array.Clear(_texData, 0, _texData.Length);

			_dirtyRect[0] = Width;
			_dirtyRect[1] = Height;
			_dirtyRect[2] = 0;
			_dirtyRect[3] = 0;
		}

		public void AddWhiteRect(int w, int h)
		{
			var x = 0;
			var y = 0;
			var gx = 0;
			var gy = 0;
			if (!AddRect(w, h, ref gx, ref gy))
				return;
			fixed (byte* dst2 = &_texData[gx + gy * Width])
			{
				var dst = dst2;
				for (y = 0; y < h; y++)
				{
					for (x = 0; x < w; x++)
						dst[x] = 0xff;
					dst += Width;
				}
			}

			_dirtyRect[0] = Math.Min(_dirtyRect[0], gx);
			_dirtyRect[1] = Math.Min(_dirtyRect[1], gy);
			_dirtyRect[2] = Math.Max(_dirtyRect[2], gx + w);
			_dirtyRect[3] = Math.Max(_dirtyRect[3], gy + h);
		}

		public void InsertNode(int idx, int x, int y, int w)
		{
			if (NodesNumber + 1 > Nodes.Length)
			{
				var oldNodes = Nodes;
				var newLength = Nodes.Length == 0 ? 8 : Nodes.Length * 2;
				Nodes = new FontAtlasNode[newLength];
				for (var i = 0; i < oldNodes.Length; ++i)
				{
					Nodes[i] = oldNodes[i];
				}
			}

			for (var i = NodesNumber; i > idx; i--)
				Nodes[i] = Nodes[i - 1];
			Nodes[idx].X = x;
			Nodes[idx].Y = y;
			Nodes[idx].Width = w;
			NodesNumber++;
		}

		public void RemoveNode(int idx)
		{
			if (NodesNumber == 0)
				return;
			for (var i = idx; i < NodesNumber - 1; i++)
				Nodes[i] = Nodes[i + 1];
			NodesNumber--;
		}

		public void Expand(int w, int h)
		{
			if (w > Width)
				InsertNode(NodesNumber, Width, 0, w - Width);
			Width = w;
			Height = h;
		}

		public void Reset(int w, int h)
		{
			Width = w;
			Height = h;
			NodesNumber = 0;
			Nodes[0].X = 0;
			Nodes[0].Y = 0;
			Nodes[0].Width = w;
			NodesNumber++;
		}

		public bool AddSkylineLevel(int idx, int x, int y, int w, int h)
		{
			InsertNode(idx, x, y + h, w);
			for (var i = idx + 1; i < NodesNumber; i++)
				if (Nodes[i].X < Nodes[i - 1].X + Nodes[i - 1].Width)
				{
					var shrink = Nodes[i - 1].X + Nodes[i - 1].Width - Nodes[i].X;
					Nodes[i].X += shrink;
					Nodes[i].Width -= shrink;
					if (Nodes[i].Width <= 0)
					{
						RemoveNode(i);
						i--;
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}

			for (var i = 0; i < NodesNumber - 1; i++)
				if (Nodes[i].Y == Nodes[i + 1].Y)
				{
					Nodes[i].Width += Nodes[i + 1].Width;
					RemoveNode(i + 1);
					i--;
				}

			return true;
		}

		public int RectFits(int i, int w, int h)
		{
			var x = Nodes[i].X;
			var y = Nodes[i].Y;
			if (x + w > Width)
				return -1;
			var spaceLeft = w;
			while (spaceLeft > 0)
			{
				if (i == NodesNumber)
					return -1;
				y = Math.Max(y, Nodes[i].Y);
				if (y + h > Height)
					return -1;
				spaceLeft -= Nodes[i].Width;
				++i;
			}

			return y;
		}

		public bool AddRect(int rw, int rh, ref int rx, ref int ry)
		{
			var besth = Height;
			var bestw = Width;
			var besti = -1;
			var bestx = -1;
			var besty = -1;
			for (var i = 0; i < NodesNumber; i++)
			{
				var y = RectFits(i, rw, rh);
				if (y != -1)
					if (y + rh < besth || y + rh == besth && Nodes[i].Width < bestw)
					{
						besti = i;
						bestw = Nodes[i].Width;
						besth = y + rh;
						bestx = Nodes[i].X;
						besty = y;
					}
			}

			if (besti == -1)
				return false;
			if (!AddSkylineLevel(besti, bestx, besty, rw, rh))
				return false;

			rx = bestx;
			ry = besty;
			return true;
		}

		public void RenderGlyph(Font renderFont, FontGlyph glyph, int gw, int gh, float scale)
		{
			var pad = glyph.Pad;

			var g = glyph.Index;
			fixed (byte* dst = &_texData[glyph.X0 + pad + (glyph.Y0 + pad) * Width])
			{
				renderFont._font.fons__tt_renderGlyphBitmap(dst, gw - pad * 2, gh - pad * 2, Width, scale,
					scale, g);
			}

			fixed (byte* dst = &_texData[glyph.X0 + glyph.Y0 * Width])
			{
				for (var y = 0; y < gh; y++)
				{
					dst[y * Width] = 0;
					dst[gw - 1 + y * Width] = 0;
				}

				for (var x = 0; x < gw; x++)
				{
					dst[x] = 0;
					dst[x + (gh - 1) * Width] = 0;
				}
			}

			if (glyph.Blur > 0)
			{
				fixed (byte* bdst = &_texData[glyph.X0 + glyph.Y0 * Width])
				{
					Blur(bdst, gw, gh, Width, glyph.Blur);
				}
			}

			_dirtyRect[0] = Math.Min(_dirtyRect[0], glyph.X0);
			_dirtyRect[1] = Math.Min(_dirtyRect[1], glyph.Y0);
			_dirtyRect[2] = Math.Max(_dirtyRect[2], glyph.X1);
			_dirtyRect[3] = Math.Max(_dirtyRect[3], glyph.Y1);
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

		public void Flush(GraphicsDevice graphicsDevice)
		{
			if (Texture == null)
				Texture = new Texture2D(graphicsDevice, Width, Height);

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
							var destPos = yy * Width + xx;

							var c = _texData[destPos];
							_colorData[destPos].R = c;
							_colorData[destPos].G = c;
							_colorData[destPos].B = c;
							_colorData[destPos].A = c;
						}
					}

					Texture.SetData(_colorData);
				}

				_dirtyRect[0] = Width;
				_dirtyRect[1] = Height;
				_dirtyRect[2] = 0;
				_dirtyRect[3] = 0;
			}
		}
	}
}