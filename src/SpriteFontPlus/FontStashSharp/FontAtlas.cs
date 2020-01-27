using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FontStashSharp
{
	internal unsafe class FontAtlas
	{
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

		public FontAtlas(int w, int h, int count)
		{
			Width = w;
			Height = h;
			Nodes = new FontAtlasNode[count];
			count = 0;
			Nodes[0].X = 0;
			Nodes[0].Y = 0;
			Nodes[0].Width = w;
			NodesNumber++;
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

		public void RenderGlyph(GraphicsDevice device, FontGlyph glyph)
		{
			var pad = glyph.Pad;

			// Render glyph to byte buffer
			var buffer = new byte[glyph.Bounds.Width * glyph.Bounds.Height];
			Array.Clear(buffer, 0, buffer.Length);

			var g = glyph.Index;
			fixed (byte* dst = &buffer[pad + pad * glyph.Bounds.Width])
			{
				glyph.Font.RenderGlyphBitmap(dst, 
					glyph.Bounds.Width - pad * 2, 
					glyph.Bounds.Height - pad * 2,
					glyph.Bounds.Width, 
					g);
			}

			if (glyph.Blur > 0)
			{
				fixed (byte* bdst = &buffer[0])
				{
					Blur(bdst, glyph.Bounds.Width, glyph.Bounds.Height, glyph.Bounds.Width, glyph.Blur);
				}
			}

			// Byte buffer to RGBA
			var colorBuffer = new Color[glyph.Bounds.Width * glyph.Bounds.Height];
			for (var i = 0; i < colorBuffer.Length; ++i)
			{
				var c = buffer[i];
				colorBuffer[i].R = colorBuffer[i].G = colorBuffer[i].B = colorBuffer[i].A = c;
			}

			// Write to texture
			if (Texture == null)
			{
				Texture = new Texture2D(device, Width, Height);
			}

			Texture.SetData(0, glyph.Bounds, colorBuffer, 0, colorBuffer.Length);
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
	}
}