# AGS.Plugin.DeepL

The is a plugin for the game development tool [`Adventure Game Studio`](https://www.adventuregamestudio.co.uk)

---

This plugin adds a root node in the game tree named `DeepL`

In this upcoming window (after double-clicking) you can set your (atm) free DeepL api key (pro key is not yet supported).

Then set your source language on the left side and the destination languages on the right side (if you have any translation files.)

Any untranslated line will now be translated when you right click on the translation file and select `Translate with DeepL`.

If you have not set an key, not set the source language and not set the destination language for your translation file then the command is grayed out.

## Beware
This plugin uses an older nuget package for the access to DeepL. It will be replaced with the official one after a feww issues are clarified.


## Issues
The official DeepL.net nuget package supports also netstandard and there needs a little change in the ags project to support this. On loading the plugin with the official version it gives an version mismatch error.
The only way I found to prevent this is to add an parameter in the plugin calling project.

`<RestoreProjectStyle>PackageReference</RestoreProjectStyle>`

## Hints
This plugin will be replaced with an updated version who is able to access multiple translation services.
So you have to replace it, when a new version is out. If you are eager to test it, feel free to do so.



---
## Debugging - How to start?

1.	For debugging: Change the path to your AGSEditor.exe used as the plugin host.
2.	Change the past compile options to copy necessary files to you AGSEditor folder
	used for debugging
	(Build -> Events) (Project -> Properties -> launchSettings.json)

---
Be ware that unlike C# in F# the declaration of a function have to be before the usage.
So the order of the files and functions are important.
