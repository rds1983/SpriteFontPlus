using Microsoft.Xna.Framework;

namespace FontStashSharp
{
	internal struct RenderItem
	{
		public FontAtlas Atlas;
		public Rectangle _textureCoords;
		public Rectangle _verts;
		public Color _colors;
	}
}
