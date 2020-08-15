using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace SpriteFontPlus
{
	public static class SpriteBatchExtensions
	{
		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, string text, Vector2 pos, Color color)
		{
			return font.DrawString(batch, text, pos, color);
		}

		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, string text, Vector2 pos, Color color, Vector2 scale)
		{
			return font.DrawString(batch, text, pos, color, scale);
		}

		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, string text, Vector2 pos, Color[] glyphColors)
		{
			return font.DrawString(batch, text, pos, glyphColors);
		}

		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, string text, Vector2 pos, Color[] glyphColors, Vector2 scale)
		{
			return font.DrawString(batch, text, pos, glyphColors, scale);
		}

		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, StringBuilder text, Vector2 pos, Color color)
		{
			return font.DrawString(batch, text, pos, color);
		}

		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, StringBuilder text, Vector2 pos, Color color, Vector2 scale)
		{
			return font.DrawString(batch, text, pos, color, scale);
		}

		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, StringBuilder text, Vector2 pos, Color[] glyphColors)
		{
			return font.DrawString(batch, text, pos, glyphColors);
		}

		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, StringBuilder text, Vector2 pos, Color[] glyphColors, Vector2 scale)
		{
			return font.DrawString(batch, text, pos, glyphColors, scale);
		}
	}
}