# MmImageLoading
## MmImageLoading - Mad Maui Image Loading

Library to load images quickly & easily on Maui. 

This Library enables fast loading and rendering of Animations and Images. This project is based on the the famous [FFImageLoading](https://github.com/luberda-molinet/FFImageLoading), so it is standing on the shoulder of a Giant.

<p align="center">
  <img src="./Resources/mSoSoG.jpeg" height="150">
</p>

## How to pack nuget
- change version in csproj file
- build release 
``` 
dotnet build de.level28.MmImageLoading/de.level28.MmImageLoading.NuGet.csproj -c Release
``` 
- pack the nuget
``` 
dotnet pack de.level28.MmImageLoading/de.level28.MmImageLoading.NuGet.csproj -c Release
``` 
- upload to nuget from ".../MmImageLoading/de.level28.MmImageLoading/de.level28.MmImageLoading/bin/Release/"