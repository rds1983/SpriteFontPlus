using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteFontPlus
{
    public static class SpriteBatchExtensions
    {
        public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font,
            float pixelHeight, string _string_, Vector2 pos, Color color)
        {
            return font.DrawString(batch, pixelHeight,_string_,  pos, color);
        }
    }
}