**BREAKING CHANGE:** DynamicSpriteFont had been moved to the project: https://github.com/rds1983/FontStashSharp

# SpriteFontPlus
[![NuGet](https://img.shields.io/nuget/v/SpriteFontPlus.svg)](https://www.nuget.org/packages/SpriteFontPlus/)
![Build & Publish](https://github.com/rds1983/SpriteFontPlus/workflows/Build%20&%20Publish/badge.svg)
[![Chat](https://img.shields.io/discord/628186029488340992.svg)](https://discord.gg/ZeHxhCY)

Library that extends functionality of the SpriteFont.

# Features
* Creation of SpriteFont in the run-time from ttf.
* Creation of SpriteFont in the run-time from AngelCode BMFont(only XML with single texture works for now).

# Adding Reference
There are two ways of referencing SpriteFontPlus in the project:
1. Through nuget(works only for MonoGame): https://www.nuget.org/packages/SpriteFontPlus/
2. As source code(works for both MonoGame and FNA):
    
    a. Clone this repo.
    
    b. Execute `git submodule update --init --recursive` within the folder the repo was cloned to.
    
    c. Add src/SpriteFontPlus.MonoGame.csproj or src/SpriteFontPlus.FNA.csproj to the solution.

      * If FNA is used, then the folder structure is expected to be following: ![Folder Structure](/images/FolderStructure.png)
            
# Loading SpriteFont from a ttf
Following code creates a SpriteFont from a ttf:
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
Full sample is here:
[samples/SpriteFontPlus.Samples.TtfBaking](samples/SpriteFontPlus.Samples.TtfBaking)


# Loading SpriteFont from AngelCode BMFont
```c#
Texture2D texture;
using (var stream = TitleContainer.OpenStream("Fonts/test_0.png"))
{
	texture = Texture2D.FromStream(GraphicsDevice, stream);
}

string fontData;
using (var stream = TitleContainer.OpenStream("Fonts/test.fnt"))
{
	using (var reader = new StreamReader(stream))
	{
		fontData = reader.ReadToEnd();
	}
}

// As we use font with one texture, always return it independently from requested name
SpriteFont font = BMFontLoader.LoadXml(fontData, name => texture);
```

Full sample is here:
[samples/SpriteFontPlus.Samples.BMFont](samples/SpriteFontPlus.Samples.BMFont)

## Building From Source Code
1. Clone this repo.
2. `git submodule update --init --recursive`
3. Open a solution from the "build" folder.

## Credits
* [MonoGame](http://www.monogame.net/)
* [FNA](https://github.com/FNA-XNA/FNA)
* [BMFont](https://www.angelcode.com/products/bmfont/)
* [Cyotek.Drawing.BitmapFont](https://github.com/cyotek/Cyotek.Drawing.BitmapFont)
* [stb](https://github.com/nothings/stb)
