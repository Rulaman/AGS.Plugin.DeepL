module TranslationInfo

open System.Collections.Generic

type TranslationInfo() =

    let mutable id = ""
    let mutable name = ""
    let mutable langname = ""
    let mutable langcode = ""
    let mutable languagelist = new Dictionary<string, string>()

    /// <summary>
    /// This is the id of the information. It comes from the menu entries (e.g. Trl0, Trl1 Trl...)
    /// </summary>
    member this.Id with get () = id
    member this.Id with set(value) = id <- value

    /// <summary>
    /// This ist the name of the translation file who is set in the tree (e.g. English, ...)
    /// </summary>
    member this.Name with get() = name
    member this.Name with set(value) = name <- value

    /// <summary>
    /// This is the language name return from the deepl api (e.g. German, English, ...)
    /// </summary>
    member this.LanguageName with get() = langname
    member this.LanguageName with set(value) = langname <- value

    /// <summary>
    /// This is the language code returned from the deepl api (e.g. de, en-GB, ...)
    /// </summary>
    member this.LanguageCode with get() = langcode
    member this.LanguageCode with set(value) = langcode <- value

     /// <summary>
    /// These are the available languages. Code and Name from the deepl api
    /// </summary>
    member this.LanguageList with get() = languagelist
    member this.LanguageList with set(value) = languagelist<- value

type ComboBoxItem() =
    let mutable id = ""
    let mutable name = ""

    member this.Id with get () = id
    member this.Id with set(value) = id <- value

    member this.Name with get() = name
    member this.Name with set(value) = name <- value

    with override this.ToString() = this.Name
