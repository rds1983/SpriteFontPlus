using Microsoft.Xna.Framework;

namespace FontStashSharp
{
	internal class FontSystemState
	{
		public int FontId;
		public int Alignment;
		public float Size;
		public Color Color;
		public float Blur;
		public float Spacing;

		public FontSystemState Clone()
		{
			return new FontSystemState
			{
				FontId = FontId,
				Alignment = Alignment,
				Size = Size,
				Color = Color,
				Blur = Blur,
				Spacing = Spacing
			};
		}
	}
}