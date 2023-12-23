module AGS.Plugin.DeepLT

open System.Collections

type DeepLTranslator(key: string) =

    member private this.translator = new DeepL.Translator(key)

    member this.Translate(text: Generic.IEnumerable<string>, fromLang: string, toLang: string) =
        async {
            let! translatedText = this.translator.TranslateTextAsync(text, fromLang, toLang)  |> Async.AwaitTask
            return translatedText
        }

    member this.GetAvailableLanguages() =
        async {
            let! languages = this.translator.GetTargetLanguagesAsync() |> Async.AwaitTask
            return languages
        }
