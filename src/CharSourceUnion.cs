using System.Text;

namespace SpriteFontPlus
{
    public readonly ref struct CharSourceUnion
    {
        public readonly StringBuilder StringBuilder;
        public readonly string String;

        private CharSourceUnion(StringBuilder stringBuilder, string @string)
        {
            StringBuilder = stringBuilder;
            String = @string;
        }

        public static implicit operator CharSourceUnion(string str) => CreateString(str);
        public static implicit operator CharSourceUnion(StringBuilder str) => CreateStringBuilder(str);

        public static CharSourceUnion CreateString(string str) => 
            new CharSourceUnion(stringBuilder: null, @string: str);

        public static CharSourceUnion CreateStringBuilder(StringBuilder stringBuilder) =>
            new CharSourceUnion(
                stringBuilder: stringBuilder,
                @string: null
            );

        public bool IsStringSource => String != null;
        public bool IsStringBuilderSource => StringBuilder != null;
    }
}