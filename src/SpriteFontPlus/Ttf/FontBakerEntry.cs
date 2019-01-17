using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpriteFontPlus.Ttf
{
    public struct FontBakerEntry
    {
        public byte[] Ttf { get; private set; }
        public IEnumerable<CharacterRange> CharacterRanges { get; private set; }

        public FontBakerEntry(byte[] ttf, IEnumerable<CharacterRange> characterRanges)
        {
            if (ttf == null || ttf.Length == 0)
            {
                throw new ArgumentNullException(nameof(ttf));
            }
            
            if (characterRanges == null)
            {
                throw new ArgumentNullException(nameof(characterRanges));
            }

            if (!characterRanges.Any())
            {
                throw new ArgumentException("characterRanges must have a least one value.");
            }

            Ttf = ttf;
            CharacterRanges = characterRanges;
        }
    }
}