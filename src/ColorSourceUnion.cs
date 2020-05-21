using Microsoft.Xna.Framework;

namespace SpriteFontPlus
{
    public ref struct ColorSourceUnion
    {
        public readonly Color Color;
        public readonly Color[] GlyphColors;

        private ColorSourceUnion(Color color, Color[] glyphColors)
        {
            Color = color;
            GlyphColors = glyphColors;
        }

        public static implicit operator ColorSourceUnion(Color color) => CreateColor(color);
        public static implicit operator ColorSourceUnion(Color[] glyphColors) => CreateGlyphColors(glyphColors);

        public static ColorSourceUnion CreateColor(Color color) =>
            new ColorSourceUnion(color: color, glyphColors: default);

        public static ColorSourceUnion CreateGlyphColors(Color[] glyphColors) =>
            new ColorSourceUnion(color: default, glyphColors: glyphColors);

        public bool IsColor => Color != default;
        
        public bool IsGlyphColors => GlyphColors != default;
    }
}