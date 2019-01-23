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
                width = textureWidth, height = textureHeight, flags = FontSystem.FONS_ZERO_TOPLEFT
            };
            
            _fontSystem = new FontSystem(fontParams);
            
            _fontId = _fontSystem.fonsAddFontMem(string.Empty, ttf, 0);
        }
      
        public float DrawString(SpriteBatch batch, float pixelHeight, string _string_, Vector2 pos, Color color)
        {
	        _fontSystem.fonsSetFont(_fontId);
	        _fontSystem.fonsSetSize(pixelHeight);
	        _fontSystem.fonsSetColor(color.PackedValue);

            var font = _fontSystem.fonts[_fontSystem.fons__getState().font];
            var scale = font.font.fons__tt_getPixelHeightScale((float)(pixelHeight));
            
            var yOff = font.ascent * scale;

            return _fontSystem.fonsDrawText(batch, pos.X, pos.Y + yOff, _string_);
        }

        public static DynamicSpriteFont FromTtf(byte[] ttf, int textureWidth = 1024, int textureHeight = 1024)
        {
            return new DynamicSpriteFont(ttf, textureWidth, textureHeight);
        }
    }
}