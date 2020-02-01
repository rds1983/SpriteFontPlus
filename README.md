# SpriteFontPlus
[![NuGet](https://img.shields.io/nuget/v/SpriteFontPlus.svg)](https://www.nuget.org/packages/SpriteFontPlus/) [![Build status](https://ci.appveyor.com/api/projects/status/2mbacxymarcxq4we?svg=true)](https://ci.appveyor.com/project/RomanShapiro/spritefontplus) [![Chat](https://img.shields.io/discord/628186029488340992.svg)](https://discord.gg/ZeHxhCY)

Library that extends functionality of the SpriteFont.

# Features
* Creation of SpriteFont in the run-time from ttf.
* Creation of SpriteFont in the run-time from AngelCode BMFont(only XML with single texture works for now).
* DynamicSpriteFont class that renders glyphs on demand to the underlying texture atlas. Also it supports 32-bit characters and blurry text.

# Adding Reference
There are two ways of referencing SpriteFontPlus in the project:
1. Through nuget(works only for MonoGame): `install-package SpriteFontPlus` or `dotnet add <csproj path> package SpriteFontPlus` for .NET Core
2. As submodule(works for both MonoGame and FNA):
    
    a. `git submodule add https://github.com/rds1983/SpriteFontPlus.git`
    
    b. `git submodule update --init --recursive`
    
    c. Copy SolutionDefines.targets from SpriteFontPlus/build/MonoGame(or SpriteFontPlus/build/FNA) to your solution folder.

      * If FNA is used, SolutionDefines.targets needs to be edited and FNAProj variable should be updated to the location of FNA.csproj next to the SpriteFontPlus.csproj location.
    
    d. Add SpriteFontPlus/src/SpriteFontPlus/SpriteFontPlus.csproj to the solution.
    
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


# DynamicSpriteFont
DynamicSpriteFont renders glyphs on demand to the underlying texture atlas. Thus it doesnt require to explicity specify character ranges that are going to be used during the font creation.

Following code creates DynamicSpriteFont from 3 different ttfs:
```c#
	_font = DynamicSpriteFont.FromTtf(File.ReadAllBytes(@"Fonts/DroidSans.ttf"), 20);
	_font.AddTtf(File.ReadAllBytes(@"Fonts/DroidSansJapanese.ttf"));
	_font.AddTtf(File.ReadAllBytes(@"Fonts/Symbola-Emoji.ttf"));
```			

Now it's possible to draw text using glyphs from all those fonts, using Size property to change the font size. 

I.e. consider following code:
```c#
	_spriteBatch.Begin();

	// Render some text
	_font.Size = 18;
	_spriteBatch.DrawString(_font, "The quick いろは brown\nfox にほへ jumps over\nt🙌h📦e l👏a👏zy dog", new Vector2(0, 0), Color.White);

	_font.Size = 30;
	_spriteBatch.DrawString(_font, "The quick いろは brown\nfox にほへ jumps over\nt🙌h📦e l👏a👏zy dog", new Vector2(0, 80), Color.Yellow);

	_spriteBatch.End();
```

It would render following:
![](/images/sampleDynamicSpriteFont.png)

Full sample is here:
[samples/SpriteFontPlus.Samples.DynamicSpriteFont](samples/SpriteFontPlus.Samples.DynamicSpriteFont)

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
* [fontstash](https://github.com/memononen/fontstash)
