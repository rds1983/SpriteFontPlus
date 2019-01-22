using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteFontPlus
{
    public class DynamicSpriteFont
    {
        private static readonly FontSystem _fontSystem;
        private readonly int _fontId;

        static DynamicSpriteFont()
        {
            var fontParams = new FontSystemParams
            {
                width = 1024, height = 1024, flags = FontSystem.FONS_ZERO_TOPLEFT
            };
            
            _fontSystem = new FontSystem(fontParams);
        }

        private DynamicSpriteFont(int fontId)
        {
            _fontId = fontId;
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

        public static DynamicSpriteFont FromTTF(byte[] ttf)
        {
            var fontId = _fontSystem.fonsAddFontMem(string.Empty, ttf, 0);
            
            return new DynamicSpriteFont(fontId);
        }
    }
}