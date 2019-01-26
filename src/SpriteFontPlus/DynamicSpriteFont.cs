using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteFontPlus
{
	public class DynamicSpriteFont
	{
		private readonly FontSystem _fontSystem;
		private readonly int _fontId;

		public Texture2D Texture
		{
			get { return _fontSystem.Texture; }
		}

		private DynamicSpriteFont(byte[] ttf, int textureWidth, int textureHeight)
		{
			var fontParams = new FontSystemParams
			{
				Width = textureWidth,
				Height = textureHeight,
				Flags = FontSystem.FONS_ZERO_TOPLEFT
			};

			_fontSystem = new FontSystem(fontParams);

			_fontId = _fontSystem.AddFontMem(string.Empty, ttf);
		}

		public float DrawString(SpriteBatch batch, float pixelHeight, string _string_, Vector2 pos, Color color)
		{
			_fontSystem.SetFont(_fontId);
			_fontSystem.SetSize(pixelHeight);
			_fontSystem.SetColor(color);

			var font = _fontSystem.Fonts[_fontSystem.GetState().FontId];
			var scale = font._font.fons__tt_getPixelHeightScale((float)(pixelHeight));

			var yOff = font.Ascent * scale;

			return _fontSystem.DrawText(batch, pos.X, pos.Y + yOff, _string_);
		}

		public static DynamicSpriteFont FromTtf(byte[] ttf, int textureWidth = 1024, int textureHeight = 1024)
		{
			return new DynamicSpriteFont(ttf, textureWidth, textureHeight);
		}
	}
}