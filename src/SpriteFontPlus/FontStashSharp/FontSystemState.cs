namespace FontStashSharp
{
	internal class FontSystemState
	{
		public int font;
		public int align;
		public float size;
		public uint color;
		public float blur;
		public float spacing;

		public FontSystemState Clone()
		{
			return new FontSystemState
			{
				font = font,
				align = align,
				size = size,
				color = color,
				blur = blur,
				spacing = spacing
			};
		}
	}
}