using FontStashSharp;
using FontStashSharp.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpriteFontPlus
{
	public class DynamicSpriteFont : IDisposable
	{
		internal struct TextureEnumerator : IEnumerable<Texture2D>
		{
			readonly FontSystem _font;

			public TextureEnumerator(FontSystem font)
			{
				_font = font;
			}

			public IEnumerator<Texture2D> GetEnumerator()
			{
				foreach (var atlas in _font.Atlases)
				{
					var Texture2DWrapper = (Texture2DWrapper)atlas.Texture;
					yield return Texture2DWrapper.Texture;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		readonly FontSystem _fontSystem;

		public IEnumerable<Texture2D> Textures
		{
			get { return new TextureEnumerator(_fontSystem); }
		}

		public int FontSize
		{
			get { return _fontSystem.FontSize; }
			set { _fontSystem.FontSize = value; }
		}

		public float CharacterSpacing
		{
			get { return _fontSystem.CharacterSpacing; }
			set { _fontSystem.CharacterSpacing = value; }
		}

		public Vector2 Scale
		{
			get { return _fontSystem.Scale; }
			set { _fontSystem.Scale = value; }
		}

		public bool UseKernings
		{
			get { return _fontSystem.UseKernings; }

			set { _fontSystem.UseKernings = value; }
		}

		public int? DefaultCharacter
		{
			get { return _fontSystem.DefaultCharacter; }

			set { _fontSystem.DefaultCharacter = value; }
		}

		public event EventHandler CurrentAtlasFull
		{
			add { _fontSystem.CurrentAtlasFull += value; }

			remove { _fontSystem.CurrentAtlasFull -= value; }
		}

		DynamicSpriteFont(GraphicsDevice graphicsDevice, byte[] ttf, int defaultSize, int textureWidth, int textureHeight, int blur, int stroke)
		{
			var textureCreator = new Texture2DCreator(graphicsDevice);

			_fontSystem = new FontSystem(StbTrueTypeSharpFontLoader.Instance, textureCreator, textureWidth, textureHeight, blur, stroke)
			{
				FontSize = defaultSize
			};

			_fontSystem.AddFontMem(ttf);
		}

		public void Dispose()
		{
			_fontSystem?.Dispose();
		}

		public float DrawString(IFontStashRenderer renderer, string text, Vector2 pos, Color color, float depth = 0f)
		{
			return _fontSystem.DrawText(renderer, pos.X, pos.Y, text, color, depth);
		}

		public float DrawString(IFontStashRenderer renderer, string text, Vector2 pos, Color[] glyphColors, float depth = 0f)
		{
			return _fontSystem.DrawText(renderer, pos.X, pos.Y, text, glyphColors, depth);
		}

		public float DrawString(IFontStashRenderer renderer, StringBuilder text, Vector2 pos, Color color, float depth = 0f)
		{
			return _fontSystem.DrawText(renderer, pos.X, pos.Y, text, color, depth);
		}

		public float DrawString(IFontStashRenderer renderer, StringBuilder text, Vector2 pos, Color[] glyphColors, float depth = 0f)
		{
			return _fontSystem.DrawText(renderer, pos.X, pos.Y, text, glyphColors, depth);
		}

		public float DrawString(SpriteBatch batch, string text, Vector2 pos, Color color, float depth = 0f)
		{
			var renderer = SpriteBatchRenderer.Instance;
			renderer.Batch = batch;
			return _fontSystem.DrawText(renderer, pos.X, pos.Y, text, color, depth);
		}

		public float DrawString(SpriteBatch batch, string text, Vector2 pos, Color[] glyphColors, float depth = 0f)
		{
			var renderer = SpriteBatchRenderer.Instance;
			renderer.Batch = batch;

			return _fontSystem.DrawText(renderer, pos.X, pos.Y, text, glyphColors, depth);
		}

		public float DrawString(SpriteBatch batch, StringBuilder text, Vector2 pos, Color color, float depth = 0f)
		{
			var renderer = SpriteBatchRenderer.Instance;
			renderer.Batch = batch;

			return _fontSystem.DrawText(renderer, pos.X, pos.Y, text, color, depth);
		}

		public float DrawString(SpriteBatch batch, StringBuilder text, Vector2 pos, Color[] glyphColors, float depth = 0f)
		{
			var renderer = SpriteBatchRenderer.Instance;
			renderer.Batch = batch;

			return _fontSystem.DrawText(renderer, pos.X, pos.Y, text, glyphColors, depth);
		}

		public void AddTtf(byte[] ttf)
		{
			_fontSystem.AddFontMem(ttf);
		}

		public void AddTtf(Stream ttfStream)
		{
			AddTtf(ttfStream.ToByteArray());
		}

		public Vector2 MeasureString(string text)
		{
			Bounds bounds = new Bounds();
			_fontSystem.TextBounds(0, 0, text, ref bounds);

			return new Vector2(bounds.X2, bounds.Y2);
		}

		public Vector2 MeasureString(StringBuilder text)
		{
			Bounds bounds = new Bounds();
			_fontSystem.TextBounds(0, 0, text, ref bounds);

			return new Vector2(bounds.X2, bounds.Y2);
		}

		public Rectangle GetTextBounds(Vector2 position, string text)
		{
			Bounds bounds = new Bounds();
			_fontSystem.TextBounds(position.X, position.Y, text, ref bounds);

			return new Rectangle((int)bounds.X, (int)bounds.Y, (int)(bounds.X2 - bounds.X), (int)(bounds.Y2 - bounds.Y));
		}

		public Rectangle GetTextBounds(Vector2 position, StringBuilder text)
		{
			Bounds bounds = new Bounds();
			_fontSystem.TextBounds(position.X, position.Y, text, ref bounds);

			return new Rectangle((int)bounds.X, (int)bounds.Y, (int)(bounds.X2 - bounds.X), (int)(bounds.Y2 - bounds.Y));
		}

		public List<Rectangle> GetGlyphRects(Vector2 position, string text)
		{
			return _fontSystem.GetGlyphRects(position.X, position.Y, text);
		}

		public List<Rectangle> GetGlyphRects(Vector2 position, StringBuilder text)
		{
			return _fontSystem.GetGlyphRects(position.X, position.Y, text);
		}

		public void Reset(int width, int height)
		{
			_fontSystem.Reset(width, height);
		}

		public void Reset()
		{
			_fontSystem.Reset();
		}

		public static DynamicSpriteFont FromTtf(GraphicsDevice graphicsDevice, byte[] ttf, int defaultSize, int textureWidth = 1024, int textureHeight = 1024, int blur = 0, int stroke = 0)
		{
			return new DynamicSpriteFont(graphicsDevice, ttf, defaultSize, textureWidth, textureHeight, blur, stroke);
		}

		public static DynamicSpriteFont FromTtf(GraphicsDevice graphicsDevice, Stream ttfStream, int defaultSize, int textureWidth = 1024, int textureHeight = 1024, int blur = 0, int stroke = 0)
		{
			return FromTtf(graphicsDevice, ttfStream.ToByteArray(), defaultSize, textureWidth, textureHeight, blur, stroke);
		}
	}
}
