# AGS.Plugin.DeepL

The is a plugin for the game development tool [`Adventure Game Studio`](https://www.adventuregamestudio.co.uk)

This plugin adds a root node in the game tree named `DeepL`

In this upcoming window (after double-clicking) you can set your DeepL api key (an DeepL account is needed).

Then set your source language on the left side and the destination language(s) on the right side (if you have any translation files.)

Any untranslated line will now be translated when you right click on the translation file and select `Translate with DeepL` (within the Translations tree node).

If you have not set an key, not set the source language and not set the destination language for your translation file then the command is grayed out.

---

## Hint
This plugin uses the official nuget package from DeepL.

---

## Issues
After extracting the content into your editor directory you have to modify your `AGSEditor.exe.config` with the following lines
```
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="System.Text.Json" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-5.0.0.2" newVersion="5.0.0.2" />
      </dependentAssembly>
    </assemblyBinding>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="System.Runtime.CompilerServices.Unsafe" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-5.0.0.0" newVersion="5.0.0.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
```
(The content has to be between the `<configuration> </configuration>` tags)

---

## Additionals
This plugin will be replaced with an updated version who is able to access multiple translation services. So you have to replace it, when a new version is out. If you are eager to test it, feel free to do so.



---
## Development
### Debugging - How to start?

1.	For debugging: Change the path to your AGSEditor.exe used as the plugin host.
2.	Change the past compile options to copy necessary files to you AGSEditor folder
    used for debugging
    (Build -> Events)
    (Project -> Properties -> launchSettings.json)

### File order

Be aware that unlike C# in F# the declaration of a function have to be before the usage.
So the order of the files and functions are important.
