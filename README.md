# MmImageLoading
## MmImageLoading - Mad Maui Image Loading

Library to load images quickly & easily on Maui. 

This Library enables fast loading and rendering of Animations and Images. This project is based on the the famous [FFImageLoading](https://github.com/luberda-molinet/FFImageLoading), so it is standing on the shoulder of a Giant.

<p align="center">
  <img src="./Resources/mSoSoG.jpeg" height="150">
</p>

## Working at this point 

| OS | iOS | Android | Mac | Windows | Tizen |
|:-----:|:-------------:|:-------------:|:-------------:|:-------------:|:-------------:|
| lib | ✅ |  ✅ | TBD | TBD | TBD |
|nuget | ✅ | ✅ |TBD |TBD |TBD |


## current tested Features:
- Configurable disk and memory caching
- Multiple image views using the same image source (url, - path, resource) will use only one bitmap which is cached in memory (less memory usage)
- Deduplication of similar download/load requests. (If 100 similar requests arrive at same time then one real  loading will be performed while 99 others will wait).
- Error and loading placeholders support
- Images can be automatically downsampled to specified size (less memory usage)
- Fluent API which is inspired by Picasso naming
SVG / WebP / GIF support
- Image loading Fade-In animations support
- Can retry image downloads (RetryCount, RetryDelay)
- Android bitmap optimization. Saves 50% of memory by trying not to use transparency channel when possible.

### Community
- Support:
We are looking for help to cary this on. So please if you can help us with PRs.

## install

via nuget

## enable

add the following line to your csproj file to when you need local gifs to avoid issues with resitizer 

`
	<MauiImage Update="Resources\Images\*.gif" Resize="False" />
`

add the following to your MauiProgram.cs:
(see sample project)
``` 
using de.level28.MmImageLoading;
...

#if (ANDROID || IOS)
             .UseMmImageLoading()
#endif
...

``` 

for how to use see sample code or the orignal [documentation](https://github.com/luberda-molinet/FFImageLoading/wiki). 

