using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteFontPlus
{
	public class DynamicSpriteFont
	{
		private static readonly string DefaultFontName = string.Empty;

		private readonly FontSystem _fontSystem;
		private readonly int _defaultFontId;
		public Texture2D Texture
		{
			get { return _fontSystem.Texture; }
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

		private DynamicSpriteFont(byte[] ttf, float defaultSize, int textureWidth, int textureHeight)
		{
			var fontParams = new FontSystemParams
			{
				Width = textureWidth,
				Height = textureHeight,
				Flags = FontSystem.FONS_ZERO_TOPLEFT
			};

			_fontSystem = new FontSystem(fontParams);

			_defaultFontId = _fontSystem.AddFontMem(DefaultFontName, ttf);
			Size = defaultSize;
		}

		public float DrawString(SpriteBatch batch, string text, Vector2 pos, Color color)
		{
			var font = _fontSystem.Fonts[FontId];
			var scale = font._font.fons__tt_getPixelHeightScale(Size);

			_fontSystem.Color = color;

			var yOff = font.Ascent * scale;

			return _fontSystem.DrawText(batch, pos.X, pos.Y + yOff, text);
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

			 return new Vector2(bounds.X2 - bounds.X, bounds.Y2 - bounds.Y);
		}

		public static DynamicSpriteFont FromTtf(byte[] ttf, float defaultSize, int textureWidth = 1024, int textureHeight = 1024)
		{
			return new DynamicSpriteFont(ttf, defaultSize, textureWidth, textureHeight);
		}
	}
}