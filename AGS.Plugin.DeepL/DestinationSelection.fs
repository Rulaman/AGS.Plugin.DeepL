module DestinationSelection

open System.Windows.Forms
open System.Drawing
open TranslationInfo
open System

type Selection() as this =
    inherit UserControl()

    let text = new Label()
    let dropdown = new ComboBox()

    let mutable info = new TranslationInfo()
    let monitor = new Object()

    do
        base.Width <- 300
        base.Height <- 30

        text.Location <- new Point(10, 10)
        text.Width <- 150
        text.Height <- 52

        dropdown.Location <- new Point(170, 10)
        dropdown.Width <- 100
        dropdown.Height <- 52
        let _ = dropdown.TextChanged.Add(fun(evArgs) -> this.DropDownTextChanged(evArgs))

        base.SuspendLayout()

        base.Controls.Add(text)
        base.Controls.Add(dropdown)

        base.ResumeLayout(true)

    member this.DropDownTextChanged(evArgs): unit =
        let mutable code = dropdown.Text

        lock monitor ( fun() ->
            for lang in info.LanguageList do
                if lang.Value = dropdown.Text then
                    code <- lang.Key

            info.LanguageName <- dropdown.Text
            info.LanguageCode <- code
        )
        ()

    member this.TranslationInfo with set (value: TranslationInfo) =
        info <- value
        text.Text <- value.Name

        lock monitor ( fun() ->
            for language in value.LanguageList do
                let comboBoxItem = new ComboBoxItem()
                comboBoxItem.Id <- language.Key
                comboBoxItem.Name <- language.Value
                dropdown.Items.Add(comboBoxItem) |> ignore
        )

        dropdown.SelectedIndex <- dropdown.FindStringExact(value.LanguageName)
        ()

    member this.TranslationInfo with get () =
        info
