rem delete existing
rmdir "ZipPackage" /Q /S

rem Create required folders
mkdir "ZipPackage"
mkdir "ZipPackage\MonoGame"
mkdir "ZipPackage\FNA"

set "CONFIGURATION=Release\net45"

rem Copy output files
copy "src\SpriteFontPlus\bin\MonoGame\%CONFIGURATION%\SpriteFontPlus.dll" "ZipPackage\MonoGame" /Y
copy "src\SpriteFontPlus\bin\MonoGame\%CONFIGURATION%\SpriteFontPlus.pdb" "ZipPackage\MonoGame" /Y
copy "src\SpriteFontPlus\bin\FNA\%CONFIGURATION%\SpriteFontPlus.dll" "ZipPackage\FNA" /Y
copy "src\SpriteFontPlus\bin\FNA\%CONFIGURATION%\SpriteFontPlus.pdb" "ZipPackage\FNA" /Y