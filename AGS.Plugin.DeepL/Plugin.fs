namespace AGS.Plugin.DeepL

open AGS.Types
open System.Drawing
open AGS.Plugin.DeepLT
open System.Collections.Generic
open TranslationInfo
open Common
open System.Collections

type DeepLPlugin (host: IAGSEditor) as this =
    let COMPONENT_ID = "TranslatorSettings"
    let COMPONENT_MENU_COMMAND = "DeepL-MenuCommand"
    let CONTROL_ID_ROOT_NODE = "DeepL-Root"
    let ICON_KEY = "DeepL-Icon"
    let MENU_PREFIX = "DeepL-" // DeepL^!^

    let assembly = System.Reflection.Assembly.GetExecutingAssembly()
    let stream = assembly.GetManifestResourceStream("AGS.Plugin.DeepL.Resources.deepl-logo-blue-16.ico")
    let icon = new Icon(stream)

    let _ = host.GUIController.RegisterIcon(ICON_KEY, icon)
    let _ = host.GUIController.ProjectTree.AddTreeRoot(this, CONTROL_ID_ROOT_NODE, "DeepL", ICON_KEY)
    let _ = host.GUIController.ProjectTree.add_BeforeShowContextMenu(fun(evArgs) -> this.ProjectTree_BeforeShowContextMenu(evArgs))

    let pane = new AGSPanel()
    let document = new ContentDocument(pane, "Translations - DeepL", this, ICON_KEY)

    let userPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)
    let keyPath = System.IO.Path.Combine(userPath, "deepl-api-key")
    let key = if System.IO.File.Exists(keyPath) then System.IO.File.ReadAllText(keyPath) else ""

    let mutable translatorInstance = new DeepLTranslator(key)
    let availableTranslations = new System.Collections.Generic.List<DeepL.Model.TargetLanguage>();

    let _ = pane.Event1.Add(fun(key) -> this.SetKey(key))
    let _ = pane.Key <- key

    member this.SetKey(key) =
        System.IO.File.WriteAllText(keyPath, key)
        translatorInstance <- new DeepLTranslator(key)

    interface IEditorComponent with
        member this.BeforeSaveGame(): unit =
            ()

        member this.CommandClick(controlID: string): unit =
            if CONTROL_ID_ROOT_NODE = controlID then
                host.GUIController.AddOrShowPane(document)
            else if controlID.StartsWith MENU_PREFIX then
                let num = int (controlID.Replace(MENU_PREFIX, ""))
                // do here the magic for the n-th file
                let currentTranslation = host.CurrentGame.Translations[num]

                // read the whole file
                // if the internal structure gets saved, then don't read the file.
                let gamePath = host.CurrentGame.DirectoryPath
                let translationsFile = currentTranslation.FileName
                let outFile = currentTranslation.FileName
                let filePath = System.IO.Path.Combine(gamePath, translationsFile)
                let outPath = System.IO.Path.Combine(gamePath, outFile)
                let lines = System.IO.File.ReadAllLines filePath

                // get the untranslated lines (with index?)
                let headerLines = new System.Collections.Generic.List<string>()
                let untranslated = new System.Collections.Generic.List<string>()
                let alreadyTranslated = new System.Collections.Generic.Dictionary<string, string>()

                let mutable count: int = 0
                let max = lines.Length

                while count < max do
                    let thisLine = if count < max then lines[count] else ""
                    let nextLine = if count < max then lines[count + 1] else ""

                    if thisLine.StartsWith "//" then
                        headerLines.Add thisLine
                    if nextLine = "" then
                        untranslated.Add(thisLine)
                        count <- count + 1
                    else
                        alreadyTranslated.Add(thisLine, nextLine)
                        count <- count + 1
                    if  thisLine = "" then
                        ()
                    count <- count + 1

                let destLang = pane.TranslationsInfo[num].LanguageCode
                let returnedLines = translatorInstance.Translate(untranslated, "de", destLang)  |> Async.RunSynchronously
                let translated = List.ofSeq returnedLines

                let outLines = new System.Collections.Generic.List<string>()


                // if the internal translations could be saved on save game
                //currentTranslation.TranslatedLines["MMM37"] <- ret.Text
                //currentTranslation.Modified <- true

                // put the translations back
                outLines.AddRange(headerLines)

                for entry in alreadyTranslated do
                    outLines.Add(entry.Key)
                    outLines.Add(entry.Value)

                if translated.Length = untranslated.Count then
                    let mutable cnt: int = 0

                    while cnt < translated.Length do
                        outLines.Add(untranslated[cnt])
                        outLines.Add(translated[cnt].Text)
                        cnt <- cnt + 1


                if translated.Length = untranslated.Count then
                    System.IO.File.WriteAllLines(outPath, outLines)
            ()

        member this.ComponentID: string = COMPONENT_ID

        member this.EditorShutdown(): unit =
            ()

        member this.FromXml(node: System.Xml.XmlNode): unit =
            if availableTranslations.Count <= 0 && key <> "" then
                let languages = translatorInstance.GetAvailableLanguages() |> Async.RunSynchronously
                availableTranslations.AddRange(languages)

            let dict = new Dictionary<string, string>()
            for entry in availableTranslations do
                dict.Add(entry.Code, entry.Name)

            match node with
            | NotNull ->
                for innerNode in node.ChildNodes do
                    if innerNode.Name = "SourceLanguage" then
                        let info = new TranslationInfo()
                        info.LanguageName <- innerNode.InnerText
                        info.Name <- "Source language"
                        info.Id <- "SourceLanguage"
                        info.LanguageList <- dict

                        pane.SourceLanguage <- info

                    if innerNode.Name.StartsWith("Trl") then
                        let num = (int) (innerNode.Name.Replace("Trl", ""))
                        let info = new TranslationInfo()
                        info.Id <- innerNode.Name
                        info.Name <- host.CurrentGame.Translations[num].Name
                        info.LanguageName <- innerNode.InnerText
                        info.LanguageList <- dict

                        match innerNode.InnerText with
                        | "" -> ()
                        | NotNull -> info.LanguageCode <-  availableTranslations.Find(fun element -> element.Name = innerNode.InnerText).Code
                        | _ -> ()


                        pane.TranslationsInfo.Add(info)
                pane.UpdateUi()
            | _ -> ()

        member this.GameSettingsChanged(): unit =
            let max =  host.CurrentGame.Translations.Count
            let mutable count = 0

            ()

        member this.GetContextMenu(controlID: string): System.Collections.Generic.IList<MenuCommand> =
            // no context menu for deepl plugin
            null

        member this.PropertyChanged(propertyName: string, oldValue: obj): unit =
            ()

        member this.RefreshDataFromGame(): unit =
            if availableTranslations.Count <= 0 && key <> "" then
                let languages = translatorInstance.GetAvailableLanguages() |> Async.RunSynchronously
                availableTranslations.AddRange(languages)

            let dict = new Dictionary<string, string>()
            for entry in availableTranslations do
                dict.Add(entry.Code, entry.Name)


            let max =  host.CurrentGame.Translations.Count
            let mutable count = 0
            let mutable found = false

            for translation in host.CurrentGame.Translations do
                let mutable info = pane.TranslationsInfo.Find(fun (element) -> element.LanguageName = translation.Name)

                match info with
                | NotNull -> found <- true
                | _ -> info <- new TranslationInfo()

                info.Id <- "Trl" + count.ToString()
                info.Name <- translation.Name
                info.LanguageList <- dict

                count <- count + 1

                if not found then
                    pane.TranslationsInfo.Add(info)
                    pane.UpdateUi()

            if pane.SourceLanguage.Id = "" then
                let info = new TranslationInfo()
                info.Name <- "Source language"
                info.Id <- "SourceLanguage"
                info.LanguageList <- dict

                pane.SourceLanguage <- info


        member this.ToXml(writer: System.Xml.XmlTextWriter): unit =
            writer.WriteStartElement("SourceLanguage")
            writer.WriteString(pane.SourceLanguage.LanguageName)
            writer.WriteEndElement()

            for translation in pane.TranslationsInfo do
                // for each language
                writer.WriteStartElement(translation.Id)
                writer.WriteString(translation.LanguageName)
                writer.WriteEndElement()
            ()


    member this.LocalHost = host

    member this.ProjectTree_BeforeShowContextMenu(evArgs: BeforeShowContextMenuEventArgs) =
        let nodeId = evArgs.SelectedNodeID;
        if nodeId.StartsWith "Trl" then
            let numString = nodeId.Replace("Trl", "")
            let menu = host.GUIController.CreateMenuCommand(this, MENU_PREFIX + numString, "Translate with DeepL")

            let dstLangSet = if pane.TranslationsInfo[(int)numString].LanguageName <> "" then true else false
            let srcLangSet = if pane.SourceLanguage.LanguageName <> "" then true else false
            let keyIsSet = if key <> "" then true else false

            menu.Enabled <- dstLangSet && srcLangSet && keyIsSet
            evArgs.MenuCommands.Commands.Add(menu)
        ()

