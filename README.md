# SpriteFontPlus
[![NuGet](https://img.shields.io/nuget/v/SpriteFontPlus.MonoGame.svg)](https://www.nuget.org/packages/SpriteFontPlus.MonoGame/) [![Build status](https://ci.appveyor.com/api/projects/status/2mbacxymarcxq4we?svg=true)](https://ci.appveyor.com/project/RomanShapiro/spritefontplus)

Library that extends functionality of the SpriteFont. For now it has only one feature - ability to create SpiteFont dynamically from ttf.

# Adding Reference
1. `Install-Package SpriteFontPlus.MonoGame` (or `Install-Package SpriteFontPlus.FNA` for FNA)

# Loading SpriteFont from a ttf
2. Following code creates a SpriteFont from a ttf:
```c#
var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(@"C:\\Windows\\Fonts\arial.ttf"),
	25,
	1024,
	1024,
	new[]
	{
		CharacterRange.BasicLatin,
		CharacterRange.Latin1Supplement,
		CharacterRange.LatinExtendedA,
		CharacterRange.Cyrillic
	}
);

SpriteFont font = fontBakeResult.CreateSpriteFont(GraphicsDevice);
```

# DynamicSpriteFont
DynamicSpriteFont allows to render glyphs of different fonts and sizes on demand(it doesnt require to explicity specify what glyphs are required during the font creation).
It's usage is demonstrating in the following sample:
[samples/SpriteFontPlus.Samples.DynamicSpriteFont/Game1.cs](samples/SpriteFontPlus.Samples.DynamicSpriteFont/Game1.cs)

It demonstrates creation of DynamicSpriteFont from 3 different ttfs and rendering it with different sizes:
![](/images/sampleDynamicSpriteFont.png)

## Credits
* [MonoGame](http://www.monogame.net/)
* [Cyotek.Drawing.BitmapFont](https://github.com/cyotek/Cyotek.Drawing.BitmapFont)
* [stb](https://github.com/nothings/stb)
* [fontstash](https://github.com/memononen/fontstash)
