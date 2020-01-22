using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SpriteFontPlus
{
	public class DynamicSpriteFont
	{
		internal struct TextureEnumerator : IEnumerable<Texture2D>
		{
			private readonly FontSystem _font;

			public TextureEnumerator(FontSystem font)
			{
				_font = font;
			}

			public IEnumerator<Texture2D> GetEnumerator()
			{
				foreach (var atlas in _font.Atlases)
				{
					yield return atlas.Texture;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		private static readonly string DefaultFontName = string.Empty;

		private readonly FontSystem _fontSystem;
		private readonly int _defaultFontId;

		public IEnumerable<Texture2D> Textures
		{
			get { return new TextureEnumerator(_fontSystem); }
		}

		public float Size
		{
			get
			{
				return _fontSystem.Size;
			}
			set
			{
				_fontSystem.Size = value;
			}
		}

		/// <summary>
		/// Blur level(0 - no blur)
		/// </summary>
		public float Blur
		{
			get
			{
				return _fontSystem.BlurValue;
			}

			set
			{
				_fontSystem.BlurValue = value;
			}
		}

		public float Spacing
		{
			get
			{
				return _fontSystem.Spacing;
			}
			set
			{
				_fontSystem.Spacing = value;
			}
		}

		public int FontId
		{
			get
			{
				return _fontSystem.FontId;
			}
			set
			{
				_fontSystem.FontId = value;
			}
		}

		public int DefaultFontId
		{
			get
			{
				return _defaultFontId;
			}
		}

		public bool UseKernings
		{
			get
			{
				return _fontSystem.UseKernings;
			}

			set
			{
				_fontSystem.UseKernings = value;
			}
		}

		public bool TryFallback
		{
			get
			{
				return _fontSystem.TryFallback;
				
			}
			set
			{
				_fontSystem.TryFallback = value;
				
			}
		}
		
		public event EventHandler CurrentAtlasFull
		{
			add
			{
				_fontSystem.CurrentAtlasFull += value;
			}

			remove
			{
				_fontSystem.CurrentAtlasFull -= value;
			}
		}

		private DynamicSpriteFont(byte[] ttf, float defaultSize, int textureWidth, int textureHeight)
		{
			_fontSystem = new FontSystem(textureWidth, textureHeight);

			_defaultFontId = _fontSystem.AddFontMem(DefaultFontName, ttf);
			Size = defaultSize;
		}

		public float DrawString(SpriteBatch batch, string text, Vector2 pos, Color color)
		{
			return DrawString(batch, text, pos, color, Vector2.One);
		}

		public float DrawString(SpriteBatch batch, string text, Vector2 pos, Color color, Vector2 scale, float depth = 0f)
		{
			_fontSystem.Color = color;
			_fontSystem.Scale = scale;

			var result = _fontSystem.DrawText(batch, pos.X, pos.Y, text, depth);

			_fontSystem.Scale = Vector2.One;

			return result;
		}

		public int AddTtf(string name, byte[] ttf)
		{
			return _fontSystem.AddFontMem(name, ttf);
		}

		public int? GetFontIdByName(string name)
		{
			return _fontSystem.GetFontByName(name);
		}

		public Vector2 MeasureString(string text)
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

		public void Reset(int width, int height)
		{
			_fontSystem.Reset(width, height);
		}

		public void Reset()
		{
			_fontSystem.Reset();
		}

		public static DynamicSpriteFont FromTtf(byte[] ttf, float defaultSize, int textureWidth = 1024, int textureHeight = 1024)
		{
			return new DynamicSpriteFont(ttf, defaultSize, textureWidth, textureHeight);
		}
	}
}
