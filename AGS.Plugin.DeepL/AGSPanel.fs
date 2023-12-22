namespace AGS.Plugin.DeepL

open AGS.Types
open System.Windows.Forms
open System.Drawing
open System.Collections.Generic
open DestinationSelection
open TranslationInfo

type AGSPanel () as this =
    inherit EditorContentPanel()

    let event1 = new Event<string>()

    let label = new Label()
    let password = new TextBox()
    let setButton = new Button()

    let srcLang = new Selection()
    let flowPanel = new FlowLayoutPanel()

    let mutable translationsInfo = new List<TranslationInfo>()
    let mutable sourceLanguageInfo = new TranslationInfo()

    do
        label.Location <- Point(10, 10)
        label.Text <- "DeepL api key:"
        password.Location <- Point(120, 10)
        password.PasswordChar <- '*'
        password.Width <- 250
        setButton.Location <- Point (10, 40)
        setButton.Text <- "Set key"
        setButton.Click.Add(fun(args) -> this.OnSetClicked(args))

        srcLang.Location <- new Point (10, 60)

        flowPanel.Location <- new Point(300, 60)
        flowPanel.Width <- 300
        flowPanel.Height <- 500
        flowPanel.FlowDirection <- FlowDirection.TopDown;

        base.SuspendLayout()

        base.Controls.Add label
        base.Controls.Add password
        base.Controls.Add setButton
        base.Controls.Add srcLang
        base.Controls.Add flowPanel

        base.ResumeLayout false

    member this.Event1 = event1.Publish

    member this.OnSetClicked(args) =
        event1.Trigger(password.Text)

    member this.Key with set (value) =
        password.Text <- value;

    member this.SourceLanguage with set (value) =
        sourceLanguageInfo <- value
        srcLang.TranslationInfo <- value
    member this.SourceLanguage with get() =
        sourceLanguageInfo <- srcLang.TranslationInfo
        sourceLanguageInfo


    member this.TranslationsInfo with set (value) =
        translationsInfo <- value
    member this.TranslationsInfo with get() =
        let mutable index = 0
        for control in flowPanel.Controls do
            translationsInfo[index] <- (control :?> Selection).TranslationInfo
            index <- index + 1
            ()

        translationsInfo

    member this.UpdateUi() =
        flowPanel.Controls.Clear()

        for info in this.TranslationsInfo do
            let control = new Selection()
            control.TranslationInfo <- info
            flowPanel.Controls.Add(control)
        flowPanel.PerformLayout()
        base.PerformLayout()
