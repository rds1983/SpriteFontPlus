using System.Runtime.InteropServices;

namespace SpriteFontPlus
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GlyphInfo
    {
        public int X0;
        public int Y0;
        public int X1;
        public int Y1;
        public float XOffset;
        public float YOffset;
        public float XAdvance;
        public float XOffset2;
        public float YOffset2;
    }
}